using System;
using UnityEngine;

// Token: 0x02000112 RID: 274
public class AudioSourceMovingClips : MonoBehaviour
{
	// Token: 0x06000890 RID: 2192 RVA: 0x00028370 File Offset: 0x00026570
	private void OnEnable()
	{
		this.previousLocalPos = this.target.localPosition;
		this.audioSource.volume = 0f;
	}

	// Token: 0x06000891 RID: 2193 RVA: 0x00028398 File Offset: 0x00026598
	private void LateUpdate()
	{
		if (Time.deltaTime <= Mathf.Epsilon)
		{
			return;
		}
		Vector2 a = this.target.localPosition;
		float b = (a - this.previousLocalPos).magnitude / Time.deltaTime;
		this.smoothedSpeed = Mathf.Lerp(this.smoothedSpeed, b, this.lerpSpeed * Time.deltaTime);
		AudioClip audioClip = null;
		foreach (AudioSourceMovingClips.Clip clip in this.clips)
		{
			if (this.smoothedSpeed >= clip.SpeedThreshold)
			{
				audioClip = clip.AudioClip;
			}
		}
		if (this.audioSource.clip != audioClip)
		{
			bool isPlaying = this.audioSource.isPlaying;
			this.audioSource.clip = audioClip;
			if (isPlaying && this.audioSource.clip != null)
			{
				this.audioSource.Play();
			}
		}
		this.previousLocalPos = a;
	}

	// Token: 0x0400082F RID: 2095
	[SerializeField]
	private Transform target;

	// Token: 0x04000830 RID: 2096
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000831 RID: 2097
	[SerializeField]
	private float lerpSpeed;

	// Token: 0x04000832 RID: 2098
	[SerializeField]
	private AudioSourceMovingClips.Clip[] clips;

	// Token: 0x04000833 RID: 2099
	private float smoothedSpeed;

	// Token: 0x04000834 RID: 2100
	private Vector2 previousLocalPos;

	// Token: 0x0200145F RID: 5215
	[Serializable]
	private class Clip
	{
		// Token: 0x040082F8 RID: 33528
		public float SpeedThreshold;

		// Token: 0x040082F9 RID: 33529
		public AudioClip AudioClip;
	}
}
