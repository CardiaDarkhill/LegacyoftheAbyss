using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003D5 RID: 981
public sealed class CurrencyManager : ManagerSingleton<CurrencyManager>
{
	// Token: 0x1700037B RID: 891
	// (get) Token: 0x06002176 RID: 8566 RVA: 0x0009AB0E File Offset: 0x00098D0E
	private static PlayerData playerData
	{
		get
		{
			return PlayerData.instance;
		}
	}

	// Token: 0x06002177 RID: 8567 RVA: 0x0009AB15 File Offset: 0x00098D15
	protected override void Awake()
	{
		base.Awake();
		if (ManagerSingleton<CurrencyManager>.UnsafeInstance == this)
		{
			CurrencyManager.hasInstance = true;
		}
	}

	// Token: 0x06002178 RID: 8568 RVA: 0x0009AB30 File Offset: 0x00098D30
	protected override void OnDestroy()
	{
		if (ManagerSingleton<CurrencyManager>.UnsafeInstance == this)
		{
			CurrencyManager.hasInstance = false;
		}
		base.OnDestroy();
	}

	// Token: 0x06002179 RID: 8569 RVA: 0x0009AB4C File Offset: 0x00098D4C
	private void LateUpdate()
	{
		CurrencyManager.updateQueued = false;
		for (int i = 0; i < CurrencyManager.currencyQueue.Count; i++)
		{
			CurrencyManager.CurrencyQueue currencyQueue = CurrencyManager.currencyQueue[i];
			if (currencyQueue.amount != 0)
			{
				CurrencyManager.ProcessCurrency(currencyQueue.amount, (CurrencyType)i, currencyQueue.showCounter);
			}
			currencyQueue.Reset();
		}
		base.enabled = CurrencyManager.updateQueued;
	}

	// Token: 0x0600217A RID: 8570 RVA: 0x0009ABAB File Offset: 0x00098DAB
	public static void AddGeo(int amount)
	{
		CurrencyManager.ChangeCurrency(amount, CurrencyType.Money, true);
	}

	// Token: 0x0600217B RID: 8571 RVA: 0x0009ABB8 File Offset: 0x00098DB8
	public static void ToZero()
	{
		foreach (CurrencyManager.CurrencyQueue currencyQueue in CurrencyManager.currencyQueue)
		{
			currencyQueue.Reset();
		}
		CurrencyCounter.ToZero(CurrencyType.Money);
	}

	// Token: 0x0600217C RID: 8572 RVA: 0x0009AC10 File Offset: 0x00098E10
	public static void AddGeoQuietly(int amount)
	{
		CurrencyManager.playerData.AddGeo(amount);
	}

	// Token: 0x0600217D RID: 8573 RVA: 0x0009AC1D File Offset: 0x00098E1D
	public static void AddGeoToCounter(int amount)
	{
		CurrencyCounter.Add(amount, CurrencyType.Money);
	}

	// Token: 0x0600217E RID: 8574 RVA: 0x0009AC26 File Offset: 0x00098E26
	public static void TakeGeo(int amount)
	{
		CurrencyManager.ChangeCurrency(-amount, CurrencyType.Money, true);
	}

	// Token: 0x0600217F RID: 8575 RVA: 0x0009AC31 File Offset: 0x00098E31
	public static void AddShards(int amount)
	{
		CurrencyManager.ChangeCurrency(amount, CurrencyType.Shard, true);
	}

	// Token: 0x06002180 RID: 8576 RVA: 0x0009AC3B File Offset: 0x00098E3B
	public static void TakeShards(int amount)
	{
		CurrencyManager.ChangeCurrency(-amount, CurrencyType.Shard, true);
	}

	// Token: 0x06002181 RID: 8577 RVA: 0x0009AC46 File Offset: 0x00098E46
	public static int GetCurrencyAmount(CurrencyType type)
	{
		if (type == CurrencyType.Money)
		{
			return CurrencyManager.playerData.geo;
		}
		if (type != CurrencyType.Shard)
		{
			return 0;
		}
		return CurrencyManager.playerData.ShellShards;
	}

	// Token: 0x06002182 RID: 8578 RVA: 0x0009AC68 File Offset: 0x00098E68
	public static void TempStoreCurrency()
	{
		if (CurrencyManager.playerData.geo > 0)
		{
			CurrencyManager.playerData.TempGeoStore += CurrencyManager.playerData.geo;
			CurrencyManager.playerData.geo = 0;
		}
		if (CurrencyManager.playerData.ShellShards > 0)
		{
			CurrencyManager.playerData.TempShellShardStore += CurrencyManager.playerData.ShellShards;
			CurrencyManager.playerData.ShellShards = 0;
		}
	}

	// Token: 0x06002183 RID: 8579 RVA: 0x0009ACDC File Offset: 0x00098EDC
	public static void RestoreTempStoredCurrency()
	{
		if (CurrencyManager.playerData.TempGeoStore > 0)
		{
			CurrencyManager.ProcessAddCurrency(CurrencyManager.playerData.TempGeoStore, CurrencyType.Money, true);
			CurrencyManager.playerData.TempGeoStore = 0;
		}
		if (CurrencyManager.playerData.TempShellShardStore > 0)
		{
			CurrencyManager.ProcessAddCurrency(CurrencyManager.playerData.TempShellShardStore, CurrencyType.Shard, true);
			CurrencyManager.playerData.TempShellShardStore = 0;
		}
	}

	// Token: 0x06002184 RID: 8580 RVA: 0x0009AD3B File Offset: 0x00098F3B
	public static void AddCurrency(int amount, CurrencyType type, bool showCounter = true)
	{
		CurrencyManager.ChangeCurrency(amount, type, showCounter);
	}

	// Token: 0x06002185 RID: 8581 RVA: 0x0009AD45 File Offset: 0x00098F45
	public static void TakeCurrency(int amount, CurrencyType type, bool showCounter = true)
	{
		CurrencyManager.ChangeCurrency(-amount, type, showCounter);
	}

	// Token: 0x06002186 RID: 8582 RVA: 0x0009AD50 File Offset: 0x00098F50
	public static void ChangeCurrency(int amount, CurrencyType type, bool showCounter = true)
	{
		CurrencyManager.CurrencyQueue currencyQueue = CurrencyManager.currencyQueue[(int)type];
		currencyQueue.amount += amount;
		if (showCounter)
		{
			currencyQueue.showCounter = true;
		}
		if (CurrencyManager.hasInstance && !CurrencyManager.updateQueued && amount != 0)
		{
			ManagerSingleton<CurrencyManager>.UnsafeInstance.enabled = (CurrencyManager.updateQueued = true);
		}
	}

	// Token: 0x06002187 RID: 8583 RVA: 0x0009ADA3 File Offset: 0x00098FA3
	private static void ProcessCurrency(int amount, CurrencyType type, bool showCounter = true)
	{
		if (amount > 0)
		{
			CurrencyManager.ProcessAddCurrency(amount, type, showCounter);
			return;
		}
		CurrencyManager.ProcessTakeCurrency(amount, type, showCounter);
	}

	// Token: 0x06002188 RID: 8584 RVA: 0x0009ADBC File Offset: 0x00098FBC
	private static void ProcessAddCurrency(int amount, CurrencyType type, bool showCounter = true)
	{
		CurrencyCounter.RefreshStartCount(type);
		if (type != CurrencyType.Money)
		{
			if (type != CurrencyType.Shard)
			{
				Debug.LogError(string.Format("Unknown currency type: {0}", type));
			}
			else
			{
				int shellShards = CurrencyManager.playerData.ShellShards;
				CurrencyManager.playerData.AddShards(amount);
				amount = CurrencyManager.playerData.ShellShards - shellShards;
			}
		}
		else
		{
			int geo = CurrencyManager.playerData.geo;
			CurrencyManager.playerData.AddGeo(amount);
			amount = CurrencyManager.playerData.geo - geo;
		}
		if (showCounter)
		{
			CurrencyCounter.Add(amount, type);
		}
	}

	// Token: 0x06002189 RID: 8585 RVA: 0x0009AE44 File Offset: 0x00099044
	public static void ProcessTakeCurrency(int amount, CurrencyType type, bool showCounter = true)
	{
		amount = Mathf.Abs(amount);
		CurrencyCounter.RefreshStartCount(type);
		if (type != CurrencyType.Money)
		{
			if (type != CurrencyType.Shard)
			{
				Debug.LogError(string.Format("Unknown currency type: {0}", type));
			}
			else
			{
				CurrencyManager.playerData.TakeShards(amount);
			}
		}
		else
		{
			CurrencyManager.playerData.TakeGeo(amount);
		}
		if (showCounter)
		{
			CurrencyCounter.Take(amount, type);
		}
	}

	// Token: 0x04002048 RID: 8264
	private static readonly List<CurrencyManager.CurrencyQueue> currencyQueue = new List<CurrencyManager.CurrencyQueue>
	{
		new CurrencyManager.CurrencyQueue(),
		new CurrencyManager.CurrencyQueue()
	};

	// Token: 0x04002049 RID: 8265
	private static bool hasInstance;

	// Token: 0x0400204A RID: 8266
	private static bool updateQueued;

	// Token: 0x02001687 RID: 5767
	private sealed class CurrencyQueue
	{
		// Token: 0x06008A4E RID: 35406 RVA: 0x0027F845 File Offset: 0x0027DA45
		public void Reset()
		{
			this.amount = 0;
			this.showCounter = false;
		}

		// Token: 0x04008B3A RID: 35642
		public int amount;

		// Token: 0x04008B3B RID: 35643
		public bool showCounter;
	}
}
