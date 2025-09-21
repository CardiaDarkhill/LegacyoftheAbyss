using System;
using HutongGames.PlayMaker;

// Token: 0x020002C1 RID: 705
[ActionCategory("Enemy AI")]
public class GetCrawlerType : FSMUtility.GetComponentFsmStateAction<Crawler>
{
	// Token: 0x060018EC RID: 6380 RVA: 0x00072397 File Offset: 0x00070597
	public override void Reset()
	{
		base.Reset();
		this.StoreType = null;
	}

	// Token: 0x060018ED RID: 6381 RVA: 0x000723A6 File Offset: 0x000705A6
	protected override void DoAction(Crawler crawler)
	{
		this.StoreType.Value = crawler.Type;
	}

	// Token: 0x040017E2 RID: 6114
	[ObjectType(typeof(Crawler.CrawlerTypes))]
	[UIHint(UIHint.Variable)]
	public FsmEnum StoreType;
}
