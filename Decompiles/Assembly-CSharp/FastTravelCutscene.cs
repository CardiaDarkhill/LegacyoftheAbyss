using System;
using System.Collections;
using GlobalEnums;
using TeamCherry.Cinematics;
using UnityEngine;

// Token: 0x02000084 RID: 132
public class FastTravelCutscene : MonoBehaviour, GameManager.ISkippable
{
	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060003A8 RID: 936 RVA: 0x00012921 File Offset: 0x00010B21
	protected virtual bool IsReadyToActivate
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060003A9 RID: 937 RVA: 0x00012924 File Offset: 0x00010B24
	// (set) Token: 0x060003AA RID: 938 RVA: 0x0001292C File Offset: 0x00010B2C
	private protected bool IsCutsceneComplete { protected get; private set; }

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060003AB RID: 939 RVA: 0x00012935 File Offset: 0x00010B35
	protected virtual bool ShouldFlipX
	{
		get
		{
			return false;
		}
	}

	// Token: 0x060003AC RID: 940 RVA: 0x00012938 File Offset: 0x00010B38
	protected IEnumerator Start()
	{
		this.CheckPlatformOverrides();
		GameManager gm = GameManager.instance;
		gm.RegisterSkippable(this);
		while (gm.IsInSceneTransition)
		{
			yield return null;
		}
		if (StaticVariableList.GetValue<bool>("SkipCutscene", false))
		{
			StaticVariableList.SetValue("SkipCutscene", false, 0);
			this.DoTransition();
			this.OnSkipped();
			this.IsCutsceneComplete = true;
			yield break;
		}
		this.isAsync = global::Platform.Current.FetchScenesBeforeFade;
		this.IsCutsceneComplete = false;
		CinematicVideoReference videoReference = this.GetVideoReference();
		if (videoReference)
		{
			this.cinematicSequence.VideoReference = videoReference;
		}
		this.cinematicSequence.VibrationDataAsset = this.GetVibrationData();
		GameCameras.instance.OnCinematicBegin();
		if (this.ShouldFlipX)
		{
			this.cinematicSequence.FlipScaleX();
		}
		if (!this.isAsync)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
			GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.SKIP_INSTANT);
			if (this.cinematicSequence.HasVideoReference)
			{
				bool flag = false;
				try
				{
					this.cinematicSequence.Begin();
					flag = true;
				}
				catch (Exception)
				{
				}
				if (flag)
				{
					while (this.cinematicSequence.IsPlaying && !this.isSkipFadeComplete)
					{
						yield return null;
					}
				}
			}
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE OUT INSTANT");
			yield return null;
			this.OnFadedOut();
			while (!this.IsReadyToActivate)
			{
				yield return null;
			}
			this.IsCutsceneComplete = true;
			this.DoTransition();
		}
		else
		{
			base.StartCoroutine("FadeInRoutine");
			GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
			if (this.cinematicSequence.HasVideoReference)
			{
				if (this.useLoadPriority)
				{
					this.ApplyStreamingPriority();
				}
				this.cinematicSequence.Begin();
			}
			this.DoTransition();
		}
		yield break;
	}

	// Token: 0x060003AD RID: 941 RVA: 0x00012948 File Offset: 0x00010B48
	private void OnDestroy()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance)
		{
			silentInstance.DeregisterSkippable(this);
		}
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0001296A File Offset: 0x00010B6A
	private void OnValidate()
	{
		if (this.platformLoadSettings == null)
		{
			this.platformLoadSettings = new FastTravelCutscene.PlatformLoadSettings[]
			{
				new FastTravelCutscene.PlatformLoadSettings()
			};
		}
		ArrayForEnumAttribute.EnsureArraySize<FastTravelCutscene.PlatformLoadSettings>(ref this.platformLoadSettings, typeof(FastTravelCutscene.Platform));
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0001299D File Offset: 0x00010B9D
	private void OnDisable()
	{
		this.RestoreLoadingPriority();
	}

	// Token: 0x060003B0 RID: 944 RVA: 0x000129A8 File Offset: 0x00010BA8
	private void DoTransition()
	{
		if (this.hasStartedTransition)
		{
			return;
		}
		this.hasStartedTransition = true;
		FastTravelCutscene.FastTravelAsyncLoadInfo fastTravelAsyncLoadInfo = new FastTravelCutscene.FastTravelAsyncLoadInfo(this)
		{
			EntryGateName = "door_fastTravelExit",
			SceneName = GameManager.instance.playerData.nextScene,
			PreventCameraFadeOut = true,
			Visualization = GameManager.SceneLoadVisualizations.Custom,
			AsyncPriority = -10,
			AlwaysUnloadUnusedAssets = true
		};
		if (CheatManager.OverrideAsyncLoadPriority)
		{
			fastTravelAsyncLoadInfo.AsyncPriority = CheatManager.AsyncLoadPriority;
		}
		else if (this.hasPlatformSettings && this.loadSettings.overrideAsyncLoadPriority)
		{
			fastTravelAsyncLoadInfo.AsyncPriority = this.loadSettings.asyncLoadPriority;
		}
		GameManager.instance.BeginSceneTransition(fastTravelAsyncLoadInfo);
	}

	// Token: 0x060003B1 RID: 945 RVA: 0x00012A4E File Offset: 0x00010C4E
	private IEnumerator FadeInRoutine()
	{
		GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE OUT INSTANT");
		yield return new WaitForSeconds(1.5f);
		GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
		yield break;
	}

	// Token: 0x060003B2 RID: 946 RVA: 0x00012A58 File Offset: 0x00010C58
	protected void Update()
	{
		if (!this.cinematicSequence.DidPlay)
		{
			return;
		}
		CinematicVideoPlayer videoPlayer = this.cinematicSequence.VideoPlayer;
		if (this.isAsync && !this.isSkipping && this.isFetchComplete && (videoPlayer == null || videoPlayer.CurrentTime >= this.cinematicSequence.VideoLength))
		{
			base.StartCoroutine(this.Skip());
		}
	}

	// Token: 0x060003B3 RID: 947 RVA: 0x00012ABA File Offset: 0x00010CBA
	protected void NotifyFetchComplete()
	{
		this.isFetchComplete = true;
		GameManager.instance.inputHandler.SetSkipMode(SkipPromptMode.SKIP_INSTANT);
	}

	// Token: 0x060003B4 RID: 948 RVA: 0x00012AD3 File Offset: 0x00010CD3
	public IEnumerator Skip()
	{
		if (this.isSkipping)
		{
			yield break;
		}
		base.StopCoroutine("FadeInRoutine");
		this.isSkipping = true;
		GameCameras.instance.cameraFadeFSM.SendEventSafe("CINEMATIC SKIP FADE");
		yield return new WaitForSecondsRealtime(0.3f);
		this.RestoreLoadingPriority();
		this.isSkipFadeComplete = true;
		this.IsCutsceneComplete = true;
		this.OnSkipped();
		yield break;
	}

	// Token: 0x060003B5 RID: 949 RVA: 0x00012AE2 File Offset: 0x00010CE2
	protected virtual CinematicVideoReference GetVideoReference()
	{
		return null;
	}

	// Token: 0x060003B6 RID: 950 RVA: 0x00012AE5 File Offset: 0x00010CE5
	protected virtual void OnFadedOut()
	{
	}

	// Token: 0x060003B7 RID: 951 RVA: 0x00012AE7 File Offset: 0x00010CE7
	protected virtual void OnSkipped()
	{
	}

	// Token: 0x060003B8 RID: 952 RVA: 0x00012AE9 File Offset: 0x00010CE9
	protected virtual VibrationDataAsset GetVibrationData()
	{
		return null;
	}

	// Token: 0x060003B9 RID: 953 RVA: 0x00012AEC File Offset: 0x00010CEC
	private void ApplyStreamingPriority()
	{
		if (this.appliedThreadPriority)
		{
			return;
		}
		this.appliedThreadPriority = true;
		this.oldThreadPriority = Application.backgroundLoadingPriority;
		if (CheatManager.OverrideFastTravelBackgroundLoadPriority)
		{
			global::Platform.Current.SetBackgroundLoadingPriority(CheatManager.BackgroundLoadPriority);
			return;
		}
		global::Platform.Current.SetBackgroundLoadingPriority(this.backgroundLoadPriority);
	}

	// Token: 0x060003BA RID: 954 RVA: 0x00012B3B File Offset: 0x00010D3B
	private void RestoreLoadingPriority()
	{
		if (!this.appliedThreadPriority)
		{
			return;
		}
		this.appliedThreadPriority = false;
		global::Platform.Current.RestoreBackgroundLoadingPriority();
	}

	// Token: 0x060003BB RID: 955 RVA: 0x00012B57 File Offset: 0x00010D57
	private void CheckPlatformOverrides()
	{
	}

	// Token: 0x0400034D RID: 845
	[SerializeField]
	private CinematicSequence cinematicSequence;

	// Token: 0x0400034E RID: 846
	[Header("Load Priority")]
	[SerializeField]
	private bool useLoadPriority;

	// Token: 0x0400034F RID: 847
	[SerializeField]
	private ThreadPriority backgroundLoadPriority = ThreadPriority.BelowNormal;

	// Token: 0x04000350 RID: 848
	[Header("Platform Specific")]
	[SerializeField]
	[ArrayForEnum(typeof(FastTravelCutscene.Platform))]
	private FastTravelCutscene.PlatformLoadSettings[] platformLoadSettings = new FastTravelCutscene.PlatformLoadSettings[0];

	// Token: 0x04000351 RID: 849
	private bool isAsync;

	// Token: 0x04000352 RID: 850
	private bool isFetchComplete;

	// Token: 0x04000353 RID: 851
	private bool isSkipping;

	// Token: 0x04000354 RID: 852
	private bool isSkipFadeComplete;

	// Token: 0x04000356 RID: 854
	private ThreadPriority oldThreadPriority;

	// Token: 0x04000357 RID: 855
	private bool appliedThreadPriority;

	// Token: 0x04000358 RID: 856
	private bool hasPlatformSettings;

	// Token: 0x04000359 RID: 857
	private FastTravelCutscene.PlatformLoadSettings loadSettings;

	// Token: 0x0400035A RID: 858
	private bool hasStartedTransition;

	// Token: 0x020013F8 RID: 5112
	private class FastTravelAsyncLoadInfo : GameManager.SceneLoadInfo
	{
		// Token: 0x06008212 RID: 33298 RVA: 0x00263CDC File Offset: 0x00261EDC
		public FastTravelAsyncLoadInfo(FastTravelCutscene fastTravel)
		{
			this.fastTravel = fastTravel;
		}

		// Token: 0x06008213 RID: 33299 RVA: 0x00263CEB File Offset: 0x00261EEB
		public override void NotifyFetchComplete()
		{
			base.NotifyFetchComplete();
			this.fastTravel.NotifyFetchComplete();
		}

		// Token: 0x06008214 RID: 33300 RVA: 0x00263CFE File Offset: 0x00261EFE
		public override bool IsReadyToActivate()
		{
			return base.IsReadyToActivate() && this.fastTravel.IsReadyToActivate && this.fastTravel.IsCutsceneComplete;
		}

		// Token: 0x06008215 RID: 33301 RVA: 0x00263D22 File Offset: 0x00261F22
		public override void NotifyFinished()
		{
			base.NotifyFinished();
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE OUT INSTANT");
			GameCameras.instance.OnCinematicEnd();
		}

		// Token: 0x04008161 RID: 33121
		private readonly FastTravelCutscene fastTravel;
	}

	// Token: 0x020013F9 RID: 5113
	[Serializable]
	private class PlatformLoadSettings
	{
		// Token: 0x04008162 RID: 33122
		public bool overrideBackgroundLoadPriority;

		// Token: 0x04008163 RID: 33123
		public ThreadPriority backgroundLoadPriority;

		// Token: 0x04008164 RID: 33124
		[Space]
		public bool overrideAsyncLoadPriority;

		// Token: 0x04008165 RID: 33125
		public int asyncLoadPriority = 100;
	}

	// Token: 0x020013FA RID: 5114
	[Serializable]
	private enum Platform
	{
		// Token: 0x04008167 RID: 33127
		XBoxOne
	}
}
