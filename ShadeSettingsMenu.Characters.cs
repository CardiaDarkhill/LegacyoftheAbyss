#nullable disable
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using LegacyoftheAbyss.Shade;

public static partial class ShadeSettingsMenu
{
    private const float SkinRowMinHeight = 48f;
    private const float SkinPreviewFramePadding = 48f;
    private static Sprite fallbackGlowSprite;
    private static readonly Color SkinEquippedColor = new Color(0.92f, 0.86f, 0.55f, 1f);
    // Most Shade skins are near-black silhouettes; the pause menu darkens everything behind
    // it, so without something behind the preview the sprite is nearly invisible. Soft and
    // low-opacity so it reads as a light source rather than a sticker behind the character.
    private static readonly Color SkinPreviewBackdropColor = new Color(1f, 0.93f, 0.78f, 0.35f);

    /// <summary>
    /// A square white texture with alpha falling off smoothly from the center to the edge —
    /// a soft radial glow, cached and tinted per use via Image.color.
    /// </summary>
    private static Sprite GetGlowSprite()
    {
        if (fallbackGlowSprite != null)
            return fallbackGlowSprite;

        const int size = 256;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.hideFlags = HideFlags.HideAndDontSave;
        tex.name = "ShadeSkinPreviewGlowTex";

        Vector2 center = new Vector2(size / 2f, size / 2f);
        float radius = size / 2f;
        var pixels = new Color32[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center) / radius;
                float alpha = Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(dist));
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        fallbackGlowSprite = Sprite.Create(tex, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f));
        fallbackGlowSprite.name = "ShadeSkinPreviewGlow";
        fallbackGlowSprite.hideFlags = HideFlags.HideAndDontSave;
        return fallbackGlowSprite;
    }

    // Retained so switching character can rebuild this one screen. The build pass destroys its own
    // templates once every screen has cloned them, so this keeps a clone of its own.
    private static UIManager charactersUi;
    private static MenuButton charactersButtonTemplate;

    private static void BuildCharactersMenu(UIManager ui, MenuScreen ms, MenuButton buttonTemplate)
    {
        if (ms == null || buttonTemplate == null)
            return;

        charactersUi = ui;
        if (charactersButtonTemplate == null)
        {
            charactersButtonTemplate = Object.Instantiate(buttonTemplate.gameObject).GetComponent<MenuButton>();
            if (charactersButtonTemplate != null)
            {
                charactersButtonTemplate.gameObject.hideFlags = HideFlags.HideAndDontSave;
                charactersButtonTemplate.gameObject.SetActive(false);
            }
        }

        var content = CreateContentRoot(ms);
        if (content == null)
            return;

        skinsController = ms.gameObject.GetComponent<SkinMenuController>() ?? ms.gameObject.AddComponent<SkinMenuController>();

        // Rescan on every build so a skin folder dropped in mid-session shows up without a restart.
        ShadeSkinManager.Reload();
        var skins = ShadeSkinManager.Skins;

        // Scale the preview with the screen instead of a fixed size, so it reads clearly on a
        // 4K display without overrunning the content area on a 1080p one. 900 is 3x the size
        // this originally shipped at.
        float previewSize = Mathf.Clamp(Screen.height * 0.45f, 260f, 900f);
        float previewColumnWidth = previewSize + 120f;

        // Preview column on the left, selectable skin list on the right.
        var row = new GameObject("SkinsRow");
        var rowRect = row.AddComponent<RectTransform>();
        rowRect.SetParent(content, false);
        var rowLayoutGroup = row.AddComponent<HorizontalLayoutGroup>();
        rowLayoutGroup.spacing = 48f;
        rowLayoutGroup.childControlHeight = true;
        rowLayoutGroup.childControlWidth = true;
        rowLayoutGroup.childForceExpandHeight = true;
        rowLayoutGroup.childForceExpandWidth = false;
        rowLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        var rowLayout = row.AddComponent<LayoutElement>();
        rowLayout.flexibleHeight = 1f;
        rowLayout.flexibleWidth = 1f;

        var previewColumn = new GameObject("SkinPreview");
        var previewColumnRect = previewColumn.AddComponent<RectTransform>();
        previewColumnRect.SetParent(row.transform, false);
        var previewLayoutGroup = previewColumn.AddComponent<VerticalLayoutGroup>();
        previewLayoutGroup.spacing = 16f;
        // Must control height, or the LayoutElement heights set below on the image/labels are
        // never applied — children keep whatever size their RectTransform happened to start
        // with instead.
        previewLayoutGroup.childControlHeight = true;
        previewLayoutGroup.childControlWidth = true;
        previewLayoutGroup.childForceExpandHeight = false;
        previewLayoutGroup.childForceExpandWidth = true;
        previewLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        var previewColumnLayout = previewColumn.AddComponent<LayoutElement>();
        previewColumnLayout.minWidth = previewColumnWidth;
        previewColumnLayout.preferredWidth = previewColumnWidth;
        previewColumnLayout.flexibleWidth = 0f;

        // A soft glow behind the sprite — not a panel — so near-black skins (most of them)
        // read against the pause menu's darkened backdrop like they're lit from behind,
        // rather than sitting on a sticker. Width is pinned (flexibleWidth = 0) so the
        // column's force-expand doesn't stretch it into an oval.
        var previewFrame = new GameObject("PreviewGlow");
        previewFrame.transform.SetParent(previewColumn.transform, false);
        var previewFrameImage = previewFrame.AddComponent<Image>();
        previewFrameImage.sprite = GetGlowSprite();
        previewFrameImage.type = Image.Type.Simple;
        previewFrameImage.color = SkinPreviewBackdropColor;
        previewFrameImage.raycastTarget = false;
        float previewFrameSize = previewSize + SkinPreviewFramePadding * 2f;
        var previewFrameLayout = previewFrame.AddComponent<LayoutElement>();
        previewFrameLayout.minWidth = previewFrameSize;
        previewFrameLayout.preferredWidth = previewFrameSize;
        previewFrameLayout.flexibleWidth = 0f;
        previewFrameLayout.minHeight = previewFrameSize;
        previewFrameLayout.preferredHeight = previewFrameSize;
        previewFrameLayout.flexibleHeight = 0f;

        var previewImageObj = new GameObject("PreviewImage");
        previewImageObj.transform.SetParent(previewFrame.transform, false);
        var previewImage = previewImageObj.AddComponent<Image>();
        previewImage.preserveAspect = true;
        previewImage.raycastTarget = false;
        var previewImageRect = previewImageObj.GetComponent<RectTransform>();
        previewImageRect.anchorMin = Vector2.zero;
        previewImageRect.anchorMax = Vector2.one;
        previewImageRect.offsetMin = new Vector2(SkinPreviewFramePadding, SkinPreviewFramePadding);
        previewImageRect.offsetMax = new Vector2(-SkinPreviewFramePadding, -SkinPreviewFramePadding);

        var previewNameObj = new GameObject("PreviewName");
        previewNameObj.transform.SetParent(previewColumn.transform, false);
        var previewName = previewNameObj.AddComponent<Text>();
        ApplyTextStyle(previewName, sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        var previewNameLayout = previewNameObj.AddComponent<LayoutElement>();
        previewNameLayout.minHeight = 48f;
        previewNameLayout.preferredHeight = 48f;

        var previewMovesetObj = new GameObject("PreviewMoveset");
        previewMovesetObj.transform.SetParent(previewColumn.transform, false);
        var previewMoveset = previewMovesetObj.AddComponent<Text>();
        ApplyTextStyle(previewMoveset, sliderValueStyle ?? sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        ScaleTextElements(previewMovesetObj, 0.85f);
        var previewMovesetLayout = previewMovesetObj.AddComponent<LayoutElement>();
        previewMovesetLayout.minHeight = 36f;
        previewMovesetLayout.preferredHeight = 36f;

        var previewStatusObj = new GameObject("PreviewStatus");
        previewStatusObj.transform.SetParent(previewColumn.transform, false);
        var previewStatus = previewStatusObj.AddComponent<Text>();
        ApplyTextStyle(previewStatus, sliderValueStyle ?? sliderLabelStyle, TextAnchor.MiddleCenter, Color.white);
        ScaleTextElements(previewStatusObj, 0.85f);
        var previewStatusLayout = previewStatusObj.AddComponent<LayoutElement>();
        previewStatusLayout.minHeight = 40f;
        previewStatusLayout.preferredHeight = 40f;

        var listRoot = new GameObject("SkinList");
        var listRect = listRoot.AddComponent<RectTransform>();
        listRect.SetParent(row.transform, false);
        var listLayoutGroup = listRoot.AddComponent<VerticalLayoutGroup>();
        listLayoutGroup.spacing = 12f;
        listLayoutGroup.childControlHeight = true;
        listLayoutGroup.childControlWidth = true;
        listLayoutGroup.childForceExpandHeight = true;
        listLayoutGroup.childForceExpandWidth = true;
        listLayoutGroup.childAlignment = TextAnchor.UpperCenter;
        var listLayout = listRoot.AddComponent<LayoutElement>();
        listLayout.flexibleWidth = 1f;
        listLayout.flexibleHeight = 1f;

        var selectables = new List<MenuSelectable>();

        // The menu configures the primary companion; when more than one can spawn this grows a
        // companion picker above these rows.
        int companionId = ShadeCompanionRegistry.PrimaryId;

        // A character that brings its own skins is represented by those skin rows rather than a row
        // of its own - a "Shade" row sitting above a "Shade" skin listed the same choice twice.
        foreach (var character in ShadeCharacterRegistry.Characters)
        {
            if (character.SupportsSkins)
            {
                continue;
            }

            var characterSelectable = CreateMenuButton(
                listRoot.transform,
                buttonTemplate,
                character.DisplayName,
                () => skinsController.HandleCharacterSubmitted(companionId, character),
                CancelTarget.ShadeMain);

            if (characterSelectable is not MenuButton characterButton)
            {
                if (characterSelectable != null)
                    selectables.Add(characterSelectable);
                continue;
            }

            var characterLayout = characterButton.GetComponent<LayoutElement>() ?? characterButton.gameObject.AddComponent<LayoutElement>();
            characterLayout.minHeight = SkinRowMinHeight;
            characterLayout.preferredHeight = ButtonRowHeight;
            characterLayout.flexibleHeight = 1f;

            var characterDriver = characterButton.gameObject.AddComponent<CharacterButtonDriver>();
            characterDriver.Initialize(skinsController, companionId, character, characterButton);
            selectables.Add(characterButton);
        }

        // Always listed, whichever character is equipped: picking a skin is also how you come back
        // to the Shade from the Knight.
        foreach (var skin in skins ?? (IReadOnlyList<ShadeSkinDefinition>)Array.Empty<ShadeSkinDefinition>())
        {
            if (skin == null)
                continue;

            // Route equip through the onSubmit callback (-> OnSubmitPressed), not a separate
            // ISubmitHandler on the row: MenuButton.OnPointerClick calls its own OnSubmit
            // directly rather than through ExecuteEvents, so a sibling ISubmitHandler never
            // sees mouse clicks — only OnSubmitPressed fires for both mouse and gamepad.
            var selectable = CreateMenuButton(listRoot.transform, buttonTemplate, skin.DisplayName, () => skinsController.HandleSkinSubmitted(skin), CancelTarget.ShadeMain);
            if (selectable is not MenuButton button)
            {
                if (selectable != null)
                    selectables.Add(selectable);
                continue;
            }

            // Let the rows share whatever vertical space is left so extra skin folders
            // stay on screen instead of overflowing the list.
            var buttonLayout = button.GetComponent<LayoutElement>() ?? button.gameObject.AddComponent<LayoutElement>();
            buttonLayout.minHeight = SkinRowMinHeight;
            buttonLayout.preferredHeight = ButtonRowHeight;
            buttonLayout.flexibleHeight = 1f;

            var driver = button.gameObject.AddComponent<SkinButtonDriver>();
            driver.Initialize(skinsController, skin, button);
            selectables.Add(button);
        }

        skinsController.Initialize(previewImage, previewName, previewMoveset, previewStatus);

        SetupButtonList(ms, selectables);
        var preferred = skinsController.GetSelectedSkinSelectable() ?? (selectables.Count > 0 ? selectables[0] : null);
        if (preferred != null)
        {
            screenFirstSelectables[ms] = preferred;
            ms.defaultHighlight = preferred;
        }
        else if (ms.backButton != null)
        {
            screenFirstSelectables[ms] = ms.backButton;
            ms.defaultHighlight = ms.backButton;
        }

        ConfigureBackButton(ms, CancelTarget.ShadeMain, ui);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
    }

    private sealed class SkinMenuController : MonoBehaviour
    {
        private readonly List<SkinButtonDriver> buttons = new();
        private readonly List<CharacterButtonDriver> characterButtons = new();
        private Image previewImage;
        private Text previewName;
        private Text previewMoveset;
        private Text previewStatus;
        private ShadeSkinDefinition focusedSkin;

        public void RegisterCharacterButton(CharacterButtonDriver driver)
        {
            if (driver != null && !characterButtons.Contains(driver))
                characterButtons.Add(driver);
        }

        public void UnregisterCharacterButton(CharacterButtonDriver driver)
        {
            if (driver != null)
                characterButtons.Remove(driver);
        }

        public void HandleCharacterFocused(int companionId, ShadeCharacterDefinition character)
        {
            if (character == null)
                return;

            if (previewImage != null)
            {
                // A character with skins previews through the selected one; the Knight renders from
                // an asset bundle and carries a still of its own instead.
                var sprite = character.SupportsSkins
                    ? ShadeSkinManager.GetPreviewSprite(ShadeSkinManager.SelectedSkin)
                    : ShadeCharacterManager.GetPreviewSprite(character);
                previewImage.sprite = sprite;
                previewImage.enabled = sprite != null;
                previewImage.color = Color.white;
            }

            if (previewName != null)
                previewName.text = character.DisplayName;

            SetMovesetLabel(character.MovesetName);

            if (previewStatus != null)
            {
                bool equipped = ShadeCharacterManager.GetSelected(companionId).Id == character.Id;
                previewStatus.text = equipped ? "Equipped" : character.Description;
                previewStatus.color = equipped ? SkinEquippedColor : Color.white;
            }
        }

        public void HandleCharacterSubmitted(int companionId, ShadeCharacterDefinition character)
        {
            if (character == null)
                return;

            LegacyHelper.SetShadeCharacter(companionId, character.Id);
            RefreshCharacterButtons(companionId);
            HandleCharacterFocused(companionId, character);

            // The skin list belongs to the character, so switching character rebuilds the screen.
            // Deferred a frame: the rebuild destroys the row this call is running inside.
            StartCoroutine(RebuildNextFrame());
        }

        private static System.Collections.IEnumerator RebuildNextFrame()
        {
            yield return null;
            RebuildCharactersScreen();
        }

        private void RefreshCharacterButtons(int companionId)
        {
            var selected = ShadeCharacterManager.GetSelected(companionId);
            for (int i = characterButtons.Count - 1; i >= 0; i--)
            {
                var driver = characterButtons[i];
                if (driver == null)
                {
                    characterButtons.RemoveAt(i);
                    continue;
                }

                driver.Refresh(selected.Id);
            }
        }

        public void Initialize(Image image, Text nameLabel, Text movesetLabel, Text statusLabel)
        {
            previewImage = image;
            previewName = nameLabel;
            previewMoveset = movesetLabel;
            previewStatus = statusLabel;
            RefreshButtons();
            ShowPreview(ShadeSkinManager.SelectedSkin);
        }

        public void RegisterSkinButton(SkinButtonDriver driver)
        {
            if (driver != null && !buttons.Contains(driver))
                buttons.Add(driver);
        }

        public void UnregisterSkinButton(SkinButtonDriver driver)
        {
            if (driver != null)
                buttons.Remove(driver);
        }

        public MenuSelectable GetSelectedSkinSelectable()
        {
            string selectedId = ShadeSkinManager.SelectedSkinId;
            foreach (var driver in buttons)
            {
                if (driver != null && driver.Skin != null && driver.Skin.Matches(selectedId))
                    return driver.Button;
            }

            return null;
        }

        public void HandleScreenShown()
        {
            int companionId = ShadeCompanionRegistry.PrimaryId;
            RefreshButtons();
            RefreshCharacterButtons(companionId);

            var character = ShadeCharacterManager.GetSelected(companionId);
            if (!character.SupportsSkins)
            {
                // No skin rows exist for this character, so the preview shows the character itself.
                HandleCharacterFocused(companionId, character);
                return;
            }

            ShowPreview(focusedSkin ?? ShadeSkinManager.SelectedSkin);
        }

        public void HandleSkinFocused(ShadeSkinDefinition skin)
        {
            focusedSkin = skin;
            ShowPreview(skin);
        }

        public void HandleSkinSubmitted(ShadeSkinDefinition skin)
        {
            if (skin == null)
                return;

            // Skins belong to the Shade, so choosing one is also how you leave the Knight.
            int companionId = ShadeCompanionRegistry.PrimaryId;
            bool leavingKnight = ShadeCharacterManager.GetSelected(companionId).Id != ShadeCharacterId.Shade;
            if (leavingKnight)
            {
                LegacyHelper.SetShadeCharacter(companionId, ShadeCharacterId.Shade);
            }

            LegacyHelper.SetShadeSkin(skin.Id);
            focusedSkin = skin;
            RefreshButtons();
            RefreshCharacterButtons(companionId);
            ShowPreview(skin);
        }

        private void SetMovesetLabel(string moveset)
        {
            if (previewMoveset != null)
            {
                previewMoveset.text = moveset;
                previewMoveset.color = Color.white;
            }
        }

        private void RefreshButtons()
        {
            // No skin is equipped while another character is: marking one would show two rows as
            // equipped at once.
            bool shadeEquipped = ShadeCharacterManager
                .GetSelected(ShadeCompanionRegistry.PrimaryId).Id == ShadeCharacterId.Shade;
            string selectedId = shadeEquipped ? ShadeSkinManager.SelectedSkinId : null;
            for (int i = buttons.Count - 1; i >= 0; i--)
            {
                var driver = buttons[i];
                if (driver == null)
                {
                    buttons.RemoveAt(i);
                    continue;
                }

                driver.Refresh(selectedId);

                // screenFirstSelectables[skinsScreen] is only a build-time snapshot otherwise —
                // keep it pointed at whatever is actually equipped so re-entering this screen
                // (or MenuFocusDriver reclaiming a lost selection) lands on the current pick
                // instead of snapping back to whatever was equipped when the menu was built.
                if (skinsScreen != null && driver.Button != null && driver.Skin != null && driver.Skin.Matches(selectedId))
                {
                    screenFirstSelectables[skinsScreen] = driver.Button;
                    skinsScreen.defaultHighlight = driver.Button;
                }
            }
        }

        private void ShowPreview(ShadeSkinDefinition skin)
        {
            if (skin == null)
                return;

            if (previewImage != null)
            {
                var sprite = ShadeSkinManager.GetPreviewSprite(skin);
                previewImage.sprite = sprite;
                previewImage.enabled = sprite != null;
                previewImage.color = Color.white;
            }

            if (previewName != null)
                previewName.text = skin.DisplayName;

            SetMovesetLabel(ShadeCharacterRegistry.Get(ShadeCharacterId.Shade).MovesetName);

            if (previewStatus != null)
            {
                bool equipped = skin.Matches(ShadeSkinManager.SelectedSkinId);
                previewStatus.text = equipped ? "Equipped" : "Press to equip";
                previewStatus.color = equipped ? SkinEquippedColor : Color.white;
            }
        }
    }

    /// <summary>
    /// Rebuilds the Characters screen in place after a character switch, because the skin rows
    /// beneath belong to the character and are created at build time.
    /// </summary>
    /// <summary>
    /// Rebuilds every screen whose contents depend on which character is equipped. The Controls
    /// screen is one of them: the two characters do not share a control scheme, and it is built
    /// once at launch.
    /// </summary>
    internal static void NotifyCharacterChanged()
    {
        RebuildCharactersScreen();

        if (controlsScreen != null && charactersButtonTemplate != null)
        {
            BuildControlsMenu(charactersUi, controlsScreen, charactersButtonTemplate);
        }
    }

    private static void RebuildCharactersScreen()
    {
        if (skinsScreen == null || charactersButtonTemplate == null)
            return;

        BuildCharactersMenu(charactersUi, skinsScreen, charactersButtonTemplate);

        var preferred = screenFirstSelectables.TryGetValue(skinsScreen, out var first) ? first : null;
        if (preferred != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(preferred.gameObject);
        }
    }

    private sealed class CharacterButtonDriver : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        private SkinMenuController controller;
        private Text label;
        private string baseLabel;
        private int companionId;

        public ShadeCharacterDefinition Character { get; private set; }

        public MenuButton Button { get; private set; }

        public void Initialize(SkinMenuController owner, int companion, ShadeCharacterDefinition character, MenuButton button)
        {
            controller = owner;
            companionId = companion;
            Character = character;
            Button = button;
            label = button != null ? button.GetComponentInChildren<Text>(true) : null;
            baseLabel = character != null ? character.DisplayName : string.Empty;
            if (button != null)
            {
                // Same reason as the skin rows: anything but Activate clears the EventSystem
                // selection on submit, and equipping a character stays on this screen.
                button.buttonType = MenuButton.MenuButtonType.Activate;
            }
            controller?.RegisterCharacterButton(this);
            Refresh(ShadeCharacterManager.GetSelected(companionId).Id);
        }

        public void Refresh(ShadeCharacterId selected)
        {
            if (label == null || Character == null)
                return;

            bool equipped = Character.Id == selected;
            label.text = equipped ? baseLabel + "  —  Equipped" : baseLabel;
            label.color = equipped ? SkinEquippedColor : Color.white;
        }

        public void OnSelect(BaseEventData eventData)
        {
            controller?.HandleCharacterFocused(companionId, Character);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != gameObject)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
                return;
            }

            controller?.HandleCharacterFocused(companionId, Character);
        }

        private void OnDestroy()
        {
            controller?.UnregisterCharacterButton(this);
        }
    }

    private sealed class SkinButtonDriver : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        private SkinMenuController controller;
        private Text label;
        private string baseLabel;

        public ShadeSkinDefinition Skin { get; private set; }

        public MenuButton Button { get; private set; }

        public void Initialize(SkinMenuController owner, ShadeSkinDefinition skin, MenuButton button)
        {
            controller = owner;
            Skin = skin;
            Button = button;
            label = button != null ? button.GetComponentInChildren<Text>(true) : null;
            baseLabel = skin != null ? skin.DisplayName : string.Empty;
            // Do not touch OnSubmitPressed here — CreateMenuButton already wired the equip
            // callback onto it; clearing listeners again would strip that out.
            if (button != null)
            {
                // MenuButton.OnSubmit calls ForceDeselect() (clears EventSystem selection,
                // hiding the highlight flair) unless buttonType is Activate. That default
                // assumes submit navigates to a new screen; equipping a skin stays put, so
                // opt out or the flair drops out until the player moves the cursor.
                button.buttonType = MenuButton.MenuButtonType.Activate;
            }
            controller?.RegisterSkinButton(this);
            Refresh(ShadeSkinManager.SelectedSkinId);
        }

        public void Refresh(string selectedId)
        {
            if (label == null || Skin == null)
                return;

            bool equipped = Skin.Matches(selectedId);
            label.text = equipped ? baseLabel + "  —  Equipped" : baseLabel;
            label.color = equipped ? SkinEquippedColor : Color.white;
        }

        public void OnSelect(BaseEventData eventData)
        {
            controller?.HandleSkinFocused(Skin);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            // Move the selection so hovering and the highlighted row stay in sync;
            // OnSelect then drives the preview.
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != gameObject)
            {
                EventSystem.current.SetSelectedGameObject(gameObject);
                return;
            }

            controller?.HandleSkinFocused(Skin);
        }

        private void OnDestroy()
        {
            controller?.UnregisterSkinButton(this);
        }
    }
}
#nullable restore
