using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C67 RID: 3175
	public class GetDirection2D : FsmStateAction
	{
		// Token: 0x06005FF2 RID: 24562 RVA: 0x001E6433 File Offset: 0x001E4633
		public override void Reset()
		{
			this.From = null;
			this.To = null;
			this.StoreVector = null;
			this.StoreX = null;
			this.StoreY = null;
			this.EveryFrame = false;
		}

		// Token: 0x06005FF3 RID: 24563 RVA: 0x001E645F File Offset: 0x001E465F
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FF4 RID: 24564 RVA: 0x001E6475 File Offset: 0x001E4675
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06005FF5 RID: 24565 RVA: 0x001E6480 File Offset: 0x001E4680
		private void DoAction()
		{
			GameObject safe = this.From.GetSafe(this);
			if (!safe || !this.To.Value)
			{
				return;
			}
			Transform transform = safe.transform;
			Vector2 vector = this.To.Value.transform.position - transform.position;
			this.StoreVector.Value = vector.normalized;
			this.StoreX.Value = Mathf.Sign(vector.x);
			this.StoreY.Value = Mathf.Sign(vector.y);
		}

		// Token: 0x04005D44 RID: 23876
		public FsmOwnerDefault From;

		// Token: 0x04005D45 RID: 23877
		public FsmGameObject To;

		// Token: 0x04005D46 RID: 23878
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreVector;

		// Token: 0x04005D47 RID: 23879
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreX;

		// Token: 0x04005D48 RID: 23880
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreY;

		// Token: 0x04005D49 RID: 23881
		public bool EveryFrame;
	}
}
