using System;
using System.Collections;
using GlobalEnums;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Token: 0x0200009A RID: 154
public class OpeningSequence : MonoBehaviour, GameManager.ISkippable
{
	// Token: 0x060004CA RID: 1226 RVA: 0x00019766 File Offset: 0x00017966
	protected void OnEnable()
	{
		this.chainSequence.TransitionedToNextSequence += this.OnChangingSequences;
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0001977F File Offset: 0x0001797F
	protected void OnDisable()
	{
		this.chainSequence.TransitionedToNextSequence -= this.OnChangingSequences;
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x00019798 File Offset: 0x00017998
	private void OnDestroy()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance)
		{
			silentInstance.DeregisterSkippable(this);
		}
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x000197BA File Offset: 0x000179BA
	protected IEnumerator Start()
	{
		this.blanker.AlphaSelf = 0f;
		this.isAsync = Platform.Current.FetchScenesBeforeFade;
		if (!this.isAsync)
		{
			return this.StartSync();
		}
		return this.StartAsync();
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x000197F1 File Offset: 0x000179F1
	private IEnumerator StartAsync()
	{
		GameCameras gc = GameCameras.instance;
		GameManager gm = GameManager.instance;
		PlayerData instance = PlayerData.instance;
		gc.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
		gc.OnCinematicBegin();
		PlayMakerFSM.BroadcastEvent("START FADE OUT");
		gm.ui.SetState(UIState.CUTSCENE);
		gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING);
		gm.RegisterSkippable(this);
		this.chainSequence.Begin();
		Platform.Current.SetSceneLoadState(true, false);
		Platform.Current.SetBackgroundLoadingPriority(this.streamingLoadPriority);
		string worldSceneName = this.loadSave ? instance.respawnScene : "Tut_01";
		bool knightLoadDone = false;
		bool worldLoadDone = false;
		AsyncOperationHandle<SceneInstance> asyncWorldLoad = default(AsyncOperationHandle<SceneInstance>);
		AsyncOperationHandle<GameObject> heroPrefabHandle = gm.LoadHeroPrefab();
		Action<AsyncOperationHandle<SceneInstance>> <>9__1;
		heroPrefabHandle.Completed += delegate(AsyncOperationHandle<GameObject> _)
		{
			knightLoadDone = true;
			asyncWorldLoad = Addressables.LoadSceneAsync("Scenes/" + worldSceneName, LoadSceneMode.Single, false, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
			Action<AsyncOperationHandle<SceneInstance>> value;
			if ((value = <>9__1) == null)
			{
				value = (<>9__1 = delegate(AsyncOperationHandle<SceneInstance> _)
				{
					worldLoadDone = true;
				});
			}
			asyncWorldLoad.Completed += value;
		};
		this.isLevelReady = false;
		bool forceUpdateSkip = true;
		while (this.chainSequence.IsPlaying)
		{
			if (!this.isLevelReady)
			{
				this.isLevelReady = (knightLoadDone & worldLoadDone);
				bool flag = this.isLevelReady;
			}
			SkipPromptMode skipPromptMode;
			if (this.chainSequence.IsCurrentSkipped || !this.chainSequence.CanSkipCurrent)
			{
				skipPromptMode = SkipPromptMode.NOT_SKIPPABLE;
			}
			else if (!this.isLevelReady || this.skipChargeTimer < this.skipChargeDuration)
			{
				skipPromptMode = SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING;
			}
			else
			{
				skipPromptMode = SkipPromptMode.SKIP_PROMPT;
			}
			if (gm.inputHandler.SkipMode != skipPromptMode || forceUpdateSkip)
			{
				forceUpdateSkip = false;
				gm.inputHandler.SetSkipMode(skipPromptMode);
			}
			yield return null;
		}
		Platform.Current.SetBackgroundLoadingPriority(this.completedLoadPriority);
		gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		Platform.Current.SetSceneLoadState(true, true);
		ObjectPool.CreateStartupPools();
		Object.Instantiate<GameObject>(heroPrefabHandle.Result);
		yield return null;
		yield return null;
		if (this.loadSave)
		{
			gm.RespawningHero = true;
		}
		else
		{
			gm.OnWillActivateFirstLevel();
		}
		gm.nextSceneName = worldSceneName;
		AsyncOperation asyncOperation = asyncWorldLoad.Result.ActivateAsync();
		gm.LastSceneLoad = new SceneLoad(asyncWorldLoad, new GameManager.SceneLoadInfo
		{
			IsFirstLevelForPlayer = true,
			SceneName = worldSceneName
		});
		Platform.Current.RestoreBackgroundLoadingPriority();
		gm.SetupSceneRefs(true);
		gm.BeginScene();
		gm.OnNextLevelReady();
		gc.OnCinematicEnd();
		Platform.Current.SetSceneLoadState(false, false);
		yield return asyncOperation;
		yield break;
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x00019800 File Offset: 0x00017A00
	protected IEnumerator StartSync()
	{
		GameCameras gc = GameCameras.instance;
		GameManager gm = GameManager.instance;
		PlayerData pd = PlayerData.instance;
		gc.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
		gc.OnCinematicBegin();
		PlayMakerFSM.BroadcastEvent("START FADE OUT");
		Debug.LogFormat(this, "Starting opening sequence.", Array.Empty<object>());
		gm.ui.SetState(UIState.CUTSCENE);
		gm.RegisterSkippable(this);
		this.chainSequence.Begin();
		bool forceUpdateSkipMode = true;
		while (this.chainSequence.IsPlaying)
		{
			SkipPromptMode skipPromptMode;
			if (this.chainSequence.IsCurrentSkipped || !this.chainSequence.CanSkipCurrent)
			{
				skipPromptMode = SkipPromptMode.NOT_SKIPPABLE;
			}
			else if (this.skipChargeTimer < this.skipChargeDuration)
			{
				skipPromptMode = SkipPromptMode.NOT_SKIPPABLE_DUE_TO_LOADING;
			}
			else
			{
				skipPromptMode = SkipPromptMode.SKIP_PROMPT;
			}
			if (gm.inputHandler.SkipMode != skipPromptMode || forceUpdateSkipMode)
			{
				forceUpdateSkipMode = false;
				gm.inputHandler.SetSkipMode(skipPromptMode);
			}
			yield return null;
		}
		gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		Platform.Current.SetSceneLoadState(true, true);
		AsyncOperationHandle<GameObject> heroPrefabHandle = gm.LoadHeroPrefab();
		yield return heroPrefabHandle;
		Object.Instantiate<GameObject>(heroPrefabHandle.Result);
		ObjectPool.CreateStartupPools();
		yield return null;
		yield return null;
		if (this.loadSave)
		{
			gm.RespawningHero = true;
		}
		else
		{
			gm.OnWillActivateFirstLevel();
		}
		string worldSceneName = this.loadSave ? pd.respawnScene : "Tut_01";
		gm.nextSceneName = worldSceneName;
		AsyncOperationHandle<SceneInstance> worldLoad = Addressables.LoadSceneAsync("Scenes/" + worldSceneName, LoadSceneMode.Single, false, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
		Platform.Current.SetSceneLoadState(false, false);
		yield return worldLoad;
		AsyncOperation asyncOperation = worldLoad.Result.ActivateAsync();
		gm.LastSceneLoad = new SceneLoad(worldLoad, new GameManager.SceneLoadInfo
		{
			IsFirstLevelForPlayer = true,
			SceneName = worldSceneName
		});
		gm.SetupSceneRefs(true);
		gm.BeginScene();
		gm.OnNextLevelReady();
		gc.OnCinematicEnd();
		yield return asyncOperation;
		yield break;
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0001980F File Offset: 0x00017A0F
	protected void Update()
	{
		this.skipChargeTimer += Time.unscaledDeltaTime;
	}

	// Token: 0x060004D1 RID: 1233 RVA: 0x00019823 File Offset: 0x00017A23
	public IEnumerator Skip()
	{
		if (!this.chainSequence.CanSkipCurrent)
		{
			yield break;
		}
		for (float elapsed = 0f; elapsed < 0.3f; elapsed += Time.deltaTime)
		{
			this.blanker.AlphaSelf = elapsed / 0.3f;
			yield return null;
		}
		this.blanker.AlphaSelf = 1f;
		yield return null;
		this.chainSequence.SkipSingle();
		while (this.chainSequence.IsCurrentSkipped)
		{
			this.skipChargeTimer = 0f;
			yield return null;
		}
		yield break;
	}

	// Token: 0x060004D2 RID: 1234 RVA: 0x00019832 File Offset: 0x00017A32
	private void OnChangingSequences()
	{
		Debug.LogFormat("Opening sequence changing sequences.", Array.Empty<object>());
		this.skipChargeTimer = 0f;
		this.blanker.AlphaSelf = 0f;
	}

	// Token: 0x0400049D RID: 1181
	[SerializeField]
	private ChainSequence chainSequence;

	// Token: 0x0400049E RID: 1182
	[SerializeField]
	private ThreadPriority streamingLoadPriority;

	// Token: 0x0400049F RID: 1183
	[SerializeField]
	private ThreadPriority completedLoadPriority;

	// Token: 0x040004A0 RID: 1184
	[SerializeField]
	private float skipChargeDuration;

	// Token: 0x040004A1 RID: 1185
	[SerializeField]
	private bool loadSave;

	// Token: 0x040004A2 RID: 1186
	[SerializeField]
	private NestedFadeGroupBase blanker;

	// Token: 0x040004A3 RID: 1187
	private bool isAsync;

	// Token: 0x040004A4 RID: 1188
	private bool isLevelReady;

	// Token: 0x040004A5 RID: 1189
	private float skipChargeTimer;
}
