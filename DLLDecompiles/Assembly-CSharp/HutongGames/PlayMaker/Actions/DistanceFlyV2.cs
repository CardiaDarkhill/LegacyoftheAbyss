using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1E RID: 3102
	[ActionCategory("Enemy AI")]
	[Tooltip("Try to keep a certain distance from target. Optionally try to stay on left or right of target")]
	public class DistanceFlyV2 : RigidBody2dActionBase
	{
		// Token: 0x06005E74 RID: 24180 RVA: 0x001DCD68 File Offset: 0x001DAF68
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.targetsHeight = false;
			this.height = null;
			this.maxHeight = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x06005E75 RID: 24181 RVA: 0x001DCDB8 File Offset: 0x001DAFB8
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E76 RID: 24182 RVA: 0x001DCDC6 File Offset: 0x001DAFC6
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E77 RID: 24183 RVA: 0x001DCDD4 File Offset: 0x001DAFD4
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoBuzz();
		}

		// Token: 0x06005E78 RID: 24184 RVA: 0x001DCE0F File Offset: 0x001DB00F
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06005E79 RID: 24185 RVA: 0x001DCE18 File Offset: 0x001DB018
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			GameObject value = this.target.Value;
			if (value == null)
			{
				return;
			}
			GameObject value2 = this.self.Value;
			value2 == null;
			this.distanceAway = Mathf.Sqrt(Mathf.Pow(value2.transform.position.x - value.transform.position.x, 2f) + Mathf.Pow(value2.transform.position.y - value.transform.position.y, 2f));
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.distanceAway > this.distance.Value)
			{
				if (this.stayLeft.Value && !this.stayRight.Value && value2.transform.position.x > value.transform.position.x + 1f)
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				else if (this.stayRight.Value && !this.stayLeft.Value && value2.transform.position.x < value.transform.position.x - 1f)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else if (value2.transform.position.x < value.transform.position.x)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				if (!this.targetsHeight)
				{
					if (value2.transform.position.y < value.transform.position.y)
					{
						linearVelocity.y += this.acceleration.Value;
					}
					else
					{
						linearVelocity.y -= this.acceleration.Value;
					}
				}
			}
			else
			{
				if (this.stayLeft.Value && !this.stayRight.Value && value2.transform.position.x > value.transform.position.x + 1f)
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				else if (this.stayRight.Value && !this.stayLeft.Value && value2.transform.position.x < value.transform.position.x - 1f)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else if (value2.transform.position.x < value.transform.position.x)
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				else
				{
					linearVelocity.x += this.acceleration.Value;
				}
				if (!this.targetsHeight)
				{
					if (value2.transform.position.y < value.transform.position.y)
					{
						linearVelocity.y -= this.acceleration.Value;
					}
					else
					{
						linearVelocity.y += this.acceleration.Value;
					}
				}
			}
			if (this.targetsHeight)
			{
				if (value2.transform.position.y > value.transform.position.y + this.height.Value)
				{
					linearVelocity.y -= this.acceleration.Value;
				}
				else if (!this.maxHeight.IsNone && this.maxHeight.Value != 0f && value2.transform.position.y > this.maxHeight.Value)
				{
					linearVelocity.y -= this.acceleration.Value;
				}
				else if (value2.transform.position.y < value.transform.position.y + this.height.Value)
				{
					linearVelocity.y += this.acceleration.Value;
				}
			}
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

		// Token: 0x04005ACE RID: 23246
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005ACF RID: 23247
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005AD0 RID: 23248
		public FsmFloat distance;

		// Token: 0x04005AD1 RID: 23249
		public FsmFloat speedMax;

		// Token: 0x04005AD2 RID: 23250
		public FsmFloat acceleration;

		// Token: 0x04005AD3 RID: 23251
		[Tooltip("If true, object tries to keep to a certain height relative to target")]
		public bool targetsHeight;

		// Token: 0x04005AD4 RID: 23252
		public FsmFloat height;

		// Token: 0x04005AD5 RID: 23253
		[UIHint(UIHint.Variable)]
		public FsmFloat maxHeight;

		// Token: 0x04005AD6 RID: 23254
		public FsmBool stayLeft;

		// Token: 0x04005AD7 RID: 23255
		public FsmBool stayRight;

		// Token: 0x04005AD8 RID: 23256
		private float distanceAway;

		// Token: 0x04005AD9 RID: 23257
		private FsmGameObject self;
	}
}
