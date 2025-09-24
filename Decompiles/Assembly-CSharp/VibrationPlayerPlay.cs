using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020007B3 RID: 1971
[ActionCategory("Hollow Knight")]
public class VibrationPlayerPlay : FsmStateAction
{
	// Token: 0x0600459E RID: 17822 RVA: 0x0012F27A File Offset: 0x0012D47A
	public override void Reset()
	{
		base.Reset();
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x0600459F RID: 17823 RVA: 0x0012F290 File Offset: 0x0012D490
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

	// Token: 0x04004640 RID: 17984
	public FsmOwnerDefault target;
}
