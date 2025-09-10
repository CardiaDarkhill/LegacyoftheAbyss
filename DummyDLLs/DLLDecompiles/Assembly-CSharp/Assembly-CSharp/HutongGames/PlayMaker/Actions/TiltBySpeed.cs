using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D8A RID: 3466
	[ActionCategory(ActionCategory.Physics2D)]
	public class TiltBySpeed : ComponentAction<Rigidbody2D>
	{
		// Token: 0x060064DE RID: 25822 RVA: 0x001FD300 File Offset: 0x001FB500
		public override void Reset()
		{
			this.gameObject = null;
			this.tiltFactor = null;
			this.tiltMax = null;
		}

		// Token: 0x060064DF RID: 25823 RVA: 0x001FD317 File Offset: 0x001FB517
		public override void OnEnter()
		{
			this.DoTilt();
		}

		// Token: 0x060064E0 RID: 25824 RVA: 0x001FD31F File Offset: 0x001FB51F
		public override void OnUpdate()
		{
			this.DoTilt();
		}

		// Token: 0x060064E1 RID: 25825 RVA: 0x001FD328 File Offset: 0x001FB528
		private void DoTilt()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			float num = base.rigidbody2d.linearVelocity.x * this.tiltFactor.Value;
			if (!this.tiltMax.IsNone)
			{
				if (num > this.tiltMax.Value)
				{
					num = this.tiltMax.Value;
				}
				if (num < -this.tiltMax.Value)
				{
					num = -this.tiltMax.Value;
				}
			}
			this.DoRotateTo(num);
		}

		// Token: 0x060064E2 RID: 25826 RVA: 0x001FD3B8 File Offset: 0x001FB5B8
		private void DoRotateTo(float targetAngle)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			float num = targetAngle - ownerDefaultTarget.transform.localEulerAngles.z;
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
				ownerDefaultTarget.transform.Rotate(0f, 0f, this.rotationSpeed.Value * Time.deltaTime);
				if (ownerDefaultTarget.transform.localEulerAngles.z > targetAngle)
				{
					ownerDefaultTarget.transform.localEulerAngles = new Vector3(ownerDefaultTarget.transform.rotation.x, ownerDefaultTarget.transform.rotation.y, targetAngle);
					return;
				}
			}
			else
			{
				ownerDefaultTarget.transform.Rotate(0f, 0f, -this.rotationSpeed.Value * Time.deltaTime);
				if (ownerDefaultTarget.transform.localEulerAngles.z < targetAngle)
				{
					ownerDefaultTarget.transform.localEulerAngles = new Vector3(ownerDefaultTarget.transform.rotation.x, ownerDefaultTarget.transform.rotation.y, targetAngle);
				}
			}
		}

		// Token: 0x040063D6 RID: 25558
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040063D7 RID: 25559
		public FsmFloat tiltFactor;

		// Token: 0x040063D8 RID: 25560
		public FsmFloat tiltMax;

		// Token: 0x040063D9 RID: 25561
		public FsmFloat rotationSpeed;

		// Token: 0x040063DA RID: 25562
		private float targetAngle;
	}
}
