using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FD9 RID: 4057
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the gravity vector, or individual axis.")]
	public class SetGravity2d : FsmStateAction
	{
		// Token: 0x06006FC4 RID: 28612 RVA: 0x00228F2E File Offset: 0x0022712E
		public override void Reset()
		{
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006FC5 RID: 28613 RVA: 0x00228F62 File Offset: 0x00227162
		public override void OnEnter()
		{
			this.DoSetGravity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FC6 RID: 28614 RVA: 0x00228F78 File Offset: 0x00227178
		public override void OnUpdate()
		{
			this.DoSetGravity();
		}

		// Token: 0x06006FC7 RID: 28615 RVA: 0x00228F80 File Offset: 0x00227180
		private void DoSetGravity()
		{
			Vector2 value = this.vector.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			Physics2D.gravity = value;
		}

		// Token: 0x04006FBE RID: 28606
		[Tooltip("Gravity as Vector2.")]
		public FsmVector2 vector;

		// Token: 0x04006FBF RID: 28607
		[Tooltip("Override the x value of the gravity")]
		public FsmFloat x;

		// Token: 0x04006FC0 RID: 28608
		[Tooltip("Override the y value of the gravity")]
		public FsmFloat y;

		// Token: 0x04006FC1 RID: 28609
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
