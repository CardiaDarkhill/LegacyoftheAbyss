using System;

// Token: 0x0200043E RID: 1086
public struct PlayTime
{
	// Token: 0x06002592 RID: 9618 RVA: 0x000AB709 File Offset: 0x000A9909
	public PlayTime(float rawTime)
	{
		this.RawTime = rawTime;
	}

	// Token: 0x170003F8 RID: 1016
	// (get) Token: 0x06002593 RID: 9619 RVA: 0x000AB712 File Offset: 0x000A9912
	private TimeSpan Time
	{
		get
		{
			return TimeSpan.FromSeconds((double)this.RawTime);
		}
	}

	// Token: 0x170003F9 RID: 1017
	// (get) Token: 0x06002594 RID: 9620 RVA: 0x000AB720 File Offset: 0x000A9920
	public float Hours
	{
		get
		{
			return (float)Math.Floor(this.Time.TotalHours);
		}
	}

	// Token: 0x170003FA RID: 1018
	// (get) Token: 0x06002595 RID: 9621 RVA: 0x000AB744 File Offset: 0x000A9944
	public float Minutes
	{
		get
		{
			return (float)this.Time.Minutes;
		}
	}

	// Token: 0x170003FB RID: 1019
	// (get) Token: 0x06002596 RID: 9622 RVA: 0x000AB760 File Offset: 0x000A9960
	public float Seconds
	{
		get
		{
			return (float)this.Time.Seconds;
		}
	}

	// Token: 0x170003FC RID: 1020
	// (get) Token: 0x06002597 RID: 9623 RVA: 0x000AB77C File Offset: 0x000A997C
	public bool HasHours
	{
		get
		{
			return this.Time.TotalHours >= 1.0;
		}
	}

	// Token: 0x170003FD RID: 1021
	// (get) Token: 0x06002598 RID: 9624 RVA: 0x000AB7A8 File Offset: 0x000A99A8
	public bool HasMinutes
	{
		get
		{
			return this.Time.TotalMinutes >= 1.0;
		}
	}

	// Token: 0x06002599 RID: 9625 RVA: 0x000AB7D1 File Offset: 0x000A99D1
	public override string ToString()
	{
		return string.Format("{0:0}:{1:00}", (int)this.Hours, (int)this.Minutes);
	}

	// Token: 0x04002328 RID: 9000
	public float RawTime;
}
