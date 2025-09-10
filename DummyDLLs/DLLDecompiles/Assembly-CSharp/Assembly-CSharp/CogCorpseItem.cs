using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004BA RID: 1210
public class CogCorpseItem : MonoBehaviour
{
	// Token: 0x17000513 RID: 1299
	// (get) Token: 0x06002BAA RID: 11178 RVA: 0x000BF6A0 File Offset: 0x000BD8A0
	public bool IsBroken
	{
		get
		{
			return this.breaker && this.breaker.IsBroken;
		}
	}

	// Token: 0x17000514 RID: 1300
	// (get) Token: 0x06002BAB RID: 11179 RVA: 0x000BF6BC File Offset: 0x000BD8BC
	public bool TrackDisable
	{
		get
		{
			return this.trackDisable;
		}
	}

	// Token: 0x06002BAC RID: 11180 RVA: 0x000BF6C4 File Offset: 0x000BD8C4
	private void Awake()
	{
		this.col = base.GetComponent<Collider2D>();
		if (this.col)
		{
			this.wasTrigger = this.col.isTrigger;
		}
		this.breaker = base.GetComponent<BreakWhenNotMoving>();
	}

	// Token: 0x06002BAD RID: 11181 RVA: 0x000BF6FC File Offset: 0x000BD8FC
	private void OnDisable()
	{
		if (this.trackDisable)
		{
			foreach (CogCorpseReaction cogCorpseReaction in this.insideReactions)
			{
				cogCorpseReaction.RemoveCorpse(base.transform);
			}
			this.insideReactions.Clear();
		}
	}

	// Token: 0x06002BAE RID: 11182 RVA: 0x000BF768 File Offset: 0x000BD968
	public void EnteredCogs()
	{
		if (this.col)
		{
			this.col.isTrigger = true;
		}
	}

	// Token: 0x06002BAF RID: 11183 RVA: 0x000BF783 File Offset: 0x000BD983
	public void ExitedCogs()
	{
		if (this.col)
		{
			this.col.isTrigger = this.wasTrigger;
		}
		if (this.breaker)
		{
			this.breaker.Break();
		}
	}

	// Token: 0x06002BB0 RID: 11184 RVA: 0x000BF7BB File Offset: 0x000BD9BB
	public void AddTrackedRegion(CogCorpseReaction cogReaction)
	{
		if (this.trackDisable)
		{
			this.insideReactions.Add(cogReaction);
		}
	}

	// Token: 0x06002BB1 RID: 11185 RVA: 0x000BF7D2 File Offset: 0x000BD9D2
	public void RemoveTrackedRegion(CogCorpseReaction cogReaction)
	{
		if (this.trackDisable)
		{
			this.insideReactions.Remove(cogReaction);
		}
	}

	// Token: 0x04002CFA RID: 11514
	private Collider2D col;

	// Token: 0x04002CFB RID: 11515
	private bool wasTrigger;

	// Token: 0x04002CFC RID: 11516
	private BreakWhenNotMoving breaker;

	// Token: 0x04002CFD RID: 11517
	[SerializeField]
	private bool trackDisable = true;

	// Token: 0x04002CFE RID: 11518
	private HashSet<CogCorpseReaction> insideReactions = new HashSet<CogCorpseReaction>();
}
