using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012D8 RID: 4824
	public class ListenForAnimationEvent : FsmStateAction
	{
		// Token: 0x06007DD7 RID: 32215 RVA: 0x002576B3 File Offset: 0x002558B3
		public override void Reset()
		{
			this.Target = null;
			this.Response = null;
		}

		// Token: 0x06007DD8 RID: 32216 RVA: 0x002576C4 File Offset: 0x002558C4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				this.eventSource = safe.GetComponent<CaptureAnimationEvent>();
				if (this.eventSource)
				{
					this.eventSource.EventFired += this.OnEventFired;
				}
			}
		}

		// Token: 0x06007DD9 RID: 32217 RVA: 0x00257716 File Offset: 0x00255916
		public override void OnExit()
		{
			this.Unsubscribe();
		}

		// Token: 0x06007DDA RID: 32218 RVA: 0x0025771E File Offset: 0x0025591E
		private void OnEventFired()
		{
			this.Unsubscribe();
			base.Fsm.Event(this.Response);
			base.Finish();
		}

		// Token: 0x06007DDB RID: 32219 RVA: 0x0025773D File Offset: 0x0025593D
		private void Unsubscribe()
		{
			if (!this.eventSource)
			{
				return;
			}
			this.eventSource.EventFired -= this.OnEventFired;
			this.eventSource = null;
		}

		// Token: 0x04007DB9 RID: 32185
		public FsmOwnerDefault Target;

		// Token: 0x04007DBA RID: 32186
		public FsmEvent Response;

		// Token: 0x04007DBB RID: 32187
		private CaptureAnimationEvent eventSource;
	}
}
