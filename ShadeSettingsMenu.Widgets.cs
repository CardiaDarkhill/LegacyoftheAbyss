#nullable disable
using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using BepInEx.Logging;
using GlobalEnums;
using LegacyoftheAbyss.Shade;

public static partial class ShadeSettingsMenu
{
    private static Sprite GetFallbackSprite(ref Sprite cache, string spriteName, bool sliced)
    {
        if (cache != null)
            return cache;

        const int size = 16;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var colors = new Color32[size * size];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = new Color32(255, 255, 255, 255);
        tex.SetPixels32(colors);
        tex.Apply();
        tex.name = spriteName + "Tex";
        tex.hideFlags = HideFlags.HideAndDontSave;

        Vector4 border = sliced ? new Vector4(6f, 6f, 6f, 6f) : Vector4.zero;
        cache = Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size, 0, SpriteMeshType.FullRect, border);
        cache.name = spriteName;
        cache.hideFlags = HideFlags.HideAndDontSave;
        return cache;
    }

    /// <summary>
    /// The game's own slider, cloned whole: the <see cref="Slider"/>'s object with everything that is
    /// not part of the slider stripped off by <see cref="StripToSliderParts"/>.
    /// <para>
    /// Clone rather than rebuild. The knob is a mostly-transparent sprite, quarter-turned, in a rect
    /// taller than its row and hung well above its slide area, with the fill inset at one end only -
    /// every one of those is a rect that can be copied instead of guessed at.
    /// </para>
    /// <para>
    /// "The Slider's own object" promises nothing about what else is on it: on this build it also
    /// carries the game's label, its value readout, a full-row cursor hotspot and a pair of selection
    /// fleurs, which is how a clone ends up with "Master Volume" and a stray "10" on every row.
    /// Identify nothing by name or component type - the game ships two text stacks - and reach the
    /// parts through the Slider's own references instead.
    /// </para>
    /// </summary>
    private static GameObject CreateGameSliderTemplate(GameObject gameSlider)
    {
        if (gameSlider == null)
        {
            return null;
        }

        var source = gameSlider.GetComponent<Slider>();
        if (source == null || source.fillRect == null || source.handleRect == null)
        {
            RecordSliderTemplate(LastSliderTemplateDescription + " | not a usable Slider; drawing the plain fallback");
            return null;
        }

        GameObject holder = null;
        try
        {
            // Both measurements are taken from the game's own slider, where it still sits in the
            // hierarchy it was authored in. A clone hanging under a holder of its own has no such
            // hierarchy, so anything it anchors to its parent measures against nothing.
            gameSliderCanvasScale = ScaleRelativeToCanvas(gameSlider.transform);
            var sourceRect = gameSlider.GetComponent<RectTransform>();
            float height = sourceRect != null ? sourceRect.rect.height : 0f;
            if (height <= 1f)
            {
                height = sourceRect != null ? Mathf.Max(1f, sourceRect.sizeDelta.y) : 1f;
            }

            // Where the line sits within the slider's own rect, so the clone can be hung with the
            // line on the row's centre, level with the label, however the game arranged the rest of
            // the rect around it. Measured off the fill's container rather than the fill: Slider
            // drives the fill's own anchors, so its rect is whatever the current value made it.
            var lineRect = source.fillRect.parent as RectTransform ?? source.fillRect;
            float lineOffset = CentreOffsetWithin(lineRect, sourceRect);
            string shape = DescribeSubtree(gameSlider.transform, 320);

            // Built inside an inactive holder so the clone never wakes: the game's slider carries a
            // MenuAudioSlider, whose Awake reaches for the audio settings this menu has no business
            // touching. SanitizeSelectableHierarchy strips that and everything like it below.
            holder = new GameObject("ShadeSliderTemplate");
            holder.hideFlags = HideFlags.HideAndDontSave;
            holder.SetActive(false);
            holder.AddComponent<RectTransform>();

            var clone = Object.Instantiate(gameSlider, holder.transform, false);
            clone.name = "Slider";
            clone.hideFlags = HideFlags.HideAndDontSave;
            clone.SetActive(true);

            var slider = clone.GetComponent<Slider>();
            var sliderRect = clone.GetComponent<RectTransform>();
            if (slider == null || slider.fillRect == null || sliderRect == null)
            {
                Object.DestroyImmediate(holder);
                RecordSliderTemplate(LastSliderTemplateDescription + " | clone came out empty; drawing the plain fallback");
                return null;
            }

            StripToSliderParts(source, slider);
            SanitizeSelectableHierarchy(clone);

            // Spans whatever width the row gives it, and keeps the game's own height, because that
            // is what decides how tall the knob is drawn and how far above the line it sits.
            sliderRect.anchorMin = new Vector2(0f, 0.5f);
            sliderRect.anchorMax = new Vector2(1f, 0.5f);
            sliderRect.pivot = new Vector2(0.5f, 0.5f);
            sliderRect.sizeDelta = new Vector2(0f, height);
            sliderRect.anchoredPosition = new Vector2(0f, -lineOffset);

            sliderTemplateIsGameClone = true;
            string kept = DescribeSubtree(clone.transform, 320);
            RecordSliderTemplate(FormattableString.Invariant(
                $"{LastSliderTemplateDescription} | cloned {height:0.#} tall, line {lineOffset:0.#} off centre, drawn at {gameSliderCanvasScale:0.000} of canvas | from: {shape} | kept: {kept}"));
            return holder;
        }
        catch (Exception e)
        {
            LogMenuWarning($"Could not clone the game's slider: {e}");
            if (holder != null)
            {
                Object.DestroyImmediate(holder);
            }

            RecordSliderTemplate(LastSliderTemplateDescription + " | clone failed; drawing the plain fallback");
            return null;
        }
    }

    /// <summary>
    /// Cuts a cloned slider down to the three things that are the slider: the track, the filled part
    /// of it and the knob.
    /// <para>
    /// The fill and the knob are found through the Slider's own <c>fillRect</c> and <c>handleRect</c>
    /// rather than by name, so nothing rests on what this build calls them. The track is found by
    /// shape - it is whatever else is drawn the same size as the fill's container, which is what
    /// sitting behind the fill means. That size is measured on the game's own slider and passed in:
    /// a clone hanging under a holder of its own resolves its stretched rects against nothing. A build that draws no separate track matches nothing here and
    /// simply ends up without one, which is a hairline missing rather than a row of someone else's
    /// text across the screen.
    /// </para>
    /// </summary>
    private static void StripToSliderParts(Slider source, Slider clone)
    {
        if (source == null || clone == null || source.fillRect == null || clone.fillRect == null)
        {
            return;
        }

        var sourceRoot = source.transform;
        var cloneRoot = clone.transform;
        if (sourceRoot.childCount != cloneRoot.childCount)
        {
            // Instantiate copies the hierarchy exactly, so this cannot happen - but deciding on one
            // tree and cutting the other is only safe while that holds, and cutting the wrong child
            // is a worse outcome than leaving the clone whole.
            LogMenuWarning("Cloned slider does not match the game's; leaving it whole.");
            return;
        }

        // Decided on the game's own slider and applied to the clone by index. The clone hangs under
        // a holder with no size, so any of its stretched rects measures zero there; the original
        // still sits in the hierarchy it was authored in and measures properly.
        var keep = new HashSet<Transform>();
        KeepChain(source.fillRect, sourceRoot, keep);
        KeepChain(source.handleRect, sourceRoot, keep);

        var lineRect = source.fillRect.parent as RectTransform ?? source.fillRect;
        Vector2 lineSize = lineRect.rect.size;

        var doomed = new List<int>();
        for (int i = 0; i < sourceRoot.childCount; i++)
        {
            var child = sourceRoot.GetChild(i);
            if (child == null || keep.Contains(child))
            {
                continue;
            }

            if (LooksLikeTheTrack(child as RectTransform, lineSize))
            {
                continue;
            }

            doomed.Add(i);
        }

        // Back to front, because DestroyImmediate reindexes the children as it goes.
        for (int i = doomed.Count - 1; i >= 0; i--)
        {
            Object.DestroyImmediate(cloneRoot.GetChild(doomed[i]).gameObject);
        }

        // Retargeted explicitly: whatever the game pointed this at may be one of the objects just
        // destroyed, and a destroyed reference is not null as far as ?? is concerned.
        var handleImage = clone.handleRect != null ? clone.handleRect.GetComponent<Image>() : null;
        if (handleImage != null)
        {
            clone.targetGraphic = handleImage;
        }
    }

    private static void KeepChain(RectTransform part, Transform root, HashSet<Transform> keep)
    {
        var cursor = part != null ? part.transform : null;
        for (int depth = 0; depth < 8 && cursor != null && cursor != root; depth++)
        {
            keep.Add(cursor);
            cursor = cursor.parent;
        }
    }

    /// <summary>Drawn, and the same shape as the fill's container - so it is the line behind it.</summary>
    private static bool LooksLikeTheTrack(RectTransform candidate, Vector2 lineSize)
    {
        if (candidate == null || candidate.GetComponent<Graphic>() == null)
        {
            return false;
        }

        if (lineSize.x <= 1f || lineSize.y <= 1f)
        {
            return false;
        }

        var size = candidate.rect.size;
        return Mathf.Abs(size.y - lineSize.y) <= Mathf.Max(2f, lineSize.y * 0.4f)
            && size.x >= lineSize.x * 0.5f
            && size.x <= lineSize.x * 1.5f;
    }

    /// <summary>
    /// How large something is drawn relative to the canvas it is on. One when that cannot be worked
    /// out, so a failure copies sizes across unchanged rather than collapsing them.
    /// </summary>
    private static float ScaleRelativeToCanvas(Transform subject)
    {
        try
        {
            if (subject == null)
            {
                return 1f;
            }

            // includeInactive: the screens this is asked about have all just been deactivated.
            var canvas = subject.GetComponentInParent<Canvas>(true);
            var canvasTransform = canvas != null && canvas.rootCanvas != null
                ? canvas.rootCanvas.transform
                : canvas != null ? canvas.transform : null;
            if (canvasTransform == null)
            {
                return 1f;
            }

            float ours = Mathf.Abs(subject.lossyScale.x);
            float theirs = Mathf.Abs(canvasTransform.lossyScale.x);
            if (ours < 0.0001f || theirs < 0.0001f)
            {
                return 1f;
            }

            return ours / theirs;
        }
        catch
        {
            return 1f;
        }
    }

    /// <summary>
    /// How much bigger the game's slider has to be built here to come out the size it is drawn at
    /// on the game's own option screens.
    /// <para>
    /// These screens are drawn at about two thirds of the canvas, and the layout on them was given
    /// correspondingly more local units to work in rather than being scaled back up - see
    /// <c>StretchScreenOverCanvas</c>. So a rect copied straight off a screen that is drawn at full
    /// size lands here at two thirds of the size it has over there. The game's line is a few units
    /// thick to begin with; two thirds of a few units is a hairline.
    /// </para>
    /// </summary>
    private static float SliderUnitScale
    {
        get
        {
            if (!sliderTemplateIsGameClone || screenCanvasScale < 0.0001f)
            {
                return 1f;
            }

            return Mathf.Clamp(gameSliderCanvasScale / screenCanvasScale, 0.2f, 5f);
        }
    }

    /// <summary>
    /// Resizes a cloned rect and everything under it. Only sizeDelta and anchoredPosition are
    /// touched, never localScale: a scaled rect still reports its unscaled size to the layout around
    /// it, which would leave the row budgeting for a slider two thirds the size of the one drawn.
    /// Anchors are left alone deliberately - Slider drives the fill's and the handle's every frame.
    /// </summary>
    private static void ScaleRectTree(RectTransform root, float factor)
    {
        if (root == null || Mathf.Approximately(factor, 1f))
        {
            return;
        }

        root.sizeDelta *= factor;
        root.anchoredPosition *= factor;
        for (int i = 0; i < root.childCount; i++)
        {
            ScaleRectTree(root.GetChild(i) as RectTransform, factor);
        }
    }

    /// <summary>
    /// A descendant rect's centre, in an ancestor's local units, relative to that ancestor's own
    /// centre. Walked up the chain rather than taken through world space, because the screen this
    /// is read from has never been shown and so has no meaningful world transform.
    /// </summary>
    private static float CentreOffsetWithin(RectTransform descendant, RectTransform ancestor)
    {
        if (descendant == null || ancestor == null)
        {
            return 0f;
        }

        // A RectTransform's local space is centred on its own pivot, so walking localPosition up the
        // chain gives the descendant's pivot relative to the ancestor's pivot. Both ends are then
        // corrected from pivot to centre.
        float y = (0.5f - descendant.pivot.y) * descendant.rect.height;
        var cursor = descendant;
        for (int depth = 0; depth < 8 && cursor != null && cursor != ancestor; depth++)
        {
            y += cursor.localPosition.y;
            cursor = cursor.parent as RectTransform;
        }

        if (cursor != ancestor)
        {
            return 0f;
        }

        return y - (0.5f - ancestor.pivot.y) * ancestor.rect.height;
    }

    /// <summary>
    /// What the clone actually came out as, for the bug reporter's snapshot. Every round of slider
    /// reports so far was spent working out what was in a hierarchy nobody had looked at; one line
    /// in the snapshot is cheaper than another round trip.
    /// </summary>
    private static string DescribeSubtree(Transform root, int budget)
    {
        var builder = new StringBuilder();
        AppendSubtree(root, builder, budget);
        return builder.ToString();
    }

    private static void AppendSubtree(Transform node, StringBuilder builder, int budget)
    {
        if (node == null || builder.Length > budget)
        {
            return;
        }

        if (builder.Length > 0)
        {
            builder.Append(' ');
        }

        builder.Append(node.name);
        if (node is RectTransform rect)
        {
            builder.Append(FormattableString.Invariant(
                $"[{rect.rect.width:0.#}x{rect.rect.height:0.#}@{rect.localPosition.y:0.#}"));
            float turn = rect.localEulerAngles.z;
            if (turn > 0.5f)
            {
                builder.Append(FormattableString.Invariant($"/{turn:0.#}deg"));
            }

            builder.Append(']');
        }

        var image = node.GetComponent<Image>();
        if (image != null)
        {
            builder.Append(FormattableString.Invariant(
                $"<{(image.sprite != null ? image.sprite.name : "plain")} a={image.color.a:0.00}>"));
        }

        for (int i = 0; i < node.childCount; i++)
        {
            AppendSubtree(node.GetChild(i), builder, budget);
        }
    }

    /// <summary>How thick the fallback draws its line, and how big a knob it puts above it.</summary>
    private const float FallbackTrackHeight = 4f;
    private const float FallbackKnobWidth = 15f;
    private const float FallbackKnobHeight = 17f;
    private const float FallbackKnobGap = 9f;

    /// <summary>
    /// The line drawn when the game's own slider could not be found. Deliberately plain: it is what
    /// a build whose UI this mod does not recognise falls back to, not a second attempt at matching
    /// the game's look.
    /// </summary>
    private static GameObject CreateDefaultSliderTemplate()
    {
        // Authored in this menu's own units, so it wants no reconciling with the game's.
        sliderTemplateIsGameClone = false;

        var root = new GameObject("DefaultSlider");
        root.hideFlags = HideFlags.HideAndDontSave;
        root.AddComponent<RectTransform>();
        root.AddComponent<MenuSelectable>();

        const float trackHalf = FallbackTrackHeight * 0.5f;
        const float knobBottom = trackHalf + FallbackKnobGap;
        const float knobTop = knobBottom + FallbackKnobHeight;

        var sliderGo = new GameObject("Slider");
        sliderGo.transform.SetParent(root.transform, false);
        var sliderRt = sliderGo.AddComponent<RectTransform>();
        // Spans the row's width; tall enough to hold the knob above the line, and symmetric about
        // the line so that centring the whole thing in the row puts the line on the row's centre.
        sliderRt.anchorMin = new Vector2(0f, 0.5f);
        sliderRt.anchorMax = new Vector2(1f, 0.5f);
        sliderRt.pivot = new Vector2(0.5f, 0.5f);
        sliderRt.sizeDelta = new Vector2(0f, knobTop * 2f);
        sliderRt.anchoredPosition = Vector2.zero;

        var uiSprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsSliderBg", true);

        var background = new GameObject("Background");
        background.transform.SetParent(sliderGo.transform, false);
        var bgImage = background.AddComponent<Image>();
        bgImage.sprite = uiSprite;
        bgImage.type = Image.Type.Sliced;
        bgImage.color = new Color(0.86f, 0.85f, 0.80f, 0.35f);
        var backgroundRt = background.GetComponent<RectTransform>();
        backgroundRt.anchorMin = new Vector2(0f, 0.5f);
        backgroundRt.anchorMax = new Vector2(1f, 0.5f);
        backgroundRt.offsetMin = new Vector2(0f, -trackHalf);
        backgroundRt.offsetMax = new Vector2(0f, trackHalf);

        // Full width, both of these. The knob's centre travels the whole line and the fill runs out
        // to meet it, which is how the game's own slider reads a value off the track.
        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGo.transform, false);
        var fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = new Vector2(0f, 0.5f);
        fillAreaRt.anchorMax = new Vector2(1f, 0.5f);
        fillAreaRt.offsetMin = new Vector2(0f, -trackHalf);
        fillAreaRt.offsetMax = new Vector2(0f, trackHalf);

        var fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        var fillImg = fill.AddComponent<Image>();
        fillImg.sprite = uiSprite;
        fillImg.type = Image.Type.Sliced;
        fillImg.color = new Color(0.96f, 0.95f, 0.90f, 0.95f);
        var fillRt = fill.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        var handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderGo.transform, false);
        var handleAreaRt = handleArea.AddComponent<RectTransform>();
        handleAreaRt.anchorMin = new Vector2(0f, 0.5f);
        handleAreaRt.anchorMax = new Vector2(1f, 0.5f);
        handleAreaRt.offsetMin = new Vector2(0f, knobBottom);
        handleAreaRt.offsetMax = new Vector2(0f, knobTop);

        var handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.sprite = GetFallbackSprite(ref fallbackKnobSprite, "ShadeSettingsSliderKnob", false);
        handleImg.color = new Color(1f, 0.98f, 0.92f, 1f);
        var handleRt = handle.GetComponent<RectTransform>();
        // Slider drives the anchors to stretch this over the slide area's height, so sizeDelta.y is
        // an adjustment to that height rather than a height of its own; zero leaves it filling the
        // slide area, which is already the knob's height.
        handleRt.sizeDelta = new Vector2(FallbackKnobWidth, 0f);
        handleRt.anchoredPosition = Vector2.zero;

        var slider = sliderGo.AddComponent<Slider>();
        slider.fillRect = fillRt;
        slider.handleRect = handleRt;
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;
        slider.transition = Selectable.Transition.ColorTint;
        var colors = slider.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.95f, 0.78f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        colors.selectedColor = colors.normalColor;
        colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        slider.colors = colors;

        root.SetActive(false);
        return root;
    }

    private static MenuSelectable CreateDefaultToggleTemplate()
    {
        var root = new GameObject("DefaultToggle");
        root.hideFlags = HideFlags.HideAndDontSave;
        root.AddComponent<RectTransform>();
        var selectable = root.AddComponent<MenuSelectable>();

        var toggleGo = new GameObject("Toggle");
        toggleGo.transform.SetParent(root.transform, false);
        var toggleRt = toggleGo.AddComponent<RectTransform>();
        toggleRt.sizeDelta = new Vector2(20f, 20f);

        var background = new GameObject("Background");
        background.transform.SetParent(toggleGo.transform, false);
        var bgImage = background.AddComponent<Image>();
        var uiSprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsToggleBg", true);
        bgImage.sprite = uiSprite;
        bgImage.type = Image.Type.Sliced;
        bgImage.color = new Color(0.12f, 0.12f, 0.12f, 0.9f);
        var backgroundRt = background.GetComponent<RectTransform>();
        backgroundRt.anchorMin = Vector2.zero;
        backgroundRt.anchorMax = Vector2.one;
        backgroundRt.offsetMin = Vector2.zero;
        backgroundRt.offsetMax = Vector2.zero;

        var checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(background.transform, false);
        var checkImg = checkmark.AddComponent<Image>();
        var checkSprite = GetFallbackSprite(ref fallbackCheckSprite, "ShadeSettingsToggleCheck", false);
        checkImg.sprite = checkSprite;
        checkImg.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        var checkRt = checkmark.GetComponent<RectTransform>();
        checkRt.anchorMin = new Vector2(0.2f, 0.2f);
        checkRt.anchorMax = new Vector2(0.8f, 0.8f);
        checkRt.offsetMin = Vector2.zero;
        checkRt.offsetMax = Vector2.zero;

        var toggle = toggleGo.AddComponent<Toggle>();
        toggle.graphic = checkImg;
        toggle.targetGraphic = bgImage;
        toggle.transition = Selectable.Transition.ColorTint;
        var colors = toggle.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 0.95f, 0.78f, 1f);
        colors.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        colors.selectedColor = colors.normalColor;
        colors.disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.6f);
        toggle.colors = colors;

        root.SetActive(false);
        return selectable;
    }

    private static Animator CloneAnimator(Animator source, Transform parent, string nameSuffix)
    {
        if (source == null || parent == null)
            return null;

        var clone = Object.Instantiate(source.gameObject, parent, false);
        clone.name = string.IsNullOrEmpty(nameSuffix) ? source.gameObject.name : nameSuffix;
        var layout = clone.GetComponent<LayoutElement>() ?? clone.AddComponent<LayoutElement>();
        layout.ignoreLayout = true;
        layout.minWidth = 0f;
        layout.preferredWidth = 0f;
        layout.flexibleWidth = 0f;
        layout.minHeight = 0f;
        layout.preferredHeight = 0f;
        layout.flexibleHeight = 0f;

        var sourceRect = source.GetComponent<RectTransform>();
        var cloneRect = clone.GetComponent<RectTransform>();
        if (sourceRect != null && cloneRect != null)
        {
            cloneRect.anchorMin = sourceRect.anchorMin;
            cloneRect.anchorMax = sourceRect.anchorMax;
            cloneRect.pivot = sourceRect.pivot;
            cloneRect.sizeDelta = sourceRect.sizeDelta;
            cloneRect.anchoredPosition = sourceRect.anchoredPosition;
            cloneRect.anchoredPosition3D = sourceRect.anchoredPosition3D;
            cloneRect.localScale = sourceRect.localScale;
            cloneRect.localRotation = sourceRect.localRotation;
        }

        foreach (var graphic in clone.GetComponentsInChildren<Graphic>(true))
        {
            if (graphic != null)
                graphic.raycastTarget = false;
        }

        var animator = clone.GetComponent<Animator>();
        if (animator != null)
        {
            try
            {
                animator.ResetTrigger("show");
                animator.ResetTrigger("hide");
                animator.Update(0f);
            }
            catch
            {
            }
        }
        return animator;
    }

    private static GameObject CreateRowHighlight(Transform parent, MenuButton buttonTemplate, float height, string label, out Animator leftCursor, out Animator rightCursor, out Animator selectHighlight)
    {
        leftCursor = null;
        rightCursor = null;
        selectHighlight = null;

        if (parent == null)
            return null;

        string baseName = string.IsNullOrEmpty(label) ? "Highlight" : label.Replace(" ", string.Empty) + "Highlight";
        var highlight = new GameObject(baseName);
        var rect = highlight.AddComponent<RectTransform>();
        highlight.transform.SetParent(parent, false);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        rect.anchoredPosition3D = Vector3.zero;

        var layout = highlight.AddComponent<LayoutElement>();
        layout.ignoreLayout = true;
        layout.minHeight = height;
        layout.preferredHeight = height;
        layout.flexibleHeight = 0f;
        layout.minWidth = 0f;
        layout.preferredWidth = 0f;
        layout.flexibleWidth = 0f;

        if (buttonTemplate != null)
        {
            leftCursor = CloneAnimator(buttonTemplate.leftCursor, highlight.transform, baseName + "Left");
            rightCursor = CloneAnimator(buttonTemplate.rightCursor, highlight.transform, baseName + "Right");
            selectHighlight = CloneAnimator(buttonTemplate.selectHighlight, highlight.transform, baseName + "Center");
        }

        if (leftCursor == null && rightCursor == null && selectHighlight == null)
        {
            var image = highlight.AddComponent<Image>();
            image.sprite = GetFallbackSprite(ref fallbackSlicedSprite, "ShadeSettingsButtonBg", true);
            image.type = Image.Type.Sliced;
            image.color = ButtonHighlightColor;
            image.raycastTarget = false;
            highlight.SetActive(false);
        }

        highlight.transform.SetAsFirstSibling();
        return highlight;
    }

    private static List<ShadowStyle> CaptureShadowStyles(Graphic graphic)
    {
        var list = new List<ShadowStyle>();
        foreach (var shadow in graphic.GetComponents<Shadow>())
        {
            list.Add(new ShadowStyle
            {
                Type = shadow.GetType(),
                EffectColor = shadow.effectColor,
                EffectDistance = shadow.effectDistance,
                UseGraphicAlpha = shadow.useGraphicAlpha
            });
        }
        return list;
    }

    private static TextStyle CaptureTextStyle(Text text)
    {
        return new TextStyle
        {
            Font = text.font,
            FontSize = text.fontSize,
            FontStyle = text.fontStyle,
            Alignment = text.alignment,
            Color = text.color,
            RichText = text.supportRichText,
            BestFit = text.resizeTextForBestFit,
            BestFitMin = text.resizeTextMinSize,
            BestFitMax = text.resizeTextMaxSize,
            LineSpacing = text.lineSpacing,
            AlignByGeometry = text.alignByGeometry,
            HorizontalOverflow = text.horizontalOverflow,
            VerticalOverflow = text.verticalOverflow,
            Shadows = CaptureShadowStyles(text)
        };
    }

    private static void ClearAndApplyShadows(Graphic graphic, List<ShadowStyle> styles)
    {
        foreach (var shadow in graphic.GetComponents<Shadow>())
            Object.DestroyImmediate(shadow);

        if (styles == null)
            return;

        foreach (var style in styles)
        {
            if (style.Type == null)
                continue;
            if (!(graphic.gameObject.AddComponent(style.Type) is Shadow newShadow))
                continue;
            newShadow.effectColor = style.EffectColor;
            newShadow.effectDistance = style.EffectDistance;
            newShadow.useGraphicAlpha = style.UseGraphicAlpha;
        }
    }

    private static void ApplyTextStyle(Text text, TextStyle? style, TextAnchor defaultAlignment, Color defaultColor)
    {
        var resolved = style.GetValueOrDefault();
        bool hasStyle = style.HasValue;

        var fontToUse = resolved.Font != null ? resolved.Font : fallbackFont;
        if (fontToUse == null)
            fontToUse = Resources.GetBuiltinResource<Font>("Arial.ttf");

        text.font = fontToUse;
        text.color = hasStyle ? resolved.Color : defaultColor;
        text.enabled = true;
        text.raycastTarget = false;
        text.alignment = hasStyle ? resolved.Alignment : defaultAlignment;
        text.fontSize = hasStyle && resolved.FontSize > 0 ? resolved.FontSize : 24;
        text.fontStyle = hasStyle ? resolved.FontStyle : FontStyle.Normal;
        text.supportRichText = hasStyle ? resolved.RichText : true;
        text.lineSpacing = hasStyle ? resolved.LineSpacing : 1f;
        text.resizeTextForBestFit = hasStyle && resolved.BestFit;
        text.resizeTextMinSize = hasStyle && resolved.BestFit ? resolved.BestFitMin : 10;
        text.resizeTextMaxSize = hasStyle && resolved.BestFit ? resolved.BestFitMax : 40;
        text.alignByGeometry = hasStyle ? resolved.AlignByGeometry : false;
        text.horizontalOverflow = hasStyle ? resolved.HorizontalOverflow : HorizontalWrapMode.Overflow;
        text.verticalOverflow = hasStyle ? resolved.VerticalOverflow : VerticalWrapMode.Overflow;

        // Only the fallback size gets scaled up. MenuFontScale exists to lift this file's own
        // default (24) to something readable at the size these screens draw at; a size captured from
        // a real game text is already right for this canvas, and multiplying it again is how every
        // label on the Difficulty screen ended up three times too large the moment a game slider
        // became the template.
        if (!hasStyle && MenuFontScale > 1f)
        {
            text.fontSize = Mathf.Max(1, Mathf.RoundToInt(text.fontSize * MenuFontScale));
            if (text.resizeTextForBestFit)
            {
                int originalMin = text.resizeTextMinSize;
                int originalMax = text.resizeTextMaxSize;
                text.resizeTextMinSize = Mathf.Max(1, Mathf.RoundToInt(originalMin * MenuFontScale));
                text.resizeTextMaxSize = Mathf.Max(text.resizeTextMinSize, Mathf.RoundToInt(originalMax * MenuFontScale));
            }
        }

        if (text.color.a <= 0.01f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }

        ClearAndApplyShadows(text, hasStyle ? resolved.Shadows : null);
    }

    private static void ScaleUnityText(Text text, float scale)
    {
        if (text == null || scale <= 0f || Mathf.Approximately(scale, 1f))
            return;

        int adjustedSize = Mathf.Max(1, Mathf.RoundToInt(text.fontSize * scale));
        text.fontSize = adjustedSize;

        if (text.resizeTextForBestFit)
        {
            int min = Mathf.Max(1, Mathf.RoundToInt(text.resizeTextMinSize * scale));
            int max = Mathf.Max(min, Mathf.RoundToInt(text.resizeTextMaxSize * scale));
            text.resizeTextMinSize = min;
            text.resizeTextMaxSize = max;
        }
    }

    private static void ScaleTextElements(GameObject root, float scale)
    {
        if (root == null || scale <= 0f || Mathf.Approximately(scale, 1f))
            return;

        foreach (var text in root.GetComponentsInChildren<Text>(true))
        {
            ScaleUnityText(text, scale);
        }

        var tmpType = Type.GetType("TMPro.TextMeshProUGUI, Unity.TextMeshPro");
        if (tmpType == null)
            return;

        foreach (var tmp in root.GetComponentsInChildren(tmpType, true))
        {
            try
            {
                var fontSizeProp = tmpType.GetProperty("fontSize");
                if (fontSizeProp != null)
                {
                    float currentSize = Convert.ToSingle(fontSizeProp.GetValue(tmp, null));
                    fontSizeProp.SetValue(tmp, currentSize * scale, null);
                }

                var autoSizeProp = tmpType.GetProperty("enableAutoSizing");
                if (autoSizeProp != null && autoSizeProp.GetValue(tmp, null) is bool autoSize && autoSize)
                {
                    var minProp = tmpType.GetProperty("fontSizeMin");
                    var maxProp = tmpType.GetProperty("fontSizeMax");
                    if (minProp != null)
                    {
                        float min = Convert.ToSingle(minProp.GetValue(tmp, null));
                        minProp.SetValue(tmp, min * scale, null);
                    }
                    if (maxProp != null)
                    {
                        float max = Convert.ToSingle(maxProp.GetValue(tmp, null));
                        maxProp.SetValue(tmp, max * scale, null);
                    }
                }
            }
            catch
            {
            }
        }
    }

    private static void ApplyButtonColors(Selectable selectable)
    {
        if (selectable == null)
            return;

        selectable.transition = Selectable.Transition.ColorTint;
        var colors = selectable.colors;
        colors.normalColor = ButtonNormalColor;
        colors.highlightedColor = ButtonHighlightColor;
        colors.selectedColor = ButtonHighlightColor;
        colors.pressedColor = ButtonPressedColor;
        colors.disabledColor = ButtonDisabledColor;
        colors.colorMultiplier = 1f;
        selectable.colors = colors;

        if (selectable.targetGraphic != null)
        {
            selectable.targetGraphic.color = ButtonNormalColor;
            selectable.targetGraphic.raycastTarget = true;
        }
    }

    private static void CacheTextStyles(GameObject sliderTemplate, MenuSelectable toggleTemplate)
    {
        sliderLabelStyle = null;
        sliderValueStyle = null;
        toggleLabelStyle = null;
        fallbackFont = null;

        if (sliderTemplate != null)
        {
            foreach (var text in sliderTemplate.GetComponentsInChildren<Text>(true))
            {
                if (text == null)
                    continue;
                var hasAuto = text.GetComponent<AutoLocalizeTextUI>() != null;
                if (hasAuto)
                {
                    if (!sliderLabelStyle.HasValue)
                    {
                        sliderLabelStyle = CaptureTextStyle(text);
                        if (text.font != null)
                            fallbackFont ??= text.font;
                    }
                }
                else
                {
                    if (!sliderValueStyle.HasValue)
                    {
                        sliderValueStyle = CaptureTextStyle(text);
                        if (text.font != null)
                            fallbackFont ??= text.font;
                    }
                }
            }
        }

        if (toggleTemplate != null)
        {
            foreach (var text in toggleTemplate.GetComponentsInChildren<Text>(true))
            {
                if (text == null)
                    continue;
                if (!toggleLabelStyle.HasValue)
                {
                    toggleLabelStyle = CaptureTextStyle(text);
                    if (text.font != null)
                        fallbackFont ??= text.font;
                }
            }
        }

        if (fallbackFont == null)
            fallbackFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
    }

    private static void SanitizeSelectableHierarchy(GameObject root)
    {
        if (root == null)
            return;

        foreach (var comp in root.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (comp == null)
                continue;
            if (comp is CancelRouter || comp is SliderMenuDriver)
                continue;
            var type = comp.GetType();
            string ns = type.Namespace ?? string.Empty;
            if (ns.StartsWith("UnityEngine"))
                continue;
            Object.DestroyImmediate(comp);
        }

        foreach (var group in root.GetComponentsInChildren<CanvasGroup>(true))
        {
            if (group == null)
                continue;
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        var selectable = root.GetComponent<MenuSelectable>();
        if (selectable != null && selectable.targetGraphic == null)
        {
            var graphic = root.GetComponent<Graphic>();
            if (graphic == null)
                graphic = root.GetComponentInChildren<Graphic>(true);
            if (graphic != null)
            {
                selectable.targetGraphic = graphic;
                graphic.raycastTarget = true;
            }
        }
    }

    private static void SetAutomaticNavigation(Selectable selectable)
    {
        if (selectable == null)
            return;

        var navigation = selectable.navigation;
        navigation.mode = Navigation.Mode.Automatic;
        navigation.wrapAround = false;
        selectable.navigation = navigation;
    }

    private static void ConfigureHorizontalNavigation(IList<MenuButton> buttons)
    {
        if (buttons == null || buttons.Count == 0)
            return;

        for (int i = 0; i < buttons.Count; i++)
        {
            var button = buttons[i];
            if (button == null)
                continue;

            var navigation = button.navigation;
            var up = navigation.selectOnUp;
            var down = navigation.selectOnDown;
            navigation.mode = Navigation.Mode.Explicit;
            navigation.wrapAround = false;
            navigation.selectOnLeft = i > 0 ? buttons[i - 1] : navigation.selectOnLeft;
            navigation.selectOnRight = i < buttons.Count - 1 ? buttons[i + 1] : navigation.selectOnRight;
            navigation.selectOnUp = up;
            navigation.selectOnDown = down;
            button.navigation = navigation;
        }
    }

    private static Font FindFontInObject(GameObject root)
    {
        if (root == null)
            return null;
        foreach (var text in root.GetComponentsInChildren<Text>(true))
        {
            if (text != null && text.font != null)
                return text.font;
        }
        return null;
    }

    private static void ApplyPreferredFont(Font font)
    {
        if (font == null)
            return;
        fallbackFont = font;
        if (sliderLabelStyle.HasValue)
        {
            var style = sliderLabelStyle.Value;
            style.Font = font;
            sliderLabelStyle = style;
        }
        if (sliderValueStyle.HasValue)
        {
            var style = sliderValueStyle.Value;
            style.Font = font;
            sliderValueStyle = style;
        }
        if (toggleLabelStyle.HasValue)
        {
            var style = toggleLabelStyle.Value;
            style.Font = font;
            toggleLabelStyle = style;
        }
    }

    private static float SnapSliderValue(float value, float min, float max, bool whole)
    {
        value = Mathf.Clamp(value, min, max);
        if (whole)
        {
            var rounded = Mathf.Round(value);
            if (rounded < min)
                rounded = min;
            if (rounded > max)
                rounded = max;
            return rounded;
        }

        float snapped = Mathf.Round((value - min) / FractionalSliderStep) * FractionalSliderStep + min;
        snapped = Mathf.Clamp(snapped, min, max);
        float multiplier = 1f / FractionalSliderStep;
        snapped = Mathf.Round(snapped * multiplier) / multiplier;
        return snapped;
    }

    private static string FormatSliderValue(float value, bool whole)
    {
        return whole ? Mathf.RoundToInt(value).ToString() : value.ToString("0.0", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Column widths for one slider row. Parameters rather than constants because the Difficulty
    /// screen puts sliders inside half-width panels, where full-width defaults do not fit.
    /// </summary>
    private struct SliderRowMetrics
    {
        public float LabelWidth;
        public float SliderWidth;
        public float ValueWidth;
        public float Spacing;
        public float Height;

        /// <summary>
        /// True when <see cref="Height"/> is the row's height rather than its minimum. The default
        /// metrics take the taller of the cloned template and <see cref="SliderRowHeight"/>, which is
        /// right for a screen that stacks rows down a list, and wrong for the Difficulty screen's
        /// panels, where the rows sit at computed offsets and one taller than its slot overlaps the
        /// row below it.
        /// </summary>
        public bool FixedHeight;

        public static SliderRowMetrics Default => new SliderRowMetrics
        {
            LabelWidth = LabelColumnWidth,
            SliderWidth = 800f,
            ValueWidth = ValueColumnWidth,
            Spacing = 64f,
            Height = SliderRowHeight
        };

        /// <summary>Proportional columns for a row that has to live inside <paramref name="rowWidth"/>.</summary>
        public static SliderRowMetrics ForWidth(float rowWidth, float height)
        {
            float spacing = Mathf.Clamp(rowWidth * 0.04f, 16f, 48f);
            float label = Mathf.Clamp(rowWidth * 0.36f, 160f, 460f);
            float value = Mathf.Clamp(rowWidth * 0.11f, 64f, ValueColumnWidth);
            float slider = Mathf.Max(80f, rowWidth - label - value - spacing * 2f);
            return new SliderRowMetrics
            {
                LabelWidth = label,
                SliderWidth = slider,
                ValueWidth = value,
                Spacing = spacing,
                Height = height,
                FixedHeight = true
            };
        }
    }

    private static MenuSelectable CreateSlider(Transform parent, GameObject template, MenuButton buttonTemplate, string label, float min, float max, float value, System.Action<float> onChange, CancelTarget cancelTarget, bool whole = false)
        => CreateSlider(parent, template, buttonTemplate, label, min, max, value, onChange, cancelTarget, SliderRowMetrics.Default, out _, whole);

    private static MenuSelectable CreateSlider(Transform parent, GameObject template, MenuButton buttonTemplate, string label, float min, float max, float value, System.Action<float> onChange, CancelTarget cancelTarget, SliderRowMetrics metrics, bool whole = false)
        => CreateSlider(parent, template, buttonTemplate, label, min, max, value, onChange, cancelTarget, metrics, out _, whole);

    /// <param name="rowTransform">
    /// The row this built, for callers that position rows themselves. Handed back rather than left
    /// to be derived from the returned selectable's parent: the selectable sits on the row or on the
    /// clone depending on whether the template carried a MenuSelectable of its own, so deriving it
    /// moves a whole panel when the guess is wrong.
    /// </param>
    private static MenuSelectable CreateSlider(Transform parent, GameObject template, MenuButton buttonTemplate, string label, float min, float max, float value, System.Action<float> onChange, CancelTarget cancelTarget, SliderRowMetrics metrics, out RectTransform rowTransform, bool whole = false)
    {
        rowTransform = null;
        // container row stretching full width
        var row = new GameObject(label + "Row");
        var rowRect = row.AddComponent<RectTransform>();
        rowRect.SetParent(parent, false);
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0.5f, 1f);
        var hLayout = row.AddComponent<HorizontalLayoutGroup>();
        hLayout.childControlHeight = true;
        hLayout.childControlWidth = true;
        hLayout.childForceExpandHeight = false;
        hLayout.childForceExpandWidth = false;
        hLayout.spacing = metrics.Spacing;
        hLayout.childAlignment = TextAnchor.MiddleLeft;

        // label text
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(row.transform, false);
        var labelTxt = labelObj.AddComponent<Text>();
        ApplyTextStyle(labelTxt, sliderLabelStyle, TextAnchor.MiddleLeft, Color.white);
        labelTxt.text = label;
        labelTxt.raycastTarget = false;
        var labelLe = labelObj.AddComponent<LayoutElement>();
        labelLe.minWidth = metrics.LabelWidth;
        labelLe.preferredWidth = metrics.LabelWidth;
        labelLe.flexibleWidth = 0f;

        // slider instance
        var go = Object.Instantiate(template, row.transform, false);
        go.SetActive(true);
        // The template is hidden and flagged DontSave so it does not clutter the hierarchy or get
        // unloaded mid-build; a row cloned from it wants neither.
        go.hideFlags = HideFlags.None;
        go.name = label + "Slider";

        var slider = go.GetComponentInChildren<Slider>(true);
        if (slider == null)
        {
            LogMenuError($"Created slider '{label}' missing Slider component");
            Object.DestroyImmediate(row);
            return null;
        }

        SanitizeSelectableHierarchy(go);
        // Done per row rather than once on the template: the figure it needs is measured while the
        // screens are set up, which happens after the template is built and before any row is.
        ScaleRectTree(slider.GetComponent<RectTransform>(), SliderUnitScale);
        rowTransform = rowRect;
        Object.DestroyImmediate(slider.GetComponent<MenuAudioSlider>());
        Object.DestroyImmediate(slider.GetComponent<MenuPreventDeselect>());
        slider.onValueChanged.RemoveAllListeners();
        slider.interactable = true;
        slider.enabled = true;
        // No SliderRightStickInput. The game wires that component's slider reference in the editor
        // through OnValidate, which never runs at runtime, so one added here has a null slider and
        // throws the moment it decides the stick is pushed. It is also not wanted: it snaps the
        // value to its minimum or maximum rather than stepping, and SliderMenuDriver already handles
        // left/right on the row.
        Object.DestroyImmediate(slider.GetComponent<SliderRightStickInput>());
        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0.5f);
        rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(0f, 0.5f);
        var sliderLe = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        sliderLe.minWidth = metrics.SliderWidth;
        sliderLe.preferredWidth = metrics.SliderWidth;
        sliderLe.flexibleWidth = 1f;
        // The slider itself is anchored to stretch across this row slot, so it takes whatever width
        // the layout settles on. Setting a width here instead would fight that: with stretched
        // anchors sizeDelta.x is an inset from the parent, not a width, so a row wider or narrower
        // than asked for came out with the track overhanging or stopping short.

        // value text to the right of slider
        var valueObj = new GameObject("Value");
        valueObj.transform.SetParent(row.transform, false);
        var valueTxt = valueObj.AddComponent<Text>();
        ApplyTextStyle(valueTxt, sliderValueStyle, TextAnchor.MiddleRight, Color.white);
        valueTxt.raycastTarget = false;
        var valueLe = valueObj.AddComponent<LayoutElement>();
        valueLe.minWidth = metrics.ValueWidth;
        valueLe.preferredWidth = metrics.ValueWidth;
        valueLe.flexibleWidth = 0f;

        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = whole;
        float initialValue = SnapSliderValue(value, min, max, whole);
        slider.SetValueWithoutNotify(initialValue);
        valueTxt.text = FormatSliderValue(initialValue, whole);
        if (!Mathf.Approximately(initialValue, value))
        {
            try
            {
                onChange.Invoke(initialValue);
            }
            catch (Exception e)
            {
                LogMenuWarning($"Error normalizing slider '{label}' value: {e}");
            }
        }
        slider.onValueChanged.AddListener(v =>
        {
            var snapped = SnapSliderValue(v, min, max, whole);
            if (!Mathf.Approximately(snapped, v))
                slider.SetValueWithoutNotify(snapped);
            onChange.Invoke(snapped);
            valueTxt.text = FormatSliderValue(snapped, whole);
        });

        SetAutomaticNavigation(slider);

        var rowLe = row.AddComponent<LayoutElement>();
        float baseHeight = 0f;
        if (rect != null)
        {
            baseHeight = rect.rect.height;
            if (baseHeight <= 0f)
                baseHeight = rect.sizeDelta.y;
        }
        if (metrics.FixedHeight || baseHeight <= 0f)
            baseHeight = metrics.Height;
        else
            baseHeight = Mathf.Max(baseHeight, metrics.Height);
        rowLe.preferredHeight = baseHeight;
        rowLe.minHeight = baseHeight;
        rowRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);
        if (rect != null)
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, baseHeight);
        // Told to the LayoutElement as well as stamped on the RectTransform. hLayout has
        // childControlHeight on, so it recomputes this child's height on every rebuild from
        // whatever the LayoutElement reports - and with no height reported that is zero, which
        // silently undoes the size set on the line above the first time anything triggers one.
        sliderLe.minHeight = baseHeight;
        sliderLe.preferredHeight = baseHeight;

        // The row's selectable is normally the one the cloned template carries. The game's volume
        // rows put the Slider itself on the object we clone and have no MenuSelectable of their own,
        // so one goes on the row instead - not on the clone, because a second Selectable beside the
        // Slider makes both reachable and the highlight lands on whichever the EventSystem picks.
        var selectable = go.GetComponent<MenuSelectable>();
        GameObject driverHost;
        if (selectable != null)
        {
            driverHost = go;
        }
        else
        {
            selectable = row.AddComponent<MenuSelectable>();
            driverHost = row;
            // The Slider stays interactive for the mouse but is taken out of keyboard/controller
            // navigation, so Up/Down cannot stop on it instead of on the row.
            var sliderNav = slider.navigation;
            sliderNav.mode = Navigation.Mode.None;
            slider.navigation = sliderNav;
        }

        selectable.DontPlaySelectSound = true;
        selectable.cancelAction = CancelAction.DoNothing;

        // A slider row is a label, a track and a number, none of which fills the row - so without a
        // raycast target of its own only the few pixels of track were hoverable. Fully transparent,
        // and behind the slider's own graphics in the hierarchy, so dragging the handle still goes
        // to the slider rather than to this.
        // On the row itself, not a child: children of a UI object draw and raycast above it, so the
        // slider's own graphics still receive the drag. (Reordering siblings here would reorder the
        // row within its list, which is not what a raycast target should do.)
        var rowRaycast = row.GetComponent<Image>() ?? row.AddComponent<Image>();
        rowRaycast.color = Color.clear;
        rowRaycast.raycastTarget = true;

        var rowPointerSelect = driverHost.GetComponent<PointerSelectDriver>() ?? driverHost.AddComponent<PointerSelectDriver>();
        rowPointerSelect.target = selectable;
        var rowSurfacePointerSelect = row.GetComponent<PointerSelectDriver>() ?? row.AddComponent<PointerSelectDriver>();
        rowSurfacePointerSelect.target = selectable;
        if (selectable.targetGraphic == null)
        {
            selectable.targetGraphic = slider.targetGraphic ?? go.GetComponentInChildren<Graphic>(true);
        }
        var router = driverHost.GetComponent<CancelRouter>() ?? driverHost.AddComponent<CancelRouter>();
        router.target = cancelTarget;
        var driver = driverHost.GetComponent<SliderMenuDriver>() ?? driverHost.AddComponent<SliderMenuDriver>();
        driver.Initialize(slider, whole);

        var highlight = CreateRowHighlight(row.transform, buttonTemplate, baseHeight, label, out var leftCursor, out var rightCursor, out var selectHighlight);
        var highlightDriver = driverHost.GetComponent<RowHighlightDriver>() ?? driverHost.AddComponent<RowHighlightDriver>();
        highlightDriver.Initialize(highlight, new[] { leftCursor, rightCursor, selectHighlight });
        selectable.leftCursor = leftCursor;
        selectable.rightCursor = rightCursor;
        selectable.selectHighlight = selectHighlight;
        SetAutomaticNavigation(selectable);
        return selectable;
    }

    /// <summary>
    /// A yes/no row. Rendered as an ordinary menu button whose label reads
    /// <c>"Something: On"</c>, rather than as a checkbox square.
    /// <para>
    /// Not a clone of the game's Toggle prefab: that is a second visual language for what the other
    /// rows already say in words, and a Toggle clone carries none of the selection fleurs a
    /// MenuButton clone does, so those rows are the ones that look permanently unselected.
    /// </para>
    /// </summary>
    private static MenuSelectable CreateToggle(Transform parent, MenuButton buttonTemplate, string label, bool value, System.Action<bool> onChange, CancelTarget cancelTarget, System.Func<bool> unavailable = null)
    {
        var selectable = CreateMenuButton(parent, buttonTemplate, label, null, cancelTarget);
        if (selectable is not MenuButton button)
        {
            if (selectable == null)
                LogMenuError($"Could not create toggle row '{label}'");
            return selectable;
        }

        var driver = button.gameObject.AddComponent<LabeledToggleDriver>();
        driver.Initialize(button, label, value, onChange, unavailable);
        return button;
    }

}
#nullable restore
