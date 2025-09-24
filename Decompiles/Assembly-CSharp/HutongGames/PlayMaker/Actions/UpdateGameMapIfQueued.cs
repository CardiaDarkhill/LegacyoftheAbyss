using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001373 RID: 4979
	public sealed class UpdateGameMapIfQueued : FsmStateAction
	{
		// Token: 0x06008043 RID: 32835 RVA: 0x0025DF1A File Offset: 0x0025C11A
		private bool IsSilent()
		{
			return this.silent.Value;
		}

		// Token: 0x06008044 RID: 32836 RVA: 0x0025DF27 File Offset: 0x0025C127
		public override void Reset()
		{
			this.silent = null;
			this.delay = null;
			this.didUpdateEvent = null;
		}

		// Token: 0x06008045 RID: 32837 RVA: 0x0025DF40 File Offset: 0x0025C140
		public override void OnEnter()
		{
			if (PlayerData.instance.mapUpdateQueued)
			{
				GameManager instance = GameManager.instance;
				if (instance)
				{
					bool flag;
					if (this.silent.Value)
					{
						flag = instance.UpdateGameMap();
					}
					else
					{
						flag = instance.UpdateGameMapWithPopup(this.delay.Value);
					}
					if (flag)
					{
						base.Fsm.Event(this.didUpdateEvent);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007FAC RID: 32684
		public FsmBool silent;

		// Token: 0x04007FAD RID: 32685
		[HideIf("IsSilent")]
		public FsmFloat delay;

		// Token: 0x04007FAE RID: 32686
		public FsmEvent didUpdateEvent;
	}
}
