using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FAF RID: 4015
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Sets the gravity vector, or individual axis.")]
	public class SetGravity : FsmStateAction
	{
		// Token: 0x06006EDA RID: 28378 RVA: 0x00224C64 File Offset: 0x00222E64
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
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006EDB RID: 28379 RVA: 0x00224CB5 File Offset: 0x00222EB5
		public override void OnEnter()
		{
			this.DoSetGravity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EDC RID: 28380 RVA: 0x00224CCB File Offset: 0x00222ECB
		public override void OnUpdate()
		{
			this.DoSetGravity();
		}

		// Token: 0x06006EDD RID: 28381 RVA: 0x00224CD4 File Offset: 0x00222ED4
		private void DoSetGravity()
		{
			Vector3 value = this.vector.Value;
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				value.z = this.z.Value;
			}
			Physics.gravity = value;
		}

		// Token: 0x04006E92 RID: 28306
		[Tooltip("Set Gravity using a Vector3. Or set override individual axis below.")]
		public FsmVector3 vector;

		// Token: 0x04006E93 RID: 28307
		[Tooltip("X amount.")]
		public FsmFloat x;

		// Token: 0x04006E94 RID: 28308
		[Tooltip("Y amount. The most common up/down axis for gravity.")]
		public FsmFloat y;

		// Token: 0x04006E95 RID: 28309
		[Tooltip("Z amount.")]
		public FsmFloat z;

		// Token: 0x04006E96 RID: 28310
		[Tooltip("Update gravity every frame. Useful if you're changing gravity over a period of time.")]
		public bool everyFrame;
	}
}
