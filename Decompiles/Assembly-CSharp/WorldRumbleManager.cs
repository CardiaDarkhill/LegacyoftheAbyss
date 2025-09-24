using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000443 RID: 1091
public class WorldRumbleManager : MonoBehaviour
{
	// Token: 0x17000409 RID: 1033
	// (get) Token: 0x06002651 RID: 9809 RVA: 0x000AD7E3 File Offset: 0x000AB9E3
	// (set) Token: 0x06002652 RID: 9810 RVA: 0x000AD7EB File Offset: 0x000AB9EB
	public double WaitStartTime { get; private set; }

	// Token: 0x1700040A RID: 1034
	// (get) Token: 0x06002653 RID: 9811 RVA: 0x000AD7F4 File Offset: 0x000AB9F4
	// (set) Token: 0x06002654 RID: 9812 RVA: 0x000AD7FC File Offset: 0x000AB9FC
	public double WaitEndTime { get; private set; }

	// Token: 0x1700040B RID: 1035
	// (get) Token: 0x06002655 RID: 9813 RVA: 0x000AD805 File Offset: 0x000ABA05
	// (set) Token: 0x06002656 RID: 9814 RVA: 0x000AD80D File Offset: 0x000ABA0D
	public double SoundEndTime { get; private set; }

	// Token: 0x1700040C RID: 1036
	// (get) Token: 0x06002657 RID: 9815 RVA: 0x000AD818 File Offset: 0x000ABA18
	private bool IsRumblesPrevented
	{
		get
		{
			if (this.isRumblesPrevented)
			{
				return true;
			}
			if (CheatManager.IsWorldRumbleDisabled)
			{
				return true;
			}
			if (this.rumblePreventers == null)
			{
				return false;
			}
			using (List<WorldRumbleManager.IWorldRumblePreventer>.Enumerator enumerator = this.rumblePreventers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.AllowRumble)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x06002658 RID: 9816 RVA: 0x000AD890 File Offset: 0x000ABA90
	private void Awake()
	{
		this.initialVolume = this.source.volume;
		this.hasTransitionAudioFader = (this.transitionFader != null);
		if (!this.hasTransitionAudioFader)
		{
			this.transitionFader = base.gameObject.GetComponentInChildren<TransitionAudioFader>();
			this.hasTransitionAudioFader = (this.transitionFader != null);
		}
	}

	// Token: 0x06002659 RID: 9817 RVA: 0x000AD8EC File Offset: 0x000ABAEC
	private void OnEnable()
	{
		WorldRumbleManager.WorldRumbleGroup[] array = this.rumbleGroups;
		for (int i = 0; i < array.Length; i++)
		{
			foreach (WorldRumbleManager.WorldRumble worldRumble in array[i].Rumbles)
			{
				if (worldRumble.ActiveWhileRumbling)
				{
					worldRumble.ActiveWhileRumbling.SetActive(false);
				}
			}
		}
		if (this.hasStarted)
		{
			this.OnSceneStarted();
		}
	}

	// Token: 0x0600265A RID: 9818 RVA: 0x000AD954 File Offset: 0x000ABB54
	private void Start()
	{
		WorldRumbleManager.<>c__DisplayClass38_0 CS$<>8__locals1 = new WorldRumbleManager.<>c__DisplayClass38_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.gm = GameManager.instance;
		if (this.isInScene)
		{
			if (!GameManager.IsWaitingForSceneReady)
			{
				this.OnSceneStarted();
				this.hasStarted = true;
			}
			else
			{
				CS$<>8__locals1.gm.OnBeforeFinishedSceneTransition += CS$<>8__locals1.<Start>g__SceneBeganCallback|0;
			}
			this.OnRumbleCurveEvaluated(0f);
			return;
		}
		if (!GameManager.IsWaitingForSceneReady)
		{
			this.OnSceneStarted();
		}
		this.hasStarted = true;
		CS$<>8__locals1.gm.GamePausedChange += this.OnGamePauseChanged;
		CS$<>8__locals1.gm.OnBeforeFinishedSceneTransition += this.OnSceneStarted;
		CS$<>8__locals1.gm.GameStateChange += this.OnGameStateChanged;
	}

	// Token: 0x0600265B RID: 9819 RVA: 0x000ADA13 File Offset: 0x000ABC13
	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.CancelCurrentRumble();
	}

	// Token: 0x0600265C RID: 9820 RVA: 0x000ADA24 File Offset: 0x000ABC24
	private void OnDestroy()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (this.isInScene || !unsafeInstance)
		{
			return;
		}
		unsafeInstance.GamePausedChange -= this.OnGamePauseChanged;
		unsafeInstance.OnBeforeFinishedSceneTransition -= this.OnSceneStarted;
		unsafeInstance.GameStateChange -= this.OnGameStateChanged;
	}

	// Token: 0x0600265D RID: 9821 RVA: 0x000ADA7E File Offset: 0x000ABC7E
	private void OnGamePauseChanged(bool isPaused)
	{
		if (this.currentRumble == null || this.source == null)
		{
			return;
		}
		if (isPaused)
		{
			this.source.Pause();
			return;
		}
		this.source.UnPause();
	}

	// Token: 0x0600265E RID: 9822 RVA: 0x000ADAB1 File Offset: 0x000ABCB1
	private void OnGameStateChanged(GameState gameState)
	{
		if (gameState != GameState.CUTSCENE)
		{
			return;
		}
		this.isRumblesPrevented = true;
		this.CancelCurrentRumble();
	}

	// Token: 0x0600265F RID: 9823 RVA: 0x000ADAC8 File Offset: 0x000ABCC8
	public void OnSceneStarted()
	{
		this.isRumblesPrevented = false;
		List<WorldRumbleManager.IWorldRumblePreventer> list = this.rumblePreventers;
		if (list != null)
		{
			list.Clear();
		}
		if (!this.source || this.rumbleGroups.Length == 0)
		{
			return;
		}
		this.sceneRumbleGroup = null;
		foreach (WorldRumbleManager.WorldRumbleGroup worldRumbleGroup in this.rumbleGroups)
		{
			if (worldRumbleGroup.Condition.IsFulfilled)
			{
				this.sceneRumbleGroup = worldRumbleGroup;
				break;
			}
		}
		bool flag = this.sceneRumbleGroup != null && this.CanRumbleInScene(this.sceneRumbleGroup);
		HeroController silentInstance = HeroController.SilentInstance;
		if (silentInstance && silentInstance.cState.dead)
		{
			flag = false;
		}
		if (flag)
		{
			if (this.currentRumble != null)
			{
				return;
			}
			this.CancelCurrentRumble();
			base.StopAllCoroutines();
			if (this.currentRumble != null)
			{
				this.currentRumble.CameraShake.CurveEvaluated -= this.OnRumbleCurveEvaluated;
				if (this.shakeCamera)
				{
					this.shakeCamera.CancelShake(this.currentRumble.CameraShake);
				}
			}
			this.currentRumble = null;
		}
		else
		{
			this.DoFadeOutAndEnd(0.25f);
		}
		this.WaitStartTime = 0.0;
		this.WaitEndTime = 0.0;
		this.SoundEndTime = 0.0;
		if (!flag)
		{
			return;
		}
		MinMaxFloat waitTime = this.sceneRumbleGroup.WaitTime;
		if (waitTime.Start < Mathf.Epsilon && waitTime.End < Mathf.Epsilon)
		{
			return;
		}
		base.StartCoroutine(this.DoRumbles());
	}

	// Token: 0x06002660 RID: 9824 RVA: 0x000ADC4F File Offset: 0x000ABE4F
	public void DoRumbleNow()
	{
		if (Time.timeAsDouble < this.SoundEndTime)
		{
			return;
		}
		this.OnSceneStarted();
		this.DoNewRumble();
	}

	// Token: 0x06002661 RID: 9825 RVA: 0x000ADC6C File Offset: 0x000ABE6C
	private IEnumerator DoRumbles()
	{
		yield return new WaitForSeconds(this.GetWaitTime(0f));
		for (;;)
		{
			if (this.IsRumblesPrevented)
			{
				while (this.IsRumblesPrevented)
				{
					yield return null;
				}
				yield return new WaitForSeconds(this.GetWaitTime(0f));
			}
			float seconds = this.DoNewRumble();
			this.hasRumbled = true;
			yield return new WaitForSeconds(seconds);
		}
		yield break;
	}

	// Token: 0x06002662 RID: 9826 RVA: 0x000ADC7C File Offset: 0x000ABE7C
	private void OnRumbleCurveEvaluated(float magnitude)
	{
		if (magnitude <= 0.001f)
		{
			magnitude = 0f;
		}
		if (this.fadeUpGroup)
		{
			this.fadeUpGroup.AlphaSelf = magnitude;
		}
		float speedMultiplier;
		if (this.currentRumble != null)
		{
			speedMultiplier = Mathf.Lerp(1f, this.currentRumble.WindSpeedMultiplier, magnitude);
			if (this.source.loop)
			{
				this.source.volume = this.initialVolume * magnitude * this.GetVolumeMultiplier();
			}
			else
			{
				this.source.volume = this.initialVolume * this.GetVolumeMultiplier();
			}
			if (this.previousT < this.currentRumble.EventThreshold)
			{
				if (magnitude >= this.currentRumble.EventThreshold)
				{
					EventRegister.SendEvent(this.currentRumble.SendEventOver, null);
				}
			}
			else if (magnitude < this.currentRumble.EventThreshold)
			{
				EventRegister.SendEvent(this.currentRumble.SendEventUnder, null);
			}
			if (this.currentRumble.FadeWithRumble)
			{
				this.currentRumble.FadeWithRumble.AlphaSelf = magnitude;
			}
			if (magnitude <= Mathf.Epsilon)
			{
				if (this.currentRumble.ActiveWhileRumbling && this.currentRumble.ActiveWhileRumbling.activeSelf)
				{
					this.currentRumble.ActiveWhileRumbling.SetActive(false);
				}
			}
			else if (this.currentRumble.ActiveWhileRumbling && !this.currentRumble.ActiveWhileRumbling.activeSelf)
			{
				this.currentRumble.ActiveWhileRumbling.SetActive(true);
			}
		}
		else
		{
			speedMultiplier = 1f;
		}
		foreach (UmbrellaWindRegion umbrellaWindRegion in UmbrellaWindRegion.EnumerateActiveRegions())
		{
			umbrellaWindRegion.SpeedMultiplier = speedMultiplier;
		}
		foreach (IdleForceAnimator idleForceAnimator in IdleForceAnimator.EnumerateActiveAnimators())
		{
			idleForceAnimator.SpeedMultiplier = speedMultiplier;
		}
		this.previousT = magnitude;
	}

	// Token: 0x06002663 RID: 9827 RVA: 0x000ADE84 File Offset: 0x000AC084
	private float DoNewRumble()
	{
		this.CancelFadeOut();
		this.CancelCurrentRumble();
		if (this.sceneRumbleGroup != null)
		{
			this.currentRumble = this.sceneRumbleGroup.Rumbles.GetRandomElement<WorldRumbleManager.WorldRumble>();
		}
		if (this.currentRumble == null)
		{
			return this.GetWaitTime(0f);
		}
		if (!string.IsNullOrEmpty(this.sceneRumbleGroup.SendEventToRegister))
		{
			EventRegister.SendEvent(this.sceneRumbleGroup.SendEventToRegister, null);
		}
		this.currentRumble.CameraShake.CurveEvaluated += this.OnRumbleCurveEvaluated;
		AudioClip randomElement = this.currentRumble.Sounds.GetRandomElement<AudioClip>();
		this.source.clip = randomElement;
		float volume = this.source.volume;
		this.source.volume = 0f;
		this.source.Play();
		float length = this.currentRumble.GetLength(randomElement);
		if (this.shakeCamera)
		{
			this.source.volume = this.currentRumble.CameraShake.GetInitialMagnitude() * this.GetVolumeMultiplier();
			this.currentRumble.CameraShake.Setup(length, new Func<float, float>(this.OnProcessMagnitude));
			this.shakeCamera.DoShake(this.currentRumble.CameraShake, this, true, true, true);
		}
		else
		{
			this.source.volume = volume * this.GetVolumeMultiplier();
		}
		if (this.particleController)
		{
			this.particleController.Play(length);
		}
		return this.GetWaitTime(length);
	}

	// Token: 0x06002664 RID: 9828 RVA: 0x000ADFFC File Offset: 0x000AC1FC
	private float OnProcessMagnitude(float magnitude)
	{
		if (this.magnitudeMultipliers == null)
		{
			return magnitude;
		}
		foreach (KeyValuePair<Object, float> keyValuePair in this.magnitudeMultipliers)
		{
			magnitude *= keyValuePair.Value;
		}
		return magnitude;
	}

	// Token: 0x06002665 RID: 9829 RVA: 0x000AE060 File Offset: 0x000AC260
	private float GetWaitTime(float clipTime)
	{
		MinMaxFloat waitTime = this.sceneRumbleGroup.WaitTime;
		float num;
		if (!this.hasRumbled && this.isInScene)
		{
			num = Random.Range(0f, waitTime.End) + clipTime;
		}
		else
		{
			num = waitTime.GetRandomValue() + clipTime;
		}
		this.WaitStartTime = Time.timeAsDouble;
		this.WaitEndTime = this.WaitStartTime + (double)num;
		this.SoundEndTime = Time.timeAsDouble + (double)clipTime;
		return num;
	}

	// Token: 0x06002666 RID: 9830 RVA: 0x000AE0D1 File Offset: 0x000AC2D1
	private void CancelFadeOut()
	{
		if (this.fadeOutRoutine != null)
		{
			base.StopCoroutine(this.fadeOutRoutine);
			this.fadeOutRoutine = null;
		}
	}

	// Token: 0x06002667 RID: 9831 RVA: 0x000AE0F0 File Offset: 0x000AC2F0
	private void CancelCurrentRumble()
	{
		this.OnRumbleCurveEvaluated(0f);
		if (this.currentRumble == null)
		{
			return;
		}
		if (this.shakeCamera)
		{
			this.shakeCamera.CancelShake(this.currentRumble.CameraShake);
		}
		if (this.particleController)
		{
			this.particleController.Stop();
		}
		this.source.Stop();
		this.currentRumble.CameraShake.CurveEvaluated -= this.OnRumbleCurveEvaluated;
		this.currentRumble = null;
	}

	// Token: 0x06002668 RID: 9832 RVA: 0x000AE17C File Offset: 0x000AC37C
	private bool CanRumbleInScene(WorldRumbleManager.WorldRumbleGroup group)
	{
		GameManager instance = GameManager.instance;
		switch (instance.sm.WorldRumble)
		{
		case CustomSceneManager.WorldRumbleSettings.MapZone:
		{
			MapZone currentMapZoneEnum = instance.GetCurrentMapZoneEnum();
			if (!group.MapZoneMask.IsBitSet((int)currentMapZoneEnum))
			{
				return false;
			}
			if (instance.gameMap != null)
			{
				bool result;
				instance.gameMap.HasMapForScene(instance.GetSceneNameString(), out result);
				return result;
			}
			return false;
		}
		case CustomSceneManager.WorldRumbleSettings.Enabled:
			return true;
		case CustomSceneManager.WorldRumbleSettings.Disabled:
			return false;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x06002669 RID: 9833 RVA: 0x000AE1F5 File Offset: 0x000AC3F5
	public void ForceRumble()
	{
		this.DoNewRumble();
	}

	// Token: 0x0600266A RID: 9834 RVA: 0x000AE1FE File Offset: 0x000AC3FE
	public void PreventRumbles()
	{
		this.isRumblesPrevented = true;
	}

	// Token: 0x0600266B RID: 9835 RVA: 0x000AE207 File Offset: 0x000AC407
	public void AllowRumbles()
	{
		this.isRumblesPrevented = false;
	}

	// Token: 0x0600266C RID: 9836 RVA: 0x000AE210 File Offset: 0x000AC410
	public void AddPreventer(WorldRumbleManager.IWorldRumblePreventer area)
	{
		if (this.rumblePreventers == null)
		{
			this.rumblePreventers = new List<WorldRumbleManager.IWorldRumblePreventer>();
		}
		this.rumblePreventers.AddIfNotPresent(area);
	}

	// Token: 0x0600266D RID: 9837 RVA: 0x000AE232 File Offset: 0x000AC432
	public void RemovePreventer(WorldRumbleManager.IWorldRumblePreventer area)
	{
		List<WorldRumbleManager.IWorldRumblePreventer> list = this.rumblePreventers;
		if (list == null)
		{
			return;
		}
		list.Remove(area);
	}

	// Token: 0x0600266E RID: 9838 RVA: 0x000AE248 File Offset: 0x000AC448
	public void DoFadeOutAndDisable(float fadeTime)
	{
		base.StopAllCoroutines();
		if (this.currentRumble != null)
		{
			this.currentRumble.CameraShake.CurveEvaluated -= this.OnRumbleCurveEvaluated;
			if (this.shakeCamera)
			{
				this.shakeCamera.CancelShake(this.currentRumble.CameraShake);
			}
		}
		this.CancelFadeOut();
		this.fadeOutRoutine = base.StartCoroutine(this.FadeOutAndDisable(fadeTime));
	}

	// Token: 0x0600266F RID: 9839 RVA: 0x000AE2BB File Offset: 0x000AC4BB
	private IEnumerator FadeOutAndDisable(float fadeTime)
	{
		float initialAlpha = this.fadeUpGroup ? this.fadeUpGroup.AlphaSelf : this.previousT;
		for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.deltaTime)
		{
			float magnitude = Mathf.Lerp(initialAlpha, 0f, elapsed / fadeTime);
			this.OnRumbleCurveEvaluated(magnitude);
			yield return null;
		}
		this.OnRumbleCurveEvaluated(0f);
		base.gameObject.SetActive(false);
		this.fadeOutRoutine = null;
		yield break;
	}

	// Token: 0x06002670 RID: 9840 RVA: 0x000AE2D1 File Offset: 0x000AC4D1
	public void DoFadeOutAndEnd(float fadeTime)
	{
		base.StopAllCoroutines();
		if (this.currentRumble != null)
		{
			this.CancelFadeOut();
			this.fadeOutRoutine = base.StartCoroutine(this.FadeOutAndEnd(fadeTime));
		}
	}

	// Token: 0x06002671 RID: 9841 RVA: 0x000AE2FA File Offset: 0x000AC4FA
	private IEnumerator FadeOutAndEnd(float fadeTime)
	{
		float initialAlpha = this.fadeUpGroup ? this.fadeUpGroup.AlphaSelf : this.previousT;
		for (float elapsed = 0f; elapsed < fadeTime; elapsed += Time.deltaTime)
		{
			float magnitude = Mathf.Lerp(initialAlpha, 0f, elapsed / fadeTime);
			this.OnRumbleCurveEvaluated(magnitude);
			yield return null;
		}
		this.OnRumbleCurveEvaluated(0f);
		this.CancelCurrentRumble();
		this.fadeOutRoutine = null;
		yield break;
	}

	// Token: 0x06002672 RID: 9842 RVA: 0x000AE310 File Offset: 0x000AC510
	public void AddMagnitudeMultiplier(Object from, float multiplier)
	{
		if (this.magnitudeMultipliers == null)
		{
			this.magnitudeMultipliers = new Dictionary<Object, float>();
		}
		this.magnitudeMultipliers[from] = multiplier;
	}

	// Token: 0x06002673 RID: 9843 RVA: 0x000AE332 File Offset: 0x000AC532
	public void RemoveMagnitudeMultiplier(Object from)
	{
		Dictionary<Object, float> dictionary = this.magnitudeMultipliers;
		if (dictionary == null)
		{
			return;
		}
		dictionary.Remove(from);
	}

	// Token: 0x06002674 RID: 9844 RVA: 0x000AE346 File Offset: 0x000AC546
	private float GetVolumeMultiplier()
	{
		if (!this.hasTransitionAudioFader)
		{
			return 1f;
		}
		return this.transitionFader.Volume;
	}

	// Token: 0x040023B2 RID: 9138
	[SerializeField]
	private CameraManagerReference shakeCamera;

	// Token: 0x040023B3 RID: 9139
	[SerializeField]
	private AudioSource source;

	// Token: 0x040023B4 RID: 9140
	[SerializeField]
	private TransitionAudioFader transitionFader;

	// Token: 0x040023B5 RID: 9141
	[SerializeField]
	private ParticleEffectsLerpEmission particleController;

	// Token: 0x040023B6 RID: 9142
	[SerializeField]
	private NestedFadeGroupBase fadeUpGroup;

	// Token: 0x040023B7 RID: 9143
	[Space]
	[SerializeField]
	private bool isInScene;

	// Token: 0x040023B8 RID: 9144
	[SerializeField]
	private WorldRumbleManager.WorldRumbleGroup[] rumbleGroups;

	// Token: 0x040023B9 RID: 9145
	private WorldRumbleManager.WorldRumbleGroup sceneRumbleGroup;

	// Token: 0x040023BA RID: 9146
	private bool hasStarted;

	// Token: 0x040023BB RID: 9147
	private bool hasRumbled;

	// Token: 0x040023BC RID: 9148
	private WorldRumbleManager.WorldRumble currentRumble;

	// Token: 0x040023BD RID: 9149
	private float previousT;

	// Token: 0x040023BE RID: 9150
	private bool isRumblesPrevented;

	// Token: 0x040023BF RID: 9151
	private List<WorldRumbleManager.IWorldRumblePreventer> rumblePreventers;

	// Token: 0x040023C0 RID: 9152
	private float initialVolume;

	// Token: 0x040023C1 RID: 9153
	private Dictionary<Object, float> magnitudeMultipliers;

	// Token: 0x040023C5 RID: 9157
	private bool hasTransitionAudioFader;

	// Token: 0x040023C6 RID: 9158
	private Coroutine fadeOutRoutine;

	// Token: 0x02001725 RID: 5925
	[Serializable]
	private class CameraShake : ICameraShake, ICameraShakeVibration
	{
		// Token: 0x14000104 RID: 260
		// (add) Token: 0x06008CCF RID: 36047 RVA: 0x0028811C File Offset: 0x0028631C
		// (remove) Token: 0x06008CD0 RID: 36048 RVA: 0x00288154 File Offset: 0x00286354
		public event Action<float> CurveEvaluated;

		// Token: 0x17000EF6 RID: 3830
		// (get) Token: 0x06008CD1 RID: 36049 RVA: 0x00288189 File Offset: 0x00286389
		public bool CanFinish
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000EF7 RID: 3831
		// (get) Token: 0x06008CD2 RID: 36050 RVA: 0x0028818C File Offset: 0x0028638C
		public bool PersistThroughScenes
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000EF8 RID: 3832
		// (get) Token: 0x06008CD3 RID: 36051 RVA: 0x0028818F File Offset: 0x0028638F
		public float Magnitude
		{
			get
			{
				return this.magnitude;
			}
		}

		// Token: 0x17000EF9 RID: 3833
		// (get) Token: 0x06008CD4 RID: 36052 RVA: 0x00288197 File Offset: 0x00286397
		public int FreezeFrames
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000EFA RID: 3834
		// (get) Token: 0x06008CD5 RID: 36053 RVA: 0x0028819A File Offset: 0x0028639A
		public ICameraShakeVibration CameraShakeVibration
		{
			get
			{
				return this;
			}
		}

		// Token: 0x17000EFB RID: 3835
		// (get) Token: 0x06008CD6 RID: 36054 RVA: 0x0028819D File Offset: 0x0028639D
		public CameraShakeWorldForceIntensities WorldForceOnStart
		{
			get
			{
				return CameraShakeWorldForceIntensities.None;
			}
		}

		// Token: 0x06008CD7 RID: 36055 RVA: 0x002881A0 File Offset: 0x002863A0
		public Vector2 GetOffset(float elapsedTime)
		{
			if (this.length <= 0f)
			{
				return Vector2.zero;
			}
			float num = this.Curve.Evaluate(elapsedTime / this.length);
			Action<float> curveEvaluated = this.CurveEvaluated;
			if (curveEvaluated != null)
			{
				curveEvaluated(num);
			}
			if (this.CustomUpdateRate > 0f)
			{
				if (elapsedTime >= this.nextUpdateTime)
				{
					this.startOffset = this.targetOffset;
					this.targetOffset = this.GetTargetOffset(num);
					this.startUpdateTime = elapsedTime;
					this.nextUpdateTime = elapsedTime + 1f / this.CustomUpdateRate;
				}
				float num2 = this.nextUpdateTime - this.startUpdateTime;
				float num3 = elapsedTime - this.startUpdateTime;
				return Vector2.Lerp(this.startOffset, this.targetOffset, num3 / num2);
			}
			return this.GetTargetOffset(num);
		}

		// Token: 0x06008CD8 RID: 36056 RVA: 0x00288264 File Offset: 0x00286464
		private Vector2 GetTargetOffset(float t)
		{
			float num = this.processMagnitude(this.magnitude);
			return Random.insideUnitCircle * (num * t);
		}

		// Token: 0x06008CD9 RID: 36057 RVA: 0x00288290 File Offset: 0x00286490
		public bool IsDone(float elapsedTime)
		{
			return elapsedTime >= this.length;
		}

		// Token: 0x06008CDA RID: 36058 RVA: 0x002882A0 File Offset: 0x002864A0
		public void Setup(float newLength, Func<float, float> onProcessMagnitude)
		{
			this.length = newLength;
			if (this.length > 0f)
			{
				this.inverseLength = 1f / this.length;
			}
			else
			{
				this.inverseLength = 1f;
			}
			this.nextUpdateTime = 0f;
			this.processMagnitude = onProcessMagnitude;
		}

		// Token: 0x06008CDB RID: 36059 RVA: 0x002882F2 File Offset: 0x002864F2
		public float GetInitialMagnitude()
		{
			return this.Curve.Evaluate(0f);
		}

		// Token: 0x06008CDC RID: 36060 RVA: 0x00288304 File Offset: 0x00286504
		public VibrationEmission PlayVibration(bool isRealtime)
		{
			if (!this.vibration)
			{
				return null;
			}
			return VibrationManager.PlayVibrationClipOneShot(this.vibration, null, true, "", isRealtime);
		}

		// Token: 0x06008CDD RID: 36061 RVA: 0x00288342 File Offset: 0x00286542
		public float GetVibrationStrength(float timeElapsed)
		{
			return this.Curve.Evaluate(timeElapsed * this.inverseLength) * this.vibrationStrength;
		}

		// Token: 0x04008D61 RID: 36193
		[SerializeField]
		[FormerlySerializedAs("Magnitude")]
		private float magnitude;

		// Token: 0x04008D62 RID: 36194
		public AnimationCurve Curve;

		// Token: 0x04008D63 RID: 36195
		public float CustomUpdateRate;

		// Token: 0x04008D64 RID: 36196
		[Space]
		public VibrationDataAsset vibration;

		// Token: 0x04008D65 RID: 36197
		public float vibrationStrength = 1f;

		// Token: 0x04008D66 RID: 36198
		private float length;

		// Token: 0x04008D67 RID: 36199
		private float inverseLength = 1f;

		// Token: 0x04008D68 RID: 36200
		private Func<float, float> processMagnitude;

		// Token: 0x04008D69 RID: 36201
		private float startUpdateTime;

		// Token: 0x04008D6A RID: 36202
		private float nextUpdateTime;

		// Token: 0x04008D6B RID: 36203
		private Vector2 startOffset;

		// Token: 0x04008D6C RID: 36204
		private Vector2 targetOffset;
	}

	// Token: 0x02001726 RID: 5926
	[Serializable]
	private class WorldRumble
	{
		// Token: 0x06008CDF RID: 36063 RVA: 0x0028837C File Offset: 0x0028657C
		public float GetLength(AudioClip sound)
		{
			if (this.length.Start > Mathf.Epsilon || this.length.End > Mathf.Epsilon)
			{
				return this.length.GetRandomValue();
			}
			if (sound)
			{
				return sound.length;
			}
			return 0f;
		}

		// Token: 0x04008D6D RID: 36205
		public WorldRumbleManager.CameraShake CameraShake;

		// Token: 0x04008D6E RID: 36206
		public AudioClip[] Sounds;

		// Token: 0x04008D6F RID: 36207
		[SerializeField]
		private MinMaxFloat length;

		// Token: 0x04008D70 RID: 36208
		public float WindSpeedMultiplier = 1f;

		// Token: 0x04008D71 RID: 36209
		public float EventThreshold;

		// Token: 0x04008D72 RID: 36210
		public string SendEventOver;

		// Token: 0x04008D73 RID: 36211
		public string SendEventUnder;

		// Token: 0x04008D74 RID: 36212
		public GameObject ActiveWhileRumbling;

		// Token: 0x04008D75 RID: 36213
		public NestedFadeGroupBase FadeWithRumble;
	}

	// Token: 0x02001727 RID: 5927
	[Serializable]
	private class WorldRumbleGroup
	{
		// Token: 0x04008D76 RID: 36214
		public PlayerDataTest Condition;

		// Token: 0x04008D77 RID: 36215
		[EnumPickerBitmask(typeof(MapZone))]
		public long MapZoneMask;

		// Token: 0x04008D78 RID: 36216
		public MinMaxFloat WaitTime;

		// Token: 0x04008D79 RID: 36217
		public WorldRumbleManager.WorldRumble[] Rumbles;

		// Token: 0x04008D7A RID: 36218
		public string SendEventToRegister;
	}

	// Token: 0x02001728 RID: 5928
	public interface IWorldRumblePreventer
	{
		// Token: 0x17000EFC RID: 3836
		// (get) Token: 0x06008CE2 RID: 36066
		bool AllowRumble { get; }
	}
}
