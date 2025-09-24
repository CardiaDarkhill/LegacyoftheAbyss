using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C86 RID: 3206
	[ActionCategory("Enemy AI")]
	[Tooltip("Object idly buzzes about within a defined range")]
	public class IdleBuzz : RigidBody2dActionBase
	{
		// Token: 0x06006073 RID: 24691 RVA: 0x001E8778 File Offset: 0x001E6978
		public override void Reset()
		{
			this.gameObject = null;
			this.waitMin = 0f;
			this.waitMax = 0f;
			this.accelerationMax = 0f;
		}

		// Token: 0x06006074 RID: 24692 RVA: 0x001E87B1 File Offset: 0x001E69B1
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006075 RID: 24693 RVA: 0x001E87BF File Offset: 0x001E69BF
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x001E87D0 File Offset: 0x001E69D0
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.target = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.startX = this.target.Value.transform.position.x;
			this.startY = this.target.Value.transform.position.y;
			this.DoBuzz();
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x001E8856 File Offset: 0x001E6A56
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06006078 RID: 24696 RVA: 0x001E8860 File Offset: 0x001E6A60
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.target.Value.transform.position.y < this.startY - this.roamingRange.Value)
			{
				if (linearVelocity.y < 0f)
				{
					this.accelY = this.accelerationMax.Value;
					this.accelY /= 2000f;
					linearVelocity.y /= 1.125f;
					this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
				}
			}
			else if (this.target.Value.transform.position.y > this.startY + this.roamingRange.Value && linearVelocity.y > 0f)
			{
				this.accelY = -this.accelerationMax.Value;
				this.accelY /= 2000f;
				linearVelocity.y /= 1.125f;
				this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
			}
			if (this.target.Value.transform.position.x < this.startX - this.roamingRange.Value)
			{
				if (linearVelocity.x < 0f)
				{
					this.accelX = this.accelerationMax.Value;
					this.accelX /= 2000f;
					linearVelocity.x /= 1.125f;
					this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
				}
			}
			else if (this.target.Value.transform.position.x > this.startX + this.roamingRange.Value && linearVelocity.x > 0f)
			{
				this.accelX = -this.accelerationMax.Value;
				this.accelX /= 2000f;
				linearVelocity.x /= 1.125f;
				this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
			}
			if (this.waitTime <= Mathf.Epsilon)
			{
				if (this.target.Value.transform.position.y < this.startY - this.roamingRange.Value)
				{
					this.accelY = Random.Range(0f, this.accelerationMax.Value);
				}
				else if (this.target.Value.transform.position.y > this.startY + this.roamingRange.Value)
				{
					this.accelY = Random.Range(-this.accelerationMax.Value, 0f);
				}
				else
				{
					this.accelY = Random.Range(-this.accelerationMax.Value, this.accelerationMax.Value);
				}
				if (this.target.Value.transform.position.x < this.startX - this.roamingRange.Value)
				{
					this.accelX = Random.Range(0f, this.accelerationMax.Value);
				}
				else if (this.target.Value.transform.position.x > this.startX + this.roamingRange.Value)
				{
					this.accelX = Random.Range(-this.accelerationMax.Value, 0f);
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

		// Token: 0x04005DD5 RID: 24021
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005DD6 RID: 24022
		public FsmFloat waitMin;

		// Token: 0x04005DD7 RID: 24023
		public FsmFloat waitMax;

		// Token: 0x04005DD8 RID: 24024
		public FsmFloat speedMax;

		// Token: 0x04005DD9 RID: 24025
		public FsmFloat accelerationMax;

		// Token: 0x04005DDA RID: 24026
		public FsmFloat roamingRange;

		// Token: 0x04005DDB RID: 24027
		private FsmGameObject target;

		// Token: 0x04005DDC RID: 24028
		private float startX;

		// Token: 0x04005DDD RID: 24029
		private float startY;

		// Token: 0x04005DDE RID: 24030
		private float accelX;

		// Token: 0x04005DDF RID: 24031
		private float accelY;

		// Token: 0x04005DE0 RID: 24032
		private float waitTime;

		// Token: 0x04005DE1 RID: 24033
		private const float dampener = 1.125f;
	}
}
