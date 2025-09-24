using System;
using System.Collections.Generic;
using GlobalEnums;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006D4 RID: 1748
public class MappableKey : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ICancelHandler
{
	// Token: 0x06003F26 RID: 16166 RVA: 0x00116B74 File Offset: 0x00114D74
	private new void Start()
	{
		if (Application.isPlaying)
		{
			this.active = true;
			this.SetupRefs();
		}
	}

	// Token: 0x06003F27 RID: 16167 RVA: 0x00116B8A File Offset: 0x00114D8A
	private new void OnEnable()
	{
		if (Application.isPlaying)
		{
			if (!this.active)
			{
				this.Start();
			}
			this.GetBinding();
			Platform.OnSaveStoreStateChanged += this.OnSaveStoreStateChanged;
		}
	}

	// Token: 0x06003F28 RID: 16168 RVA: 0x00116BB8 File Offset: 0x00114DB8
	protected override void OnDisable()
	{
		base.OnDisable();
		this.ClearSwapCache();
		Platform.OnSaveStoreStateChanged -= this.OnSaveStoreStateChanged;
	}

	// Token: 0x06003F29 RID: 16169 RVA: 0x00116BD7 File Offset: 0x00114DD7
	private void OnSaveStoreStateChanged(bool mounted)
	{
		if (mounted)
		{
			this.ShowCurrentBinding();
		}
	}

	// Token: 0x06003F2A RID: 16170 RVA: 0x00116BE2 File Offset: 0x00114DE2
	public void GetBinding()
	{
		this.currentBinding = this.ih.GetKeyBindingForAction(this.playerAction);
	}

	// Token: 0x06003F2B RID: 16171 RVA: 0x00116BFB File Offset: 0x00114DFB
	private void ClearSwapCache()
	{
		this.bindingToSwap = null;
		this.actionToSwap = null;
	}

	// Token: 0x06003F2C RID: 16172 RVA: 0x00116C0C File Offset: 0x00114E0C
	public void ListenForNewButton()
	{
		this.oldBindings.Clear();
		this.oldBindings.AddRange(this.playerAction.Bindings);
		this.playerAction.ClearBindings();
		this.oldFontSize = this.keymapText.fontSize;
		this.oldAlignment = this.keymapText.alignment;
		this.oldSprite = this.keymapSprite.sprite;
		this.oldText = this.keymapText.text;
		this.keymapSprite.sprite = this.uibs.blankKey;
		this.keymapText.text = Language.Get("KEYBOARD_PRESSKEY", "MainMenu");
		this.keymapText.fontSize = 46;
		this.keymapText.alignment = TextAnchor.MiddleRight;
		this.keymapText.horizontalOverflow = HorizontalWrapMode.Overflow;
		base.interactable = false;
		this.ClearSwapCache();
		this.SetupBindingListenOptions();
		this.isListening = true;
		this.uibs.ListeningForKeyRebind(this);
		this.playerAction.ListenForBinding();
	}

	// Token: 0x06003F2D RID: 16173 RVA: 0x00116D10 File Offset: 0x00114F10
	public void ShowCurrentBinding()
	{
		if (!this.active)
		{
			this.Start();
		}
		if (InputHandler.KeyOrMouseBinding.IsNone(this.currentBinding))
		{
			this.keymapSprite.sprite = this.uibs.blankKey;
			this.keymapText.text = Language.Get("KEYBOARD_UNMAPPED", "MainMenu");
			this.keymapText.fontSize = 46;
			this.keymapText.alignment = TextAnchor.MiddleRight;
			this.keymapText.resizeTextForBestFit = false;
			this.keymapText.horizontalOverflow = HorizontalWrapMode.Overflow;
			this.keymapText.GetComponent<FixVerticalAlign>().AlignText();
		}
		else
		{
			ButtonSkin keyboardSkinFor = this.uibs.GetKeyboardSkinFor(this.playerAction);
			this.keymapSprite.sprite = keyboardSkinFor.sprite;
			this.keymapText.text = keyboardSkinFor.symbol;
			if (keyboardSkinFor.skinType == ButtonSkinType.SQUARE)
			{
				this.keymapText.fontSize = 46;
				this.keymapText.alignment = TextAnchor.MiddleCenter;
				this.keymapText.rectTransform.anchoredPosition = new Vector2(32f, this.keymapText.rectTransform.anchoredPosition.y);
				this.keymapText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 65f);
				this.keymapText.resizeTextForBestFit = true;
				this.keymapText.resizeTextMinSize = 20;
				this.keymapText.resizeTextMaxSize = 46;
				this.keymapText.horizontalOverflow = HorizontalWrapMode.Wrap;
			}
			else if (keyboardSkinFor.skinType == ButtonSkinType.WIDE)
			{
				this.keymapText.fontSize = 40;
				this.keymapText.alignment = TextAnchor.MiddleCenter;
				this.keymapText.rectTransform.anchoredPosition = new Vector2(4f, this.keymapText.rectTransform.anchoredPosition.y);
				this.keymapText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 137f);
				this.keymapText.resizeTextForBestFit = false;
				this.keymapText.horizontalOverflow = HorizontalWrapMode.Wrap;
			}
			else
			{
				this.keymapText.alignment = this.uibs.labelAlignment;
			}
			if (this.keymapSprite.sprite == null)
			{
				this.keymapSprite.sprite = this.uibs.blankKey;
			}
			this.keymapText.GetComponent<FixVerticalAlign>().AlignTextKeymap();
		}
		base.interactable = true;
	}

	// Token: 0x06003F2E RID: 16174 RVA: 0x00116F5C File Offset: 0x0011515C
	public void AbortRebind()
	{
		if (this.isListening)
		{
			foreach (BindingSource binding in this.oldBindings)
			{
				this.playerAction.AddBinding(binding);
			}
			this.oldBindings.Clear();
			this.keymapText.text = this.oldText;
			this.keymapText.fontSize = this.oldFontSize;
			this.keymapText.alignment = this.oldAlignment;
			this.keymapSprite.sprite = this.oldSprite;
			this.keymapText.GetComponent<FixVerticalAlign>().AlignTextKeymap();
			base.interactable = true;
			this.isListening = false;
			this.ClearSwapCache();
		}
	}

	// Token: 0x06003F2F RID: 16175 RVA: 0x00117034 File Offset: 0x00115234
	public void StopActionListening()
	{
		this.playerAction.StopListeningForBinding();
		this.ClearSwapCache();
	}

	// Token: 0x06003F30 RID: 16176 RVA: 0x00117048 File Offset: 0x00115248
	public bool OnBindingFound(PlayerAction action, BindingSource binding)
	{
		if (!(binding is MouseBindingSource))
		{
			KeyBindingSource keyBindingSource = binding as KeyBindingSource;
			if (keyBindingSource == null || this.unmappableKeys.Contains(keyBindingSource))
			{
				this.uibs.FinishedListeningForKey();
				action.StopListeningForBinding();
				this.AbortRebind();
				return false;
			}
		}
		if (binding != null)
		{
			foreach (PlayerAction playerAction in this.ih.MappableKeyboardActions)
			{
				if (playerAction != this.playerAction && playerAction.Bindings.Contains(binding))
				{
					this.actionToSwap = playerAction;
					break;
				}
			}
			if (this.actionToSwap != null)
			{
				foreach (BindingSource bindingSource in this.oldBindings)
				{
					if (bindingSource.BindingSourceType == binding.BindingSourceType || bindingSource.BindingSourceType == BindingSourceType.KeyBindingSource || bindingSource.BindingSourceType == BindingSourceType.MouseBindingSource)
					{
						this.bindingToSwap = bindingSource;
						break;
					}
				}
				if (this.bindingToSwap == null)
				{
					foreach (BindingSource bindingSource2 in action.Bindings)
					{
						if (bindingSource2.BindingSourceType == binding.BindingSourceType || bindingSource2.BindingSourceType == BindingSourceType.KeyBindingSource || bindingSource2.BindingSourceType == BindingSourceType.MouseBindingSource)
						{
							this.bindingToSwap = bindingSource2;
							break;
						}
					}
				}
			}
		}
		return true;
	}

	// Token: 0x06003F31 RID: 16177 RVA: 0x001171EC File Offset: 0x001153EC
	public void OnBindingAdded(PlayerAction action, BindingSource binding)
	{
		this.oldBindings.RemoveAll((BindingSource o) => o == null || o.DeviceClass == InputDeviceClass.Keyboard || o.DeviceClass == InputDeviceClass.Mouse);
		foreach (BindingSource binding2 in this.oldBindings)
		{
			this.playerAction.AddBinding(binding2);
		}
		this.oldBindings.Clear();
		if (this.actionToSwap != null && this.bindingToSwap != null)
		{
			this.actionToSwap.AddBinding(this.bindingToSwap);
		}
		this.isListening = false;
		base.interactable = true;
		this.uibs.FinishedListeningForKey();
		this.ClearSwapCache();
	}

	// Token: 0x06003F32 RID: 16178 RVA: 0x001172C4 File Offset: 0x001154C4
	public void OnBindingRejected(PlayerAction action, BindingSource binding, BindingSourceRejectionType rejection)
	{
		if (rejection == BindingSourceRejectionType.DuplicateBindingOnAction)
		{
			this.uibs.FinishedListeningForKey();
			this.AbortRebind();
			action.StopListeningForBinding();
			this.isListening = false;
		}
		else if (rejection != BindingSourceRejectionType.DuplicateBindingOnActionSet)
		{
			this.uibs.FinishedListeningForKey();
			this.AbortRebind();
			action.StopListeningForBinding();
			this.isListening = false;
		}
		this.ClearSwapCache();
	}

	// Token: 0x06003F33 RID: 16179 RVA: 0x0011731D File Offset: 0x0011551D
	public new void OnSubmit(BaseEventData eventData)
	{
		if (!this.isListening)
		{
			this.ListenForNewButton();
		}
	}

	// Token: 0x06003F34 RID: 16180 RVA: 0x0011732D File Offset: 0x0011552D
	public new void OnPointerClick(PointerEventData eventData)
	{
		this.OnSubmit(eventData);
	}

	// Token: 0x06003F35 RID: 16181 RVA: 0x00117336 File Offset: 0x00115536
	public new void OnCancel(BaseEventData eventData)
	{
		if (this.isListening)
		{
			this.StopListeningForNewKey();
			return;
		}
		base.OnCancel(eventData);
	}

	// Token: 0x06003F36 RID: 16182 RVA: 0x0011734E File Offset: 0x0011554E
	private void StopListeningForNewKey()
	{
		this.uibs.FinishedListeningForKey();
		this.StopActionListening();
		this.AbortRebind();
	}

	// Token: 0x06003F37 RID: 16183 RVA: 0x00117368 File Offset: 0x00115568
	private void SetupUnmappableKeys()
	{
		this.unmappableKeys = new List<KeyBindingSource>();
		this.unmappableKeys.Add(new KeyBindingSource(new Key[]
		{
			Key.Escape
		}));
		this.unmappableKeys.Add(new KeyBindingSource(new Key[]
		{
			Key.Return
		}));
		this.unmappableKeys.Add(new KeyBindingSource(new Key[]
		{
			Key.Numlock
		}));
		this.unmappableKeys.Add(new KeyBindingSource(new Key[]
		{
			Key.LeftCommand
		}));
		this.unmappableKeys.Add(new KeyBindingSource(new Key[]
		{
			Key.RightCommand
		}));
	}

	// Token: 0x06003F38 RID: 16184 RVA: 0x00117408 File Offset: 0x00115608
	private void SetupBindingListenOptions()
	{
		BindingListenOptions bindingListenOptions = new BindingListenOptions();
		bindingListenOptions.IncludeControllers = true;
		bindingListenOptions.IncludeNonStandardControls = false;
		bindingListenOptions.IncludeMouseButtons = true;
		bindingListenOptions.IncludeKeys = true;
		bindingListenOptions.IncludeModifiersAsFirstClassKeys = true;
		bindingListenOptions.IncludeUnknownControllers = false;
		bindingListenOptions.MaxAllowedBindingsPerType = 1U;
		bindingListenOptions.OnBindingFound = new Func<PlayerAction, BindingSource, bool>(this.OnBindingFound);
		bindingListenOptions.OnBindingAdded = new Action<PlayerAction, BindingSource>(this.OnBindingAdded);
		bindingListenOptions.OnBindingRejected = new Action<PlayerAction, BindingSource, BindingSourceRejectionType>(this.OnBindingRejected);
		bindingListenOptions.UnsetDuplicateBindingsOnSet = true;
		this.ih.inputActions.ListenOptions = bindingListenOptions;
	}

	// Token: 0x06003F39 RID: 16185 RVA: 0x0011749C File Offset: 0x0011569C
	private void SetupRefs()
	{
		this.gm = GameManager.instance;
		this.ui = this.gm.ui;
		this.uibs = this.ui.uiButtonSkins;
		this.ih = this.gm.inputHandler;
		this.playerAction = this.ih.ActionButtonToPlayerAction(this.actionButtonType);
		base.HookUpAudioPlayer();
		base.HookUpEventTrigger();
		this.SetupUnmappableKeys();
	}

	// Token: 0x040040E6 RID: 16614
	private GameManager gm;

	// Token: 0x040040E7 RID: 16615
	private InputHandler ih;

	// Token: 0x040040E8 RID: 16616
	private UIManager ui;

	// Token: 0x040040E9 RID: 16617
	private UIButtonSkins uibs;

	// Token: 0x040040EA RID: 16618
	private PlayerAction playerAction;

	// Token: 0x040040EB RID: 16619
	private bool active;

	// Token: 0x040040EC RID: 16620
	private bool isListening;

	// Token: 0x040040ED RID: 16621
	private int oldFontSize;

	// Token: 0x040040EE RID: 16622
	private TextAnchor oldAlignment;

	// Token: 0x040040EF RID: 16623
	private Sprite oldSprite;

	// Token: 0x040040F0 RID: 16624
	private string oldText;

	// Token: 0x040040F1 RID: 16625
	private InputHandler.KeyOrMouseBinding currentBinding;

	// Token: 0x040040F2 RID: 16626
	private PlayerAction actionToSwap;

	// Token: 0x040040F3 RID: 16627
	private BindingSource bindingToSwap;

	// Token: 0x040040F4 RID: 16628
	private List<KeyBindingSource> unmappableKeys;

	// Token: 0x040040F5 RID: 16629
	private const float sqrX = 32f;

	// Token: 0x040040F6 RID: 16630
	private const float sqrWidth = 65f;

	// Token: 0x040040F7 RID: 16631
	private const bool sqrBestFit = true;

	// Token: 0x040040F8 RID: 16632
	private const int sqrFontSize = 46;

	// Token: 0x040040F9 RID: 16633
	private const int sqrMinFont = 20;

	// Token: 0x040040FA RID: 16634
	private const int sqrMaxFont = 46;

	// Token: 0x040040FB RID: 16635
	private const HorizontalWrapMode sqrHOverflow = HorizontalWrapMode.Wrap;

	// Token: 0x040040FC RID: 16636
	private const TextAnchor sqrAlignment = TextAnchor.MiddleCenter;

	// Token: 0x040040FD RID: 16637
	private const float wideX = 4f;

	// Token: 0x040040FE RID: 16638
	private const float wideWidth = 137f;

	// Token: 0x040040FF RID: 16639
	private const bool wideBestFit = false;

	// Token: 0x04004100 RID: 16640
	private const int wideFontSize = 40;

	// Token: 0x04004101 RID: 16641
	private const HorizontalWrapMode wideHOverflow = HorizontalWrapMode.Wrap;

	// Token: 0x04004102 RID: 16642
	private const TextAnchor wideAlignment = TextAnchor.MiddleCenter;

	// Token: 0x04004103 RID: 16643
	private const bool blankBestFit = false;

	// Token: 0x04004104 RID: 16644
	private const int blankFontSize = 46;

	// Token: 0x04004105 RID: 16645
	private const HorizontalWrapMode blankOverflow = HorizontalWrapMode.Overflow;

	// Token: 0x04004106 RID: 16646
	private const TextAnchor blankAlignment = TextAnchor.MiddleRight;

	// Token: 0x04004107 RID: 16647
	[Space(6f)]
	[Header("Button Mapping")]
	public HeroActionButton actionButtonType;

	// Token: 0x04004108 RID: 16648
	public Text keymapText;

	// Token: 0x04004109 RID: 16649
	public Image keymapSprite;

	// Token: 0x0400410A RID: 16650
	private List<BindingSource> oldBindings = new List<BindingSource>();
}
