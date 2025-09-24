using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F6 RID: 4342
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the sine. You can use degrees, simply check on the DegToRad conversion")]
	public class GetSine : FsmStateAction
	{
		// Token: 0x06007569 RID: 30057 RVA: 0x0023DFBB File Offset: 0x0023C1BB
		public override void Reset()
		{
			this.angle = null;
			this.DegToRad = true;
			this.everyFrame = false;
			this.result = null;
		}

		// Token: 0x0600756A RID: 30058 RVA: 0x0023DFDE File Offset: 0x0023C1DE
		public override void OnEnter()
		{
			this.DoSine();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600756B RID: 30059 RVA: 0x0023DFF4 File Offset: 0x0023C1F4
		public override void OnUpdate()
		{
			this.DoSine();
		}

		// Token: 0x0600756C RID: 30060 RVA: 0x0023DFFC File Offset: 0x0023C1FC
		private void DoSine()
		{
			float num = this.angle.Value;
			if (this.DegToRad.Value)
			{
				num *= 0.017453292f;
			}
			this.result.Value = Mathf.Sin(num);
		}

		// Token: 0x040075D6 RID: 30166
		[RequiredField]
		[Tooltip("The angle. Note: You can use degrees, simply check DegtoRad if the angle is expressed in degrees.")]
		public FsmFloat angle;

		// Token: 0x040075D7 RID: 30167
		[Tooltip("Check on if the angle is expressed in degrees.")]
		public FsmBool DegToRad;

		// Token: 0x040075D8 RID: 30168
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The angle tan")]
		public FsmFloat result;

		// Token: 0x040075D9 RID: 30169
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
