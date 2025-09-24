using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D8C RID: 3468
	[ActionCategory(ActionCategory.Physics2D)]
	public class TiltBySpeedY : ComponentAction<Rigidbody2D>
	{
		// Token: 0x060064EA RID: 25834 RVA: 0x001FD6EE File Offset: 0x001FB8EE
		public override void Reset()
		{
			this.gameObject = null;
			this.tiltFactor = null;
			this.tiltMax = null;
		}

		// Token: 0x060064EB RID: 25835 RVA: 0x001FD705 File Offset: 0x001FB905
		public override void OnEnter()
		{
			this.DoTilt();
		}

		// Token: 0x060064EC RID: 25836 RVA: 0x001FD70D File Offset: 0x001FB90D
		public override void OnUpdate()
		{
			this.DoTilt();
		}

		// Token: 0x060064ED RID: 25837 RVA: 0x001FD718 File Offset: 0x001FB918
		private void DoTilt()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			float num = base.rigidbody2d.linearVelocity.y * this.tiltFactor.Value;
			if (ownerDefaultTarget.transform.localScale.x < 0f)
			{
				num *= -1f;
			}
			if (num > this.tiltMax.Value)
			{
				num = this.tiltMax.Value;
			}
			if (num < -this.tiltMax.Value)
			{
				num = -this.tiltMax.Value;
			}
			this.DoRotateTo(num);
		}

		// Token: 0x060064EE RID: 25838 RVA: 0x001FD7BC File Offset: 0x001FB9BC
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

		// Token: 0x040063E3 RID: 25571
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040063E4 RID: 25572
		public FsmFloat tiltFactor;

		// Token: 0x040063E5 RID: 25573
		public FsmFloat tiltMax;

		// Token: 0x040063E6 RID: 25574
		public FsmFloat rotationSpeed;

		// Token: 0x040063E7 RID: 25575
		private float targetAngle;
	}
}
