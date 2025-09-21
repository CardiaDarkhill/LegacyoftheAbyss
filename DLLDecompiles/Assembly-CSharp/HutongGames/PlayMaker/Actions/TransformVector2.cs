using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9C RID: 3484
	public class TransformVector2 : TransformVector<FsmVector2>
	{
		// Token: 0x06006534 RID: 25908 RVA: 0x001FE7BA File Offset: 0x001FC9BA
		protected override Vector3 GetInputVector()
		{
			return this.Vector.Value;
		}

		// Token: 0x06006535 RID: 25909 RVA: 0x001FE7CC File Offset: 0x001FC9CC
		protected override void SetStoreResult(Vector3 value)
		{
			this.StoreResult.Value = value;
		}
	}
}
