using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA4 RID: 3492
	[ActionCategory("Hornet")]
	[Tooltip("Controls Hornet's horizontal motion in umbrella float")]
	public class UmbrellaMotionXV2 : RigidBody2dActionBase
	{
		// Token: 0x06006569 RID: 25961 RVA: 0x001FFF77 File Offset: 0x001FE177
		public override void Reset()
		{
			this.gameObject = null;
			this.accelerationFactor = null;
			this.accelerationFactor = null;
			this.decelerationFactor = null;
			this.active = null;
		}

		// Token: 0x0600656A RID: 25962 RVA: 0x001FFF9C File Offset: 0x001FE19C
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600656B RID: 25963 RVA: 0x001FFFAC File Offset: 0x001FE1AC
		public override void OnEnter()
		{
			this.gm = GameManager.instance;
			this.inputHandler = this.gm.GetComponent<InputHandler>();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			base.CacheRigidBody2d(ownerDefaultTarget);
			this.windObject = ownerDefaultTarget.GetComponent<UmbrellaWindObject>();
			if (this.windObject)
			{
				this.windObject.IsActive = true;
				this.windObject.SelfXSpeed = this.rb2d.linearVelocity.x;
			}
			this.DoAccelerate();
		}

		// Token: 0x0600656C RID: 25964 RVA: 0x00200034 File Offset: 0x001FE234
		public override void OnFixedUpdate()
		{
			if (this.active.Value)
			{
				this.DoAccelerate();
			}
		}

		// Token: 0x0600656D RID: 25965 RVA: 0x00200049 File Offset: 0x001FE249
		public override void OnExit()
		{
			if (this.windObject)
			{
				this.windObject.IsActive = false;
				this.windObject = null;
			}
		}

		// Token: 0x0600656E RID: 25966 RVA: 0x0020006C File Offset: 0x001FE26C
		private void DoAccelerate()
		{
			if (this.rb2d == null)
			{
				return;
			}
			float num = this.windObject ? this.windObject.SelfXSpeed : this.rb2d.linearVelocity.x;
			if (this.inputHandler.inputActions.Right.IsPressed)
			{
				if (num < this.maxSpeed.Value)
				{
					num += this.accelerationFactor.Value;
				}
			}
			else if (this.inputHandler.inputActions.Left.IsPressed)
			{
				if (num > -this.maxSpeed.Value)
				{
					num -= this.accelerationFactor.Value;
				}
			}
			else if (num > 0f)
			{
				num -= this.decelerationFactor.Value;
				if (num < 0f)
				{
					num = 0f;
				}
			}
			else if (num < 0f)
			{
				num += this.decelerationFactor.Value;
				if (num > 0f)
				{
					num = 0f;
				}
			}
			if (num < -this.maxSpeed.Value)
			{
				num = -this.maxSpeed.Value;
			}
			if (num > this.maxSpeed.Value)
			{
				num = this.maxSpeed.Value;
			}
			if (this.windObject)
			{
				this.windObject.SelfXSpeed = num;
				return;
			}
			this.rb2d.linearVelocity = new Vector2(num, this.rb2d.linearVelocity.y);
		}

		// Token: 0x04006468 RID: 25704
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006469 RID: 25705
		public FsmFloat accelerationFactor;

		// Token: 0x0400646A RID: 25706
		public FsmFloat decelerationFactor;

		// Token: 0x0400646B RID: 25707
		public FsmFloat maxSpeed;

		// Token: 0x0400646C RID: 25708
		public FsmBool active;

		// Token: 0x0400646D RID: 25709
		private GameManager gm;

		// Token: 0x0400646E RID: 25710
		private InputHandler inputHandler;

		// Token: 0x0400646F RID: 25711
		private UmbrellaWindObject windObject;
	}
}
