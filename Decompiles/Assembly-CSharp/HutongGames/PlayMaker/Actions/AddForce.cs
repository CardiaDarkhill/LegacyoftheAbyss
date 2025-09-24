using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F9A RID: 3994
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Adds a force to a Game Object. Use Vector3 variable and/or Float variables for each axis.")]
	public class AddForce : ComponentAction<Rigidbody>
	{
		// Token: 0x06006E52 RID: 28242 RVA: 0x00222C7C File Offset: 0x00220E7C
		public override void Reset()
		{
			this.gameObject = null;
			this.atPosition = new FsmVector3
			{
				UseVariable = true
			};
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
			this.space = Space.World;
			this.forceMode = ForceMode.Force;
			this.everyFrame = false;
		}

		// Token: 0x06006E53 RID: 28243 RVA: 0x00222CF4 File Offset: 0x00220EF4
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006E54 RID: 28244 RVA: 0x00222D02 File Offset: 0x00220F02
		public override void OnEnter()
		{
			this.DoAddForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E55 RID: 28245 RVA: 0x00222D18 File Offset: 0x00220F18
		public override void OnFixedUpdate()
		{
			this.DoAddForce();
		}

		// Token: 0x06006E56 RID: 28246 RVA: 0x00222D20 File Offset: 0x00220F20
		private void DoAddForce()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 force = this.vector.IsNone ? default(Vector3) : this.vector.Value;
			if (!this.x.IsNone)
			{
				force.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				force.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				force.z = this.z.Value;
			}
			if (this.space != Space.World)
			{
				base.rigidbody.AddRelativeForce(force, this.forceMode);
				return;
			}
			if (!this.atPosition.IsNone)
			{
				base.rigidbody.AddForceAtPosition(force, this.atPosition.Value, this.forceMode);
				return;
			}
			base.rigidbody.AddForce(force, this.forceMode);
		}

		// Token: 0x04006DFA RID: 28154
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006DFB RID: 28155
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollisionInfo actions.")]
		public FsmVector3 atPosition;

		// Token: 0x04006DFC RID: 28156
		[UIHint(UIHint.Variable)]
		[Tooltip("A Vector3 force to add. Optionally override any axis with the X, Y, Z parameters.")]
		public FsmVector3 vector;

		// Token: 0x04006DFD RID: 28157
		[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		// Token: 0x04006DFE RID: 28158
		[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		// Token: 0x04006DFF RID: 28159
		[Tooltip("Force along the Z axis. To leave unchanged, set to 'None'.")]
		public FsmFloat z;

		// Token: 0x04006E00 RID: 28160
		[Tooltip("Apply the force in world or local space.")]
		public Space space;

		// Token: 0x04006E01 RID: 28161
		[Tooltip("The type of force to apply. See Unity Physics docs.")]
		public ForceMode forceMode;

		// Token: 0x04006E02 RID: 28162
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
