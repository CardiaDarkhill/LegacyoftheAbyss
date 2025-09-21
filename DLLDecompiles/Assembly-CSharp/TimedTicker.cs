using System;
using UnityEngine;

// Token: 0x020004B9 RID: 1209
public class TimedTicker : EventBase
{
	// Token: 0x17000511 RID: 1297
	// (get) Token: 0x06002BA0 RID: 11168 RVA: 0x000BF540 File Offset: 0x000BD740
	public override string InspectorInfo
	{
		get
		{
			return string.Format("{0} seconds", this.TickDelay);
		}
	}

	// Token: 0x17000512 RID: 1298
	// (get) Token: 0x06002BA1 RID: 11169 RVA: 0x000BF557 File Offset: 0x000BD757
	public float TickDelay
	{
		get
		{
			if (!this.parentTicker)
			{
				return this.tickDelay;
			}
			return this.parentTicker.TickDelay * (float)(1 + this.tickMisses);
		}
	}

	// Token: 0x06002BA2 RID: 11170 RVA: 0x000BF582 File Offset: 0x000BD782
	private void OnEnable()
	{
		this.timeElapsed = 0f;
		this.ticksMissed = 0;
		if (this.parentTicker)
		{
			this.parentTicker.ReceivedEvent += this.UpdateTickParent;
		}
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x000BF5BA File Offset: 0x000BD7BA
	private void Start()
	{
		if (this.tickOnStart)
		{
			this.Tick();
		}
	}

	// Token: 0x06002BA4 RID: 11172 RVA: 0x000BF5CA File Offset: 0x000BD7CA
	private void OnDisable()
	{
		if (this.parentTicker)
		{
			this.parentTicker.ReceivedEvent -= this.UpdateTickParent;
		}
	}

	// Token: 0x06002BA5 RID: 11173 RVA: 0x000BF5F0 File Offset: 0x000BD7F0
	private void Update()
	{
		if (!this.parentTicker)
		{
			this.UpdateTickTimed();
		}
	}

	// Token: 0x06002BA6 RID: 11174 RVA: 0x000BF608 File Offset: 0x000BD808
	private void UpdateTickTimed()
	{
		float num = this.TickDelay;
		if (this.timeElapsed > num)
		{
			this.timeElapsed %= num;
			this.Tick();
		}
		this.timeElapsed += Time.deltaTime;
	}

	// Token: 0x06002BA7 RID: 11175 RVA: 0x000BF64B File Offset: 0x000BD84B
	private void UpdateTickParent()
	{
		if (this.ticksMissed >= this.tickMisses)
		{
			this.ticksMissed = 0;
			this.Tick();
			return;
		}
		this.ticksMissed++;
	}

	// Token: 0x06002BA8 RID: 11176 RVA: 0x000BF677 File Offset: 0x000BD877
	private void Tick()
	{
		if (!string.IsNullOrEmpty(this.sendEventToRegister))
		{
			EventRegister.SendEvent(this.sendEventToRegister, null);
		}
		base.CallReceivedEvent();
	}

	// Token: 0x04002CF3 RID: 11507
	[SerializeField]
	private TimedTicker parentTicker;

	// Token: 0x04002CF4 RID: 11508
	[SerializeField]
	[ModifiableProperty]
	[Conditional("parentTicker", false, false, true)]
	private float tickDelay;

	// Token: 0x04002CF5 RID: 11509
	[SerializeField]
	[ModifiableProperty]
	[Conditional("parentTicker", true, false, true)]
	private int tickMisses;

	// Token: 0x04002CF6 RID: 11510
	[SerializeField]
	private bool tickOnStart;

	// Token: 0x04002CF7 RID: 11511
	[Space]
	[SerializeField]
	private string sendEventToRegister;

	// Token: 0x04002CF8 RID: 11512
	private float timeElapsed;

	// Token: 0x04002CF9 RID: 11513
	private int ticksMissed;
}
