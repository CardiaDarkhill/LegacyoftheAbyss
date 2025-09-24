using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020007B6 RID: 1974
[ActionCategory("Hollow Knight")]
public class VibrationPlayerSetMotors : FsmStateAction
{
	// Token: 0x060045A8 RID: 17832 RVA: 0x0012F401 File Offset: 0x0012D601
	public override void Reset()
	{
		base.Reset();
		this.target = new FsmOwnerDefault();
		this.motors = new FsmEnum
		{
			Value = VibrationMotors.All
		};
	}

	// Token: 0x060045A9 RID: 17833 RVA: 0x0012F42C File Offset: 0x0012D62C
	public override void OnEnter()
	{
		base.OnEnter();
		GameObject safe = this.target.GetSafe(this);
		if (safe != null)
		{
			VibrationPlayer component = safe.GetComponent<VibrationPlayer>();
			if (component != null && !this.motors.IsNone)
			{
				component.Target = new VibrationTarget((VibrationMotors)this.motors.Value);
			}
		}
		base.Finish();
	}

	// Token: 0x04004644 RID: 17988
	public FsmOwnerDefault target;

	// Token: 0x04004645 RID: 17989
	[ObjectType(typeof(VibrationMotors))]
	public FsmEnum motors;
}
