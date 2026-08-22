using LegacyoftheAbyss.Shade;
using UnityEngine;
using Xunit;

/// <summary>
/// Pixel-level checks on the skin-preview resample and the frame gutters that make the optional
/// global bilinear filtering safe on a packed sprite strip.
/// </summary>
public class ShadeSpriteSmoothingTests
{
    private static Color32[] Fill(int width, int height, Color32 color)
    {
        var pixels = new Color32[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        return pixels;
    }

    /// <summary>A left-opaque / right-transparent split, i.e. one hard vertical alpha edge.</summary>
    private static Color32[] HalfOpaque(int size, Color32 solid, Color32 empty)
    {
        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                pixels[y * size + x] = x < size / 2 ? solid : empty;
            }
        }

        return pixels;
    }

    [Fact]
    public void PreviewSizeResamplesUpTowardsTheTarget()
    {
        Assert.Equal(512, ShadeSpriteSmoothing.ChoosePreviewSize(180, 512));
        Assert.Equal(512, ShadeSpriteSmoothing.ChoosePreviewSize(64, 512));
    }

    [Fact]
    public void PreviewSizeLeavesArtThatIsAlreadyLargeEnoughAlone()
    {
        // Downscaling a sheet that already exceeds the preview size would throw away detail the
        // GPU can show, so the caller is told to skip the resample.
        Assert.Equal(512, ShadeSpriteSmoothing.ChoosePreviewSize(512, 512));
        Assert.Equal(900, ShadeSpriteSmoothing.ChoosePreviewSize(900, 512));
    }

    [Fact]
    public void PreviewSizeIsSafeForDegenerateInput()
    {
        Assert.Equal(0, ShadeSpriteSmoothing.ChoosePreviewSize(0, 512));
        Assert.Equal(0, ShadeSpriteSmoothing.ChoosePreviewSize(-8, 512));
    }

    [Fact]
    public void ResampleProducesTheRequestedSize()
    {
        var source = Fill(8, 8, new Color32(0, 0, 0, 255));
        var result = ShadeSpriteSmoothing.Resample(source, 8, 8, 32, 32);
        Assert.Equal(32 * 32, result.Length);
    }

    [Fact]
    public void ResampleLeavesTheInteriorOfASolidRegionUntouched()
    {
        var source = Fill(9, 9, new Color32(20, 20, 30, 255));
        var result = ShadeSpriteSmoothing.Resample(source, 9, 9, 27, 27);

        var center = result[13 * 27 + 13];
        Assert.Equal(255, center.a);
        Assert.Equal(20, center.r);
        Assert.Equal(20, center.g);
        Assert.Equal(30, center.b);
    }

    [Fact]
    public void ResamplePreservesAGradedAlphaEdgeInsteadOfSteppingIt()
    {
        // The real source art already anti-aliases its edges, so what matters is that magnifying
        // it produces a monotonic ramp rather than the flat plateaus a point upscale leaves behind.
        const int size = 8;
        var source = HalfOpaque(size, new Color32(10, 10, 10, 255), new Color32(0, 0, 0, 0));
        var result = ShadeSpriteSmoothing.Resample(source, size, size, size * 4, size * 4);

        int width = size * 4;
        int row = width / 2;
        int partial = 0;
        for (int x = 0; x < width; x++)
        {
            byte alpha = result[row * width + x].a;
            if (alpha > 0 && alpha < 255)
            {
                partial++;
            }
        }

        // A point upscale would give zero partially transparent pixels across this edge.
        Assert.True(partial >= 2, $"expected a graded edge, got {partial} partial pixels");
    }

    [Fact]
    public void ResampleDoesNotDragTransparentPixelColourIntoTheEdge()
    {
        // A white-but-transparent background next to black art is the case that produces a pale
        // fringe when interpolation runs on straight (non-premultiplied) RGBA.
        const int size = 8;
        var source = HalfOpaque(size, new Color32(0, 0, 0, 255), new Color32(255, 255, 255, 0));
        var result = ShadeSpriteSmoothing.Resample(source, size, size, size * 4, size * 4);

        for (int i = 0; i < result.Length; i++)
        {
            if (result[i].a > 0)
            {
                Assert.Equal(0, result[i].r);
                Assert.Equal(0, result[i].g);
                Assert.Equal(0, result[i].b);
            }
        }
    }

    [Fact]
    public void ResampleClampsCatmullRomOvershootRatherThanWrapping()
    {
        // The kernel has negative lobes, so a hard edge overshoots past 0 and 255. Unclamped that
        // wraps a byte and puts bright speckles along the silhouette.
        const int size = 8;
        var source = HalfOpaque(size, new Color32(255, 255, 255, 255), new Color32(0, 0, 0, 0));
        var result = ShadeSpriteSmoothing.Resample(source, size, size, size * 3, size * 3);

        int width = size * 3;
        int row = width / 2;
        byte previous = result[row * width].a;
        for (int x = 1; x < width; x++)
        {
            byte alpha = result[row * width + x].a;
            // The source is opaque on the left and transparent on the right, so a correctly clamped
            // row falls monotonically. An unclamped overshoot wraps its byte and shows up here as a
            // value that climbs back up partway across the edge.
            Assert.True(alpha <= previous, $"alpha climbed from {previous} to {alpha} at x={x}");
            previous = alpha;
        }

        Assert.Equal(255, result[row * width].a);
        Assert.Equal(0, result[row * width + width - 1].a);
    }

    [Fact]
    public void AntialiasGrowsTheImageToTheTargetSize()
    {
        var source = Fill(8, 8, new Color32(0, 0, 0, 255));
        var result = ShadeSpriteSmoothing.Antialias(source, 8, 8, 32, out int width, out int height);

        Assert.Equal(32, width);
        Assert.Equal(32, height);
        Assert.Equal(width * height, result.Length);
    }

    [Fact]
    public void AntialiasSkipsTheResampleWhenTheSizeAlreadyMatches()
    {
        var source = Fill(8, 8, new Color32(0, 0, 0, 255));
        var result = ShadeSpriteSmoothing.Antialias(source, 8, 8, 8, out int width, out int height);

        Assert.Same(source, result);
        Assert.Equal(8, width);
        Assert.Equal(8, height);
    }

    [Fact]
    public void AntialiasLeavesDegenerateInputAlone()
    {
        var source = System.Array.Empty<Color32>();
        var result = ShadeSpriteSmoothing.Antialias(source, 0, 0, 512, out int width, out int height);

        Assert.Same(source, result);
        Assert.Equal(0, width);
        Assert.Equal(0, height);
    }

    [Fact]
    public void PadStripSurroundsEveryFrameWithATransparentGutter()
    {
        // Two 4x4 frames side by side, each a distinct solid colour.
        const int frame = 4;
        const int columns = 2;
        const int padding = 2;
        var source = new Color32[frame * columns * frame];
        for (int y = 0; y < frame; y++)
        {
            for (int x = 0; x < frame * columns; x++)
            {
                source[y * frame * columns + x] = x < frame
                    ? new Color32(255, 0, 0, 255)
                    : new Color32(0, 0, 255, 255);
            }
        }

        var padded = ShadeSpriteSmoothing.PadStrip(source, frame * columns, frame, columns, padding, out int width, out int height);

        Assert.Equal((frame + padding * 2) * columns, width);
        Assert.Equal(frame + padding * 2, height);

        // Every frame's content lands inset by the padding, in its own cell.
        int cell = frame + padding * 2;
        Assert.Equal(255, padded[padding * width + padding].r);
        Assert.Equal(255, padded[padding * width + cell + padding].b);

        // The gutter between the two frames is fully transparent, so bilinear sampling at a frame
        // edge cannot pull in the neighbouring frame.
        for (int y = 0; y < height; y++)
        {
            for (int gutter = 0; gutter < padding * 2; gutter++)
            {
                int x = padding + frame + gutter;
                Assert.Equal(0, padded[y * width + x].a);
            }
        }

        // Top and bottom rows are gutter too.
        for (int x = 0; x < width; x++)
        {
            Assert.Equal(0, padded[x].a);
            Assert.Equal(0, padded[(height - 1) * width + x].a);
        }
    }
}
