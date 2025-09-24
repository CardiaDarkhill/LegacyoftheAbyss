using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F3 RID: 4339
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Arc Tangent 2 as in atan2(y,x) from a vector 2. You can get the result in degrees, simply check on the RadToDeg conversion")]
	public class GetAtan2FromVector2 : FsmStateAction
	{
		// Token: 0x0600755A RID: 30042 RVA: 0x0023DD7A File Offset: 0x0023BF7A
		public override void Reset()
		{
			this.vector2 = null;
			this.RadToDeg = true;
			this.everyFrame = false;
			this.angle = null;
		}

		// Token: 0x0600755B RID: 30043 RVA: 0x0023DD9D File Offset: 0x0023BF9D
		public override void OnEnter()
		{
			this.DoATan();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600755C RID: 30044 RVA: 0x0023DDB3 File Offset: 0x0023BFB3
		public override void OnUpdate()
		{
			this.DoATan();
		}

		// Token: 0x0600755D RID: 30045 RVA: 0x0023DDBC File Offset: 0x0023BFBC
		private void DoATan()
		{
			float num = Mathf.Atan2(this.vector2.Value.y, this.vector2.Value.x);
			if (this.RadToDeg.Value)
			{
				num *= 57.29578f;
			}
			this.angle.Value = num;
		}

		// Token: 0x040075C8 RID: 30152
		[RequiredField]
		[Tooltip("The vector2 of the tan")]
		public FsmVector2 vector2;

		// Token: 0x040075C9 RID: 30153
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
		public FsmFloat angle;

		// Token: 0x040075CA RID: 30154
		[Tooltip("Check on if you want the angle expressed in degrees.")]
		public FsmBool RadToDeg;

		// Token: 0x040075CB RID: 30155
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
