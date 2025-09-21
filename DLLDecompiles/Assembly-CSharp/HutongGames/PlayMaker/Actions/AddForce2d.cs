using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FBA RID: 4026
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Adds a 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
	[SeeAlso("To add a force in local space use {{Add Relative Force 2d}}")]
	public class AddForce2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F18 RID: 28440 RVA: 0x00225768 File Offset: 0x00223968
		public override void Reset()
		{
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

		// Token: 0x06006F19 RID: 28441 RVA: 0x002257D9 File Offset: 0x002239D9
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006F1A RID: 28442 RVA: 0x002257E7 File Offset: 0x002239E7
		public override void OnEnter()
		{
			this.DoAddForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F1B RID: 28443 RVA: 0x002257FD File Offset: 0x002239FD
		public override void OnFixedUpdate()
		{
			this.DoAddForce();
		}

		// Token: 0x06006F1C RID: 28444 RVA: 0x00225808 File Offset: 0x00223A08
		private void DoAddForce()
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

		// Token: 0x04006EB8 RID: 28344
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006EB9 RID: 28345
		[Tooltip("Option for applying the force")]
		public ForceMode2D forceMode;

		// Token: 0x04006EBA RID: 28346
		[UIHint(UIHint.Variable)]
		[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollision2dInfo actions.")]
		public FsmVector2 atPosition;

		// Token: 0x04006EBB RID: 28347
		[UIHint(UIHint.Variable)]
		[Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
		public FsmVector2 vector;

		// Token: 0x04006EBC RID: 28348
		[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		// Token: 0x04006EBD RID: 28349
		[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		// Token: 0x04006EBE RID: 28350
		[Tooltip("A Vector3 force to add. z is ignored")]
		public FsmVector3 vector3;

		// Token: 0x04006EBF RID: 28351
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
