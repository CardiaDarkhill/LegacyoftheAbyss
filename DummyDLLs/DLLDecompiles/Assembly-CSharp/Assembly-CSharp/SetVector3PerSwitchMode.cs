using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000469 RID: 1129
public class SetVector3PerSwitchMode : SwitchPlatformModeUpdateHandler
{
	// Token: 0x06002874 RID: 10356 RVA: 0x000B2756 File Offset: 0x000B0956
	protected override void OnOperationModeChanged(bool isHandheld)
	{
		if (isHandheld)
		{
			this.isHandheldEvent.Invoke(this.isHandheldValue);
			return;
		}
		this.notHandheldEvent.Invoke(this.notHandheldValue);
	}

	// Token: 0x0400249C RID: 9372
	[SerializeField]
	private Vector3 isHandheldValue;

	// Token: 0x0400249D RID: 9373
	[SerializeField]
	private SetVector3PerSwitchMode.Vector3UnityEvent isHandheldEvent;

	// Token: 0x0400249E RID: 9374
	[Space]
	[SerializeField]
	private Vector3 notHandheldValue;

	// Token: 0x0400249F RID: 9375
	[SerializeField]
	private SetVector3PerSwitchMode.Vector3UnityEvent notHandheldEvent;

	// Token: 0x0200177F RID: 6015
	[Serializable]
	private class Vector3UnityEvent : UnityEvent<Vector3>
	{
	}
}
