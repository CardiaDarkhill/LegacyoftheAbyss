using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000638 RID: 1592
public class NPCEventResponder : MonoBehaviour
{
	// Token: 0x0600390F RID: 14607 RVA: 0x000FB6F6 File Offset: 0x000F98F6
	private bool? ValidateFsmEvent(string value)
	{
		return this.fsmTarget.IsEventValid(value, false);
	}

	// Token: 0x06003910 RID: 14608 RVA: 0x000FB708 File Offset: 0x000F9908
	private void Awake()
	{
		if (!this.control)
		{
			return;
		}
		if (this.fsmTarget)
		{
			if (!string.IsNullOrEmpty(this.convoStartEvent))
			{
				this.control.StartedDialogue += delegate()
				{
					this.fsmTarget.SendEvent(this.convoStartEvent);
				};
			}
			if (!string.IsNullOrEmpty(this.convoEndEvent))
			{
				this.control.EndingDialogue += delegate()
				{
					this.fsmTarget.SendEvent(this.convoEndEvent);
				};
			}
			if (!string.IsNullOrEmpty(this.newLinePlayerEvent))
			{
				this.control.StartedNewLine += delegate(DialogueBox.DialogueLine line)
				{
					if (line.IsPlayer)
					{
						this.fsmTarget.SendEvent(this.newLinePlayerEvent);
					}
				};
			}
			if (!string.IsNullOrEmpty(this.newLineEvent))
			{
				this.control.StartedNewLine += delegate(DialogueBox.DialogueLine _)
				{
					this.fsmTarget.SendEvent(this.newLineEvent);
				};
			}
		}
		this.control.StartedNewLine += delegate(DialogueBox.DialogueLine line)
		{
			if (line.IsPlayer)
			{
				return;
			}
			if (this.fsmTarget && !string.IsNullOrEmpty(this.newLineNPCEvent))
			{
				this.fsmTarget.SendEvent(this.newLineNPCEvent);
			}
			this.OnNewLineNpc.Invoke();
		};
	}

	// Token: 0x04003BE6 RID: 15334
	[SerializeField]
	private NPCControlBase control;

	// Token: 0x04003BE7 RID: 15335
	[Space]
	[SerializeField]
	private PlayMakerFSM fsmTarget;

	// Token: 0x04003BE8 RID: 15336
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string convoStartEvent;

	// Token: 0x04003BE9 RID: 15337
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string convoEndEvent;

	// Token: 0x04003BEA RID: 15338
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string newLineEvent;

	// Token: 0x04003BEB RID: 15339
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string newLineNPCEvent;

	// Token: 0x04003BEC RID: 15340
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmEvent")]
	[Conditional("fsmTarget", true, false, false)]
	private string newLinePlayerEvent;

	// Token: 0x04003BED RID: 15341
	[Space]
	public UnityEvent OnNewLineNpc;
}
