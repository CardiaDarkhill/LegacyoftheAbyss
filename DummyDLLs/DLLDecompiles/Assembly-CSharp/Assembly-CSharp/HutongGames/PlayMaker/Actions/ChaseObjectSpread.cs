using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDF RID: 3039
	[ActionCategory("Enemy AI")]
	[Tooltip("Object buzzes towards target")]
	public class ChaseObjectSpread : RigidBody2dActionBase
	{
		// Token: 0x06005D1E RID: 23838 RVA: 0x001D4684 File Offset: 0x001D2884
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x06005D1F RID: 23839 RVA: 0x001D46B4 File Offset: 0x001D28B4
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D20 RID: 23840 RVA: 0x001D46C2 File Offset: 0x001D28C2
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D21 RID: 23841 RVA: 0x001D46D0 File Offset: 0x001D28D0
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoBuzz();
		}

		// Token: 0x06005D22 RID: 23842 RVA: 0x001D470B File Offset: 0x001D290B
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06005D23 RID: 23843 RVA: 0x001D4714 File Offset: 0x001D2914
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (this.targetSpreadX.Value > 0f || this.targetSpreadY.Value > 0f)
			{
				if (this.timer >= this.spreadResetTime)
				{
					this.spreadX = Random.Range(-this.targetSpreadX.Value, this.targetSpreadX.Value);
					this.spreadY = Random.Range(-this.targetSpreadY.Value, this.targetSpreadY.Value);
					this.timer = 0f;
					this.spreadResetTime = Random.Range(this.spreadResetTimeMin.Value, this.spreadResetTimeMax.Value);
				}
				else
				{
					this.timer += Time.deltaTime;
				}
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			if (this.self.Value.transform.position.x < this.target.Value.transform.position.x + this.spreadX + this.offsetX.Value)
			{
				linearVelocity.x += this.acceleration.Value;
			}
			else
			{
				linearVelocity.x -= this.acceleration.Value;
			}
			if (this.self.Value.transform.position.y < this.target.Value.transform.position.y + this.spreadY + this.offsetY.Value)
			{
				linearVelocity.y += this.acceleration.Value;
			}
			else
			{
				linearVelocity.y -= this.acceleration.Value;
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

		// Token: 0x040058DC RID: 22748
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x040058DD RID: 22749
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x040058DE RID: 22750
		public FsmFloat speedMax;

		// Token: 0x040058DF RID: 22751
		public FsmFloat acceleration;

		// Token: 0x040058E0 RID: 22752
		public FsmFloat targetSpreadX;

		// Token: 0x040058E1 RID: 22753
		public FsmFloat targetSpreadY;

		// Token: 0x040058E2 RID: 22754
		public FsmFloat spreadResetTimeMin;

		// Token: 0x040058E3 RID: 22755
		public FsmFloat spreadResetTimeMax;

		// Token: 0x040058E4 RID: 22756
		private float spreadResetTime;

		// Token: 0x040058E5 RID: 22757
		private float spreadX;

		// Token: 0x040058E6 RID: 22758
		private float spreadY;

		// Token: 0x040058E7 RID: 22759
		public FsmFloat offsetX;

		// Token: 0x040058E8 RID: 22760
		public FsmFloat offsetY;

		// Token: 0x040058E9 RID: 22761
		private FsmGameObject self;

		// Token: 0x040058EA RID: 22762
		private float timer;

		// Token: 0x040058EB RID: 22763
		private float spreadResetTimer;
	}
}
