using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200130D RID: 4877
	public class ShowNeedolinTextForOwnerInState : FsmStateAction
	{
		// Token: 0x06007EB0 RID: 32432 RVA: 0x002598F8 File Offset: 0x00257AF8
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007EB1 RID: 32433 RVA: 0x00259904 File Offset: 0x00257B04
		public override void OnEnter()
		{
			this.owner = null;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				BlackThreadState component = safe.GetComponent<BlackThreadState>();
				if (component && component.IsInForcedSing)
				{
					return;
				}
				this.owner = safe.GetComponent<NeedolinTextOwner>();
				if (this.owner)
				{
					this.owner.AddNeedolinText();
				}
			}
		}

		// Token: 0x06007EB2 RID: 32434 RVA: 0x00259969 File Offset: 0x00257B69
		public override void OnExit()
		{
			if (!this.owner)
			{
				return;
			}
			this.owner.RemoveNeedolinText();
			this.owner = null;
		}

		// Token: 0x04007E63 RID: 32355
		[RequiredField]
		[CheckForComponent(typeof(NeedolinTextOwner))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E64 RID: 32356
		private NeedolinTextOwner owner;
	}
}
