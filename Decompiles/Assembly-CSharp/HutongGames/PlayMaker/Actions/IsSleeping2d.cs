using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FCE RID: 4046
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Tests if a Game Object's Rigidbody 2D is sleeping.")]
	public class IsSleeping2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F8D RID: 28557 RVA: 0x00227B39 File Offset: 0x00225D39
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x06006F8E RID: 28558 RVA: 0x00227B5E File Offset: 0x00225D5E
		public override void OnEnter()
		{
			this.DoIsSleeping();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F8F RID: 28559 RVA: 0x00227B74 File Offset: 0x00225D74
		public override void OnUpdate()
		{
			this.DoIsSleeping();
		}

		// Token: 0x06006F90 RID: 28560 RVA: 0x00227B7C File Offset: 0x00225D7C
		private void DoIsSleeping()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			bool flag = base.rigidbody2d.IsSleeping();
			this.store.Value = flag;
			base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x04006F59 RID: 28505
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F5A RID: 28506
		[Tooltip("Event sent if sleeping")]
		public FsmEvent trueEvent;

		// Token: 0x04006F5B RID: 28507
		[Tooltip("Event sent if not sleeping")]
		public FsmEvent falseEvent;

		// Token: 0x04006F5C RID: 28508
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the value in a Boolean variable")]
		public FsmBool store;

		// Token: 0x04006F5D RID: 28509
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
