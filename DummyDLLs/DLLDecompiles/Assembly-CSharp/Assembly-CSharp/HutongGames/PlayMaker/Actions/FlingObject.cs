using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3D RID: 3133
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Fling")]
	public class FlingObject : RigidBody2dActionBase
	{
		// Token: 0x06005F32 RID: 24370 RVA: 0x001E3160 File Offset: 0x001E1360
		public override void Reset()
		{
			this.flungObject = null;
			this.speedMin = null;
			this.speedMax = null;
			this.angleMin = null;
			this.angleMax = null;
		}

		// Token: 0x06005F33 RID: 24371 RVA: 0x001E3188 File Offset: 0x001E1388
		public override void OnEnter()
		{
			if (this.flungObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.flungObject);
				if (ownerDefaultTarget != null)
				{
					float num = Random.Range(this.speedMin.Value, this.speedMax.Value);
					float num2 = Random.Range(this.angleMin.Value, this.angleMax.Value);
					this.vectorX = num * Mathf.Cos(num2 * 0.017453292f);
					this.vectorY = num * Mathf.Sin(num2 * 0.017453292f);
					Vector2 linearVelocity;
					linearVelocity.x = this.vectorX;
					linearVelocity.y = this.vectorY;
					base.CacheRigidBody2d(ownerDefaultTarget);
					this.rb2d.linearVelocity = linearVelocity;
				}
			}
			base.Finish();
		}

		// Token: 0x04005C31 RID: 23601
		[RequiredField]
		public FsmOwnerDefault flungObject;

		// Token: 0x04005C32 RID: 23602
		public FsmFloat speedMin;

		// Token: 0x04005C33 RID: 23603
		public FsmFloat speedMax;

		// Token: 0x04005C34 RID: 23604
		public FsmFloat angleMin;

		// Token: 0x04005C35 RID: 23605
		public FsmFloat angleMax;

		// Token: 0x04005C36 RID: 23606
		private float vectorX;

		// Token: 0x04005C37 RID: 23607
		private float vectorY;

		// Token: 0x04005C38 RID: 23608
		private bool originAdjusted;
	}
}
