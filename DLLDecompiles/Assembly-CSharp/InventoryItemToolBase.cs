using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020006A7 RID: 1703
public abstract class InventoryItemToolBase : InventoryItemUpdateable
{
	// Token: 0x170006F3 RID: 1779
	// (get) Token: 0x06003CD2 RID: 15570
	public abstract Sprite Sprite { get; }

	// Token: 0x170006F4 RID: 1780
	// (get) Token: 0x06003CD3 RID: 15571 RVA: 0x0010AF55 File Offset: 0x00109155
	public virtual Color SpriteTint
	{
		get
		{
			return Color.white;
		}
	}

	// Token: 0x170006F5 RID: 1781
	// (get) Token: 0x06003CD4 RID: 15572
	public abstract ToolItem ItemData { get; }

	// Token: 0x06003CD5 RID: 15573 RVA: 0x0010AF5C File Offset: 0x0010915C
	protected override void Awake()
	{
		base.Awake();
		this.manager = base.GetComponentInParent<InventoryItemToolManager>();
		this.extraDesc = base.GetComponent<InventoryItemExtraDescription>();
		if (this.extraDesc)
		{
			this.extraDesc.ActivatedDesc += delegate(GameObject obj)
			{
				ToolItem itemData = this.ItemData;
				if (itemData)
				{
					itemData.SetupExtraDescription(obj);
				}
			};
		}
		if (this.poisonEquipEffect)
		{
			this.poisonEquipEffect.SetActive(false);
		}
		if (this.zapEquipEffect)
		{
			this.zapEquipEffect.SetActive(false);
		}
		if (this.zapIcon)
		{
			this.zapIcon.sprite = null;
		}
	}

	// Token: 0x06003CD6 RID: 15574 RVA: 0x0010AFF8 File Offset: 0x001091F8
	protected virtual void Update()
	{
		if (this.holdTimerLeft > 0f)
		{
			this.holdTimerLeft -= Time.unscaledDeltaTime;
			if (this.holdTimerLeft <= 0f)
			{
				if (!this.manager.CanChangeEquips(this.ItemData.Type, InventoryItemToolManager.CanChangeEquipsTypes.Reload))
				{
					return;
				}
				if (this.CanReload())
				{
					this.SetReloading(true);
				}
				else
				{
					this.ReportFailure();
				}
			}
		}
		if (!this.isReloading)
		{
			return;
		}
		if (this.reloadTimeElapsed < 0.025f)
		{
			this.reloadTimeElapsed += Time.unscaledDeltaTime;
			return;
		}
		if (this.ReloadSingle())
		{
			this.manager.RefreshTools();
		}
		this.reloadTimeElapsed %= 0.025f;
	}

	// Token: 0x06003CD7 RID: 15575 RVA: 0x0010B0B0 File Offset: 0x001092B0
	protected void ItemDataUpdated()
	{
		ToolItem itemData = this.ItemData;
		if (this.extraDesc)
		{
			this.extraDesc.ExtraDescPrefab = (itemData ? itemData.ExtraDescriptionSection : null);
		}
		if (this.zapIcon)
		{
			this.zapIcon.sprite = null;
		}
		this.wasPoison = false;
		this.wasZap = false;
	}

	// Token: 0x06003CD8 RID: 15576
	protected abstract bool IsToolEquipped(ToolItem toolItem);

	// Token: 0x06003CD9 RID: 15577 RVA: 0x0010B114 File Offset: 0x00109314
	public Color UpdateGetIconColour(SpriteRenderer applyToSprite, Color sourceColor, bool spawnEffects)
	{
		ToolItem itemData = this.ItemData;
		if (!itemData)
		{
			this.wasPoison = false;
			this.wasZap = false;
			return sourceColor;
		}
		bool flag;
		bool flag2;
		if (itemData && this.IsToolEquipped(itemData))
		{
			flag = (itemData.PoisonDamageTicks > 0 && this.IsToolEquipped(Gameplay.PoisonPouchTool));
			flag2 = (itemData.ZapDamageTicks > 0 && this.IsToolEquipped(Gameplay.ZapImbuementTool));
		}
		else
		{
			flag = false;
			flag2 = false;
		}
		if (spawnEffects)
		{
			bool flag3 = Time.frameCount != this.lastEffectFrame;
			this.lastEffectFrame = Time.frameCount;
			if (flag && !this.wasPoison && this.poisonEquipEffect)
			{
				if (flag3)
				{
					this.poisonEquipEffect.SetActive(false);
				}
				this.poisonEquipEffect.SetActive(true);
			}
			if (flag2 && !this.wasZap && this.zapEquipEffect)
			{
				if (flag3)
				{
					this.zapEquipEffect.SetActive(false);
				}
				this.zapEquipEffect.SetActive(true);
			}
		}
		this.wasPoison = flag;
		this.wasZap = flag2;
		bool flag4 = itemData.GetInventorySprite((itemData.PoisonDamageTicks > 0 && this.IsToolEquipped(Gameplay.PoisonPouchTool)) ? ToolItem.IconVariants.Poison : ToolItem.IconVariants.Default) == itemData.InventorySpriteBase;
		Color result;
		if (flag4 && flag && itemData.UsePoisonTintRecolour)
		{
			result = sourceColor * Gameplay.PoisonPouchTintColour;
			if (!applyToSprite.sharedMaterial.IsKeywordEnabled("RECOLOUR"))
			{
				applyToSprite.material.EnableKeyword("RECOLOUR");
			}
		}
		else
		{
			result = sourceColor;
			if (applyToSprite.sharedMaterial.IsKeywordEnabled("RECOLOUR"))
			{
				applyToSprite.material.DisableKeyword("RECOLOUR");
			}
		}
		if (flag4 && flag)
		{
			if (!applyToSprite.sharedMaterial.IsKeywordEnabled("CAN_HUESHIFT"))
			{
				applyToSprite.material.EnableKeyword("CAN_HUESHIFT");
			}
			applyToSprite.material.SetFloat(InventoryItemToolBase._hueShiftPropId, itemData.PoisonHueShift);
		}
		else if (applyToSprite.sharedMaterial.IsKeywordEnabled("CAN_HUESHIFT"))
		{
			applyToSprite.material.DisableKeyword("CAN_HUESHIFT");
		}
		if (flag2)
		{
			ToolItemSkill toolItemSkill = itemData as ToolItemSkill;
			if (toolItemSkill != null)
			{
				if (this.zapIcon)
				{
					this.zapIcon.sprite = toolItemSkill.InvGlowSprite;
					return result;
				}
				return result;
			}
		}
		if (this.zapIcon)
		{
			this.zapIcon.sprite = null;
		}
		return result;
	}

	// Token: 0x06003CDA RID: 15578 RVA: 0x0010B356 File Offset: 0x00109556
	public override bool Submit()
	{
		return this.DoPress();
	}

	// Token: 0x06003CDB RID: 15579
	protected abstract bool DoPress();

	// Token: 0x06003CDC RID: 15580 RVA: 0x0010B360 File Offset: 0x00109560
	public override bool Extra()
	{
		ToolItem itemData = this.ItemData;
		if (itemData && itemData.ReplenishUsage == ToolItem.ReplenishUsages.OneForOne)
		{
			this.holdTimerLeft = 0.3f;
			return true;
		}
		if (!itemData || !itemData.DisplayTogglePrompt)
		{
			return base.Extra();
		}
		return this.DoExtraPress();
	}

	// Token: 0x06003CDD RID: 15581 RVA: 0x0010B3AF File Offset: 0x001095AF
	public override bool ExtraReleased()
	{
		if (this.holdTimerLeft > 0f)
		{
			if (!this.DoExtraPress())
			{
				Debug.LogError("Release press could not be performed", this);
			}
		}
		else if (this.isReloading)
		{
			this.SetReloading(false);
		}
		this.holdTimerLeft = 0f;
		return true;
	}

	// Token: 0x06003CDE RID: 15582 RVA: 0x0010B3F0 File Offset: 0x001095F0
	private bool DoExtraPress()
	{
		ToolItem itemData = this.ItemData;
		bool flag;
		if (!itemData.CanToggle || !this.manager.CanChangeEquips(itemData.Type, InventoryItemToolManager.CanChangeEquipsTypes.Transform) || !itemData.DoToggle(out flag))
		{
			return base.Extra();
		}
		this.UpdateDisplay();
		this.manager.RefreshTools();
		if (!flag)
		{
			return true;
		}
		if (this.reloadFailAnimator)
		{
			this.reloadFailAnimator.SetTrigger(InventoryItemToolBase._changeProp);
		}
		if (this.changeEffectPrefab)
		{
			this.changeEffectPrefab.Spawn().transform.SetPosition2D(base.transform.position);
		}
		itemData.PlayToggleAudio(this.reloadAudioSource);
		return true;
	}

	// Token: 0x06003CDF RID: 15583 RVA: 0x0010B4A8 File Offset: 0x001096A8
	private void SetReloading(bool value)
	{
		if (this.isReloading == value)
		{
			return;
		}
		this.isReloading = value;
		if (this.isReloading)
		{
			if (this.reloadShakeParent)
			{
				this.reloadShakeParent.StartJitter();
			}
			this.ItemData.StartedReloading(this.reloadAudioSource);
			return;
		}
		if (this.reloadShakeParent)
		{
			this.reloadShakeParent.StopJitter();
		}
		this.ItemData.StoppedReloading(this.reloadAudioSource, !this.CanReload());
	}

	// Token: 0x06003CE0 RID: 15584 RVA: 0x0010B52C File Offset: 0x0010972C
	private bool CanReload()
	{
		ToolItem itemData = this.ItemData;
		return itemData && itemData.CanReload();
	}

	// Token: 0x06003CE1 RID: 15585 RVA: 0x0010B550 File Offset: 0x00109750
	private bool ReloadSingle()
	{
		ToolItem itemData = this.ItemData;
		if (!itemData)
		{
			return false;
		}
		itemData.ReloadSingle();
		if (!this.CanReload())
		{
			this.SetReloading(false);
		}
		return true;
	}

	// Token: 0x06003CE2 RID: 15586 RVA: 0x0010B584 File Offset: 0x00109784
	public void ReportFailure()
	{
		if (this.reloadFailAnimator)
		{
			this.reloadFailAnimator.SetTrigger(InventoryItemToolBase._failedProp);
		}
		this.failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, false, 1f, null);
	}

	// Token: 0x04003E82 RID: 16002
	private const float HOLD_PRESS_TIME = 0.3f;

	// Token: 0x04003E83 RID: 16003
	private const float RELOAD_TIME = 0.025f;

	// Token: 0x04003E84 RID: 16004
	[Header("Tool Base")]
	[SerializeField]
	private JitterSelf reloadShakeParent;

	// Token: 0x04003E85 RID: 16005
	[SerializeField]
	private Animator reloadFailAnimator;

	// Token: 0x04003E86 RID: 16006
	[SerializeField]
	private RandomAudioClipTable failedAudioTable;

	// Token: 0x04003E87 RID: 16007
	[SerializeField]
	private GameObject changeEffectPrefab;

	// Token: 0x04003E88 RID: 16008
	[SerializeField]
	private AudioSource reloadAudioSource;

	// Token: 0x04003E89 RID: 16009
	[SerializeField]
	private GameObject poisonEquipEffect;

	// Token: 0x04003E8A RID: 16010
	[SerializeField]
	private GameObject zapEquipEffect;

	// Token: 0x04003E8B RID: 16011
	[SerializeField]
	private SpriteRenderer zapIcon;

	// Token: 0x04003E8C RID: 16012
	private float holdTimerLeft;

	// Token: 0x04003E8D RID: 16013
	private bool isReloading;

	// Token: 0x04003E8E RID: 16014
	private float reloadTimeElapsed;

	// Token: 0x04003E8F RID: 16015
	[NonSerialized]
	private InventoryItemToolManager manager;

	// Token: 0x04003E90 RID: 16016
	private InventoryItemExtraDescription extraDesc;

	// Token: 0x04003E91 RID: 16017
	private bool wasPoison;

	// Token: 0x04003E92 RID: 16018
	private bool wasZap;

	// Token: 0x04003E93 RID: 16019
	private int lastEffectFrame;

	// Token: 0x04003E94 RID: 16020
	private static readonly int _failedProp = Animator.StringToHash("Failed");

	// Token: 0x04003E95 RID: 16021
	private static readonly int _changeProp = Animator.StringToHash("Change");

	// Token: 0x04003E96 RID: 16022
	private static readonly int _hueShiftPropId = Shader.PropertyToID("_HueShift");
}
