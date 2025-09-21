using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000521 RID: 1313
public class NeedolinTextOwner : MonoBehaviour
{
	// Token: 0x17000540 RID: 1344
	// (get) Token: 0x06002F32 RID: 12082 RVA: 0x000D0479 File Offset: 0x000CE679
	public NeedolinTextOwner.NeedolinRangeCheckTypes RangeCheck
	{
		get
		{
			return this.rangeCheck;
		}
	}

	// Token: 0x06002F33 RID: 12083 RVA: 0x000D0481 File Offset: 0x000CE681
	private bool IsUsingCustomRange()
	{
		return this.rangeCheck == NeedolinTextOwner.NeedolinRangeCheckTypes.Custom;
	}

	// Token: 0x06002F34 RID: 12084 RVA: 0x000D048C File Offset: 0x000CE68C
	private bool IsUsingTrigger()
	{
		return this.rangeCheck == NeedolinTextOwner.NeedolinRangeCheckTypes.Trigger;
	}

	// Token: 0x06002F35 RID: 12085 RVA: 0x000D0498 File Offset: 0x000CE698
	private void OnDrawGizmosSelected()
	{
		if (this.IsUsingCustomRange())
		{
			Vector3 position = base.transform.position;
			float? z = new float?(0f);
			Gizmos.DrawWireSphere(position.Where(null, null, z), this.customRange);
		}
	}

	// Token: 0x06002F36 RID: 12086 RVA: 0x000D04E7 File Offset: 0x000CE6E7
	private void OnEnable()
	{
		HeroPerformanceRegion.StartedPerforming += this.OnStartedNeedolin;
		HeroPerformanceRegion.StoppedPerforming += this.OnStoppedNeedolin;
		if (HeroPerformanceRegion.IsPerforming)
		{
			this.OnStartedNeedolin();
		}
	}

	// Token: 0x06002F37 RID: 12087 RVA: 0x000D0518 File Offset: 0x000CE718
	private void OnDisable()
	{
		HeroPerformanceRegion.StartedPerforming -= this.OnStartedNeedolin;
		HeroPerformanceRegion.StoppedPerforming -= this.OnStoppedNeedolin;
		this.OnStoppedNeedolin();
		if (this.wasInRange)
		{
			this.RemoveNeedolinText();
			this.wasInRange = false;
		}
	}

	// Token: 0x06002F38 RID: 12088 RVA: 0x000D0558 File Offset: 0x000CE758
	private void Update()
	{
		bool flag = this.isPlaying && this.IsInRange();
		if (flag && !this.wasInRange)
		{
			this.AddNeedolinText();
		}
		else if (!flag && this.wasInRange)
		{
			this.RemoveNeedolinText();
		}
		this.wasInRange = flag;
	}

	// Token: 0x06002F39 RID: 12089 RVA: 0x000D05A2 File Offset: 0x000CE7A2
	private void OnStartedNeedolin()
	{
		this.isPlaying = true;
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x000D05AB File Offset: 0x000CE7AB
	private void OnStoppedNeedolin()
	{
		this.isPlaying = false;
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x000D05B4 File Offset: 0x000CE7B4
	private bool IsInRange()
	{
		if (InteractManager.BlockingInteractable != null)
		{
			return false;
		}
		switch (this.rangeCheck)
		{
		case NeedolinTextOwner.NeedolinRangeCheckTypes.Inner:
			return HeroPerformanceRegion.GetAffectedState(base.transform, false) == HeroPerformanceRegion.AffectedState.ActiveInner;
		case NeedolinTextOwner.NeedolinRangeCheckTypes.Outer:
			return HeroPerformanceRegion.GetAffectedState(base.transform, false) > HeroPerformanceRegion.AffectedState.None;
		case NeedolinTextOwner.NeedolinRangeCheckTypes.Custom:
			return HeroPerformanceRegion.IsPlayingInRange(base.transform.position, this.customRange);
		case NeedolinTextOwner.NeedolinRangeCheckTypes.Manual:
			return false;
		case NeedolinTextOwner.NeedolinRangeCheckTypes.Trigger:
			return this.insideTrigger && this.insideTrigger.IsInside;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x000D0654 File Offset: 0x000CE854
	public void AddNeedolinText()
	{
		if (this.addedText != null)
		{
			return;
		}
		MapZone currentMapZoneEnum = GameManager.instance.GetCurrentMapZoneEnum();
		LocalisedTextCollectionField textCollection = this.text;
		foreach (NeedolinTextOwner.MapZoneOverride mapZoneOverride in this.mapZoneOverrides)
		{
			if (mapZoneOverride.MapZone == currentMapZoneEnum)
			{
				textCollection = mapZoneOverride.TextCollection;
			}
		}
		NeedolinMsgBox.AddText(textCollection.GetCollection(), false, false);
		this.addedText = textCollection;
		this.OnAddText.Invoke();
	}

	// Token: 0x06002F3D RID: 12093 RVA: 0x000D06EC File Offset: 0x000CE8EC
	public void RemoveNeedolinText()
	{
		if (this.addedText == null)
		{
			return;
		}
		NeedolinMsgBox.RemoveText(this.addedText.GetCollection());
		this.addedText = null;
		this.OnRemoveText.Invoke();
	}

	// Token: 0x06002F3E RID: 12094 RVA: 0x000D0719 File Offset: 0x000CE919
	public void SetTextCollection(LocalisedTextCollection textCollection)
	{
		this.text.SetCollection(textCollection);
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x000D0727 File Offset: 0x000CE927
	public void SetRangeCheckInner()
	{
		this.rangeCheck = NeedolinTextOwner.NeedolinRangeCheckTypes.Inner;
	}

	// Token: 0x040031FE RID: 12798
	[SerializeField]
	private NeedolinTextOwner.NeedolinRangeCheckTypes rangeCheck;

	// Token: 0x040031FF RID: 12799
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingCustomRange", true, true, false)]
	private float customRange;

	// Token: 0x04003200 RID: 12800
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsUsingTrigger", true, true, false)]
	private TrackTriggerObjects insideTrigger;

	// Token: 0x04003201 RID: 12801
	[Space]
	[SerializeField]
	private LocalisedTextCollectionField text;

	// Token: 0x04003202 RID: 12802
	[Space]
	[SerializeField]
	private List<NeedolinTextOwner.MapZoneOverride> mapZoneOverrides;

	// Token: 0x04003203 RID: 12803
	[Space]
	public UnityEvent OnAddText;

	// Token: 0x04003204 RID: 12804
	public UnityEvent OnRemoveText;

	// Token: 0x04003205 RID: 12805
	private LocalisedTextCollectionField addedText;

	// Token: 0x04003206 RID: 12806
	private bool isPlaying;

	// Token: 0x04003207 RID: 12807
	private bool wasInRange;

	// Token: 0x02001831 RID: 6193
	public enum NeedolinRangeCheckTypes
	{
		// Token: 0x04009111 RID: 37137
		Inner,
		// Token: 0x04009112 RID: 37138
		Outer,
		// Token: 0x04009113 RID: 37139
		Custom,
		// Token: 0x04009114 RID: 37140
		Manual,
		// Token: 0x04009115 RID: 37141
		Trigger
	}

	// Token: 0x02001832 RID: 6194
	[Serializable]
	private struct MapZoneOverride
	{
		// Token: 0x04009116 RID: 37142
		public MapZone MapZone;

		// Token: 0x04009117 RID: 37143
		public LocalisedTextCollectionField TextCollection;
	}
}
