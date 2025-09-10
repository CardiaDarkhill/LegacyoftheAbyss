using System;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020001C0 RID: 448
public class FakeCollectable : QuestTargetCounter
{
	// Token: 0x06001169 RID: 4457 RVA: 0x00051615 File Offset: 0x0004F815
	public override bool CanGetMore()
	{
		return true;
	}

	// Token: 0x0600116A RID: 4458 RVA: 0x00051618 File Offset: 0x0004F818
	public override void Get(bool showPopup = true)
	{
		if (!this.uiMsgName.IsEmpty || !this.uiMsgNameAlt.IsEmpty)
		{
			if (showPopup || this.showUpgradeIcon != FakeCollectable.UpgradeIconTypes.None)
			{
				CollectableUIMsg.Spawn(this, null, false);
			}
			if (this.showItemGetEffects)
			{
				CollectableItemHeroReaction.DoReaction();
			}
		}
		this.SetItemUpdated(showPopup);
	}

	// Token: 0x0600116B RID: 4459 RVA: 0x00051667 File Offset: 0x0004F867
	protected void SetItemUpdated(bool showPopup = true)
	{
		if (this.setItemUpdated)
		{
			this.setItemUpdated.SetHasNew(showPopup);
		}
	}

	// Token: 0x0600116C RID: 4460 RVA: 0x00051682 File Offset: 0x0004F882
	public override Sprite GetPopupIcon()
	{
		return this.icon;
	}

	// Token: 0x0600116D RID: 4461 RVA: 0x0005168A File Offset: 0x0004F88A
	public override string GetPopupName()
	{
		if (!this.uiMsgNameAlt.IsEmpty && this.HasUpgradeIcon())
		{
			return this.uiMsgNameAlt;
		}
		return this.uiMsgName;
	}

	// Token: 0x0600116E RID: 4462 RVA: 0x000516B8 File Offset: 0x0004F8B8
	public override bool HasUpgradeIcon()
	{
		switch (this.showUpgradeIcon)
		{
		case FakeCollectable.UpgradeIconTypes.None:
			return false;
		case FakeCollectable.UpgradeIconTypes.Always:
			return true;
		case FakeCollectable.UpgradeIconTypes.AfterFirst:
			return this.GetSavedAmount() > 0;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0400105A RID: 4186
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString uiMsgName;

	// Token: 0x0400105B RID: 4187
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString uiMsgNameAlt;

	// Token: 0x0400105C RID: 4188
	[FormerlySerializedAs("uiMsgSprite")]
	[SerializeField]
	private Sprite icon;

	// Token: 0x0400105D RID: 4189
	[SerializeField]
	private SavedItem setItemUpdated;

	// Token: 0x0400105E RID: 4190
	[SerializeField]
	private bool showItemGetEffects = true;

	// Token: 0x0400105F RID: 4191
	[Space]
	[SerializeField]
	private FakeCollectable.UpgradeIconTypes showUpgradeIcon;

	// Token: 0x02001501 RID: 5377
	private enum UpgradeIconTypes
	{
		// Token: 0x04008581 RID: 34177
		None,
		// Token: 0x04008582 RID: 34178
		Always,
		// Token: 0x04008583 RID: 34179
		AfterFirst
	}
}
