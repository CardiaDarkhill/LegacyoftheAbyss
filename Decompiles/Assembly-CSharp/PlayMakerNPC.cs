using System;
using UnityEngine;

// Token: 0x0200063A RID: 1594
public class PlayMakerNPC : NPCControlBase
{
	// Token: 0x1700067F RID: 1663
	// (get) Token: 0x06003927 RID: 14631 RVA: 0x000FBD89 File Offset: 0x000F9F89
	public override bool AutoEnd
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000680 RID: 1664
	// (get) Token: 0x06003928 RID: 14632 RVA: 0x000FBD8C File Offset: 0x000F9F8C
	protected override bool AutoCallEndAction
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000681 RID: 1665
	// (get) Token: 0x06003929 RID: 14633 RVA: 0x000FBD8F File Offset: 0x000F9F8F
	protected override bool AllowMovePlayer
	{
		get
		{
			return !this.IsTemporary;
		}
	}

	// Token: 0x17000682 RID: 1666
	// (get) Token: 0x0600392A RID: 14634 RVA: 0x000FBD9A File Offset: 0x000F9F9A
	// (set) Token: 0x0600392B RID: 14635 RVA: 0x000FBDA2 File Offset: 0x000F9FA2
	public bool IsRunningDialogue { get; private set; }

	// Token: 0x17000683 RID: 1667
	// (get) Token: 0x0600392C RID: 14636 RVA: 0x000FBDAB File Offset: 0x000F9FAB
	// (set) Token: 0x0600392D RID: 14637 RVA: 0x000FBDB3 File Offset: 0x000F9FB3
	public bool IsTemporary { get; private set; }

	// Token: 0x17000684 RID: 1668
	// (get) Token: 0x0600392E RID: 14638 RVA: 0x000FBDBC File Offset: 0x000F9FBC
	// (set) Token: 0x0600392F RID: 14639 RVA: 0x000FBDC4 File Offset: 0x000F9FC4
	public PlayMakerFSM CustomEventTarget { get; set; }

	// Token: 0x06003930 RID: 14640 RVA: 0x000FBDCD File Offset: 0x000F9FCD
	protected override void OnDisable()
	{
		base.OnDisable();
		this.RemoveTalkTableOverride();
	}

	// Token: 0x06003931 RID: 14641 RVA: 0x000FBDDB File Offset: 0x000F9FDB
	private bool? IsFsmEventValidRequired(string eventName)
	{
		return this.dialogueFsm.IsEventValid(eventName, true);
	}

	// Token: 0x06003932 RID: 14642 RVA: 0x000FBDEA File Offset: 0x000F9FEA
	private bool? IsFsmEventValidNotRequired(string eventName)
	{
		return this.dialogueFsm.IsEventValid(eventName, false);
	}

	// Token: 0x06003933 RID: 14643 RVA: 0x000FBDF9 File Offset: 0x000F9FF9
	public override void OnDialogueBoxEnded()
	{
		this.isFirstLine = true;
		this.SendEvent("LINE_END");
		this.SendEvent("CONVO_END");
	}

	// Token: 0x06003934 RID: 14644 RVA: 0x000FBE1C File Offset: 0x000FA01C
	public void CloseDialogueBox(bool returnControl, bool returnHud, Action onBoxHidden)
	{
		if (!this.IsRunningDialogue)
		{
			if (onBoxHidden != null)
			{
				onBoxHidden();
			}
			return;
		}
		Action action = null;
		if (returnControl)
		{
			base.RecordControlVersion();
			action = delegate()
			{
				if (onBoxHidden != null)
				{
					onBoxHidden();
				}
				this.IsRunningDialogue = false;
				this.EnableInteraction();
				this.CallEndAction();
				if (this.IsTemporary)
				{
					Object.Destroy(this);
				}
			};
		}
		DialogueBox.EndConversation(returnHud, action ?? onBoxHidden);
	}

	// Token: 0x06003935 RID: 14645 RVA: 0x000FBE82 File Offset: 0x000FA082
	protected override void OnNewLineStarted(DialogueBox.DialogueLine line)
	{
		if (this.isFirstLine)
		{
			this.isFirstLine = false;
		}
		else
		{
			this.SendEvent("LINE_END");
		}
		if (!string.IsNullOrEmpty(line.Event))
		{
			this.SendEvent(line.Event);
		}
	}

	// Token: 0x06003936 RID: 14646 RVA: 0x000FBEBB File Offset: 0x000FA0BB
	protected override void OnStartingDialogue()
	{
		if (!string.IsNullOrEmpty(this.preInteractEvent))
		{
			this.SendEvent(this.preInteractEvent);
		}
	}

	// Token: 0x06003937 RID: 14647 RVA: 0x000FBED8 File Offset: 0x000FA0D8
	protected override void OnStartDialogue()
	{
		base.DisableInteraction();
		this.IsRunningDialogue = true;
		this.isFirstLine = true;
		if (!string.IsNullOrEmpty(this.interactEvent) && this.SendEvent(this.interactEvent))
		{
			return;
		}
		if (this.wasAutoStarted)
		{
			this.wasAutoStarted = false;
			return;
		}
		if (this.IsTemporary)
		{
			return;
		}
		base.EnableInteraction();
		base.CallEndAction();
		this.IsRunningDialogue = false;
	}

	// Token: 0x06003938 RID: 14648 RVA: 0x000FBF44 File Offset: 0x000FA144
	private bool SendEvent(string eventName)
	{
		if (this.secondaryFsms != null)
		{
			PlayMakerFSM[] array = this.secondaryFsms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SendEventRecursive(eventName);
			}
		}
		PlayMakerFSM playMakerFSM = this.CustomEventTarget ? this.CustomEventTarget : this.dialogueFsm;
		if (!playMakerFSM.enabled)
		{
			playMakerFSM.enabled = true;
		}
		return playMakerFSM.SendEventRecursive(eventName);
	}

	// Token: 0x06003939 RID: 14649 RVA: 0x000FBFAA File Offset: 0x000FA1AA
	public void ForceEndDialogue()
	{
		base.CancelHeroMove();
		this.SendEvent("CONVO_END_FORCED");
		this.CloseDialogueBox(true, true, null);
	}

	// Token: 0x0600393A RID: 14650 RVA: 0x000FBFC7 File Offset: 0x000FA1C7
	public static PlayMakerNPC GetNewTemp(PlayMakerFSM eventTarget)
	{
		if (!eventTarget)
		{
			return null;
		}
		PlayMakerNPC playMakerNPC = eventTarget.gameObject.AddComponent<PlayMakerNPC>();
		playMakerNPC.IsTemporary = true;
		playMakerNPC.interactEvent = null;
		playMakerNPC.dialogueFsm = eventTarget;
		return playMakerNPC;
	}

	// Token: 0x0600393B RID: 14651 RVA: 0x000FBFF3 File Offset: 0x000FA1F3
	public void SetAutoStarting()
	{
		this.wasAutoStarted = true;
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x000FBFFC File Offset: 0x000FA1FC
	public void SetTalkTableOverride(RandomAudioClipTable table)
	{
		this.overridingTalkTable = true;
		this.id = HeroTalkAnimation.SetTalkTableOverride(table);
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x000FC011 File Offset: 0x000FA211
	public void RemoveTalkTableOverride()
	{
		if (this.overridingTalkTable)
		{
			HeroTalkAnimation.RemoveTalkTableOverride(this.id);
			this.overridingTalkTable = false;
		}
	}

	// Token: 0x04003BFC RID: 15356
	public const string DIALOGUE_END_EVENT = "CONVO_END";

	// Token: 0x04003BFD RID: 15357
	public const string LINE_END_EVENT = "LINE_END";

	// Token: 0x04003BFE RID: 15358
	public const string DIALOGUE_END_FORCED_EVENT = "CONVO_END_FORCED";

	// Token: 0x04003BFF RID: 15359
	[Space]
	[SerializeField]
	private PlayMakerFSM dialogueFsm;

	// Token: 0x04003C00 RID: 15360
	[SerializeField]
	private PlayMakerFSM[] secondaryFsms;

	// Token: 0x04003C01 RID: 15361
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidNotRequired")]
	private string preInteractEvent = string.Empty;

	// Token: 0x04003C02 RID: 15362
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidRequired")]
	private string interactEvent = "INTERACT";

	// Token: 0x04003C03 RID: 15363
	private bool isFirstLine;

	// Token: 0x04003C04 RID: 15364
	private bool wasAutoStarted;

	// Token: 0x04003C05 RID: 15365
	private bool overridingTalkTable;

	// Token: 0x04003C06 RID: 15366
	private int id;
}
