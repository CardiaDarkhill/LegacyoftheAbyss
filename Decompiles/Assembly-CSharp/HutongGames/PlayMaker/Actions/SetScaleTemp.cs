using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D87 RID: 3463
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the SCALE of a target GameObject, and will reset back to initial value when target is disabled.")]
	public class SetScaleTemp : SetVectorTempAction
	{
		// Token: 0x060064D4 RID: 25812 RVA: 0x001FD274 File Offset: 0x001FB474
		public override bool HideSpace()
		{
			return true;
		}

		// Token: 0x060064D5 RID: 25813 RVA: 0x001FD277 File Offset: 0x001FB477
		protected override Vector3 GetVector(Transform transform)
		{
			return transform.localScale;
		}

		// Token: 0x060064D6 RID: 25814 RVA: 0x001FD27F File Offset: 0x001FB47F
		protected override void SetVector(Transform transform, Vector3 vector)
		{
			transform.localScale = vector;
		}
	}
}
