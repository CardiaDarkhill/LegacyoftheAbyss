using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000713 RID: 1811
public sealed class SaveSlotCompletionIcons : MonoBehaviour
{
	// Token: 0x0600408F RID: 16527 RVA: 0x0011BF86 File Offset: 0x0011A186
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.completionIcons.RemoveAll((SaveSlotCompletionIcons.CompletionIcon o) => o.icon == null);
		return true;
	}

	// Token: 0x06004090 RID: 16528 RVA: 0x0011BFC5 File Offset: 0x0011A1C5
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x06004091 RID: 16529 RVA: 0x0011BFE0 File Offset: 0x0011A1E0
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06004092 RID: 16530 RVA: 0x0011BFEC File Offset: 0x0011A1EC
	public void SetCompletionIconState(SaveStats SaveStats)
	{
		if (SaveStats == null)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.OnAwake();
		SaveSlotCompletionIcons.CompletionState completionState = SaveStats.LastCompletedEnding;
		if (CheatManager.IsCheatsEnabled && CheatManager.ShowAllCompletionIcons)
		{
			completionState = SaveStats.CompletedEndings;
		}
		if (completionState == SaveSlotCompletionIcons.CompletionState.None)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		foreach (SaveSlotCompletionIcons.CompletionIcon completionIcon in this.completionIcons)
		{
			completionIcon.icon.gameObject.SetActive(completionIcon.state != SaveSlotCompletionIcons.CompletionState.None && completionState.HasFlag(completionIcon.state));
		}
	}

	// Token: 0x04004224 RID: 16932
	[SerializeField]
	private List<SaveSlotCompletionIcons.CompletionIcon> completionIcons = new List<SaveSlotCompletionIcons.CompletionIcon>();

	// Token: 0x04004225 RID: 16933
	private bool hasAwaken;

	// Token: 0x04004226 RID: 16934
	private bool hasStarted;

	// Token: 0x020019FF RID: 6655
	[Serializable]
	private struct CompletionIcon
	{
		// Token: 0x0400980E RID: 38926
		public SaveSlotCompletionIcons.CompletionState state;

		// Token: 0x0400980F RID: 38927
		public GameObject icon;
	}

	// Token: 0x02001A00 RID: 6656
	[Flags]
	[Serializable]
	public enum CompletionState
	{
		// Token: 0x04009811 RID: 38929
		None = 0,
		// Token: 0x04009812 RID: 38930
		Act2Regular = 1,
		// Token: 0x04009813 RID: 38931
		Act2Cursed = 2,
		// Token: 0x04009814 RID: 38932
		Act2SoulSnare = 4,
		// Token: 0x04009815 RID: 38933
		Act3Ending = 8
	}
}
