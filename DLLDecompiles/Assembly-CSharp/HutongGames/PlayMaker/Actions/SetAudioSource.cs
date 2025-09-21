using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1F RID: 3359
	[ActionCategory("GameObject")]
	[Tooltip("Set Audio Source to active or inactive. Can only be one Audio Source on object. ")]
	public class SetAudioSource : FsmStateAction
	{
		// Token: 0x06006317 RID: 25367 RVA: 0x001F54DE File Offset: 0x001F36DE
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x06006318 RID: 25368 RVA: 0x001F54F4 File Offset: 0x001F36F4
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
					if (component != null)
					{
						component.enabled = this.active.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04006186 RID: 24966
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006187 RID: 24967
		public FsmBool active;
	}
}
