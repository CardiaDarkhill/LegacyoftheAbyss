using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FE6 RID: 4070
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Translates a Game Object's RigidBody2d. Unlike Translate2d this will respect physics collisions.")]
	public class TranslatePosition2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06007005 RID: 28677 RVA: 0x0022A1E8 File Offset: 0x002283E8
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
			this.perSecond = true;
			this.everyFrame = true;
		}

		// Token: 0x06007006 RID: 28678 RVA: 0x0022A23C File Offset: 0x0022843C
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007007 RID: 28679 RVA: 0x0022A24A File Offset: 0x0022844A
		public override void OnFixedUpdate()
		{
			this.DoTranslatePosition2d();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007008 RID: 28680 RVA: 0x0022A260 File Offset: 0x00228460
		private void DoTranslatePosition2d()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector2 vector = this.vector.IsNone ? new Vector2(this.x.Value, this.y.Value) : this.vector.Value;
			if (!this.x.IsNone)
			{
				vector.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				vector.y = this.y.Value;
			}
			if (this.perSecond)
			{
				vector *= Time.deltaTime;
			}
			if (this.space == Space.Self)
			{
				vector = base.cachedTransform.TransformVector(new Vector3(vector.x, vector.y, 0f));
			}
			base.rigidbody2d.MovePosition(base.rigidbody2d.position + vector);
		}

		// Token: 0x0400700C RID: 28684
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400700D RID: 28685
		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector.")]
		public FsmVector2 vector;

		// Token: 0x0400700E RID: 28686
		[Tooltip("Movement in x axis.")]
		public FsmFloat x;

		// Token: 0x0400700F RID: 28687
		[Tooltip("Movement in y axis.")]
		public FsmFloat y;

		// Token: 0x04007010 RID: 28688
		[Tooltip("Coordinate space to move in.")]
		public Space space;

		// Token: 0x04007011 RID: 28689
		[Tooltip("Translate over one second")]
		public bool perSecond;

		// Token: 0x04007012 RID: 28690
		[Tooltip("Keep running every frame.")]
		public bool everyFrame;
	}
}
