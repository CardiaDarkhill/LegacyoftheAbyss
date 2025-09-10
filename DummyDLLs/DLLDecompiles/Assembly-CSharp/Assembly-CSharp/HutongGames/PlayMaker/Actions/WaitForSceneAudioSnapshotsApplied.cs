using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F3 RID: 4851
	public class WaitForSceneAudioSnapshotsApplied : FsmStateAction
	{
		// Token: 0x06007E53 RID: 32339 RVA: 0x00258AD5 File Offset: 0x00256CD5
		public override void Reset()
		{
			this.SendEvent = null;
		}

		// Token: 0x06007E54 RID: 32340 RVA: 0x00258AE0 File Offset: 0x00256CE0
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance)
			{
				CustomSceneManager sm = instance.sm;
				if (!sm.IsAudioSnapshotsApplied)
				{
					Action temp = null;
					temp = delegate()
					{
						this.Fsm.Event(this.SendEvent);
						sm.AudioSnapshotsApplied -= temp;
						this.Finish();
					};
					sm.AudioSnapshotsApplied += temp;
					return;
				}
			}
			base.Fsm.Event(this.SendEvent);
			base.Finish();
		}

		// Token: 0x04007E16 RID: 32278
		public FsmEvent SendEvent;
	}
}
