using System;
using UnityEngine;

// Token: 0x0200046D RID: 1133
public abstract class SwitchPlatformModeUpdateHandler : MonoBehaviour
{
	// Token: 0x06002894 RID: 10388 RVA: 0x000B2D20 File Offset: 0x000B0F20
	protected virtual void OnEnable()
	{
		this.RegisterEvents();
		this.OnScreenModeUpdated(Platform.Current.ScreenMode);
	}

	// Token: 0x06002895 RID: 10389 RVA: 0x000B2D38 File Offset: 0x000B0F38
	protected virtual void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x06002896 RID: 10390 RVA: 0x000B2D40 File Offset: 0x000B0F40
	private void RegisterEvents()
	{
		if (!this.registeredEvents)
		{
			this.registeredEvents = true;
			Platform.Current.OnScreenModeChanged += this.OnScreenModeUpdated;
		}
	}

	// Token: 0x06002897 RID: 10391 RVA: 0x000B2D67 File Offset: 0x000B0F67
	private void UnregisterEvents()
	{
		if (this.registeredEvents)
		{
			this.registeredEvents = false;
			Platform.Current.OnScreenModeChanged -= this.OnScreenModeUpdated;
		}
	}

	// Token: 0x06002898 RID: 10392 RVA: 0x000B2D90 File Offset: 0x000B0F90
	private void OnScreenModeUpdated(Platform.ScreenModeState screenMode)
	{
		bool flag = (screenMode & (Platform.ScreenModeState.HandHeld | Platform.ScreenModeState.HandHeldSmall)) > Platform.ScreenModeState.Standard;
		if (flag && !Platform.Current.IsTargetHandHeld(this.handHeldTarget))
		{
			flag = false;
		}
		this.OnOperationModeChanged(flag);
	}

	// Token: 0x06002899 RID: 10393
	protected abstract void OnOperationModeChanged(bool isHandheld);

	// Token: 0x040024AC RID: 9388
	[SerializeField]
	protected Platform.HandHeldTypes handHeldTarget;

	// Token: 0x040024AD RID: 9389
	private bool registeredEvents;
}
