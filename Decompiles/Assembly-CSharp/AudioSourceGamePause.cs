using System;
using UnityEngine;

// Token: 0x02000111 RID: 273
[RequireComponent(typeof(AudioSource))]
public class AudioSourceGamePause : MonoBehaviour
{
	// Token: 0x170000B4 RID: 180
	// (get) Token: 0x0600088A RID: 2186 RVA: 0x000282E2 File Offset: 0x000264E2
	// (set) Token: 0x0600088B RID: 2187 RVA: 0x000282EA File Offset: 0x000264EA
	public bool IsPaused { get; private set; }

	// Token: 0x0600088C RID: 2188 RVA: 0x000282F3 File Offset: 0x000264F3
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		GameManager.instance.GamePausedChange += this.OnGamePausedChanged;
	}

	// Token: 0x0600088D RID: 2189 RVA: 0x00028317 File Offset: 0x00026517
	private void OnDestroy()
	{
		if (GameManager.UnsafeInstance)
		{
			GameManager.UnsafeInstance.GamePausedChange -= this.OnGamePausedChanged;
		}
	}

	// Token: 0x0600088E RID: 2190 RVA: 0x0002833B File Offset: 0x0002653B
	private void OnGamePausedChanged(bool isPaused)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (isPaused)
		{
			this.source.Pause();
		}
		else
		{
			this.source.UnPause();
		}
		this.IsPaused = isPaused;
	}

	// Token: 0x0400082E RID: 2094
	private AudioSource source;
}
