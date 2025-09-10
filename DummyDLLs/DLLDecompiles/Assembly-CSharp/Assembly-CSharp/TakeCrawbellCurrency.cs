using System;
using HutongGames.PlayMaker;

// Token: 0x02000438 RID: 1080
public class TakeCrawbellCurrency : FsmStateAction
{
	// Token: 0x06002538 RID: 9528 RVA: 0x000AAE78 File Offset: 0x000A9078
	public override void Reset()
	{
		this.StoreRosaries = null;
		this.StoreShellShards = null;
	}

	// Token: 0x06002539 RID: 9529 RVA: 0x000AAE88 File Offset: 0x000A9088
	public override void OnEnter()
	{
		PlayerData instance = PlayerData.instance;
		ArrayForEnumAttribute.EnsureArraySize<int>(ref instance.CrawbellCurrency, typeof(CurrencyType));
		this.StoreRosaries.Value = instance.CrawbellCurrency[0];
		instance.CrawbellCurrency[0] = 0;
		this.StoreShellShards.Value = instance.CrawbellCurrency[1];
		instance.CrawbellCurrency[1] = 0;
		if (instance.CrawbellCurrencyCaps != null)
		{
			for (int i = 0; i < instance.CrawbellCurrencyCaps.Length; i++)
			{
				instance.CrawbellCurrencyCaps[i] = 0;
			}
		}
		base.Finish();
	}

	// Token: 0x040022FA RID: 8954
	[UIHint(UIHint.Variable)]
	public FsmInt StoreRosaries;

	// Token: 0x040022FB RID: 8955
	[UIHint(UIHint.Variable)]
	public FsmInt StoreShellShards;
}
