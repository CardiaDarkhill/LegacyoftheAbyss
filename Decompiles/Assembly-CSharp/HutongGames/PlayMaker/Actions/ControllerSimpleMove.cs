using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E5A RID: 3674
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
	public class ControllerSimpleMove : ComponentAction<CharacterController>
	{
		// Token: 0x17000BEC RID: 3052
		// (get) Token: 0x060068EF RID: 26863 RVA: 0x0020F455 File Offset: 0x0020D655
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068F0 RID: 26864 RVA: 0x0020F45D File Offset: 0x0020D65D
		public override void Reset()
		{
			this.gameObject = null;
			this.moveVector = new FsmVector3
			{
				UseVariable = true
			};
			this.speed = new FsmFloat
			{
				Value = 1f
			};
			this.space = Space.World;
			this.fallingEvent = null;
		}

		// Token: 0x060068F1 RID: 26865 RVA: 0x0020F49C File Offset: 0x0020D69C
		public override void OnUpdate()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 a = (this.space == Space.World) ? this.moveVector.Value : base.cachedTransform.TransformDirection(this.moveVector.Value);
			this.controller.SimpleMove(a * this.speed.Value);
			if (!this.controller.isGrounded && !Physics.Raycast(base.cachedTransform.position, Vector3.down, this.controller.stepOffset))
			{
				base.Fsm.Event(this.fallingEvent);
			}
		}

		// Token: 0x0400683B RID: 26683
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("A Game Object with a Character Controller.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400683C RID: 26684
		[RequiredField]
		[Tooltip("The movement vector.")]
		public FsmVector3 moveVector;

		// Token: 0x0400683D RID: 26685
		[Tooltip("Multiply the Move Vector by a speed factor.")]
		public FsmFloat speed;

		// Token: 0x0400683E RID: 26686
		[Tooltip("Move in local or world space.")]
		public Space space;

		// Token: 0x0400683F RID: 26687
		[Tooltip("Event sent if the Character Controller starts falling.")]
		public FsmEvent fallingEvent;
	}
}
