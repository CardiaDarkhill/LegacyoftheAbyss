using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001316 RID: 4886
	public class ActivateNPCFlyAroundTalking : FsmStateAction
	{
		// Token: 0x06007ED2 RID: 32466 RVA: 0x00259E58 File Offset: 0x00258058
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007ED3 RID: 32467 RVA: 0x00259E64 File Offset: 0x00258064
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			NPCFlyAround component = safe.GetComponent<NPCFlyAround>();
			if (this.SetActive.Value)
			{
				component.EnableTalkingFlyAround();
			}
			else
			{
				component.DisableTalkingFlyAround();
			}
			base.Finish();
		}

		// Token: 0x04007E80 RID: 32384
		[CheckForComponent(typeof(NPCFlyAround))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E81 RID: 32385
		public FsmBool SetActive;
	}
}
