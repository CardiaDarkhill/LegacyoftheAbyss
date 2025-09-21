using System;
using UnityEngine;

// Token: 0x02000399 RID: 921
public class BossStatueCompletionStates : MonoBehaviour
{
	// Token: 0x06001F20 RID: 7968 RVA: 0x0008E535 File Offset: 0x0008C735
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<BossStatueCompletionStates.State>(ref this.tierStates, typeof(BossStatueCompletionStates.Tiers));
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x0008E54C File Offset: 0x0008C74C
	private void Start()
	{
		BossStatueCompletionStates.Tiers? highestCompletedTier = this.GetHighestCompletedTier();
		if (highestCompletedTier == null)
		{
			this.defaultState.SetActive(true);
			return;
		}
		for (int i = 0; i < this.tierStates.Length; i++)
		{
			this.tierStates[i].SetActive(false);
		}
		for (int j = 0; j < this.tierStates.Length; j++)
		{
			if (j == (int)highestCompletedTier.Value)
			{
				this.tierStates[j].SetActive(true);
			}
		}
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x0008E5CC File Offset: 0x0008C7CC
	public BossStatueCompletionStates.Tiers? GetHighestCompletedTier()
	{
		for (int i = Enum.GetNames(typeof(BossStatueCompletionStates.Tiers)).Length - 1; i >= 0; i--)
		{
			if (this.GetIsTierCompleted((BossStatueCompletionStates.Tiers)i))
			{
				return new BossStatueCompletionStates.Tiers?((BossStatueCompletionStates.Tiers)i);
			}
		}
		return null;
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x0008E610 File Offset: 0x0008C810
	public bool GetIsTierCompleted(BossStatueCompletionStates.Tiers tier)
	{
		int num = 0;
		int num2 = 0;
		this.CountCompletion(tier, out num, out num2);
		return num >= num2;
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x0008E634 File Offset: 0x0008C834
	public void CountCompletion(BossStatueCompletionStates.Tiers tier, out int completed, out int total)
	{
		completed = 0;
		total = 0;
		foreach (BossStatue bossStatue in this.bossListSource ? this.bossListSource.bossStatues.ToArray() : new BossStatue[0])
		{
			if (bossStatue.gameObject.activeInHierarchy && !bossStatue.hasNoTiers && !bossStatue.dontCountCompletion)
			{
				if (bossStatue.HasRegularVersion)
				{
					total++;
					if (this.HasCompletedTier(bossStatue.StatueState, tier))
					{
						completed++;
					}
				}
				if (bossStatue.HasDreamVersion)
				{
					total++;
					if (this.HasCompletedTier(bossStatue.DreamStatueState, tier))
					{
						completed++;
					}
				}
			}
		}
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x0008E6E4 File Offset: 0x0008C8E4
	private bool HasCompletedTier(BossStatue.Completion completion, BossStatueCompletionStates.Tiers tier)
	{
		switch (tier)
		{
		case BossStatueCompletionStates.Tiers.Tier1:
			if (completion.completedTier1)
			{
				return true;
			}
			break;
		case BossStatueCompletionStates.Tiers.Tier2:
			if (completion.completedTier2)
			{
				return true;
			}
			break;
		case BossStatueCompletionStates.Tiers.Tier3:
			if (completion.completedTier3)
			{
				return true;
			}
			break;
		}
		return this.checkTiersAdditive && tier < (BossStatueCompletionStates.Tiers)Enum.GetNames(typeof(BossStatueCompletionStates.Tiers)).Length - 1 && this.HasCompletedTier(completion, tier + 1);
	}

	// Token: 0x04001E0E RID: 7694
	public BossSummaryBoard bossListSource;

	// Token: 0x04001E0F RID: 7695
	public BossStatueCompletionStates.State defaultState;

	// Token: 0x04001E10 RID: 7696
	[ArrayForEnum(typeof(BossStatueCompletionStates.Tiers))]
	public BossStatueCompletionStates.State[] tierStates;

	// Token: 0x04001E11 RID: 7697
	public bool checkTiersAdditive = true;

	// Token: 0x02001640 RID: 5696
	public enum Tiers
	{
		// Token: 0x04008A36 RID: 35382
		Tier1,
		// Token: 0x04008A37 RID: 35383
		Tier2,
		// Token: 0x04008A38 RID: 35384
		Tier3
	}

	// Token: 0x02001641 RID: 5697
	[Serializable]
	public struct State
	{
		// Token: 0x06008970 RID: 35184 RVA: 0x0027CFC8 File Offset: 0x0027B1C8
		public void SetActive(bool value)
		{
			if (this.gameObject)
			{
				this.gameObject.SetActive(value);
				if (value && !string.IsNullOrEmpty(this.playmakerEvent))
				{
					FSMUtility.SendEventToGameObject(this.gameObject, this.playmakerEvent, false);
				}
			}
		}

		// Token: 0x04008A39 RID: 35385
		[SerializeField]
		private GameObject gameObject;

		// Token: 0x04008A3A RID: 35386
		[SerializeField]
		private string playmakerEvent;
	}
}
