using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001301 RID: 4865
	public class RecordPlayerStory : FsmStateAction
	{
		// Token: 0x06007E80 RID: 32384 RVA: 0x0025934A File Offset: 0x0025754A
		public override void Reset()
		{
			this.EventType = null;
		}

		// Token: 0x06007E81 RID: 32385 RVA: 0x00259353 File Offset: 0x00257553
		public override void OnEnter()
		{
			PlayerStory.RecordEvent((PlayerStory.EventTypes)this.EventType.Value);
			base.Finish();
		}

		// Token: 0x04007E41 RID: 32321
		[ObjectType(typeof(PlayerStory.EventTypes))]
		public FsmEnum EventType;
	}
}
