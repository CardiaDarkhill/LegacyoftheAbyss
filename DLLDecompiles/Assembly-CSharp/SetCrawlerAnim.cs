using System;
using HutongGames.PlayMaker;

// Token: 0x020002C3 RID: 707
[ActionCategory("Enemy AI")]
public class SetCrawlerAnim : FSMUtility.GetComponentFsmStateAction<Crawler>
{
	// Token: 0x060018F2 RID: 6386 RVA: 0x000723F0 File Offset: 0x000705F0
	public override void Reset()
	{
		base.Reset();
		this.CrawlAnim = null;
		this.TurnAnim = null;
	}

	// Token: 0x060018F3 RID: 6387 RVA: 0x00072406 File Offset: 0x00070606
	protected override void DoAction(Crawler crawler)
	{
		if (!this.CrawlAnim.IsNone)
		{
			crawler.crawlAnimName = this.CrawlAnim.Value;
		}
		if (!this.TurnAnim.IsNone)
		{
			crawler.turnAnimName = this.TurnAnim.Value;
		}
	}

	// Token: 0x040017E4 RID: 6116
	public FsmString CrawlAnim;

	// Token: 0x040017E5 RID: 6117
	public FsmString TurnAnim;
}
