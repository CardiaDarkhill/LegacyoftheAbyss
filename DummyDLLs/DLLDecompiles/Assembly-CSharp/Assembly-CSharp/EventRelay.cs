using System;
using UnityEngine;

// Token: 0x020002B9 RID: 697
public sealed class EventRelay : MonoBehaviour
{
	// Token: 0x1400003B RID: 59
	// (add) Token: 0x060018AC RID: 6316 RVA: 0x000711EC File Offset: 0x0006F3EC
	// (remove) Token: 0x060018AD RID: 6317 RVA: 0x00071224 File Offset: 0x0006F424
	public event Action<string> OnSendEvent;

	// Token: 0x1400003C RID: 60
	// (add) Token: 0x060018AE RID: 6318 RVA: 0x0007125C File Offset: 0x0006F45C
	// (remove) Token: 0x060018AF RID: 6319 RVA: 0x00071294 File Offset: 0x0006F494
	public event Action<string> TemporaryEvent;

	// Token: 0x060018B0 RID: 6320 RVA: 0x000712C9 File Offset: 0x0006F4C9
	private void OnDisable()
	{
		this.TemporaryEvent = null;
	}

	// Token: 0x060018B1 RID: 6321 RVA: 0x000712D2 File Offset: 0x0006F4D2
	public void SendEvent(string eventName)
	{
		Action<string> onSendEvent = this.OnSendEvent;
		if (onSendEvent != null)
		{
			onSendEvent(eventName);
		}
		Action<string> temporaryEvent = this.TemporaryEvent;
		if (temporaryEvent == null)
		{
			return;
		}
		temporaryEvent(eventName);
	}
}
