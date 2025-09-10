using System;
using System.Collections;
using System.Runtime.CompilerServices;
using GlobalEnums;
using JetBrains.Annotations;
using TeamCherry.Cinematics;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Video;

// Token: 0x02000071 RID: 113
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(MeshRenderer))]
public class CinematicPlayer : MonoBehaviour, GameManager.ISkippable
{
	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000302 RID: 770 RVA: 0x000102C9 File Offset: 0x0000E4C9
	// (set) Token: 0x06000303 RID: 771 RVA: 0x000102D1 File Offset: 0x0000E4D1
	public CinematicVideoReference VideoClip
	{
		get
		{
			return this.videoClip;
		}
		set
		{
			this.videoClip = value;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x06000304 RID: 772 RVA: 0x000102DA File Offset: 0x0000E4DA
	public float Duration
	{
		get
		{
			if (!this.videoClip)
			{
				return 0f;
			}
			return this.videoClip.VideoFileLength;
		}
	}

	// Token: 0x06000305 RID: 773 RVA: 0x000102FA File Offset: 0x0000E4FA
	[UsedImplicitly]
	private bool IsInGameVideo()
	{
		return this.videoType == CinematicPlayer.VideoType.InGameVideo;
	}

	// Token: 0x06000306 RID: 774 RVA: 0x00010308 File Offset: 0x0000E508
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
		this.myRenderer = base.GetComponent<MeshRenderer>();
		if (this.videoType == CinematicPlayer.VideoType.InGameVideo || Platform.Current.ExtendedVideoBlankFrames > 0)
		{
			this.myRenderer.enabled = false;
		}
		if (this.actCard)
		{
			this.actCard.gameObject.SetActive(false);
		}
		if (this.activeWhilePlaying)
		{
			this.activeWhilePlaying.SetActive(false);
		}
	}

	// Token: 0x06000307 RID: 775 RVA: 0x00010386 File Offset: 0x0000E586
	private void OnEnable()
	{
		ForceCameraAspect.ViewportAspectChanged += this.OnCameraAspectChanged;
		if (this.gm)
		{
			this.gm.RegisterSkippable(this);
		}
	}

	// Token: 0x06000308 RID: 776 RVA: 0x000103B2 File Offset: 0x0000E5B2
	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
		CameraRenderScaled.RemoveForceFullResolution(this);
		if (this.gm)
		{
			this.gm.DeregisterSkippable(this);
		}
	}

	// Token: 0x06000309 RID: 777 RVA: 0x000103E4 File Offset: 0x0000E5E4
	protected void OnDestroy()
	{
		if (this.cinematicVideoPlayer != null)
		{
			this.cinematicVideoPlayer.Dispose();
			this.cinematicVideoPlayer = null;
		}
		this.RestoreScreen();
		this.RestoreVsync();
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0001040C File Offset: 0x0000E60C
	private void Start()
	{
		this.gm = GameManager.instance;
		this.ui = UIManager.instance;
		this.pd = PlayerData.instance;
		if (this.playTrigger != CinematicPlayer.MovieTrigger.ManualTrigger)
		{
			base.StartCoroutine(this.StartVideo());
		}
		if (this.gm)
		{
			this.gm.RegisterSkippable(this);
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00010469 File Offset: 0x0000E669
	private void Update()
	{
		if (this.cinematicVideoPlayer != null)
		{
			this.cinematicVideoPlayer.Update();
		}
		if (this.videoIsPlaying)
		{
			this.TestFinish();
			return;
		}
		if (Time.frameCount % 10 == 0)
		{
			this.Update10();
		}
	}

	// Token: 0x0600030C RID: 780 RVA: 0x0001049E File Offset: 0x0000E69E
	private void Update10()
	{
		this.TestFinish();
	}

	// Token: 0x0600030D RID: 781 RVA: 0x000104A8 File Offset: 0x0000E6A8
	private bool TestFinish()
	{
		if ((this.cinematicVideoPlayer == null || (!this.isWaitingToPlay && !this.cinematicVideoPlayer.IsLoading && !this.cinematicVideoPlayer.IsPlaying && !this.isWaiting)) && !this.loadingLevel && this.videoTriggered)
		{
			this.videoIsPlaying = false;
			if (this.videoType == CinematicPlayer.VideoType.InGameVideo)
			{
				if (this.finishRoutine == null)
				{
					this.finishRoutine = base.StartCoroutine(this.FinishInGameVideo());
				}
			}
			else
			{
				this.FinishVideo();
			}
			return true;
		}
		return false;
	}

	// Token: 0x0600030E RID: 782 RVA: 0x0001052C File Offset: 0x0000E72C
	private void OnCameraAspectChanged(float _)
	{
		if (!this.autoScaleToHUDCamera)
		{
			return;
		}
		bool flag = base.gameObject.layer == 5;
		Camera camera = null;
		if (flag)
		{
			camera = GameCameras.instance.hudCamera;
		}
		if (camera == null || !camera.isActiveAndEnabled)
		{
			camera = GameCameras.instance.mainCamera;
		}
		if (!camera)
		{
			return;
		}
		Transform transform = base.transform;
		Transform transform2 = camera.transform;
		Vector3 position = transform.position;
		position.x = transform2.position.x;
		position.y = transform2.position.y;
		transform.position = position;
		float num;
		if (camera.orthographic)
		{
			num = camera.orthographicSize * 2f / 10f;
		}
		else
		{
			tk2dCamera component = camera.GetComponent<tk2dCamera>();
			if (component)
			{
				component.UpdateCameraMatrix();
			}
			float num2 = Mathf.Abs(transform2.position.z - transform.position.z);
			float num3 = 0.017453292f * camera.fieldOfView;
			num = 2f * num2 * Mathf.Tan(num3 / 2f) / 10f;
		}
		float x = num * 1.7777778f;
		Vector3 localScale = new Vector3(x, 1f, num);
		transform.localScale = localScale;
	}

	// Token: 0x0600030F RID: 783 RVA: 0x00010667 File Offset: 0x0000E867
	public IEnumerator Skip()
	{
		if (this.isSkipped)
		{
			yield break;
		}
		if (!this.videoTriggered)
		{
			yield break;
		}
		this.isSkipped = true;
		this.selfBlanker.AlphaSelf = 0f;
		this.selfBlanker.gameObject.SetActive(true);
		this.OnSkip.Invoke();
		for (float elapsed = 0f; elapsed < 0.3f; elapsed += Time.deltaTime)
		{
			float num = elapsed / 0.3f;
			this.selfBlanker.AlphaSelf = num;
			if (this.audioSource)
			{
				this.audioSource.volume = Mathf.Clamp01(1f - num);
			}
			yield return null;
		}
		this.selfBlanker.AlphaSelf = 1f;
		yield return null;
		if (this.cinematicVideoPlayer != null)
		{
			this.cinematicVideoPlayer.Stop();
		}
		if (this.audioSource)
		{
			this.audioSource.Stop();
		}
		if (this.additionalAudio)
		{
			this.additionalAudio.Stop();
		}
		VibrationEmission vibrationEmission = this.emission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		while (!this.TestFinish())
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000310 RID: 784 RVA: 0x00010676 File Offset: 0x0000E876
	public void TriggerStartVideo()
	{
		this.isSkipped = false;
		if (this.playTrigger == CinematicPlayer.MovieTrigger.ManualPlayPreloaded)
		{
			this.allowPlay = true;
			return;
		}
		base.StartCoroutine(this.StartVideo());
	}

	// Token: 0x06000311 RID: 785 RVA: 0x0001069D File Offset: 0x0000E89D
	public void TriggerStopVideo()
	{
		if (this.videoType == CinematicPlayer.VideoType.InGameVideo)
		{
			base.StartCoroutine(this.Skip());
		}
	}

	// Token: 0x06000312 RID: 786 RVA: 0x000106B5 File Offset: 0x0000E8B5
	public void UnlockSkip()
	{
		this.gm.inputHandler.SetSkipMode(this.skipMode);
	}

	// Token: 0x06000313 RID: 787 RVA: 0x000106CD File Offset: 0x0000E8CD
	private IEnumerator StartVideo()
	{
		CinematicPlayer.<>c__DisplayClass71_0 CS$<>8__locals1 = new CinematicPlayer.<>c__DisplayClass71_0();
		CS$<>8__locals1.<>4__this = this;
		this.isSkipped = false;
		if (this.finishRoutine != null)
		{
			base.StopCoroutine(this.finishRoutine);
			this.finishRoutine = null;
		}
		if (this.cinematicVideoPlayer == null)
		{
			this.cinematicVideoPlayer = CinematicVideoPlayer.Create(new CinematicVideoPlayerConfig(this.videoClip, this.myRenderer, this.audioSource, this.faderStyle, GameManager.instance.GetImplicitCinematicVolume()));
			if (this.cinematicVideoPlayer == null)
			{
				this.OnVideoBecameNull();
				yield break;
			}
		}
		bool shouldEnable = false;
		CS$<>8__locals1.shouldDisableBlanker = false;
		if (this.myRenderer)
		{
			shouldEnable = !this.myRenderer.enabled;
		}
		CS$<>8__locals1.unityVideo = this.myRenderer.GetComponent<VideoPlayer>();
		if (CS$<>8__locals1.unityVideo)
		{
			CS$<>8__locals1.videoBlankFrames = (CheatManager.OverrideReadyWaitFrames ? CheatManager.ReadyWaitFrames : Platform.Current.ExtendedVideoBlankFrames);
			CS$<>8__locals1.videoBlankFrames = Mathf.Max(this.prePlayFrames, CS$<>8__locals1.videoBlankFrames);
			CS$<>8__locals1.wasSendingEvent = CS$<>8__locals1.unityVideo.sendFrameReadyEvents;
			CS$<>8__locals1.unityVideo.sendFrameReadyEvents = true;
			CS$<>8__locals1.unityVideo.frameReady += CS$<>8__locals1.<StartVideo>g__UnityVideoOnframeReady|0;
			shouldEnable = false;
		}
		if (CheatManager.OverrideSkipFrameOnDrop)
		{
			this.cinematicVideoPlayer.SkipFrameOnDrop = CheatManager.SkipVideoFrameOnDrop;
		}
		else
		{
			this.cinematicVideoPlayer.SkipFrameOnDrop = false;
		}
		this.cinematicVideoPlayer.IsLooping = this.loopVideo;
		this.cinematicVideoPlayer.CurrentTime = this.startAtTime;
		if (this.playTrigger == CinematicPlayer.MovieTrigger.ManualPlayPreloaded)
		{
			while (!this.allowPlay)
			{
				yield return null;
			}
		}
		if (this.blankerMode == CinematicPlayer.Blanker.DoNotBlank)
		{
			this.selfBlanker.gameObject.SetActive(false);
		}
		else
		{
			this.selfBlanker.AlphaSelf = 1f;
			this.selfBlanker.gameObject.SetActive(true);
		}
		if (this.masterOff != null)
		{
			this.masterOff.TransitionTo(this.masterOffFadeTime);
		}
		this.gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		this.videoTriggered = true;
		this.isWaitingToPlay = true;
		this.OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		if (this.videoType == CinematicPlayer.VideoType.InGameVideo)
		{
			this.gm.SetState(GameState.CUTSCENE);
			if (this.startDelay > 0f)
			{
				this.isWaiting = true;
				yield return new WaitForSeconds(this.startDelay);
				this.isWaiting = false;
			}
			while (this.cinematicVideoPlayer != null && this.cinematicVideoPlayer.IsLoading)
			{
				yield return null;
			}
			if (this.cinematicVideoPlayer == null)
			{
				this.OnVideoBecameNull();
				yield break;
			}
			this.OnPlay.Invoke();
			this.cinematicVideoPlayer.Play();
			this.SetScreenToMatchVideo();
			if (this.additionalAudio != null)
			{
				this.additionalAudio.Play();
			}
			if (this.vibrationDataAsset != null)
			{
				VibrationEmission vibrationEmission = this.emission;
				if (vibrationEmission != null)
				{
					vibrationEmission.Stop();
				}
				this.emission = VibrationManager.PlayVibrationClipOneShot(this.vibrationDataAsset, null, false, "", true);
			}
			int num;
			for (int i = 0; i < this.prePlayFrames; i = num + 1)
			{
				yield return null;
				num = i;
			}
			if (shouldEnable)
			{
				this.myRenderer.enabled = true;
			}
		}
		else if (this.videoType == CinematicPlayer.VideoType.StagTravel)
		{
			GameCameras.instance.OnCinematicBegin();
			if (this.startDelay > 0f)
			{
				this.isWaiting = true;
				yield return new WaitForSeconds(this.startDelay);
				this.isWaiting = false;
			}
			while (this.cinematicVideoPlayer != null && this.cinematicVideoPlayer.IsLoading)
			{
				yield return null;
			}
			if (this.cinematicVideoPlayer == null)
			{
				this.OnVideoBecameNull();
				yield break;
			}
			this.cinematicVideoPlayer.Play();
			if (shouldEnable)
			{
				this.myRenderer.enabled = true;
			}
			base.StartCoroutine(this.WaitForStagFadeOut());
			this.pd.disablePause = true;
		}
		else
		{
			GameCameras.instance.OnCinematicBegin();
			if (this.startDelay > 0f)
			{
				this.isWaiting = true;
				yield return new WaitForSeconds(this.startDelay);
				this.isWaiting = false;
			}
			while (this.cinematicVideoPlayer != null && this.cinematicVideoPlayer.IsLoading)
			{
				yield return null;
			}
			if (this.cinematicVideoPlayer == null)
			{
				this.OnVideoBecameNull();
				yield break;
			}
			this.cinematicVideoPlayer.Play();
			this.SetScreenToMatchVideo();
			if (this.additionalAudio != null)
			{
				this.additionalAudio.Play();
			}
			int num;
			for (int i = 0; i < this.prePlayFrames; i = num + 1)
			{
				yield return null;
				num = i;
			}
			if (shouldEnable)
			{
				this.myRenderer.enabled = true;
			}
		}
		this.isWaitingToPlay = false;
		while (!this.cinematicVideoPlayer.IsPlaying)
		{
			yield return null;
		}
		if (this.blankerMode == CinematicPlayer.Blanker.Default)
		{
			yield return null;
		}
		if (this.activeWhilePlaying)
		{
			this.activeWhilePlaying.SetActive(true);
		}
		EventRegister.SendEvent(EventRegisterEvents.CinematicStart, null);
		float blankTime = this.startBlankTime;
		if (blankTime > 0f)
		{
			while (blankTime > 0f)
			{
				yield return null;
				blankTime -= Time.deltaTime;
			}
		}
		this.videoIsPlaying = true;
		CS$<>8__locals1.shouldDisableBlanker = true;
		if (this.myRenderer.enabled)
		{
			this.selfBlanker.gameObject.SetActive(false);
		}
		if (this.disableGameCam && shouldEnable)
		{
			GameCameras.instance.SetMainCameraActive(false);
		}
		if (!this.startSkipLocked)
		{
			yield return new WaitForSeconds(1f);
			this.gm.inputHandler.SetSkipMode(this.skipMode);
		}
		yield break;
	}

	// Token: 0x06000314 RID: 788 RVA: 0x000106DC File Offset: 0x0000E8DC
	private void OnVideoBecameNull()
	{
		this.isWaitingToPlay = false;
		this.TestFinish();
	}

	// Token: 0x06000315 RID: 789 RVA: 0x000106EC File Offset: 0x0000E8EC
	private void SetScreenToMatchVideo()
	{
		Platform.ResolutionModes resolutionMode = Platform.Current.ResolutionMode;
		this.ApplyVsync();
		if (resolutionMode == Platform.ResolutionModes.Native)
		{
			return;
		}
		this.previousResMode = resolutionMode;
		Platform.Current.ResolutionMode = Platform.ResolutionModes.Native;
		Screen.SetResolution(this.videoClip.VideoFileWidth, this.videoClip.VideoFileHeight, true);
	}

	// Token: 0x06000316 RID: 790 RVA: 0x0001073C File Offset: 0x0000E93C
	private void RestoreScreen()
	{
		this.RestoreVsync();
		if (this.previousResMode == Platform.ResolutionModes.Native)
		{
			return;
		}
		Platform.Current.ResolutionMode = this.previousResMode;
		Platform.Current.RefreshGraphicsTier();
	}

	// Token: 0x06000317 RID: 791 RVA: 0x00010768 File Offset: 0x0000E968
	private void ApplyVsync()
	{
		if (!this.appliedVsync)
		{
			this.appliedVsync = true;
			int targetFrameRate = Mathf.RoundToInt(this.videoClip.VideoFileFrameRate);
			Platform.Current.SetTargetFrameRate(targetFrameRate);
		}
	}

	// Token: 0x06000318 RID: 792 RVA: 0x000107A0 File Offset: 0x0000E9A0
	private void RestoreVsync()
	{
		if (this.appliedVsync)
		{
			this.appliedVsync = false;
			Platform.Current.RestoreFrameRate();
		}
	}

	// Token: 0x06000319 RID: 793 RVA: 0x000107BC File Offset: 0x0000E9BC
	private void FinishVideo()
	{
		this.selfBlanker.AlphaSelf = 0f;
		GameCameras.instance.OnCinematicEnd();
		this.videoTriggered = false;
		this.videoIsPlaying = false;
		if (this.fireQueuedAchievements && this.gm)
		{
			this.ExecuteDelayed(this.achievementDelay, new Action(this.gm.AwardQueuedAchievements));
			this.endDelay = Mathf.Max(this.achievementDelay + 4f, this.endDelay);
		}
		if (this.endDelay > 0f)
		{
			this.ExecuteDelayed(this.endDelay, new Action(this.<FinishVideo>g__End|77_0));
			return;
		}
		this.<FinishVideo>g__End|77_0();
	}

	// Token: 0x0600031A RID: 794 RVA: 0x0001086E File Offset: 0x0000EA6E
	private IEnumerator FinishInGameVideo()
	{
		if (this.activeWhilePlaying)
		{
			this.activeWhilePlaying.SetActive(false);
		}
		this.selfBlanker.AlphaSelf = 0f;
		this.myRenderer.enabled = false;
		this.gm.ui.HideCutscenePrompt(false, null);
		this.RestoreScreen();
		if (this.actCard)
		{
			this.gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
			if (this.actCardDelay > 0f)
			{
				yield return new WaitForSeconds(this.actCardDelay);
			}
			this.selfBlanker.gameObject.SetActive(false);
			CameraRenderScaled.AddForceFullResolution(this);
			this.actCard.gameObject.SetActive(true);
			yield return null;
			yield return new WaitForSeconds(this.actCard.GetCurrentAnimatorStateInfo(0).length);
			CameraRenderScaled.RemoveForceFullResolution(this);
		}
		if (this.disableGameCam)
		{
			GameCameras.instance.SetMainCameraActive(true);
		}
		this.selfBlanker.gameObject.SetActive(false);
		if (this.masterResume != null)
		{
			this.masterResume.TransitionTo(this.masterResumeFadeTime);
		}
		if (!this.additionalAudioContinuesPastVideo && this.additionalAudio != null)
		{
			this.additionalAudio.Stop();
		}
		if (this.cinematicVideoPlayer != null)
		{
			this.cinematicVideoPlayer.Stop();
			this.cinematicVideoPlayer.Dispose();
			this.cinematicVideoPlayer = null;
		}
		VibrationEmission vibrationEmission = this.emission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.emission = null;
		this.videoTriggered = false;
		this.gm.SetState(GameState.PLAYING);
		this.gm.inputHandler.StartAcceptingInput();
		if (this.isSkipped)
		{
			EventRegister.SendEvent(EventRegisterEvents.CinematicSkipped, null);
		}
		EventRegister.SendEvent(EventRegisterEvents.CinematicEnd, null);
		GameCameras.instance.OnCinematicEnd();
		yield break;
	}

	// Token: 0x0600031B RID: 795 RVA: 0x0001087D File Offset: 0x0000EA7D
	private IEnumerator WaitForStagFadeOut()
	{
		yield return new WaitForSeconds(2.6f);
		GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
		yield break;
	}

	// Token: 0x0600031C RID: 796 RVA: 0x00010885 File Offset: 0x0000EA85
	public void SetAdditionalAudio(AudioSource newAudioSource, bool continuesPastVideo)
	{
		this.additionalAudio = newAudioSource;
		this.additionalAudioContinuesPastVideo = continuesPastVideo;
	}

	// Token: 0x0600031E RID: 798 RVA: 0x000108A8 File Offset: 0x0000EAA8
	[CompilerGenerated]
	private void <FinishVideo>g__End|77_0()
	{
		switch (this.videoType)
		{
		case CinematicPlayer.VideoType.OpeningCutscene:
			GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
			this.ui.SetState(UIState.INACTIVE);
			this.loadingLevel = true;
			base.StartCoroutine(this.gm.LoadFirstScene());
			return;
		case CinematicPlayer.VideoType.StagTravel:
			this.ui.SetState(UIState.INACTIVE);
			this.loadingLevel = true;
			this.gm.ChangeToScene(this.pd.nextScene, "door_fastTravelExit", 0f);
			return;
		case CinematicPlayer.VideoType.InGameVideo:
			return;
		case CinematicPlayer.VideoType.OpeningPrologue:
			GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
			this.ui.SetState(UIState.INACTIVE);
			this.loadingLevel = true;
			this.gm.LoadOpeningCinematic();
			return;
		case CinematicPlayer.VideoType.Ending:
			GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
			this.ui.SetState(UIState.INACTIVE);
			this.loadingLevel = true;
			this.gm.LoadScene(this.gm.playerData.bossRushMode ? "GG_End_Sequence" : "End_Credits");
			return;
		case CinematicPlayer.VideoType.EndingE:
			GameCameras.instance.cameraFadeFSM.SendEventSafe("JUST FADE");
			this.ui.SetState(UIState.INACTIVE);
			this.loadingLevel = true;
			this.gm.LoadScene(this.pd.MushroomQuestCompleted ? "Cinematic_MrMushroom" : "End_Credits_Scroll");
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x0400028A RID: 650
	[SerializeField]
	private CinematicVideoReference videoClip;

	// Token: 0x0400028B RID: 651
	[SerializeField]
	private float startAtTime;

	// Token: 0x0400028C RID: 652
	private CinematicVideoPlayer cinematicVideoPlayer;

	// Token: 0x0400028D RID: 653
	[SerializeField]
	private AudioSource additionalAudio;

	// Token: 0x0400028E RID: 654
	[SerializeField]
	private bool additionalAudioContinuesPastVideo;

	// Token: 0x0400028F RID: 655
	[SerializeField]
	private NestedFadeGroupBase selfBlanker;

	// Token: 0x04000290 RID: 656
	[SerializeField]
	private GameObject activeWhilePlaying;

	// Token: 0x04000291 RID: 657
	[SerializeField]
	private bool autoScaleToHUDCamera;

	// Token: 0x04000292 RID: 658
	[Header("Cinematic Settings")]
	[Tooltip("Determines what will trigger the video playing.")]
	public CinematicPlayer.MovieTrigger playTrigger;

	// Token: 0x04000293 RID: 659
	[SerializeField]
	private float startDelay;

	// Token: 0x04000294 RID: 660
	[SerializeField]
	private float endDelay;

	// Token: 0x04000295 RID: 661
	[SerializeField]
	private bool fireQueuedAchievements;

	// Token: 0x04000296 RID: 662
	[SerializeField]
	private float achievementDelay = 1f;

	// Token: 0x04000297 RID: 663
	[Space]
	[Tooltip("Allows the player to skip the video.")]
	public SkipPromptMode skipMode;

	// Token: 0x04000298 RID: 664
	[Tooltip("Prevents the skip action from taking place until the lock is released. Useful for animators delaying skip feature.")]
	public bool startSkipLocked;

	// Token: 0x04000299 RID: 665
	[Tooltip("Video keeps looping until the player is explicitly told to stop.")]
	public bool loopVideo;

	// Token: 0x0400029A RID: 666
	public float startBlankTime;

	// Token: 0x0400029B RID: 667
	public CinematicPlayer.Blanker blankerMode;

	// Token: 0x0400029C RID: 668
	public int prePlayFrames;

	// Token: 0x0400029D RID: 669
	[Space]
	[Tooltip("The name of the scene to load when the video ends. Leaving this blank will load the \"next scene\" as set in PlayerData.")]
	public CinematicPlayer.VideoType videoType;

	// Token: 0x0400029E RID: 670
	public CinematicVideoFaderStyles faderStyle;

	// Token: 0x0400029F RID: 671
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsInGameVideo", true, true, false)]
	private bool disableGameCam;

	// Token: 0x040002A0 RID: 672
	[Space]
	[SerializeField]
	private AudioMixerSnapshot masterOff;

	// Token: 0x040002A1 RID: 673
	[SerializeField]
	private float masterOffFadeTime;

	// Token: 0x040002A2 RID: 674
	[SerializeField]
	private AudioMixerSnapshot masterResume;

	// Token: 0x040002A3 RID: 675
	[SerializeField]
	private float masterResumeFadeTime;

	// Token: 0x040002A4 RID: 676
	[Space]
	[SerializeField]
	private float actCardDelay;

	// Token: 0x040002A5 RID: 677
	[SerializeField]
	private Animator actCard;

	// Token: 0x040002A6 RID: 678
	[Space]
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	// Token: 0x040002A7 RID: 679
	[Space]
	public UnityEvent OnPlay;

	// Token: 0x040002A8 RID: 680
	public UnityEvent OnSkip;

	// Token: 0x040002A9 RID: 681
	private AudioSource audioSource;

	// Token: 0x040002AA RID: 682
	private MeshRenderer myRenderer;

	// Token: 0x040002AB RID: 683
	private GameManager gm;

	// Token: 0x040002AC RID: 684
	private UIManager ui;

	// Token: 0x040002AD RID: 685
	private PlayerData pd;

	// Token: 0x040002AE RID: 686
	private PlayMakerFSM cameraFsm;

	// Token: 0x040002AF RID: 687
	private bool videoTriggered;

	// Token: 0x040002B0 RID: 688
	private bool loadingLevel;

	// Token: 0x040002B1 RID: 689
	private bool isWaiting;

	// Token: 0x040002B2 RID: 690
	private bool allowPlay;

	// Token: 0x040002B3 RID: 691
	private bool hasPlayed;

	// Token: 0x040002B4 RID: 692
	private bool isWaitingToPlay;

	// Token: 0x040002B5 RID: 693
	private Platform.ResolutionModes previousResMode;

	// Token: 0x040002B6 RID: 694
	private Coroutine finishRoutine;

	// Token: 0x040002B7 RID: 695
	private bool appliedVsync;

	// Token: 0x040002B8 RID: 696
	private VibrationEmission emission;

	// Token: 0x040002B9 RID: 697
	private bool videoIsPlaying;

	// Token: 0x040002BA RID: 698
	private bool isSkipped;

	// Token: 0x020013E6 RID: 5094
	public enum MovieTrigger
	{
		// Token: 0x04008116 RID: 33046
		OnStart,
		// Token: 0x04008117 RID: 33047
		ManualTrigger,
		// Token: 0x04008118 RID: 33048
		ManualPlayPreloaded
	}

	// Token: 0x020013E7 RID: 5095
	public enum VideoType
	{
		// Token: 0x0400811A RID: 33050
		OpeningCutscene,
		// Token: 0x0400811B RID: 33051
		StagTravel,
		// Token: 0x0400811C RID: 33052
		InGameVideo,
		// Token: 0x0400811D RID: 33053
		OpeningPrologue,
		// Token: 0x0400811E RID: 33054
		Ending,
		// Token: 0x0400811F RID: 33055
		EndingE
	}

	// Token: 0x020013E8 RID: 5096
	public enum Blanker
	{
		// Token: 0x04008121 RID: 33057
		Default,
		// Token: 0x04008122 RID: 33058
		SkipBlankFrame,
		// Token: 0x04008123 RID: 33059
		DoNotBlank
	}
}
