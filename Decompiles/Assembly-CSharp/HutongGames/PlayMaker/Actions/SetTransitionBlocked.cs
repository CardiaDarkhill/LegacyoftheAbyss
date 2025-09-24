using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200133E RID: 4926
	public class SetTransitionBlocked : FsmStateAction
	{
		// Token: 0x06007F57 RID: 32599 RVA: 0x0025B18C File Offset: 0x0025938C
		public override void Reset()
		{
			this.SetTime = null;
			this.SetIsBlocked = null;
		}

		// Token: 0x06007F58 RID: 32600 RVA: 0x0025B19C File Offset: 0x0025939C
		public override void OnEnter()
		{
			if (this.SetTime.Value <= 0f)
			{
				this.Set();
			}
		}

		// Token: 0x06007F59 RID: 32601 RVA: 0x0025B1B6 File Offset: 0x002593B6
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007F5A RID: 32602 RVA: 0x0025B1BE File Offset: 0x002593BE
		public override void OnExit()
		{
			if (!base.Finished)
			{
				this.Set();
			}
		}

		// Token: 0x06007F5B RID: 32603 RVA: 0x0025B1CE File Offset: 0x002593CE
		private void DoAction()
		{
			if (Time.time >= this.SetTime.Value)
			{
				this.Set();
			}
		}

		// Token: 0x06007F5C RID: 32604 RVA: 0x0025B1E8 File Offset: 0x002593E8
		private void Set()
		{
			TransitionPoint.IsTransitionBlocked = this.SetIsBlocked.Value;
			base.Finish();
		}

		// Token: 0x04007EE0 RID: 32480
		public FsmFloat SetTime;

		// Token: 0x04007EE1 RID: 32481
		[RequiredField]
		public FsmBool SetIsBlocked;
	}
}
