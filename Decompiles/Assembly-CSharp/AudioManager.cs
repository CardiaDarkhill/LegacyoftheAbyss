using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

// Token: 0x02000109 RID: 265
public class AudioManager : ManagerSingleton<AudioManager>
{
	// Token: 0x170000AF RID: 175
	// (get) Token: 0x0600083C RID: 2108 RVA: 0x00026EDC File Offset: 0x000250DC
	public AudioSource[] MusicSources
	{
		get
		{
			return this.musicSources;
		}
	}

	// Token: 0x170000B0 RID: 176
	// (get) Token: 0x0600083D RID: 2109 RVA: 0x00026EE4 File Offset: 0x000250E4
	// (set) Token: 0x0600083E RID: 2110 RVA: 0x00026EEB File Offset: 0x000250EB
	public static bool BlockAudioChange { get; set; }

	// Token: 0x170000B1 RID: 177
	// (get) Token: 0x0600083F RID: 2111 RVA: 0x00026EF3 File Offset: 0x000250F3
	// (set) Token: 0x06000840 RID: 2112 RVA: 0x00026EFB File Offset: 0x000250FB
	public AtmosCue CurrentAtmosCue
	{
		get
		{
			return this.currentAtmosCue;
		}
		private set
		{
			this.SetSoundCue<AtmosCue>(ref this.currentAtmosCue, value, ref this.currentAtmosCueSceneHandle);
		}
	}

	// Token: 0x170000B2 RID: 178
	// (get) Token: 0x06000841 RID: 2113 RVA: 0x00026F10 File Offset: 0x00025110
	// (set) Token: 0x06000842 RID: 2114 RVA: 0x00026F18 File Offset: 0x00025118
	public MusicCue CurrentMusicCue
	{
		get
		{
			return this.currentMusicCue;
		}
		private set
		{
			this.SetSoundCue<MusicCue>(ref this.currentMusicCue, value, ref this.currentMusicCueSceneHandle);
		}
	}

	// Token: 0x06000843 RID: 2115 RVA: 0x00026F2D File Offset: 0x0002512D
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<AudioSource>(ref this.atmosSources, typeof(AtmosChannels));
		ArrayForEnumAttribute.EnsureArraySize<AudioSource>(ref this.musicSources, typeof(MusicChannels));
	}

	// Token: 0x06000844 RID: 2116 RVA: 0x00026F59 File Offset: 0x00025159
	protected override void Awake()
	{
		this.OnValidate();
		base.Awake();
	}

	// Token: 0x06000845 RID: 2117 RVA: 0x00026F67 File Offset: 0x00025167
	private void OnEnable()
	{
		this.cameraCtrl = GameCameras.instance.cameraController;
		this.cameraCtrl.PositionedAtHero += this.OnCameraPositionedAtHero;
	}

	// Token: 0x06000846 RID: 2118 RVA: 0x00026F90 File Offset: 0x00025190
	private void OnDisable()
	{
		if (this.cameraCtrl)
		{
			this.cameraCtrl.PositionedAtHero -= this.OnCameraPositionedAtHero;
			this.cameraCtrl = null;
		}
	}

	// Token: 0x06000847 RID: 2119 RVA: 0x00026FC0 File Offset: 0x000251C0
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (this.CurrentAtmosCue != null)
		{
			this.CurrentAtmosCue = null;
		}
		if (this.CurrentMusicCue != null)
		{
			this.CurrentMusicCue = null;
		}
		if (ManagerSingleton<AudioManager>.UnsafeInstance == this)
		{
			PersistentAudioManager.ClearAndReset();
		}
		HeroController.OnHeroInstanceSet -= this.OnSetHeroInstance;
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x00027020 File Offset: 0x00025220
	private void OnCameraPositionedAtHero()
	{
		this.waitingForCameraPosition = false;
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x00027029 File Offset: 0x00025229
	private void Start()
	{
		this.gm = GameManager.instance;
		this.hc = HeroController.SilentInstance;
		HeroController.OnHeroInstanceSet += this.OnSetHeroInstance;
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x00027054 File Offset: 0x00025254
	private void Update()
	{
		PersistentAudioManager.Update();
		if (this.hc && !this.hc.isHeroInPosition)
		{
			this.waitingForCameraPosition = true;
			return;
		}
		this.UpdateSnapshotCallback();
		if (!this.camera)
		{
			return;
		}
		if (this.waitingForAtmosBlock && Time.timeAsDouble >= this.markerBlendUpdateBlockedAtmos)
		{
			this.waitingForAtmosBlock = false;
			this.forceUpdate = true;
		}
		if (this.waitingForMusicBlock && Time.timeAsDouble >= this.markerBlendUpdateBlockedMusic)
		{
			this.waitingForMusicBlock = false;
			this.forceUpdate = true;
		}
		if (!this.ShouldUpdate() && Vector2.SqrMagnitude(this.camera.position - this.previousCameraPosition) <= 1f)
		{
			return;
		}
		if (this.gm.IsInSceneTransition)
		{
			return;
		}
		if (this.waitingForCameraPosition)
		{
			return;
		}
		this.forceUpdate = false;
		this.previousCameraPosition = this.camera.position;
		if (this.atmosMarkers.Count > 0)
		{
			if (Time.timeAsDouble >= this.markerBlendUpdateBlockedAtmos)
			{
				this.TransitionToCurrentAtmos(0f);
			}
			else
			{
				this.waitingForAtmosBlock = true;
			}
		}
		if (this.musicMarkers.Count > 0 || this.forceMusicUpdate)
		{
			if (Time.timeAsDouble >= this.markerBlendUpdateBlockedMusic)
			{
				this.TransitionToCurrentMusic(0f);
				return;
			}
			this.waitingForMusicBlock = true;
		}
	}

	// Token: 0x0600084B RID: 2123 RVA: 0x000271AB File Offset: 0x000253AB
	private bool ShouldUpdate()
	{
		return this.forceUpdate || this.forceMusicUpdate;
	}

	// Token: 0x0600084C RID: 2124 RVA: 0x000271C0 File Offset: 0x000253C0
	private void UpdateSnapshotCallback()
	{
		if (AudioManager.actorCallbackWaitFrames > 0)
		{
			if (AudioManager.waitForCustomSceneManager > 0f)
			{
				AudioManager.waitForCustomSceneManager -= Time.deltaTime;
				return;
			}
			if (this.gm && this.gm.HasFinishedEnteringScene)
			{
				AudioManager.actorCallbackWaitFrames--;
				if (AudioManager.actorCallbackWaitFrames <= 0)
				{
					AudioManager.RunActorSnapshotCallback();
				}
			}
		}
	}

	// Token: 0x0600084D RID: 2125 RVA: 0x00027225 File Offset: 0x00025425
	private void OnSetHeroInstance(HeroController controller)
	{
		this.hc = controller;
	}

	// Token: 0x0600084E RID: 2126 RVA: 0x0002722E File Offset: 0x0002542E
	public void ApplyAtmosCue(AtmosCue atmosCue, float transitionTime)
	{
		this.ApplyAtmosCue(atmosCue, transitionTime, false);
	}

	// Token: 0x0600084F RID: 2127 RVA: 0x00027239 File Offset: 0x00025439
	public void ApplyAtmosCue(AtmosCue atmosCue, float transitionTime, bool markWaitForAtmos)
	{
		atmosCue = (atmosCue ? atmosCue.ResolveAlternatives() : null);
		if (atmosCue == null)
		{
			return;
		}
		if (this.currentAtmosCue == atmosCue)
		{
			return;
		}
		if (markWaitForAtmos)
		{
			this.isWaitingForAtmosCue = true;
		}
		this.BeginApplyAtmosCue(atmosCue, transitionTime);
	}

	// Token: 0x06000850 RID: 2128 RVA: 0x0002727C File Offset: 0x0002547C
	public void StopAndClearMusic()
	{
		this.CurrentMusicCue = null;
		for (int i = 0; i < this.musicSources.Length; i++)
		{
			AudioSource audioSource = this.musicSources[i];
			audioSource.Stop();
			audioSource.clip = null;
		}
	}

	// Token: 0x06000851 RID: 2129 RVA: 0x000272B8 File Offset: 0x000254B8
	public void StopAndClearAtmos()
	{
		this.CurrentAtmosCue = null;
		for (int i = 0; i < this.atmosSources.Length; i++)
		{
			AudioSource audioSource = this.atmosSources[i];
			audioSource.Stop();
			audioSource.clip = null;
		}
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x000272F3 File Offset: 0x000254F3
	private void OnFinishedWaitingForAtmosCue()
	{
		if (this.isWaitingForAtmosCue)
		{
			this.isWaitingForAtmosCue = false;
			if (this.hasQueuedAtmosOverride)
			{
				this.hasQueuedAtmosOverride = false;
				AudioManager.TransitionToAtmosOverride(this.queuedAtmosOverrideSnapshot, this.queuedAtmosTransitionTime);
				this.queuedAtmosOverrideSnapshot = null;
			}
		}
	}

	// Token: 0x06000853 RID: 2131 RVA: 0x0002732B File Offset: 0x0002552B
	public void ClearAtmosOverrides()
	{
		this.atmosSnapshotOverride = null;
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x00027334 File Offset: 0x00025534
	private void BeginApplyAtmosCue(AtmosCue atmosCue, float transitionTime)
	{
		this.CurrentAtmosCue = atmosCue;
		if (atmosCue == null)
		{
			this.OnFinishedWaitingForAtmosCue();
			return;
		}
		if (atmosCue.Snapshot)
		{
			this.atmosSnapshotOverride = null;
		}
		for (int i = 0; i < this.atmosSources.Length; i++)
		{
			AtmosCue.AtmosChannelInfo channelInfo = atmosCue.GetChannelInfo((AtmosChannels)i);
			AudioSource audioSource = this.atmosSources[i];
			if (channelInfo == null || !channelInfo.IsEnabled)
			{
				if (audioSource.isPlaying)
				{
					audioSource.Stop();
					audioSource.clip = null;
				}
			}
			else
			{
				if (audioSource.clip != channelInfo.Clip)
				{
					audioSource.Stop();
				}
				audioSource.clip = channelInfo.Clip;
				if (!audioSource.isPlaying)
				{
					if (channelInfo.Clip)
					{
						float time = Random.Range(0f, channelInfo.Clip.length);
						audioSource.time = time;
					}
					audioSource.Play();
				}
			}
		}
		this.TransitionToCurrentAtmos(transitionTime);
		this.markerBlendUpdateBlockedAtmos = Time.timeAsDouble + (double)transitionTime;
		this.OnFinishedWaitingForAtmosCue();
	}

	// Token: 0x06000855 RID: 2133 RVA: 0x00027430 File Offset: 0x00025630
	private void TransitionToCurrentAtmos(float transitionTime)
	{
		this.TransitionToCurrentSnapshots(this.atmosMixer, transitionTime, this.atmosMarkers, this.currentAtmosCue ? this.currentAtmosCue.Snapshot : null, this.atmosSnapshotOverride, this.atmosSnapshots, this.atmosWeights, ref this.previousAtmosMarkerTime);
	}

	// Token: 0x06000856 RID: 2134 RVA: 0x00027484 File Offset: 0x00025684
	private void TransitionToCurrentMusic(float transitionTime)
	{
		this.forceMusicUpdate = false;
		this.TransitionToCurrentSnapshots(this.musicMixer, transitionTime, this.musicMarkers, this.currentMusicSnapshot, null, this.musicSnapshots, this.musicWeights, ref this.previousMusicMarkerTime);
	}

	// Token: 0x06000857 RID: 2135 RVA: 0x000274C4 File Offset: 0x000256C4
	private void TransitionToCurrentSnapshots(AudioMixer mixer, float transitionTime, IEnumerable<SnapshotMarker> markers, AudioMixerSnapshot cueSnapshot, AudioMixerSnapshot snapshotOverride, AudioMixerSnapshot[] snapshots, float[] weights, ref float previousMarkerTime)
	{
		float num = 0f;
		SnapshotMarker snapshotMarker = null;
		foreach (SnapshotMarker snapshotMarker2 in markers)
		{
			float blendAmount = snapshotMarker2.GetBlendAmount();
			if (blendAmount > num)
			{
				num = blendAmount;
				snapshotMarker = snapshotMarker2;
				previousMarkerTime = snapshotMarker2.TransitionTime;
			}
		}
		bool flag = cueSnapshot;
		bool flag2 = flag || snapshotOverride;
		bool flag3 = snapshotMarker != null && snapshotMarker.Snapshot;
		if (transitionTime <= Mathf.Epsilon)
		{
			transitionTime = previousMarkerTime;
		}
		if (num >= 1f)
		{
			flag2 = false;
		}
		if (flag2 && flag3)
		{
			snapshots[0] = (flag ? cueSnapshot : snapshotOverride);
			weights[0] = 1f - num;
			snapshots[1] = snapshotMarker.Snapshot;
			weights[1] = num;
			mixer.TransitionToSnapshots(snapshots, weights, transitionTime);
			return;
		}
		if (flag2)
		{
			(flag ? cueSnapshot : snapshotOverride).TransitionTo(transitionTime);
			return;
		}
		if (flag3)
		{
			snapshotMarker.Snapshot.TransitionTo(transitionTime);
		}
	}

	// Token: 0x06000858 RID: 2136 RVA: 0x000275D4 File Offset: 0x000257D4
	public void ApplyMusicCue(MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot)
	{
		if (AudioManager.BlockAudioChange)
		{
			return;
		}
		musicCue = (musicCue ? musicCue.ResolveAlternatives() : null);
		if (musicCue == null)
		{
			return;
		}
		if (this.CurrentMusicCue == musicCue)
		{
			this.CurrentMusicCue = musicCue;
			return;
		}
		if (this.applyMusicCueRoutine != null)
		{
			base.StopCoroutine(this.applyMusicCueRoutine);
			this.applyMusicCueRoutine = null;
		}
		this.applyMusicCueRoutine = base.StartCoroutine(this.BeginApplyMusicCue(musicCue, delayTime));
	}

	// Token: 0x06000859 RID: 2137 RVA: 0x0002764B File Offset: 0x0002584B
	private IEnumerator BeginApplyMusicCue(MusicCue musicCue, float delayTime)
	{
		this.CurrentMusicCue = musicCue;
		yield return new WaitForSecondsRealtime(delayTime);
		foreach (AudioSource audioSource in this.musicSources)
		{
			audioSource.Stop();
			audioSource.clip = null;
		}
		double time = AudioSettings.dspTime + 0.1;
		for (int j = 0; j < this.musicSources.Length; j++)
		{
			AudioSource audioSource2 = this.musicSources[j];
			MusicCue.MusicChannelInfo channelInfo = musicCue.GetChannelInfo((MusicChannels)j);
			if (channelInfo != null && channelInfo.IsEnabled)
			{
				audioSource2.clip = channelInfo.Clip;
				audioSource2.time = 0f;
				audioSource2.volume = 1f;
				audioSource2.PlayScheduled(time);
			}
			this.UpdateMusicSync((MusicChannels)j, channelInfo != null && channelInfo.IsSyncRequired);
		}
		yield return new WaitForSeconds(0.1f);
		this.audioLoopMaster.AllowSync();
		yield break;
	}

	// Token: 0x0600085A RID: 2138 RVA: 0x00027668 File Offset: 0x00025868
	private void UpdateMusicSync(MusicChannels musicChannel, bool isSyncRequired)
	{
		switch (musicChannel)
		{
		case MusicChannels.Main:
			return;
		case MusicChannels.MainAlt:
			this.audioLoopMaster.SetSyncMainAlt(isSyncRequired);
			return;
		case MusicChannels.Action:
			this.audioLoopMaster.SetSyncAction(isSyncRequired);
			return;
		case MusicChannels.Sub:
			this.audioLoopMaster.SetSyncSub(isSyncRequired);
			return;
		case MusicChannels.Tension:
			this.audioLoopMaster.SetSyncTension(isSyncRequired);
			return;
		case MusicChannels.Extra:
			this.audioLoopMaster.SetSyncExtra(isSyncRequired);
			return;
		default:
			throw new ArgumentOutOfRangeException("musicChannel", musicChannel, null);
		}
	}

	// Token: 0x0600085B RID: 2139 RVA: 0x000276E8 File Offset: 0x000258E8
	public void ApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime)
	{
		this.ApplyMusicSnapshot(snapshot, delayTime, transitionTime, true);
	}

	// Token: 0x0600085C RID: 2140 RVA: 0x000276F4 File Offset: 0x000258F4
	public void ApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime, bool blockMusicMarker)
	{
		if (this.applyMusicSnapshotRoutine != null)
		{
			base.StopCoroutine(this.applyMusicSnapshotRoutine);
			this.applyMusicSnapshotRoutine = null;
		}
		if (snapshot != null)
		{
			this.applyMusicSnapshotRoutine = base.StartCoroutine(AudioManager.BeginApplyMusicSnapshot(snapshot, delayTime, transitionTime, blockMusicMarker));
		}
	}

	// Token: 0x0600085D RID: 2141 RVA: 0x00027730 File Offset: 0x00025930
	private void SetSoundCue<T>(ref T cueRef, T newCue, ref AsyncOperationHandle<SceneInstance> sceneHandle) where T : ScriptableObject
	{
		if (sceneHandle.IsValid())
		{
			Addressables.ResourceManager.Release(sceneHandle);
			sceneHandle = default(AsyncOperationHandle<SceneInstance>);
		}
		cueRef = newCue;
		if (newCue != null && this.gm != null)
		{
			SceneLoad lastSceneLoad = this.gm.LastSceneLoad;
			if (lastSceneLoad != null && lastSceneLoad.OperationHandle.IsValid())
			{
				sceneHandle = this.gm.LastSceneLoad.OperationHandle;
				Addressables.ResourceManager.Acquire<SceneInstance>(sceneHandle);
			}
		}
	}

	// Token: 0x0600085E RID: 2142 RVA: 0x000277CC File Offset: 0x000259CC
	private static IEnumerator BeginApplyMusicSnapshot(AudioMixerSnapshot snapshot, float delayTime, float transitionTime, bool blockMusicMarker)
	{
		if (delayTime > Mathf.Epsilon)
		{
			yield return new WaitForSecondsRealtime(delayTime);
		}
		if (snapshot != null)
		{
			snapshot.TransitionTo(transitionTime);
		}
		AudioManager instance = ManagerSingleton<AudioManager>.Instance;
		instance.currentMusicSnapshot = snapshot;
		instance.TransitionToCurrentMusic(transitionTime);
		if (blockMusicMarker)
		{
			instance.markerBlendUpdateBlockedMusic = Time.timeAsDouble + (double)transitionTime;
		}
		else
		{
			instance.markerBlendUpdateBlockedMusic = 0.0;
		}
		yield break;
	}

	// Token: 0x0600085F RID: 2143 RVA: 0x000277F0 File Offset: 0x000259F0
	public static void AddAtmosMarker(AtmosSnapshotMarker marker)
	{
		if (!ManagerSingleton<AudioManager>.Instance)
		{
			return;
		}
		ManagerSingleton<AudioManager>.Instance.atmosMarkers.AddIfNotPresent(marker);
		if (ManagerSingleton<AudioManager>.Instance.camera || !GameCameras.instance)
		{
			return;
		}
		ManagerSingleton<AudioManager>.Instance.camera = GameCameras.instance.mainCamera.transform;
		ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
	}

	// Token: 0x06000860 RID: 2144 RVA: 0x0002785D File Offset: 0x00025A5D
	public static void RemoveAtmosMarker(AtmosSnapshotMarker marker)
	{
		if (!ManagerSingleton<AudioManager>.Instance)
		{
			return;
		}
		if (ManagerSingleton<AudioManager>.Instance.atmosMarkers.Remove(marker))
		{
			ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
		}
	}

	// Token: 0x06000861 RID: 2145 RVA: 0x0002788C File Offset: 0x00025A8C
	public static void TransitionToAtmosOverride(AudioMixerSnapshot snapshot, float transitionTime)
	{
		AudioManager instance = ManagerSingleton<AudioManager>.Instance;
		if (!instance)
		{
			return;
		}
		if (instance.isWaitingForAtmosCue)
		{
			instance.hasQueuedAtmosOverride = true;
			instance.queuedAtmosOverrideSnapshot = snapshot;
			instance.queuedAtmosTransitionTime = transitionTime;
			return;
		}
		instance.atmosSnapshotOverride = snapshot;
		instance.TransitionToCurrentAtmos(transitionTime);
		instance.markerBlendUpdateBlockedAtmos = Time.timeAsDouble + (double)transitionTime;
	}

	// Token: 0x06000862 RID: 2146 RVA: 0x000278E4 File Offset: 0x00025AE4
	public static void AddMusicMarker(MusicSnapshotMarker marker)
	{
		if (!ManagerSingleton<AudioManager>.Instance)
		{
			return;
		}
		ManagerSingleton<AudioManager>.Instance.musicMarkers.AddIfNotPresent(marker);
		if (ManagerSingleton<AudioManager>.Instance.camera || !GameCameras.instance)
		{
			return;
		}
		ManagerSingleton<AudioManager>.Instance.camera = GameCameras.instance.mainCamera.transform;
		ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
	}

	// Token: 0x06000863 RID: 2147 RVA: 0x00027951 File Offset: 0x00025B51
	public static void ForceMarkerUpdate()
	{
		ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
	}

	// Token: 0x06000864 RID: 2148 RVA: 0x0002795E File Offset: 0x00025B5E
	public static void RemoveMusicMarker(MusicSnapshotMarker marker)
	{
		if (!ManagerSingleton<AudioManager>.Instance)
		{
			return;
		}
		if (ManagerSingleton<AudioManager>.Instance.musicMarkers.Remove(marker))
		{
			ManagerSingleton<AudioManager>.Instance.forceUpdate = true;
			ManagerSingleton<AudioManager>.Instance.forceMusicUpdate = true;
		}
	}

	// Token: 0x1400000D RID: 13
	// (add) Token: 0x06000865 RID: 2149 RVA: 0x00027998 File Offset: 0x00025B98
	// (remove) Token: 0x06000866 RID: 2150 RVA: 0x000279CC File Offset: 0x00025BCC
	public static event Action OnAppliedActorSnapshot;

	// Token: 0x06000867 RID: 2151 RVA: 0x000279FF File Offset: 0x00025BFF
	public static void PauseActorSnapshot()
	{
		AudioManager.waitingToUnpause = true;
		AudioManager.levelIsReady = false;
		AudioManager.customSceneManagerIsReady = false;
		AudioManager.actorSnapshotCallback = null;
	}

	// Token: 0x06000868 RID: 2152 RVA: 0x00027A1C File Offset: 0x00025C1C
	public static void UnpauseActorSnapshot(Action callback = null)
	{
		if (!AudioManager.waitingToUnpause)
		{
			if (callback != null)
			{
				callback();
			}
			return;
		}
		AudioManager.levelIsReady = true;
		if (AudioManager.customSceneManagerIsReady)
		{
			if (AudioManager.actorSnapshotCallback != null)
			{
				AudioManager.actorSnapshotCallback();
			}
			else if (callback != null)
			{
				callback();
			}
			AudioManager.actorSnapshotCallback = null;
			AudioManager.waitingToUnpause = false;
			AudioManager.actorCallbackWaitFrames = -1;
			AudioManager.OnDidAppliedActorSnapshot();
			return;
		}
		AudioManager.actorSnapshotCallback = callback;
		AudioManager.actorCallbackWaitFrames = 5;
	}

	// Token: 0x06000869 RID: 2153 RVA: 0x00027A88 File Offset: 0x00025C88
	public static void CustomSceneManagerSnapshotReady(Action callback = null)
	{
		if (!AudioManager.waitingToUnpause)
		{
			if (callback != null)
			{
				callback();
			}
			return;
		}
		AudioManager.customSceneManagerIsReady = true;
		if (AudioManager.levelIsReady)
		{
			if (callback != null)
			{
				callback();
			}
			else
			{
				Action action = AudioManager.actorSnapshotCallback;
				if (action != null)
				{
					action();
				}
			}
			AudioManager.actorSnapshotCallback = null;
			AudioManager.waitingToUnpause = false;
			AudioManager.actorCallbackWaitFrames = -1;
			AudioManager.OnDidAppliedActorSnapshot();
			return;
		}
		AudioManager.actorSnapshotCallback = callback;
		AudioManager.actorCallbackWaitFrames = 5;
	}

	// Token: 0x0600086A RID: 2154 RVA: 0x00027AF2 File Offset: 0x00025CF2
	public static void SetIsWaitingForCustomSceneManager()
	{
		AudioManager.waitForCustomSceneManager = 3f;
	}

	// Token: 0x0600086B RID: 2155 RVA: 0x00027AFE File Offset: 0x00025CFE
	public static void CustomSceneManagerReady()
	{
		AudioManager.waitForCustomSceneManager = 0f;
	}

	// Token: 0x0600086C RID: 2156 RVA: 0x00027B0A File Offset: 0x00025D0A
	private static void RunActorSnapshotCallback()
	{
		if (!AudioManager.waitingToUnpause)
		{
			return;
		}
		AudioManager.waitingToUnpause = false;
		Action action = AudioManager.actorSnapshotCallback;
		if (action != null)
		{
			action();
		}
		AudioManager.actorCallbackWaitFrames = -1;
		AudioManager.OnDidAppliedActorSnapshot();
	}

	// Token: 0x0600086D RID: 2157 RVA: 0x00027B35 File Offset: 0x00025D35
	private static void OnDidAppliedActorSnapshot()
	{
		Action onAppliedActorSnapshot = AudioManager.OnAppliedActorSnapshot;
		if (onAppliedActorSnapshot == null)
		{
			return;
		}
		onAppliedActorSnapshot();
	}

	// Token: 0x040007CE RID: 1998
	[SerializeField]
	[ArrayForEnum(typeof(AtmosChannels))]
	private AudioSource[] atmosSources;

	// Token: 0x040007CF RID: 1999
	[SerializeField]
	private AudioMixer atmosMixer;

	// Token: 0x040007D0 RID: 2000
	[Space]
	[SerializeField]
	private AudioLoopMaster audioLoopMaster;

	// Token: 0x040007D1 RID: 2001
	[SerializeField]
	[ArrayForEnum(typeof(MusicChannels))]
	private AudioSource[] musicSources;

	// Token: 0x040007D2 RID: 2002
	[SerializeField]
	private AudioMixer musicMixer;

	// Token: 0x040007D4 RID: 2004
	public AudioSource FanfareEnemyBattleClear;

	// Token: 0x040007D5 RID: 2005
	private AudioMixerSnapshot atmosSnapshotOverride;

	// Token: 0x040007D6 RID: 2006
	private AudioMixerSnapshot currentMusicSnapshot;

	// Token: 0x040007D7 RID: 2007
	private readonly List<AtmosSnapshotMarker> atmosMarkers = new List<AtmosSnapshotMarker>();

	// Token: 0x040007D8 RID: 2008
	private readonly AudioMixerSnapshot[] atmosSnapshots = new AudioMixerSnapshot[2];

	// Token: 0x040007D9 RID: 2009
	private readonly float[] atmosWeights = new float[2];

	// Token: 0x040007DA RID: 2010
	private readonly List<MusicSnapshotMarker> musicMarkers = new List<MusicSnapshotMarker>();

	// Token: 0x040007DB RID: 2011
	private readonly AudioMixerSnapshot[] musicSnapshots = new AudioMixerSnapshot[2];

	// Token: 0x040007DC RID: 2012
	private readonly float[] musicWeights = new float[2];

	// Token: 0x040007DD RID: 2013
	private double markerBlendUpdateBlockedAtmos;

	// Token: 0x040007DE RID: 2014
	private double markerBlendUpdateBlockedMusic;

	// Token: 0x040007DF RID: 2015
	private Transform camera;

	// Token: 0x040007E0 RID: 2016
	private Vector2 previousCameraPosition;

	// Token: 0x040007E1 RID: 2017
	private bool forceUpdate;

	// Token: 0x040007E2 RID: 2018
	private bool forceMusicUpdate;

	// Token: 0x040007E3 RID: 2019
	private bool waitingForCameraPosition;

	// Token: 0x040007E4 RID: 2020
	private GameManager gm;

	// Token: 0x040007E5 RID: 2021
	private HeroController hc;

	// Token: 0x040007E6 RID: 2022
	private CameraController cameraCtrl;

	// Token: 0x040007E7 RID: 2023
	private Coroutine applyMusicCueRoutine;

	// Token: 0x040007E8 RID: 2024
	private Coroutine applyAtmosCueRoutine;

	// Token: 0x040007E9 RID: 2025
	private Coroutine applyMusicSnapshotRoutine;

	// Token: 0x040007EA RID: 2026
	private AtmosCue currentAtmosCue;

	// Token: 0x040007EB RID: 2027
	private AsyncOperationHandle<SceneInstance> currentAtmosCueSceneHandle;

	// Token: 0x040007EC RID: 2028
	private float previousAtmosMarkerTime;

	// Token: 0x040007ED RID: 2029
	private MusicCue currentMusicCue;

	// Token: 0x040007EE RID: 2030
	private AsyncOperationHandle<SceneInstance> currentMusicCueSceneHandle;

	// Token: 0x040007EF RID: 2031
	private float previousMusicMarkerTime;

	// Token: 0x040007F0 RID: 2032
	private bool waitingForAtmosBlock;

	// Token: 0x040007F1 RID: 2033
	private bool waitingForMusicBlock;

	// Token: 0x040007F2 RID: 2034
	private bool isWaitingForAtmosCue;

	// Token: 0x040007F3 RID: 2035
	private bool hasQueuedAtmosOverride;

	// Token: 0x040007F4 RID: 2036
	private AudioMixerSnapshot queuedAtmosOverrideSnapshot;

	// Token: 0x040007F5 RID: 2037
	private float queuedAtmosTransitionTime;

	// Token: 0x040007F7 RID: 2039
	private static bool waitingToUnpause;

	// Token: 0x040007F8 RID: 2040
	private static bool levelIsReady;

	// Token: 0x040007F9 RID: 2041
	private static bool customSceneManagerIsReady;

	// Token: 0x040007FA RID: 2042
	private static float waitForCustomSceneManager;

	// Token: 0x040007FB RID: 2043
	private static int actorCallbackWaitFrames = -1;

	// Token: 0x040007FC RID: 2044
	private const int ACTOR_CALLBACK_WAIT = 5;

	// Token: 0x040007FD RID: 2045
	private const float CUSTOM_SCENE_MANAGER_WAIT_TIME = 3f;

	// Token: 0x040007FE RID: 2046
	private static Action actorSnapshotCallback;
}
