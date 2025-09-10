using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000141 RID: 321
public sealed class BasicNPCRepeatDialogueLeaveCondition : MonoBehaviour
{
	// Token: 0x060009F0 RID: 2544 RVA: 0x0002D040 File Offset: 0x0002B240
	private void Awake()
	{
		if (this.basicNpc == null)
		{
			this.basicNpc = base.GetComponent<BasicNPC>();
			if (this.basicNpc == null)
			{
				return;
			}
		}
		if (this.npcEcounter == null)
		{
			this.npcEcounter = base.GetComponent<NPCEncounterStateController>();
			if (this.npcEcounter == null)
			{
				return;
			}
		}
		this.basicNpc.OnEnd.AddListener(new UnityAction(this.OnDialogueEnd));
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x0002D0BB File Offset: 0x0002B2BB
	private void OnValidate()
	{
		if (this.basicNpc == null)
		{
			this.basicNpc = base.GetComponent<BasicNPC>();
		}
		if (this.npcEcounter == null)
		{
			this.npcEcounter = base.GetComponent<NPCEncounterStateController>();
		}
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x0002D0F1 File Offset: 0x0002B2F1
	private void OnDialogueEnd()
	{
		if (this.npcEcounter != null)
		{
			this.npcEcounter.GetCurrentState();
			this.npcEcounter.SetMet();
			if (this.basicNpc.HasRepeated)
			{
				this.npcEcounter.SetReadyToLeave();
			}
		}
	}

	// Token: 0x0400097E RID: 2430
	[SerializeField]
	private BasicNPC basicNpc;

	// Token: 0x0400097F RID: 2431
	[SerializeField]
	private NPCEncounterStateController npcEcounter;
}
