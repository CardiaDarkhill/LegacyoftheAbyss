using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000570 RID: 1392
public sealed class BlackThreadStrandGroup : MonoBehaviour
{
	// Token: 0x17000563 RID: 1379
	// (get) Token: 0x060031D4 RID: 12756 RVA: 0x000DD288 File Offset: 0x000DB488
	public int TotalStrands
	{
		get
		{
			return this.strands.Count;
		}
	}

	// Token: 0x17000564 RID: 1380
	// (get) Token: 0x060031D5 RID: 12757 RVA: 0x000DD295 File Offset: 0x000DB495
	public int TotalVisibleStrands
	{
		get
		{
			return this.visibleStrands.Count;
		}
	}

	// Token: 0x060031D6 RID: 12758 RVA: 0x000DD2A4 File Offset: 0x000DB4A4
	private void Awake()
	{
		this.strands.RemoveAll((BlackThreadStrand o) => o == null);
		this.visibleStrands.AddRange(this.strands);
		this.visibleStrands.RemoveAll((BlackThreadStrand o) => !o.gameObject.activeSelf);
		this.visibleStrands.Shuffle<BlackThreadStrand>();
	}

	// Token: 0x060031D7 RID: 12759 RVA: 0x000DD324 File Offset: 0x000DB524
	public void ResetVisibleStrands()
	{
		foreach (BlackThreadStrand blackThreadStrand in this.strands)
		{
			blackThreadStrand.gameObject.SetActive(true);
		}
		this.visibleStrands.Clear();
		this.visibleStrands.AddRange(this.strands);
		this.visibleStrands.Shuffle<BlackThreadStrand>();
	}

	// Token: 0x060031D8 RID: 12760 RVA: 0x000DD3A4 File Offset: 0x000DB5A4
	public void HideStrands(int count)
	{
		if (count <= 0)
		{
			return;
		}
		for (int i = this.visibleStrands.Count - 1; i >= 0; i--)
		{
			this.visibleStrands[i].gameObject.SetActive(false);
			this.visibleStrands.RemoveAt(i);
			count--;
			if (count <= 0)
			{
				break;
			}
		}
	}

	// Token: 0x0400354E RID: 13646
	[SerializeField]
	private List<BlackThreadStrand> strands = new List<BlackThreadStrand>();

	// Token: 0x0400354F RID: 13647
	private List<BlackThreadStrand> visibleStrands = new List<BlackThreadStrand>();
}
