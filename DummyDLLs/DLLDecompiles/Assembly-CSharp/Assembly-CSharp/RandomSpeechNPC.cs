using System;
using System.Linq;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200063E RID: 1598
public class RandomSpeechNPC : BasicNPCBase
{
	// Token: 0x06003951 RID: 14673 RVA: 0x000FC38C File Offset: 0x000FA58C
	protected override void Awake()
	{
		base.Awake();
		this.runningProbabilities = (from dlg in this.randomDialogues
		select dlg.Probability).ToArray<float>();
		if (this.stateTracker)
		{
			this.stateTracker.OnGetSaveState += delegate(out int value)
			{
				value = this.talkState;
			};
			this.stateTracker.OnSetSaveState += delegate(int value)
			{
				this.talkState = value;
				this.endTalkState = value;
			};
		}
	}

	// Token: 0x06003952 RID: 14674 RVA: 0x000FC410 File Offset: 0x000FA610
	protected override LocalisedString GetDialogue()
	{
		if (this.meetText.Length != 0 && this.talkState < this.meetText.Length)
		{
			if (this.talkState < 0)
			{
				this.talkState = 0;
			}
			LocalisedString result = this.meetText[this.talkState];
			this.endTalkState = this.talkState + 1;
			return result;
		}
		int num;
		LocalisedString randomItemByProbability = Probability.GetRandomItemByProbability<RandomSpeechNPC.RandomDialogue, LocalisedString>(this.randomDialogues, out num, this.runningProbabilities, null);
		for (int i = 0; i < this.randomDialogues.Length; i++)
		{
			this.runningProbabilities[i] = ((i == num) ? this.randomDialogues[i].Probability : (this.runningProbabilities[i] * this.notChosenMultiplier));
		}
		return randomItemByProbability;
	}

	// Token: 0x06003953 RID: 14675 RVA: 0x000FC4B9 File Offset: 0x000FA6B9
	protected override void OnEndDialogue()
	{
		base.OnEndDialogue();
		this.talkState = this.endTalkState;
	}

	// Token: 0x04003C16 RID: 15382
	[Space]
	[SerializeField]
	private PersistentIntItem stateTracker;

	// Token: 0x04003C17 RID: 15383
	[SerializeField]
	private LocalisedString[] meetText;

	// Token: 0x04003C18 RID: 15384
	[SerializeField]
	private RandomSpeechNPC.RandomDialogue[] randomDialogues;

	// Token: 0x04003C19 RID: 15385
	[SerializeField]
	private float notChosenMultiplier = 2f;

	// Token: 0x04003C1A RID: 15386
	private float[] runningProbabilities;

	// Token: 0x04003C1B RID: 15387
	private int talkState;

	// Token: 0x04003C1C RID: 15388
	private int endTalkState;

	// Token: 0x0200195E RID: 6494
	[Serializable]
	private class RandomDialogue : Probability.ProbabilityBase<LocalisedString>
	{
		// Token: 0x17001093 RID: 4243
		// (get) Token: 0x0600940A RID: 37898 RVA: 0x002A0EF6 File Offset: 0x0029F0F6
		public override LocalisedString Item
		{
			get
			{
				return this.Dialogue;
			}
		}

		// Token: 0x0400957D RID: 38269
		public LocalisedString Dialogue;
	}
}
