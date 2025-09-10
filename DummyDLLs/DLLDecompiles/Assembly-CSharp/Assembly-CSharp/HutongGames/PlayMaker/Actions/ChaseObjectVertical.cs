using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE2 RID: 3042
	[ActionCategory("Enemy AI")]
	[Tooltip("Object chases target on Y axis")]
	public class ChaseObjectVertical : RigidBody2dActionBase
	{
		// Token: 0x06005D33 RID: 23859 RVA: 0x001D4D90 File Offset: 0x001D2F90
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.offset = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x06005D34 RID: 23860 RVA: 0x001D4DC7 File Offset: 0x001D2FC7
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x001D4DD5 File Offset: 0x001D2FD5
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D36 RID: 23862 RVA: 0x001D4DE4 File Offset: 0x001D2FE4
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.offsetValue = 0f;
			if (!this.offset.IsNone)
			{
				this.offsetValue = this.offset.Value;
			}
			this.DoChase();
		}

		// Token: 0x06005D37 RID: 23863 RVA: 0x001D4E53 File Offset: 0x001D3053
		public override void OnFixedUpdate()
		{
			this.DoChase();
		}

		// Token: 0x06005D38 RID: 23864 RVA: 0x001D4E5C File Offset: 0x001D305C
		private void DoChase()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.self.Value.transform.position.y < this.target.Value.transform.position.y + this.offsetValue || this.self.Value.transform.position.y > this.target.Value.transform.position.y + this.offsetValue)
			{
				if (this.self.Value.transform.position.y < this.target.Value.transform.position.y + this.offsetValue)
				{
					linearVelocity.y += this.acceleration.Value;
				}
				else
				{
					linearVelocity.y -= this.acceleration.Value;
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
		}

		// Token: 0x040058FB RID: 22779
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x040058FC RID: 22780
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x040058FD RID: 22781
		public FsmFloat offset;

		// Token: 0x040058FE RID: 22782
		public FsmFloat speedMax;

		// Token: 0x040058FF RID: 22783
		public FsmFloat acceleration;

		// Token: 0x04005900 RID: 22784
		private FsmGameObject self;

		// Token: 0x04005901 RID: 22785
		private bool turning;

		// Token: 0x04005902 RID: 22786
		private float offsetValue;
	}
}
