using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

// Token: 0x02000415 RID: 1045
[Serializable]
public class CustomSceneManager : MonoBehaviour
{
	// Token: 0x1400006C RID: 108
	// (add) Token: 0x06002363 RID: 9059 RVA: 0x000A1AAC File Offset: 0x0009FCAC
	// (remove) Token: 0x06002364 RID: 9060 RVA: 0x000A1AE4 File Offset: 0x0009FCE4
	public event Action AudioSnapshotsApplied;

	// Token: 0x170003A4 RID: 932
	// (get) Token: 0x06002365 RID: 9061 RVA: 0x000A1B19 File Offset: 0x0009FD19
	public bool ForceNotMemory
	{
		get
		{
			return this.forceNotMemory;
		}
	}

	// Token: 0x170003A5 RID: 933
	// (get) Token: 0x06002366 RID: 9062 RVA: 0x000A1B21 File Offset: 0x0009FD21
	public NoTeleportRegion.TeleportAllowState TeleportAllowState
	{
		get
		{
			return this.teleportAllowState;
		}
	}

	// Token: 0x170003A6 RID: 934
	// (get) Token: 0x06002367 RID: 9063 RVA: 0x000A1B29 File Offset: 0x0009FD29
	public bool HeroKeepHealth
	{
		get
		{
			return this.heroKeepHealth;
		}
	}

	// Token: 0x170003A7 RID: 935
	// (get) Token: 0x06002368 RID: 9064 RVA: 0x000A1B31 File Offset: 0x0009FD31
	public float FrostSpeed
	{
		get
		{
			if (!this.frostSpeed)
			{
				return 0f;
			}
			return this.frostSpeed.FrostSpeed;
		}
	}

	// Token: 0x170003A8 RID: 936
	// (get) Token: 0x06002369 RID: 9065 RVA: 0x000A1B51 File Offset: 0x0009FD51
	// (set) Token: 0x0600236A RID: 9066 RVA: 0x000A1B59 File Offset: 0x0009FD59
	public float AngleToSilkThread { get; private set; }

	// Token: 0x170003A9 RID: 937
	// (get) Token: 0x0600236B RID: 9067 RVA: 0x000A1B62 File Offset: 0x0009FD62
	// (set) Token: 0x0600236C RID: 9068 RVA: 0x000A1B6A File Offset: 0x0009FD6A
	public bool IsAudioSnapshotsApplied { get; private set; }

	// Token: 0x0600236D RID: 9069 RVA: 0x000A1B73 File Offset: 0x0009FD73
	private bool IsOverridingMapZoneColorSettings()
	{
		return this.mapZone == MapZone.NONE || this.overrideColorSettings;
	}

	// Token: 0x170003AA RID: 938
	// (get) Token: 0x0600236E RID: 9070 RVA: 0x000A1B85 File Offset: 0x0009FD85
	// (set) Token: 0x0600236F RID: 9071 RVA: 0x000A1B8C File Offset: 0x0009FD8C
	public static int Version { get; private set; }

	// Token: 0x06002370 RID: 9072 RVA: 0x000A1B94 File Offset: 0x0009FD94
	public static void IncrementVersion()
	{
		CustomSceneManager.Version++;
	}

	// Token: 0x06002371 RID: 9073 RVA: 0x000A1BA4 File Offset: 0x0009FDA4
	private void Awake()
	{
		this.OnValidate();
		CustomSceneManager.IncrementVersion();
		foreach (SceneObjectPool sceneObjectPool in this.scenePools)
		{
			if (sceneObjectPool)
			{
				sceneObjectPool.SpawnPool(base.gameObject);
			}
		}
	}

	// Token: 0x06002372 RID: 9074 RVA: 0x000A1BEC File Offset: 0x0009FDEC
	private void Start()
	{
		this.gm = GameManager.instance;
		this.gc = GameCameras.instance;
		this.pd = PlayerData.instance;
		if (this.musicCue)
		{
			this.musicCue.Preload(base.gameObject);
		}
		this.heroCtrl = HeroController.SilentInstance;
		this.isGameplayScene = this.gm.IsGameplayScene();
		this.gm.SceneInit += this.OnSceneInit;
		this.OnSceneInit();
		if (!this.IsOverridingMapZoneColorSettings())
		{
			AsyncOperationHandle<References> handle = Addressables.LoadAssetAsync<References>("ReferencesData");
			References references = handle.WaitForCompletion();
			if (references && references.sceneDefaultSettings)
			{
				this.UpdateSceneSettings(references.sceneDefaultSettings.GetMapZoneSettingsRuntime(this.mapZone, this.pd.blackThreadWorld ? SceneManagerSettings.Conditions.BlackThread : SceneManagerSettings.Conditions.None));
			}
			Addressables.Release<References>(handle);
		}
		this.UpdateScene();
		this.pd.environmentType = this.environmentType;
		Action afterCameraPositioned = null;
		this.isValid = true;
		Action <>9__1;
		afterCameraPositioned = delegate()
		{
			if (!this.isValid)
			{
				this.isValid = false;
				GameCameras.instance.cameraController.PositionedAtHero -= afterCameraPositioned;
				return;
			}
			float num;
			float num2;
			if (this.useAltsIfAlreadyPlaying && this.musicCue != null && this.gm.AudioManager.CurrentMusicCue == this.musicCue)
			{
				num = this.altMusicDelayTime;
				num2 = this.altMusicTransitionTime;
			}
			else
			{
				num = this.musicDelayTime;
				num2 = this.musicTransitionTime;
			}
			if (this.gm.entryGateName == "door_tubeEnter")
			{
				num += 2.5f;
			}
			if (this.musicCue != null)
			{
				this.gm.AudioManager.ApplyMusicCue(this.musicCue, num, num2, false);
			}
			if (this.musicSnapshot != null)
			{
				this.gm.AudioManager.ApplyMusicSnapshot(this.musicSnapshot, num, num2);
			}
			if (this.atmosCue != null)
			{
				this.gm.AudioManager.ClearAtmosOverrides();
				this.gm.AudioManager.ApplyAtmosCue(this.atmosCue, this.transitionTime, true);
			}
			if (this.atmosSnapshot != null)
			{
				AudioManager.TransitionToAtmosOverride(this.atmosSnapshot, this.transitionTime);
			}
			if (this.enviroSnapshot != null && !this.cancelEnviroSnapshot)
			{
				this.enviroSnapshot.TransitionTo(this.transitionTime);
			}
			if (this.actorSnapshot != null)
			{
				Action callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate()
					{
						if (this.actorSnapshot != null)
						{
							if (this.gm != null && this.gm.SkipNormalActorFadeIn())
							{
								return;
							}
							this.actorSnapshot.TransitionTo(0.25f);
						}
					});
				}
				AudioManager.CustomSceneManagerSnapshotReady(callback);
			}
			else
			{
				AudioManager.CustomSceneManagerSnapshotReady(null);
			}
			if (this.shadeSnapshot != null)
			{
				this.shadeSnapshot.TransitionTo(this.transitionTime);
			}
			this.IsAudioSnapshotsApplied = true;
			Action audioSnapshotsApplied = this.AudioSnapshotsApplied;
			if (audioSnapshotsApplied != null)
			{
				audioSnapshotsApplied();
			}
			GameCameras.instance.cameraController.PositionedAtHero -= afterCameraPositioned;
			AudioManager.CustomSceneManagerReady();
		};
		GameCameras.instance.cameraController.PositionedAtHero += afterCameraPositioned;
		AudioManager.SetIsWaitingForCustomSceneManager();
		if (!this.isGameplayScene)
		{
			afterCameraPositioned();
		}
		if (this.sceneType == SceneType.GAMEPLAY)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("Vignette");
			if (gameObject)
			{
				PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "Darkness Control");
				if (playMakerFSM)
				{
					FSMUtility.SetInt(playMakerFSM, "Darkness Level", this.darknessLevel);
				}
				if (!this.noLantern)
				{
					if (playMakerFSM)
					{
						playMakerFSM.SendEvent("RESET");
					}
				}
				else if (playMakerFSM)
				{
					playMakerFSM.SendEvent("SCENE RESET NO LANTERN");
				}
			}
		}
		if (this.isWindy)
		{
			WindRegion.AddWind();
			this.addedWind = true;
		}
		if (this.isGameplayScene)
		{
			this.DrawBlackBorders();
		}
		if ((!MazeController.NewestInstance || MazeController.NewestInstance.IsCapScene) && !string.IsNullOrEmpty(this.pd.HeroCorpseScene) && this.isGameplayScene && this.pd.HeroCorpseScene == base.gameObject.scene.name)
		{
			Vector2 vector = this.pd.HeroDeathScenePos;
			HeroCorpseMarker byGuid = HeroCorpseMarker.GetByGuid(this.pd.HeroCorpseMarkerGuid);
			if (byGuid)
			{
				vector = byGuid.Position;
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.heroCorpsePrefab, new Vector3(vector.x, vector.y, this.heroCorpsePrefab.transform.position.z), Quaternion.identity);
			if (byGuid)
			{
				RepositionFromWalls component = gameObject2.GetComponent<RepositionFromWalls>();
				if (component)
				{
					component.enabled = false;
					gameObject2.transform.position = vector;
				}
			}
			gameObject2.transform.SetParent(base.transform, true);
			gameObject2.transform.SetParent(null);
		}
		if (this.sceneType == SceneType.MENU)
		{
			Platform.Current.SetSceneLoadState(false, false);
		}
	}

	// Token: 0x06002373 RID: 9075 RVA: 0x000A1F14 File Offset: 0x000A0114
	private void OnSceneInit()
	{
		if (this.gm.gameMap)
		{
			this.AngleToSilkThread = this.gm.gameMap.GetAngleBetweenScenes(this.gm.sceneName, this.pd.blackThreadWorld ? "Thread_Target_Down" : "Thread_Target_Up");
		}
		if (this.isGameplayScene)
		{
			this.AddSceneMapped();
		}
	}

	// Token: 0x06002374 RID: 9076 RVA: 0x000A1F7C File Offset: 0x000A017C
	private void OnDestroy()
	{
		this.isValid = false;
		if (this.addedWind)
		{
			WindRegion.RemoveWind();
			this.addedWind = false;
		}
		if (this.gm)
		{
			this.gm.SceneInit -= this.OnSceneInit;
			this.gm = null;
		}
		CustomSceneManager.IncrementVersion();
	}

	// Token: 0x06002375 RID: 9077 RVA: 0x000A1FD4 File Offset: 0x000A01D4
	public void AddInsideAppearanceRegion(SceneAppearanceRegion region, bool forceImmediate)
	{
		this.appearanceRegions.AddIfNotPresent(region);
		this.UpdateAppearanceRegion(forceImmediate);
	}

	// Token: 0x06002376 RID: 9078 RVA: 0x000A1FEA File Offset: 0x000A01EA
	public void RemoveInsideAppearanceRegion(SceneAppearanceRegion region, bool forceImmediate)
	{
		this.appearanceRegions.Remove(region);
		this.UpdateAppearanceRegion(forceImmediate);
	}

	// Token: 0x06002377 RID: 9079 RVA: 0x000A2000 File Offset: 0x000A0200
	private void UpdateAppearanceRegion(bool forceImmediate)
	{
		Color ambientLightColor = this.defaultColor;
		float ambientLightIntensity = this.defaultIntensity;
		float newSaturation = this.saturation;
		bool flag = false;
		bool flag2 = false;
		float duration = this.ambientLightLastFadeTime;
		float duration2 = this.saturationLastFadeTime;
		foreach (SceneAppearanceRegion sceneAppearanceRegion in this.appearanceRegions)
		{
			if (sceneAppearanceRegion.AffectAmbientLight)
			{
				ambientLightColor = sceneAppearanceRegion.AmbientLightColor;
				ambientLightIntensity = sceneAppearanceRegion.AmbientLightIntensity;
				duration = sceneAppearanceRegion.FadeDuration;
				this.ambientLightLastFadeTime = sceneAppearanceRegion.ExitFadeDuration;
				flag = true;
			}
			if (sceneAppearanceRegion.AffectSaturation)
			{
				newSaturation = sceneAppearanceRegion.Saturation;
				duration2 = sceneAppearanceRegion.FadeDuration;
				this.saturationLastFadeTime = sceneAppearanceRegion.ExitFadeDuration;
				flag2 = true;
			}
		}
		if (forceImmediate)
		{
			duration = 0f;
			duration2 = 0f;
		}
		if (flag || this.didAffectAmbientLight)
		{
			if (this.ambientLightFadeRoutine != null)
			{
				base.StopCoroutine(this.ambientLightFadeRoutine);
			}
			Color startColor = CustomSceneManager._currentAmbientLightColor;
			float startIntensity = CustomSceneManager._currentAmbientLightIntensity;
			this.ambientLightFadeRoutine = this.StartTimerRoutine(0f, duration, delegate(float time)
			{
				this.gc.sceneColorManager.AmbientColorA = Color.Lerp(startColor, ambientLightColor, time);
				this.gc.sceneColorManager.AmbientIntensityA = Mathf.Lerp(startIntensity, ambientLightIntensity, time);
				this.gc.sceneColorManager.UpdateScript(false);
			}, null, null, false);
		}
		if (flag2 || this.didAffectSaturation)
		{
			if (!this.setSaturation)
			{
				this.gc.colorCorrectionCurves.saturation = this.AdjustSaturation(this.saturation);
				this.setSaturation = true;
			}
			if (this.saturationFadeRoutine != null)
			{
				base.StopCoroutine(this.saturationFadeRoutine);
			}
			float startSaturation = this.gc.colorCorrectionCurves.saturation;
			this.saturationFadeRoutine = this.StartTimerRoutine(0f, duration2, delegate(float time)
			{
				float num = Mathf.Lerp(startSaturation, newSaturation, time);
				num = this.AdjustSaturation(num);
				this.gc.colorCorrectionCurves.saturation = num;
				this.gc.sceneColorManager.SaturationA = num;
			}, null, null, false);
		}
		this.didAffectAmbientLight = flag;
		this.didAffectSaturation = flag2;
	}

	// Token: 0x170003AB RID: 939
	// (get) Token: 0x06002378 RID: 9080 RVA: 0x000A2218 File Offset: 0x000A0418
	// (set) Token: 0x06002379 RID: 9081 RVA: 0x000A2220 File Offset: 0x000A0420
	public bool IsGradeOverridden { get; set; }

	// Token: 0x0600237A RID: 9082 RVA: 0x000A222C File Offset: 0x000A042C
	public static void SetLighting(Color ambientLightColor, float ambientLightIntensity)
	{
		CustomSceneManager._currentAmbientLightColor = ambientLightColor;
		CustomSceneManager._currentAmbientLightIntensity = ambientLightIntensity;
		float num = Mathf.Lerp(1f, ambientLightIntensity, CustomSceneManager.AmbientIntesityMix);
		RenderSettings.ambientLight = new Color(ambientLightColor.r * num, ambientLightColor.g * num, ambientLightColor.b * num, 1f);
		RenderSettings.ambientIntensity = 1f;
	}

	// Token: 0x0600237B RID: 9083 RVA: 0x000A2288 File Offset: 0x000A0488
	private void Update()
	{
		if (this.isGameplayScene)
		{
			if (this.enviroTimer < 0.25f)
			{
				this.enviroTimer += Time.deltaTime;
			}
			else if (!this.enviroSent && this.heroCtrl != null)
			{
				this.heroCtrl.checkEnvironment();
				this.enviroSent = true;
			}
			if (!this.heroInfoSent && this.heroCtrl != null)
			{
				this.heroCtrl.heroLight.MaterialColor = Color.white;
				this.heroCtrl.SetDarkness(this.darknessLevel);
				this.heroInfoSent = true;
			}
			if (!this.setSaturation)
			{
				if (Math.Abs(this.AdjustSaturation(this.saturation) - this.gc.colorCorrectionCurves.saturation) > Mathf.Epsilon)
				{
					this.gc.colorCorrectionCurves.saturation = this.AdjustSaturation(this.saturation);
				}
				this.setSaturation = true;
			}
		}
	}

	// Token: 0x0600237C RID: 9084 RVA: 0x000A2380 File Offset: 0x000A0580
	public int GetDarknessLevel()
	{
		return this.darknessLevel;
	}

	// Token: 0x0600237D RID: 9085 RVA: 0x000A2388 File Offset: 0x000A0588
	public void SetWindy(bool setting)
	{
		this.isWindy = setting;
	}

	// Token: 0x0600237E RID: 9086 RVA: 0x000A2391 File Offset: 0x000A0591
	public float AdjustSaturation(float originalSaturation)
	{
		if (!this.ignorePlatformSaturationModifiers)
		{
			return CustomSceneManager.AdjustSaturationForPlatform(originalSaturation, new MapZone?(this.mapZone));
		}
		return originalSaturation;
	}

	// Token: 0x0600237F RID: 9087 RVA: 0x000A23AE File Offset: 0x000A05AE
	public static float AdjustSaturationForPlatform(float originalSaturation, MapZone? mapZone = null)
	{
		return originalSaturation + 0.4f;
	}

	// Token: 0x06002380 RID: 9088 RVA: 0x000A23B7 File Offset: 0x000A05B7
	public void CancelEnviroSnapshot()
	{
		this.cancelEnviroSnapshot = true;
	}

	// Token: 0x06002381 RID: 9089 RVA: 0x000A23C0 File Offset: 0x000A05C0
	public void SetExtraRestZone(int zone)
	{
		this.extraRestZone = (ExtraRestZones)zone;
	}

	// Token: 0x06002382 RID: 9090 RVA: 0x000A23CC File Offset: 0x000A05CC
	private void PrintDebugInfo()
	{
		string text = "SM Setting Curves to ";
		text += "R: (";
		foreach (Keyframe keyframe in this.redChannel.keys)
		{
			text = text + keyframe.value.ToString() + ", ";
		}
		text += ") G: (";
		foreach (Keyframe keyframe2 in this.greenChannel.keys)
		{
			text = text + keyframe2.value.ToString() + ", ";
		}
		text += " ) B: (";
		foreach (Keyframe keyframe3 in this.blueChannel.keys)
		{
			text = text + keyframe3.value.ToString() + ", ";
		}
		text = text + ") S: " + this.saturation.ToString();
		Debug.Log(text);
	}

	// Token: 0x06002383 RID: 9091 RVA: 0x000A24DC File Offset: 0x000A06DC
	private void DrawBlackBorders()
	{
		float x = this.borderPrefab.transform.localScale.x;
		if (this.sceneBordersMask.IsBitSet(3))
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.borderPrefab);
			gameObject.transform.SetPosition2D(this.gm.sceneWidth + x / 2f, this.gm.sceneHeight / 2f);
			gameObject.transform.localScale = new Vector2(x, this.gm.sceneHeight + x * 2f);
			gameObject.transform.eulerAngles = new Vector3(0f, 0f, 180f);
			SceneManager.MoveGameObjectToScene(gameObject, base.gameObject.scene);
		}
		if (this.sceneBordersMask.IsBitSet(2))
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.borderPrefab);
			gameObject2.transform.SetPosition2D(-(x / 2f), this.gm.sceneHeight / 2f);
			gameObject2.transform.localScale = new Vector2(x, this.gm.sceneHeight + x * 2f);
			SceneManager.MoveGameObjectToScene(gameObject2, base.gameObject.scene);
		}
		if (this.sceneBordersMask.IsBitSet(0))
		{
			GameObject gameObject3 = Object.Instantiate<GameObject>(this.borderPrefab);
			gameObject3.transform.SetPosition2D(this.gm.sceneWidth / 2f, this.gm.sceneHeight + x / 2f);
			gameObject3.transform.localScale = new Vector2(x, x * 2f + this.gm.sceneWidth);
			gameObject3.transform.eulerAngles = new Vector3(0f, 0f, -90f);
			SceneManager.MoveGameObjectToScene(gameObject3, base.gameObject.scene);
		}
		if (this.sceneBordersMask.IsBitSet(1))
		{
			GameObject gameObject4 = Object.Instantiate<GameObject>(this.borderPrefab);
			gameObject4.transform.SetPosition2D(this.gm.sceneWidth / 2f, -(x / 2f));
			gameObject4.transform.localScale = new Vector2(x, x * 2f + this.gm.sceneWidth);
			gameObject4.transform.eulerAngles = new Vector3(0f, 0f, 90f);
			SceneManager.MoveGameObjectToScene(gameObject4, base.gameObject.scene);
		}
	}

	// Token: 0x06002384 RID: 9092 RVA: 0x000A2754 File Offset: 0x000A0954
	private void AddSceneMapped()
	{
		string sceneNameString = this.gm.GetSceneNameString();
		if (this.pd.scenesVisited.Contains(sceneNameString) || this.manualMapTrigger)
		{
			return;
		}
		this.pd.scenesVisited.Add(sceneNameString);
		GameManager instance = GameManager.instance;
		if (!instance)
		{
			return;
		}
		GameMap gameMap = instance.gameMap;
		if (gameMap)
		{
			gameMap.SetupMap(true);
		}
	}

	// Token: 0x06002385 RID: 9093 RVA: 0x000A27C0 File Offset: 0x000A09C0
	public void UpdateSceneSettings(SceneManagerSettings sms)
	{
		if (sms == null)
		{
			return;
		}
		this.mapZone = sms.mapZone;
		this.defaultColor = new Color(sms.defaultColor.r, sms.defaultColor.g, sms.defaultColor.b, sms.defaultColor.a);
		this.defaultIntensity = sms.defaultIntensity;
		this.saturation = sms.saturation;
		this.redChannel = new AnimationCurve(sms.redChannel.keys.Clone() as Keyframe[]);
		this.greenChannel = new AnimationCurve(sms.greenChannel.keys.Clone() as Keyframe[]);
		this.blueChannel = new AnimationCurve(sms.blueChannel.keys.Clone() as Keyframe[]);
		this.heroLightColor = new Color(sms.heroLightColor.r, sms.heroLightColor.g, sms.heroLightColor.b, sms.heroLightColor.a);
		this.blurPlaneVibranceOffset = sms.blurPlaneVibranceOffset;
		this.heroSaturationOffset = sms.heroSaturationOffset;
	}

	// Token: 0x06002386 RID: 9094 RVA: 0x000A28DC File Offset: 0x000A0ADC
	public void UpdateScene()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.IsGradeOverridden)
		{
			return;
		}
		if (this.gc)
		{
			this.gc.colorCorrectionCurves.saturation = this.AdjustSaturation(this.saturation);
			this.gc.colorCorrectionCurves.redChannel = this.redChannel;
			this.gc.colorCorrectionCurves.greenChannel = this.greenChannel;
			this.gc.colorCorrectionCurves.blueChannel = this.blueChannel;
			this.gc.colorCorrectionCurves.UpdateParameters();
			this.gc.sceneColorManager.SaturationA = this.AdjustSaturation(this.saturation);
			this.gc.sceneColorManager.RedA = this.redChannel;
			this.gc.sceneColorManager.GreenA = this.greenChannel;
			this.gc.sceneColorManager.BlueA = this.blueChannel;
			CustomSceneManager.SetLighting(this.defaultColor, this.defaultIntensity);
			this.gc.sceneColorManager.AmbientColorA = this.defaultColor;
			this.gc.sceneColorManager.AmbientIntensityA = this.defaultIntensity;
			if (this.isGameplayScene)
			{
				if (this.heroCtrl != null)
				{
					this.heroCtrl.heroLight.BaseColor = this.heroLightColor;
					this.heroCtrl.heroLight.Alpha = 1f;
					this.heroCtrl.heroLight.UpdateColor(true);
				}
				this.gc.sceneColorManager.HeroLightColorA = this.heroLightColor;
				float num = this.heroSaturationOffset + 0.5f;
				Shader.SetGlobalFloat(CustomSceneManager._desaturationPropId, num * -1f);
			}
			this.gc.sceneColorManager.UpdateScript(true);
		}
		BlurPlane.SetVibranceOffset(this.blurPlaneVibranceOffset);
	}

	// Token: 0x06002387 RID: 9095 RVA: 0x000A2AB5 File Offset: 0x000A0CB5
	public void SetMusicCue(MusicCue newMusicCue)
	{
		this.musicCue = newMusicCue;
		if (newMusicCue)
		{
			newMusicCue.Preload(base.gameObject);
		}
	}

	// Token: 0x06002388 RID: 9096 RVA: 0x000A2AD2 File Offset: 0x000A0CD2
	private void OnValidate()
	{
		if (this.noTeleport)
		{
			this.teleportAllowState = NoTeleportRegion.TeleportAllowState.Blocked;
			this.noTeleport = false;
		}
	}

	// Token: 0x06002389 RID: 9097 RVA: 0x000A2AEA File Offset: 0x000A0CEA
	private void OnEnable()
	{
	}

	// Token: 0x0600238A RID: 9098 RVA: 0x000A2AEC File Offset: 0x000A0CEC
	private void OnDisable()
	{
	}

	// Token: 0x040021F8 RID: 8696
	private const float HERO_SATURATION_GLOBAL = 0.5f;

	// Token: 0x040021F9 RID: 8697
	private const float ACTOR_MIXER_FADE_UP_TIME = 0.25f;

	// Token: 0x040021FB RID: 8699
	[Space]
	[Tooltip("This denotes the type of this scene, mainly if it is a gameplay scene or not.")]
	public SceneType sceneType;

	// Token: 0x040021FC RID: 8700
	[Header("Gameplay Scene Settings")]
	[Tooltip("The area of the map this scene belongs to.")]
	[Space]
	public MapZone mapZone;

	// Token: 0x040021FD RID: 8701
	[SerializeField]
	private bool forceNotMemory;

	// Token: 0x040021FE RID: 8702
	public ExtraRestZones extraRestZone;

	// Token: 0x040021FF RID: 8703
	[Tooltip("Determines if this area is currently windy.")]
	public bool isWindy;

	// Token: 0x04002200 RID: 8704
	[SerializeField]
	[AssetPickerDropdown]
	private FrostSpeedProfile frostSpeed;

	// Token: 0x04002201 RID: 8705
	[Tooltip("Determines if this level experiences tremors.")]
	public bool isTremorZone;

	// Token: 0x04002202 RID: 8706
	[Tooltip("Set environment type on scene entry. 0 = Dust, 1 = Grass, 2 = Bone, 3 = Spa, 4 = Metal, 5 = No Effect, 6 = Wet")]
	public EnvironmentTypes environmentType;

	// Token: 0x04002203 RID: 8707
	public int darknessLevel;

	// Token: 0x04002204 RID: 8708
	public bool noLantern;

	// Token: 0x04002205 RID: 8709
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private bool noTeleport;

	// Token: 0x04002206 RID: 8710
	[SerializeField]
	private NoTeleportRegion.TeleportAllowState teleportAllowState;

	// Token: 0x04002207 RID: 8711
	[SerializeField]
	private bool heroKeepHealth;

	// Token: 0x04002208 RID: 8712
	[Header("Camera Color Correction Curves")]
	[SerializeField]
	private bool overrideColorSettings;

	// Token: 0x04002209 RID: 8713
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public float saturation;

	// Token: 0x0400220A RID: 8714
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public AnimationCurve redChannel;

	// Token: 0x0400220B RID: 8715
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public AnimationCurve greenChannel;

	// Token: 0x0400220C RID: 8716
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public AnimationCurve blueChannel;

	// Token: 0x0400220D RID: 8717
	[Space]
	public bool ignorePlatformSaturationModifiers;

	// Token: 0x0400220E RID: 8718
	public float heroSaturationOffset;

	// Token: 0x0400220F RID: 8719
	[Header("Ambient Light")]
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public Color defaultColor;

	// Token: 0x04002210 RID: 8720
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public float defaultIntensity;

	// Token: 0x04002211 RID: 8721
	[Header("Hero Light")]
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public Color heroLightColor;

	// Token: 0x04002212 RID: 8722
	[Header("Blur Plane")]
	[ModifiableProperty]
	[Conditional("IsOverridingMapZoneColorSettings", true, true, false)]
	public float blurPlaneVibranceOffset;

	// Token: 0x04002213 RID: 8723
	[Header("Scene Particles")]
	public bool noParticles;

	// Token: 0x04002214 RID: 8724
	public MapZone overrideParticlesWith;

	// Token: 0x04002215 RID: 8725
	public CustomSceneParticles overrideParticlesWithCustom;

	// Token: 0x04002216 RID: 8726
	public CustomSceneManager.OverrideBoolFriendly act3ParticlesOverride;

	// Token: 0x04002217 RID: 8727
	[Header("Audio Snapshots")]
	[SerializeField]
	private AtmosCue atmosCue;

	// Token: 0x04002218 RID: 8728
	[Space]
	[SerializeField]
	private MusicCue musicCue;

	// Token: 0x04002219 RID: 8729
	[SerializeField]
	private AudioMixerSnapshot musicSnapshot;

	// Token: 0x0400221A RID: 8730
	[SerializeField]
	private float musicDelayTime;

	// Token: 0x0400221B RID: 8731
	[SerializeField]
	private float musicTransitionTime;

	// Token: 0x0400221C RID: 8732
	[SerializeField]
	private bool useAltsIfAlreadyPlaying;

	// Token: 0x0400221D RID: 8733
	[SerializeField]
	[ModifiableProperty]
	[Conditional("useAltsIfAlreadyPlaying", true, false, false)]
	private float altMusicDelayTime;

	// Token: 0x0400221E RID: 8734
	[SerializeField]
	[ModifiableProperty]
	[Conditional("useAltsIfAlreadyPlaying", true, false, false)]
	private float altMusicTransitionTime;

	// Token: 0x0400221F RID: 8735
	[Space]
	public AudioMixerSnapshot atmosSnapshot;

	// Token: 0x04002220 RID: 8736
	public AudioMixerSnapshot enviroSnapshot;

	// Token: 0x04002221 RID: 8737
	public AudioMixerSnapshot actorSnapshot;

	// Token: 0x04002222 RID: 8738
	public AudioMixerSnapshot shadeSnapshot;

	// Token: 0x04002223 RID: 8739
	public float transitionTime;

	// Token: 0x04002224 RID: 8740
	[Header("Scene Border")]
	public GameObject borderPrefab;

	// Token: 0x04002225 RID: 8741
	[EnumPickerBitmask(typeof(CustomSceneManager.SceneBorderPositions))]
	public int sceneBordersMask = -1;

	// Token: 0x04002226 RID: 8742
	[Header("Mapping")]
	public bool manualMapTrigger;

	// Token: 0x04002227 RID: 8743
	[Header("Object Spawns")]
	public GameObject heroCorpsePrefab;

	// Token: 0x04002228 RID: 8744
	public SceneObjectPool[] scenePools;

	// Token: 0x04002229 RID: 8745
	[Header("World Rumble")]
	public CustomSceneManager.WorldRumbleSettings WorldRumble;

	// Token: 0x0400222A RID: 8746
	private GameManager gm;

	// Token: 0x0400222B RID: 8747
	private GameCameras gc;

	// Token: 0x0400222C RID: 8748
	private HeroController heroCtrl;

	// Token: 0x0400222D RID: 8749
	private PlayerData pd;

	// Token: 0x0400222E RID: 8750
	private List<SceneAppearanceRegion> appearanceRegions = new List<SceneAppearanceRegion>();

	// Token: 0x0400222F RID: 8751
	private Coroutine ambientLightFadeRoutine;

	// Token: 0x04002230 RID: 8752
	private float ambientLightLastFadeTime;

	// Token: 0x04002231 RID: 8753
	private bool didAffectAmbientLight;

	// Token: 0x04002232 RID: 8754
	private static Color _currentAmbientLightColor;

	// Token: 0x04002233 RID: 8755
	private static float _currentAmbientLightIntensity;

	// Token: 0x04002234 RID: 8756
	private Coroutine saturationFadeRoutine;

	// Token: 0x04002235 RID: 8757
	private float saturationLastFadeTime;

	// Token: 0x04002236 RID: 8758
	private bool didAffectSaturation;

	// Token: 0x04002237 RID: 8759
	private static readonly int _desaturationPropId = Shader.PropertyToID("_HeroDesaturation");

	// Token: 0x04002238 RID: 8760
	private float enviroTimer;

	// Token: 0x04002239 RID: 8761
	private bool enviroSent;

	// Token: 0x0400223A RID: 8762
	private bool heroInfoSent;

	// Token: 0x0400223B RID: 8763
	private bool setSaturation;

	// Token: 0x0400223C RID: 8764
	private bool isGameplayScene;

	// Token: 0x0400223D RID: 8765
	private bool addedWind;

	// Token: 0x0400223E RID: 8766
	private bool cancelEnviroSnapshot;

	// Token: 0x04002242 RID: 8770
	private bool isValid;

	// Token: 0x04002243 RID: 8771
	public static float AmbientIntesityMix = 0.5f;

	// Token: 0x04002245 RID: 8773
	private const float REGULAR_CONSTANT = 0.4f;

	// Token: 0x020016A7 RID: 5799
	public enum WorldRumbleSettings
	{
		// Token: 0x04008BA0 RID: 35744
		MapZone,
		// Token: 0x04008BA1 RID: 35745
		Enabled,
		// Token: 0x04008BA2 RID: 35746
		Disabled
	}

	// Token: 0x020016A8 RID: 5800
	public enum SceneBorderPositions
	{
		// Token: 0x04008BA4 RID: 35748
		Top,
		// Token: 0x04008BA5 RID: 35749
		Bottom,
		// Token: 0x04008BA6 RID: 35750
		Left,
		// Token: 0x04008BA7 RID: 35751
		Right
	}

	// Token: 0x020016A9 RID: 5801
	public enum BoolFriendly
	{
		// Token: 0x04008BA9 RID: 35753
		Off,
		// Token: 0x04008BAA RID: 35754
		On
	}

	// Token: 0x020016AA RID: 5802
	[Serializable]
	public class OverrideBoolFriendly : OverrideValue<CustomSceneManager.BoolFriendly>
	{
	}
}
