using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9B RID: 3483
	public class TransformVector3 : TransformVector<FsmVector3>
	{
		// Token: 0x06006531 RID: 25905 RVA: 0x001FE797 File Offset: 0x001FC997
		protected override Vector3 GetInputVector()
		{
			return this.Vector.Value;
		}

		// Token: 0x06006532 RID: 25906 RVA: 0x001FE7A4 File Offset: 0x001FC9A4
		protected override void SetStoreResult(Vector3 value)
		{
			this.StoreResult.Value = value;
		}
	}
}
