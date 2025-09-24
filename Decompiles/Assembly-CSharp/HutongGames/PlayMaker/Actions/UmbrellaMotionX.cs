using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA3 RID: 3491
	[ActionCategory("Hornet")]
	[Tooltip("Controls Hornet's horizontal motion in umbrella float")]
	public class UmbrellaMotionX : RigidBody2dActionBase
	{
		// Token: 0x06006562 RID: 25954 RVA: 0x001FFD19 File Offset: 0x001FDF19
		public override void Reset()
		{
			this.gameObject = null;
			this.accelerationFactor = null;
			this.accelerationFactor = null;
			this.decelerationFactor = null;
		}

		// Token: 0x06006563 RID: 25955 RVA: 0x001FFD37 File Offset: 0x001FDF37
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006564 RID: 25956 RVA: 0x001FFD48 File Offset: 0x001FDF48
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

		// Token: 0x06006565 RID: 25957 RVA: 0x001FFDD0 File Offset: 0x001FDFD0
		public override void OnFixedUpdate()
		{
			this.DoAccelerate();
		}

		// Token: 0x06006566 RID: 25958 RVA: 0x001FFDD8 File Offset: 0x001FDFD8
		public override void OnExit()
		{
			if (this.windObject)
			{
				this.windObject.IsActive = false;
				this.windObject = null;
			}
		}

		// Token: 0x06006567 RID: 25959 RVA: 0x001FFDFC File Offset: 0x001FDFFC
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

		// Token: 0x04006461 RID: 25697
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006462 RID: 25698
		public FsmFloat accelerationFactor;

		// Token: 0x04006463 RID: 25699
		public FsmFloat decelerationFactor;

		// Token: 0x04006464 RID: 25700
		public FsmFloat maxSpeed;

		// Token: 0x04006465 RID: 25701
		private GameManager gm;

		// Token: 0x04006466 RID: 25702
		private InputHandler inputHandler;

		// Token: 0x04006467 RID: 25703
		private UmbrellaWindObject windObject;
	}
}
