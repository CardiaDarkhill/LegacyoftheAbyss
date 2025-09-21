using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F5 RID: 4341
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Cosine.")]
	public class GetCosine : FsmStateAction
	{
		// Token: 0x06007564 RID: 30052 RVA: 0x0023DF33 File Offset: 0x0023C133
		public override void Reset()
		{
			this.angle = null;
			this.DegToRad = true;
			this.everyFrame = false;
			this.result = null;
		}

		// Token: 0x06007565 RID: 30053 RVA: 0x0023DF56 File Offset: 0x0023C156
		public override void OnEnter()
		{
			this.DoCosine();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007566 RID: 30054 RVA: 0x0023DF6C File Offset: 0x0023C16C
		public override void OnUpdate()
		{
			this.DoCosine();
		}

		// Token: 0x06007567 RID: 30055 RVA: 0x0023DF74 File Offset: 0x0023C174
		private void DoCosine()
		{
			float num = this.angle.Value;
			if (this.DegToRad.Value)
			{
				num *= 0.017453292f;
			}
			this.result.Value = Mathf.Cos(num);
		}

		// Token: 0x040075D2 RID: 30162
		[RequiredField]
		[Tooltip("The angle. Note: Check Deg To Rad if the angle is expressed in degrees.")]
		public FsmFloat angle;

		// Token: 0x040075D3 RID: 30163
		[Tooltip("Check if the angle is expressed in degrees.")]
		public FsmBool DegToRad;

		// Token: 0x040075D4 RID: 30164
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The angle cosine.")]
		public FsmFloat result;

		// Token: 0x040075D5 RID: 30165
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
