using System;
using System.Reflection;
using UnityEngine;

// Token: 0x02000378 RID: 888
public class BossDoorCompletionStates : MonoBehaviour
{
	// Token: 0x06001E5C RID: 7772 RVA: 0x0008BD54 File Offset: 0x00089F54
	private void Start()
	{
		this.completedIndex = 0;
		foreach (FieldInfo fieldInfo in typeof(PlayerData).GetFields())
		{
			if (fieldInfo.FieldType == typeof(BossSequenceDoor.Completion) && ((BossSequenceDoor.Completion)fieldInfo.GetValue(GameManager.instance.playerData)).completed)
			{
				this.completedIndex++;
			}
		}
		if (!string.IsNullOrEmpty(this.stateSeenPlayerData))
		{
			int num = GameManager.instance.GetPlayerDataInt(this.stateSeenPlayerData) + 1;
			if (this.completedIndex > num)
			{
				this.completedIndex = num;
			}
		}
		if (this.completedIndex >= this.completionStates.Length)
		{
			this.completedIndex = this.completionStates.Length - 1;
		}
		for (int j = 0; j < this.completionStates.Length; j++)
		{
			if (this.completionStates[j].stateObject)
			{
				this.completionStates[j].stateObject.SetActive(false);
			}
		}
		BossDoorCompletionStates.CompletionState completionState = this.completionStates[this.completedIndex];
		if (completionState.stateObject)
		{
			completionState.stateObject.SetActive(true);
			if (!string.IsNullOrEmpty(completionState.sendEvent))
			{
				FSMUtility.SendEventToGameObject(completionState.stateObject, completionState.sendEvent, false);
			}
		}
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x0008BEA4 File Offset: 0x0008A0A4
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!string.IsNullOrEmpty(this.stateSeenPlayerData))
		{
			GameManager.instance.SetPlayerDataInt(this.stateSeenPlayerData, this.completedIndex);
		}
	}

	// Token: 0x04001D5D RID: 7517
	public BossDoorCompletionStates.CompletionState[] completionStates;

	// Token: 0x04001D5E RID: 7518
	[Space]
	[Tooltip("OPTIONAL - using an int, will ensure each state is seen at least once. (Requires a 2D trigger on this GameObject)")]
	public string stateSeenPlayerData;

	// Token: 0x04001D5F RID: 7519
	private int completedIndex;

	// Token: 0x02001624 RID: 5668
	[Serializable]
	public class CompletionState
	{
		// Token: 0x040089CF RID: 35279
		public GameObject stateObject;

		// Token: 0x040089D0 RID: 35280
		public string sendEvent;
	}
}
