using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FAD RID: 4013
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Sets the velocity of a CharacterController on a GameObject. To leave any axis unchanged, set variable to 'None'.")]
	public class SetControllerVelocity : ComponentAction<CharacterController>
	{
		// Token: 0x17000C02 RID: 3074
		// (get) Token: 0x06006ED0 RID: 28368 RVA: 0x00224A63 File Offset: 0x00222C63
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006ED1 RID: 28369 RVA: 0x00224A6C File Offset: 0x00222C6C
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.Self;
			this.everyFrame = false;
		}

		// Token: 0x06006ED2 RID: 28370 RVA: 0x00224ACB File Offset: 0x00222CCB
		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006ED3 RID: 28371 RVA: 0x00224AE4 File Offset: 0x00222CE4
		private void DoSetVelocity()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space == Space.World) ? this.controller.velocity : base.cachedTransform.InverseTransformDirection(this.controller.velocity));
			}
			else
			{
				vector = this.vector.Value;
			}
			if (!this.x.IsNone)
			{
				vector.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				vector.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				vector.z = this.z.Value;
			}
			if (this.space == Space.Self)
			{
				vector = base.cachedTransform.TransformDirection(vector);
			}
			this.controller.Move(vector * Time.deltaTime);
		}

		// Token: 0x04006E88 RID: 28296
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject with the Character Controller component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E89 RID: 28297
		[UIHint(UIHint.Variable)]
		[Tooltip("Set velocity using Vector3 variable and/or individual channels below.")]
		public FsmVector3 vector;

		// Token: 0x04006E8A RID: 28298
		[Tooltip("Velocity in X axis.")]
		public FsmFloat x;

		// Token: 0x04006E8B RID: 28299
		[Tooltip("Velocity in Y axis.")]
		public FsmFloat y;

		// Token: 0x04006E8C RID: 28300
		[Tooltip("Velocity in Z axis.")]
		public FsmFloat z;

		// Token: 0x04006E8D RID: 28301
		[Tooltip("You can set velocity in world or local space.")]
		public Space space;

		// Token: 0x04006E8E RID: 28302
		[Tooltip("Set the velocity every frame.")]
		public bool everyFrame;
	}
}
