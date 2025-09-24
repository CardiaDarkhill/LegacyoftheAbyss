using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA7 RID: 4007
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets the Velocity of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable. NOTE: The Game Object must have a Rigid Body.")]
	public class GetVelocity : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EB0 RID: 28336 RVA: 0x00224046 File Offset: 0x00222246
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.z = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x06006EB1 RID: 28337 RVA: 0x00224079 File Offset: 0x00222279
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006EB2 RID: 28338 RVA: 0x00224087 File Offset: 0x00222287
		public override void OnEnter()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EB3 RID: 28339 RVA: 0x0022409D File Offset: 0x0022229D
		public override void OnUpdate()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EB4 RID: 28340 RVA: 0x002240B3 File Offset: 0x002222B3
		public override void OnFixedUpdate()
		{
			this.DoGetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EB5 RID: 28341 RVA: 0x002240CC File Offset: 0x002222CC
		private void DoGetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 vector = base.rigidbody.linearVelocity;
			if (this.space == Space.Self)
			{
				vector = ownerDefaultTarget.transform.InverseTransformDirection(vector);
			}
			this.vector.Value = vector;
			this.x.Value = vector.x;
			this.y.Value = vector.y;
			this.z.Value = vector.z;
		}

		// Token: 0x04006E4B RID: 28235
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The Game Object.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E4C RID: 28236
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the velocity in a Vector3 Variable.")]
		public FsmVector3 vector;

		// Token: 0x04006E4D RID: 28237
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X component of the velocity in a Float Variable.")]
		public FsmFloat x;

		// Token: 0x04006E4E RID: 28238
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y component of the velocity in a Float Variable.")]
		public FsmFloat y;

		// Token: 0x04006E4F RID: 28239
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Z component of the velocity in a Float Variable.")]
		public FsmFloat z;

		// Token: 0x04006E50 RID: 28240
		[Tooltip("The coordinate space to get the velocity in.")]
		public Space space;

		// Token: 0x04006E51 RID: 28241
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
