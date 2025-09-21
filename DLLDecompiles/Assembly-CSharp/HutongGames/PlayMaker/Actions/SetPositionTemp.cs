using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D88 RID: 3464
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the POSITION of a target GameObject, and will reset back to initial value when target is disabled.")]
	public class SetPositionTemp : SetVectorTempAction
	{
		// Token: 0x060064D8 RID: 25816 RVA: 0x001FD290 File Offset: 0x001FB490
		protected override Vector3 GetVector(Transform transform)
		{
			if (this.Space != Space.World)
			{
				return transform.localPosition;
			}
			return transform.position;
		}

		// Token: 0x060064D9 RID: 25817 RVA: 0x001FD2A7 File Offset: 0x001FB4A7
		protected override void SetVector(Transform transform, Vector3 vector)
		{
			if (this.Space == Space.World)
			{
				transform.position = vector;
				return;
			}
			transform.localPosition = vector;
		}
	}
}
