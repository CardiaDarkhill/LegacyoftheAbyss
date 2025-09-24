using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002BF RID: 703
[ActionCategory("Enemy AI")]
public class StartCrawler : FsmStateAction
{
	// Token: 0x060018E4 RID: 6372 RVA: 0x00072269 File Offset: 0x00070469
	public override void Reset()
	{
		this.Target = null;
		this.crawler = null;
		this.ScheduleTurn = null;
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x00072280 File Offset: 0x00070480
	public override void OnEnter()
	{
		GameObject safe = this.Target.GetSafe(this);
		if (safe)
		{
			this.crawler = safe.GetComponent<Crawler>();
		}
		if (this.crawler)
		{
			this.crawler.StartCrawling(this.ScheduleTurn.Value);
		}
		base.Finish();
	}

	// Token: 0x040017DC RID: 6108
	public FsmOwnerDefault Target;

	// Token: 0x040017DD RID: 6109
	private Crawler crawler;

	// Token: 0x040017DE RID: 6110
	public FsmBool ScheduleTurn;
}
