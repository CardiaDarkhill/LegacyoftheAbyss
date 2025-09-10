using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class CaravanSpider : SimpleShopMenuOwner
{
	// Token: 0x06000BCD RID: 3021 RVA: 0x000359A4 File Offset: 0x00033BA4
	protected override void Start()
	{
		base.Start();
		bool flag = this.isSceneLocked;
		this.isSceneLocked = false;
		string sceneNameString = GameManager.instance.GetSceneNameString();
		this.travelLocationsOrdered = new List<CaravanSpider.TravelLocationInfo>();
		if (!string.IsNullOrEmpty(sceneNameString))
		{
			foreach (FieldInfo fieldInfo in this.travelLocations.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
			{
				if (!(fieldInfo.FieldType != typeof(CaravanSpider.TravelLocationInfo)))
				{
					CaravanSpider.TravelLocationInfo location = (CaravanSpider.TravelLocationInfo)fieldInfo.GetValue(this.travelLocations);
					this.SetupTravelLocation(location, sceneNameString);
				}
			}
		}
		if (this.isSceneLocked)
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (flag)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "JUST UNLOCKED", false);
		}
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x00035A67 File Offset: 0x00033C67
	private void SetupTravelLocation(CaravanSpider.TravelLocationInfo location, string currentSceneName)
	{
		if (location.TargetScene == currentSceneName)
		{
			if (!location.AppearCondition.IsFulfilled)
			{
				this.isSceneLocked = true;
			}
			return;
		}
		if (!location.AppearCondition.IsFulfilled)
		{
			return;
		}
		this.travelLocationsOrdered.Add(location);
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x00035AA6 File Offset: 0x00033CA6
	protected override List<ISimpleShopItem> GetItems()
	{
		return this.travelLocationsOrdered.Cast<ISimpleShopItem>().ToList<ISimpleShopItem>();
	}

	// Token: 0x06000BD0 RID: 3024 RVA: 0x00035AB8 File Offset: 0x00033CB8
	protected override void OnPurchasedItem(int itemIndex)
	{
		CaravanSpider.TravelLocationInfo travelLocationInfo = this.travelLocationsOrdered.ElementAtOrDefault(itemIndex);
		PlayerData instance = PlayerData.instance;
		instance.CaravanSpiderTargetScene = string.Empty;
		if (travelLocationInfo == null)
		{
			return;
		}
		instance.CaravanSpiderTravelDirection = travelLocationInfo.DirectionTo;
		instance.CaravanSpiderTargetScene = travelLocationInfo.TargetScene;
	}

	// Token: 0x06000BD1 RID: 3025 RVA: 0x00035B00 File Offset: 0x00033D00
	public int GetCostForScene(string sceneName)
	{
		foreach (CaravanSpider.TravelLocationInfo travelLocationInfo in this.travelLocationsOrdered)
		{
			if (travelLocationInfo.TargetScene == sceneName)
			{
				return travelLocationInfo.GetCost();
			}
		}
		return 0;
	}

	// Token: 0x04000B5C RID: 2908
	[SerializeField]
	private CaravanSpider.TravelLocations travelLocations;

	// Token: 0x04000B5D RID: 2909
	private List<CaravanSpider.TravelLocationInfo> travelLocationsOrdered;

	// Token: 0x04000B5E RID: 2910
	private bool isSceneLocked;

	// Token: 0x0200149D RID: 5277
	[Serializable]
	public class TravelLocationInfo : ISimpleShopItem
	{
		// Token: 0x06008422 RID: 33826 RVA: 0x0026AE02 File Offset: 0x00269002
		public string GetDisplayName()
		{
			return this.DisplayName;
		}

		// Token: 0x06008423 RID: 33827 RVA: 0x0026AE0F File Offset: 0x0026900F
		public Sprite GetIcon()
		{
			return null;
		}

		// Token: 0x06008424 RID: 33828 RVA: 0x0026AE14 File Offset: 0x00269014
		public int GetCost()
		{
			CostReference costReference;
			if (this.AltCostCondition.IsDefined && this.AltCostCondition.IsFulfilled)
			{
				costReference = this.AltCost;
			}
			else
			{
				costReference = this.Cost;
			}
			if (!costReference)
			{
				return 0;
			}
			return costReference.Value;
		}

		// Token: 0x06008425 RID: 33829 RVA: 0x0026AE5B File Offset: 0x0026905B
		public bool DelayPurchase()
		{
			return this.AltCostCondition.IsDefined && this.AltCostCondition.IsFulfilled && this.DelayAltPurchase;
		}

		// Token: 0x040083E2 RID: 33762
		public LocalisedString DisplayName;

		// Token: 0x040083E3 RID: 33763
		public string TargetScene;

		// Token: 0x040083E4 RID: 33764
		[Range(0f, 360f)]
		public float DirectionTo;

		// Token: 0x040083E5 RID: 33765
		public PlayerDataTest AppearCondition;

		// Token: 0x040083E6 RID: 33766
		public CostReference Cost;

		// Token: 0x040083E7 RID: 33767
		public CostReference AltCost;

		// Token: 0x040083E8 RID: 33768
		public PlayerDataTest AltCostCondition;

		// Token: 0x040083E9 RID: 33769
		public bool DelayAltPurchase;
	}

	// Token: 0x0200149E RID: 5278
	[Serializable]
	public class TravelLocations
	{
		// Token: 0x040083EA RID: 33770
		public CaravanSpider.TravelLocationInfo Bonetown;

		// Token: 0x040083EB RID: 33771
		public CaravanSpider.TravelLocationInfo Wilds;

		// Token: 0x040083EC RID: 33772
		public CaravanSpider.TravelLocationInfo Belltown;

		// Token: 0x040083ED RID: 33773
		public CaravanSpider.TravelLocationInfo Coral;
	}
}
