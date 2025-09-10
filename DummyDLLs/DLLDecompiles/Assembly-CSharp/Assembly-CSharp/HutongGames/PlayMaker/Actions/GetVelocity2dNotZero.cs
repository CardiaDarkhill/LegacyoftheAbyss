using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C81 RID: 3201
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets the 2d Velocity of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable. Ignores very low speeds.")]
	public class GetVelocity2dNotZero : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006057 RID: 24663 RVA: 0x001E7CF4 File Offset: 0x001E5EF4
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x06006058 RID: 24664 RVA: 0x001E7D20 File Offset: 0x001E5F20
		public override void OnEnter()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006059 RID: 24665 RVA: 0x001E7D36 File Offset: 0x001E5F36
		public override void OnUpdate()
		{
			this.DoGetVelocity();
		}

		// Token: 0x0600605A RID: 24666 RVA: 0x001E7D40 File Offset: 0x001E5F40
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
			if (vector.x > 0.1f || (vector.x < -0.1f && vector.y > 0.1f) || vector.y < -0.1f)
			{
				this.vector.Value = vector;
			}
			if (vector.x > 0.1f || vector.x < -0.1f)
			{
				this.x.Value = vector.x;
			}
			if (vector.y > 0.1f || vector.y < -0.1f)
			{
				this.y.Value = vector.y;
			}
		}

		// Token: 0x04005DAB RID: 23979
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005DAC RID: 23980
		[UIHint(UIHint.Variable)]
		[Tooltip("The velocity")]
		public FsmVector2 vector;

		// Token: 0x04005DAD RID: 23981
		[UIHint(UIHint.Variable)]
		[Tooltip("The x value of the velocity")]
		public FsmFloat x;

		// Token: 0x04005DAE RID: 23982
		[UIHint(UIHint.Variable)]
		[Tooltip("The y value of the velocity")]
		public FsmFloat y;

		// Token: 0x04005DAF RID: 23983
		[Tooltip("The space reference to express the velocity")]
		public Space space;

		// Token: 0x04005DB0 RID: 23984
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
