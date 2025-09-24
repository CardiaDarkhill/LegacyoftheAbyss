using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEC RID: 3308
	public class RandomVector3 : FsmStateAction
	{
		// Token: 0x06006242 RID: 25154 RVA: 0x001F0C3F File Offset: 0x001EEE3F
		public override void Reset()
		{
			this.Start = null;
			this.End = null;
			this.StoreResult = null;
			this.EveryFrame = false;
		}

		// Token: 0x06006243 RID: 25155 RVA: 0x001F0C5D File Offset: 0x001EEE5D
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006244 RID: 25156 RVA: 0x001F0C73 File Offset: 0x001EEE73
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06006245 RID: 25157 RVA: 0x001F0C7C File Offset: 0x001EEE7C
		private void DoAction()
		{
			Vector3 value = this.Start.Value;
			Vector3 value2 = this.End.Value;
			this.StoreResult.Value = new Vector3(Random.Range(value.x, value2.x), Random.Range(value.y, value2.y), Random.Range(value.z, value2.z));
		}

		// Token: 0x04006055 RID: 24661
		public FsmVector3 Start;

		// Token: 0x04006056 RID: 24662
		public FsmVector3 End;

		// Token: 0x04006057 RID: 24663
		[UIHint(UIHint.Variable)]
		public FsmVector3 StoreResult;

		// Token: 0x04006058 RID: 24664
		public bool EveryFrame;
	}
}
