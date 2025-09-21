using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEA RID: 3306
	[ActionCategory(ActionCategory.Math)]
	public class RandomlyFlipYScale : FsmStateAction
	{
		// Token: 0x0600623A RID: 25146 RVA: 0x001F0B0D File Offset: 0x001EED0D
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x0600623B RID: 25147 RVA: 0x001F0B18 File Offset: 0x001EED18
		public override void OnEnter()
		{
			if ((double)Random.value >= 0.5)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				ownerDefaultTarget.transform.localScale = new Vector3(ownerDefaultTarget.transform.localScale.x, -ownerDefaultTarget.transform.localScale.y, ownerDefaultTarget.transform.localScale.z);
			}
			base.Finish();
		}

		// Token: 0x04006050 RID: 24656
		public FsmOwnerDefault gameObject;
	}
}
