using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D11 RID: 3345
	public class SendEventEnableFsm : FsmStateAction
	{
		// Token: 0x060062D5 RID: 25301 RVA: 0x001F3BAB File Offset: 0x001F1DAB
		public override void Reset()
		{
			this.Target = null;
			this.SendToChildren = null;
			this.EventName = null;
		}

		// Token: 0x060062D6 RID: 25302 RVA: 0x001F3BC4 File Offset: 0x001F1DC4
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				if (this.SendToChildren.Value)
				{
					foreach (PlayMakerFSM playMakerFSM in safe.GetComponentsInChildren<PlayMakerFSM>())
					{
						if (!playMakerFSM.enabled)
						{
							playMakerFSM.enabled = true;
						}
						playMakerFSM.SendEvent(this.EventName.Value);
					}
				}
				else
				{
					PlayMakerFSM component = safe.GetComponent<PlayMakerFSM>();
					if (component)
					{
						if (!component.enabled)
						{
							component.enabled = true;
						}
						component.SendEvent(this.EventName.Value);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400613D RID: 24893
		public FsmOwnerDefault Target;

		// Token: 0x0400613E RID: 24894
		public FsmBool SendToChildren;

		// Token: 0x0400613F RID: 24895
		public FsmString EventName;
	}
}
