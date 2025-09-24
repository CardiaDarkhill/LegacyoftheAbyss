using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E88 RID: 3720
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets the rotation of the device around its z axis (into the screen). For example when you steer with the iPhone in a driving game.")]
	public class GetDeviceRoll : FsmStateAction
	{
		// Token: 0x060069BE RID: 27070 RVA: 0x0021148D File Offset: 0x0020F68D
		public override void Reset()
		{
			this.baseOrientation = GetDeviceRoll.BaseOrientation.LandscapeLeft;
			this.storeAngle = null;
			this.limitAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.smoothing = 5f;
			this.everyFrame = true;
		}

		// Token: 0x060069BF RID: 27071 RVA: 0x002114C6 File Offset: 0x0020F6C6
		public override void OnEnter()
		{
			this.DoGetDeviceRoll();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069C0 RID: 27072 RVA: 0x002114DC File Offset: 0x0020F6DC
		public override void OnUpdate()
		{
			this.DoGetDeviceRoll();
		}

		// Token: 0x060069C1 RID: 27073 RVA: 0x002114E4 File Offset: 0x0020F6E4
		private void DoGetDeviceRoll()
		{
			Vector3 deviceAcceleration = ActionHelpers.GetDeviceAcceleration();
			float x = deviceAcceleration.x;
			float y = deviceAcceleration.y;
			float num = 0f;
			switch (this.baseOrientation)
			{
			case GetDeviceRoll.BaseOrientation.Portrait:
				num = -Mathf.Atan2(x, -y);
				break;
			case GetDeviceRoll.BaseOrientation.LandscapeLeft:
				num = Mathf.Atan2(y, -x);
				break;
			case GetDeviceRoll.BaseOrientation.LandscapeRight:
				num = -Mathf.Atan2(y, x);
				break;
			}
			if (!this.limitAngle.IsNone)
			{
				num = Mathf.Clamp(57.29578f * num, -this.limitAngle.Value, this.limitAngle.Value);
			}
			if (this.smoothing.Value > 0f)
			{
				num = Mathf.LerpAngle(this.lastZAngle, num, this.smoothing.Value * Time.deltaTime);
			}
			this.lastZAngle = num;
			this.storeAngle.Value = num;
		}

		// Token: 0x040068E5 RID: 26853
		[Tooltip("How the user is expected to hold the device (where angle will be zero).")]
		public GetDeviceRoll.BaseOrientation baseOrientation;

		// Token: 0x040068E6 RID: 26854
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the roll angle in a Float Variable.")]
		public FsmFloat storeAngle;

		// Token: 0x040068E7 RID: 26855
		[Tooltip("Limit the roll angle.")]
		public FsmFloat limitAngle;

		// Token: 0x040068E8 RID: 26856
		[Tooltip("Smooth the roll angle as it changes. You can play with this value to balance responsiveness and lag/smoothness.")]
		public FsmFloat smoothing;

		// Token: 0x040068E9 RID: 26857
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040068EA RID: 26858
		private float lastZAngle;

		// Token: 0x02001BA6 RID: 7078
		public enum BaseOrientation
		{
			// Token: 0x04009E0E RID: 40462
			Portrait,
			// Token: 0x04009E0F RID: 40463
			LandscapeLeft,
			// Token: 0x04009E10 RID: 40464
			LandscapeRight
		}
	}
}
