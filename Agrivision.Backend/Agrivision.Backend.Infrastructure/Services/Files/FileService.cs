using Agrivision.Backend.Application.Services.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using OpenCvSharp;

namespace Agrivision.Backend.Infrastructure.Services.Files;

public class FileService(IWebHostEnvironment environment, ILogger<FileService> logger) : IFileService
{
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var allowedExtensions = new List<string>{
            ".jpg", ".jpeg", ".png"
        };

        if (file == null || file.Length == 0)
        {
            logger.LogWarning("Image file is empty or null");
            return string.Empty;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            logger.LogWarning("Unsupported image file extension: {Extension}", ext);
            return string.Empty;
        }

        if (!await IsValidImageSignatureAsync(file))
        {
            logger.LogWarning("Image file signature does not match known formats");
            return string.Empty;
        }

        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueName = Guid.NewGuid() + ext;
        var filePath = Path.Combine(uploadsFolder, uniqueName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return uniqueName;
    }

    public async Task<string> UploadVideoAsync(IFormFile file)
    {
        var allowedExtensions = new List<string>{
            ".mp4"
        };

        if (file == null || file.Length == 0)
        {
            logger.LogWarning("Video file is empty or null");
            return string.Empty;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            logger.LogWarning("Unsupported video file extension: {Extension}", ext);
            return string.Empty;
        }

        if (!await IsValidVideoSignatureAsync(file))
        {
            logger.LogWarning("Video file signature does not match known formats");
            return string.Empty;
        }

        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueName = Guid.NewGuid() + ext;
        var filePath = Path.Combine(uploadsFolder, uniqueName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return uniqueName;
    }

    public async Task<List<string>> ExtractVideoFramesAsync(string videoFilename, int intervalSeconds = 10)
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

    public async Task<string> CreateCompositeImageAsync(List<(string imagePath, string label, double confidence)> annotatedFrames, string outputFilename)
    {
        try
        {
            var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
            var resultsFolder = Path.Combine(environment.WebRootPath, "results");
            Directory.CreateDirectory(resultsFolder);
            
            var outputPath = Path.Combine(resultsFolder, outputFilename);
            
            if (!annotatedFrames.Any())
            {
                logger.LogWarning("No annotated frames provided for composite image: {OutputFilename}", outputFilename);
                return string.Empty;
            }

            // layout configuration
            const int frameWidth = 400;
            const int frameHeight = 400;
            const int textHeight = 60;
            const int padding = 20;
            const int itemHeight = frameHeight + textHeight + padding;
            const int itemWidth = frameWidth + padding;
            const int columns = 3;
            
            var rows = (int)Math.Ceiling((double)annotatedFrames.Count / columns);
            var canvasWidth = columns * itemWidth;
            var canvasHeight = rows * itemHeight + padding;
            
            // create canvas
            var info = new SKImageInfo(canvasWidth, canvasHeight);
            using var surface = SKSurface.Create(info);
            using var canvas = surface.Canvas;
            
            canvas.Clear(SKColors.White);
            
            // setup text styling
            using var textPaint = new SKPaint();
            textPaint.Color = SKColors.Black;
            textPaint.IsAntialias = true;

            using var titlePaint = new SKPaint();
            titlePaint.Color = SKColors.DarkBlue;
            titlePaint.IsAntialias = true;

            using var textFont = new SKFont(SKTypeface.FromFamilyName("Arial"), 14);
            using var titleFont = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold), 16);
            
            // validate font creation
            if (textFont.Typeface == null || titleFont.Typeface == null)
            {
                logger.LogWarning("Failed to create fonts, using default");
                textFont.Typeface = SKTypeface.Default;
                titleFont.Typeface = SKTypeface.Default;
            }
            
            // draw title
            var title = "Disease Detection Results";
            var titleWidth = titleFont.MeasureText(title);
            canvas.DrawText(title, (canvasWidth - titleWidth) / 2, 30, SKTextAlign.Left, titleFont, titlePaint);
            
            // draw frames in grid
            for (int i = 0; i < annotatedFrames.Count; i++)
            {
                var frame = annotatedFrames[i];
                var row = i / columns;
                var col = i % columns;
                
                var x = col * itemWidth + padding / 2;
                var y = row * itemHeight + padding / 2 + 50;
                
                // load and draw frame image
                var fullImagePath = Path.Combine(uploadsFolder, frame.imagePath);
                if (File.Exists(fullImagePath))
                {
                    try
                    {
                        using var bitmap = SKBitmap.Decode(fullImagePath);
                        if (bitmap != null)
                        {
                            var destRect = new SKRect(x, y, x + frameWidth, y + frameHeight);
                            canvas.DrawBitmap(bitmap, destRect);
                            
                            // draw border around image
                            using var borderPaint = new SKPaint();
                            borderPaint.Color = SKColors.Gray;
                            borderPaint.Style = SKPaintStyle.Stroke;
                            borderPaint.StrokeWidth = 2;
                            canvas.DrawRect(destRect, borderPaint);
                        }
                        else
                        {
                            logger.LogWarning("Failed to decode bitmap: {ImagePath}", fullImagePath);
                        }
                    }
                    catch (Exception bitmapEx)
                    {
                        logger.LogWarning(bitmapEx, "Failed to load bitmap: {ImagePath}", fullImagePath);
                    }
                }
                else
                {
                    logger.LogWarning("Frame image not found: {ImagePath}", fullImagePath);
                }
                
                // draw label and confidence text
                var labelText = frame.label ?? "Unknown";
                var confidenceText = $"Confidence: {frame.confidence:P1}";
                
                var labelY = y + frameHeight + 20;
                var confidenceY = labelY + 20;
                
                canvas.DrawText(labelText, x, labelY, SKTextAlign.Left, titleFont, titlePaint);
                canvas.DrawText(confidenceText, x, confidenceY, SKTextAlign.Left, textFont, textPaint);
            }
            
            // save composite image
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 95);
            await File.WriteAllBytesAsync(outputPath, data.ToArray());
            
            logger.LogInformation("Successfully created composite image: {OutputFilename}", outputFilename);
            return outputFilename;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create composite image: {OutputFilename}", outputFilename);
            return string.Empty;
        }
    }

    private async Task<bool> IsValidImageSignatureAsync(IFormFile file)
    {
        byte[] buffer = new byte[8];
        await using (var stream = file.OpenReadStream())
        {
            await stream.ReadExactlyAsync(buffer, 0, buffer.Length);
        }

        // JPEG
        if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            return true;

        // PNG
        if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E &&
            buffer[3] == 0x47 && buffer[4] == 0x0D && buffer[5] == 0x0A &&
            buffer[6] == 0x1A && buffer[7] == 0x0A)
            return true;

        return false;
    }

    private async Task<bool> IsValidVideoSignatureAsync(IFormFile file)
    {
        byte[] buffer = new byte[12];
        await using (var stream = file.OpenReadStream())
        {
            await stream.ReadExactlyAsync(buffer, 0, buffer.Length);
        }

        // MP4
        if (buffer[4] == 0x66 && buffer[5] == 0x74 && buffer[6] == 0x79 && buffer[7] == 0x70)
        {
            return true;
        }

        return false;
    }
}