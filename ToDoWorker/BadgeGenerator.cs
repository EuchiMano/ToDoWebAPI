using SkiaSharp;

namespace ToDoWorker
{
    public class BadgeGenerator
    {
        public string GenerateBadge(string badgeName, SKColor circleColor)
        {
            string outputPath = $"badge_{DateTime.Now:yyyyMMddHHmmss}.png";

            using (var bitmap = CreateBadgeBitmap(circleColor, badgeName))
            {
                SaveBitmapToFile(bitmap, outputPath);
            }

            return outputPath;
        }

        private void SaveBitmapToFile(SKBitmap bitmap, string outputPath)
        {
            using (var stream = new SKFileWStream(outputPath))
            {
                bitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            }
        }

        private SKBitmap CreateBadgeBitmap(SKColor circleColor, string badgeName)
        {
            var bitmap = new SKBitmap(400, 120);

            using (var canvas = new SKCanvas(bitmap))
            {
                // Change the background color to not so dark black
                canvas.Clear(new SKColor(30, 30, 30));

                // Draw a filled circle with the specified color (reduced size)
                using (var circlePaint = new SKPaint())
                {
                    circlePaint.Color = circleColor;
                    circlePaint.IsAntialias = true; // Enable anti-aliasing
                    canvas.DrawCircle(50, 60, 25, circlePaint); // Reduced size and corrected coordinates
                }

                // Create an SKPaint for drawing text
                using (var textPaint = new SKPaint())
                {
                    // Change the text color to white
                    textPaint.Color = SKColors.White;
                    textPaint.TextSize = 36;
                    textPaint.TextAlign = SKTextAlign.Center;

                    // Draw badge name centered next to the circle
                    canvas.DrawText(badgeName, 240, 60 + textPaint.TextSize / 2, textPaint);
                }
            }

            return bitmap;
        }
    }
}