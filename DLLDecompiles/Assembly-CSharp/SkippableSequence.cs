using System;
using UnityEngine;

// Token: 0x020000B0 RID: 176
public abstract class SkippableSequence : MonoBehaviour
{
	// Token: 0x1700005E RID: 94
	// (get) Token: 0x0600052E RID: 1326 RVA: 0x0001AA76 File Offset: 0x00018C76
	// (set) Token: 0x0600052F RID: 1327 RVA: 0x0001AA7E File Offset: 0x00018C7E
	public bool CanSkip
	{
		get
		{
			return this.canSkip;
		}
		protected set
		{
			this.canSkip = value;
		}
	}

	// Token: 0x1700005F RID: 95
	// (get) Token: 0x06000530 RID: 1328 RVA: 0x0001AA87 File Offset: 0x00018C87
	public bool WaitForSkip
	{
		get
		{
			return this.waitForSkip;
		}
	}

	// Token: 0x17000060 RID: 96
	// (get) Token: 0x06000531 RID: 1329 RVA: 0x0001AA8F File Offset: 0x00018C8F
	public virtual bool ShouldShow
	{
		get
		{
			return this.condition.IsFulfilled;
		}
	}

	// Token: 0x06000532 RID: 1330 RVA: 0x0001AA9C File Offset: 0x00018C9C
	public void SetActive(bool value)
	{
		base.gameObject.SetActive(value);
		if (this.scaler == null)
		{
			this.scaler = GameCameras.instance.tk2dCam.GetComponent<CameraRenderScaled>();
			if (!this.scaler)
			{
				return;
			}
		}
		if (value)
		{
			this.scaler.ForceFullResolution = this.forceFullResolution;
		}
	}

	// Token: 0x06000533 RID: 1331 RVA: 0x0001AAFA File Offset: 0x00018CFA
	[ContextMenu("Allow Skip")]
	public virtual void AllowSkip()
	{
		this.canSkip = true;
	}

	// Token: 0x06000534 RID: 1332
	public abstract void Begin();

	// Token: 0x17000061 RID: 97
	// (get) Token: 0x06000535 RID: 1333
	public abstract bool IsPlaying { get; }

	// Token: 0x06000536 RID: 1334
	public abstract void Skip();

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000537 RID: 1335
	public abstract bool IsSkipped { get; }

	// Token: 0x17000063 RID: 99
	// (get) Token: 0x06000538 RID: 1336
	// (set) Token: 0x06000539 RID: 1337
	public abstract float FadeByController { get; set; }

	// Token: 0x0400050D RID: 1293
	[SerializeField]
	private PlayerDataTest condition;

	// Token: 0x0400050E RID: 1294
	[Space]
	[SerializeField]
	private bool forceFullResolution;

	// Token: 0x0400050F RID: 1295
	[SerializeField]
	private bool canSkip = true;

	// Token: 0x04000510 RID: 1296
	[SerializeField]
	private bool waitForSkip;

	// Token: 0x04000511 RID: 1297
	private CameraRenderScaled scaler;
}
