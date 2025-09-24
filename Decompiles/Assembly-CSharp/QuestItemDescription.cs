using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMProOld;
using UnityEngine;

// Token: 0x0200059E RID: 1438
public class QuestItemDescription : MonoBehaviour
{
	// Token: 0x060033B9 RID: 13241 RVA: 0x000E668F File Offset: 0x000E488F
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<QuestItemDescription.CounterMaterial>(ref this.counterMaterials, typeof(FullQuestBase.IconTypes));
	}

	// Token: 0x060033BA RID: 13242 RVA: 0x000E66A8 File Offset: 0x000E48A8
	private void Awake()
	{
		this.OnValidate();
		if (this.progressBarText)
		{
			this.initialProgressBarText = this.progressBarText.text;
		}
		if (this.donateCostIcon)
		{
			this.donateCostIconScale = this.donateCostIcon.transform.localScale;
		}
		if (this.rangeDisplay)
		{
			this.initialRangeIconOffset = this.rangeDisplay.MaxItemOffset;
		}
		this.hasAwoken = true;
	}

	// Token: 0x060033BB RID: 13243 RVA: 0x000E6724 File Offset: 0x000E4924
	public void SetDisplay(BasicQuestBase quest)
	{
		if (!this.hasAwoken)
		{
			this.Awake();
		}
		this.bottomSection.SetAllActive(false);
		if (this.rangeDisplay)
		{
			this.rangeDisplay.MaxValue = 0;
		}
		if (this.rewardGroup)
		{
			this.rewardGroup.SetActive(false);
		}
		if (this.spawnedTextDisplays != null)
		{
			using (List<QuestItemDescriptionText>.Enumerator enumerator = this.spawnedTextDisplays.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					QuestItemDescriptionText questItemDescriptionText = enumerator.Current;
					questItemDescriptionText.gameObject.SetActive(false);
				}
				goto IL_B8;
			}
		}
		if (this.textDisplayTemplate)
		{
			this.textDisplayTemplate.gameObject.SetActive(false);
		}
		IL_B8:
		if (this.donateCostGroup)
		{
			this.donateCostGroup.SetActive(false);
		}
		if (this.progressBarParent)
		{
			this.progressBarParent.SetActive(false);
		}
		List<ValueTuple<FullQuestBase.QuestTarget, int>> list = this.targetsList;
		if (list != null)
		{
			list.Clear();
		}
		FullQuestBase fullQuest = quest as FullQuestBase;
		if (!fullQuest || fullQuest.IsCompleted)
		{
			return;
		}
		bool flag = false;
		this.bottomSection.SetAllActive(true);
		switch (fullQuest.DescCounterType)
		{
		case FullQuestBase.DescCounterTypes.Icons:
			if (this.rangeDisplay)
			{
				if (this.targetsList == null)
				{
					this.targetsList = new List<ValueTuple<FullQuestBase.QuestTarget, int>>(fullQuest.TargetsAndCounters);
				}
				else
				{
					this.targetsList.AddRange(fullQuest.TargetsAndCounters);
				}
				int num = 0;
				int num2 = 0;
				foreach (ValueTuple<FullQuestBase.QuestTarget, int> valueTuple in this.targetsList)
				{
					FullQuestBase.QuestTarget item = valueTuple.Item1;
					int item2 = valueTuple.Item2;
					num2 += fullQuest.GetCollectedCountOverride(item, item2);
					num += item.Count;
				}
				this.rangeDisplay.MaxValue = num;
				this.rangeDisplay.CurrentValue = num2;
				this.rangeDisplay.MaxItemOffset = this.initialRangeIconOffset + fullQuest.CounterIconPadding;
				if (this.targetsList.Count > 0)
				{
					flag = true;
				}
				if (this.targetsList.Count <= 1)
				{
					this.rangeDisplay.SetFilledOverride(null);
				}
				else
				{
					this.rangeDisplay.SetFilledOverride(delegate(int index)
					{
						int index2 = base.<SetDisplay>g__FindTargetIndex|0(index);
						ValueTuple<FullQuestBase.QuestTarget, int> valueTuple4 = this.targetsList[index2];
						return valueTuple4.Item2 >= valueTuple4.Item1.Count;
					});
				}
				this.rangeDisplay.SetItemSprites(delegate(int index)
				{
					int index2 = base.<SetDisplay>g__FindTargetIndex|0(index);
					return fullQuest.GetCounterSpriteOverride(this.targetsList[index2].Item1, index);
				}, fullQuest.CounterIconScale);
			}
			break;
		case FullQuestBase.DescCounterTypes.Text:
			if (this.textDisplayTemplate)
			{
				if (this.targetsList == null)
				{
					this.targetsList = new List<ValueTuple<FullQuestBase.QuestTarget, int>>(fullQuest.TargetsAndCounters);
				}
				else
				{
					this.targetsList.AddRange(fullQuest.TargetsAndCounters);
				}
				if (this.spawnedTextDisplays == null)
				{
					this.spawnedTextDisplays = new List<QuestItemDescriptionText>(this.targetsList.Count)
					{
						this.textDisplayTemplate
					};
				}
				this.textDisplayTemplate.ResetDisplay();
				while (this.spawnedTextDisplays.Count < this.targetsList.Count)
				{
					this.spawnedTextDisplays.Add(Object.Instantiate<QuestItemDescriptionText>(this.textDisplayTemplate, this.textDisplayTemplate.transform.parent));
				}
				for (int i = 0; i < this.targetsList.Count; i++)
				{
					ValueTuple<FullQuestBase.QuestTarget, int> valueTuple2 = this.targetsList[i];
					QuestItemDescriptionText questItemDescriptionText2 = this.spawnedTextDisplays[i];
					questItemDescriptionText2.gameObject.SetActive(true);
					questItemDescriptionText2.SetDisplay(fullQuest, valueTuple2.Item1, valueTuple2.Item2);
				}
				if (this.neededTextGroup)
				{
					if (this.targetsList.Count > 0)
					{
						flag = true;
					}
					if (this.targetsList.Count <= 1)
					{
						this.neededTextGroup.SetLocalPositionY(0f);
					}
					else
					{
						float newY = this.neededTextGroupOffsetPerItemY * (float)(this.targetsList.Count - 1);
						this.neededTextGroup.SetLocalPositionY(newY);
					}
				}
			}
			break;
		case FullQuestBase.DescCounterTypes.ProgressBar:
			if (this.progressBarParent)
			{
				int num3 = 0;
				int num4 = 0;
				foreach (ValueTuple<FullQuestBase.QuestTarget, int> valueTuple3 in fullQuest.TargetsAndCounters)
				{
					FullQuestBase.QuestTarget item3 = valueTuple3.Item1;
					int item4 = valueTuple3.Item2;
					num4 += fullQuest.GetCollectedCountOverride(item3, item4);
					num3 += item3.Count;
				}
				if (num3 > 0)
				{
					flag = true;
					this.progressBarParent.SetActive(true);
					if (this.progressBarSlider)
					{
						this.progressBarSlider.Value = (float)num4 / (float)num3;
						this.progressBarSlider.Color = fullQuest.ProgressBarTint;
					}
					if (this.progressBarText)
					{
						this.progressBarText.text = string.Format(this.initialProgressBarText, num4, num3);
					}
				}
			}
			break;
		case FullQuestBase.DescCounterTypes.None:
			break;
		case FullQuestBase.DescCounterTypes.Custom:
			flag = true;
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		Sprite sprite = fullQuest.RewardIcon;
		if (this.rewardGroup)
		{
			this.rewardGroup.SetActive(sprite);
		}
		if (this.rewardIcon)
		{
			this.rewardIcon.sprite = sprite;
			if (sprite)
			{
				flag = true;
				QuestItemDescription.CounterMaterial counterMaterial = this.counterMaterials[(int)fullQuest.RewardIconType];
				this.rewardIcon.sharedMaterial = counterMaterial.Material;
			}
		}
		if (fullQuest.IsDonateType)
		{
			flag = true;
			if (this.donateCostGroup)
			{
				this.donateCostGroup.SetActive(true);
			}
			FullQuestBase.QuestTarget forTarget = fullQuest.Targets.FirstOrDefault<FullQuestBase.QuestTarget>();
			if (this.donateCostIcon)
			{
				this.donateCostIcon.sprite = fullQuest.GetCounterSpriteOverride(forTarget, 0);
				this.donateCostIcon.transform.localScale = this.donateCostIconScale.MultiplyElements(new Vector3(fullQuest.CounterIconScale, fullQuest.CounterIconScale, 1f));
			}
			if (this.donateCostText)
			{
				this.donateCostText.text = forTarget.Count.ToString();
			}
		}
		if (!flag)
		{
			this.bottomSection.SetAllActive(false);
		}
	}

	// Token: 0x0400376B RID: 14187
	[SerializeField]
	private GameObject[] bottomSection;

	// Token: 0x0400376C RID: 14188
	[Space]
	[SerializeField]
	private IconCounter rangeDisplay;

	// Token: 0x0400376D RID: 14189
	[SerializeField]
	private QuestItemDescriptionText textDisplayTemplate;

	// Token: 0x0400376E RID: 14190
	[SerializeField]
	private Transform neededTextGroup;

	// Token: 0x0400376F RID: 14191
	[SerializeField]
	private float neededTextGroupOffsetPerItemY;

	// Token: 0x04003770 RID: 14192
	[SerializeField]
	private GameObject rewardGroup;

	// Token: 0x04003771 RID: 14193
	[SerializeField]
	private SpriteRenderer rewardIcon;

	// Token: 0x04003772 RID: 14194
	[SerializeField]
	[ArrayForEnum(typeof(FullQuestBase.IconTypes))]
	private QuestItemDescription.CounterMaterial[] counterMaterials;

	// Token: 0x04003773 RID: 14195
	[Space]
	[SerializeField]
	private GameObject progressBarParent;

	// Token: 0x04003774 RID: 14196
	[SerializeField]
	private ImageSlider progressBarSlider;

	// Token: 0x04003775 RID: 14197
	[SerializeField]
	private TMP_Text progressBarText;

	// Token: 0x04003776 RID: 14198
	[Space]
	[SerializeField]
	private GameObject donateCostGroup;

	// Token: 0x04003777 RID: 14199
	[SerializeField]
	private SpriteRenderer donateCostIcon;

	// Token: 0x04003778 RID: 14200
	[SerializeField]
	private TMP_Text donateCostText;

	// Token: 0x04003779 RID: 14201
	private bool hasAwoken;

	// Token: 0x0400377A RID: 14202
	private string initialProgressBarText;

	// Token: 0x0400377B RID: 14203
	private Vector3 donateCostIconScale;

	// Token: 0x0400377C RID: 14204
	private Vector2 initialRangeIconOffset;

	// Token: 0x0400377D RID: 14205
	[TupleElementNames(new string[]
	{
		"target",
		"count"
	})]
	private List<ValueTuple<FullQuestBase.QuestTarget, int>> targetsList;

	// Token: 0x0400377E RID: 14206
	private List<QuestItemDescriptionText> spawnedTextDisplays;

	// Token: 0x0400377F RID: 14207
	private MaterialPropertyBlock block;

	// Token: 0x020018C7 RID: 6343
	[Serializable]
	private struct CounterMaterial
	{
		// Token: 0x04009355 RID: 37717
		public Material Material;
	}
}
