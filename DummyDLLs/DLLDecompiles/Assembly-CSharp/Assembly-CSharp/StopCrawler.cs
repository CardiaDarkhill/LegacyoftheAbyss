using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002C0 RID: 704
[ActionCategory("Enemy AI")]
public class StopCrawler : FsmStateAction
{
	// Token: 0x060018E7 RID: 6375 RVA: 0x000722DF File Offset: 0x000704DF
	public override void Reset()
	{
		this.Target = null;
		this.crawler = null;
		this.WaitForTurn = new FsmBool(true);
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x00072300 File Offset: 0x00070500
	public override void OnEnter()
	{
		GameObject safe = this.Target.GetSafe(this);
		if (safe)
		{
			this.crawler = safe.GetComponent<Crawler>();
		}
		if (!this.crawler)
		{
			base.Finish();
		}
		if (this.WaitForTurn.Value)
		{
			this.Evaluate();
			return;
		}
		this.crawler.StopCrawling();
		base.Finish();
	}

	// Token: 0x060018E9 RID: 6377 RVA: 0x00072366 File Offset: 0x00070566
	public override void OnUpdate()
	{
		this.Evaluate();
	}

	// Token: 0x060018EA RID: 6378 RVA: 0x0007236E File Offset: 0x0007056E
	private void Evaluate()
	{
		if (this.crawler.IsTurning)
		{
			return;
		}
		this.crawler.StopCrawling();
		base.Finish();
	}

	// Token: 0x040017DF RID: 6111
	public FsmOwnerDefault Target;

	// Token: 0x040017E0 RID: 6112
	private Crawler crawler;

	// Token: 0x040017E1 RID: 6113
	public FsmBool WaitForTurn;
}
