using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C80 RID: 3200
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body 2D.")]
	public class GetVelocity2dLate : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006052 RID: 24658 RVA: 0x001E7C17 File Offset: 0x001E5E17
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x06006053 RID: 24659 RVA: 0x001E7C43 File Offset: 0x001E5E43
		public override void OnEnter()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006054 RID: 24660 RVA: 0x001E7C59 File Offset: 0x001E5E59
		public override void OnLateUpdate()
		{
			this.DoGetVelocity();
		}

		// Token: 0x06006055 RID: 24661 RVA: 0x001E7C64 File Offset: 0x001E5E64
		private void DoGetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 vector = base.rigidbody2d.linearVelocity;
			if (this.space == Space.Self)
			{
				vector = base.rigidbody2d.transform.InverseTransformDirection(vector);
			}
			this.vector.Value = vector;
			this.x.Value = vector.x;
			this.y.Value = vector.y;
		}

		// Token: 0x04005DA5 RID: 23973
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005DA6 RID: 23974
		[UIHint(UIHint.Variable)]
		[Tooltip("The velocity")]
		public FsmVector2 vector;

		// Token: 0x04005DA7 RID: 23975
		[UIHint(UIHint.Variable)]
		[Tooltip("The x value of the velocity")]
		public FsmFloat x;

		// Token: 0x04005DA8 RID: 23976
		[UIHint(UIHint.Variable)]
		[Tooltip("The y value of the velocity")]
		public FsmFloat y;

		// Token: 0x04005DA9 RID: 23977
		[Tooltip("The space reference to express the velocity")]
		public Space space;

		// Token: 0x04005DAA RID: 23978
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
