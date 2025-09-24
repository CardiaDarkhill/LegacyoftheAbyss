using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FD4 RID: 4052
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Moves a Game Object's RigidBody2D to a new position. Unlike SetPosition this will respect physics collisions.")]
	public class MovePosition2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FAC RID: 28588 RVA: 0x002284D0 File Offset: 0x002266D0
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
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x06006FAD RID: 28589 RVA: 0x0022851D File Offset: 0x0022671D
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006FAE RID: 28590 RVA: 0x0022852B File Offset: 0x0022672B
		public override void OnFixedUpdate()
		{
			this.DoMovePosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FAF RID: 28591 RVA: 0x00228544 File Offset: 0x00226744
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
		}

		// Token: 0x04006F8F RID: 28559
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F90 RID: 28560
		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector.")]
		public FsmVector2 vector;

		// Token: 0x04006F91 RID: 28561
		[Tooltip("Movement in x axis.")]
		public FsmFloat x;

		// Token: 0x04006F92 RID: 28562
		[Tooltip("Movement in y axis.")]
		public FsmFloat y;

		// Token: 0x04006F93 RID: 28563
		[Tooltip("Coordinate space to move in.")]
		public Space space;

		// Token: 0x04006F94 RID: 28564
		[Tooltip("Keep running every frame.")]
		public bool everyFrame;
	}
}
