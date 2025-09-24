using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020007B4 RID: 1972
[ActionCategory("Hollow Knight")]
public class VibrationPlayerPlayV2 : FsmStateAction
{
	// Token: 0x060045A1 RID: 17825 RVA: 0x0012F2E5 File Offset: 0x0012D4E5
	public override void Reset()
	{
		base.Reset();
		this.target = new FsmOwnerDefault();
		this.stopOnStateExit = null;
	}

	// Token: 0x060045A2 RID: 17826 RVA: 0x0012F300 File Offset: 0x0012D500
	public override void OnEnter()
	{
		base.OnEnter();
		GameObject safe = this.target.GetSafe(this);
		if (safe != null)
		{
			VibrationPlayer component = safe.GetComponent<VibrationPlayer>();
			if (component != null)
			{
				if (ObjectPool.IsCreatingPool)
				{
					return;
				}
				component.Play();
			}
		}
		base.Finish();
	}

	// Token: 0x060045A3 RID: 17827 RVA: 0x0012F350 File Offset: 0x0012D550
	public override void OnExit()
	{
		if (this.stopOnStateExit.Value)
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				VibrationPlayer component = safe.GetComponent<VibrationPlayer>();
				if (component != null)
				{
					component.Stop();
				}
			}
		}
	}

	// Token: 0x04004641 RID: 17985
	public FsmOwnerDefault target;

	// Token: 0x04004642 RID: 17986
	public FsmBool stopOnStateExit;
}
