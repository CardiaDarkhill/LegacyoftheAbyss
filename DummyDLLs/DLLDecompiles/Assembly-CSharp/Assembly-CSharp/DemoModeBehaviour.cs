using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000417 RID: 1047
public class DemoModeBehaviour : MonoBehaviour
{
	// Token: 0x06002392 RID: 9106 RVA: 0x000A2BC1 File Offset: 0x000A0DC1
	private void OnValidate()
	{
		if (this.exhibitionMode)
		{
			this.OnIsExhibitionMode = this.OnIsDemoMode.Clone<UnityEvent>();
			this.OnIsDemoMode = this.OnIsNotDemoMode.Clone<UnityEvent>();
			this.exhibitionMode = false;
		}
	}

	// Token: 0x06002393 RID: 9107 RVA: 0x000A2BF4 File Offset: 0x000A0DF4
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06002394 RID: 9108 RVA: 0x000A2BFC File Offset: 0x000A0DFC
	private void OnEnable()
	{
		if (!this.waitForEvent)
		{
			this.DoBehaviour();
			return;
		}
		this.waitForEvent.ReceivedEvent += this.DoBehaviour;
	}

	// Token: 0x06002395 RID: 9109 RVA: 0x000A2C29 File Offset: 0x000A0E29
	private void OnDisable()
	{
		if (this.waitForEvent)
		{
			this.waitForEvent.ReceivedEvent -= this.DoBehaviour;
		}
	}

	// Token: 0x06002396 RID: 9110 RVA: 0x000A2C50 File Offset: 0x000A0E50
	private void DoBehaviour()
	{
		if (this.waitForEvent)
		{
			this.waitForEvent.ReceivedEvent -= this.DoBehaviour;
		}
		if (DemoHelper.IsExhibitionMode)
		{
			this.OnIsExhibitionMode.Invoke();
			return;
		}
		if (DemoHelper.IsDemoMode)
		{
			this.OnIsDemoMode.Invoke();
			return;
		}
		this.OnIsNotDemoMode.Invoke();
	}

	// Token: 0x04002249 RID: 8777
	[SerializeField]
	private EventBase waitForEvent;

	// Token: 0x0400224A RID: 8778
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private bool exhibitionMode;

	// Token: 0x0400224B RID: 8779
	public UnityEvent OnIsExhibitionMode;

	// Token: 0x0400224C RID: 8780
	public UnityEvent OnIsDemoMode;

	// Token: 0x0400224D RID: 8781
	public UnityEvent OnIsNotDemoMode;
}
