using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1C RID: 3100
	[ActionCategory("Enemy AI")]
	public class DistanceFlyHorizontal : RigidBody2dActionBase
	{
		// Token: 0x06005E66 RID: 24166 RVA: 0x001DC408 File Offset: 0x001DA608
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.distance = null;
			this.yDistanceAllowance = null;
			this.startY = null;
			this.floorDistance = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x06005E67 RID: 24167 RVA: 0x001DC45F File Offset: 0x001DA65F
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E68 RID: 24168 RVA: 0x001DC46D File Offset: 0x001DA66D
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005E69 RID: 24169 RVA: 0x001DC47C File Offset: 0x001DA67C
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.layerMask = LayerMask.GetMask(new string[]
			{
				"Terrain"
			});
			this.DoBuzz();
		}

		// Token: 0x06005E6A RID: 24170 RVA: 0x001DC4E0 File Offset: 0x001DA6E0
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06005E6B RID: 24171 RVA: 0x001DC4E8 File Offset: 0x001DA6E8
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector3 position = this.self.Value.transform.position;
			float num = Mathf.Sqrt(Mathf.Pow(position.x - this.target.Value.transform.position.x, 2f));
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (num > this.distance.Value)
			{
				if (this.stayLeft.Value && !this.stayRight.Value && position.x > this.target.Value.transform.position.x + 1f)
				{
					linearVelocity.x -= this.acceleration.Value;
				}
				else if (this.stayRight.Value && !this.stayLeft.Value && position.x < this.target.Value.transform.position.x - 1f)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else if (position.x < this.target.Value.transform.position.x)
				{
					linearVelocity.x += this.acceleration.Value;
				}
				else
				{
					linearVelocity.x -= this.acceleration.Value;
				}
			}
			else if (this.stayLeft.Value && !this.stayRight.Value && this.self.Value.transform.position.x > this.target.Value.transform.position.x + 1f)
			{
				linearVelocity.x -= this.acceleration.Value;
			}
			else if (this.stayRight.Value && !this.stayLeft.Value && this.self.Value.transform.position.x < this.target.Value.transform.position.x - 1f)
			{
				linearVelocity.x += this.acceleration.Value;
			}
			else if (this.self.Value.transform.position.x < this.target.Value.transform.position.x)
			{
				linearVelocity.x -= this.acceleration.Value;
			}
			else
			{
				linearVelocity.x += this.acceleration.Value;
			}
			if (!this.dontAffectY.Value)
			{
				if (this.floorDistance.Value == 0f)
				{
					if (position.y < this.startY.Value - this.yDistanceAllowance.Value)
					{
						linearVelocity.y += this.acceleration.Value;
					}
					else if (position.y > this.startY.Value + this.yDistanceAllowance.Value)
					{
						linearVelocity.y -= this.acceleration.Value;
					}
				}
				else if (Helper.Raycast2D(position, Vector2.down, this.floorDistance.Value, this.layerMask).collider == null)
				{
					linearVelocity.y -= this.acceleration.Value;
				}
				else
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
			if (!this.dontAffectY.Value)
			{
				if (linearVelocity.y > this.speedMax.Value)
				{
					linearVelocity.y = this.speedMax.Value;
				}
				if (linearVelocity.y < -this.speedMax.Value)
				{
					linearVelocity.y = -this.speedMax.Value;
				}
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005AB7 RID: 23223
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005AB8 RID: 23224
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005AB9 RID: 23225
		public FsmFloat distance;

		// Token: 0x04005ABA RID: 23226
		public FsmFloat startY;

		// Token: 0x04005ABB RID: 23227
		public FsmFloat floorDistance;

		// Token: 0x04005ABC RID: 23228
		public FsmFloat yDistanceAllowance;

		// Token: 0x04005ABD RID: 23229
		public FsmFloat speedMax;

		// Token: 0x04005ABE RID: 23230
		public FsmFloat acceleration;

		// Token: 0x04005ABF RID: 23231
		public FsmBool stayLeft;

		// Token: 0x04005AC0 RID: 23232
		public FsmBool stayRight;

		// Token: 0x04005AC1 RID: 23233
		public FsmBool dontAffectY;

		// Token: 0x04005AC2 RID: 23234
		private FsmGameObject self;

		// Token: 0x04005AC3 RID: 23235
		private LayerMask layerMask;
	}
}
