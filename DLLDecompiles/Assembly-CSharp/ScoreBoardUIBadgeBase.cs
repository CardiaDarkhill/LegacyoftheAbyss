using System;
using TMProOld;
using UnityEngine;

// Token: 0x02000715 RID: 1813
public abstract class ScoreBoardUIBadgeBase : MonoBehaviour
{
	// Token: 0x17000751 RID: 1873
	// (get) Token: 0x06004099 RID: 16537
	public abstract int Score { get; }

	// Token: 0x17000752 RID: 1874
	// (get) Token: 0x0600409A RID: 16538 RVA: 0x0011C230 File Offset: 0x0011A430
	protected virtual bool IsVisible
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600409B RID: 16539 RVA: 0x0011C234 File Offset: 0x0011A434
	private void OnValidate()
	{
		this.Refresh();
		if (!Application.isPlaying)
		{
			ScoreBoardUI componentInParent = base.GetComponentInParent<ScoreBoardUI>();
			if (componentInParent)
			{
				componentInParent.Refresh();
			}
		}
	}

	// Token: 0x0600409C RID: 16540 RVA: 0x0011C263 File Offset: 0x0011A463
	private void OnEnable()
	{
		this.Refresh();
	}

	// Token: 0x0600409D RID: 16541 RVA: 0x0011C26C File Offset: 0x0011A46C
	private void Refresh()
	{
		int score = this.Score;
		if (this.scoreText)
		{
			this.scoreText.text = score.ToString();
		}
	}

	// Token: 0x0600409E RID: 16542 RVA: 0x0011C29F File Offset: 0x0011A49F
	public void Evaluate()
	{
		base.gameObject.SetActive(this.IsVisible);
	}

	// Token: 0x04004228 RID: 16936
	[SerializeField]
	private TMP_Text scoreText;
}
