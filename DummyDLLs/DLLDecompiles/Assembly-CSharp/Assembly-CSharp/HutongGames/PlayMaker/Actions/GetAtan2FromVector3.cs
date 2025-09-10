using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010F4 RID: 4340
	[ActionCategory(ActionCategory.Trigonometry)]
	[Tooltip("Get the Arc Tangent 2 as in atan2(y,x) from a vector 3, where you pick which is x and y from the vector 3. You can get the result in degrees, simply check on the RadToDeg conversion")]
	public class GetAtan2FromVector3 : FsmStateAction
	{
		// Token: 0x0600755F RID: 30047 RVA: 0x0023DE18 File Offset: 0x0023C018
		public override void Reset()
		{
			this.vector3 = null;
			this.xAxis = GetAtan2FromVector3.aTan2EnumAxis.x;
			this.yAxis = GetAtan2FromVector3.aTan2EnumAxis.y;
			this.RadToDeg = true;
			this.everyFrame = false;
			this.angle = null;
		}

		// Token: 0x06007560 RID: 30048 RVA: 0x0023DE49 File Offset: 0x0023C049
		public override void OnEnter()
		{
			this.DoATan();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007561 RID: 30049 RVA: 0x0023DE5F File Offset: 0x0023C05F
		public override void OnUpdate()
		{
			this.DoATan();
		}

		// Token: 0x06007562 RID: 30050 RVA: 0x0023DE68 File Offset: 0x0023C068
		private void DoATan()
		{
			float x = this.vector3.Value.x;
			if (this.xAxis == GetAtan2FromVector3.aTan2EnumAxis.y)
			{
				x = this.vector3.Value.y;
			}
			else if (this.xAxis == GetAtan2FromVector3.aTan2EnumAxis.z)
			{
				x = this.vector3.Value.z;
			}
			float y = this.vector3.Value.y;
			if (this.yAxis == GetAtan2FromVector3.aTan2EnumAxis.x)
			{
				y = this.vector3.Value.x;
			}
			else if (this.yAxis == GetAtan2FromVector3.aTan2EnumAxis.z)
			{
				y = this.vector3.Value.z;
			}
			float num = Mathf.Atan2(y, x);
			if (this.RadToDeg.Value)
			{
				num *= 57.29578f;
			}
			this.angle.Value = num;
		}

		// Token: 0x040075CC RID: 30156
		[RequiredField]
		[Tooltip("The vector3 definition of the tan")]
		public FsmVector3 vector3;

		// Token: 0x040075CD RID: 30157
		[RequiredField]
		[Tooltip("which axis in the vector3 to use as the x value of the tan")]
		public GetAtan2FromVector3.aTan2EnumAxis xAxis;

		// Token: 0x040075CE RID: 30158
		[RequiredField]
		[Tooltip("which axis in the vector3 to use as the y value of the tan")]
		public GetAtan2FromVector3.aTan2EnumAxis yAxis;

		// Token: 0x040075CF RID: 30159
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The resulting angle. Note:If you want degrees, simply check RadToDeg")]
		public FsmFloat angle;

		// Token: 0x040075D0 RID: 30160
		[Tooltip("Check on if you want the angle expressed in degrees.")]
		public FsmBool RadToDeg;

		// Token: 0x040075D1 RID: 30161
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x02001BCB RID: 7115
		public enum aTan2EnumAxis
		{
			// Token: 0x04009EC7 RID: 40647
			x,
			// Token: 0x04009EC8 RID: 40648
			y,
			// Token: 0x04009EC9 RID: 40649
			z
		}
	}
}
