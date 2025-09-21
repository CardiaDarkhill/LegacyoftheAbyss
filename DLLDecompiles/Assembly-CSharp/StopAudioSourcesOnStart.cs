using System;
using UnityEngine;

// Token: 0x02000282 RID: 642
public class StopAudioSourcesOnStart : MonoBehaviour, IBeginStopper
{
	// Token: 0x060016B1 RID: 5809 RVA: 0x00066372 File Offset: 0x00064572
	private void Awake()
	{
		this.sources = base.GetComponentsInChildren<AudioSource>(true);
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x00066381 File Offset: 0x00064581
	private void OnEnable()
	{
		this.hasStopped = false;
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x0006638C File Offset: 0x0006458C
	private void LateUpdate()
	{
		if (this.hasStopped)
		{
			return;
		}
		this.hasStopped = true;
		AudioSource[] array = this.sources;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x000663C6 File Offset: 0x000645C6
	public void DoBeginStop()
	{
		this.hasStopped = false;
	}

	// Token: 0x04001531 RID: 5425
	private AudioSource[] sources;

	// Token: 0x04001532 RID: 5426
	private bool hasStopped;
}
