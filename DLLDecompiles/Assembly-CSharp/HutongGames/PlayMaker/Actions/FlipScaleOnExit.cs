using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C47 RID: 3143
	[ActionCategory(ActionCategory.Transform)]
	public class FlipScaleOnExit : FsmStateAction
	{
		// Token: 0x06005F5A RID: 24410 RVA: 0x001E4C29 File Offset: 0x001E2E29
		public override void Reset()
		{
			this.flipHorizontally = false;
			this.flipVertically = false;
		}

		// Token: 0x06005F5B RID: 24411 RVA: 0x001E4C39 File Offset: 0x001E2E39
		public override void OnExit()
		{
			this.DoFlipScale();
		}

		// Token: 0x06005F5C RID: 24412 RVA: 0x001E4C44 File Offset: 0x001E2E44
		private void DoFlipScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 localScale = ownerDefaultTarget.transform.localScale;
			if (this.flipHorizontally)
			{
				localScale.x = -localScale.x;
			}
			if (this.flipVertically)
			{
				localScale.y = -localScale.y;
			}
			ownerDefaultTarget.transform.localScale = localScale;
		}

		// Token: 0x04005CBC RID: 23740
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005CBD RID: 23741
		public bool flipHorizontally;

		// Token: 0x04005CBE RID: 23742
		public bool flipVertically;
	}
}
