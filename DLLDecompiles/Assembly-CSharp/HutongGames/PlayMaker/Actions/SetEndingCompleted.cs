using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001380 RID: 4992
	public class SetEndingCompleted : FsmStateAction
	{
		// Token: 0x0600806D RID: 32877 RVA: 0x0025E635 File Offset: 0x0025C835
		public override void Reset()
		{
			this.EndingType = null;
		}

		// Token: 0x0600806E RID: 32878 RVA: 0x0025E640 File Offset: 0x0025C840
		public override void OnEnter()
		{
			PlayerData instance = PlayerData.instance;
			SaveSlotCompletionIcons.CompletionState completionState = (SaveSlotCompletionIcons.CompletionState)this.EndingType.Value;
			instance.CompletedEndings |= completionState;
			instance.LastCompletedEnding = completionState;
			GameManager instance2 = GameManager.instance;
			if (instance2)
			{
				instance2.RecordGameComplete();
			}
			base.Finish();
		}

		// Token: 0x04007FCF RID: 32719
		[RequiredField]
		[ObjectType(typeof(SaveSlotCompletionIcons.CompletionState))]
		public FsmEnum EndingType;
	}
}
