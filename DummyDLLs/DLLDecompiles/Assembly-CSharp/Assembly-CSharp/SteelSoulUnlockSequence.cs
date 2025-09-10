using System;

// Token: 0x020000C0 RID: 192
public class SteelSoulUnlockSequence : AnimatorSequence
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000614 RID: 1556 RVA: 0x0001F1F5 File Offset: 0x0001D3F5
	public override bool ShouldShow
	{
		get
		{
			return base.ShouldShow && GameManager.instance.GetStatusRecordInt("RecPermadeathMode") == 0;
		}
	}

	// Token: 0x06000615 RID: 1557 RVA: 0x0001F213 File Offset: 0x0001D413
	public override void Begin()
	{
		base.Begin();
		GameManager.instance.SetStatusRecordInt("RecPermadeathMode", 1);
		GameManager.instance.SaveStatusRecords();
	}
}
