using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BDD RID: 3037
	[ActionCategory("Enemy AI")]
	[Tooltip("Object buzzes towards target")]
	public class ChaseObject : RigidBody2dActionBase
	{
		// Token: 0x06005D0F RID: 23823 RVA: 0x001D3F5C File Offset: 0x001D215C
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.acceleration = 0f;
			this.speedMax = 0f;
		}

		// Token: 0x06005D10 RID: 23824 RVA: 0x001D3F8C File Offset: 0x001D218C
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D11 RID: 23825 RVA: 0x001D3F9A File Offset: 0x001D219A
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005D12 RID: 23826 RVA: 0x001D3FA8 File Offset: 0x001D21A8
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.self = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoBuzz();
		}

		// Token: 0x06005D13 RID: 23827 RVA: 0x001D3FE3 File Offset: 0x001D21E3
		public override void OnFixedUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06005D14 RID: 23828 RVA: 0x001D3FEC File Offset: 0x001D21EC
		private void DoBuzz()
		{
			if (this.rb2d == null)
			{
				return;
			}
			if (this.targetSpread.Value > 0f)
			{
				if (this.timer >= this.spreadResetTime)
				{
					this.spreadX = Random.Range(-this.targetSpread.Value, this.targetSpread.Value);
					this.spreadY = Random.Range(-this.targetSpread.Value, this.targetSpread.Value);
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

		// Token: 0x040058BB RID: 22715
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault gameObject;

		// Token: 0x040058BC RID: 22716
		[UIHint(UIHint.Variable)]
		public FsmGameObject target;

		// Token: 0x040058BD RID: 22717
		public FsmFloat speedMax;

		// Token: 0x040058BE RID: 22718
		public FsmFloat acceleration;

		// Token: 0x040058BF RID: 22719
		public FsmFloat targetSpread;

		// Token: 0x040058C0 RID: 22720
		public FsmFloat spreadResetTimeMin;

		// Token: 0x040058C1 RID: 22721
		public FsmFloat spreadResetTimeMax;

		// Token: 0x040058C2 RID: 22722
		private bool spreadSet;

		// Token: 0x040058C3 RID: 22723
		private float spreadResetTime;

		// Token: 0x040058C4 RID: 22724
		private float spreadX;

		// Token: 0x040058C5 RID: 22725
		private float spreadY;

		// Token: 0x040058C6 RID: 22726
		public FsmFloat offsetX;

		// Token: 0x040058C7 RID: 22727
		public FsmFloat offsetY;

		// Token: 0x040058C8 RID: 22728
		private FsmGameObject self;

		// Token: 0x040058C9 RID: 22729
		private float timer;

		// Token: 0x040058CA RID: 22730
		private float spreadResetTimer;
	}
}
