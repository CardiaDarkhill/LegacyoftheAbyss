using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000631 RID: 1585
public class InspectDoor : InteractableBase
{
	// Token: 0x06003884 RID: 14468 RVA: 0x000F99BC File Offset: 0x000F7BBC
	protected override void Awake()
	{
		base.Awake();
		this.door.Deactivate(false);
		if (this.promptPersistent)
		{
			this.promptPersistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.hasEnteredBefore;
			};
			this.promptPersistent.OnSetSaveState += delegate(bool value)
			{
				this.hasEnteredBefore = value;
			};
		}
	}

	// Token: 0x06003885 RID: 14469 RVA: 0x000F9A18 File Offset: 0x000F7C18
	public override void Interact()
	{
		base.DisableInteraction();
		if (this.hasEnteredBefore && this.promptPersistent)
		{
			this.door.Interact();
			return;
		}
		DialogueYesNoBox.Open(delegate()
		{
			this.hasEnteredBefore = true;
			this.door.Interact();
		}, new Action(base.EnableInteraction), true, this.promptText, null);
	}

	// Token: 0x04003B74 RID: 15220
	[Space]
	[SerializeField]
	private LocalisedString promptText;

	// Token: 0x04003B75 RID: 15221
	[Space]
	[SerializeField]
	private InteractableBase door;

	// Token: 0x04003B76 RID: 15222
	[SerializeField]
	private PersistentBoolItem promptPersistent;

	// Token: 0x04003B77 RID: 15223
	private bool hasEnteredBefore;
}
