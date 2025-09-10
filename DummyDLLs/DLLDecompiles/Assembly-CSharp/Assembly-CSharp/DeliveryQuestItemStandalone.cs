using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001BF RID: 447
public class DeliveryQuestItemStandalone : DeliveryQuestItem
{
	// Token: 0x170001D4 RID: 468
	// (get) Token: 0x06001166 RID: 4454 RVA: 0x000515FD File Offset: 0x0004F7FD
	public int TargetCount
	{
		get
		{
			return this.targetCount;
		}
	}

	// Token: 0x170001D5 RID: 469
	// (get) Token: 0x06001167 RID: 4455 RVA: 0x00051605 File Offset: 0x0004F805
	public LocalisedString InvItemAppendDesc
	{
		get
		{
			return this.invItemAppendDesc;
		}
	}

	// Token: 0x04001058 RID: 4184
	[SerializeField]
	private int targetCount;

	// Token: 0x04001059 RID: 4185
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString invItemAppendDesc;
}
