using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE1 RID: 3041
	[ActionCategory("Enemy AI")]
	[Tooltip("Object moves more directly toward target")]
	public class ChaseObjectV3 : RigidBody2dActionBase
	{
		// Token: 0x06005D2C RID: 23852 RVA: 0x001D4B68 File Offset: 0x001D2D68
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.accelerationForce = 0f;
			this.speedMax = 0f;
			this.speedMin = 0f;
			this.offsetX = 0f;
			this.offsetY = 0f;
		}

		// Token: 0x06005D2D RID: 23853 RVA: 0x001D4BD3 File Offset: 0x001D2DD3
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D2E RID: 23854 RVA: 0x001D4BE1 File Offset: 0x001D2DE1
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D2F RID: 23855 RVA: 0x001D4BEF File Offset: 0x001D2DEF
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoChase();
		}

		// Token: 0x06005D30 RID: 23856 RVA: 0x001D4C2A File Offset: 0x001D2E2A
		public override void OnFixedUpdate()
		{
			this.DoChase();
		}

		// Token: 0x06005D31 RID: 23857 RVA: 0x001D4C34 File Offset: 0x001D2E34
		private void DoChase()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (this.target.Value != null)
			{
				Vector2 vector = new Vector2(this.target.Value.transform.position.x + this.offsetX.Value - this.self.Value.transform.position.x, this.target.Value.transform.position.y + this.offsetY.Value - this.self.Value.transform.position.y);
				vector = Vector2.ClampMagnitude(vector, 1f);
				vector = new Vector2(vector.x * this.accelerationForce.Value, vector.y * this.accelerationForce.Value);
				this.rb2d.AddForce(vector);
				Vector2 vector2 = this.rb2d.linearVelocity;
				vector2 = Vector2.ClampMagnitude(vector2, this.speedMax.Value);
				if (vector2.magnitude < this.speedMin.Value)
				{
					vector2 = vector2.normalized * this.speedMin.Value;
				}
				this.rb2d.linearVelocity = vector2;
			}
		}

		// Token: 0x040058F3 RID: 22771
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x040058F4 RID: 22772
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x040058F5 RID: 22773
		public FsmFloat accelerationForce;

		// Token: 0x040058F6 RID: 22774
		public FsmFloat offsetX;

		// Token: 0x040058F7 RID: 22775
		public FsmFloat offsetY;

		// Token: 0x040058F8 RID: 22776
		public FsmFloat speedMax;

		// Token: 0x040058F9 RID: 22777
		public FsmFloat speedMin;

		// Token: 0x040058FA RID: 22778
		private FsmGameObject self;
	}
}
