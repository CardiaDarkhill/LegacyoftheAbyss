#nullable enable

using System;
using UnityEngine;

namespace LegacyoftheAbyss.Shade
{
    /// <summary>
    /// Pixel work behind the skin-preview anti-aliasing and the optional global sprite filtering.
    /// <para>
    /// The Shade's sheets are 180x180 frames whose edges are *already* anti-aliased in the source
    /// art (about 4% of a frame is partial alpha, with edges ramping over two to three pixels).
    /// That matters: the pixelation in the skin selector is not baked into the asset, it is the
    /// preview being point-magnified roughly 5x to fill a ~900px column. So the job is to preserve
    /// the gradients the art already has, not to invent new ones - a premultiplied Catmull-Rom
    /// resample straight to the preview texture size, with bilinear filtering left to handle
    /// whatever magnification remains.
    /// </para>
    /// <para>
    /// An earlier version point-upscaled by a whole number and then blurred the result. That
    /// destroyed the source anti-aliasing and only partially rebuilt it: smooth, but visibly softer
    /// than the art supports. Do not reintroduce it - resampling directly is both cheaper and
    /// sharper.
    /// </para>
    /// <para>
    /// All operations work on <see cref="Color32"/> spans rather than <c>Texture2D</c> so they can
    /// be tested without a Unity runtime. Row order is whatever the caller passes in (Unity's
    /// <c>GetPixels32</c> is bottom-up) - nothing here depends on it.
    /// </para>
    /// </summary>
    internal static class ShadeSpriteSmoothing
    {
        /// <summary>
        /// Pixel size the preview texture is resampled to. The skin selector draws at up to 900px,
        /// so this leaves under 2x of ordinary bilinear stretch on top; compared against the real
        /// idle sheet, larger textures are near-indistinguishable and cost memory per cached skin.
        /// </summary>
        internal const int PreviewTargetSize = 512;

        /// <summary>Transparent gutter, in source pixels, placed around each frame of a padded strip.</summary>
        internal const int StripPadding = 2;

        /// <summary>
        /// Size to resample a <paramref name="frameSize"/>-pixel square frame to. Returns
        /// <paramref name="frameSize"/> unchanged when the art is already at or above the target,
        /// which lets the caller skip the resample entirely.
        /// </summary>
        internal static int ChoosePreviewSize(int frameSize, int targetSize = PreviewTargetSize)
        {
            if (frameSize <= 0 || targetSize <= frameSize)
            {
                return Mathf.Max(0, frameSize);
            }

            return targetSize;
        }

        /// <summary>
        /// Resamples a square frame to <paramref name="targetSize"/>. Returns the source array
        /// untouched when no resize is needed or the input is degenerate.
        /// </summary>
        internal static Color32[] Antialias(Color32[] source, int width, int height, int targetSize, out int resultWidth, out int resultHeight)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            resultWidth = width;
            resultHeight = height;
            if (width <= 0 || height <= 0 || source.Length < width * height)
            {
                return source;
            }

            if (targetSize <= 0 || (targetSize == width && targetSize == height))
            {
                return source;
            }

            resultWidth = targetSize;
            resultHeight = targetSize;
            return Resample(source, width, height, targetSize, targetSize);
        }

        /// <summary>
        /// Separable Catmull-Rom resample, run in premultiplied alpha.
        /// <para>
        /// Premultiplying matters: interpolating straight RGBA drags the (arbitrary) colour of
        /// fully transparent pixels into the silhouette edge, which on near-black Shade art shows
        /// up as a pale fringe. Catmull-Rom rather than bilinear because it keeps the edge ramp
        /// narrow - a wide ramp is exactly what made the previous pass look soft.
        /// </para>
        /// </summary>
        internal static Color32[] Resample(Color32[] source, int sourceWidth, int sourceHeight, int targetWidth, int targetHeight)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (sourceWidth <= 0 || sourceHeight <= 0 || targetWidth <= 0 || targetHeight <= 0)
            {
                return source;
            }

            int sourceCount = sourceWidth * sourceHeight;
            var r = new float[sourceCount];
            var g = new float[sourceCount];
            var b = new float[sourceCount];
            var a = new float[sourceCount];
            for (int i = 0; i < sourceCount; i++)
            {
                var pixel = source[i];
                float alpha = pixel.a / 255f;
                r[i] = pixel.r * alpha;
                g[i] = pixel.g * alpha;
                b[i] = pixel.b * alpha;
                a[i] = pixel.a;
            }

            // Horizontal pass into an intermediate of the target width and the source height.
            int midCount = targetWidth * sourceHeight;
            var midR = new float[midCount];
            var midG = new float[midCount];
            var midB = new float[midCount];
            var midA = new float[midCount];
            ResampleAxis(r, g, b, a, sourceWidth, sourceHeight, midR, midG, midB, midA, targetWidth, horizontal: true);

            // Vertical pass into the final size.
            int outCount = targetWidth * targetHeight;
            var outR = new float[outCount];
            var outG = new float[outCount];
            var outB = new float[outCount];
            var outA = new float[outCount];
            ResampleAxis(midR, midG, midB, midA, targetWidth, sourceHeight, outR, outG, outB, outA, targetHeight, horizontal: false);

            var result = new Color32[outCount];
            for (int i = 0; i < outCount; i++)
            {
                // Catmull-Rom overshoots slightly at high-contrast edges. Clamping is what turns
                // that into a crisper edge rather than a ringing halo.
                float alpha = Mathf.Clamp(outA[i], 0f, 255f);
                byte resultAlpha = (byte)Mathf.RoundToInt(alpha);
                if (resultAlpha == 0)
                {
                    result[i] = new Color32(0, 0, 0, 0);
                    continue;
                }

                float inverse = 255f / alpha;
                result[i] = new Color32(
                    (byte)Mathf.Clamp(Mathf.RoundToInt(outR[i] * inverse), 0, 255),
                    (byte)Mathf.Clamp(Mathf.RoundToInt(outG[i] * inverse), 0, 255),
                    (byte)Mathf.Clamp(Mathf.RoundToInt(outB[i] * inverse), 0, 255),
                    resultAlpha);
            }

            return result;
        }

        /// <summary>
        /// Rebuilds a horizontal frame strip with a transparent gutter around every frame, so the
        /// sheet can be sampled bilinearly without neighbouring frames bleeding across the seam.
        /// </summary>
        internal static Color32[] PadStrip(Color32[] source, int width, int height, int columns, int padding, out int resultWidth, out int resultHeight)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            columns = Mathf.Max(1, columns);
            padding = Mathf.Max(0, padding);
            int frameWidth = width / columns;
            int paddedFrameWidth = frameWidth + padding * 2;
            resultWidth = paddedFrameWidth * columns;
            resultHeight = height + padding * 2;
            var result = new Color32[resultWidth * resultHeight];
            for (int column = 0; column < columns; column++)
            {
                int sourceX = column * frameWidth;
                int targetX = column * paddedFrameWidth + padding;
                for (int y = 0; y < height; y++)
                {
                    int sourceRow = y * width + sourceX;
                    int targetRow = (y + padding) * resultWidth + targetX;
                    Array.Copy(source, sourceRow, result, targetRow, frameWidth);
                }
            }

            return result;
        }

        /// <summary>
        /// One axis of the separable resample. <paramref name="horizontal"/> resizes rows (reading
        /// with stride <paramref name="width"/>, writing with stride <paramref name="targetLength"/>);
        /// otherwise it resizes columns and the stride is <paramref name="width"/> throughout.
        /// </summary>
        private static void ResampleAxis(
            float[] r, float[] g, float[] b, float[] a,
            int width, int height,
            float[] outR, float[] outG, float[] outB, float[] outA,
            int targetLength, bool horizontal)
        {
            int sourceLength = horizontal ? width : height;
            int otherLength = horizontal ? height : width;
            int outStride = horizontal ? targetLength : width;
            float scale = (float)sourceLength / targetLength;

            // Tap positions and weights depend only on the output index, so they are built once per
            // axis rather than recomputed for every pixel.
            var taps = new int[targetLength * 4];
            var weights = new float[targetLength * 4];
            for (int d = 0; d < targetLength; d++)
            {
                float center = (d + 0.5f) * scale - 0.5f;
                int baseIndex = Mathf.FloorToInt(center);
                float total = 0f;
                for (int k = 0; k < 4; k++)
                {
                    int sample = baseIndex - 1 + k;
                    float weight = CatmullRom(center - sample);
                    taps[d * 4 + k] = Mathf.Clamp(sample, 0, sourceLength - 1);
                    weights[d * 4 + k] = weight;
                    total += weight;
                }

                if (total != 0f)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        weights[d * 4 + k] /= total;
                    }
                }
            }

            for (int o = 0; o < otherLength; o++)
            {
                for (int d = 0; d < targetLength; d++)
                {
                    float sumR = 0f, sumG = 0f, sumB = 0f, sumA = 0f;
                    for (int k = 0; k < 4; k++)
                    {
                        int sample = taps[d * 4 + k];
                        float weight = weights[d * 4 + k];
                        int index = horizontal ? o * width + sample : sample * width + o;
                        sumR += r[index] * weight;
                        sumG += g[index] * weight;
                        sumB += b[index] * weight;
                        sumA += a[index] * weight;
                    }

                    int target = horizontal ? o * outStride + d : d * outStride + o;
                    outR[target] = sumR;
                    outG[target] = sumG;
                    outB[target] = sumB;
                    outA[target] = sumA;
                }
            }
        }

        /// <summary>Catmull-Rom cubic (the B=0, C=0.5 member of the Mitchell-Netravali family).</summary>
        private static float CatmullRom(float x)
        {
            x = Mathf.Abs(x);
            if (x < 1f)
            {
                return 1.5f * x * x * x - 2.5f * x * x + 1f;
            }

            if (x < 2f)
            {
                return -0.5f * x * x * x + 2.5f * x * x - 4f * x + 2f;
            }

            return 0f;
        }
    }
}
