using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB3 RID: 3507
	[ActionCategory("Physics 2d")]
	[Tooltip("Rotate to a specific z angle over time")]
	public class RotateTo : RigidBody2dActionBase
	{
		// Token: 0x060065BC RID: 26044 RVA: 0x0020190E File Offset: 0x001FFB0E
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060065BD RID: 26045 RVA: 0x00201917 File Offset: 0x001FFB17
		public override void OnUpdate()
		{
			this.DoRotateTo();
		}

		// Token: 0x060065BE RID: 26046 RVA: 0x00201920 File Offset: 0x001FFB20
		private void DoRotateTo()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.targetAngle.Value > 360f)
			{
				this.targetAngle.Value -= 360f;
			}
			float num = this.targetAngle.Value - ownerDefaultTarget.transform.localEulerAngles.z;
			bool flag;
			if (num < 0f)
			{
				flag = (num < -180f);
			}
			else
			{
				flag = (num <= 180f);
			}
			if (flag)
			{
				ownerDefaultTarget.transform.Rotate(0f, 0f, this.speed.Value * Time.deltaTime);
				if (ownerDefaultTarget.transform.localEulerAngles.z > this.targetAngle.Value)
				{
					ownerDefaultTarget.transform.localEulerAngles = new Vector3(ownerDefaultTarget.transform.rotation.x, ownerDefaultTarget.transform.rotation.y, this.targetAngle.Value);
					return;
				}
			}
			else
			{
				ownerDefaultTarget.transform.Rotate(0f, 0f, -this.speed.Value * Time.deltaTime);
				if (ownerDefaultTarget.transform.localEulerAngles.z < this.targetAngle.Value)
				{
					ownerDefaultTarget.transform.localEulerAngles = new Vector3(ownerDefaultTarget.transform.rotation.x, ownerDefaultTarget.transform.rotation.y, this.targetAngle.Value);
				}
			}
		}

		// Token: 0x040064DE RID: 25822
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040064DF RID: 25823
		public FsmFloat targetAngle;

		// Token: 0x040064E0 RID: 25824
		public FsmFloat speed;
	}
}
