using System;
using HutongGames.PlayMaker;
using UnityEngine;

namespace QuestPlaymakerActions
{
	// Token: 0x02000891 RID: 2193
	[ActionCategory("Quests")]
	public class CheckQuestBoard : FsmStateAction
	{
		// Token: 0x06004C31 RID: 19505 RVA: 0x00167403 File Offset: 0x00165603
		public override void Reset()
		{
			this.Target = null;
			this.AvailableQuestsCount = null;
			this.HasQuestsEvent = null;
			this.NoQuestsEvent = null;
		}

		// Token: 0x06004C32 RID: 19506 RVA: 0x00167424 File Offset: 0x00165624
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				QuestItemBoard component = safe.GetComponent<QuestItemBoard>();
				if (component)
				{
					int availableQuestsCount = component.AvailableQuestsCount;
					this.AvailableQuestsCount.Value = availableQuestsCount;
					base.Fsm.Event((availableQuestsCount > 0) ? this.HasQuestsEvent : this.NoQuestsEvent);
				}
			}
			base.Finish();
		}

		// Token: 0x04004D8A RID: 19850
		public FsmOwnerDefault Target;

		// Token: 0x04004D8B RID: 19851
		[UIHint(UIHint.Variable)]
		public FsmInt AvailableQuestsCount;

		// Token: 0x04004D8C RID: 19852
		public FsmEvent HasQuestsEvent;

		// Token: 0x04004D8D RID: 19853
		public FsmEvent NoQuestsEvent;
	}
}
