using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000714 RID: 1812
public class ScoreBoardUI : MonoBehaviour
{
	// Token: 0x06004094 RID: 16532 RVA: 0x0011C0CB File Offset: 0x0011A2CB
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<Transform>(ref this.columns, typeof(ScoreBoardUI.FleaFestivalGames));
	}

	// Token: 0x06004095 RID: 16533 RVA: 0x0011C0E2 File Offset: 0x0011A2E2
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06004096 RID: 16534 RVA: 0x0011C0EA File Offset: 0x0011A2EA
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x06004097 RID: 16535 RVA: 0x0011C0F4 File Offset: 0x0011A2F4
	public void Refresh()
	{
		foreach (Transform transform in this.columns)
		{
			List<ScoreBoardUIBadgeBase> list = new List<ScoreBoardUIBadgeBase>(transform.childCount);
			foreach (object obj in transform)
			{
				ScoreBoardUIBadgeBase component = ((Transform)obj).GetComponent<ScoreBoardUIBadgeBase>();
				if (component)
				{
					list.Add(component);
				}
			}
			IEnumerable<ScoreBoardUIBadgeBase> enumerable = from s in list
			orderby s.Score descending, s is ScoreBoardUIBadgeHero
			select s;
			int num = 0;
			foreach (ScoreBoardUIBadgeBase scoreBoardUIBadgeBase in enumerable)
			{
				scoreBoardUIBadgeBase.transform.SetSiblingIndex(num);
				num++;
				if (Application.isPlaying)
				{
					scoreBoardUIBadgeBase.Evaluate();
				}
			}
		}
	}

	// Token: 0x04004227 RID: 16935
	[SerializeField]
	[ArrayForEnum(typeof(ScoreBoardUI.FleaFestivalGames))]
	private Transform[] columns;

	// Token: 0x02001A02 RID: 6658
	private enum FleaFestivalGames
	{
		// Token: 0x04009819 RID: 38937
		Juggling,
		// Token: 0x0400981A RID: 38938
		Bouncing,
		// Token: 0x0400981B RID: 38939
		Dodging
	}
}
