using System;
using UnityEngine;

// Token: 0x020000A8 RID: 168
public sealed class ReparentOnParticleStop : MonoBehaviour
{
	// Token: 0x0600050E RID: 1294 RVA: 0x0001A51C File Offset: 0x0001871C
	private void Awake()
	{
		if (this.particleSystem == null)
		{
			this.particleSystem = base.GetComponent<ParticleSystem>();
			if (this.particleSystem == null)
			{
				return;
			}
		}
		if (this.parent == null)
		{
			this.parent = base.transform.parent;
		}
		this.particleSystem.main.stopAction = ParticleSystemStopAction.Callback;
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x0001A585 File Offset: 0x00018785
	private void OnValidate()
	{
		if (this.parent == null)
		{
			this.parent = base.transform.parent;
		}
		if (this.particleSystem == null)
		{
			this.particleSystem = base.GetComponent<ParticleSystem>();
		}
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x0001A5C0 File Offset: 0x000187C0
	private void OnParticleSystemStopped()
	{
		if (this.parent != null)
		{
			base.transform.SetParent(this.parent);
		}
	}

	// Token: 0x040004EE RID: 1262
	[SerializeField]
	private ParticleSystem particleSystem;

	// Token: 0x040004EF RID: 1263
	[SerializeField]
	private Transform parent;
}
