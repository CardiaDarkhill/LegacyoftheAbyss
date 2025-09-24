using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7B RID: 3195
	public class GetPosition2dAs3d : FsmStateAction
	{
		// Token: 0x06006042 RID: 24642 RVA: 0x001E7825 File Offset: 0x001E5A25
		public override void Reset()
		{
			this.SourceXY = null;
			this.SourceZ = null;
			this.StoreVector = null;
		}

		// Token: 0x06006043 RID: 24643 RVA: 0x001E783C File Offset: 0x001E5A3C
		public override void OnEnter()
		{
			GameObject safe = this.SourceXY.GetSafe(this);
			GameObject safe2 = this.SourceZ.GetSafe(this);
			Vector2 vector = safe ? safe.transform.position : Vector2.zero;
			float z = safe2 ? safe2.transform.position.z : 0f;
			this.StoreVector.Value = new Vector3(vector.x, vector.y, z);
			base.Finish();
		}

		// Token: 0x04005D97 RID: 23959
		public FsmOwnerDefault SourceXY;

		// Token: 0x04005D98 RID: 23960
		public FsmOwnerDefault SourceZ;

		// Token: 0x04005D99 RID: 23961
		[UIHint(UIHint.Variable)]
		public FsmVector3 StoreVector;
	}
}
