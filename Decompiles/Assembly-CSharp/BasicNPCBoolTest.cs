using System;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200062D RID: 1581
public class BasicNPCBoolTest : BasicNPCBase
{
	// Token: 0x1700066B RID: 1643
	// (get) Token: 0x06003849 RID: 14409 RVA: 0x000F8856 File Offset: 0x000F6A56
	private bool HasFinishedAllTalks
	{
		get
		{
			return this.GetNextTalk(false) == null;
		}
	}

	// Token: 0x0600384A RID: 14410 RVA: 0x000F8864 File Offset: 0x000F6A64
	protected override void Awake()
	{
		base.Awake();
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
				this.startedAlreadySpoken = this.HasFinishedAllTalks;
			};
		}
	}

	// Token: 0x0600384B RID: 14411 RVA: 0x000F88B2 File Offset: 0x000F6AB2
	protected override void OnStartDialogue()
	{
		base.OnStartDialogue();
		if (this.activateOnInteract)
		{
			this.activateOnInteract.SetActive(true);
		}
	}

	// Token: 0x0600384C RID: 14412 RVA: 0x000F88D3 File Offset: 0x000F6AD3
	protected override void OnEndDialogue()
	{
		base.OnEndDialogue();
		this.talkState = this.endTalkState;
		if (this.activateOnInteract)
		{
			this.activateOnInteract.SetActive(false);
		}
	}

	// Token: 0x0600384D RID: 14413 RVA: 0x000F8900 File Offset: 0x000F6B00
	protected override LocalisedString GetDialogue()
	{
		if (this.startedAlreadySpoken && !this.returnText.IsEmpty)
		{
			return this.returnText;
		}
		if (this.HasFinishedAllTalks && !this.repeatText.IsEmpty)
		{
			return this.repeatText;
		}
		if (this.talks.Length == 0)
		{
			return default(LocalisedString);
		}
		BasicNPCBoolTest.ConditionalTalk nextTalk = this.GetNextTalk(true, out this.endTalkState);
		if (nextTalk != null)
		{
			return nextTalk.Text;
		}
		return default(LocalisedString);
	}

	// Token: 0x0600384E RID: 14414 RVA: 0x000F897C File Offset: 0x000F6B7C
	private BasicNPCBoolTest.ConditionalTalk GetNextTalk(bool canSettleOnLast)
	{
		int num;
		return this.GetNextTalk(canSettleOnLast, out num);
	}

	// Token: 0x0600384F RID: 14415 RVA: 0x000F8994 File Offset: 0x000F6B94
	private BasicNPCBoolTest.ConditionalTalk GetNextTalk(bool canSettleOnLast, out int newTalkMask)
	{
		BasicNPCBoolTest.ConditionalTalk result = null;
		for (int i = 0; i < this.talks.Length; i++)
		{
			BasicNPCBoolTest.ConditionalTalk conditionalTalk = this.talks[i];
			if (conditionalTalk.IsTestFulfilled)
			{
				int num = 1 << i;
				if ((this.talkState & num) != num)
				{
					newTalkMask = (this.talkState | num);
					return conditionalTalk;
				}
				if (canSettleOnLast)
				{
					result = conditionalTalk;
				}
			}
		}
		newTalkMask = this.talkState;
		return result;
	}

	// Token: 0x04003B3A RID: 15162
	[Space]
	[SerializeField]
	private GameObject activateOnInteract;

	// Token: 0x04003B3B RID: 15163
	[SerializeField]
	private PersistentIntItem stateTracker;

	// Token: 0x04003B3C RID: 15164
	[SerializeField]
	private BasicNPCBoolTest.ConditionalTalk[] talks;

	// Token: 0x04003B3D RID: 15165
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString repeatText;

	// Token: 0x04003B3E RID: 15166
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString returnText;

	// Token: 0x04003B3F RID: 15167
	protected int talkState;

	// Token: 0x04003B40 RID: 15168
	protected int endTalkState;

	// Token: 0x04003B41 RID: 15169
	protected bool startedAlreadySpoken;

	// Token: 0x0200193D RID: 6461
	[Serializable]
	private class ConditionalTalk
	{
		// Token: 0x17001079 RID: 4217
		// (get) Token: 0x060093A1 RID: 37793 RVA: 0x0029EA2F File Offset: 0x0029CC2F
		public bool IsTestFulfilled
		{
			get
			{
				return string.IsNullOrEmpty(this.BoolTest) || PlayerData.instance.GetVariable(this.BoolTest) == this.ExpectedBoolValue;
			}
		}

		// Token: 0x040094EC RID: 38124
		[PlayerDataField(typeof(bool), false)]
		public string BoolTest;

		// Token: 0x040094ED RID: 38125
		public bool ExpectedBoolValue;

		// Token: 0x040094EE RID: 38126
		public LocalisedString Text;
	}
}
