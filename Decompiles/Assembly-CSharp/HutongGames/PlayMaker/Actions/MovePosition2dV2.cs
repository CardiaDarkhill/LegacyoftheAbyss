using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FD5 RID: 4053
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Moves a Game Object's RigidBody2D to a new position. Unlike SetPosition this will respect physics collisions.")]
	public class MovePosition2dV2 : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FB1 RID: 28593 RVA: 0x00228648 File Offset: 0x00226848
		public override void Reset()
		{
			this.gameObject = null;
			this.targetPosition = new FsmGameObject
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
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x06006FB2 RID: 28594 RVA: 0x002286A7 File Offset: 0x002268A7
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006FB3 RID: 28595 RVA: 0x002286B5 File Offset: 0x002268B5
		public override void OnFixedUpdate()
		{
			if (!this.DoMovePosition() || !this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FB4 RID: 28596 RVA: 0x002286D0 File Offset: 0x002268D0
		private bool DoMovePosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return false;
			}
			Vector3 vector;
			if (!this.targetPosition.IsNone && this.targetPosition.Value != null)
			{
				if (this.space == Space.World)
				{
					vector = this.targetPosition.Value.transform.position;
				}
				else
				{
					vector = this.targetPosition.Value.transform.localPosition;
				}
			}
			else if (this.vector.IsNone)
			{
				vector = ((this.space == Space.World) ? new Vector3(base.rigidbody2d.position.x, base.rigidbody2d.position.y, 0f) : base.cachedTransform.TransformPoint(base.rigidbody.position));
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
			base.rigidbody2d.MovePosition((this.space == Space.World) ? vector : ownerDefaultTarget.transform.InverseTransformPoint(vector));
			return true;
		}

		// Token: 0x04006F95 RID: 28565
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F96 RID: 28566
		public FsmGameObject targetPosition;

		// Token: 0x04006F97 RID: 28567
		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector.")]
		public FsmVector2 vector;

		// Token: 0x04006F98 RID: 28568
		[Tooltip("Movement in x axis.")]
		public FsmFloat x;

		// Token: 0x04006F99 RID: 28569
		[Tooltip("Movement in y axis.")]
		public FsmFloat y;

		// Token: 0x04006F9A RID: 28570
		[Tooltip("Coordinate space to move in.")]
		public Space space;

		// Token: 0x04006F9B RID: 28571
		[Tooltip("Keep running every frame.")]
		public bool everyFrame;
	}
}
