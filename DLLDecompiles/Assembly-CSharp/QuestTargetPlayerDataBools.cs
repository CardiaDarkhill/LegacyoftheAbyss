using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005A9 RID: 1449
[CreateAssetMenu(menuName = "Hornet/Quests/Quest Target PlayerData Bools")]
public class QuestTargetPlayerDataBools : QuestTargetCounter, SceneLintLogger.ILogProvider
{
	// Token: 0x170005BD RID: 1469
	// (get) Token: 0x06003417 RID: 13335 RVA: 0x000E7ED8 File Offset: 0x000E60D8
	[UsedImplicitly]
	public int CompletedCount
	{
		get
		{
			return this.CountCompleted();
		}
	}

	// Token: 0x170005BE RID: 1470
	// (get) Token: 0x06003418 RID: 13336 RVA: 0x000E7EE0 File Offset: 0x000E60E0
	[UsedImplicitly]
	public bool AllCollected
	{
		get
		{
			return !this.CanGetMore();
		}
	}

	// Token: 0x06003419 RID: 13337 RVA: 0x000E7EEC File Offset: 0x000E60EC
	public override bool CanGetMore()
	{
		int num;
		int num2;
		this.GetCounts(out num, out num2);
		return num < num2;
	}

	// Token: 0x0600341A RID: 13338 RVA: 0x000E7F07 File Offset: 0x000E6107
	public override int GetCompletionAmount(QuestCompletionData.Completion sourceCompletion)
	{
		return this.CountCompleted();
	}

	// Token: 0x0600341B RID: 13339 RVA: 0x000E7F10 File Offset: 0x000E6110
	public int CountCompleted()
	{
		int result;
		int num;
		this.GetCounts(out result, out num);
		return result;
	}

	// Token: 0x0600341C RID: 13340 RVA: 0x000E7F28 File Offset: 0x000E6128
	private void GetCounts(out int completed, out int total)
	{
		PlayerData instance = PlayerData.instance;
		completed = 0;
		total = 0;
		if (!string.IsNullOrWhiteSpace(this.pdFieldTemplate))
		{
			foreach (bool flag in instance.GetVariables(this.pdFieldTemplate))
			{
				total++;
				if (flag)
				{
					completed++;
				}
			}
		}
		foreach (QuestTargetPlayerDataBools.BoolInfo boolInfo in this.pdBools)
		{
			total++;
			if (instance.GetVariable(boolInfo.BoolName))
			{
				completed++;
			}
		}
	}

	// Token: 0x0600341D RID: 13341 RVA: 0x000E7FDC File Offset: 0x000E61DC
	private bool UsesSceneBools()
	{
		return !string.IsNullOrWhiteSpace(this.pdFieldTemplate) && this.getAppendsSceneName;
	}

	// Token: 0x0600341E RID: 13342 RVA: 0x000E7FF4 File Offset: 0x000E61F4
	public override void Get(bool showPopup = true)
	{
		if (!this.UsesSceneBools())
		{
			base.Get(showPopup);
			return;
		}
		PlayerData instance = PlayerData.instance;
		GameManager instance2 = GameManager.instance;
		string text = this.pdFieldTemplate + instance2.GetSceneNameString();
		if (VariableExtensions.VariableExists<bool, PlayerData>(text))
		{
			instance.SetVariable(text, true);
		}
		else
		{
			Debug.LogWarning("PD Variable " + text + " does not exist! This is fine if we're just calling this to display quest updated after bool set.", this);
		}
		if (showPopup)
		{
			QuestManager.MaybeShowQuestUpdated(null, this, null);
		}
		this.CheckAchievements();
	}

	// Token: 0x0600341F RID: 13343 RVA: 0x000E806C File Offset: 0x000E626C
	public bool GetSceneBoolValue()
	{
		if (!this.UsesSceneBools())
		{
			return false;
		}
		GameManager instance = GameManager.instance;
		IIncludeVariableExtensions instance2 = PlayerData.instance;
		string fieldName = this.pdFieldTemplate + instance.GetSceneNameString();
		return instance2.GetVariable(fieldName);
	}

	// Token: 0x06003420 RID: 13344 RVA: 0x000E80A8 File Offset: 0x000E62A8
	public string GetSceneLintLog(string sceneName)
	{
		if (!this.UsesSceneBools())
		{
			return null;
		}
		string text = this.pdFieldTemplate + GameManager.GetBaseSceneName(sceneName);
		if (!VariableExtensions.VariableExists<PlayerData>(text, typeof(bool)))
		{
			return base.name + " missing " + text + " in PlayerData";
		}
		return null;
	}

	// Token: 0x06003421 RID: 13345 RVA: 0x000E80FC File Offset: 0x000E62FC
	public void CheckAchievements()
	{
		int num;
		int num2;
		this.GetCounts(out num, out num2);
		GameManager instance = GameManager.instance;
		if (!string.IsNullOrWhiteSpace(this.linkedAchievementHalf))
		{
			int max = Mathf.RoundToInt((float)num2 * 0.5f);
			instance.UpdateAchievementProgress(this.linkedAchievementHalf, num, max);
		}
		if (!string.IsNullOrWhiteSpace(this.linkedAchievementFull))
		{
			instance.UpdateAchievementProgress(this.linkedAchievementFull, num, num2);
		}
		this.RecordOrder(num);
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x000E8164 File Offset: 0x000E6364
	public override Sprite GetPopupIcon()
	{
		return this.questCounterSprite;
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x000E816C File Offset: 0x000E636C
	public override Sprite GetQuestCounterSprite(int index)
	{
		PlayerData instance = PlayerData.instance;
		if (!string.IsNullOrEmpty(this.orderListPd))
		{
			List<int> variable = instance.GetVariable(this.orderListPd);
			if (variable != null && variable.Count > 0)
			{
				if (index >= variable.Count)
				{
					return base.GetQuestCounterSprite(index);
				}
				int num = variable[index];
				if (num < 0 || num >= this.pdBools.Length)
				{
					return base.GetQuestCounterSprite(index);
				}
				QuestTargetPlayerDataBools.BoolInfo boolInfo = this.pdBools[num];
				if (!boolInfo.AltSprite)
				{
					return base.GetQuestCounterSprite(index);
				}
				return boolInfo.AltSprite;
			}
		}
		if (index >= 0 && index < this.pdBools.Length)
		{
			QuestTargetPlayerDataBools.BoolInfo boolInfo2 = this.pdBools[index];
			if (boolInfo2.AltSprite && instance.GetVariable(boolInfo2.BoolName))
			{
				return boolInfo2.AltSprite;
			}
		}
		return base.GetQuestCounterSprite(index);
	}

	// Token: 0x06003424 RID: 13348 RVA: 0x000E8248 File Offset: 0x000E6448
	public void RecordOrder(int completedCount)
	{
		if (string.IsNullOrEmpty(this.orderListPd))
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		List<int> list = instance.GetVariable(this.orderListPd);
		if (list == null)
		{
			list = new List<int>();
			instance.SetVariable(this.orderListPd, list);
		}
		if (this.pdBoolsFoundIcon == null || this.pdBoolsFoundIcon.Length != this.pdBools.Length)
		{
			this.pdBoolsFoundIcon = new bool[this.pdBools.Length];
		}
		for (int i = 0; i < this.pdBoolsFoundIcon.Length; i++)
		{
			this.pdBoolsFoundIcon[i] = false;
		}
		foreach (int num in list)
		{
			if (num >= 0 && num < this.pdBoolsFoundIcon.Length)
			{
				this.pdBoolsFoundIcon[num] = true;
			}
		}
		for (int j = list.Count; j < completedCount; j++)
		{
			list.Add(-1);
			for (int k = 0; k < this.pdBools.Length; k++)
			{
				if (!this.pdBoolsFoundIcon[k])
				{
					QuestTargetPlayerDataBools.BoolInfo boolInfo = this.pdBools[k];
					if (instance.GetVariable(boolInfo.BoolName))
					{
						list[j] = k;
						this.pdBoolsFoundIcon[k] = true;
						break;
					}
				}
			}
		}
	}

	// Token: 0x040037A9 RID: 14249
	[SerializeField]
	private Sprite questCounterSprite;

	// Token: 0x040037AA RID: 14250
	[Space]
	[SerializeField]
	private QuestTargetPlayerDataBools.BoolInfo[] pdBools;

	// Token: 0x040037AB RID: 14251
	[SerializeField]
	[PlayerDataField(typeof(List<int>), true)]
	private string orderListPd;

	// Token: 0x040037AC RID: 14252
	[Space]
	[SerializeField]
	private string pdFieldTemplate;

	// Token: 0x040037AD RID: 14253
	[SerializeField]
	private bool getAppendsSceneName;

	// Token: 0x040037AE RID: 14254
	[Space]
	[SerializeField]
	private string linkedAchievementHalf;

	// Token: 0x040037AF RID: 14255
	[SerializeField]
	private string linkedAchievementFull;

	// Token: 0x040037B0 RID: 14256
	private bool[] pdBoolsFoundIcon;

	// Token: 0x020018D0 RID: 6352
	[Serializable]
	private struct BoolInfo
	{
		// Token: 0x0400936A RID: 37738
		[PlayerDataField(typeof(bool), true)]
		public string BoolName;

		// Token: 0x0400936B RID: 37739
		public Sprite AltSprite;
	}
}
