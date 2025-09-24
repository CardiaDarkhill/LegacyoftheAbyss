using System;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class MateriumItemManager : ManagerSingleton<MateriumItemManager>
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x060011F1 RID: 4593 RVA: 0x00053B4C File Offset: 0x00051D4C
	public MateriumItemList MasterList
	{
		get
		{
			return this.masterList;
		}
	}

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x060011F2 RID: 4594 RVA: 0x00053B54 File Offset: 0x00051D54
	public static bool ConstructedMaterium
	{
		get
		{
			return PlayerData.instance.ConstructedMaterium;
		}
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x00053B60 File Offset: 0x00051D60
	private void Start()
	{
		if (this.updateMessagePrefab)
		{
			this.updateMessage = Object.Instantiate<GameObject>(this.updateMessagePrefab, base.transform, true);
			this.updateMessage.SetActive(false);
		}
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x00053B93 File Offset: 0x00051D93
	public static void CheckAchievements()
	{
		MateriumItemManager.CheckAchievements(false);
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x00053B9C File Offset: 0x00051D9C
	public static void CheckAchievements(bool queue)
	{
		if (!PlayerData.instance.ConstructedMaterium)
		{
			return;
		}
		if (!ManagerSingleton<MateriumItemManager>.Instance)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		foreach (MateriumItem materiumItem in ManagerSingleton<MateriumItemManager>.Instance.masterList)
		{
			if (materiumItem.IsRequiredForCompletion)
			{
				num2++;
				if (materiumItem.IsCollected)
				{
					num++;
				}
			}
		}
		if (num >= num2)
		{
			if (queue)
			{
				GameManager.instance.QueueAchievement("MATERIUM_FULL");
				return;
			}
			GameManager.instance.AwardAchievement("MATERIUM_FULL");
		}
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x00053C44 File Offset: 0x00051E44
	public static void ShowUpdateMessage()
	{
		MateriumItemManager instance = ManagerSingleton<MateriumItemManager>.Instance;
		if (!instance.updateMessage)
		{
			return;
		}
		if (instance.updateMessage.activeSelf)
		{
			instance.updateMessage.SetActive(false);
		}
		instance.updateMessage.SetActive(true);
	}

	// Token: 0x040010CA RID: 4298
	[SerializeField]
	private MateriumItemList masterList;

	// Token: 0x040010CB RID: 4299
	[SerializeField]
	private GameObject updateMessagePrefab;

	// Token: 0x040010CC RID: 4300
	private GameObject updateMessage;
}
