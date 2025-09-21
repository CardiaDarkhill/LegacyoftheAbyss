using System;
using TeamCherry.Cinematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Video;

// Token: 0x02000072 RID: 114
public class CinematicSequence : SkippableSequence
{
	// Token: 0x14000008 RID: 8
	// (add) Token: 0x0600031F RID: 799 RVA: 0x00010A24 File Offset: 0x0000EC24
	// (remove) Token: 0x06000320 RID: 800 RVA: 0x00010A5C File Offset: 0x0000EC5C
	public event Action OnPlaybackEnded;

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000321 RID: 801 RVA: 0x00010A91 File Offset: 0x0000EC91
	// (set) Token: 0x06000322 RID: 802 RVA: 0x00010A99 File Offset: 0x0000EC99
	public CinematicVideoPlayer VideoPlayer { get; private set; }

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000323 RID: 803 RVA: 0x00010AA2 File Offset: 0x0000ECA2
	public override bool IsSkipped
	{
		get
		{
			return this.isSkipped;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x06000324 RID: 804 RVA: 0x00010AAA File Offset: 0x0000ECAA
	public bool HasVideoReference
	{
		get
		{
			return this.videoReference;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x06000325 RID: 805 RVA: 0x00010AB7 File Offset: 0x0000ECB7
	public float VideoLength
	{
		get
		{
			if (!this.videoReference)
			{
				return 0f;
			}
			return this.videoReference.VideoFileLength;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x06000326 RID: 806 RVA: 0x00010AD7 File Offset: 0x0000ECD7
	// (set) Token: 0x06000327 RID: 807 RVA: 0x00010ADF File Offset: 0x0000ECDF
	public bool DidPlay { get; private set; }

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000328 RID: 808 RVA: 0x00010AE8 File Offset: 0x0000ECE8
	// (set) Token: 0x06000329 RID: 809 RVA: 0x00010B00 File Offset: 0x0000ED00
	public bool IsLooping
	{
		get
		{
			CinematicVideoPlayer videoPlayer = this.VideoPlayer;
			if (videoPlayer == null)
			{
				return this.isLooping;
			}
			return videoPlayer.IsLooping;
		}
		set
		{
			if (this.VideoPlayer != null)
			{
				this.VideoPlayer.IsLooping = value;
			}
			this.isLooping = value;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x0600032A RID: 810 RVA: 0x00010B1D File Offset: 0x0000ED1D
	// (set) Token: 0x0600032B RID: 811 RVA: 0x00010B25 File Offset: 0x0000ED25
	public CinematicVideoReference VideoReference
	{
		get
		{
			return this.videoReference;
		}
		set
		{
			this.videoReference = value;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600032C RID: 812 RVA: 0x00010B2E File Offset: 0x0000ED2E
	// (set) Token: 0x0600032D RID: 813 RVA: 0x00010B36 File Offset: 0x0000ED36
	public VibrationDataAsset VibrationDataAsset
	{
		get
		{
			return this.vibrationDataAsset;
		}
		set
		{
			this.vibrationDataAsset = value;
		}
	}

	// Token: 0x0600032E RID: 814 RVA: 0x00010B3F File Offset: 0x0000ED3F
	protected void Awake()
	{
		if (!this.audioSource)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		this.targetRenderer.enabled = this.DidPlay;
		this.fadeByController = 1f;
	}

	// Token: 0x0600032F RID: 815 RVA: 0x00010B76 File Offset: 0x0000ED76
	private void OnEnable()
	{
		ForceCameraAspect.ViewportAspectChanged += this.OnCameraAspectChanged;
	}

	// Token: 0x06000330 RID: 816 RVA: 0x00010B89 File Offset: 0x0000ED89
	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
		this.End();
	}

	// Token: 0x06000331 RID: 817 RVA: 0x00010BA4 File Offset: 0x0000EDA4
	protected void Update()
	{
		if (this.videoStartDelayLeft > 0f)
		{
			this.videoStartDelayLeft -= Time.deltaTime;
			if (this.videoStartDelayLeft > 0f)
			{
				return;
			}
			this.videoStartDelayLeft = 0f;
		}
		if (this.VideoPlayer == null)
		{
			if (this.wasPlaying)
			{
				this.OnEndPlayback();
			}
			return;
		}
		this.framesSinceBegan++;
		this.VideoPlayer.Update();
		if (!this.VideoPlayer.IsLoading && !this.DidPlay)
		{
			this.DidPlay = true;
			if (this.atmosSnapshot != null)
			{
				this.atmosSnapshot.TransitionTo(this.atmosSnapshotTransitionDuration);
			}
			if (CheatManager.OverrideSkipFrameOnDrop)
			{
				this.VideoPlayer.SkipFrameOnDrop = CheatManager.SkipVideoFrameOnDrop;
			}
			else
			{
				this.VideoPlayer.SkipFrameOnDrop = false;
			}
			this.OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
			this.previousResMode = Platform.Current.ResolutionMode;
			if (!this.hasChangedResolution)
			{
				this.hasChangedResolution = true;
				int targetFrameRate = Mathf.RoundToInt(this.VideoReference.VideoFileFrameRate);
				Platform.Current.SetTargetFrameRate(targetFrameRate);
				if (this.previousResMode != Platform.ResolutionModes.Native)
				{
					Platform.Current.ResolutionMode = Platform.ResolutionModes.Native;
					Screen.SetResolution(this.videoReference.VideoFileWidth, this.videoReference.VideoFileHeight, true);
				}
			}
			this.VideoPlayer.Play();
			this.wasPlaying = true;
			this.UpdateBlanker(1f - this.fadeByController);
			UnityEvent onVideoLoaded = this.OnVideoLoaded;
			if (onVideoLoaded != null)
			{
				onVideoLoaded.Invoke();
			}
			VibrationEmission vibrationEmission = this.vibrationEmission;
			if (vibrationEmission != null)
			{
				vibrationEmission.Stop();
			}
			if (this.vibrationDataAsset)
			{
				this.vibrationEmission = VibrationManager.PlayVibrationClipOneShot(this.vibrationDataAsset, null, false, "", true);
			}
		}
		if (!this.VideoPlayer.IsPlaying && !this.VideoPlayer.IsLoading && this.framesSinceBegan >= 10)
		{
			this.targetRenderer.enabled = false;
			this.End();
			return;
		}
		if (this.isSkipQueued)
		{
			if (this.VideoPlayer != null)
			{
				this.VideoPlayer.Stop();
			}
			if (this.audioSource)
			{
				this.audioSource.Stop();
			}
			VibrationEmission vibrationEmission2 = this.vibrationEmission;
			if (vibrationEmission2 != null)
			{
				vibrationEmission2.Stop();
			}
			this.vibrationEmission = null;
			this.isSkipped = true;
			this.OnEndPlayback();
			this.isSkipQueued = false;
		}
	}

	// Token: 0x06000332 RID: 818 RVA: 0x00010E04 File Offset: 0x0000F004
	private void OnCameraAspectChanged(float _)
	{
		if (!this.autoScaleToHUDCamera)
		{
			return;
		}
		if (!this.targetRenderer)
		{
			return;
		}
		bool flag = this.targetRenderer.gameObject.layer == 5;
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
		Transform transform = this.targetRenderer.transform;
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
		Vector3 self = new Vector3(x, 1f, num);
		transform.localScale = self.MultiplyElements(transform.localScale.GetSign());
		if (this.blankerRenderer)
		{
			Transform transform3 = this.blankerRenderer.transform;
			transform3.localPosition = transform.localPosition;
			transform3.localRotation = transform.localRotation;
			transform3.localScale = transform.localScale;
		}
	}

	// Token: 0x06000333 RID: 819 RVA: 0x00010FA2 File Offset: 0x0000F1A2
	private void End()
	{
		if (this.VideoPlayer != null)
		{
			this.VideoPlayer.Dispose();
			this.VideoPlayer = null;
		}
		VibrationEmission vibrationEmission = this.vibrationEmission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.vibrationEmission = null;
		this.OnEndPlayback();
	}

	// Token: 0x06000334 RID: 820 RVA: 0x00010FDC File Offset: 0x0000F1DC
	private void RestoreResolution()
	{
		Platform.Current.RestoreFrameRate();
		if (this.hasChangedResolution)
		{
			this.hasChangedResolution = false;
			if (this.previousResMode != Platform.ResolutionModes.Native)
			{
				Platform.Current.ResolutionMode = this.previousResMode;
				Platform.Current.RefreshGraphicsTier();
			}
		}
	}

	// Token: 0x06000335 RID: 821 RVA: 0x0001101C File Offset: 0x0000F21C
	public override void Begin()
	{
		CinematicSequence.<>c__DisplayClass61_0 CS$<>8__locals1 = new CinematicSequence.<>c__DisplayClass61_0();
		CS$<>8__locals1.<>4__this = this;
		if (this.isStarted)
		{
			return;
		}
		if (this.VideoPlayer != null && this.VideoPlayer.IsPlaying)
		{
			return;
		}
		this.OnCameraAspectChanged(ForceCameraAspect.CurrentViewportAspect);
		if (this.VideoPlayer != null)
		{
			this.VideoPlayer.Dispose();
			this.VideoPlayer = null;
		}
		VibrationEmission vibrationEmission = this.vibrationEmission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.vibrationEmission = null;
		this.VideoPlayer = CinematicVideoPlayer.Create(new CinematicVideoPlayerConfig(this.videoReference, this.targetRenderer, this.audioSource, CinematicVideoFaderStyles.Black, GameManager.instance.GetImplicitCinematicVolume()));
		this.VideoPlayer.IsLooping = this.isLooping;
		this.unityVideoPlayer = this.targetRenderer.GetComponent<VideoPlayer>();
		if (this.unityVideoPlayer)
		{
			CS$<>8__locals1.extendedWaitFrames = (CheatManager.OverrideReadyWaitFrames ? CheatManager.ReadyWaitFrames : Platform.Current.ExtendedVideoBlankFrames);
			CS$<>8__locals1.wasSendingEvent = this.unityVideoPlayer.sendFrameReadyEvents;
			this.unityVideoPlayer.sendFrameReadyEvents = true;
			this.unityVideoPlayer.frameReady += CS$<>8__locals1.<Begin>g__UnityVideoOnframeReady|0;
		}
		this.UpdateBlanker(1f - this.fadeByController);
		this.targetRenderer.enabled = this.DidPlay;
		this.isStarted = true;
		this.isSkipped = false;
		this.isSkipQueued = false;
		this.framesSinceBegan = 0;
		if (this.extraAudio)
		{
			this.extraAudio.Play();
		}
		this.videoStartDelayLeft = this.extraAudioEarlyPadding;
	}

	// Token: 0x06000336 RID: 822 RVA: 0x000111A4 File Offset: 0x0000F3A4
	private void UpdateBlanker(float alpha)
	{
		if (this.VideoPlayer != null)
		{
			this.VideoPlayer.Volume = 1f - Mathf.Clamp01(alpha);
		}
		if (alpha > Mathf.Epsilon)
		{
			if (!this.blankerRenderer.enabled)
			{
				this.blankerRenderer.enabled = true;
			}
			if (this.lastAlpha != alpha)
			{
				this.lastAlpha = alpha;
				this.blankerRenderer.material.color = new Color(0f, 0f, 0f, alpha);
				return;
			}
		}
		else if (this.blankerRenderer.enabled)
		{
			this.blankerRenderer.enabled = false;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000337 RID: 823 RVA: 0x00011240 File Offset: 0x0000F440
	public override bool IsPlaying
	{
		get
		{
			if (this.waitForAudioFinish && this.audioSource && this.audioSource.isPlaying)
			{
				return true;
			}
			bool flag = this.framesSinceBegan < 10 || !this.DidPlay;
			return !this.isSkipped && (flag || (this.VideoPlayer != null && this.VideoPlayer.IsPlaying));
		}
	}

	// Token: 0x06000338 RID: 824 RVA: 0x000112AC File Offset: 0x0000F4AC
	public override void Skip()
	{
		this.isSkipQueued = true;
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000339 RID: 825 RVA: 0x000112B5 File Offset: 0x0000F4B5
	// (set) Token: 0x0600033A RID: 826 RVA: 0x000112BD File Offset: 0x0000F4BD
	public override float FadeByController
	{
		get
		{
			return this.fadeByController;
		}
		set
		{
			this.fadeByController = value;
			this.UpdateBlanker(1f - this.fadeByController);
		}
	}

	// Token: 0x0600033B RID: 827 RVA: 0x000112D8 File Offset: 0x0000F4D8
	public void FlipScaleX()
	{
		if (!this.targetRenderer)
		{
			return;
		}
		this.targetRenderer.transform.FlipLocalScale(true, false, false);
	}

	// Token: 0x0600033C RID: 828 RVA: 0x000112FC File Offset: 0x0000F4FC
	private void OnEndPlayback()
	{
		this.RestoreResolution();
		if (!this.isStarted)
		{
			return;
		}
		this.isStarted = false;
		this.wasPlaying = false;
		Action onPlaybackEnded = this.OnPlaybackEnded;
		if (onPlaybackEnded != null)
		{
			onPlaybackEnded();
		}
		if (this.isSkipped && this.extraAudio)
		{
			this.extraAudio.Stop();
		}
	}

	// Token: 0x040002BB RID: 699
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040002BC RID: 700
	[SerializeField]
	private bool waitForAudioFinish;

	// Token: 0x040002BD RID: 701
	[SerializeField]
	private AudioMixerSnapshot atmosSnapshot;

	// Token: 0x040002BE RID: 702
	[SerializeField]
	private float atmosSnapshotTransitionDuration;

	// Token: 0x040002BF RID: 703
	[SerializeField]
	private CinematicVideoReference videoReference;

	// Token: 0x040002C0 RID: 704
	[SerializeField]
	private VibrationDataAsset vibrationDataAsset;

	// Token: 0x040002C1 RID: 705
	[SerializeField]
	private bool isLooping;

	// Token: 0x040002C2 RID: 706
	[SerializeField]
	private MeshRenderer targetRenderer;

	// Token: 0x040002C3 RID: 707
	[SerializeField]
	private MeshRenderer blankerRenderer;

	// Token: 0x040002C4 RID: 708
	private float fadeRate;

	// Token: 0x040002C5 RID: 709
	private float lastAlpha = -1f;

	// Token: 0x040002C6 RID: 710
	[SerializeField]
	private bool autoScaleToHUDCamera = true;

	// Token: 0x040002C7 RID: 711
	[Space]
	[SerializeField]
	private AudioSource extraAudio;

	// Token: 0x040002C8 RID: 712
	[SerializeField]
	private float extraAudioEarlyPadding;

	// Token: 0x040002C9 RID: 713
	[Space]
	public UnityEvent OnVideoLoaded;

	// Token: 0x040002CC RID: 716
	private VideoPlayer unityVideoPlayer;

	// Token: 0x040002CD RID: 717
	private bool isSkipQueued;

	// Token: 0x040002CE RID: 718
	private bool isSkipped;

	// Token: 0x040002CF RID: 719
	private int startFramesWaited;

	// Token: 0x040002D0 RID: 720
	private int revealFramesWaited;

	// Token: 0x040002D1 RID: 721
	private int framesSinceBegan;

	// Token: 0x040002D2 RID: 722
	private float fadeByController;

	// Token: 0x040002D3 RID: 723
	private Platform.ResolutionModes previousResMode;

	// Token: 0x040002D4 RID: 724
	private VibrationEmission vibrationEmission;

	// Token: 0x040002D5 RID: 725
	private bool hasChangedResolution;

	// Token: 0x040002D6 RID: 726
	private bool wasPlaying;

	// Token: 0x040002D8 RID: 728
	private bool isStarted;

	// Token: 0x040002D9 RID: 729
	private float videoStartDelayLeft;
}
