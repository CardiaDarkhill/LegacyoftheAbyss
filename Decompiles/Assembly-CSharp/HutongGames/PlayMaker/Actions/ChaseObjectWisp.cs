using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE3 RID: 3043
	[ActionCategory("Enemy AI")]
	[Tooltip("Object moves more directly toward target")]
	public class ChaseObjectWisp : RigidBody2dActionBase
	{
		// Token: 0x06005D3A RID: 23866 RVA: 0x001D4FCC File Offset: 0x001D31CC
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

		// Token: 0x06005D3B RID: 23867 RVA: 0x001D5037 File Offset: 0x001D3237
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D3C RID: 23868 RVA: 0x001D5045 File Offset: 0x001D3245
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D3D RID: 23869 RVA: 0x001D5054 File Offset: 0x001D3254
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.speedMin.Value = 0f;
			this.DoChase();
		}

		// Token: 0x06005D3E RID: 23870 RVA: 0x001D50AA File Offset: 0x001D32AA
		public override void OnFixedUpdate()
		{
			this.DoChase();
		}

		// Token: 0x06005D3F RID: 23871 RVA: 0x001D50B4 File Offset: 0x001D32B4
		private void DoChase()
		{
			if (this.rb2d == null || this.target.Value == null)
			{
				return;
			}
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
			float magnitude = this.rb2d.linearVelocity.magnitude;
			if (magnitude > this.speedMin.Value)
			{
				this.speedMin.Value = magnitude;
			}
		}

		// Token: 0x04005903 RID: 22787
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005904 RID: 22788
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x04005905 RID: 22789
		public FsmFloat accelerationForce;

		// Token: 0x04005906 RID: 22790
		public FsmFloat offsetX;

		// Token: 0x04005907 RID: 22791
		public FsmFloat offsetY;

		// Token: 0x04005908 RID: 22792
		public FsmFloat speedMax;

		// Token: 0x04005909 RID: 22793
		public FsmFloat speedMin;

		// Token: 0x0400590A RID: 22794
		private FsmGameObject self;
	}
}
