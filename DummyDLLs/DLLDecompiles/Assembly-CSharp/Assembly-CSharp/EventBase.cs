using System;

// Token: 0x02000331 RID: 817
public abstract class EventBase : DebugDrawColliderRuntimeAdder
{
	// Token: 0x14000051 RID: 81
	// (add) Token: 0x06001C99 RID: 7321 RVA: 0x000852E8 File Offset: 0x000834E8
	// (remove) Token: 0x06001C9A RID: 7322 RVA: 0x00085320 File Offset: 0x00083520
	public event Action ReceivedEvent;

	// Token: 0x170002E9 RID: 745
	// (get) Token: 0x06001C9B RID: 7323
	public abstract string InspectorInfo { get; }

	// Token: 0x06001C9C RID: 7324 RVA: 0x00085355 File Offset: 0x00083555
	protected void CallReceivedEvent()
	{
		if (this.ReceivedEvent != null)
		{
			this.ReceivedEvent();
		}
	}

	// Token: 0x06001C9D RID: 7325 RVA: 0x0008536A File Offset: 0x0008356A
	public override void AddDebugDrawComponent()
	{
	}
}
