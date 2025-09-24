using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F7 RID: 4343
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Tangent. You can use degrees, simply check on the DegToRad conversion")]
	public class GetTan : FsmStateAction
	{
		// Token: 0x0600756E RID: 30062 RVA: 0x0023E043 File Offset: 0x0023C243
		public override void Reset()
		{
			this.angle = null;
			this.DegToRad = true;
			this.everyFrame = false;
			this.result = null;
		}

		// Token: 0x0600756F RID: 30063 RVA: 0x0023E066 File Offset: 0x0023C266
		public override void OnEnter()
		{
			this.DoTan();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007570 RID: 30064 RVA: 0x0023E07C File Offset: 0x0023C27C
		public override void OnUpdate()
		{
			this.DoTan();
		}

		// Token: 0x06007571 RID: 30065 RVA: 0x0023E084 File Offset: 0x0023C284
		private void DoTan()
		{
			float num = this.angle.Value;
			if (this.DegToRad.Value)
			{
				num *= 0.017453292f;
			}
			this.result.Value = Mathf.Tan(num);
		}

		// Token: 0x040075DA RID: 30170
		[RequiredField]
		[Tooltip("The angle. Note: You can use degrees, simply check DegtoRad if the angle is expressed in degrees.")]
		public FsmFloat angle;

		// Token: 0x040075DB RID: 30171
		[Tooltip("Check on if the angle is expressed in degrees.")]
		public FsmBool DegToRad;

		// Token: 0x040075DC RID: 30172
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The angle tan")]
		public FsmFloat result;

		// Token: 0x040075DD RID: 30173
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
