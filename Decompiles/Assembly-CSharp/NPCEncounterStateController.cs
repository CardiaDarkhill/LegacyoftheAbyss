using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000447 RID: 1095
public sealed class NPCEncounterStateController : MonoBehaviour
{
	// Token: 0x06002682 RID: 9858 RVA: 0x000AE594 File Offset: 0x000AC794
	private void Awake()
	{
		this.localState = this.GetState();
		if (this.localState == NPCEncounterState.HasLeft)
		{
			this.Leave();
			return;
		}
		if (this.localState == NPCEncounterState.AuthorisedToLeave && this.leaveProbability >= Random.Range(0f, 1f))
		{
			this.SetState(NPCEncounterState.HasLeft);
			this.Leave();
			return;
		}
		this.Here();
	}

	// Token: 0x06002683 RID: 9859 RVA: 0x000AE5F1 File Offset: 0x000AC7F1
	private NPCEncounterState GetState()
	{
		return PlayerData.instance.GetVariable(this.encounterStateName);
	}

	// Token: 0x06002684 RID: 9860 RVA: 0x000AE603 File Offset: 0x000AC803
	public NPCEncounterState GetCurrentState()
	{
		this.localState = this.GetState();
		return this.localState;
	}

	// Token: 0x06002685 RID: 9861 RVA: 0x000AE617 File Offset: 0x000AC817
	public void UpdateState()
	{
		this.localState = this.GetState();
	}

	// Token: 0x06002686 RID: 9862 RVA: 0x000AE625 File Offset: 0x000AC825
	private void SetPlayerDataValue(NPCEncounterState state)
	{
		this.localState = state;
		PlayerData.instance.SetVariable(this.encounterStateName, state);
	}

	// Token: 0x06002687 RID: 9863 RVA: 0x000AE63F File Offset: 0x000AC83F
	public void SetState(NPCEncounterState newState)
	{
		if (this.localState >= newState)
		{
			return;
		}
		if (newState == NPCEncounterState.HasLeft && this.requireLeaveAuthorisation && this.localState != NPCEncounterState.AuthorisedToLeave)
		{
			this.SetPlayerDataValue(NPCEncounterState.ReadyToLeave);
			return;
		}
		this.SetPlayerDataValue(newState);
	}

	// Token: 0x06002688 RID: 9864 RVA: 0x000AE66F File Offset: 0x000AC86F
	public void SetMet()
	{
		this.SetState(NPCEncounterState.Met);
	}

	// Token: 0x06002689 RID: 9865 RVA: 0x000AE678 File Offset: 0x000AC878
	public void SetReadyToLeave()
	{
		this.SetState(NPCEncounterState.ReadyToLeave);
	}

	// Token: 0x0600268A RID: 9866 RVA: 0x000AE681 File Offset: 0x000AC881
	public void SetAuthorisedToLeave()
	{
		this.SetState(NPCEncounterState.AuthorisedToLeave);
	}

	// Token: 0x0600268B RID: 9867 RVA: 0x000AE68A File Offset: 0x000AC88A
	public void SetHasLeft()
	{
		this.SetState(NPCEncounterState.HasLeft);
	}

	// Token: 0x0600268C RID: 9868 RVA: 0x000AE694 File Offset: 0x000AC894
	private void Here()
	{
		foreach (NPCEncounterStateController.ObjectState objectState in this.presentStates)
		{
			objectState.Apply();
		}
	}

	// Token: 0x0600268D RID: 9869 RVA: 0x000AE6E8 File Offset: 0x000AC8E8
	private void Leave()
	{
		foreach (NPCEncounterStateController.ObjectState objectState in this.awayStates)
		{
			objectState.Apply();
		}
		if (this.disableSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040023DE RID: 9182
	[SerializeField]
	[PlayerDataField(typeof(NPCEncounterState), true)]
	private string encounterStateName;

	// Token: 0x040023DF RID: 9183
	private NPCEncounterState localState;

	// Token: 0x040023E0 RID: 9184
	[SerializeField]
	private bool requireLeaveAuthorisation;

	// Token: 0x040023E1 RID: 9185
	[Range(0f, 1f)]
	[SerializeField]
	private float leaveProbability = 1f;

	// Token: 0x040023E2 RID: 9186
	[SerializeField]
	private bool disableSelf = true;

	// Token: 0x040023E3 RID: 9187
	[Tooltip("Applied when npc present")]
	[SerializeField]
	private List<NPCEncounterStateController.ObjectState> presentStates = new List<NPCEncounterStateController.ObjectState>();

	// Token: 0x040023E4 RID: 9188
	[Tooltip("Applied when npc away")]
	[SerializeField]
	private List<NPCEncounterStateController.ObjectState> awayStates = new List<NPCEncounterStateController.ObjectState>();

	// Token: 0x02001730 RID: 5936
	[Serializable]
	private struct ObjectState
	{
		// Token: 0x06008D03 RID: 36099 RVA: 0x00288B8D File Offset: 0x00286D8D
		public void Apply()
		{
			if (this.gameObject)
			{
				this.gameObject.SetActive(this.isActive);
			}
		}

		// Token: 0x04008D95 RID: 36245
		public GameObject gameObject;

		// Token: 0x04008D96 RID: 36246
		public bool isActive;
	}
}
