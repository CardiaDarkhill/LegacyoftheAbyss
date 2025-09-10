using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA1 RID: 2977
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
	[SeeAlso("To add a force in local space use {{Add Relative Force 2d}}")]
	public class AddForce2dConditional : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06005BFD RID: 23549 RVA: 0x001CEE18 File Offset: 0x001CD018
		public override void Reset()
		{
			this.addForceBool = null;
			this.gameObject = null;
			this.atPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.forceMode = ForceMode2D.Force;
			this.vector = null;
			this.vector3 = new FsmVector3
			{
				UseVariable = true
			};
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06005BFE RID: 23550 RVA: 0x001CEE90 File Offset: 0x001CD090
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005BFF RID: 23551 RVA: 0x001CEE9E File Offset: 0x001CD09E
		public override void OnEnter()
		{
			this.DoAddForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005C00 RID: 23552 RVA: 0x001CEEB4 File Offset: 0x001CD0B4
		public override void OnFixedUpdate()
		{
			this.DoAddForce();
		}

		// Token: 0x06005C01 RID: 23553 RVA: 0x001CEEBC File Offset: 0x001CD0BC
		private void DoAddForce()
		{
			if (this.addForceBool.Value)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (!base.UpdateCache(ownerDefaultTarget))
				{
					return;
				}
				Vector2 force = this.vector.IsNone ? new Vector2(this.x.Value, this.y.Value) : this.vector.Value;
				if (!this.vector3.IsNone)
				{
					force.x = this.vector3.Value.x;
					force.y = this.vector3.Value.y;
				}
				if (!this.x.IsNone)
				{
					force.x = this.x.Value;
				}
				if (!this.y.IsNone)
				{
					force.y = this.y.Value;
				}
				if (!this.atPosition.IsNone)
				{
					base.rigidbody2d.AddForceAtPosition(force, this.atPosition.Value, this.forceMode);
					return;
				}
				base.rigidbody2d.AddForce(force, this.forceMode);
			}
		}

		// Token: 0x04005762 RID: 22370
		public FsmBool addForceBool;

		// Token: 0x04005763 RID: 22371
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005764 RID: 22372
		[Tooltip("Option for applying the force")]
		public ForceMode2D forceMode;

		// Token: 0x04005765 RID: 22373
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
		public FsmVector2 atPosition;

		// Token: 0x04005766 RID: 22374
		[UIHint(UIHint.Variable)]
		[Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
		public FsmVector2 vector;

		// Token: 0x04005767 RID: 22375
		[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		// Token: 0x04005768 RID: 22376
		[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		// Token: 0x04005769 RID: 22377
		[Tooltip("A Vector3 force to add. z is ignored")]
		public FsmVector3 vector3;

		// Token: 0x0400576A RID: 22378
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
