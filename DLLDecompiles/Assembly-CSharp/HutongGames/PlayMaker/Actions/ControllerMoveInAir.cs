using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E58 RID: 3672
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Handles CharacterController while in the air, e.g., after jumping.")]
	public class ControllerMoveInAir : ComponentAction<CharacterController>
	{
		// Token: 0x17000BEA RID: 3050
		// (get) Token: 0x060068E5 RID: 26853 RVA: 0x0020F0BE File Offset: 0x0020D2BE
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068E6 RID: 26854 RVA: 0x0020F0C8 File Offset: 0x0020D2C8
		public override void Reset()
		{
			this.gameObject = null;
			this.moveVector = null;
			this.maxMoveSpeed = null;
			this.gravityMultiplier = new FsmFloat
			{
				Value = 1f
			};
			this.fallMultiplier = new FsmFloat
			{
				Value = 1f
			};
			this.space = Space.World;
			this.landedEvent = null;
		}

		// Token: 0x060068E7 RID: 26855 RVA: 0x0020F124 File Offset: 0x0020D324
		public override void OnUpdate()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 vector = this.controller.velocity;
			if (!this.moveVector.IsNone)
			{
				vector += this.moveVector.Value;
			}
			float num = Physics.gravity.y * this.gravityMultiplier.Value * ((vector.y < 0f) ? this.fallMultiplier.Value : 1f);
			vector.y += num * Time.deltaTime;
			if (!this.maxMoveSpeed.IsNone)
			{
				Vector2 vector2 = Vector2.ClampMagnitude(new Vector2(vector.x, vector.z), this.maxMoveSpeed.Value);
				vector.Set(vector2.x, vector.y, vector2.y);
			}
			if (this.space == Space.Self)
			{
				vector = base.cachedTransform.TransformDirection(vector);
			}
			this.controller.Move(vector * Time.deltaTime);
			if (this.controller.isGrounded && this.controller.velocity.y < 0.1f)
			{
				this.controller.Move(Vector3.zero);
				Fsm.EventData.FloatData = vector.magnitude;
				Fsm.EventData.Vector2Data = vector;
				base.Fsm.Event(this.landedEvent);
			}
		}

		// Token: 0x0400682C RID: 26668
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400682D RID: 26669
		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector applied while in the air. Usually to allow the player to influence the jump.")]
		public FsmVector3 moveVector;

		// Token: 0x0400682E RID: 26670
		[Tooltip("Clamp horizontal speed while jumping. Set to None for no clamping.")]
		public FsmFloat maxMoveSpeed;

		// Token: 0x0400682F RID: 26671
		[Tooltip("Multiply the gravity set in the Physics system.")]
		public FsmFloat gravityMultiplier;

		// Token: 0x04006830 RID: 26672
		[Tooltip("Extra gravity multiplier when falling. Note: This is on top of the gravity multiplier above. This can be used to make jumps less 'floaty.'")]
		public FsmFloat fallMultiplier;

		// Token: 0x04006831 RID: 26673
		[Tooltip("Move in local or word space.")]
		public Space space;

		// Token: 0x04006832 RID: 26674
		[Tooltip("Event to send when landed.")]
		public FsmEvent landedEvent;
	}
}
