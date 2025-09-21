using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE0 RID: 3040
	[ActionCategory("Enemy AI")]
	[Tooltip("Object moves more directly toward target")]
	public class ChaseObjectV2 : RigidBody2dActionBase
	{
		// Token: 0x06005D25 RID: 23845 RVA: 0x001D4990 File Offset: 0x001D2B90
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.accelerationForce = 0f;
			this.speedMax = 0f;
			this.offsetX = 0f;
			this.offsetY = 0f;
		}

		// Token: 0x06005D26 RID: 23846 RVA: 0x001D49EB File Offset: 0x001D2BEB
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D27 RID: 23847 RVA: 0x001D49F9 File Offset: 0x001D2BF9
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D28 RID: 23848 RVA: 0x001D4A07 File Offset: 0x001D2C07
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoChase();
		}

		// Token: 0x06005D29 RID: 23849 RVA: 0x001D4A42 File Offset: 0x001D2C42
		public override void OnFixedUpdate()
		{
			this.DoChase();
		}

		// Token: 0x06005D2A RID: 23850 RVA: 0x001D4A4C File Offset: 0x001D2C4C
		private void DoChase()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 vector = new Vector2(this.target.Value.transform.position.x + this.offsetX.Value - this.self.Value.transform.position.x, this.target.Value.transform.position.y + this.offsetY.Value - this.self.Value.transform.position.y);
			vector = Vector2.ClampMagnitude(vector, 1f);
			vector = new Vector2(vector.x * this.accelerationForce.Value, vector.y * this.accelerationForce.Value);
			this.rb2d.AddForce(vector);
			Vector2 vector2 = this.rb2d.linearVelocity;
			vector2 = Vector2.ClampMagnitude(vector2, this.speedMax.Value);
			this.rb2d.linearVelocity = vector2;
		}

		// Token: 0x040058EC RID: 22764
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x040058ED RID: 22765
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x040058EE RID: 22766
		public FsmFloat speedMax;

		// Token: 0x040058EF RID: 22767
		public FsmFloat accelerationForce;

		// Token: 0x040058F0 RID: 22768
		public FsmFloat offsetX;

		// Token: 0x040058F1 RID: 22769
		public FsmFloat offsetY;

		// Token: 0x040058F2 RID: 22770
		private FsmGameObject self;
	}
}
