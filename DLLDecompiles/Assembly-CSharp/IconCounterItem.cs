using System;
using UnityEngine;

// Token: 0x0200066B RID: 1643
public class IconCounterItem : MonoBehaviour
{
	// Token: 0x170006A8 RID: 1704
	// (set) Token: 0x06003AFA RID: 15098 RVA: 0x00103C79 File Offset: 0x00101E79
	public Sprite Sprite
	{
		set
		{
			if (this.spriteRenderer)
			{
				this.spriteRenderer.sprite = value;
			}
		}
	}

	// Token: 0x170006A9 RID: 1705
	// (set) Token: 0x06003AFB RID: 15099 RVA: 0x00103C94 File Offset: 0x00101E94
	public Vector3 Scale
	{
		set
		{
			if (this.spriteRenderer)
			{
				this.spriteRenderer.transform.localScale = value.MultiplyElements(this.initialScale);
				return;
			}
			base.transform.localScale = value.MultiplyElements(this.initialScale);
		}
	}

	// Token: 0x170006AA RID: 1706
	// (get) Token: 0x06003AFC RID: 15100 RVA: 0x00103CE2 File Offset: 0x00101EE2
	// (set) Token: 0x06003AFD RID: 15101 RVA: 0x00103CEA File Offset: 0x00101EEA
	public Color TintColor
	{
		get
		{
			return this.tintColor;
		}
		set
		{
			this.tintColor = value;
			if (this.spriteRenderer)
			{
				this.spriteRenderer.color = this.baseColor.MultiplyElements(this.tintColor);
			}
		}
	}

	// Token: 0x06003AFE RID: 15102 RVA: 0x00103D1C File Offset: 0x00101F1C
	private void Awake()
	{
		this.initialScale = (this.spriteRenderer ? this.spriteRenderer.transform.localScale : base.transform.localScale);
	}

	// Token: 0x06003AFF RID: 15103 RVA: 0x00103D50 File Offset: 0x00101F50
	private void OnEnable()
	{
		if (this.customCondition.IsDefined && this.customCondition.IsFulfilled)
		{
			this.SetFilled(true);
			return;
		}
		if (this.orItemCondition)
		{
			this.SetFilled(this.orItemCondition.CollectedAmount > 0);
			return;
		}
		if (this.customCondition.IsDefined)
		{
			this.SetFilled(false);
		}
	}

	// Token: 0x06003B00 RID: 15104 RVA: 0x00103DB8 File Offset: 0x00101FB8
	public void SetFilled(bool value)
	{
		if (!this.spriteRenderer)
		{
			return;
		}
		IconCounterItem.DisplayState displayState;
		if (value)
		{
			displayState = this.activeState;
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}
		else
		{
			if (this.inactiveDisable)
			{
				base.gameObject.SetActive(false);
				return;
			}
			displayState = this.inactiveState;
		}
		this.baseColor = displayState.Color;
		this.TintColor = this.TintColor;
		if (displayState.Sprite)
		{
			this.spriteRenderer.sprite = displayState.Sprite;
		}
		if (displayState.Material)
		{
			this.spriteRenderer.sharedMaterial = displayState.Material;
		}
		if (!this.setFlashColour)
		{
			return;
		}
		if (this.propBlock == null)
		{
			this.propBlock = new MaterialPropertyBlock();
		}
		this.propBlock.Clear();
		this.spriteRenderer.GetPropertyBlock(this.propBlock);
		this.propBlock.SetColor(IconCounterItem._flashColor, displayState.Color);
		this.spriteRenderer.SetPropertyBlock(this.propBlock);
	}

	// Token: 0x04003D47 RID: 15687
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04003D48 RID: 15688
	[SerializeField]
	private bool setFlashColour;

	// Token: 0x04003D49 RID: 15689
	[SerializeField]
	private IconCounterItem.DisplayState activeState;

	// Token: 0x04003D4A RID: 15690
	[SerializeField]
	private IconCounterItem.DisplayState inactiveState;

	// Token: 0x04003D4B RID: 15691
	[SerializeField]
	private bool inactiveDisable;

	// Token: 0x04003D4C RID: 15692
	[SerializeField]
	private PlayerDataTest customCondition;

	// Token: 0x04003D4D RID: 15693
	[SerializeField]
	private CollectableItem orItemCondition;

	// Token: 0x04003D4E RID: 15694
	private Color baseColor = Color.white;

	// Token: 0x04003D4F RID: 15695
	private Color tintColor = Color.white;

	// Token: 0x04003D50 RID: 15696
	private Vector3 initialScale;

	// Token: 0x04003D51 RID: 15697
	private MaterialPropertyBlock propBlock;

	// Token: 0x04003D52 RID: 15698
	private static readonly int _flashColor = Shader.PropertyToID("_FlashColor");

	// Token: 0x0200197D RID: 6525
	[Serializable]
	private struct DisplayState
	{
		// Token: 0x04009602 RID: 38402
		public Color Color;

		// Token: 0x04009603 RID: 38403
		public Sprite Sprite;

		// Token: 0x04009604 RID: 38404
		public Material Material;
	}
}
