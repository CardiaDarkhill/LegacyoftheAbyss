using System;
using GlobalEnums;
using TMProOld;
using UnityEngine;

// Token: 0x02000606 RID: 1542
[RequireComponent(typeof(SpriteRenderer))]
public abstract class ActionButtonIconBase : MonoBehaviour
{
	// Token: 0x140000B3 RID: 179
	// (add) Token: 0x0600370B RID: 14091 RVA: 0x000F2C68 File Offset: 0x000F0E68
	// (remove) Token: 0x0600370C RID: 14092 RVA: 0x000F2CA0 File Offset: 0x000F0EA0
	public event ActionButtonIconBase.IconUpdateEvent OnIconUpdate;

	// Token: 0x17000654 RID: 1620
	// (get) Token: 0x0600370D RID: 14093
	public abstract HeroActionButton Action { get; }

	// Token: 0x0600370E RID: 14094 RVA: 0x000F2CD8 File Offset: 0x000F0ED8
	private void Awake()
	{
		this.hasAwaked = true;
		this.sr = base.GetComponent<SpriteRenderer>();
		this.uibs = UIManager.instance.uiButtonSkins;
		if (this.label)
		{
			this.initialAutoSize = this.label.enableAutoSizing;
		}
	}

	// Token: 0x0600370F RID: 14095 RVA: 0x000F2D28 File Offset: 0x000F0F28
	protected virtual void OnEnable()
	{
		if (this.ih == null)
		{
			this.ih = GameManager.instance.inputHandler;
		}
		if (this.ih != null)
		{
			this.ih.RefreshActiveControllerEvent += this.RefreshController;
		}
		this.RefreshButtonIcon();
	}

	// Token: 0x06003710 RID: 14096 RVA: 0x000F2D7E File Offset: 0x000F0F7E
	protected virtual void OnDisable()
	{
		if (this.ih != null)
		{
			this.ih.RefreshActiveControllerEvent -= this.RefreshController;
		}
	}

	// Token: 0x06003711 RID: 14097 RVA: 0x000F2DA8 File Offset: 0x000F0FA8
	protected void GetButtonIcon(HeroActionButton actionButton)
	{
		if (!this.hasAwaked)
		{
			this.Awake();
		}
		ButtonSkin buttonSkinFor = this.uibs.GetButtonSkinFor(actionButton);
		if (buttonSkinFor == null)
		{
			Debug.LogError("Couldn't get button skin for " + actionButton.ToString(), this);
			return;
		}
		this.sr.sprite = buttonSkinFor.sprite;
		if (this.textContainer != null)
		{
			if (buttonSkinFor.skinType == ButtonSkinType.BLANK)
			{
				this.textContainer.width = this.blnkWidth;
				this.textContainer.height = this.blnkHeight;
			}
			else if (buttonSkinFor.skinType == ButtonSkinType.SQUARE)
			{
				this.textContainer.width = this.sqrWidth;
				this.textContainer.height = this.sqrHeight;
			}
			else if (buttonSkinFor.skinType == ButtonSkinType.WIDE)
			{
				this.textContainer.width = this.wideWidth;
				this.textContainer.height = this.wideHeight;
			}
		}
		if (this.label != null)
		{
			if (buttonSkinFor.skinType == ButtonSkinType.BLANK)
			{
				this.label.fontSizeMin = this.blnkFontMin;
				this.label.fontSizeMax = this.blnkFontMax;
			}
			else if (buttonSkinFor.skinType == ButtonSkinType.SQUARE)
			{
				this.label.fontSizeMin = this.sqrFontMin;
				this.label.fontSizeMax = this.sqrFontMax;
			}
			else if (buttonSkinFor.skinType == ButtonSkinType.WIDE)
			{
				this.label.fontSizeMin = this.wideFontMin;
				this.label.fontSizeMax = this.wideFontMax;
				this.label.enableAutoSizing = true;
			}
			this.label.text = buttonSkinFor.symbol;
		}
		if (this.OnIconUpdate != null)
		{
			this.OnIconUpdate();
		}
	}

	// Token: 0x06003712 RID: 14098 RVA: 0x000F2F5C File Offset: 0x000F115C
	public void RefreshController()
	{
		if (this.liveUpdate)
		{
			this.RefreshButtonIcon();
		}
	}

	// Token: 0x06003713 RID: 14099 RVA: 0x000F2F6C File Offset: 0x000F116C
	public void RefreshButtonIcon()
	{
		this.GetButtonIcon(this.Action);
	}

	// Token: 0x040039D3 RID: 14803
	[Header("Optional")]
	[Tooltip("This will update the button skin to reflect the currently active controller at all times.")]
	public bool liveUpdate;

	// Token: 0x040039D4 RID: 14804
	public TextMeshPro label;

	// Token: 0x040039D5 RID: 14805
	public TextContainer textContainer;

	// Token: 0x040039D6 RID: 14806
	protected SpriteRenderer sr;

	// Token: 0x040039D7 RID: 14807
	private UIButtonSkins uibs;

	// Token: 0x040039D8 RID: 14808
	private InputHandler ih;

	// Token: 0x040039D9 RID: 14809
	private float blnkWidth = 1.685f;

	// Token: 0x040039DA RID: 14810
	private float blnkHeight = 0.6f;

	// Token: 0x040039DB RID: 14811
	private float blnkFontMax = 9.5f;

	// Token: 0x040039DC RID: 14812
	private float blnkFontMin = 4f;

	// Token: 0x040039DD RID: 14813
	private float sqrWidth = 0.7f;

	// Token: 0x040039DE RID: 14814
	private float sqrHeight = 0.8f;

	// Token: 0x040039DF RID: 14815
	private float sqrFontMax = 5.07f;

	// Token: 0x040039E0 RID: 14816
	private float sqrFontMin = 3.35f;

	// Token: 0x040039E1 RID: 14817
	private float wideWidth = 1.4f;

	// Token: 0x040039E2 RID: 14818
	private float wideHeight = 0.7f;

	// Token: 0x040039E3 RID: 14819
	private float wideFontMax = 5.07f;

	// Token: 0x040039E4 RID: 14820
	private float wideFontMin = 3.35f;

	// Token: 0x040039E5 RID: 14821
	private bool hasAwaked;

	// Token: 0x040039E6 RID: 14822
	protected bool initialAutoSize;

	// Token: 0x0200190F RID: 6415
	// (Invoke) Token: 0x06009304 RID: 37636
	public delegate void IconUpdateEvent();
}
