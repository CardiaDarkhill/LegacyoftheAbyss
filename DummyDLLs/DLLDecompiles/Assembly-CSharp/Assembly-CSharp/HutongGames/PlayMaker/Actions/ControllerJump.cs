using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E56 RID: 3670
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Makes a CharacterController Jump.")]
	public class ControllerJump : ComponentAction<CharacterController>
	{
		// Token: 0x17000BE9 RID: 3049
		// (get) Token: 0x060068DD RID: 26845 RVA: 0x0020EC3B File Offset: 0x0020CE3B
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068DE RID: 26846 RVA: 0x0020EC44 File Offset: 0x0020CE44
		public override void Reset()
		{
			this.gameObject = null;
			this.jumpHeight = new FsmFloat
			{
				Value = 0.5f
			};
			this.jumpSpeedMultiplier = new FsmFloat
			{
				Value = 1f
			};
			this.gravityMultiplier = new FsmFloat
			{
				Value = 1f
			};
			this.space = Space.World;
			this.moveVector = null;
			this.speed = new FsmFloat
			{
				Value = 1f
			};
			this.maxSpeed = new FsmFloat
			{
				Value = 2f
			};
			this.fallMultiplier = new FsmFloat
			{
				Value = 1f
			};
		}

		// Token: 0x060068DF RID: 26847 RVA: 0x0020ECEC File Offset: 0x0020CEEC
		public override void OnEnter()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			this.startJumpPosition = base.cachedTransform.position;
			Vector3 vector = this.controller.velocity * (this.jumpSpeedMultiplier.IsNone ? 1f : this.jumpSpeedMultiplier.Value);
			vector.y = 0f;
			if (this.space == Space.Self)
			{
				vector = base.cachedTransform.InverseTransformDirection(vector);
			}
			float num = Physics.gravity.y * (this.gravityMultiplier.IsNone ? 1f : this.gravityMultiplier.Value);
			float y = vector.y + Mathf.Sqrt(this.jumpHeight.Value * -3f * num);
			Vector3 vector2 = new Vector3(vector.x, y, vector.z);
			if (this.space == Space.Self)
			{
				vector2 = base.cachedTransform.TransformDirection(vector2);
			}
			this.controller.Move(vector2 * Time.deltaTime);
		}

		// Token: 0x060068E0 RID: 26848 RVA: 0x0020EE08 File Offset: 0x0020D008
		public override void OnUpdate()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			Vector3 vector = this.controller.velocity;
			if (!this.moveVector.IsNone)
			{
				Vector3 vector2 = this.moveVector.Value;
				if (!this.speed.IsNone)
				{
					vector2 *= this.speed.Value;
				}
				vector += vector2;
			}
			float num = Physics.gravity.y * this.gravityMultiplier.Value * ((vector.y < 0f) ? this.fallMultiplier.Value : 1f);
			vector.y += num * Time.deltaTime;
			if (!this.maxSpeed.IsNone)
			{
				Vector2 vector3 = Vector2.ClampMagnitude(new Vector2(vector.x, vector.z), this.maxSpeed.Value);
				vector.Set(vector3.x, vector.y, vector3.y);
			}
			if (this.space == Space.Self)
			{
				vector = base.cachedTransform.TransformDirection(vector);
			}
			this.controller.Move(vector * Time.deltaTime);
			if (this.controller.isGrounded && this.controller.velocity.y < 0.1f)
			{
				this.controller.Move(Vector3.zero);
				this.landingMotion.Value = vector;
				this.landingSpeed.Value = vector.magnitude;
				this.fallDistance.Value = this.startJumpPosition.y - base.cachedTransform.position.y;
				base.Fsm.Event(this.landedEvent);
			}
		}

		// Token: 0x04006817 RID: 26647
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006818 RID: 26648
		[Tooltip("How high to jump.")]
		public FsmFloat jumpHeight;

		// Token: 0x04006819 RID: 26649
		[Tooltip("Jump in local or word space.")]
		public Space space;

		// Token: 0x0400681A RID: 26650
		[Tooltip("Multiplies the speed of the CharacterController at moment of jumping. Higher numbers will jump further. Note: Does not effect the jump height.")]
		public FsmFloat jumpSpeedMultiplier;

		// Token: 0x0400681B RID: 26651
		[Tooltip("Gravity multiplier used in air, to correctly calculate jump height.")]
		public FsmFloat gravityMultiplier;

		// Token: 0x0400681C RID: 26652
		[Tooltip("Extra gravity multiplier when falling. Note: This is on top of the gravity multiplier above. This can be used to make jumps less 'floaty.'")]
		public FsmFloat fallMultiplier;

		// Token: 0x0400681D RID: 26653
		[ActionSection("In Air Controls")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector applied while in the air. Usually from a Get Axis Vector, allowing the player to influence the jump.")]
		public FsmVector3 moveVector;

		// Token: 0x0400681E RID: 26654
		[Tooltip("Multiplies the Move Vector by a Speed factor.")]
		public FsmFloat speed;

		// Token: 0x0400681F RID: 26655
		[Tooltip("Clamp horizontal speed while jumping. Set to None for no clamping.")]
		public FsmFloat maxSpeed;

		// Token: 0x04006820 RID: 26656
		[ActionSection("Landing")]
		[Tooltip("Event to send when landing. Use this to transition back to a grounded State.")]
		public FsmEvent landedEvent;

		// Token: 0x04006821 RID: 26657
		[UIHint(UIHint.Variable)]
		[Tooltip("Store how fast the Character Controlling was moving when it landed.")]
		public FsmFloat landingSpeed;

		// Token: 0x04006822 RID: 26658
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the last movement before landing.")]
		public FsmVector3 landingMotion;

		// Token: 0x04006823 RID: 26659
		[UIHint(UIHint.Variable)]
		[Tooltip("The total distance fallen, from the start of the jump to landing point. NOTE: This will be negative when jumping to higher ground.")]
		public FsmFloat fallDistance;

		// Token: 0x04006824 RID: 26660
		private Vector3 startJumpPosition;

		// Token: 0x04006825 RID: 26661
		private Vector3 totalJumpMovement;
	}
}
