using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E5B RID: 3675
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Gets the Collision Flags from a CharacterController on a GameObject. Collision flags give you a broad overview of where the character collided with another object.")]
	public class GetControllerCollisionFlags : ComponentAction<CharacterController>
	{
		// Token: 0x17000BED RID: 3053
		// (get) Token: 0x060068F3 RID: 26867 RVA: 0x0020F554 File Offset: 0x0020D754
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068F4 RID: 26868 RVA: 0x0020F55C File Offset: 0x0020D75C
		public override void Reset()
		{
			this.gameObject = null;
			this.isGrounded = null;
			this.none = null;
			this.sides = null;
			this.above = null;
			this.below = null;
		}

		// Token: 0x060068F5 RID: 26869 RVA: 0x0020F588 File Offset: 0x0020D788
		public override void OnUpdate()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.isGrounded.Value = this.controller.isGrounded;
			this.none.Value = (this.controller.collisionFlags == CollisionFlags.None);
			this.sides.Value = ((this.controller.collisionFlags & CollisionFlags.Sides) > CollisionFlags.None);
			this.above.Value = ((this.controller.collisionFlags & CollisionFlags.Above) > CollisionFlags.None);
			this.below.Value = ((this.controller.collisionFlags & CollisionFlags.Below) > CollisionFlags.None);
		}

		// Token: 0x04006840 RID: 26688
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject with a Character Controller component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006841 RID: 26689
		[UIHint(UIHint.Variable)]
		[Tooltip("True if the Character Controller capsule is on the ground")]
		public FsmBool isGrounded;

		// Token: 0x04006842 RID: 26690
		[UIHint(UIHint.Variable)]
		[Tooltip("True if no collisions in last move.")]
		public FsmBool none;

		// Token: 0x04006843 RID: 26691
		[UIHint(UIHint.Variable)]
		[Tooltip("True if the Character Controller capsule was hit on the sides.")]
		public FsmBool sides;

		// Token: 0x04006844 RID: 26692
		[UIHint(UIHint.Variable)]
		[Tooltip("True if the Character Controller capsule was hit from above.")]
		public FsmBool above;

		// Token: 0x04006845 RID: 26693
		[UIHint(UIHint.Variable)]
		[Tooltip("True if the Character Controller capsule was hit from below.")]
		public FsmBool below;
	}
}
