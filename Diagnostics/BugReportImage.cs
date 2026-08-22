using System;
using UnityEngine;

namespace LegacyoftheAbyss.Diagnostics
{
    /// <summary>
    /// Screenshot resampling, kept as plain array maths so it can be reasoned about and tested without
    /// a Unity player loop.
    /// </summary>
    internal static class BugReportImage
    {
        /// <summary>Height that preserves the aspect ratio at <paramref name="targetWidth"/>.</summary>
        internal static int ScaledHeight(int sourceWidth, int sourceHeight, int targetWidth)
        {
            if (sourceWidth <= 0 || sourceHeight <= 0 || targetWidth <= 0)
            {
                return 1;
            }

            int scaled = (int)Math.Round(sourceHeight * (targetWidth / (double)sourceWidth), MidpointRounding.AwayFromZero);
            return scaled < 1 ? 1 : scaled;
        }

        /// <summary>
        /// Box filter: each destination pixel is the mean of the source block it covers.
        /// <para>
        /// Point sampling every Nth pixel would be far cheaper, but it aliases badly, and the detail it
        /// destroys first is thin high-contrast edges - which is precisely what a report about the
        /// Shade's outline, sorting layer or anti-aliasing needs to show. A box filter over the whole
        /// block keeps a one-pixel feature visible as a faint one instead of dropping it entirely.
        /// </para>
        /// <para>
        /// Block bounds are computed by integer scaling per destination pixel, so the blocks tile the
        /// source exactly with no gaps or overlaps even when the ratio is not a whole number.
        /// </para>
        /// </summary>
        internal static Color32[] BoxDownscale(Color32[] source, int sourceWidth, int sourceHeight, int targetWidth, int targetHeight)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sourceWidth <= 0 || sourceHeight <= 0 || targetWidth <= 0 || targetHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetWidth), "Image dimensions must be positive.");
            }

            if (source.Length < sourceWidth * sourceHeight)
            {
                throw new ArgumentException("Pixel buffer is smaller than the stated dimensions.", nameof(source));
            }

            var result = new Color32[targetWidth * targetHeight];

            for (int destinationY = 0; destinationY < targetHeight; destinationY++)
            {
                int sourceY0 = (int)((long)destinationY * sourceHeight / targetHeight);
                int sourceY1 = (int)((long)(destinationY + 1) * sourceHeight / targetHeight);
                if (sourceY1 <= sourceY0)
                {
                    sourceY1 = sourceY0 + 1;
                }

                if (sourceY1 > sourceHeight)
                {
                    sourceY1 = sourceHeight;
                }

                int destinationRow = destinationY * targetWidth;

                for (int destinationX = 0; destinationX < targetWidth; destinationX++)
                {
                    int sourceX0 = (int)((long)destinationX * sourceWidth / targetWidth);
                    int sourceX1 = (int)((long)(destinationX + 1) * sourceWidth / targetWidth);
                    if (sourceX1 <= sourceX0)
                    {
                        sourceX1 = sourceX0 + 1;
                    }

                    if (sourceX1 > sourceWidth)
                    {
                        sourceX1 = sourceWidth;
                    }

                    int red = 0;
                    int green = 0;
                    int blue = 0;
                    int alpha = 0;
                    int samples = 0;

                    for (int y = sourceY0; y < sourceY1; y++)
                    {
                        int row = y * sourceWidth;
                        for (int x = sourceX0; x < sourceX1; x++)
                        {
                            var pixel = source[row + x];
                            red += pixel.r;
                            green += pixel.g;
                            blue += pixel.b;
                            alpha += pixel.a;
                            samples++;
                        }
                    }

                    if (samples == 0)
                    {
                        continue;
                    }

                    result[destinationRow + destinationX] = new Color32(
                        (byte)(red / samples),
                        (byte)(green / samples),
                        (byte)(blue / samples),
                        (byte)(alpha / samples));
                }
            }

            return result;
        }
    }
}
