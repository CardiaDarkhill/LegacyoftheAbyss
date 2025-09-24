using System;

// Token: 0x0200034D RID: 845
public interface IMutable
{
	// Token: 0x170002F0 RID: 752
	// (get) Token: 0x06001D4A RID: 7498
	bool Muted { get; }

	// Token: 0x06001D4B RID: 7499
	void SetMute(bool muted);
}
