using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TeamCherry.NestedFadeGroup;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TeamCherry.Localization;
using LegacyoftheAbyss.Shade;

internal sealed partial class ShadeInventoryPane : InventoryPane
{
    private const int CharmRows = 4;
    private const int DefaultCharmColumns = 6;
    private static readonly Vector2 DefaultCharmCellSize = new Vector2(104f, 112f);
    private static readonly Vector2 DefaultCharmSpacing = new Vector2(16f, 16f);
    private const float RowOffsetFactor = 0.5f;
    private const float CharmIconSizeMultiplier = 0.69f;
    private const float CharmCellShrinkScale = 0.9f;
    private const float CharmCellShrinkWidthThreshold = 96f;
    private const float CharmCellShrinkHeightThreshold = 108f;
    private const float CharmCellMinWidth = 72f;
    private const float CharmCellMinHeight = 60f;
    private const float CharmSpacingScale = 0.4f;
    private const float CharmSpacingMin = 4f;
    private const float BackgroundAlpha = 0.82f;
    private const float CharmGridHorizontalScreenFraction = 0.15f;
    private const float CharmGridVerticalScreenFraction = 0.12f;
    private const float CharmGridHorizontalParentFraction = 0.12f;
    private const float CharmGridVerticalParentFraction = 0.04f;
    private const float SectionOffsetFraction = 0.05f;
    private const float DetailPreviewScale = 1.6f;
    private const float HighlightScaleMultiplier = 1.85f;
    private const float HighlightMinAlpha = 0.55f;
    // Vanilla charm panes report RectTransform sizes of roughly 6.5 × 8 units even
    // though the UI fills the screen once the canvas scale factor is applied. Treat
    // anything above a minimal epsilon as "valid" so we can adopt those template
    // metrics instead of falling back to oversized screen-space defaults.
    internal const float MinRootSizeThreshold = 0.1f;
    internal const float MinTemplateCopyDimension = 4f;
    internal const float MinTemplateCopyArea = 16f;
    private const float ShadeInputInitialRepeatDelay = 0.25f;
    private const float ShadeInputRepeatInterval = 0.15f;

    private static readonly Color DefaultPanelColor = new Color(0.05f, 0.05f, 0.08f, 0.92f);
    private static readonly Color DefaultHighlightColor = new Color(0.9f, 0.97f, 1f, 0.78f);
    private static readonly Color DefaultCellColor = new Color(0.18f, 0.2f, 0.26f, BackgroundAlpha);
    private static readonly Vector2 DefaultStandaloneRootSize = new Vector2(1920f, 1080f);
    private const string LockedCharmSpriteName = "shade_charm_charmui0001charmcost02unlit";
    private const string NotchLitSpriteName = "shade_charm_charmui0000charmcost02lit";
    private const int MaxNotchIcons = 11;
    private const int MaxEquippedIcons = 11;

    private static readonly Color LockedIconColor = new Color(1f, 1f, 1f, 0.72f);

    /// <summary>
    /// How large an undiscovered charm's placeholder is drawn relative to a real charm icon. The
    /// placeholder is a notch sprite, not charm art, and at full size it read as the biggest thing
    /// in the grid.
    /// </summary>
    private static readonly Vector3 LockedIconScale = new Vector3(0.5f, 0.5f, 1f);
    private static readonly Color InactiveIconColor = new Color(1f, 1f, 1f, 0.3f);
    private static readonly Color EquippedIconColor = new Color(0.82f, 0.95f, 1f, 1f);
    private static readonly Color BrokenIconColor = new Color(1f, 0.64f, 0.64f, 1f);
    private static readonly Color OvercharmedTextColor = new Color(1f, 0.45f, 0.45f, 1f);
    private static readonly Color OvercharmedEquippedIconColor = new Color(1f, 0.62f, 0.62f, 1f);
    private static readonly Color OvercharmedNotchFillColor = new Color(1f, 0.55f, 0.55f, 1f);
    private static readonly Color OvercharmedNotchHighlightColor = new Color(1f, 0.67f, 0.67f, 1f);
    private static readonly Color OvercharmedNotchEmptyColor = new Color(1f, 0.45f, 0.45f, 0.45f);
    private static readonly Color OvercharmedBackdropFallbackColor = new Color(1f, 0.45f, 0.45f, 0.32f);

    private static Sprite? lockedCharmSprite;
    private static bool lockedCharmSpriteSearched;
    private static TMP_FontAsset? cachedTrajanFont;
    private static Font? cachedTrajanSourceFont;
    private static bool searchedTrajanFont;
    private static ShadeInventoryPane? activePane;
    private struct RectSnapshot
    {
        public Vector2 AnchorMin;
        public Vector2 AnchorMax;
        public Vector2 Pivot;
        public Vector2 OffsetMin;
        public Vector2 OffsetMax;
        public Vector2 AnchoredPosition;
        public float AnchoredPositionZ;
        public Vector2 SizeDelta;
        public Quaternion LocalRotation;
        public Vector3 LocalScale;

        public static RectSnapshot From(RectTransform rect)
        {
            return new RectSnapshot
            {
                AnchorMin = rect.anchorMin,
                AnchorMax = rect.anchorMax,
                Pivot = rect.pivot,
                OffsetMin = rect.offsetMin,
                OffsetMax = rect.offsetMax,
                AnchoredPosition = rect.anchoredPosition,
                AnchoredPositionZ = rect.anchoredPosition3D.z,
                SizeDelta = rect.sizeDelta,
                LocalRotation = rect.localRotation,
                LocalScale = rect.localScale
            };
        }

        public void Apply(RectTransform rect)
        {
            rect.anchorMin = AnchorMin;
            rect.anchorMax = AnchorMax;
            rect.pivot = Pivot;
            rect.offsetMin = OffsetMin;
            rect.offsetMax = OffsetMax;
            rect.anchoredPosition = AnchoredPosition;
            rect.anchoredPosition3D = new Vector3(AnchoredPosition.x, AnchoredPosition.y, AnchoredPositionZ);
            rect.sizeDelta = SizeDelta;
            rect.localRotation = LocalRotation;
            rect.localScale = LocalScale;
        }
    }

    private struct LayoutElementSnapshot
    {
        public bool IgnoreLayout;
        public float MinWidth;
        public float PreferredWidth;
        public float FlexibleWidth;
        public float MinHeight;
        public float PreferredHeight;
        public float FlexibleHeight;
        public int LayoutPriority;

        public static LayoutElementSnapshot From(LayoutElement element)
        {
            return new LayoutElementSnapshot
            {
                IgnoreLayout = element.ignoreLayout,
                MinWidth = element.minWidth,
                PreferredWidth = element.preferredWidth,
                FlexibleWidth = element.flexibleWidth,
                MinHeight = element.minHeight,
                PreferredHeight = element.preferredHeight,
                FlexibleHeight = element.flexibleHeight,
                LayoutPriority = element.layoutPriority
            };
        }

        public void Apply(LayoutElement element)
        {
            element.ignoreLayout = IgnoreLayout;
            element.minWidth = MinWidth;
            element.preferredWidth = PreferredWidth;
            element.flexibleWidth = FlexibleWidth;
            element.minHeight = MinHeight;
            element.preferredHeight = PreferredHeight;
            element.flexibleHeight = FlexibleHeight;
            element.layoutPriority = LayoutPriority;
        }
    }

    private struct GridLayoutSnapshot
    {
        public Vector2 CellSize;
        public Vector2 Spacing;
        public GridLayoutGroup.Axis StartAxis;
        public GridLayoutGroup.Corner StartCorner;
        public GridLayoutGroup.Constraint Constraint;
        public int ConstraintCount;
        public RectOffset Padding;
        public TextAnchor ChildAlignment;

        public static GridLayoutSnapshot From(GridLayoutGroup grid)
        {
            return new GridLayoutSnapshot
            {
                CellSize = grid.cellSize,
                Spacing = grid.spacing,
                StartAxis = grid.startAxis,
                StartCorner = grid.startCorner,
                Constraint = grid.constraint,
                ConstraintCount = grid.constraintCount,
                Padding = new RectOffset(grid.padding.left, grid.padding.right, grid.padding.top, grid.padding.bottom),
                ChildAlignment = grid.childAlignment
            };
        }

        public void Apply(GridLayoutGroup grid)
        {
            grid.cellSize = CellSize;
            grid.spacing = Spacing;
            grid.startAxis = StartAxis;
            grid.startCorner = StartCorner;
            grid.constraint = Constraint;
            grid.constraintCount = ConstraintCount;
            grid.padding = new RectOffset(Padding.left, Padding.right, Padding.top, Padding.bottom);
            grid.childAlignment = ChildAlignment;
        }
    }

    private readonly List<CharmEntry> entries = new List<CharmEntry>();
    private readonly List<Vector2Int> entryGridPositions = new List<Vector2Int>();
    private readonly List<float> entryCenterXs = new List<float>();

    private RectTransform panelRoot = null!;
    private RectTransform contentRoot = null!;
    private RectTransform gridRoot = null!;
    private RectTransform? highlight;
    private Text? titleText;
    private Text? notchText;
    private Text? detailTitleText;
    private Text? descriptionText;
    private Text? statusText;
    private Text? hintText;
    private Text? detailCostLabel;
    private TMP_Text? titleTextTMP;
    private TMP_Text? notchTextTMP;
    private TMP_Text? detailTitleTextTMP;
    private TMP_Text? descriptionTextTMP;
    private TMP_Text? statusTextTMP;
    private bool statusTextAlignmentCaptured;
    private TextAnchor statusTextDefaultAlignment = TextAnchor.UpperLeft;
    private TextAlignmentOptions statusTextDefaultTmpAlignment = TextAlignmentOptions.TopLeft;
    private TMP_Text? hintTextTMP;
    private TMP_Text? detailCostLabelTMP;
    private Image? detailPreviewImage;
    private RectTransform? detailPreviewRect;
    private CanvasGroup canvasGroup = null!;
    private float detailPreviewTopOffset;
    private float detailDescriptionGap;
    private float detailDescriptionBottomPadding;
    private float detailHorizontalMargin;

    private GameObject? overlayCanvasObject;
    private RectTransform? overlayRoot;
    private Canvas? overlayCanvas;
    private CanvasScaler? overlayCanvasScaler;
    private GraphicRaycaster? overlayRaycaster;
    private ShadeInventoryPaneSlide? overlaySlide;
    private readonly Vector3[] overlayWorldCorners = new Vector3[4];

    private Font? bodyFont;
    private Font? headerFont;
    private Color bodyFontColor = Color.white;
    private Color headerFontColor = Color.white;
    private Sprite? panelBackgroundSprite;
    private Color panelBackgroundColor = DefaultPanelColor;
    private Sprite? highlightSpriteTemplate;
    private Color highlightColor = DefaultHighlightColor;
    private Sprite? cellFrameSprite;
    private Color cellFrameColor = DefaultCellColor;
    private TextStyle? bodyTextStyle;
    private TextStyle? headerTextStyle;
    private TmpTextStyle? bodyTmpTextStyle;
    private TmpTextStyle? headerTmpTextStyle;
    private Sprite? generatedHighlightSprite;
    private Texture2D? generatedHighlightTexture;

    private ShadeCharmInventory? inventory;
    private ShadeCharmInventory? subscribedInventory;
    private InventoryPaneList? attachedPaneList;
    private int selectedIndex;
    private bool isBuilt;
    private bool isActive;
    private readonly HashSet<InventoryPaneInput> boundInputs = new HashSet<InventoryPaneInput>();
    private bool hasCapturedInputFocus;
    private float labelPulseTimer;
    private Sprite? fallbackSprite;
    private string displayLabel = "Charms";
    private bool inputHandlersRegistered;
    private int lastPaneInputFrame = -1;
    private InventoryPaneBase.InputEventType lastPaneInputDirection = InventoryPaneBase.InputEventType.Left;
    private bool lastPaneInputCameFromEvent;
    private InventoryPaneBase.InputEventType? shadeHeldDirection;
    private float shadeDirectionRepeatTimer;
    private int lastShadeInputFrame = -1;
    private bool loggedInactiveHierarchyProcessing;

    private RectSnapshot? panelRectTemplate;
    private RectSnapshot? contentRectTemplate;
    private RectSnapshot? gridRectTemplate;
    private RectSnapshot? detailRectTemplate;
    private RectSnapshot? rootRectTemplate;
    private GridLayoutSnapshot? gridLayoutTemplate;
    private Vector2? templateRootSize;
    private LayoutElementSnapshot? rootLayoutTemplate;
    private bool useNormalizedFallbackLayout;
    private Vector2 normalizedFallbackRootSize;
    private Vector2 charmCellSize = DefaultCharmCellSize;
    private Vector2 charmSpacing = DefaultCharmSpacing;
    private float currentCharmIconSize = Mathf.Max(DefaultCharmCellSize.x, DefaultCharmCellSize.y) * CharmIconSizeMultiplier;
    private RectTransform? leftContentRoot;
    private RectTransform? notchIconContainer;
    private RectTransform? detailCostRow;
    private RectTransform? detailCostIconContainer;
    private RectTransform? equippedIconsRoot;
    private Image? equippedOvercharmBackdrop;
    private HorizontalLayoutGroup? equippedIconsLayout;
    private readonly List<Image> notchMeterIcons = new List<Image>(MaxNotchIcons);
    private readonly List<Image> detailCostIcons = new List<Image>(MaxNotchIcons);
    private readonly List<Image> equippedIcons = new List<Image>(MaxEquippedIcons);
    private readonly List<ShadeCharmId> previousEquippedOrder = new List<ShadeCharmId>();
    private readonly List<ShadeCharmId?> equippedDisplayIds = new List<ShadeCharmId?>(MaxEquippedIcons);
    private bool hasRenderedEquippedRow;
    private bool notchLabelDefaultsCaptured;
    private Color notchLabelDefaultColor = Color.white;
    private Color notchLabelDefaultTmpColor = Color.white;
    private readonly List<GameObject> activeCharmFlights = new List<GameObject>();
    private readonly HashSet<Image> animatingEquippedIcons = new HashSet<Image>();
    private readonly List<OverlayAnimation> overlayAnimations = new List<OverlayAnimation>();
    private readonly HashSet<OverlayAnimation> overcharmAnimations = new HashSet<OverlayAnimation>();
    private ActiveShakeAnimation? activeShakeAnimation;
    private readonly Dictionary<RectTransform, Vector2> shakeBasePositions = new Dictionary<RectTransform, Vector2>();
    private readonly HashSet<Image> animatingSourceIcons = new HashSet<Image>();
    private GameObject? activeOvercharmFlight;
    private bool overlayAnimationTimeInitialized;
    private int lastOverlayAnimationFrame = -1;
    private float lastOverlayAnimationTime;
    private static Sprite? notchLitSprite;
    private static Sprite? notchUnlitSprite;
    private static bool notchSpritesSearched;
    private static Sprite? overcharmBackdropSprite;
    private static bool overcharmBackdropSpriteSearched;

    private struct CharmEntry
    {
        public ShadeCharmDefinition Definition;
        public ShadeCharmId Id;
        public RectTransform Root;
        public Image Icon;
        public Image Background;
        public GameObject? NewMarker;
        public Sprite? BaseSprite;
        public Sprite? BrokenSprite;
    }

    private struct NotchAssignment
    {
        public Sprite? Icon;
        public ShadeCharmDefinition? Definition;
        public ShadeCharmId? CharmId;
    }

    private struct ShadowStyle
    {
        public Type Type;
        public Color EffectColor;
        public Vector2 EffectDistance;
        public bool UseGraphicAlpha;
    }

    private struct TextStyle
    {
        public Font? Font;
        public int FontSize;
        public FontStyle FontStyle;
        public TextAnchor Alignment;
        public Color Color;
        public bool RichText;
        public bool BestFit;
        public int BestFitMin;
        public int BestFitMax;
        public float LineSpacing;
        public bool AlignByGeometry;
        public HorizontalWrapMode HorizontalOverflow;
        public VerticalWrapMode VerticalOverflow;
        public List<ShadowStyle>? Shadows;
    }

    private struct TmpTextStyle
    {
        public TMP_FontAsset? Font;
        public Material? FontMaterial;
        public FontStyles FontStyle;
        public float FontSize;
        public bool EnableAutoSizing;
        public float FontSizeMin;
        public float FontSizeMax;
        public TextAlignmentOptions Alignment;
        public Color Color;
        public TextOverflowModes OverflowMode;
        public float CharacterSpacing;
        public float LineSpacing;
        public float ParagraphSpacing;
        public TextWrappingModes WrappingMode;
        public Vector4 Margin;
        public bool RichText;
        public List<ShadowStyle>? Shadows;
    }

    private enum OverlayAnimationEase
    {
        Linear,
        EaseIn,
        EaseOut
    }

    private sealed class OverlayAnimation
    {
        public RectTransform? Rect;
        public Vector2 Start;
        public Vector2 End;
        public float StartScale;
        public float EndScale;
        public float Duration;
        public float Elapsed;
        public OverlayAnimationEase Ease;
        public Action? OnCompleted;
        public Action? OnCancelled;

        public bool Update(float deltaTime)
        {
            if (Rect == null)
            {
                Complete();
                return true;
            }

            Elapsed += deltaTime;
            float progress = Duration <= 0f ? 1f : Mathf.Clamp01(Elapsed / Mathf.Max(Duration, Mathf.Epsilon));
            float eased = EvaluateEase(progress);
            Rect.anchoredPosition = Vector2.LerpUnclamped(Start, End, eased);
            float scale = Mathf.Lerp(StartScale, EndScale, eased);
            Rect.localScale = new Vector3(scale, scale, 1f);

            if (Elapsed >= Duration)
            {
                Complete();
                return true;
            }

            return false;
        }

        public void Cancel()
        {
            if (Rect != null)
            {
                Rect.anchoredPosition = End;
                Rect.localScale = new Vector3(EndScale, EndScale, 1f);
            }

            OnCancelled?.Invoke();
            OnCancelled = null;
            OnCompleted = null;
        }

        private void Complete()
        {
            if (Rect != null)
            {
                Rect.anchoredPosition = End;
                Rect.localScale = new Vector3(EndScale, EndScale, 1f);
            }

            OnCompleted?.Invoke();
            OnCompleted = null;
            OnCancelled = null;
        }

        private float EvaluateEase(float t)
        {
            switch (Ease)
            {
                case OverlayAnimationEase.EaseIn:
                    return ShadeInventoryPane.EaseInCubic(t);
                case OverlayAnimationEase.EaseOut:
                    return ShadeInventoryPane.EaseOutCubic(t);
                default:
                    return Mathf.Clamp01(t);
            }
        }
    }

    private sealed class ActiveShakeAnimation
    {
        public float Amplitude;
        public float Duration;
        public float Elapsed;
        public Action? OnCompleted;
        public Action? OnCancelled;

        public bool Update(float deltaTime, ShadeInventoryPane owner)
        {
            Elapsed += deltaTime;
            float normalized = Duration <= 0f ? 1f : Mathf.Clamp01(Elapsed / Mathf.Max(Duration, Mathf.Epsilon));
            float damp = 1f - Mathf.Pow(normalized, 2f);

            foreach (var kvp in owner.shakeBasePositions)
            {
                var rect = kvp.Key;
                if (rect == null)
                {
                    continue;
                }

                Vector2 basePos = kvp.Value;
                Vector2 offset = UnityEngine.Random.insideUnitCircle * Amplitude * damp;
                rect.anchoredPosition = basePos + offset;
            }

            if (Elapsed >= Duration)
            {
                owner.RestoreShakeTargets();
                OnCompleted?.Invoke();
                OnCompleted = null;
                OnCancelled = null;
                return true;
            }

            return false;
        }

        public void Cancel(ShadeInventoryPane owner)
        {
            owner.RestoreShakeTargets();
            OnCancelled?.Invoke();
            OnCancelled = null;
            OnCompleted = null;
        }
    }

    private static void SetTextValue(Text? text, TMP_Text? tmp, string value)
    {
        if (text != null)
        {
            text.text = value;
        }

        if (tmp != null)
        {
            tmp.text = value;
        }
    }

    private static string GetTextValue(Text? text, TMP_Text? tmp)
    {
        if (tmp != null)
        {
            return tmp.text ?? string.Empty;
        }

        if (text != null)
        {
            return text.text ?? string.Empty;
        }

        return string.Empty;
    }

    private void SetHintMessage(string? message)
    {
        string text = string.IsNullOrWhiteSpace(message) ? string.Empty : message!;
        SetTextValue(hintText, hintTextTMP, text);
        bool active = !string.IsNullOrWhiteSpace(text);
        if (hintText != null)
        {
            hintText.gameObject.SetActive(active);
        }
        if (hintTextTMP != null)
        {
            hintTextTMP.gameObject.SetActive(active);
        }
    }

    private void EnsureStatusTextAlignmentCaptured()
    {
        if (statusTextAlignmentCaptured)
        {
            return;
        }

        if (statusText != null)
        {
            statusTextDefaultAlignment = statusText.alignment;
        }

        if (statusTextTMP != null)
        {
            statusTextDefaultTmpAlignment = statusTextTMP.alignment;
        }

        statusTextAlignmentCaptured = true;
    }

    private void ApplyStatusTextAlignment(TextAnchor? textAlignment, TextAlignmentOptions? tmpAlignment)
    {
        EnsureStatusTextAlignmentCaptured();

        if (statusText != null)
        {
            statusText.alignment = textAlignment ?? statusTextDefaultAlignment;
        }

        if (statusTextTMP != null)
        {
            statusTextTMP.alignment = tmpAlignment ?? statusTextDefaultTmpAlignment;
        }
    }

    private static List<ShadowStyle> CaptureShadowStyles(Graphic graphic)
    {
        var list = new List<ShadowStyle>();
        if (graphic == null)
        {
            return list;
        }

        try
        {
            foreach (var shadow in graphic.GetComponents<Shadow>())
            {
                if (shadow == null)
                {
                    continue;
                }

                list.Add(new ShadowStyle
                {
                    Type = shadow.GetType(),
                    EffectColor = shadow.effectColor,
                    EffectDistance = shadow.effectDistance,
                    UseGraphicAlpha = shadow.useGraphicAlpha
                });
            }
        }
        catch
        {
        }

        return list;
    }

    private static void ClearAndApplyShadows(Graphic graphic, List<ShadowStyle>? styles)
    {
        if (graphic == null)
        {
            return;
        }

        try
        {
            foreach (var shadow in graphic.GetComponents<Shadow>())
            {
                if (shadow == null)
                {
                    continue;
                }

                UnityEngine.Object.DestroyImmediate(shadow);
            }
        }
        catch
        {
        }

        if (styles == null)
        {
            return;
        }

        foreach (var style in styles)
        {
            if (style.Type == null)
            {
                continue;
            }

            if (!(graphic.gameObject.AddComponent(style.Type) is Shadow newShadow))
            {
                continue;
            }

            newShadow.effectColor = style.EffectColor;
            newShadow.effectDistance = style.EffectDistance;
            newShadow.useGraphicAlpha = style.UseGraphicAlpha;
        }
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

    private static TmpTextStyle CaptureTmpTextStyle(TMP_Text text)
    {
        return new TmpTextStyle
        {
            Font = text.font,
            FontMaterial = text.fontSharedMaterial,
            FontStyle = text.fontStyle,
            FontSize = text.fontSize,
            EnableAutoSizing = text.enableAutoSizing,
            FontSizeMin = text.fontSizeMin,
            FontSizeMax = text.fontSizeMax,
            Alignment = text.alignment,
            Color = text.color,
            OverflowMode = text.overflowMode,
            CharacterSpacing = text.characterSpacing,
            LineSpacing = text.lineSpacing,
            ParagraphSpacing = text.paragraphSpacing,
            WrappingMode = text.textWrappingMode,
            Margin = text.margin,
            RichText = text.richText,
            Shadows = CaptureShadowStyles(text)
        };
    }

    private void ApplyTextStyle(Text text, TextStyle? style, Font? fallbackFont, Color fallbackColor, FontStyle fallbackStyle, int fallbackSize, TextAnchor fallbackAlignment)
    {
        if (text == null)
        {
            return;
        }

        if (style.HasValue)
        {
            var data = style.Value;
            text.font = data.Font ?? fallbackFont ?? text.font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = data.FontSize > 0 ? data.FontSize : Mathf.Max(fallbackSize, 1);
            text.fontStyle = data.FontStyle;
            text.alignment = data.Alignment;
            text.color = data.Color;
            text.supportRichText = data.RichText;
            text.resizeTextForBestFit = data.BestFit;
            if (data.BestFit)
            {
                text.resizeTextMinSize = data.BestFitMin;
                text.resizeTextMaxSize = data.BestFitMax;
            }
            text.lineSpacing = data.LineSpacing;
            text.alignByGeometry = data.AlignByGeometry;
            text.horizontalOverflow = data.HorizontalOverflow;
            text.verticalOverflow = data.VerticalOverflow;
            ClearAndApplyShadows(text, data.Shadows);
            if (fallbackFont != null && text.font != null &&
                text.font.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) < 0)
            {
                text.font = fallbackFont;
            }
            return;
        }

        text.font = fallbackFont ?? text.font ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontStyle = fallbackStyle;
        text.fontSize = Mathf.Max(fallbackSize, 1);
        text.alignment = fallbackAlignment;
        text.color = fallbackColor;
        text.supportRichText = true;
        text.resizeTextForBestFit = false;
        text.lineSpacing = 1f;
        text.alignByGeometry = false;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        ClearAndApplyShadows(text, null);
    }

    private void ApplyTmpTextStyle(TMP_Text text, TmpTextStyle? style, TMP_FontAsset? fallbackFont, Color fallbackColor, FontStyles fallbackStyle, float fallbackSize, TextAlignmentOptions fallbackAlignment)
    {
        if (text == null)
        {
            return;
        }

        if (style.HasValue)
        {
            var data = style.Value;
            if (data.Font != null)
            {
                text.font = data.Font;
            }
            else if (fallbackFont != null)
            {
                text.font = fallbackFont;
            }

            if (data.FontMaterial != null)
            {
                text.fontSharedMaterial = data.FontMaterial;
            }

            text.fontStyle = StripForcedUpperCase(data.FontStyle);
            text.fontSize = data.FontSize > 0f ? data.FontSize : Mathf.Max(fallbackSize, 1f);
            text.enableAutoSizing = data.EnableAutoSizing;
            if (data.EnableAutoSizing)
            {
                text.fontSizeMin = data.FontSizeMin;
                text.fontSizeMax = data.FontSizeMax > 0f ? data.FontSizeMax : text.fontSizeMax;
            }

            text.alignment = data.Alignment;
            text.color = data.Color;
            text.overflowMode = data.OverflowMode;
            text.characterSpacing = data.CharacterSpacing;
            text.lineSpacing = data.LineSpacing;
            text.paragraphSpacing = data.ParagraphSpacing;
            text.textWrappingMode = data.WrappingMode;
            text.margin = data.Margin;
            text.richText = data.RichText;
            ClearAndApplyShadows(text, data.Shadows);
            if (fallbackFont != null && text.font != null &&
                text.font.name.IndexOf("Trajan", StringComparison.OrdinalIgnoreCase) < 0)
            {
                text.font = fallbackFont;
            }
            return;
        }

        if (fallbackFont != null)
        {
            text.font = fallbackFont;
        }
        text.fontStyle = StripForcedUpperCase(fallbackStyle);
        text.fontSize = Mathf.Max(fallbackSize, 1f);
        text.enableAutoSizing = false;
        text.alignment = fallbackAlignment;
        text.color = fallbackColor;
        text.overflowMode = TextOverflowModes.Overflow;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.characterSpacing = 0f;
        text.lineSpacing = 1f;
        text.paragraphSpacing = 0f;
        text.margin = Vector4.zero;
        text.richText = true;
        ClearAndApplyShadows(text, null);
    }

    /// <summary>
    /// Drops <see cref="FontStyles.UpperCase"/> from a style captured off a game text.
    /// <para>
    /// The templates this pane clones its text styling from carry it, so every heading here came out
    /// as "EQUIPPED" and "NOTCHES" in flat capitals - which is not how the first game's inventory
    /// read. Without the flag the Trajan face renders its lowercase as small capitals on its own,
    /// which is the look being asked for, and is already what the charm titles beside them do.
    /// </para>
    /// </summary>
    private static FontStyles StripForcedUpperCase(FontStyles style) => style & ~FontStyles.UpperCase;

    private static FontStyles ConvertFontStyle(FontStyle style) => style switch
    {
        FontStyle.Bold => FontStyles.Bold,
        FontStyle.Italic => FontStyles.Italic,
        FontStyle.BoldAndItalic => FontStyles.Bold | FontStyles.Italic,
        _ => FontStyles.Normal
    };

    private static TextAlignmentOptions ConvertAlignment(TextAnchor anchor) => anchor switch
    {
        TextAnchor.UpperLeft => TextAlignmentOptions.TopLeft,
        TextAnchor.UpperCenter => TextAlignmentOptions.Top,
        TextAnchor.UpperRight => TextAlignmentOptions.TopRight,
        TextAnchor.MiddleLeft => TextAlignmentOptions.Left,
        TextAnchor.MiddleCenter => TextAlignmentOptions.Center,
        TextAnchor.MiddleRight => TextAlignmentOptions.Right,
        TextAnchor.LowerLeft => TextAlignmentOptions.BottomLeft,
        TextAnchor.LowerCenter => TextAlignmentOptions.Bottom,
        TextAnchor.LowerRight => TextAlignmentOptions.BottomRight,
        _ => TextAlignmentOptions.Center
    };

    private static FontStyle ConvertFontStyleFromTmp(FontStyles style)
    {
        bool bold = (style & FontStyles.Bold) != 0;
        bool italic = (style & FontStyles.Italic) != 0;
        if (bold && italic)
        {
            return FontStyle.BoldAndItalic;
        }

        if (bold)
        {
            return FontStyle.Bold;
        }

        if (italic)
        {
            return FontStyle.Italic;
        }

        return FontStyle.Normal;
    }

    private static TextAnchor ConvertAlignment(TextAlignmentOptions alignment) => alignment switch
    {
        TextAlignmentOptions.TopLeft => TextAnchor.UpperLeft,
        TextAlignmentOptions.Top => TextAnchor.UpperCenter,
        TextAlignmentOptions.TopRight => TextAnchor.UpperRight,
        TextAlignmentOptions.Left => TextAnchor.MiddleLeft,
        TextAlignmentOptions.Center => TextAnchor.MiddleCenter,
        TextAlignmentOptions.Right => TextAnchor.MiddleRight,
        TextAlignmentOptions.BottomLeft => TextAnchor.LowerLeft,
        TextAlignmentOptions.Bottom => TextAnchor.LowerCenter,
        TextAlignmentOptions.BottomRight => TextAnchor.LowerRight,
        TextAlignmentOptions.MidlineLeft => TextAnchor.MiddleLeft,
        TextAlignmentOptions.Midline => TextAnchor.MiddleCenter,
        TextAlignmentOptions.MidlineRight => TextAnchor.MiddleRight,
        _ => TextAnchor.MiddleCenter
    };

    private static TextStyle? ConvertTmpToTextStyle(TMP_Text text)
    {
        if (text == null)
        {
            return null;
        }

        var style = new TextStyle
        {
            Font = text.font != null ? text.font.sourceFontFile : null,
            FontSize = Mathf.RoundToInt(text.fontSize),
            FontStyle = ConvertFontStyleFromTmp(text.fontStyle),
            Alignment = ConvertAlignment(text.alignment),
            Color = text.color,
            RichText = text.richText,
            BestFit = text.enableAutoSizing,
            BestFitMin = Mathf.RoundToInt(text.enableAutoSizing ? text.fontSizeMin : text.fontSize),
            BestFitMax = Mathf.RoundToInt(text.enableAutoSizing ? (text.fontSizeMax > 0f ? text.fontSizeMax : text.fontSize) : text.fontSize),
            LineSpacing = text.lineSpacing,
            AlignByGeometry = false,
            HorizontalOverflow = text.textWrappingMode == TextWrappingModes.NoWrap ? HorizontalWrapMode.Overflow : HorizontalWrapMode.Wrap,
            VerticalOverflow = VerticalWrapMode.Overflow,
            Shadows = CaptureShadowStyles(text)
        };

        return style;
    }

    private static RectTransform? ResolveRectTransform(Text? text, TMP_Text? tmp)
    {
        if (text != null)
        {
            return text.rectTransform;
        }

        return tmp != null ? tmp.rectTransform : null;
    }

    private static bool IsUnityObjectAlive(UnityEngine.Object? obj) => obj != null;

}
