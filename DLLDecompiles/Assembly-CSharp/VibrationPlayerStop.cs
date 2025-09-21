using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020007B5 RID: 1973
[ActionCategory("Hollow Knight")]
public class VibrationPlayerStop : FsmStateAction
{
	// Token: 0x060045A5 RID: 17829 RVA: 0x0012F39E File Offset: 0x0012D59E
	public override void Reset()
	{
		base.Reset();
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x060045A6 RID: 17830 RVA: 0x0012F3B4 File Offset: 0x0012D5B4
	public override void OnEnter()
	{
		base.OnEnter();
		GameObject safe = this.target.GetSafe(this);
		if (safe != null)
		{
			VibrationPlayer component = safe.GetComponent<VibrationPlayer>();
			if (component != null)
			{
				component.Stop();
			}
		}
		base.Finish();
	}

	// Token: 0x04004643 RID: 17987
	public FsmOwnerDefault target;
}
