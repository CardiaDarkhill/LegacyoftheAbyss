using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D12 RID: 3346
	[ActionCategory("Hollow Knight")]
	public class SendEventToGameObjectOptimized : FsmStateAction
	{
		// Token: 0x060062D8 RID: 25304 RVA: 0x001F3C70 File Offset: 0x001F1E70
		public override void Reset()
		{
			this.target = null;
			this.sendEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x060062D9 RID: 25305 RVA: 0x001F3C88 File Offset: 0x001F1E88
		public override void OnEnter()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				FSMUtility.SendEventToGameObject(safe, this.sendEvent.Value, false);
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060062DA RID: 25306 RVA: 0x001F3CCC File Offset: 0x001F1ECC
		public override void OnUpdate()
		{
			GameObject safe = this.target.GetSafe(this);
			if (safe != null)
			{
				FSMUtility.SendEventToGameObject(safe, this.sendEvent.Value, false);
			}
		}

		// Token: 0x04006140 RID: 24896
		public FsmOwnerDefault target;

		// Token: 0x04006141 RID: 24897
		[RequiredField]
		public FsmString sendEvent;

		// Token: 0x04006142 RID: 24898
		public bool everyFrame;
	}
}
