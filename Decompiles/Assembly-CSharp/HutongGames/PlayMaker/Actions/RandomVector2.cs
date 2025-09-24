using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEB RID: 3307
	public class RandomVector2 : FsmStateAction
	{
		// Token: 0x0600623D RID: 25149 RVA: 0x001F0BA1 File Offset: 0x001EEDA1
		public override void Reset()
		{
			this.Start = null;
			this.End = null;
			this.StoreResult = null;
			this.EveryFrame = false;
		}

		// Token: 0x0600623E RID: 25150 RVA: 0x001F0BBF File Offset: 0x001EEDBF
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600623F RID: 25151 RVA: 0x001F0BD5 File Offset: 0x001EEDD5
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06006240 RID: 25152 RVA: 0x001F0BE0 File Offset: 0x001EEDE0
		private void DoAction()
		{
			Vector2 value = this.Start.Value;
			Vector2 value2 = this.End.Value;
			this.StoreResult.Value = new Vector2(Random.Range(value.x, value2.x), Random.Range(value.y, value2.y));
		}

		// Token: 0x04006051 RID: 24657
		public FsmVector2 Start;

		// Token: 0x04006052 RID: 24658
		public FsmVector2 End;

		// Token: 0x04006053 RID: 24659
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreResult;

		// Token: 0x04006054 RID: 24660
		public bool EveryFrame;
	}
}
