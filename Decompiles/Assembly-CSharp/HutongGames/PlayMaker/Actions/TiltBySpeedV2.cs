using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D8B RID: 3467
	[ActionCategory(ActionCategory.Physics2D)]
	public class TiltBySpeedV2 : ComponentAction<Rigidbody2D>
	{
		// Token: 0x060064E4 RID: 25828 RVA: 0x001FD4F4 File Offset: 0x001FB6F4
		public override void Reset()
		{
			this.gameObjectToTilt = null;
			this.getSpeedFrom = null;
			this.tiltFactor = null;
			this.tiltMax = null;
		}

		// Token: 0x060064E5 RID: 25829 RVA: 0x001FD512 File Offset: 0x001FB712
		public override void OnEnter()
		{
			this.target_transform = this.gameObjectToTilt.Value.GetComponent<Transform>();
			this.rb = this.getSpeedFrom.Value.GetComponent<Rigidbody2D>();
			this.DoTilt();
		}

		// Token: 0x060064E6 RID: 25830 RVA: 0x001FD546 File Offset: 0x001FB746
		public override void OnUpdate()
		{
			this.DoTilt();
		}

		// Token: 0x060064E7 RID: 25831 RVA: 0x001FD550 File Offset: 0x001FB750
		private void DoTilt()
		{
			float num = this.rb.linearVelocity.x * this.tiltFactor.Value;
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

		// Token: 0x060064E8 RID: 25832 RVA: 0x001FD5C4 File Offset: 0x001FB7C4
		private void DoRotateTo(float targetAngle)
		{
			float num = targetAngle - this.target_transform.localEulerAngles.z;
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
				this.target_transform.Rotate(0f, 0f, this.rotationSpeed.Value * Time.deltaTime);
				if (this.target_transform.localEulerAngles.z > targetAngle)
				{
					this.target_transform.localEulerAngles = new Vector3(this.target_transform.rotation.x, this.target_transform.rotation.y, targetAngle);
					return;
				}
			}
			else
			{
				this.target_transform.Rotate(0f, 0f, -this.rotationSpeed.Value * Time.deltaTime);
				if (this.target_transform.localEulerAngles.z < targetAngle)
				{
					this.target_transform.localEulerAngles = new Vector3(this.target_transform.rotation.x, this.target_transform.rotation.y, targetAngle);
				}
			}
		}

		// Token: 0x040063DB RID: 25563
		[RequiredField]
		public FsmGameObject gameObjectToTilt;

		// Token: 0x040063DC RID: 25564
		public FsmGameObject getSpeedFrom;

		// Token: 0x040063DD RID: 25565
		public FsmFloat tiltFactor;

		// Token: 0x040063DE RID: 25566
		public FsmFloat tiltMax;

		// Token: 0x040063DF RID: 25567
		public FsmFloat rotationSpeed;

		// Token: 0x040063E0 RID: 25568
		private float targetAngle;

		// Token: 0x040063E1 RID: 25569
		private Transform target_transform;

		// Token: 0x040063E2 RID: 25570
		private Rigidbody2D rb;
	}
}
