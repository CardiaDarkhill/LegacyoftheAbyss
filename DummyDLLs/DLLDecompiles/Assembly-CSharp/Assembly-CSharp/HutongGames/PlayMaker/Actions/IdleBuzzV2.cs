using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C87 RID: 3207
	[ActionCategory("Enemy AI")]
	[Tooltip("Object idly buzzes about within a defined range")]
	public class IdleBuzzV2 : RigidBody2dActionBase
	{
		// Token: 0x0600607A RID: 24698 RVA: 0x001E8D98 File Offset: 0x001E6F98
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

		// Token: 0x0600607B RID: 24699 RVA: 0x001E8E0E File Offset: 0x001E700E
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600607C RID: 24700 RVA: 0x001E8E1C File Offset: 0x001E701C
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600607D RID: 24701 RVA: 0x001E8E2C File Offset: 0x001E702C
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

		// Token: 0x0600607E RID: 24702 RVA: 0x001E8EEB File Offset: 0x001E70EB
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x0600607F RID: 24703 RVA: 0x001E8EF4 File Offset: 0x001E70F4
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
					linearVelocity.y /= 1.125f;
					this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
				}
			}
			else if (this.target.Value.transform.position.y > this.startY + this.roamingRangeY.Value && linearVelocity.y > 0f)
			{
				this.accelY = -this.accelerationMax.Value;
				this.accelY /= 2000f;
				linearVelocity.y /= 1.125f;
				this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
			}
			if (this.target.Value.transform.position.x < this.startX - this.roamingRangeX.Value)
			{
				if (linearVelocity.x < 0f)
				{
					this.accelX = this.accelerationMax.Value;
					this.accelX /= 2000f;
					linearVelocity.x /= 1.125f;
					this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
				}
			}
			else if (this.target.Value.transform.position.x > this.startX + this.roamingRangeX.Value && linearVelocity.x > 0f)
			{
				this.accelX = -this.accelerationMax.Value;
				this.accelX /= 2000f;
				linearVelocity.x /= 1.125f;
				this.waitTime = Random.Range(this.waitMin.Value, this.waitMax.Value);
			}
			if (this.waitTime <= Mathf.Epsilon)
			{
				if (this.target.Value.transform.position.y < this.startY - this.roamingRangeY.Value)
				{
					this.accelY = Random.Range(0f, this.accelerationMax.Value);
				}
				else if (this.target.Value.transform.position.y > this.startY + this.roamingRangeY.Value)
				{
					this.accelY = Random.Range(-this.accelerationMax.Value, 0f);
				}
				else
				{
					this.accelY = Random.Range(-this.accelerationMax.Value, this.accelerationMax.Value);
				}
				if (this.target.Value.transform.position.x < this.startX - this.roamingRangeX.Value)
				{
					this.accelX = Random.Range(0f, this.accelerationMax.Value);
				}
				else if (this.target.Value.transform.position.x > this.startX + this.roamingRangeX.Value)
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

		// Token: 0x04005DE2 RID: 24034
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005DE3 RID: 24035
		public FsmFloat waitMin;

		// Token: 0x04005DE4 RID: 24036
		public FsmFloat waitMax;

		// Token: 0x04005DE5 RID: 24037
		public FsmFloat speedMax;

		// Token: 0x04005DE6 RID: 24038
		public FsmFloat accelerationMax;

		// Token: 0x04005DE7 RID: 24039
		public FsmFloat roamingRangeX;

		// Token: 0x04005DE8 RID: 24040
		public FsmFloat roamingRangeY;

		// Token: 0x04005DE9 RID: 24041
		public FsmVector3 manualStartPos;

		// Token: 0x04005DEA RID: 24042
		private FsmGameObject target;

		// Token: 0x04005DEB RID: 24043
		private float startX;

		// Token: 0x04005DEC RID: 24044
		private float startY;

		// Token: 0x04005DED RID: 24045
		private float accelX;

		// Token: 0x04005DEE RID: 24046
		private float accelY;

		// Token: 0x04005DEF RID: 24047
		private float waitTime;

		// Token: 0x04005DF0 RID: 24048
		private const float dampener = 1.125f;
	}
}
