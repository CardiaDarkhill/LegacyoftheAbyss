using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005F3 RID: 1523
[CreateAssetMenu(fileName = "New Tool", menuName = "Hornet/Tool Item (States - Liquid)")]
public class ToolItemStatesLiquid : ToolItemStates, IToolExtraNew
{
	// Token: 0x17000638 RID: 1592
	// (get) Token: 0x06003659 RID: 13913 RVA: 0x000F0313 File Offset: 0x000EE513
	public int RefillsMax
	{
		get
		{
			return this.refillsMax;
		}
	}

	// Token: 0x17000639 RID: 1593
	// (get) Token: 0x0600365A RID: 13914 RVA: 0x000F031B File Offset: 0x000EE51B
	public bool HasInfiniteRefills
	{
		get
		{
			return !string.IsNullOrEmpty(this.infiniteRefillsBool) && PlayerData.instance.GetVariable(this.infiniteRefillsBool);
		}
	}

	// Token: 0x1700063A RID: 1594
	// (get) Token: 0x0600365B RID: 13915 RVA: 0x000F033C File Offset: 0x000EE53C
	public Color LiquidColor
	{
		get
		{
			return this.liquidColor;
		}
	}

	// Token: 0x1700063B RID: 1595
	// (get) Token: 0x0600365C RID: 13916 RVA: 0x000F0344 File Offset: 0x000EE544
	// (set) Token: 0x0600365D RID: 13917 RVA: 0x000F035B File Offset: 0x000EE55B
	public ToolItemLiquidsData.Data LiquidSavedData
	{
		get
		{
			return PlayerData.instance.ToolLiquids.GetData(base.name);
		}
		private set
		{
			PlayerData.instance.ToolLiquids.SetData(base.name, value);
		}
	}

	// Token: 0x1700063C RID: 1596
	// (get) Token: 0x0600365E RID: 13918 RVA: 0x000F0373 File Offset: 0x000EE573
	public override bool UsableWhenEmpty
	{
		get
		{
			return base.UsableWhenEmpty && !this.LiquidSavedData.UsedExtra;
		}
	}

	// Token: 0x1700063D RID: 1597
	// (get) Token: 0x0600365F RID: 13919 RVA: 0x000F038D File Offset: 0x000EE58D
	public override bool UsableWhenEmptyPrevented
	{
		get
		{
			return ToolItemStatesLiquid._waitingForBottleBreak == this;
		}
	}

	// Token: 0x1700063E RID: 1598
	// (get) Token: 0x06003660 RID: 13920 RVA: 0x000F039A File Offset: 0x000EE59A
	public override bool HideUsePrompt
	{
		get
		{
			return this.IsEmptyNoRefills();
		}
	}

	// Token: 0x1700063F RID: 1599
	// (get) Token: 0x06003661 RID: 13921 RVA: 0x000F03A4 File Offset: 0x000EE5A4
	public bool IsFull
	{
		get
		{
			int amountLeft = base.SavedData.AmountLeft;
			int toolStorageAmount = ToolItemManager.GetToolStorageAmount(this);
			return amountLeft >= toolStorageAmount && this.IsRefillsFull;
		}
	}

	// Token: 0x17000640 RID: 1600
	// (get) Token: 0x06003662 RID: 13922 RVA: 0x000F03CE File Offset: 0x000EE5CE
	public bool IsRefillsFull
	{
		get
		{
			return this.LiquidSavedData.RefillsLeft >= this.refillsMax;
		}
	}

	// Token: 0x06003663 RID: 13923 RVA: 0x000F03E8 File Offset: 0x000EE5E8
	public override void SetupExtraDescription(GameObject obj)
	{
		LiquidMeter component = obj.GetComponent<LiquidMeter>();
		if (!component)
		{
			return;
		}
		component.SetDisplay(this);
	}

	// Token: 0x06003664 RID: 13924 RVA: 0x000F040C File Offset: 0x000EE60C
	private bool IsEmptyNoRefills()
	{
		ToolItemLiquidsData.Data liquidSavedData = this.LiquidSavedData;
		return base.IsEmpty && !this.HasInfiniteRefills && liquidSavedData.RefillsLeft <= 0;
	}

	// Token: 0x06003665 RID: 13925 RVA: 0x000F0440 File Offset: 0x000EE640
	public override bool TryReplenishSingle(bool doReplenish, float inCost, out float outCost, out int reserveCost)
	{
		outCost = inCost;
		reserveCost = 1;
		if (this.HasInfiniteRefills)
		{
			reserveCost = 0;
			return true;
		}
		ToolItemLiquidsData.Data liquidSavedData = this.LiquidSavedData;
		bool flag = liquidSavedData.RefillsLeft <= 0;
		if (!liquidSavedData.UsedExtra)
		{
			return !flag;
		}
		outCost += (float)this.bottleCost;
		if (flag)
		{
			reserveCost = 0;
		}
		if (doReplenish)
		{
			liquidSavedData.UsedExtra = false;
			this.LiquidSavedData = liquidSavedData;
			if (ToolItemStatesLiquid._waitingForBottleBreak == this)
			{
				ToolItemStatesLiquid._waitingForBottleBreak = null;
			}
			AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(this);
			if (attackToolBinding != null)
			{
				ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
			}
		}
		return true;
	}

	// Token: 0x06003666 RID: 13926 RVA: 0x000F04D8 File Offset: 0x000EE6D8
	public void TakeLiquid(int amount, bool showCounter)
	{
		if (showCounter)
		{
			LiquidReserveCounter.Take(this, amount);
		}
		ToolItemLiquidsData.Data liquidSavedData = this.LiquidSavedData;
		liquidSavedData.RefillsLeft -= amount;
		this.LiquidSavedData = liquidSavedData;
	}

	// Token: 0x06003667 RID: 13927 RVA: 0x000F0509 File Offset: 0x000EE709
	public void ShowLiquidInfiniteRefills()
	{
		LiquidReserveCounter.InfiniteRefillPopup(this);
	}

	// Token: 0x06003668 RID: 13928 RVA: 0x000F0511 File Offset: 0x000EE711
	public override void OnWasUsed(bool wasEmpty)
	{
		if (!wasEmpty)
		{
			return;
		}
		if (this.LiquidSavedData.UsedExtra)
		{
			return;
		}
		if (this.delayBottleBreak)
		{
			ToolItemStatesLiquid._waitingForBottleBreak = this;
			return;
		}
		this.SetBottleBroken();
	}

	// Token: 0x06003669 RID: 13929 RVA: 0x000F053C File Offset: 0x000EE73C
	private void SetBottleBroken()
	{
		ToolItemLiquidsData.Data liquidSavedData = this.LiquidSavedData;
		liquidSavedData.UsedExtra = true;
		this.LiquidSavedData = liquidSavedData;
		AttackToolBinding? attackToolBinding = ToolItemManager.GetAttackToolBinding(this);
		if (attackToolBinding != null)
		{
			ToolItemManager.ReportBoundAttackToolUpdated(attackToolBinding.Value);
		}
	}

	// Token: 0x0600366A RID: 13930 RVA: 0x000F057B File Offset: 0x000EE77B
	public static void ReportBottleBroken(ToolItemStatesLiquid tool)
	{
		if (ToolItemStatesLiquid._waitingForBottleBreak == null || tool != ToolItemStatesLiquid._waitingForBottleBreak)
		{
			return;
		}
		ToolItemStatesLiquid._waitingForBottleBreak = null;
		tool.SetBottleBroken();
	}

	// Token: 0x0600366B RID: 13931 RVA: 0x000F05A4 File Offset: 0x000EE7A4
	protected override void OnUnlocked()
	{
		this.RefillRefills(false);
	}

	// Token: 0x17000641 RID: 1601
	// (get) Token: 0x0600366C RID: 13932 RVA: 0x000F05B0 File Offset: 0x000EE7B0
	public bool HasExtraNew
	{
		get
		{
			ToolItemLiquidsData.Data liquidSavedData = this.LiquidSavedData;
			return base.IsEmpty && liquidSavedData.RefillsLeft <= 0 && !liquidSavedData.SeenEmptyState;
		}
	}

	// Token: 0x0600366D RID: 13933 RVA: 0x000F05E0 File Offset: 0x000EE7E0
	public void SetExtraSeen()
	{
		ToolItemLiquidsData.Data liquidSavedData = this.LiquidSavedData;
		liquidSavedData.SeenEmptyState = true;
		this.LiquidSavedData = liquidSavedData;
	}

	// Token: 0x0600366E RID: 13934 RVA: 0x000F0604 File Offset: 0x000EE804
	public void RefillRefills(bool showPopup)
	{
		ToolItemLiquidsData.Data liquidSavedData = this.LiquidSavedData;
		liquidSavedData.UsedExtra = false;
		liquidSavedData.RefillsLeft = this.refillsMax;
		this.LiquidSavedData = liquidSavedData;
		if (showPopup)
		{
			base.ShowRefillMsgFull();
			this.PlayHeroRefillEffect();
		}
	}

	// Token: 0x0600366F RID: 13935 RVA: 0x000F0644 File Offset: 0x000EE844
	public void PlayHeroRefillEffect()
	{
		if (this.refillEffectHero)
		{
			HeroController instance = HeroController.instance;
			if (instance)
			{
				Vector3 position = instance.transform.position;
				this.refillEffectHero.Spawn(position);
			}
		}
	}

	// Token: 0x04003951 RID: 14673
	[Header("Liquid")]
	[SerializeField]
	private Color liquidColor;

	// Token: 0x04003952 RID: 14674
	[SerializeField]
	private int refillsMax;

	// Token: 0x04003953 RID: 14675
	[Space]
	[SerializeField]
	private int bottleCost;

	// Token: 0x04003954 RID: 14676
	[SerializeField]
	private bool delayBottleBreak;

	// Token: 0x04003955 RID: 14677
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string infiniteRefillsBool;

	// Token: 0x04003956 RID: 14678
	[Space]
	[SerializeField]
	private GameObject refillEffectHero;

	// Token: 0x04003957 RID: 14679
	private static ToolItemStatesLiquid _waitingForBottleBreak;
}
