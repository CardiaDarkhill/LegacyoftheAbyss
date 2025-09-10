using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C89 RID: 3209
	[ActionCategory("Enemy AI")]
	[Tooltip("Object idly buzzes about within a defined range")]
	public class IdleBuzzV4 : RigidBody2dActionBase
	{
		// Token: 0x06006088 RID: 24712 RVA: 0x001E9918 File Offset: 0x001E7B18
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

		// Token: 0x06006089 RID: 24713 RVA: 0x001E998E File Offset: 0x001E7B8E
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600608A RID: 24714 RVA: 0x001E999C File Offset: 0x001E7B9C
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600608B RID: 24715 RVA: 0x001E99AC File Offset: 0x001E7BAC
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
			this.DoBuzz();
		}

		// Token: 0x0600608C RID: 24716 RVA: 0x001E9A6B File Offset: 0x001E7C6B
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x0600608D RID: 24717 RVA: 0x001E9A74 File Offset: 0x001E7C74
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.target.Value.transform.position.y < this.startY - this.roamingRangeY.Value)
			{
				if (linearVelocity.y < 0f)
				{
					this.accelY = this.accelerationMax.Value;
					this.accelY /= 2000f;
					linearVelocity.y /= 1.025f;
					this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
				}
			}
			else if (this.target.Value.transform.position.y > this.startY + this.roamingRangeY.Value && linearVelocity.y > 0f)
			{
				this.accelY = -this.accelerationMax.Value;
				this.accelY /= 2000f;
				linearVelocity.y /= 1.025f;
				this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
			}
			if (this.target.Value.transform.position.x < this.startX - this.roamingRangeX.Value)
			{
				if (linearVelocity.x < 0f)
				{
					this.accelX = this.accelerationMax.Value;
					this.accelX /= 2000f;
					linearVelocity.x /= 1.025f;
					this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
				}
			}
			else if (this.target.Value.transform.position.x > this.startX + this.roamingRangeX.Value && linearVelocity.x > 0f)
			{
				this.accelX = -this.accelerationMax.Value;
				this.accelX /= 2000f;
				linearVelocity.x /= 1.025f;
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
				linearVelocity.x *= this.deceleration.Value;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			if (linearVelocity.x < -this.speedMax.Value)
			{
				linearVelocity.x *= this.deceleration.Value;
				if (linearVelocity.x > 0f)
				{
					linearVelocity.x = 0f;
				}
			}
			if (linearVelocity.y > this.speedMax.Value)
			{
				linearVelocity.y *= this.deceleration.Value;
				if (linearVelocity.y < 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			if (linearVelocity.y < -this.speedMax.Value)
			{
				linearVelocity.y *= this.deceleration.Value;
				if (linearVelocity.y > 0f)
				{
					linearVelocity.y = 0f;
				}
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005E01 RID: 24065
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005E02 RID: 24066
		public FsmFloat waitMin;

		// Token: 0x04005E03 RID: 24067
		public FsmFloat waitMax;

		// Token: 0x04005E04 RID: 24068
		public FsmFloat speedMax;

		// Token: 0x04005E05 RID: 24069
		public FsmFloat deceleration;

		// Token: 0x04005E06 RID: 24070
		public FsmFloat accelerationMin;

		// Token: 0x04005E07 RID: 24071
		public FsmFloat accelerationMax;

		// Token: 0x04005E08 RID: 24072
		public FsmFloat roamingRangeX;

		// Token: 0x04005E09 RID: 24073
		public FsmFloat roamingRangeY;

		// Token: 0x04005E0A RID: 24074
		public FsmVector3 manualStartPos;

		// Token: 0x04005E0B RID: 24075
		private FsmGameObject target;

		// Token: 0x04005E0C RID: 24076
		private float startX;

		// Token: 0x04005E0D RID: 24077
		private float startY;

		// Token: 0x04005E0E RID: 24078
		private float accelX;

		// Token: 0x04005E0F RID: 24079
		private float accelY;

		// Token: 0x04005E10 RID: 24080
		private float waitTime;

		// Token: 0x04005E11 RID: 24081
		private const float dampener = 1.025f;
	}
}
