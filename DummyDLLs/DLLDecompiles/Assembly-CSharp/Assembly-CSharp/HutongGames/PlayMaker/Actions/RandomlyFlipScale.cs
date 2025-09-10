using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE9 RID: 3305
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("50/50 chance to either x scale as is or flip it")]
	public class RandomlyFlipScale : FsmStateAction
	{
		// Token: 0x06006237 RID: 25143 RVA: 0x001F0A79 File Offset: 0x001EEC79
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006238 RID: 25144 RVA: 0x001F0A84 File Offset: 0x001EEC84
		public override void OnEnter()
		{
			if ((double)Random.value >= 0.5)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				ownerDefaultTarget.transform.localScale = new Vector3(-ownerDefaultTarget.transform.localScale.x, ownerDefaultTarget.transform.localScale.y, ownerDefaultTarget.transform.localScale.z);
			}
			base.Finish();
		}

		// Token: 0x0400604F RID: 24655
		public FsmOwnerDefault gameObject;
	}
}
