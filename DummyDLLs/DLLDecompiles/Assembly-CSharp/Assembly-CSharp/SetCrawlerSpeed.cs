using System;
using HutongGames.PlayMaker;

// Token: 0x020002C2 RID: 706
[ActionCategory("Enemy AI")]
public class SetCrawlerSpeed : FSMUtility.GetComponentFsmStateAction<Crawler>
{
	// Token: 0x060018EF RID: 6383 RVA: 0x000723C6 File Offset: 0x000705C6
	public override void Reset()
	{
		base.Reset();
		this.Speed = null;
	}

	// Token: 0x060018F0 RID: 6384 RVA: 0x000723D5 File Offset: 0x000705D5
	protected override void DoAction(Crawler crawler)
	{
		crawler.Speed = this.Speed.Value;
	}

	// Token: 0x040017E3 RID: 6115
	public FsmFloat Speed;
}
