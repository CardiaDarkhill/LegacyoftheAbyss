using System;
using UnityEngine;

// Token: 0x02000460 RID: 1120
public class GamePadAudio : MonoBehaviour
{
	// Token: 0x0600282B RID: 10283 RVA: 0x000B1DCD File Offset: 0x000AFFCD
	private void Awake()
	{
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x000B1DCF File Offset: 0x000AFFCF
	private void Init()
	{
		this.audioSource = base.gameObject.AddComponent<AudioSource>();
		this.vibrationSource = base.gameObject.AddComponent<AudioSource>();
	}

	// Token: 0x04002456 RID: 9302
	private AudioSource audioSource;

	// Token: 0x04002457 RID: 9303
	private AudioSource vibrationSource;
}
