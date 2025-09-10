using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA9 RID: 3497
	[ActionCategory("Enemy AI")]
	[Tooltip("Object idly buzzes about within a defined range")]
	public class WispBuzz : RigidBody2dActionBase
	{
		// Token: 0x0600658D RID: 25997 RVA: 0x00200ACC File Offset: 0x001FECCC
		public override void Reset()
		{
			this.gameObject = null;
			this.waitMin = 0f;
			this.waitMax = 0f;
			this.accelerationMax = 0f;
			this.roamingRangeX = 0f;
			this.roamingRangeY = 0f;
			this.manualStartPos = new FsmVector3
			{
				UseVariable = true
			};
		}

		// Token: 0x0600658E RID: 25998 RVA: 0x00200B42 File Offset: 0x001FED42
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600658F RID: 25999 RVA: 0x00200B50 File Offset: 0x001FED50
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006590 RID: 26000 RVA: 0x00200B60 File Offset: 0x001FED60
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.target = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.startX = this.target.Value.transform.position.x;
			this.startY = this.target.Value.transform.position.y;
			if (!this.manualStartPos.IsNone)
			{
				this.startX = this.manualStartPos.Value.x;
				this.startY = this.manualStartPos.Value.y;
			}
			this.offset_x = Random.Range(this.offsetX.Value.x, this.offsetX.Value.y);
			this.offset_y = Random.Range(this.offsetY.Value.x, this.offsetY.Value.y);
			this.DoBuzz();
		}

		// Token: 0x06006591 RID: 26001 RVA: 0x00200C75 File Offset: 0x001FEE75
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x00200C80 File Offset: 0x001FEE80
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (this.followTarget.Value != null)
			{
				this.startX = this.followTarget.Value.transform.position.x + this.offset_x;
				this.startY = this.followTarget.Value.transform.position.y + this.offset_y;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.target.Value.transform.position.y < this.startY - this.roamingRangeY.Value)
			{
				if (linearVelocity.y < 0f)
				{
					this.accelY = this.accelerationMax.Value;
					this.accelY /= 2000f;
					this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
				}
			}
			else if (this.target.Value.transform.position.y > this.startY + this.roamingRangeY.Value && linearVelocity.y > 0f)
			{
				this.accelY = -this.accelerationMax.Value;
				this.accelY /= 2000f;
				this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
			}
			if (this.target.Value.transform.position.x < this.startX - this.roamingRangeX.Value)
			{
				if (linearVelocity.x < 0f)
				{
					this.accelX = this.accelerationMax.Value;
					this.accelX /= 2000f;
					this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
				}
			}
			else if (this.target.Value.transform.position.x > this.startX + this.roamingRangeX.Value && linearVelocity.x > 0f)
			{
				this.accelX = -this.accelerationMax.Value;
				this.accelX /= 2000f;
				this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
			}
			if (this.waitTime <= Mathf.Epsilon)
			{
				if (this.target.Value.transform.position.y < this.startY - this.roamingRangeY.Value)
				{
					this.accelY = Random.Range(this.accelerationMin.Value, this.accelerationMax.Value);
				}
				else if (this.target.Value.transform.position.y > this.startY + this.roamingRangeY.Value)
				{
					this.accelY = Random.Range(-this.accelerationMax.Value, this.accelerationMin.Value);
				}
				else
				{
					this.accelY = Random.Range(-this.accelerationMax.Value, this.accelerationMax.Value);
				}
				if (this.target.Value.transform.position.x < this.startX - this.roamingRangeX.Value)
				{
					this.accelX = Random.Range(this.accelerationMin.Value, this.accelerationMax.Value);
				}
				else if (this.target.Value.transform.position.x > this.startX + this.roamingRangeX.Value)
				{
					this.accelX = Random.Range(-this.accelerationMax.Value, this.accelerationMin.Value);
				}
				else
				{
					this.accelX = Random.Range(-this.accelerationMax.Value, this.accelerationMax.Value);
				}
				this.accelY /= 2000f;
				this.accelX /= 2000f;
				this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
			}
			if (this.waitTime > Mathf.Epsilon)
			{
				this.waitTime -= Time.deltaTime;
			}
			linearVelocity.x += this.accelX;
			linearVelocity.y += this.accelY;
			if (linearVelocity.x > this.speedMax.Value)
			{
				linearVelocity.x = this.speedMax.Value;
			}
			if (linearVelocity.x < -this.speedMax.Value)
			{
				linearVelocity.x = -this.speedMax.Value;
			}
			if (linearVelocity.y > this.speedMax.Value)
			{
				linearVelocity.y = this.speedMax.Value;
			}
			if (linearVelocity.y < -this.speedMax.Value)
			{
				linearVelocity.y = -this.speedMax.Value;
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x040064A0 RID: 25760
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040064A1 RID: 25761
		public FsmFloat waitMin;

		// Token: 0x040064A2 RID: 25762
		public FsmFloat waitMax;

		// Token: 0x040064A3 RID: 25763
		public FsmFloat speedMax;

		// Token: 0x040064A4 RID: 25764
		public FsmFloat accelerationMin;

		// Token: 0x040064A5 RID: 25765
		public FsmFloat accelerationMax;

		// Token: 0x040064A6 RID: 25766
		public FsmFloat roamingRangeX;

		// Token: 0x040064A7 RID: 25767
		public FsmFloat roamingRangeY;

		// Token: 0x040064A8 RID: 25768
		public FsmVector3 manualStartPos;

		// Token: 0x040064A9 RID: 25769
		public FsmGameObject followTarget;

		// Token: 0x040064AA RID: 25770
		public FsmVector2 offsetX;

		// Token: 0x040064AB RID: 25771
		public FsmVector2 offsetY;

		// Token: 0x040064AC RID: 25772
		private FsmGameObject target;

		// Token: 0x040064AD RID: 25773
		private float startX;

		// Token: 0x040064AE RID: 25774
		private float startY;

		// Token: 0x040064AF RID: 25775
		private float accelX;

		// Token: 0x040064B0 RID: 25776
		private float accelY;

		// Token: 0x040064B1 RID: 25777
		private float waitTime;

		// Token: 0x040064B2 RID: 25778
		private float offset_x;

		// Token: 0x040064B3 RID: 25779
		private float offset_y;

		// Token: 0x040064B4 RID: 25780
		private const float dampener = 1.125f;
	}
}
