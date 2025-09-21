using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB3 RID: 4019
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Sets the velocity of a game object with a rigid body. To leave any axis unchanged, set variable to 'None'.\nIn most cases you should not modify the velocity directly, as this can result in unrealistic behaviour. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody-velocity.html\">Rigidbody.velocity</a>.")]
	public class SetVelocity : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EEA RID: 28394 RVA: 0x00224EB8 File Offset: 0x002230B8
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

		// Token: 0x06006EEB RID: 28395 RVA: 0x00224F17 File Offset: 0x00223117
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006EEC RID: 28396 RVA: 0x00224F25 File Offset: 0x00223125
		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EED RID: 28397 RVA: 0x00224F3B File Offset: 0x0022313B
		public override void OnUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EEE RID: 28398 RVA: 0x00224F51 File Offset: 0x00223151
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EEF RID: 28399 RVA: 0x00224F68 File Offset: 0x00223168
		private void DoSetVelocity()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space == Space.World) ? base.rigidbody.linearVelocity : base.cachedTransform.InverseTransformDirection(base.rigidbody.linearVelocity));
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
			base.rigidbody.linearVelocity = ((this.space == Space.World) ? vector : base.cachedTransform.TransformDirection(vector));
		}

		// Token: 0x04006E9D RID: 28317
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The Game Object with the RigidBody component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E9E RID: 28318
		[UIHint(UIHint.Variable)]
		[Tooltip("Set velocity using Vector3 variable and/or individual channels below.")]
		public FsmVector3 vector;

		// Token: 0x04006E9F RID: 28319
		[Tooltip("Velocity in X axis.")]
		public FsmFloat x;

		// Token: 0x04006EA0 RID: 28320
		[Tooltip("Velocity in Y axis.")]
		public FsmFloat y;

		// Token: 0x04006EA1 RID: 28321
		[Tooltip("Velocity in Z axis.")]
		public FsmFloat z;

		// Token: 0x04006EA2 RID: 28322
		[Tooltip("You can set velocity in world or local space.")]
		public Space space;

		// Token: 0x04006EA3 RID: 28323
		[Tooltip("Set the velocity every frame.")]
		public bool everyFrame;
	}
}
