using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001A9 RID: 425
[Serializable]
public class AchievementsList : ScriptableObject
{
	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06001054 RID: 4180 RVA: 0x0004E84D File Offset: 0x0004CA4D
	public IReadOnlyList<Achievement> Achievements
	{
		get
		{
			return this.achievements;
		}
	}

	// Token: 0x06001055 RID: 4181 RVA: 0x0004E858 File Offset: 0x0004CA58
	public Achievement FindAchievement(string key)
	{
		foreach (Achievement achievement in this.achievements)
		{
			if (string.Equals(achievement.PlatformKey, key))
			{
				return achievement;
			}
		}
		return null;
	}

	// Token: 0x06001056 RID: 4182 RVA: 0x0004E8BC File Offset: 0x0004CABC
	public bool AchievementExists(string key)
	{
		using (List<Achievement>.Enumerator enumerator = this.achievements.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (string.Equals(enumerator.Current.PlatformKey, key))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06001057 RID: 4183 RVA: 0x0004E91C File Offset: 0x0004CB1C
	private string GetElementName(int index)
	{
		try
		{
			Achievement achievement = this.achievements[index];
			return string.Format("{0}: {1} : ({2})", index + 1, achievement.PlatformKey, achievement.Type);
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		return string.Format("Element {0}", index);
	}

	// Token: 0x04000FCD RID: 4045
	[NamedArray("GetElementName")]
	[SerializeField]
	private List<Achievement> achievements;
}
