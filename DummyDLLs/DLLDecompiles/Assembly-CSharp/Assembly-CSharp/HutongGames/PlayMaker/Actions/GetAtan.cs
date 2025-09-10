using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F1 RID: 4337
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Arc Tangent. You can get the result in degrees, simply check on the RadToDeg conversion")]
	public class GetAtan : FsmStateAction
	{
		// Token: 0x06007550 RID: 30032 RVA: 0x0023DC57 File Offset: 0x0023BE57
		public override void Reset()
		{
			this.Value = null;
			this.RadToDeg = true;
			this.everyFrame = false;
			this.angle = null;
		}

		// Token: 0x06007551 RID: 30033 RVA: 0x0023DC7A File Offset: 0x0023BE7A
		public override void OnEnter()
		{
			this.DoATan();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007552 RID: 30034 RVA: 0x0023DC90 File Offset: 0x0023BE90
		public override void OnUpdate()
		{
			this.DoATan();
		}

		// Token: 0x06007553 RID: 30035 RVA: 0x0023DC98 File Offset: 0x0023BE98
		private void DoATan()
		{
			float num = Mathf.Atan(this.Value.Value);
			if (this.RadToDeg.Value)
			{
				num *= 57.29578f;
			}
			this.angle.Value = num;
		}

		// Token: 0x040075BF RID: 30143
		[RequiredField]
		[Tooltip("The value of the tan")]
		public FsmFloat Value;

		// Token: 0x040075C0 RID: 30144
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
		public FsmFloat angle;

		// Token: 0x040075C1 RID: 30145
		[Tooltip("Check on if you want the angle expressed in degrees.")]
		public FsmBool RadToDeg;

		// Token: 0x040075C2 RID: 30146
		[Tooltip("Repeat Every Frame")]
		public bool everyFrame;
	}
}
