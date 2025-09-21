using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020005E3 RID: 1507
public class ToolHudIcon : RadialHudIcon
{
	// Token: 0x140000A6 RID: 166
	// (add) Token: 0x06003571 RID: 13681 RVA: 0x000EC86C File Offset: 0x000EAA6C
	// (remove) Token: 0x06003572 RID: 13682 RVA: 0x000EC8A4 File Offset: 0x000EAAA4
	public event Action Updated;

	// Token: 0x170005F0 RID: 1520
	// (get) Token: 0x06003573 RID: 13683 RVA: 0x000EC8D9 File Offset: 0x000EAAD9
	// (set) Token: 0x06003574 RID: 13684 RVA: 0x000EC8E1 File Offset: 0x000EAAE1
	public ToolItem CurrentTool { get; private set; }

	// Token: 0x06003575 RID: 13685 RVA: 0x000EC8EC File Offset: 0x000EAAEC
	private void Awake()
	{
		ToolItemManager.BoundAttackToolUpdated += this.OnBoundAttackToolUpdated;
		ToolItemManager.BoundAttackToolFailed += this.OnBoundAttackToolFailed;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += base.UpdateDisplay;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "SILK REFRESHED").ReceivedEvent += this.OnSilkSpoolRefreshed;
		if (this.updateOnHealthChange)
		{
			EventRegister.GetRegisterGuaranteed(base.gameObject, "HEALTH UPDATE").ReceivedEvent += this.OnSilkSpoolRefreshed;
			EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO HEALED").ReceivedEvent += this.OnSilkSpoolRefreshed;
			EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO HEALED TO MAX").ReceivedEvent += this.OnSilkSpoolRefreshed;
		}
	}

	// Token: 0x06003576 RID: 13686 RVA: 0x000EC9C8 File Offset: 0x000EABC8
	private void OnDestroy()
	{
		ToolItemManager.BoundAttackToolUpdated -= this.OnBoundAttackToolUpdated;
		ToolItemManager.BoundAttackToolFailed -= this.OnBoundAttackToolFailed;
	}

	// Token: 0x06003577 RID: 13687 RVA: 0x000EC9EC File Offset: 0x000EABEC
	protected override void OnPreUpdateDisplay()
	{
		this.previousTool = this.CurrentTool;
		this.CurrentTool = ToolItemManager.GetBoundAttackTool(this.binding, ToolEquippedReadSource.Hud);
		if (this.CurrentTool)
		{
			this.isPoison = (this.CurrentTool.PoisonDamageTicks > 0 && Gameplay.PoisonPouchTool.IsEquippedHud);
			this.isZap = (this.CurrentTool.ZapDamageTicks > 0 && Gameplay.ZapImbuementTool.IsEquippedHud);
		}
		else
		{
			this.isPoison = false;
			this.isZap = false;
		}
		if (this.skillZapIcon)
		{
			if (!this.gotZapIconColor)
			{
				this.zapIconColor = this.skillZapIcon.color;
				this.gotZapIconColor = true;
			}
			if (this.isZap)
			{
				this.skillZapIcon.gameObject.SetActive(true);
				bool isEmpty = this.GetIsEmpty();
				this.skillZapIcon.color = (isEmpty ? this.zapIconColor.MultiplyElements(this.inactiveColor) : this.zapIconColor);
			}
			else
			{
				this.skillZapIcon.gameObject.SetActive(false);
			}
		}
		Action updated = this.Updated;
		if (updated == null)
		{
			return;
		}
		updated();
	}

	// Token: 0x06003578 RID: 13688 RVA: 0x000ECB10 File Offset: 0x000EAD10
	protected override void SetIconColour(SpriteRenderer icon, Color color)
	{
		bool flag = this.CurrentTool.HudSpriteModified == this.CurrentTool.HudSpriteBase;
		if (flag && this.isPoison && this.CurrentTool.UsePoisonTintRecolour)
		{
			base.SetIconColour(icon, color * Gameplay.PoisonPouchTintColour);
			if (!icon.sharedMaterial.IsKeywordEnabled("RECOLOUR"))
			{
				icon.material.EnableKeyword("RECOLOUR");
			}
		}
		else
		{
			base.SetIconColour(icon, color);
			if (icon.sharedMaterial.IsKeywordEnabled("RECOLOUR"))
			{
				icon.material.DisableKeyword("RECOLOUR");
			}
		}
		if (flag && this.isPoison)
		{
			if (!icon.sharedMaterial.IsKeywordEnabled("CAN_HUESHIFT"))
			{
				icon.material.EnableKeyword("CAN_HUESHIFT");
			}
			icon.material.SetFloat(ToolHudIcon._hueShiftPropId, this.CurrentTool.PoisonHueShift);
			return;
		}
		if (icon.sharedMaterial.IsKeywordEnabled("CAN_HUESHIFT"))
		{
			icon.material.DisableKeyword("CAN_HUESHIFT");
		}
	}

	// Token: 0x06003579 RID: 13689 RVA: 0x000ECC1A File Offset: 0x000EAE1A
	protected override bool GetIsActive()
	{
		return this.CurrentTool;
	}

	// Token: 0x0600357A RID: 13690 RVA: 0x000ECC28 File Offset: 0x000EAE28
	protected override void GetAmounts(out int amountLeft, out int totalCount)
	{
		PlayerData instance = PlayerData.instance;
		if (this.CurrentTool.Type == ToolItemType.Skill)
		{
			amountLeft = 0;
			totalCount = 0;
			return;
		}
		amountLeft = instance.GetToolData(this.CurrentTool.name).AmountLeft;
		totalCount = ToolItemManager.GetToolStorageAmount(this.CurrentTool);
	}

	// Token: 0x0600357B RID: 13691 RVA: 0x000ECC78 File Offset: 0x000EAE78
	protected override bool TryGetHudSprite(out Sprite sprite)
	{
		ToolItemSkill toolItemSkill = this.CurrentTool as ToolItemSkill;
		if (toolItemSkill != null && !this.GetIsEmpty())
		{
			sprite = toolItemSkill.HudGlowSprite;
			if (sprite)
			{
				return true;
			}
		}
		sprite = this.CurrentTool.HudSpriteModified;
		if (sprite)
		{
			return true;
		}
		sprite = this.CurrentTool.InventorySpriteModified;
		return false;
	}

	// Token: 0x0600357C RID: 13692 RVA: 0x000ECCD8 File Offset: 0x000EAED8
	public override bool GetIsEmpty()
	{
		PlayerData instance = PlayerData.instance;
		if (this.CurrentTool.Type != ToolItemType.Skill)
		{
			return this.CurrentTool.IsEmpty && !this.CurrentTool.UsableWhenEmpty;
		}
		return instance.silk < instance.SilkSkillCost;
	}

	// Token: 0x0600357D RID: 13693 RVA: 0x000ECD25 File Offset: 0x000EAF25
	protected override bool HasTargetChanged()
	{
		return this.CurrentTool is ToolItemSkill || this.CurrentTool != this.previousTool;
	}

	// Token: 0x0600357E RID: 13694 RVA: 0x000ECD47 File Offset: 0x000EAF47
	private void OnBoundAttackToolUpdated(AttackToolBinding otherBinding)
	{
		if (otherBinding != this.binding)
		{
			return;
		}
		base.UpdateDisplay();
	}

	// Token: 0x0600357F RID: 13695 RVA: 0x000ECD5C File Offset: 0x000EAF5C
	private void OnBoundAttackToolFailed(AttackToolBinding otherBinding)
	{
		if (otherBinding != this.binding)
		{
			return;
		}
		if (this.animator)
		{
			this.animator.SetTrigger(this.animFailedTrigger);
		}
		this.failedAudioTable.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, false, 1f, null);
	}

	// Token: 0x06003580 RID: 13696 RVA: 0x000ECDB4 File Offset: 0x000EAFB4
	private void OnSilkSpoolRefreshed()
	{
		if (this.CurrentTool && this.CurrentTool.Type == ToolItemType.Skill)
		{
			base.UpdateDisplay();
		}
	}

	// Token: 0x06003581 RID: 13697 RVA: 0x000ECDD7 File Offset: 0x000EAFD7
	public void UpdateDisplayInstant()
	{
		this.previousTool = null;
		base.UpdateDisplay();
	}

	// Token: 0x06003582 RID: 13698 RVA: 0x000ECDE6 File Offset: 0x000EAFE6
	protected override bool TryGetBarColour(out Color color)
	{
		if (!this.CurrentTool)
		{
			return base.TryGetBarColour(out color);
		}
		color = UI.GetToolTypeColor(this.CurrentTool.Type);
		return true;
	}

	// Token: 0x040038D4 RID: 14548
	[Space]
	[SerializeField]
	private AttackToolBinding binding;

	// Token: 0x040038D5 RID: 14549
	[SerializeField]
	private Animator animator;

	// Token: 0x040038D6 RID: 14550
	[SerializeField]
	private RandomAudioClipTable failedAudioTable;

	// Token: 0x040038D7 RID: 14551
	[Space]
	[SerializeField]
	private SpriteRenderer skillZapIcon;

	// Token: 0x040038D8 RID: 14552
	[SerializeField]
	private bool updateOnHealthChange;

	// Token: 0x040038D9 RID: 14553
	private ToolItem previousTool;

	// Token: 0x040038DA RID: 14554
	private bool isPoison;

	// Token: 0x040038DB RID: 14555
	private bool isZap;

	// Token: 0x040038DC RID: 14556
	private bool gotZapIconColor;

	// Token: 0x040038DD RID: 14557
	private Color zapIconColor;

	// Token: 0x040038DE RID: 14558
	private readonly int animFailedTrigger = Animator.StringToHash("Failed");

	// Token: 0x040038DF RID: 14559
	private static readonly int _hueShiftPropId = Shader.PropertyToID("_HueShift");
}
