using System;
using UnityEngine;

// Token: 0x02000159 RID: 345
public class CameraShakeWhileEnabled : MonoBehaviour
{
	// Token: 0x06000A7F RID: 2687 RVA: 0x0002F134 File Offset: 0x0002D334
	private void OnEnable()
	{
		this.runWhileEnabled.Cache();
		this.runWhileEnabled.DoShake(this, true);
		if (this.doRepeat)
		{
			this.timer = this.interval;
		}
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x0002F162 File Offset: 0x0002D362
	private void OnDisable()
	{
		this.runWhileEnabled.CancelShake();
		this.runWhenDisabled.DoShake(this, true);
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x0002F17C File Offset: 0x0002D37C
	private void Update()
	{
		if (this.doRepeat)
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.timer += this.interval;
				this.runWhenDisabled.DoShake(this, true);
			}
		}
	}

	// Token: 0x040009F8 RID: 2552
	[SerializeField]
	private CameraShakeTarget runWhileEnabled;

	// Token: 0x040009F9 RID: 2553
	[SerializeField]
	private CameraShakeTarget runWhenDisabled;

	// Token: 0x040009FA RID: 2554
	[SerializeField]
	private bool doRepeat;

	// Token: 0x040009FB RID: 2555
	[SerializeField]
	private float interval = 0.15f;

	// Token: 0x040009FC RID: 2556
	private float timer;
}
