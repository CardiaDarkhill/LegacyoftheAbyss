using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE2 RID: 3298
	[ActionCategory("Enemy AI")]
	[Tooltip("Squash projectile to match speed")]
	public class ProjectileSquash : RigidBody2dActionBase
	{
		// Token: 0x06006214 RID: 25108 RVA: 0x001F00EE File Offset: 0x001EE2EE
		public override void Reset()
		{
			this.gameObject = null;
			this.scaleModifier = 1f;
			this.everyFrame = false;
			this.stretchX = 1f;
			this.stretchY = 1f;
		}

		// Token: 0x06006215 RID: 25109 RVA: 0x001F0124 File Offset: 0x001EE324
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006216 RID: 25110 RVA: 0x001F0134 File Offset: 0x001EE334
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

		// Token: 0x06006217 RID: 25111 RVA: 0x001F0188 File Offset: 0x001EE388
		public override void OnFixedUpdate()
		{
			this.DoStretch();
		}

		// Token: 0x06006218 RID: 25112 RVA: 0x001F0190 File Offset: 0x001EE390
		private void DoStretch()
		{
			if (this.rb2d == null)
			{
				return;
			}
			this.stretchY = 1f - this.rb2d.linearVelocity.magnitude * this.stretchFactor.Value * 0.01f;
			this.stretchX = 1f + this.rb2d.linearVelocity.magnitude * this.stretchFactor.Value * 0.01f;
			if (this.stretchX < this.stretchMinX)
			{
				this.stretchX = this.stretchMinX;
			}
			if (this.stretchY > this.stretchMaxY)
			{
				this.stretchY = this.stretchMaxY;
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

		// Token: 0x04006025 RID: 24613
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006026 RID: 24614
		[Tooltip("Increase this value to make the object's stretch more pronounced")]
		public FsmFloat stretchFactor = 1.4f;

		// Token: 0x04006027 RID: 24615
		[Tooltip("Minimum scale value for X.")]
		public float stretchMinX = 0.5f;

		// Token: 0x04006028 RID: 24616
		[Tooltip("Maximum scale value for Y")]
		public float stretchMaxY = 2f;

		// Token: 0x04006029 RID: 24617
		[Tooltip("After other calculations, multiply scale by this modifier.")]
		public FsmFloat scaleModifier;

		// Token: 0x0400602A RID: 24618
		[Tooltip("Optionally, stretch another object based on our own velocity")]
		public FsmGameObject otherGameObject;

		// Token: 0x0400602B RID: 24619
		public bool everyFrame;

		// Token: 0x0400602C RID: 24620
		private FsmGameObject target;

		// Token: 0x0400602D RID: 24621
		private float stretchX = 1f;

		// Token: 0x0400602E RID: 24622
		private float stretchY = 1f;
	}
}
