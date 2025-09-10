using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010EF RID: 4335
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Arc Cosine. You can get the result in degrees, simply check on the RadToDeg conversion")]
	public class GetACosine : FsmStateAction
	{
		// Token: 0x06007546 RID: 30022 RVA: 0x0023DB46 File Offset: 0x0023BD46
		public override void Reset()
		{
			this.angle = null;
			this.RadToDeg = true;
			this.everyFrame = false;
			this.Value = null;
		}

		// Token: 0x06007547 RID: 30023 RVA: 0x0023DB69 File Offset: 0x0023BD69
		public override void OnEnter()
		{
			this.DoACosine();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007548 RID: 30024 RVA: 0x0023DB7F File Offset: 0x0023BD7F
		public override void OnUpdate()
		{
			this.DoACosine();
		}

		// Token: 0x06007549 RID: 30025 RVA: 0x0023DB88 File Offset: 0x0023BD88
		private void DoACosine()
		{
			float num = Mathf.Acos(this.Value.Value);
			if (this.RadToDeg.Value)
			{
				num *= 57.29578f;
			}
			this.angle.Value = num;
		}

		// Token: 0x040075B7 RID: 30135
		[RequiredField]
		[Tooltip("The value of the cosine")]
		public FsmFloat Value;

		// Token: 0x040075B8 RID: 30136
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
		public FsmFloat angle;

		// Token: 0x040075B9 RID: 30137
		[Tooltip("Check on if you want the angle expressed in degrees.")]
		public FsmBool RadToDeg;

		// Token: 0x040075BA RID: 30138
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
