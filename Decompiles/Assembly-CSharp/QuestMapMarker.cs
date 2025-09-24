using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020006FC RID: 1788
public class QuestMapMarker : MapMarkerArrow, GameMapPinLayout.ILayoutHook
{
	// Token: 0x06003FFE RID: 16382 RVA: 0x0011A32B File Offset: 0x0011852B
	private bool IsUsingTargetSceneName()
	{
		return this.isVisibleCondition == QuestMapMarker.IsVisibleConditions.ActiveSteelSoulSpot;
	}

	// Token: 0x06003FFF RID: 16383 RVA: 0x0011A338 File Offset: 0x00118538
	protected override bool IsActive(bool isQuickMap, MapZone currentMapZone)
	{
		if (CollectableItemManager.IsInHiddenMode())
		{
			return false;
		}
		if (isQuickMap && this.quickMapZone != MapZone.NONE && currentMapZone != this.quickMapZone)
		{
			return false;
		}
		if (this.collectableItem)
		{
			return this.collectableItem.CollectedAmount > 0;
		}
		if (this.quest == null)
		{
			return false;
		}
		if (!this.quest.IsMapMarkerVisible)
		{
			return false;
		}
		switch (this.isVisibleCondition)
		{
		case QuestMapMarker.IsVisibleConditions.Active:
			return true;
		case QuestMapMarker.IsVisibleConditions.CanComplete:
		{
			IQuestWithCompletion questWithCompletion = this.quest as IQuestWithCompletion;
			return questWithCompletion == null || questWithCompletion.CanComplete;
		}
		case QuestMapMarker.IsVisibleConditions.ActiveSteelSoulSpot:
		{
			SteelSoulQuestSpot.Spot[] steelQuestSpots = PlayerData.instance.SteelQuestSpots;
			if (steelQuestSpots == null)
			{
				Debug.LogError("Steel Quest Spots array is null!", this);
				return false;
			}
			foreach (SteelSoulQuestSpot.Spot spot in steelQuestSpots)
			{
				if (spot == null)
				{
					Debug.LogError("Quest spot is null!", this);
				}
				else if (!spot.IsSeen && spot.SceneName == this.targetSceneName)
				{
					return true;
				}
			}
			return false;
		}
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06004000 RID: 16384 RVA: 0x0011A44A File Offset: 0x0011864A
	public void LayoutFinished()
	{
		base.SetPosition(base.transform.localPosition);
	}

	// Token: 0x040041AF RID: 16815
	[SerializeField]
	private MapZone quickMapZone;

	// Token: 0x040041B0 RID: 16816
	[SerializeField]
	[ModifiableProperty]
	[Conditional("collectableItem", false, false, false)]
	private BasicQuestBase quest;

	// Token: 0x040041B1 RID: 16817
	[SerializeField]
	[Conditional("collectableItem", false, false, false)]
	private QuestMapMarker.IsVisibleConditions isVisibleCondition;

	// Token: 0x040041B2 RID: 16818
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingTargetSceneName", true, true, false)]
	private string targetSceneName;

	// Token: 0x040041B3 RID: 16819
	[SerializeField]
	private CollectableItem collectableItem;

	// Token: 0x020019EC RID: 6636
	private enum IsVisibleConditions
	{
		// Token: 0x040097C0 RID: 38848
		Active,
		// Token: 0x040097C1 RID: 38849
		CanComplete,
		// Token: 0x040097C2 RID: 38850
		ActiveSteelSoulSpot
	}
}
