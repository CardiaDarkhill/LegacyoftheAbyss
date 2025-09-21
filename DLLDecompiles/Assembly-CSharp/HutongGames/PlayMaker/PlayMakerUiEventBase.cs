using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AEE RID: 2798
	public abstract class PlayMakerUiEventBase : MonoBehaviour
	{
		// Token: 0x060058D8 RID: 22744 RVA: 0x001C33B4 File Offset: 0x001C15B4
		public void AddTargetFsm(PlayMakerFSM fsm)
		{
			if (!this.TargetsFsm(fsm))
			{
				this.targetFsms.Add(fsm);
			}
			this.Initialize();
		}

		// Token: 0x060058D9 RID: 22745 RVA: 0x001C33D4 File Offset: 0x001C15D4
		private bool TargetsFsm(PlayMakerFSM fsm)
		{
			for (int i = 0; i < this.targetFsms.Count; i++)
			{
				PlayMakerFSM y = this.targetFsms[i];
				if (fsm == y)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060058DA RID: 22746 RVA: 0x001C3410 File Offset: 0x001C1610
		protected void OnEnable()
		{
			this.Initialize();
		}

		// Token: 0x060058DB RID: 22747 RVA: 0x001C3418 File Offset: 0x001C1618
		public void PreProcess()
		{
			this.Initialize();
		}

		// Token: 0x060058DC RID: 22748 RVA: 0x001C3420 File Offset: 0x001C1620
		protected virtual void Initialize()
		{
			this.initialized = true;
		}

		// Token: 0x060058DD RID: 22749 RVA: 0x001C342C File Offset: 0x001C162C
		protected void SendEvent(FsmEvent fsmEvent)
		{
			for (int i = 0; i < this.targetFsms.Count; i++)
			{
				this.targetFsms[i].Fsm.Event(base.gameObject, fsmEvent);
			}
		}

		// Token: 0x04005417 RID: 21527
		public List<PlayMakerFSM> targetFsms = new List<PlayMakerFSM>();

		// Token: 0x04005418 RID: 21528
		[NonSerialized]
		protected bool initialized;
	}
}
