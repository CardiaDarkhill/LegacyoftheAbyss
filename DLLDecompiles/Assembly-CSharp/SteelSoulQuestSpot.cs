using System;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020007D4 RID: 2004
public class SteelSoulQuestSpot : PlayMakerNPC
{
	// Token: 0x06004690 RID: 18064 RVA: 0x00131C8C File Offset: 0x0012FE8C
	protected override void OnValidate()
	{
		base.OnValidate();
		LocalisedString[] array = this.orderedDialogues;
		if (array == null || array.Length != 3)
		{
			LocalisedString[] array2 = this.orderedDialogues;
			this.orderedDialogues = new LocalisedString[3];
			if (array2 != null)
			{
				for (int i = 0; i < Mathf.Min(3, array2.Length); i++)
				{
					this.orderedDialogues[i] = array2[i];
				}
			}
		}
	}

	// Token: 0x06004691 RID: 18065 RVA: 0x00131CEC File Offset: 0x0012FEEC
	protected override void Awake()
	{
		base.Awake();
		this.presentChild.SetActive(false);
		this.notPresentChild.SetActive(false);
		this.inspectPlink.SetActive(false);
		base.Deactivate(false);
	}

	// Token: 0x06004692 RID: 18066 RVA: 0x00131D20 File Offset: 0x0012FF20
	protected override void Start()
	{
		base.Start();
		if (!this.quest.IsAccepted && !this.quest.IsCompleted)
		{
			this.SetPresent(false);
			return;
		}
		GameManager instance = GameManager.instance;
		PlayerData playerData = instance.playerData;
		string sceneNameString = instance.GetSceneNameString();
		foreach (SteelSoulQuestSpot.Spot spot in playerData.SteelQuestSpots)
		{
			if (!(spot.SceneName != sceneNameString))
			{
				this.thisSpot = spot;
				break;
			}
		}
		if (this.thisSpot == null)
		{
			this.SetPresent(false);
			return;
		}
		this.SetPresent(true);
		if (!this.thisSpot.IsSeen)
		{
			this.inspectPlink.SetActive(true);
			base.Activate();
		}
	}

	// Token: 0x06004693 RID: 18067 RVA: 0x00131DD0 File Offset: 0x0012FFD0
	private void SetPresent(bool value)
	{
		this.presentChild.SetActive(value);
		this.notPresentChild.SetActive(!value);
	}

	// Token: 0x06004694 RID: 18068 RVA: 0x00131DF0 File Offset: 0x0012FFF0
	public string GetCurrentDialogue()
	{
		PlayerData instance = PlayerData.instance;
		int num = 0;
		SteelSoulQuestSpot.Spot[] steelQuestSpots = instance.SteelQuestSpots;
		for (int i = 0; i < steelQuestSpots.Length; i++)
		{
			if (steelQuestSpots[i].IsSeen)
			{
				num++;
			}
		}
		if (num >= this.orderedDialogues.Length)
		{
			num = this.orderedDialogues.Length - 1;
		}
		return this.orderedDialogues[num];
	}

	// Token: 0x06004695 RID: 18069 RVA: 0x00131E4D File Offset: 0x0013004D
	public void MarkSeen()
	{
		this.thisSpot.IsSeen = true;
		this.inspectPlink.SetActive(false);
		base.Deactivate(false);
		QuestManager.ShowQuestUpdatedStandalone(this.quest);
	}

	// Token: 0x040046F7 RID: 18167
	public const int SPOT_COUNT = 3;

	// Token: 0x040046F8 RID: 18168
	[Space]
	[SerializeField]
	private FullQuestBase quest;

	// Token: 0x040046F9 RID: 18169
	[SerializeField]
	private GameObject presentChild;

	// Token: 0x040046FA RID: 18170
	[SerializeField]
	private GameObject notPresentChild;

	// Token: 0x040046FB RID: 18171
	[SerializeField]
	private GameObject inspectPlink;

	// Token: 0x040046FC RID: 18172
	[Space]
	[SerializeField]
	private LocalisedString[] orderedDialogues;

	// Token: 0x040046FD RID: 18173
	private SteelSoulQuestSpot.Spot thisSpot;

	// Token: 0x040046FE RID: 18174
	private Coroutine pickupRoutine;

	// Token: 0x02001AA9 RID: 6825
	[Serializable]
	public class Spot
	{
		// Token: 0x04009A1F RID: 39455
		public string SceneName;

		// Token: 0x04009A20 RID: 39456
		public bool IsSeen;
	}
}
