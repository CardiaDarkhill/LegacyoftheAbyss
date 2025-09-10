using System;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020001AB RID: 427
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Basic)")]
public class CollectableItemBasic : CollectableItem
{
	// Token: 0x170001AF RID: 431
	// (get) Token: 0x06001095 RID: 4245 RVA: 0x0004F07F File Offset: 0x0004D27F
	public override int CollectedAmount
	{
		get
		{
			return base.CollectedAmount + (this.isAlwaysUnlocked ? 1 : 0);
		}
	}

	// Token: 0x170001B0 RID: 432
	// (get) Token: 0x06001096 RID: 4246 RVA: 0x0004F094 File Offset: 0x0004D294
	public override bool DisplayAmount
	{
		get
		{
			return this.CollectedAmount > 1;
		}
	}

	// Token: 0x06001097 RID: 4247 RVA: 0x0004F09F File Offset: 0x0004D29F
	public override string GetDisplayName(CollectableItem.ReadSource readSource)
	{
		return this.displayName;
	}

	// Token: 0x06001098 RID: 4248 RVA: 0x0004F0AC File Offset: 0x0004D2AC
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		return this.description;
	}

	// Token: 0x06001099 RID: 4249 RVA: 0x0004F0B9 File Offset: 0x0004D2B9
	public override Sprite GetIcon(CollectableItem.ReadSource readSource)
	{
		if (readSource == CollectableItem.ReadSource.Tiny && this.tinyIcon)
		{
			return this.tinyIcon;
		}
		return this.icon;
	}

	// Token: 0x0600109A RID: 4250 RVA: 0x0004F0D9 File Offset: 0x0004D2D9
	public override InventoryItemButtonPromptData[] GetButtonPromptData()
	{
		if (this.displayButtonPrompt)
		{
			return this.buttonPromptData;
		}
		return null;
	}

	// Token: 0x0600109B RID: 4251 RVA: 0x0004F0EC File Offset: 0x0004D2EC
	protected override void OnCollected()
	{
		this.SetUniqueBool();
		foreach (PlayerDataBoolOperation playerDataBoolOperation in this.setExtraPlayerDataBools)
		{
			playerDataBoolOperation.Execute();
		}
		foreach (PlayerDataIntOperation playerDataIntOperation in this.setExtraPlayerDataInts)
		{
			playerDataIntOperation.Execute();
		}
	}

	// Token: 0x0600109C RID: 4252 RVA: 0x0004F148 File Offset: 0x0004D348
	public override void ReportPreviouslyCollected()
	{
		base.ReportPreviouslyCollected();
		this.SetUniqueBool();
	}

	// Token: 0x0600109D RID: 4253 RVA: 0x0004F156 File Offset: 0x0004D356
	private void SetUniqueBool()
	{
		if (!string.IsNullOrEmpty(this.uniqueCollectBool))
		{
			PlayerData.instance.SetVariable(this.uniqueCollectBool, true);
		}
	}

	// Token: 0x0600109E RID: 4254 RVA: 0x0004F176 File Offset: 0x0004D376
	public override bool CanGetMore()
	{
		if (!string.IsNullOrEmpty(this.uniqueCollectBool))
		{
			return !PlayerData.instance.GetVariable(this.uniqueCollectBool);
		}
		return base.CanGetMore();
	}

	// Token: 0x0600109F RID: 4255 RVA: 0x0004F19F File Offset: 0x0004D39F
	public override bool ShouldStopCollectNoMsg()
	{
		if (!string.IsNullOrEmpty(this.uniqueCollectBool))
		{
			return PlayerData.instance.GetVariable(this.uniqueCollectBool);
		}
		return base.ShouldStopCollectNoMsg();
	}

	// Token: 0x04000FE0 RID: 4064
	[Space]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x04000FE1 RID: 4065
	[SerializeField]
	private LocalisedString description;

	// Token: 0x04000FE2 RID: 4066
	[SerializeField]
	private Sprite icon;

	// Token: 0x04000FE3 RID: 4067
	[SerializeField]
	private Sprite tinyIcon;

	// Token: 0x04000FE4 RID: 4068
	[SerializeField]
	private bool isAlwaysUnlocked;

	// Token: 0x04000FE5 RID: 4069
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string uniqueCollectBool;

	// Token: 0x04000FE6 RID: 4070
	[SerializeField]
	private PlayerDataBoolOperation[] setExtraPlayerDataBools;

	// Token: 0x04000FE7 RID: 4071
	[SerializeField]
	private PlayerDataIntOperation[] setExtraPlayerDataInts;

	// Token: 0x04000FE8 RID: 4072
	[Space]
	[SerializeField]
	public bool displayButtonPrompt;

	// Token: 0x04000FE9 RID: 4073
	[ModifiableProperty]
	[Conditional("displayButtonPrompt", true, false, false)]
	public InventoryItemButtonPromptData[] buttonPromptData;
}
