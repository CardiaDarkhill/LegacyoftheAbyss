using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FBB RID: 4027
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Adds a relative 2d force to a Game Object. Use Vector2 variable and/or Float variables for each axis.")]
	public class AddRelativeForce2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F1E RID: 28446 RVA: 0x00225928 File Offset: 0x00223B28
		public override void Reset()
		{
			this.gameObject = null;
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

		// Token: 0x06006F1F RID: 28447 RVA: 0x00225987 File Offset: 0x00223B87
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006F20 RID: 28448 RVA: 0x00225995 File Offset: 0x00223B95
		public override void OnEnter()
		{
			this.DoAddRelativeForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F21 RID: 28449 RVA: 0x002259AB File Offset: 0x00223BAB
		public override void OnFixedUpdate()
		{
			this.DoAddRelativeForce();
		}

		// Token: 0x06006F22 RID: 28450 RVA: 0x002259B4 File Offset: 0x00223BB4
		private void DoAddRelativeForce()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 relativeForce = this.vector.IsNone ? new Vector2(this.x.Value, this.y.Value) : this.vector.Value;
			if (!this.vector3.IsNone)
			{
				relativeForce.x = this.vector3.Value.x;
				relativeForce.y = this.vector3.Value.y;
			}
			if (!this.x.IsNone)
			{
				relativeForce.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				relativeForce.y = this.y.Value;
			}
			base.rigidbody2d.AddRelativeForce(relativeForce, this.forceMode);
		}

		// Token: 0x04006EC0 RID: 28352
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to apply the force to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006EC1 RID: 28353
		[Tooltip("Option for applying the force")]
		public ForceMode2D forceMode;

		// Token: 0x04006EC2 RID: 28354
		[UIHint(UIHint.Variable)]
		[Tooltip("A Vector2 force to add. Optionally override any axis with the X, Y parameters.")]
		public FsmVector2 vector;

		// Token: 0x04006EC3 RID: 28355
		[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		// Token: 0x04006EC4 RID: 28356
		[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		// Token: 0x04006EC5 RID: 28357
		[Tooltip("A Vector3 force to add. z is ignored")]
		public FsmVector3 vector3;

		// Token: 0x04006EC6 RID: 28358
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
