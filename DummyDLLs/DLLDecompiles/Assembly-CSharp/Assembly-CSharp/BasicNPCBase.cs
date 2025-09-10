using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x0200062C RID: 1580
public abstract class BasicNPCBase : NPCControlBase
{
	// Token: 0x06003841 RID: 14401 RVA: 0x000F8780 File Offset: 0x000F6980
	private bool? IsEventValid(string eventName)
	{
		return this.eventTarget.IsEventValid(eventName, false);
	}

	// Token: 0x06003842 RID: 14402 RVA: 0x000F878F File Offset: 0x000F698F
	protected override void Awake()
	{
		base.Awake();
		base.StartingDialogue += delegate()
		{
			this.SendDialogueEvent(this.dialogueStartingEvent);
		};
	}

	// Token: 0x06003843 RID: 14403 RVA: 0x000F87A9 File Offset: 0x000F69A9
	private void SendDialogueEvent(string eventName)
	{
		if (!this.eventTarget || string.IsNullOrEmpty(eventName))
		{
			return;
		}
		this.eventTarget.SendEvent(eventName);
	}

	// Token: 0x06003844 RID: 14404 RVA: 0x000F87D0 File Offset: 0x000F69D0
	protected override void OnStartDialogue()
	{
		base.DisableInteraction();
		LocalisedString dialogue = this.GetDialogue();
		if (!dialogue.IsEmpty)
		{
			DialogueBox.StartConversation(dialogue, this, false, this.displayOptions, null);
		}
		else
		{
			Debug.LogError("NPC Dialogue Text is empty! Canceling...", this);
			this.OnEndDialogue();
		}
		this.SendDialogueEvent(this.dialogueStartedEvent);
	}

	// Token: 0x06003845 RID: 14405 RVA: 0x000F8821 File Offset: 0x000F6A21
	protected override void OnEndDialogue()
	{
		base.EnableInteraction();
		this.SendDialogueEvent(this.dialogueEndedEvent);
	}

	// Token: 0x06003846 RID: 14406
	protected abstract LocalisedString GetDialogue();

	// Token: 0x04003B35 RID: 15157
	[Space]
	[SerializeField]
	private DialogueBox.DisplayOptions displayOptions = DialogueBox.DisplayOptions.Default;

	// Token: 0x04003B36 RID: 15158
	[Space]
	[SerializeField]
	private PlayMakerFSM eventTarget;

	// Token: 0x04003B37 RID: 15159
	[Tooltip("Sent as soon as NPC is interacted with.")]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("eventTarget", true, false, false)]
	[InspectorValidation("IsEventValid")]
	private string dialogueStartingEvent;

	// Token: 0x04003B38 RID: 15160
	[Tooltip("Sent once player has moved into position and dialogue has started.")]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("eventTarget", true, false, false)]
	[InspectorValidation("IsEventValid")]
	private string dialogueStartedEvent;

	// Token: 0x04003B39 RID: 15161
	[Tooltip("Sent after dialogue has ended.")]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("eventTarget", true, false, false)]
	[InspectorValidation("IsEventValid")]
	private string dialogueEndedEvent;
}
