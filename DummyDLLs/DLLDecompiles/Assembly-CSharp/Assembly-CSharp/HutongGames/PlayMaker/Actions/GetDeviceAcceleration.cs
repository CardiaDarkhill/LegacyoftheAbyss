using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E87 RID: 3719
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Gets the last measured linear acceleration of a device and stores it in a Vector3 Variable.")]
	public class GetDeviceAcceleration : FsmStateAction
	{
		// Token: 0x060069B9 RID: 27065 RVA: 0x002113BE File Offset: 0x0020F5BE
		public override void Reset()
		{
			this.storeVector = null;
			this.storeX = null;
			this.storeY = null;
			this.storeZ = null;
			this.multiplier = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060069BA RID: 27066 RVA: 0x002113F3 File Offset: 0x0020F5F3
		public override void OnEnter()
		{
			this.DoGetDeviceAcceleration();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069BB RID: 27067 RVA: 0x00211409 File Offset: 0x0020F609
		public override void OnUpdate()
		{
			this.DoGetDeviceAcceleration();
		}

		// Token: 0x060069BC RID: 27068 RVA: 0x00211414 File Offset: 0x0020F614
		private void DoGetDeviceAcceleration()
		{
			Vector3 vector = ActionHelpers.GetDeviceAcceleration();
			if (!this.multiplier.IsNone)
			{
				vector *= this.multiplier.Value;
			}
			this.storeVector.Value = vector;
			this.storeX.Value = vector.x;
			this.storeY.Value = vector.y;
			this.storeZ.Value = vector.z;
		}

		// Token: 0x040068DF RID: 26847
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the acceleration in a Vector3 Variable.")]
		public FsmVector3 storeVector;

		// Token: 0x040068E0 RID: 26848
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the x component of the acceleration in a Float Variable.")]
		public FsmFloat storeX;

		// Token: 0x040068E1 RID: 26849
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the y component of the acceleration in a Float Variable.")]
		public FsmFloat storeY;

		// Token: 0x040068E2 RID: 26850
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the z component of the acceleration in a Float Variable.")]
		public FsmFloat storeZ;

		// Token: 0x040068E3 RID: 26851
		[Tooltip("Multiply the acceleration by a float value.")]
		public FsmFloat multiplier;

		// Token: 0x040068E4 RID: 26852
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
