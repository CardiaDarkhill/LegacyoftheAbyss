using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FCC RID: 4044
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Is the rigidbody2D constrained from rotating? Note: Prefer SetRigidBody2dConstraints when working in Unity 5 or higher.")]
	public class IsFixedAngle2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F83 RID: 28547 RVA: 0x002279E0 File Offset: 0x00225BE0
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x06006F84 RID: 28548 RVA: 0x00227A05 File Offset: 0x00225C05
		public override void OnEnter()
		{
			this.DoIsFixedAngle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F85 RID: 28549 RVA: 0x00227A1B File Offset: 0x00225C1B
		public override void OnUpdate()
		{
			this.DoIsFixedAngle();
		}

		// Token: 0x06006F86 RID: 28550 RVA: 0x00227A24 File Offset: 0x00225C24
		private void DoIsFixedAngle()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			bool flag = (base.rigidbody2d.constraints & RigidbodyConstraints2D.FreezeRotation) > RigidbodyConstraints2D.None;
			this.store.Value = flag;
			base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x04006F4F RID: 28495
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F50 RID: 28496
		[Tooltip("Event sent if the Rigidbody2D does have fixed angle")]
		public FsmEvent trueEvent;

		// Token: 0x04006F51 RID: 28497
		[Tooltip("Event sent if the Rigidbody2D doesn't have fixed angle")]
		public FsmEvent falseEvent;

		// Token: 0x04006F52 RID: 28498
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the fixedAngle flag")]
		public FsmBool store;

		// Token: 0x04006F53 RID: 28499
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
