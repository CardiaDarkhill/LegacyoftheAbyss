using System;
using UnityEngine;

// Token: 0x0200012C RID: 300
public class PlayAudioAndRecycle : MonoBehaviour
{
	// Token: 0x170000C9 RID: 201
	// (get) Token: 0x0600094B RID: 2379 RVA: 0x0002AE88 File Offset: 0x00029088
	// (set) Token: 0x0600094C RID: 2380 RVA: 0x0002AE90 File Offset: 0x00029090
	public bool KeepAliveThroughNextScene { get; set; }

	// Token: 0x0600094D RID: 2381 RVA: 0x0002AE99 File Offset: 0x00029099
	private void Awake()
	{
		this.pauseHandler = base.GetComponent<AudioSourceGamePause>();
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x0002AEA7 File Offset: 0x000290A7
	private void OnEnable()
	{
		this.skipFrames = 2;
		if (this.audioSource.clip)
		{
			this.audioSource.Play();
		}
		PlayAudioAndRecycle.activeList.Add(this);
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x0002AED9 File Offset: 0x000290D9
	private void OnDisable()
	{
		this.KeepAliveThroughNextScene = false;
		PlayAudioAndRecycle.activeList.Remove(this);
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x0002AEF0 File Offset: 0x000290F0
	private void Update()
	{
		if (this.skipFrames > 0)
		{
			this.skipFrames--;
			return;
		}
		if (!this.audioSource.isPlaying && (!this.pauseHandler || !this.pauseHandler.IsPaused) && Time.realtimeSinceStartup > this.recycleTime)
		{
			this.Recycle();
		}
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x0002AF4F File Offset: 0x0002914F
	private void NextScene()
	{
		if (this.KeepAliveThroughNextScene)
		{
			this.KeepAliveThroughNextScene = false;
			return;
		}
		this.Recycle();
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0002AF68 File Offset: 0x00029168
	public static void RecycleActiveRecyclers()
	{
		PlayAudioAndRecycle.activeList.ReserveListUsage();
		foreach (PlayAudioAndRecycle playAudioAndRecycle in PlayAudioAndRecycle.activeList.List)
		{
			playAudioAndRecycle.NextScene();
		}
		PlayAudioAndRecycle.activeList.ReleaseListUsage();
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x0002AFD0 File Offset: 0x000291D0
	private void Recycle()
	{
		base.gameObject.Recycle();
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x0002AFDD File Offset: 0x000291DD
	public void SetRecycleTime(float time)
	{
		this.recycleTime = time;
	}

	// Token: 0x040008EF RID: 2287
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040008F0 RID: 2288
	private AudioSourceGamePause pauseHandler;

	// Token: 0x040008F2 RID: 2290
	private static readonly UniqueList<PlayAudioAndRecycle> activeList = new UniqueList<PlayAudioAndRecycle>();

	// Token: 0x040008F3 RID: 2291
	private int skipFrames;

	// Token: 0x040008F4 RID: 2292
	private float recycleTime;
}
