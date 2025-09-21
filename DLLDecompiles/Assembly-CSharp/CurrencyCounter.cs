using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000623 RID: 1571
public class CurrencyCounter : CurrencyCounterTyped<CurrencyType>
{
	// Token: 0x1700065B RID: 1627
	// (get) Token: 0x060037C5 RID: 14277 RVA: 0x000F5F20 File Offset: 0x000F4120
	protected override int Count
	{
		get
		{
			PlayerData instance = PlayerData.instance;
			CurrencyType currencyType = this.currencyType;
			if (currencyType == CurrencyType.Money)
			{
				return instance.geo;
			}
			if (currencyType != CurrencyType.Shard)
			{
				return 0;
			}
			return instance.ShellShards;
		}
	}

	// Token: 0x1700065C RID: 1628
	// (get) Token: 0x060037C6 RID: 14278 RVA: 0x000F5F52 File Offset: 0x000F4152
	protected override CurrencyType CounterType
	{
		get
		{
			return this.currencyType;
		}
	}

	// Token: 0x060037C7 RID: 14279 RVA: 0x000F5F5C File Offset: 0x000F415C
	protected override void Awake()
	{
		base.Awake();
		List<CurrencyCounter> list;
		if (!CurrencyCounter._currencyCounters.TryGetValue(this.currencyType, out list))
		{
			list = (CurrencyCounter._currencyCounters[this.currencyType] = new List<CurrencyCounter>());
		}
		list.Add(this);
		Transform parent = base.transform.parent;
		this.stack = (parent ? parent.GetComponent<CurrencyCounterStack>() : null);
		if (this.stack != null)
		{
			this.stack.AddNewCounter(this);
		}
	}

	// Token: 0x060037C8 RID: 14280 RVA: 0x000F5FE0 File Offset: 0x000F41E0
	protected override void OnDestroy()
	{
		base.OnDestroy();
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(this.currencyType, out list))
		{
			list.Remove(this);
		}
	}

	// Token: 0x060037C9 RID: 14281 RVA: 0x000F6010 File Offset: 0x000F4210
	private void OnEnable()
	{
		if (this.limitTextMesh)
		{
			if (this.limitTextFormat == null)
			{
				this.limitTextFormat = this.limitTextMesh.Text;
			}
			this.limitTextMesh.Text = string.Format(this.limitTextFormat, Gameplay.GetCurrencyCap(this.currencyType));
		}
		if (!this.hasStarted)
		{
			return;
		}
		base.UpdateCounterStart();
	}

	// Token: 0x060037CA RID: 14282 RVA: 0x000F6078 File Offset: 0x000F4278
	protected override void Start()
	{
		this.hasStarted = true;
		base.Start();
		base.UpdateCounterStart();
	}

	// Token: 0x060037CB RID: 14283 RVA: 0x000F6090 File Offset: 0x000F4290
	protected override void RefreshText(bool isCountingUp)
	{
		Color color = (!isCountingUp || this.Count < Gameplay.GetCurrencyCap(this.currencyType)) ? Color.white : UI.MaxItemsTextColor;
		this.geoTextMesh.Color = color;
		if (this.limitTextMesh)
		{
			this.limitTextMesh.Color = color;
		}
	}

	// Token: 0x060037CC RID: 14284 RVA: 0x000F60E5 File Offset: 0x000F42E5
	public void SetStackVisible(bool isVisible)
	{
		if (this.stack != null)
		{
			this.stack.ReportVisibleChange(isVisible);
		}
	}

	// Token: 0x060037CD RID: 14285 RVA: 0x000F6104 File Offset: 0x000F4304
	public static void Add(int amount, CurrencyType type)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					currencyCounter.QueueAdd(amount);
				}
			}
		}
	}

	// Token: 0x060037CE RID: 14286 RVA: 0x000F616C File Offset: 0x000F436C
	public static void Take(int amount, CurrencyType type)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					currencyCounter.QueueTake(amount);
				}
			}
		}
	}

	// Token: 0x060037CF RID: 14287 RVA: 0x000F61D4 File Offset: 0x000F43D4
	public static void ToValue(int amount, CurrencyType type)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					currencyCounter.QueueToValue(amount);
				}
			}
		}
	}

	// Token: 0x060037D0 RID: 14288 RVA: 0x000F623C File Offset: 0x000F443C
	public static void ToZero(CurrencyType type)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					currencyCounter.QueueToZero();
				}
			}
		}
	}

	// Token: 0x060037D1 RID: 14289 RVA: 0x000F62A0 File Offset: 0x000F44A0
	public static void Show(CurrencyType type, bool setStackVisible = false)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					if (setStackVisible)
					{
						currencyCounter.SetStackVisible(true);
					}
					if (!currencyCounter.IsActive)
					{
						currencyCounter.UpdateCounterStart();
					}
					currencyCounter.InternalShow();
				}
			}
		}
	}

	// Token: 0x060037D2 RID: 14290 RVA: 0x000F631C File Offset: 0x000F451C
	public static void Hide(CurrencyType type)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					currencyCounter.InternalHide(false);
				}
			}
		}
	}

	// Token: 0x060037D3 RID: 14291 RVA: 0x000F6384 File Offset: 0x000F4584
	public static void HideForced(CurrencyType type)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					currencyCounter.InternalHide(true);
				}
			}
		}
	}

	// Token: 0x060037D4 RID: 14292 RVA: 0x000F63EC File Offset: 0x000F45EC
	public static void ReportFail(CurrencyType type)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					currencyCounter.InternalFail();
				}
			}
		}
	}

	// Token: 0x060037D5 RID: 14293 RVA: 0x000F6450 File Offset: 0x000F4650
	public static void RefreshStartCount(CurrencyType type)
	{
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					if (currencyCounter.IsActive)
					{
						break;
					}
					currencyCounter.UpdateCounterStart();
				}
			}
		}
	}

	// Token: 0x060037D6 RID: 14294 RVA: 0x000F64C0 File Offset: 0x000F46C0
	private static void DoCounterAction(CurrencyType type, Action<CurrencyCounter> forEach)
	{
		if (forEach == null)
		{
			return;
		}
		List<CurrencyCounter> list;
		if (CurrencyCounter._currencyCounters.TryGetValue(type, out list))
		{
			foreach (CurrencyCounter currencyCounter in list)
			{
				if (currencyCounter.isActiveAndEnabled)
				{
					forEach(currencyCounter);
				}
			}
		}
	}

	// Token: 0x04003AD3 RID: 15059
	private static readonly Dictionary<CurrencyType, List<CurrencyCounter>> _currencyCounters = new Dictionary<CurrencyType, List<CurrencyCounter>>();

	// Token: 0x04003AD4 RID: 15060
	[Space]
	[SerializeField]
	private CurrencyType currencyType;

	// Token: 0x04003AD5 RID: 15061
	[SerializeField]
	private TextBridge limitTextMesh;

	// Token: 0x04003AD6 RID: 15062
	private string limitTextFormat;

	// Token: 0x04003AD7 RID: 15063
	private bool hasStarted;

	// Token: 0x04003AD8 RID: 15064
	private CurrencyCounterStack stack;
}
