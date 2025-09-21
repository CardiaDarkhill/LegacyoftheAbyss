using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB5 RID: 4021
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Translates a Game Object's RigidBody. Unlike Translate this will respect physics collisions.")]
	public class TranslatePosition : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EF5 RID: 28405 RVA: 0x002250B0 File Offset: 0x002232B0
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
			this.perSecond = true;
			this.everyFrame = true;
		}

		// Token: 0x06006EF6 RID: 28406 RVA: 0x00225116 File Offset: 0x00223316
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006EF7 RID: 28407 RVA: 0x00225124 File Offset: 0x00223324
		public override void OnFixedUpdate()
		{
			this.DoMovePosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EF8 RID: 28408 RVA: 0x0022513C File Offset: 0x0022333C
		private void DoMovePosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 vector = this.vector.IsNone ? new Vector3(this.x.Value, this.y.Value, this.z.Value) : this.vector.Value;
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
			if (this.perSecond)
			{
				vector *= Time.deltaTime;
			}
			base.rigidbody.MovePosition((this.space == Space.World) ? (base.rigidbody.position + vector) : (base.rigidbody.position + ownerDefaultTarget.transform.TransformVector(vector)));
		}

		// Token: 0x04006EA5 RID: 28325
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006EA6 RID: 28326
		[UIHint(UIHint.Variable)]
		[Tooltip("Movement vector.")]
		public FsmVector3 vector;

		// Token: 0x04006EA7 RID: 28327
		[Tooltip("Movement in x axis.")]
		public FsmFloat x;

		// Token: 0x04006EA8 RID: 28328
		[Tooltip("Movement in y axis.")]
		public FsmFloat y;

		// Token: 0x04006EA9 RID: 28329
		[Tooltip("Movement in z axis.")]
		public FsmFloat z;

		// Token: 0x04006EAA RID: 28330
		[Tooltip("Coordinate space to move in.")]
		public Space space;

		// Token: 0x04006EAB RID: 28331
		[Tooltip("Translate over one second")]
		public bool perSecond;

		// Token: 0x04006EAC RID: 28332
		[Tooltip("Keep running every frame.")]
		public bool everyFrame;
	}
}
