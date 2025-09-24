using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D65 RID: 3429
	[ActionCategory("Enemy AI")]
	public class SilkBossIdleBuzz : RigidBody2dActionBase
	{
		// Token: 0x0600643F RID: 25663 RVA: 0x001F9455 File Offset: 0x001F7655
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006440 RID: 25664 RVA: 0x001F9460 File Offset: 0x001F7660
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.tf = ownerDefaultTarget.GetComponent<Transform>();
			this.rb = ownerDefaultTarget.GetComponent<Rigidbody2D>();
			this.targetRangeInit = this.targetRange.Value;
			this.DoBuzz();
		}

		// Token: 0x06006441 RID: 25665 RVA: 0x001F94B8 File Offset: 0x001F76B8
		public override void OnUpdate()
		{
			this.DoBuzz();
		}

		// Token: 0x06006442 RID: 25666 RVA: 0x001F94C0 File Offset: 0x001F76C0
		private void DoBuzz()
		{
			Vector2 linearVelocity = this.rb.linearVelocity;
			if (this.tf.position.y > this.targetY.Value + this.targetRange.Value)
			{
				linearVelocity = new Vector2(linearVelocity.x, linearVelocity.y - this.acceleration.Value * Time.deltaTime);
				if (!this.movingDown)
				{
					if (this.fuzzTargetRange)
					{
						this.targetRange = this.targetRangeInit * Random.Range(0.75f, 1.25f);
					}
					this.movingDown = true;
				}
			}
			else if (this.tf.position.y < this.targetY.Value - this.targetRange.Value)
			{
				linearVelocity = new Vector2(linearVelocity.x, linearVelocity.y + this.acceleration.Value * Time.deltaTime);
				if (this.movingDown)
				{
					if (this.fuzzTargetRange)
					{
						this.targetRange = this.targetRangeInit * Random.Range(0.75f, 1.25f);
					}
					this.movingDown = false;
				}
			}
			else if (this.movingDown)
			{
				linearVelocity = new Vector2(linearVelocity.x, linearVelocity.y - this.acceleration.Value * Time.deltaTime);
			}
			else
			{
				linearVelocity = new Vector2(linearVelocity.x, linearVelocity.y + this.acceleration.Value * Time.deltaTime);
			}
			if (linearVelocity.y > this.speedMax.Value)
			{
				linearVelocity = new Vector2(linearVelocity.x, this.speedMax.Value);
			}
			if (linearVelocity.y < -this.speedMax.Value)
			{
				linearVelocity = new Vector2(linearVelocity.x, -this.speedMax.Value);
			}
			this.rb.linearVelocity = linearVelocity;
		}

		// Token: 0x040062B4 RID: 25268
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040062B5 RID: 25269
		public FsmFloat targetY;

		// Token: 0x040062B6 RID: 25270
		public FsmFloat targetRange;

		// Token: 0x040062B7 RID: 25271
		public FsmFloat acceleration;

		// Token: 0x040062B8 RID: 25272
		public FsmFloat speedMax;

		// Token: 0x040062B9 RID: 25273
		public bool fuzzTargetRange;

		// Token: 0x040062BA RID: 25274
		private float targetRangeInit;

		// Token: 0x040062BB RID: 25275
		private bool movingDown;

		// Token: 0x040062BC RID: 25276
		private Transform tf;

		// Token: 0x040062BD RID: 25277
		private Rigidbody2D rb;
	}
}
