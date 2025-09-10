using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020003A2 RID: 930
[ActionCategory("Hollow Knight")]
public class ShowBossSummaryUI : FsmStateAction
{
	// Token: 0x06001F52 RID: 8018 RVA: 0x0008F1E2 File Offset: 0x0008D3E2
	public override void Reset()
	{
		this.target = null;
		this.activate = new FsmBool(true);
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x0008F1FC File Offset: 0x0008D3FC
	public override void OnEnter()
	{
		GameObject safe = this.target.GetSafe(this);
		if (safe)
		{
			BossSummaryBoard component = safe.GetComponent<BossSummaryBoard>();
			if (component)
			{
				if (this.activate.Value)
				{
					component.Show();
				}
				else
				{
					component.Hide();
				}
			}
		}
		base.Finish();
	}

	// Token: 0x04001E3F RID: 7743
	public FsmOwnerDefault target;

	// Token: 0x04001E40 RID: 7744
	public FsmBool activate = true;
}
