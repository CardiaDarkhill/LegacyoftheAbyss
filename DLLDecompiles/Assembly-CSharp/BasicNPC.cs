using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200062B RID: 1579
public class BasicNPC : BasicNPCBase
{
	// Token: 0x17000668 RID: 1640
	// (get) Token: 0x06003835 RID: 14389 RVA: 0x000F8491 File Offset: 0x000F6691
	public List<SavedItem> GiveOnFirstTalk
	{
		get
		{
			return this.giveOnFirstTalkItems;
		}
	}

	// Token: 0x17000669 RID: 1641
	// (get) Token: 0x06003836 RID: 14390 RVA: 0x000F8499 File Offset: 0x000F6699
	public bool HasFinishedTalks
	{
		get
		{
			return this.talkState >= this.talkText.Length;
		}
	}

	// Token: 0x1700066A RID: 1642
	// (get) Token: 0x06003837 RID: 14391 RVA: 0x000F84AE File Offset: 0x000F66AE
	public bool HasRepeated
	{
		get
		{
			if (this.repeatText.IsEmpty)
			{
				return this.HasFinishedTalks;
			}
			return this.talkState >= this.talkText.Length + 1;
		}
	}

	// Token: 0x06003838 RID: 14392 RVA: 0x000F84DC File Offset: 0x000F66DC
	protected override void Awake()
	{
		this.Upgrade();
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
				this.startedAlreadySpoken = this.HasFinishedTalks;
			};
		}
	}

	// Token: 0x06003839 RID: 14393 RVA: 0x000F8530 File Offset: 0x000F6730
	protected override void OnValidate()
	{
		base.OnValidate();
		this.Upgrade();
	}

	// Token: 0x0600383A RID: 14394 RVA: 0x000F8540 File Offset: 0x000F6740
	private void Upgrade()
	{
		if (this.giveOnFirstTalk != null)
		{
			this.giveOnFirstTalkItems.RemoveAll((SavedItem o) => o == null);
			if (!this.giveOnFirstTalkItems.Contains(this.giveOnFirstTalk))
			{
				this.giveOnFirstTalkItems.Add(this.giveOnFirstTalk);
			}
			this.giveOnFirstTalk = null;
		}
	}

	// Token: 0x0600383B RID: 14395 RVA: 0x000F85B1 File Offset: 0x000F67B1
	protected override void OnStartDialogue()
	{
		base.OnStartDialogue();
		if (this.activateOnInteract)
		{
			this.activateOnInteract.SetActive(true);
		}
	}

	// Token: 0x0600383C RID: 14396 RVA: 0x000F85D4 File Offset: 0x000F67D4
	protected override void OnEndDialogue()
	{
		base.OnEndDialogue();
		if (this.talkState == 0 && this.giveOnFirstTalkItems.Count > 0)
		{
			this.giveOnFirstTalkItems.RemoveAll((SavedItem o) => o == null);
			foreach (SavedItem savedItem in this.giveOnFirstTalkItems)
			{
				savedItem.Get(true);
			}
		}
		this.talkState = this.endTalkState;
		if (this.activateOnInteract)
		{
			this.activateOnInteract.SetActive(false);
		}
		UnityEvent onEnd = this.OnEnd;
		if (onEnd == null)
		{
			return;
		}
		onEnd.Invoke();
	}

	// Token: 0x0600383D RID: 14397 RVA: 0x000F86A4 File Offset: 0x000F68A4
	protected override LocalisedString GetDialogue()
	{
		if (this.startedAlreadySpoken && !this.returnText.IsEmpty)
		{
			return this.returnText;
		}
		if (this.HasFinishedTalks && !this.repeatText.IsEmpty)
		{
			this.endTalkState = this.talkText.Length + 1;
			return this.repeatText;
		}
		if (this.talkText.Length == 0)
		{
			return default(LocalisedString);
		}
		this.talkState = Mathf.Clamp(this.talkState, 0, this.talkText.Length - 1);
		LocalisedString result = this.talkText[this.talkState];
		this.endTalkState = this.talkState + 1;
		return result;
	}

	// Token: 0x04003B2A RID: 15146
	[Space]
	[SerializeField]
	private GameObject activateOnInteract;

	// Token: 0x04003B2B RID: 15147
	[SerializeField]
	private PersistentIntItem stateTracker;

	// Token: 0x04003B2C RID: 15148
	[SerializeField]
	private LocalisedString[] talkText;

	// Token: 0x04003B2D RID: 15149
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString repeatText;

	// Token: 0x04003B2E RID: 15150
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString returnText;

	// Token: 0x04003B2F RID: 15151
	[SerializeField]
	[Obsolete("Use giveOnFirstTalkItems instead.")]
	[HideInInspector]
	private SavedItem giveOnFirstTalk;

	// Token: 0x04003B30 RID: 15152
	[Space]
	[SerializeField]
	private List<SavedItem> giveOnFirstTalkItems = new List<SavedItem>();

	// Token: 0x04003B31 RID: 15153
	[Space]
	public UnityEvent OnEnd;

	// Token: 0x04003B32 RID: 15154
	private int talkState;

	// Token: 0x04003B33 RID: 15155
	private int endTalkState;

	// Token: 0x04003B34 RID: 15156
	private bool startedAlreadySpoken;
}
