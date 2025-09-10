using System;
using GlobalEnums;
using TeamCherry.Localization;

// Token: 0x02000684 RID: 1668
[Serializable]
public struct InventoryItemButtonPromptData
{
	// Token: 0x170006BC RID: 1724
	// (get) Token: 0x06003B9F RID: 15263 RVA: 0x001064E0 File Offset: 0x001046E0
	public HeroActionButton MenuAction
	{
		get
		{
			if (!this.IsMenuButton)
			{
				return this.Action;
			}
			if (Platform.Current.AcceptRejectInputStyle == Platform.AcceptRejectInputStyles.JapaneseStyle)
			{
				HeroActionButton action = this.Action;
				if (action == HeroActionButton.JUMP)
				{
					return HeroActionButton.CAST;
				}
				if (action == HeroActionButton.CAST)
				{
					return HeroActionButton.JUMP;
				}
			}
			return this.Action;
		}
	}

	// Token: 0x04003DD2 RID: 15826
	public HeroActionButton Action;

	// Token: 0x04003DD3 RID: 15827
	public bool IsMenuButton;

	// Token: 0x04003DD4 RID: 15828
	[LocalisedString.NotRequiredAttribute]
	public LocalisedString UseText;

	// Token: 0x04003DD5 RID: 15829
	public LocalisedString ResponseText;

	// Token: 0x04003DD6 RID: 15830
	[LocalisedString.NotRequiredAttribute]
	public LocalisedString ConditionText;
}
