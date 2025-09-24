using System;
using UnityEngine;

// Token: 0x02000535 RID: 1333
public class RandomAudioStartPosition : MonoBehaviour
{
	// Token: 0x06002FCB RID: 12235 RVA: 0x000D2630 File Offset: 0x000D0830
	private void Start()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		if (this.audioSource == null || this.audioSource.clip == null)
		{
			return;
		}
		float time;
		if (this.timeMin != 0f || this.timeMax != 0f)
		{
			time = Random.Range(this.timeMin, this.timeMax);
		}
		else
		{
			time = Random.Range(0f, this.audioSource.clip.length);
		}
		this.audioSource.time = time;
	}

	// Token: 0x04003290 RID: 12944
	[SerializeField]
	private float timeMin;

	// Token: 0x04003291 RID: 12945
	[SerializeField]
	private float timeMax;

	// Token: 0x04003292 RID: 12946
	private AudioSource audioSource;
}
