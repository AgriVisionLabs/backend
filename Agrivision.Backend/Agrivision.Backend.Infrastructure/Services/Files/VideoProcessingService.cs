using Agrivision.Backend.Application.Services.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using OpenCvSharp;

namespace Agrivision.Backend.Infrastructure.Services.Files;

public class VideoProcessingService(IWebHostEnvironment environment, ILogger<VideoProcessingService> logger) : IVideoProcessingService
{
    public Task<List<string>> ExtractVideoFramesAsync(string videoFilename, int intervalSeconds = 10)
    {
        return Task.Run(() => ExtractVideoFrames(videoFilename, intervalSeconds));
    }

    // Internal helper, not part of interface
    private List<string> ExtractVideoFrames(string videoFilename, int intervalSeconds = 10)
    {
        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
        var framesFolder = Path.Combine(uploadsFolder, "frames");
        Directory.CreateDirectory(framesFolder);

        var videoPath = Path.Combine(uploadsFolder, videoFilename);
        
        if (!File.Exists(videoPath))
        {
            logger.LogWarning("Video file not found: {VideoFilename}", videoFilename);
            return new List<string>();
        }

        var extractedFrames = new List<string>();
        VideoCapture? capture = null;
        
        try
        {
            // Add timeout for video processing (5 minutes max)
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            
            capture = new VideoCapture(videoPath);
            
            if (!capture.IsOpened())
            {
                logger.LogWarning("Could not open video file: {VideoFilename}", videoFilename);
                return new List<string>();
            }

            // get video information with validation
            var totalFrames = (int)capture.Get(VideoCaptureProperties.FrameCount);
            var fps = capture.Get(VideoCaptureProperties.Fps);
            
            if (totalFrames <= 0 || fps <= 0)
            {
                logger.LogWarning("Invalid video properties: frames={TotalFrames}, fps={Fps}", totalFrames, fps);
                return new List<string>();
            }
            
            var duration = totalFrames / fps;
            
            logger.LogInformation("Video properties - Duration: {Duration:F2}s, FPS: {Fps:F2}, Total frames: {TotalFrames}", 
                duration, fps, totalFrames);
            
            // validate video duration
            if (duration < 1)
            {
                logger.LogWarning("Video too short: {Duration} seconds", duration);
                return new List<string>();
            }
            
            if (duration > 600) // 10 minutes max
            {
                logger.LogWarning("Video too long: {Duration} seconds (max 600s)", duration);
                return new List<string>();
            }
            
            // calculate frame extraction strategy
            var frameCount = 0;
            var maxFrames = 20;
            var minFrameInterval = Math.Max(1, (int)(fps * intervalSeconds));
            var maxFrameInterval = Math.Max(1, totalFrames / maxFrames);
            var frameInterval = Math.Max(minFrameInterval, maxFrameInterval);
            
            logger.LogInformation("Frame extraction strategy - Interval: {Interval} frames, Max frames: {MaxFrames}", 
                frameInterval, maxFrames);
            
            using var frame = new Mat();
            var attempts = 0;
            var maxAttempts = totalFrames + 10; // prevent infinite loops
            
            for (int frameIndex = 0; frameIndex < totalFrames && frameCount < maxFrames && attempts < maxAttempts; frameIndex += frameInterval)
            {
                attempts++;
                
                try
                {
                    // check for cancellation
                    cts.Token.ThrowIfCancellationRequested();
                    
                    // seek to specific frame
                    if (!capture.Set(VideoCaptureProperties.PosFrames, frameIndex))
                    {
                        logger.LogWarning("Failed to seek to frame index: {FrameIndex}", frameIndex);
                        continue;
                    }
                    
                    // read frame with validation
                    if (!capture.Read(frame) || frame.Empty())
                    {
                        logger.LogWarning("Failed to read frame at index: {FrameIndex}", frameIndex);
                        continue;
                    }

                    // validate frame dimensions
                    var width = frame.Width;
                    var height = frame.Height;
                    
                    if (width <= 0 || height <= 0)
                    {
                        logger.LogWarning("Invalid frame dimensions: {Width}x{Height} at index {FrameIndex}", 
                            width, height, frameIndex);
                        continue;
                    }
                    
                    // resize frame to max 1024 width while maintaining aspect ratio
                    Mat frameToSave = frame;
                    Mat? resizedFrame = null;
                    
                    if (width > 1024)
                    {
                        var newHeight = (int)((1024.0 / width) * height);
                        resizedFrame = new Mat();
                        Cv2.Resize(frame, resizedFrame, new OpenCvSharp.Size(1024, newHeight));
                        frameToSave = resizedFrame;
                    }

                    // save frame as JPEG with quality control
                    var frameFilename = $"{Path.GetFileNameWithoutExtension(videoFilename)}_frame_{frameCount:D3}_{Guid.NewGuid():N}.jpg";
                    var frameOutputPath = Path.Combine(framesFolder, frameFilename);
                    
                    var saveParams = new int[] { (int)ImwriteFlags.JpegQuality, 95 };
                    
                    if (Cv2.ImWrite(frameOutputPath, frameToSave, saveParams))
                    {
                        // validate saved file
                        var fileInfo = new FileInfo(frameOutputPath);
                        if (fileInfo.Exists && fileInfo.Length > 0)
                        {
                            extractedFrames.Add(frameFilename);
                            frameCount++;
                            logger.LogInformation("Extracted frame {FrameNumber}/{MaxFrames}: {Filename} ({FileSize} bytes)", 
                                frameCount, maxFrames, frameFilename, fileInfo.Length);
                        }
                        else
                        {
                            logger.LogWarning("Frame save validation failed for index: {FrameIndex}", frameIndex);
                        }
                    }
                    else
                    {
                        logger.LogWarning("Frame save failed for index: {FrameIndex}", frameIndex);
                    }
                    
                    // dispose resized frame if created
                    resizedFrame?.Dispose();
                }
                catch (OperationCanceledException)
                {
                    logger.LogWarning("Frame extraction cancelled due to timeout for video: {VideoFilename}", videoFilename);
                    break;
                }
                catch (Exception frameEx)
                {
                    logger.LogWarning(frameEx, "Failed to extract frame at index: {FrameIndex}", frameIndex);
                    // continue with next frame
                }
            }
            
            if (extractedFrames.Count == 0)
            {
                logger.LogWarning("No frames could be extracted from the video: {VideoFilename}", videoFilename);
                return new List<string>();
            }
            
            logger.LogInformation("Successfully extracted {FrameCount} frames from video {VideoFilename} (processed {Attempts} attempts)", 
                frameCount, videoFilename, attempts);
            return extractedFrames;
        }
        catch (OperationCanceledException)
        {
            logger.LogError("Video processing timeout (5 minutes) for video: {VideoFilename}", videoFilename);
            return new List<string>();
        }
        catch (Exception ex)
        {
            // enhanced error classification
            if (ex.Message.Contains("OpenCV") || ex.Message.Contains("cv2") || ex.Message.Contains("opencv"))
            {
                logger.LogError("OpenCV error occurred during video processing. Video: {VideoFilename}, Error: {Error}", 
                    videoFilename, ex.Message);
            }
            else if (ex.Message.Contains("codec") || ex.Message.Contains("format"))
            {
                logger.LogError("Video format/codec error for video: {VideoFilename}, Error: {Error}", 
                    videoFilename, ex.Message);
            }
            else
            {
                logger.LogError(ex, "Unexpected error during frame extraction for video: {VideoFilename}", videoFilename);
            }
            return new List<string>();
        }
        finally
        {
            // ensure proper cleanup
            capture?.Dispose();
            
            // cleanup any partial frames on error
            if (extractedFrames.Count == 0)
            {
                try
                {
                    var tempFiles = Directory.GetFiles(framesFolder, $"{Path.GetFileNameWithoutExtension(videoFilename)}_frame_*");
                    foreach (var tempFile in tempFiles)
                    {
                        File.Delete(tempFile);
                    }
                }
                catch (Exception cleanupEx)
                {
                    logger.LogWarning(cleanupEx, "Failed to cleanup partial frames for video: {VideoFilename}", videoFilename);
                }
            }
        }
    }
} 