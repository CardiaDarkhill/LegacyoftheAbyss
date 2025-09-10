using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C88 RID: 3208
	[ActionCategory("Enemy AI")]
	[Tooltip("Object idly buzzes about within a defined range")]
	public class IdleBuzzV3 : RigidBody2dActionBase
	{
		// Token: 0x06006081 RID: 24705 RVA: 0x001E942C File Offset: 0x001E762C
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

		// Token: 0x06006082 RID: 24706 RVA: 0x001E94A2 File Offset: 0x001E76A2
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006083 RID: 24707 RVA: 0x001E94B0 File Offset: 0x001E76B0
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006084 RID: 24708 RVA: 0x001E94C0 File Offset: 0x001E76C0
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

		// Token: 0x06006085 RID: 24709 RVA: 0x001E957F File Offset: 0x001E777F
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06006086 RID: 24710 RVA: 0x001E9588 File Offset: 0x001E7788
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			Vector3 position = this.target.Value.transform.position;
			float value = this.roamingRangeY.Value;
			float value2 = this.accelerationMax.Value;
			float value3 = this.waitMin.Value;
			float value4 = this.waitMax.Value;
			if (position.y < this.startY - value)
			{
				if (linearVelocity.y < 0f)
				{
					this.accelY = value2;
					this.accelY /= 2000f;
					linearVelocity.y /= 1.125f;
					this.waitTime = Random.Range(value3, value4);
				}
			}
			else if (position.y > this.startY + value && linearVelocity.y > 0f)
			{
				this.accelY = -value2;
				this.accelY /= 2000f;
				linearVelocity.y /= 1.125f;
				this.waitTime = Random.Range(value3, value4);
			}
			float value5 = this.roamingRangeX.Value;
			if (position.x < this.startX - value5)
			{
				if (linearVelocity.x < 0f)
				{
					this.accelX = value2;
					this.accelX /= 2000f;
					linearVelocity.x /= 1.125f;
					this.waitTime = Random.Range(value3, value4);
				}
			}
			else if (position.x > this.startX + value5 && linearVelocity.x > 0f)
			{
				this.accelX = -value2;
				this.accelX /= 2000f;
				linearVelocity.x /= 1.125f;
				this.waitTime = Random.Range(value3, value4);
			}
			if (this.waitTime <= Mathf.Epsilon)
			{
				float value6 = this.accelerationMin.Value;
				if (position.y < this.startY - value)
				{
					this.accelY = Random.Range(value6, value2);
				}
				else if (position.y > this.startY + value)
				{
					this.accelY = Random.Range(-value2, value6);
				}
				else
				{
					this.accelY = Random.Range(-value2, value2);
				}
				if (position.x < this.startX - value5)
				{
					this.accelX = Random.Range(value6, value2);
				}
				else if (position.x > this.startX + value5)
				{
					this.accelX = Random.Range(-value2, value6);
				}
				else
				{
					this.accelX = Random.Range(-value2, value2);
				}
				this.accelY /= 2000f;
				this.accelX /= 2000f;
				this.waitTime = Random.Range(value3, value4);
			}
			if (this.waitTime > Mathf.Epsilon)
			{
				this.waitTime -= Time.deltaTime;
			}
			linearVelocity.x += this.accelX;
			linearVelocity.y += this.accelY;
			float value7 = this.speedMax.Value;
			if (linearVelocity.x > value7)
			{
				linearVelocity.x = value7;
			}
			if (linearVelocity.x < -value7)
			{
				linearVelocity.x = -value7;
			}
			if (linearVelocity.y > value7)
			{
				linearVelocity.y = value7;
			}
			if (linearVelocity.y < -value7)
			{
				linearVelocity.y = -value7;
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005DF1 RID: 24049
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005DF2 RID: 24050
		public FsmFloat waitMin;

		// Token: 0x04005DF3 RID: 24051
		public FsmFloat waitMax;

		// Token: 0x04005DF4 RID: 24052
		public FsmFloat speedMax;

		// Token: 0x04005DF5 RID: 24053
		public FsmFloat accelerationMin;

		// Token: 0x04005DF6 RID: 24054
		public FsmFloat accelerationMax;

		// Token: 0x04005DF7 RID: 24055
		public FsmFloat roamingRangeX;

		// Token: 0x04005DF8 RID: 24056
		public FsmFloat roamingRangeY;

		// Token: 0x04005DF9 RID: 24057
		public FsmVector3 manualStartPos;

		// Token: 0x04005DFA RID: 24058
		private FsmGameObject target;

		// Token: 0x04005DFB RID: 24059
		private float startX;

		// Token: 0x04005DFC RID: 24060
		private float startY;

		// Token: 0x04005DFD RID: 24061
		private float accelX;

		// Token: 0x04005DFE RID: 24062
		private float accelY;

		// Token: 0x04005DFF RID: 24063
		private float waitTime;

		// Token: 0x04005E00 RID: 24064
		private const float dampener = 1.125f;
	}
}
