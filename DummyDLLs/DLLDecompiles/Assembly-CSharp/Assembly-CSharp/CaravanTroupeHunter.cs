using System;
using System.Collections.Generic;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000177 RID: 375
public class CaravanTroupeHunter : SimpleShopMenuOwner
{
	// Token: 0x17000116 RID: 278
	// (get) Token: 0x06000BD5 RID: 3029 RVA: 0x00035BDD File Offset: 0x00033DDD
	public static IReadOnlyDictionary<CaravanTroupeHunter.PinGroups, string> PdBools { get; } = new Dictionary<CaravanTroupeHunter.PinGroups, string>
	{
		{
			CaravanTroupeHunter.PinGroups.Marrowlands,
			"hasPinFleaMarrowlands"
		},
		{
			CaravanTroupeHunter.PinGroups.Midlands,
			"hasPinFleaMidlands"
		},
		{
			CaravanTroupeHunter.PinGroups.Blastedlands,
			"hasPinFleaBlastedlands"
		},
		{
			CaravanTroupeHunter.PinGroups.Citadel,
			"hasPinFleaCitadel"
		},
		{
			CaravanTroupeHunter.PinGroups.Peaklands,
			"hasPinFleaPeaklands"
		},
		{
			CaravanTroupeHunter.PinGroups.Mucklands,
			"hasPinFleaMucklands"
		}
	};

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x06000BD6 RID: 3030 RVA: 0x00035BE4 File Offset: 0x00033DE4
	public override bool ClosePaneOnPurchase
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000BD7 RID: 3031 RVA: 0x00035BE7 File Offset: 0x00033DE7
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<CaravanTroupeHunter.ShopItemInfo>(ref this.shopItems, typeof(CaravanTroupeHunter.PinGroups));
	}

	// Token: 0x06000BD8 RID: 3032 RVA: 0x00035BFE File Offset: 0x00033DFE
	private void Awake()
	{
		this.OnValidate();
		base.ClosedPurchase += delegate()
		{
			GameManager.instance.UpdateGameMapWithPopup(0f);
		};
	}

	// Token: 0x06000BD9 RID: 3033 RVA: 0x00035C2C File Offset: 0x00033E2C
	protected override List<ISimpleShopItem> GetItems()
	{
		PlayerData instance = PlayerData.instance;
		if (this.currentList == null)
		{
			this.currentList = new List<ISimpleShopItem>();
		}
		else
		{
			this.currentList.Clear();
		}
		if (this.currentListGroups == null)
		{
			this.currentListGroups = new List<CaravanTroupeHunter.PinGroups>();
		}
		else
		{
			this.currentListGroups.Clear();
		}
		GameMap gameMap = GameManager.instance.gameMap;
		for (int i = 0; i < this.shopItems.Length; i++)
		{
			CaravanTroupeHunter.PinGroups pinGroups = (CaravanTroupeHunter.PinGroups)i;
			CaravanTroupeHunter.ShopItemInfo item = this.shopItems[i];
			if (!instance.GetVariable(CaravanTroupeHunter.PdBools[pinGroups]) && gameMap.HasRemainingPinFor(pinGroups))
			{
				this.currentList.Add(item);
				this.currentListGroups.Add(pinGroups);
			}
		}
		return this.currentList;
	}

	// Token: 0x06000BDA RID: 3034 RVA: 0x00035CE4 File Offset: 0x00033EE4
	protected override void OnPurchasedItem(int itemIndex)
	{
		if (itemIndex < 0 || itemIndex >= this.currentListGroups.Count)
		{
			Debug.LogError("Can't handle purchase for itemIndex out of range: " + itemIndex.ToString(), this);
			return;
		}
		PlayerData.instance.SetVariable(CaravanTroupeHunter.PdBools[this.currentListGroups[itemIndex]], true);
	}

	// Token: 0x04000B62 RID: 2914
	[SerializeField]
	[ArrayForEnum(typeof(CaravanTroupeHunter.PinGroups))]
	private CaravanTroupeHunter.ShopItemInfo[] shopItems;

	// Token: 0x04000B63 RID: 2915
	private List<ISimpleShopItem> currentList;

	// Token: 0x04000B64 RID: 2916
	private List<CaravanTroupeHunter.PinGroups> currentListGroups;

	// Token: 0x0200149F RID: 5279
	public enum PinGroups
	{
		// Token: 0x040083EF RID: 33775
		None = -1,
		// Token: 0x040083F0 RID: 33776
		Marrowlands,
		// Token: 0x040083F1 RID: 33777
		Midlands,
		// Token: 0x040083F2 RID: 33778
		Blastedlands,
		// Token: 0x040083F3 RID: 33779
		Citadel,
		// Token: 0x040083F4 RID: 33780
		Peaklands,
		// Token: 0x040083F5 RID: 33781
		Mucklands
	}

	// Token: 0x020014A0 RID: 5280
	[Serializable]
	public class ShopItemInfo : ISimpleShopItem
	{
		// Token: 0x06008428 RID: 33832 RVA: 0x0026AE91 File Offset: 0x00269091
		public string GetDisplayName()
		{
			return this.DisplayName;
		}

		// Token: 0x06008429 RID: 33833 RVA: 0x0026AE9E File Offset: 0x0026909E
		public Sprite GetIcon()
		{
			return null;
		}

		// Token: 0x0600842A RID: 33834 RVA: 0x0026AEA1 File Offset: 0x002690A1
		public int GetCost()
		{
			return this.Cost.Value;
		}

		// Token: 0x0600842B RID: 33835 RVA: 0x0026AEAE File Offset: 0x002690AE
		public bool DelayPurchase()
		{
			return false;
		}

		// Token: 0x040083F6 RID: 33782
		public LocalisedString DisplayName;

		// Token: 0x040083F7 RID: 33783
		public CostReference Cost;
	}
}
