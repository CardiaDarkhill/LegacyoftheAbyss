using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE3 RID: 3299
	[ActionCategory("Enemy AI")]
	[Tooltip("Squash projectile to match speed")]
	public class ProjectileSquashV2 : RigidBody2dActionBase
	{
		// Token: 0x0600621A RID: 25114 RVA: 0x001F0363 File Offset: 0x001EE563
		public override void Reset()
		{
			this.gameObject = null;
			this.scaleModifier = 1f;
			this.everyFrame = false;
			this.stretchX = 1f;
			this.stretchY = 1f;
		}

		// Token: 0x0600621B RID: 25115 RVA: 0x001F0399 File Offset: 0x001EE599
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600621C RID: 25116 RVA: 0x001F03A8 File Offset: 0x001EE5A8
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.target = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoStretch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600621D RID: 25117 RVA: 0x001F03FC File Offset: 0x001EE5FC
		public override void OnFixedUpdate()
		{
			this.DoStretch();
		}

		// Token: 0x0600621E RID: 25118 RVA: 0x001F0404 File Offset: 0x001EE604
		private void DoStretch()
		{
			if (this.rb2d == null)
			{
				return;
			}
			this.stretchY = 1f - this.rb2d.linearVelocity.magnitude * this.stretchFactor.Value * 0.01f;
			this.stretchX = 1f + this.rb2d.linearVelocity.magnitude * this.stretchFactor.Value * 0.01f;
			if (this.stretchX < this.stretchMinY)
			{
				this.stretchY = this.stretchMinY;
			}
			if (this.stretchX > this.stretchMaxX)
			{
				this.stretchX = this.stretchMaxX;
			}
			this.stretchY *= this.scaleModifier.Value;
			this.stretchX *= this.scaleModifier.Value;
			if (!this.otherGameObject.IsNone && this.otherGameObject.Value != null)
			{
				this.otherGameObject.Value.transform.localScale = new Vector3(this.stretchX, this.stretchY, this.target.Value.transform.localScale.z);
				return;
			}
			this.target.Value.transform.localScale = new Vector3(this.stretchX, this.stretchY, this.target.Value.transform.localScale.z);
		}

		// Token: 0x0400602F RID: 24623
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006030 RID: 24624
		[Tooltip("Increase this value to make the object's stretch more pronounced")]
		public FsmFloat stretchFactor = 1.4f;

		// Token: 0x04006031 RID: 24625
		public float stretchMinY = 0.5f;

		// Token: 0x04006032 RID: 24626
		public float stretchMaxX = 2f;

		// Token: 0x04006033 RID: 24627
		[Tooltip("After other calculations, multiply scale by this modifier.")]
		public FsmFloat scaleModifier;

		// Token: 0x04006034 RID: 24628
		[Tooltip("Optionally, stretch another object based on our own velocity")]
		public FsmGameObject otherGameObject;

		// Token: 0x04006035 RID: 24629
		public bool everyFrame;

		// Token: 0x04006036 RID: 24630
		private FsmGameObject target;

		// Token: 0x04006037 RID: 24631
		private float stretchX = 1f;

		// Token: 0x04006038 RID: 24632
		private float stretchY = 1f;
	}
}
