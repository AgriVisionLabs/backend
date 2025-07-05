using Agrivision.Backend.Application.Services.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace Agrivision.Backend.Infrastructure.Services.Files;

public class ImageProcessingService(IWebHostEnvironment environment, ILogger<ImageProcessingService> logger) : IImageProcessingService
{
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
} 