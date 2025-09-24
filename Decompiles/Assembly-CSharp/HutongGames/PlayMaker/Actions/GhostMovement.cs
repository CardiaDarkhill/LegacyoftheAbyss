using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C83 RID: 3203
	[ActionCategory("Enemy AI")]
	[Tooltip("Movement for swaying ghosts, like zote salubra")]
	public class GhostMovement : RigidBody2dActionBase
	{
		// Token: 0x06006063 RID: 24675 RVA: 0x001E7F9C File Offset: 0x001E619C
		public override void Reset()
		{
			this.gameObject = null;
			this.xPosMin = null;
			this.xPosMax = null;
			this.accel_x = null;
			this.speedMax_x = null;
			this.yPosMin = null;
			this.yPosMax = null;
			this.accel_y = null;
			this.speedMax_y = null;
			this.target = null;
			this.direction_x = null;
			this.direction_y = null;
		}

		// Token: 0x06006064 RID: 24676 RVA: 0x001E7FFD File Offset: 0x001E61FD
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006065 RID: 24677 RVA: 0x001E800B File Offset: 0x001E620B
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006066 RID: 24678 RVA: 0x001E801C File Offset: 0x001E621C
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.target = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.transform = this.target.Value.GetComponent<Transform>();
			this.DoMove();
		}

		// Token: 0x06006067 RID: 24679 RVA: 0x001E8078 File Offset: 0x001E6278
		public override void OnFixedUpdate()
		{
			this.DoMove();
		}

		// Token: 0x06006068 RID: 24680 RVA: 0x001E8080 File Offset: 0x001E6280
		private void DoMove()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			Vector3 position = this.transform.position;
			if (this.direction_x.Value == 0)
			{
				if (linearVelocity.x > -this.speedMax_x.Value)
				{
					linearVelocity.x -= this.accel_x.Value;
					if (linearVelocity.x < -this.speedMax_x.Value)
					{
						linearVelocity.x = -this.speedMax_x.Value;
					}
				}
				if (position.x < this.xPosMin.Value)
				{
					this.direction_x.Value = 1;
				}
			}
			else
			{
				if (linearVelocity.x < this.speedMax_x.Value)
				{
					linearVelocity.x += this.accel_x.Value;
					if (linearVelocity.x > this.speedMax_x.Value)
					{
						linearVelocity.x = this.speedMax_x.Value;
					}
				}
				if (position.x > this.xPosMax.Value)
				{
					this.direction_x.Value = 0;
				}
			}
			if (this.direction_y.Value == 0)
			{
				if (linearVelocity.y > -this.speedMax_y.Value)
				{
					linearVelocity.y -= this.accel_y.Value;
					if (linearVelocity.y < -this.speedMax_y.Value)
					{
						linearVelocity.y = -this.speedMax_y.Value;
					}
				}
				if (position.y < this.yPosMin.Value)
				{
					this.direction_y.Value = 1;
				}
			}
			else
			{
				if (linearVelocity.y < this.speedMax_y.Value)
				{
					linearVelocity.y += this.accel_y.Value;
					if (linearVelocity.y > this.speedMax_y.Value)
					{
						linearVelocity.y = this.speedMax_y.Value;
					}
				}
				if (position.y > this.yPosMax.Value)
				{
					this.direction_y.Value = 0;
				}
			}
			this.rb2d.linearVelocity = linearVelocity;
		}

		// Token: 0x04005DB7 RID: 23991
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005DB8 RID: 23992
		public FsmFloat xPosMin;

		// Token: 0x04005DB9 RID: 23993
		public FsmFloat xPosMax;

		// Token: 0x04005DBA RID: 23994
		public FsmFloat accel_x;

		// Token: 0x04005DBB RID: 23995
		public FsmFloat speedMax_x;

		// Token: 0x04005DBC RID: 23996
		public FsmFloat yPosMin;

		// Token: 0x04005DBD RID: 23997
		public FsmFloat yPosMax;

		// Token: 0x04005DBE RID: 23998
		public FsmFloat accel_y;

		// Token: 0x04005DBF RID: 23999
		public FsmFloat speedMax_y;

		// Token: 0x04005DC0 RID: 24000
		private FsmGameObject target;

		// Token: 0x04005DC1 RID: 24001
		private Transform transform;

		// Token: 0x04005DC2 RID: 24002
		public FsmInt direction_x;

		// Token: 0x04005DC3 RID: 24003
		public FsmInt direction_y;
	}
}
