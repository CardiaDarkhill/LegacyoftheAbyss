using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F7F RID: 3967
	[ActionCategory(ActionCategory.Math)]
	public class GetDirectionBetweenAngles : FsmStateAction
	{
		// Token: 0x06006DD8 RID: 28120 RVA: 0x002217AD File Offset: 0x0021F9AD
		public override void Reset()
		{
			this.everyFrame = false;
		}

		// Token: 0x06006DD9 RID: 28121 RVA: 0x002217B6 File Offset: 0x0021F9B6
		public override void OnEnter()
		{
			this.GetDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DDA RID: 28122 RVA: 0x002217CC File Offset: 0x0021F9CC
		public override void OnUpdate()
		{
			this.GetDirection();
		}

		// Token: 0x06006DDB RID: 28123 RVA: 0x002217D4 File Offset: 0x0021F9D4
		private void GetDirection()
		{
			float num = this.startAngle.Value;
			float value = this.targetAngle.Value;
			num %= 360f;
			float num2 = value % 360f;
			float num3 = 360f - num;
			if ((num2 + num3) % 360f < 180f)
			{
				this.storeDirection.Value = true;
			}
			else
			{
				this.storeDirection.Value = false;
			}
			if (!this.storeDistance.IsNone)
			{
				float num4 = Mathf.Abs(this.startAngle.Value - this.targetAngle.Value) % 360f;
				if (num4 > 180f)
				{
					num4 = 360f - num4;
				}
				this.storeDistance.Value = num4;
			}
		}

		// Token: 0x04006D8D RID: 28045
		[RequiredField]
		public FsmFloat startAngle;

		// Token: 0x04006D8E RID: 28046
		[RequiredField]
		public FsmFloat targetAngle;

		// Token: 0x04006D8F RID: 28047
		[RequiredField]
		public FsmBool storeDirection;

		// Token: 0x04006D90 RID: 28048
		public FsmFloat storeDistance;

		// Token: 0x04006D91 RID: 28049
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
