using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FCD RID: 4045
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Tests if a Game Object's Rigid Body 2D is Kinematic.")]
	public class IsKinematic2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F88 RID: 28552 RVA: 0x00227A90 File Offset: 0x00225C90
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x06006F89 RID: 28553 RVA: 0x00227AB5 File Offset: 0x00225CB5
		public override void OnEnter()
		{
			this.DoIsKinematic();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F8A RID: 28554 RVA: 0x00227ACB File Offset: 0x00225CCB
		public override void OnUpdate()
		{
			this.DoIsKinematic();
		}

		// Token: 0x06006F8B RID: 28555 RVA: 0x00227AD4 File Offset: 0x00225CD4
		private void DoIsKinematic()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			bool isKinematic = base.rigidbody2d.isKinematic;
			this.store.Value = isKinematic;
			base.Fsm.Event(isKinematic ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x04006F54 RID: 28500
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("the GameObject with a Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F55 RID: 28501
		[Tooltip("Event Sent if Kinematic")]
		public FsmEvent trueEvent;

		// Token: 0x04006F56 RID: 28502
		[Tooltip("Event sent if not Kinematic")]
		public FsmEvent falseEvent;

		// Token: 0x04006F57 RID: 28503
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Kinematic state")]
		public FsmBool store;

		// Token: 0x04006F58 RID: 28504
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
