using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012E7 RID: 4839
	public class GetIsInteractionQueued : FsmStateAction
	{
		// Token: 0x06007E2D RID: 32301 RVA: 0x00258613 File Offset: 0x00256813
		public override void Reset()
		{
			this.Target = null;
			this.StoreIsQueued = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007E2E RID: 32302 RVA: 0x0025862C File Offset: 0x0025682C
		public override void OnEnter()
		{
			this.StoreIsQueued.Value = false;
			this.interactable = null;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.interactable = safe.GetComponent<InteractableBase>();
				if (this.interactable)
				{
					this.DoAction();
				}
			}
			if (!this.EveryFrame || !this.interactable)
			{
				base.Finish();
			}
		}

		// Token: 0x06007E2F RID: 32303 RVA: 0x0025869B File Offset: 0x0025689B
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007E30 RID: 32304 RVA: 0x002586A4 File Offset: 0x002568A4
		private void DoAction()
		{
			bool isQueued = this.interactable.IsQueued;
			this.StoreIsQueued.Value = isQueued;
			base.Fsm.Event(isQueued ? this.IsQueuedEvent : this.NotQueuedEvent);
		}

		// Token: 0x04007E02 RID: 32258
		[RequiredField]
		[CheckForComponent(typeof(InteractableBase))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E03 RID: 32259
		public FsmEvent IsQueuedEvent;

		// Token: 0x04007E04 RID: 32260
		public FsmEvent NotQueuedEvent;

		// Token: 0x04007E05 RID: 32261
		[UIHint(UIHint.Variable)]
		public FsmBool StoreIsQueued;

		// Token: 0x04007E06 RID: 32262
		public bool EveryFrame;

		// Token: 0x04007E07 RID: 32263
		private InteractableBase interactable;
	}
}
