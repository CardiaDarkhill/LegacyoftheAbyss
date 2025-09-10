using System;
using TMProOld;
using UnityEngine;

// Token: 0x0200061F RID: 1567
public class ControlReminderSingle : MonoBehaviour
{
	// Token: 0x060037C0 RID: 14272 RVA: 0x000F5E70 File Offset: 0x000F4070
	public void Activate(ControlReminder.SingleConfig config)
	{
		this.actionIcon.SetAction(ControlReminder.MapActionToAction(config.Button));
		this.actionPromptText.text = (config.Prompt.IsEmpty ? string.Empty : config.Prompt);
		this.actionText.text = config.Text;
		base.gameObject.SetActive(true);
	}

	// Token: 0x04003ACD RID: 15053
	[SerializeField]
	private ActionButtonIcon actionIcon;

	// Token: 0x04003ACE RID: 15054
	[SerializeField]
	private TMP_Text actionPromptText;

	// Token: 0x04003ACF RID: 15055
	[SerializeField]
	private TMP_Text actionText;
}
