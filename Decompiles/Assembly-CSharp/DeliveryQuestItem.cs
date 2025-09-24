using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.Localization;
using UnityEngine;

// Token: 0x020001BE RID: 446
[CreateAssetMenu(menuName = "Hornet/Collectable Items/Collectable Item (Delivery Quest)")]
public class DeliveryQuestItem : CollectableItem
{
	// Token: 0x170001CC RID: 460
	// (get) Token: 0x06001147 RID: 4423 RVA: 0x000510B3 File Offset: 0x0004F2B3
	public GameObject HitUIEffect
	{
		get
		{
			return this.hitUIEffect;
		}
	}

	// Token: 0x170001CD RID: 461
	// (get) Token: 0x06001148 RID: 4424 RVA: 0x000510BB File Offset: 0x0004F2BB
	public GameObject HeroLoopEffect
	{
		get
		{
			return this.heroLoopEffect;
		}
	}

	// Token: 0x170001CE RID: 462
	// (get) Token: 0x06001149 RID: 4425 RVA: 0x000510C3 File Offset: 0x0004F2C3
	public GameObject UILoopEffect
	{
		get
		{
			return this.uiLoopEffect;
		}
	}

	// Token: 0x170001CF RID: 463
	// (get) Token: 0x0600114A RID: 4426 RVA: 0x000510CB File Offset: 0x0004F2CB
	public override bool DisplayAmount
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170001D0 RID: 464
	// (get) Token: 0x0600114B RID: 4427 RVA: 0x000510CE File Offset: 0x0004F2CE
	public Color BarColour
	{
		get
		{
			return this.barColour;
		}
	}

	// Token: 0x170001D1 RID: 465
	// (get) Token: 0x0600114C RID: 4428 RVA: 0x000510D6 File Offset: 0x0004F2D6
	public GameObject BreakUIEffect
	{
		get
		{
			return this.breakUIEffect;
		}
	}

	// Token: 0x170001D2 RID: 466
	// (get) Token: 0x0600114D RID: 4429 RVA: 0x000510DE File Offset: 0x0004F2DE
	protected override bool CanShowQuestUpdatedForItem
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600114E RID: 4430 RVA: 0x000510E1 File Offset: 0x0004F2E1
	public override string GetDisplayName(CollectableItem.ReadSource readSource)
	{
		return this.displayName;
	}

	// Token: 0x0600114F RID: 4431 RVA: 0x000510EE File Offset: 0x0004F2EE
	public override string GetDescription(CollectableItem.ReadSource readSource)
	{
		return this.description;
	}

	// Token: 0x06001150 RID: 4432 RVA: 0x000510FB File Offset: 0x0004F2FB
	public override Sprite GetIcon(CollectableItem.ReadSource readSource)
	{
		if (readSource <= CollectableItem.ReadSource.GetPopup)
		{
			return this.icon;
		}
		if (readSource != CollectableItem.ReadSource.Tiny)
		{
			throw new ArgumentOutOfRangeException("readSource", readSource, null);
		}
		return this.tinyIcon;
	}

	// Token: 0x06001151 RID: 4433 RVA: 0x00051126 File Offset: 0x0004F326
	protected override void OnCollected()
	{
		HeroController.instance.SetupDeliveryItems();
		EventRegister.SendEvent(EventRegisterEvents.DeliveryHudRefresh, null);
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0005113D File Offset: 0x0004F33D
	protected override void OnTaken()
	{
		HeroController instance = HeroController.instance;
		instance.RemoveDeliveryItemEffect(this);
		instance.SetupDeliveryItems();
		EventRegister.SendEvent(EventRegisterEvents.DeliveryHudRefresh, null);
	}

	// Token: 0x170001D3 RID: 467
	// (get) Token: 0x06001153 RID: 4435 RVA: 0x0005115B File Offset: 0x0004F35B
	public override bool CanConsume
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0005115E File Offset: 0x0004F35E
	public override void Consume(int amount, bool showCounter)
	{
		base.Consume(amount, showCounter);
		DeliveryQuestItem.ClearGenericQuests();
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0005116D File Offset: 0x0004F36D
	public static void ClearGenericQuests()
	{
		PlayerData.instance.BelltownCouriersGenericQuests = null;
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x0005117A File Offset: 0x0004F37A
	public static bool CanTakeHit()
	{
		return ToolItemManager.ActiveState != ToolsActiveStates.Disabled && GameManager.instance.GetCurrentMapZoneEnum() != MapZone.MEMORY;
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x00051198 File Offset: 0x0004F398
	public static IEnumerable<DeliveryQuestItem.ActiveItem> GetActiveItems()
	{
		int version = QuestManager.Version + CollectableItemManager.Version;
		if (DeliveryQuestItem.activeItemsCache.ShouldUpdate(version))
		{
			IEnumerable<DeliveryQuestItem.ActiveItem> first = from a in QuestManager.GetActiveQuests().Select(delegate(FullQuestBase q)
			{
				ValueTuple<FullQuestBase.QuestTarget, int> valueTuple = q.TargetsAndCounters.FirstOrDefault<ValueTuple<FullQuestBase.QuestTarget, int>>();
				return new DeliveryQuestItem.ActiveItem
				{
					Item = (valueTuple.Item1.Counter as DeliveryQuestItem),
					Quest = q,
					CurrentCount = valueTuple.Item2,
					MaxCount = valueTuple.Item1.Count
				};
			})
			where a.Item
			select a;
			IEnumerable<DeliveryQuestItem.ActiveItem> second = from item in CollectableItemManager.GetCollectedItems().OfType<DeliveryQuestItemStandalone>()
			select new DeliveryQuestItem.ActiveItem
			{
				Item = item,
				CurrentCount = item.CollectedAmount,
				MaxCount = item.TargetCount
			};
			DeliveryQuestItem.activeItemsCache.UpdateCache(first.Union(second).ToArray<DeliveryQuestItem.ActiveItem>(), version);
		}
		return DeliveryQuestItem.activeItemsCache.Value;
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0005125F File Offset: 0x0004F45F
	public static void TakeHit()
	{
		DeliveryQuestItem.TakeHit(1);
	}

	// Token: 0x06001159 RID: 4441 RVA: 0x00051268 File Offset: 0x0004F468
	public static void TakeHit(int hits)
	{
		foreach (DeliveryQuestItem.ActiveItem item in DeliveryQuestItem.GetActiveItems())
		{
			DeliveryQuestItem.TakeHitForItem(item, true, hits);
		}
	}

	// Token: 0x0600115A RID: 4442 RVA: 0x000512B4 File Offset: 0x0004F4B4
	public static void TakeHitForItem(DeliveryQuestItem.ActiveItem item, bool hitEffect)
	{
		DeliveryQuestItem.TakeHitForItem(item, hitEffect, 1);
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x000512C0 File Offset: 0x0004F4C0
	private static void CancelQuest(DeliveryQuestItem.ActiveItem item, PlayerData pd)
	{
		if (item.Quest.WasEverCompleted)
		{
			item.Quest.SilentlyComplete();
		}
		else
		{
			pd.QuestCompletionData.SetData(item.Quest.name, default(QuestCompletionData.Completion));
		}
		QuestManager.IncrementVersion();
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x0005130C File Offset: 0x0004F50C
	public static void TakeHitForItem(DeliveryQuestItem.ActiveItem item, bool hitEffect, int amount)
	{
		if (amount <= 0)
		{
			return;
		}
		if (!DeliveryQuestItem.CanTakeHit())
		{
			return;
		}
		Vector2 heroPos = HeroController.instance.transform.position;
		PlayerData instance = PlayerData.instance;
		if (hitEffect)
		{
			EventRegister.SendEvent(EventRegisterEvents.DeliveryHudHit, null);
		}
		if (item.CurrentCount <= amount)
		{
			if (item.Quest)
			{
				DeliveryQuestItem.CancelQuest(item, instance);
			}
			item.Item.BreakEffect(heroPos);
			EventRegister.SendEvent(EventRegisterEvents.DeliveryHudBreak, null);
			instance.BelltownCouriersBrokenDlgQueued = true;
			string name;
			UI.DestroyedPopup.ToString().TryFormat(out name, new object[]
			{
				item.Item.GetDisplayName(CollectableItem.ReadSource.TakePopup)
			});
			CollectableUIMsg.Spawn(new UIMsgDisplay
			{
				Name = name,
				Icon = (item.Item.breakIcon ? item.Item.breakIcon : item.Item.icon),
				IconScale = item.Item.GetUIMsgIconScale(),
				RepresentingObject = item.Item
			}, null, false);
		}
		else if (hitEffect)
		{
			item.Item.HitEffect(heroPos);
		}
		item.Item.Take(amount, false);
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x0005144A File Offset: 0x0004F64A
	public static void BreakAll()
	{
		DeliveryQuestItem.BreakAllInternal(true, false);
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x00051453 File Offset: 0x0004F653
	public static void BreakAllNoEffects()
	{
		DeliveryQuestItem.BreakAllInternal(false, false);
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x0005145C File Offset: 0x0004F65C
	public static void BreakTimedNoEffects()
	{
		DeliveryQuestItem.BreakAllInternal(false, true);
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x00051468 File Offset: 0x0004F668
	private static void BreakAllInternal(bool withEffects, bool onlyTimed)
	{
		Vector2 heroPos = withEffects ? HeroController.instance.transform.position : Vector2.zero;
		PlayerData instance = PlayerData.instance;
		foreach (DeliveryQuestItem.ActiveItem activeItem in DeliveryQuestItem.GetActiveItems())
		{
			if (!onlyTimed || activeItem.Item.totalTimer > 0f)
			{
				if (activeItem.Quest)
				{
					DeliveryQuestItem.CancelQuest(activeItem, instance);
				}
				if (withEffects)
				{
					EventRegister.SendEvent(EventRegisterEvents.DeliveryHudBreak, null);
				}
				activeItem.Item.Take(activeItem.Item.CollectedAmount, false);
				if (withEffects)
				{
					activeItem.Item.BreakEffect(heroPos);
				}
				instance.BelltownCouriersBrokenDlgQueued = true;
			}
		}
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x00051538 File Offset: 0x0004F738
	private void HitEffect(Vector2 heroPos)
	{
		if (!this.hitHeroEffect)
		{
			return;
		}
		Vector3 position = heroPos.ToVector3(this.hitHeroEffect.transform.localPosition.z);
		this.hitHeroEffect.Spawn(position);
	}

	// Token: 0x06001162 RID: 4450 RVA: 0x0005157C File Offset: 0x0004F77C
	private void BreakEffect(Vector2 heroPos)
	{
		if (!this.breakHeroEffect)
		{
			return;
		}
		Vector3 position = heroPos.ToVector3(this.breakHeroEffect.transform.localPosition.z);
		this.breakHeroEffect.Spawn(position);
	}

	// Token: 0x06001163 RID: 4451 RVA: 0x000515C0 File Offset: 0x0004F7C0
	public float GetChunkDuration(int maxCount)
	{
		if (this.totalTimer <= 0f)
		{
			return 0f;
		}
		return this.totalTimer / (float)maxCount;
	}

	// Token: 0x0400104A RID: 4170
	[Space]
	[SerializeField]
	private LocalisedString displayName;

	// Token: 0x0400104B RID: 4171
	[SerializeField]
	private LocalisedString description;

	// Token: 0x0400104C RID: 4172
	[SerializeField]
	private Sprite icon;

	// Token: 0x0400104D RID: 4173
	[SerializeField]
	private Sprite tinyIcon;

	// Token: 0x0400104E RID: 4174
	[SerializeField]
	private Color barColour = Color.white;

	// Token: 0x0400104F RID: 4175
	[SerializeField]
	private Sprite breakIcon;

	// Token: 0x04001050 RID: 4176
	[Space]
	[SerializeField]
	private GameObject hitHeroEffect;

	// Token: 0x04001051 RID: 4177
	[SerializeField]
	private GameObject hitUIEffect;

	// Token: 0x04001052 RID: 4178
	[SerializeField]
	private GameObject heroLoopEffect;

	// Token: 0x04001053 RID: 4179
	[SerializeField]
	private GameObject uiLoopEffect;

	// Token: 0x04001054 RID: 4180
	[SerializeField]
	private GameObject breakHeroEffect;

	// Token: 0x04001055 RID: 4181
	[SerializeField]
	private GameObject breakUIEffect;

	// Token: 0x04001056 RID: 4182
	[Space]
	[SerializeField]
	private float totalTimer;

	// Token: 0x04001057 RID: 4183
	private static ObjectCache<IEnumerable<DeliveryQuestItem.ActiveItem>> activeItemsCache = new ObjectCache<IEnumerable<DeliveryQuestItem.ActiveItem>>();

	// Token: 0x020014FF RID: 5375
	public struct ActiveItem
	{
		// Token: 0x04008578 RID: 34168
		public DeliveryQuestItem Item;

		// Token: 0x04008579 RID: 34169
		public FullQuestBase Quest;

		// Token: 0x0400857A RID: 34170
		public int CurrentCount;

		// Token: 0x0400857B RID: 34171
		public int MaxCount;
	}
}
