using System;
using UnityEngine;

// Token: 0x0200010D RID: 269
public sealed class AudioPlayStateSync : MonoBehaviour
{
	// Token: 0x06000874 RID: 2164 RVA: 0x00027EB5 File Offset: 0x000260B5
	private void Awake()
	{
		if (this.selfSource == null)
		{
			this.selfSource = base.GetComponent<AudioSource>();
		}
	}

	// Token: 0x06000875 RID: 2165 RVA: 0x00027ED1 File Offset: 0x000260D1
	private void OnEnable()
	{
		if (this.selfSource == null)
		{
			base.enabled = false;
		}
		if (this.otherSource == null)
		{
			base.enabled = false;
		}
		this.didPlay = false;
	}

	// Token: 0x06000876 RID: 2166 RVA: 0x00027F04 File Offset: 0x00026104
	private void Update()
	{
		if (this.otherSource.isPlaying)
		{
			this.didPlay = true;
			if (!this.selfSource.isPlaying)
			{
				this.selfSource.Play();
				return;
			}
		}
		else
		{
			if (this.selfSource.isPlaying)
			{
				this.selfSource.Stop();
			}
			if (this.didPlay && this.disableScriptOnStop)
			{
				this.selfSource.Stop();
				base.enabled = false;
			}
		}
	}

	// Token: 0x06000877 RID: 2167 RVA: 0x00027F78 File Offset: 0x00026178
	public void SetTarget(AudioSource target)
	{
		this.otherSource = target;
		base.enabled = true;
	}

	// Token: 0x04000818 RID: 2072
	[SerializeField]
	private AudioSource otherSource;

	// Token: 0x04000819 RID: 2073
	[SerializeField]
	private AudioSource selfSource;

	// Token: 0x0400081A RID: 2074
	[SerializeField]
	private bool disableScriptOnStop;

	// Token: 0x0400081B RID: 2075
	private bool didPlay;
}
