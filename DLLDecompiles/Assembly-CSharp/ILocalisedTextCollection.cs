using System;
using TeamCherry.Localization;

// Token: 0x020001DB RID: 475
public interface ILocalisedTextCollection
{
	// Token: 0x17000209 RID: 521
	// (get) Token: 0x06001289 RID: 4745
	bool IsActive { get; }

	// Token: 0x0600128A RID: 4746
	LocalisedString GetRandom(LocalisedString skipString);

	// Token: 0x0600128B RID: 4747 RVA: 0x00056355 File Offset: 0x00054555
	NeedolinTextConfig GetConfig()
	{
		return null;
	}
}
