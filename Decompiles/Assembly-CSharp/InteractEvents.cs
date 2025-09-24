using System;
using JetBrains.Annotations;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;

// Token: 0x020003CE RID: 974
public class InteractEvents : NPCControlBase
{
	// Token: 0x14000069 RID: 105
	// (add) Token: 0x06002142 RID: 8514 RVA: 0x00099D1C File Offset: 0x00097F1C
	// (remove) Token: 0x06002143 RID: 8515 RVA: 0x00099D54 File Offset: 0x00097F54
	public event Action Interacted;

	// Token: 0x17000371 RID: 881
	// (get) Token: 0x06002144 RID: 8516 RVA: 0x00099D89 File Offset: 0x00097F89
	public override bool AutoEnd
	{
		get
		{
			return !this.showingYesNo;
		}
	}

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x06002145 RID: 8517 RVA: 0x00099D94 File Offset: 0x00097F94
	protected override bool AllowMovePlayer
	{
		get
		{
			return this.doMovePlayer;
		}
	}

	// Token: 0x06002146 RID: 8518 RVA: 0x00099D9C File Offset: 0x00097F9C
	[UsedImplicitly]
	private bool? IsFsmEventValid(string eventName)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			return null;
		}
		if (!this.targetFSM)
		{
			return new bool?(false);
		}
		return this.targetFSM.IsEventValid(eventName, true);
	}

	// Token: 0x06002147 RID: 8519 RVA: 0x00099DDC File Offset: 0x00097FDC
	[UsedImplicitly]
	private bool? IsFsmEventValidOptional(string eventName)
	{
		return this.targetFSM.IsEventValid(eventName, false);
	}

	// Token: 0x06002148 RID: 8520 RVA: 0x00099DEC File Offset: 0x00097FEC
	protected override void OnStartDialogue()
	{
		this.showingYesNo = false;
		this.isInteracting = true;
		if (!InteractableBase.TrySendStateChangeEvent(this.targetFSM, this.interactEvent, true) && this.Interacted == null)
		{
			this.isInteracting = false;
			return;
		}
		if (!this.isInteracting)
		{
			return;
		}
		base.DisableInteraction();
		if (this.inspectText.IsEmpty)
		{
			this.ShowYesNo();
			return;
		}
		DialogueBox.StartConversation(this.inspectText, this, false, new DialogueBox.DisplayOptions
		{
			Alignment = TextAlignmentOptions.Top,
			ShowDecorators = true,
			TextColor = Color.white
		}, new Action(this.ShowYesNo));
	}

	// Token: 0x06002149 RID: 8521 RVA: 0x00099E8C File Offset: 0x0009808C
	private void ShowYesNo()
	{
		if (!this.yesNoPrompt.IsEmpty)
		{
			this.showingYesNo = true;
			DialogueYesNoBox.Open(delegate()
			{
				Action interacted2 = this.Interacted;
				if (interacted2 != null)
				{
					interacted2();
				}
				this.showingYesNo = false;
			}, new Action(this.EndInteraction), true, this.yesNoPrompt, null);
			return;
		}
		Action interacted = this.Interacted;
		if (interacted == null)
		{
			return;
		}
		interacted();
	}

	// Token: 0x0600214A RID: 8522 RVA: 0x00099EE8 File Offset: 0x000980E8
	protected override void OnEndDialogue()
	{
		this.isInteracting = false;
		if (this.showingYesNo)
		{
			return;
		}
		base.EnableInteraction();
	}

	// Token: 0x0600214B RID: 8523 RVA: 0x00099F00 File Offset: 0x00098100
	public void EndInteraction()
	{
		this.showingYesNo = false;
		base.EndDialogue();
	}

	// Token: 0x0600214C RID: 8524 RVA: 0x00099F0F File Offset: 0x0009810F
	public override void CanInteract()
	{
		if (!string.IsNullOrEmpty(this.canInteractEvent))
		{
			this.targetFSM.SendEventSafe(this.canInteractEvent);
		}
	}

	// Token: 0x0600214D RID: 8525 RVA: 0x00099F2F File Offset: 0x0009812F
	public override void CanNotInteract()
	{
		if (!string.IsNullOrEmpty(this.canNotInteractEvent))
		{
			this.targetFSM.SendEventSafe(this.canNotInteractEvent);
		}
	}

	// Token: 0x0400202F RID: 8239
	[SerializeField]
	private bool doMovePlayer;

	// Token: 0x04002030 RID: 8240
	[Space]
	[SerializeField]
	private PlayMakerFSM targetFSM;

	// Token: 0x04002031 RID: 8241
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValid")]
	private string interactEvent = "INTERACT";

	// Token: 0x04002032 RID: 8242
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidOptional")]
	private string canInteractEvent;

	// Token: 0x04002033 RID: 8243
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmEventValidOptional")]
	private string canNotInteractEvent;

	// Token: 0x04002034 RID: 8244
	[Space]
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString inspectText;

	// Token: 0x04002035 RID: 8245
	[SerializeField]
	[LocalisedString.NotRequiredAttribute]
	private LocalisedString yesNoPrompt;

	// Token: 0x04002036 RID: 8246
	private bool showingYesNo;

	// Token: 0x04002037 RID: 8247
	private bool isInteracting;
}
