using System;

// Token: 0x020007AC RID: 1964
public abstract class VibrationEmission
{
	// Token: 0x170007C1 RID: 1985
	// (get) Token: 0x0600456B RID: 17771
	// (set) Token: 0x0600456C RID: 17772
	public abstract VibrationTarget Target { get; set; }

	// Token: 0x170007C2 RID: 1986
	// (get) Token: 0x0600456D RID: 17773
	// (set) Token: 0x0600456E RID: 17774
	public abstract bool IsLooping { get; set; }

	// Token: 0x170007C3 RID: 1987
	// (get) Token: 0x0600456F RID: 17775
	// (set) Token: 0x06004570 RID: 17776
	public abstract string Tag { get; set; }

	// Token: 0x170007C4 RID: 1988
	// (get) Token: 0x06004571 RID: 17777
	public abstract bool IsRealtime { get; }

	// Token: 0x170007C5 RID: 1989
	// (get) Token: 0x06004572 RID: 17778 RVA: 0x0012EF24 File Offset: 0x0012D124
	// (set) Token: 0x06004573 RID: 17779 RVA: 0x0012EF2C File Offset: 0x0012D12C
	public virtual float BaseStrength { get; set; } = 1f;

	// Token: 0x170007C6 RID: 1990
	// (get) Token: 0x06004574 RID: 17780 RVA: 0x0012EF35 File Offset: 0x0012D135
	// (set) Token: 0x06004575 RID: 17781 RVA: 0x0012EF44 File Offset: 0x0012D144
	public virtual float Strength
	{
		get
		{
			return this.strength * this.BaseStrength;
		}
		set
		{
			this.strength = value;
		}
	}

	// Token: 0x170007C7 RID: 1991
	// (get) Token: 0x06004576 RID: 17782 RVA: 0x0012EF4D File Offset: 0x0012D14D
	// (set) Token: 0x06004577 RID: 17783 RVA: 0x0012EF55 File Offset: 0x0012D155
	public virtual float Speed { get; set; } = 1f;

	// Token: 0x170007C8 RID: 1992
	// (get) Token: 0x06004578 RID: 17784 RVA: 0x0012EF5E File Offset: 0x0012D15E
	// (set) Token: 0x06004579 RID: 17785 RVA: 0x0012EF66 File Offset: 0x0012D166
	public virtual float Time { get; set; }

	// Token: 0x170007C9 RID: 1993
	// (get) Token: 0x0600457A RID: 17786
	public abstract bool IsPlaying { get; }

	// Token: 0x0600457B RID: 17787
	public abstract void Play();

	// Token: 0x0600457C RID: 17788
	public abstract void Stop();

	// Token: 0x0600457D RID: 17789 RVA: 0x0012EF6F File Offset: 0x0012D16F
	public void SetStrength(float value)
	{
		this.Strength = value;
	}

	// Token: 0x0600457E RID: 17790 RVA: 0x0012EF78 File Offset: 0x0012D178
	public void SetPlaybackTime(float time)
	{
		this.Time = time;
	}

	// Token: 0x0600457F RID: 17791 RVA: 0x0012EF81 File Offset: 0x0012D181
	public void SetSpeed(float speed)
	{
		this.Speed = speed;
	}

	// Token: 0x04004625 RID: 17957
	protected float strength = 1f;
}
