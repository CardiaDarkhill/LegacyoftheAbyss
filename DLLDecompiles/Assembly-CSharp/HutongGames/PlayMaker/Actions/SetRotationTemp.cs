using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D89 RID: 3465
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the ROTATION of a target GameObject, and will reset back to initial value when target is disabled.")]
	public class SetRotationTemp : SetVectorTempAction
	{
		// Token: 0x060064DB RID: 25819 RVA: 0x001FD2C8 File Offset: 0x001FB4C8
		protected override Vector3 GetVector(Transform transform)
		{
			if (this.Space != Space.World)
			{
				return transform.localEulerAngles;
			}
			return transform.eulerAngles;
		}

		// Token: 0x060064DC RID: 25820 RVA: 0x001FD2DF File Offset: 0x001FB4DF
		protected override void SetVector(Transform transform, Vector3 vector)
		{
			if (this.Space == Space.World)
			{
				transform.eulerAngles = vector;
				return;
			}
			transform.localEulerAngles = vector;
		}
	}
}
