using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FAA RID: 4010
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Moves a Game Object's Rigid Body to a new position. Unlike Set Position this will respect physics collisions.")]
	public class MovePosition : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EC1 RID: 28353 RVA: 0x002242B0 File Offset: 0x002224B0
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

		// Token: 0x06006EC2 RID: 28354 RVA: 0x0022430F File Offset: 0x0022250F
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006EC3 RID: 28355 RVA: 0x0022431D File Offset: 0x0022251D
		public override void OnFixedUpdate()
		{
			this.DoMovePosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EC4 RID: 28356 RVA: 0x00224334 File Offset: 0x00222534
		private void DoMovePosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space == Space.World) ? base.rigidbody.position : ownerDefaultTarget.transform.TransformPoint(base.rigidbody.position));
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
			base.rigidbody.MovePosition((this.space == Space.World) ? vector : ownerDefaultTarget.transform.InverseTransformPoint(vector));
		}

		// Token: 0x04006E5C RID: 28252
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E5D RID: 28253
		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector.")]
		public FsmVector3 vector;

		// Token: 0x04006E5E RID: 28254
		[Tooltip("Movement in x axis.")]
		public FsmFloat x;

		// Token: 0x04006E5F RID: 28255
		[Tooltip("Movement in y axis.")]
		public FsmFloat y;

		// Token: 0x04006E60 RID: 28256
		[Tooltip("Movement in z axis.")]
		public FsmFloat z;

		// Token: 0x04006E61 RID: 28257
		[Tooltip("Coordinate space to move in.")]
		public Space space;

		// Token: 0x04006E62 RID: 28258
		[Tooltip("Keep running every frame.")]
		public bool everyFrame;
	}
}
