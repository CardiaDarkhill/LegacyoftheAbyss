using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Token: 0x02000782 RID: 1922
public class SceneLoad
{
	// Token: 0x170007A3 RID: 1955
	// (get) Token: 0x06004430 RID: 17456 RVA: 0x0012B390 File Offset: 0x00129590
	public AsyncOperationHandle<SceneInstance> OperationHandle
	{
		get
		{
			return this.operationHandle;
		}
	}

	// Token: 0x170007A4 RID: 1956
	// (get) Token: 0x06004431 RID: 17457 RVA: 0x0012B398 File Offset: 0x00129598
	public string TargetSceneName
	{
		get
		{
			return this.SceneLoadInfo.SceneName;
		}
	}

	// Token: 0x170007A5 RID: 1957
	// (get) Token: 0x06004432 RID: 17458 RVA: 0x0012B3A5 File Offset: 0x001295A5
	// (set) Token: 0x06004433 RID: 17459 RVA: 0x0012B3AD File Offset: 0x001295AD
	public bool IsFetchAllowed { get; set; }

	// Token: 0x170007A6 RID: 1958
	// (get) Token: 0x06004434 RID: 17460 RVA: 0x0012B3B6 File Offset: 0x001295B6
	// (set) Token: 0x06004435 RID: 17461 RVA: 0x0012B3BE File Offset: 0x001295BE
	public bool IsActivationAllowed { get; set; }

	// Token: 0x170007A7 RID: 1959
	// (get) Token: 0x06004436 RID: 17462 RVA: 0x0012B3C7 File Offset: 0x001295C7
	// (set) Token: 0x06004437 RID: 17463 RVA: 0x0012B3CF File Offset: 0x001295CF
	public bool IsUnloadAssetsRequired { get; set; }

	// Token: 0x170007A8 RID: 1960
	// (get) Token: 0x06004438 RID: 17464 RVA: 0x0012B3D8 File Offset: 0x001295D8
	// (set) Token: 0x06004439 RID: 17465 RVA: 0x0012B3E0 File Offset: 0x001295E0
	public bool IsGarbageCollectRequired { get; set; }

	// Token: 0x170007A9 RID: 1961
	// (get) Token: 0x0600443A RID: 17466 RVA: 0x0012B3E9 File Offset: 0x001295E9
	// (set) Token: 0x0600443B RID: 17467 RVA: 0x0012B3F1 File Offset: 0x001295F1
	public bool IsFinished { get; private set; }

	// Token: 0x170007AA RID: 1962
	// (get) Token: 0x0600443C RID: 17468 RVA: 0x0012B3FA File Offset: 0x001295FA
	// (set) Token: 0x0600443D RID: 17469 RVA: 0x0012B402 File Offset: 0x00129602
	public bool WaitForFade { get; set; }

	// Token: 0x0600443E RID: 17470 RVA: 0x0012B40C File Offset: 0x0012960C
	public SceneLoad(MonoBehaviour runner, GameManager.SceneLoadInfo sceneLoadInfo)
	{
		this.runner = runner;
		this.SceneLoadInfo = sceneLoadInfo;
		this.phaseInfos = new SceneLoad.PhaseInfo[9];
		for (int i = 0; i < 9; i++)
		{
			this.phaseInfos[i] = new SceneLoad.PhaseInfo
			{
				BeginTime = null
			};
		}
	}

	// Token: 0x0600443F RID: 17471 RVA: 0x0012B460 File Offset: 0x00129660
	public SceneLoad(AsyncOperationHandle<SceneInstance> fromOperationHandle, GameManager.SceneLoadInfo sceneLoadInfo)
	{
		this.operationHandle = fromOperationHandle;
		this.SceneLoadInfo = sceneLoadInfo;
	}

	// Token: 0x170007AB RID: 1963
	// (get) Token: 0x06004440 RID: 17472 RVA: 0x0012B476 File Offset: 0x00129676
	public float? BeginTime
	{
		get
		{
			return this.phaseInfos[0].BeginTime;
		}
	}

	// Token: 0x06004441 RID: 17473 RVA: 0x0012B488 File Offset: 0x00129688
	private static string GetMemoryReadOut()
	{
		double num = (double)GCManager.GetMemoryUsage() / 1024.0 / 1024.0;
		double num2 = (double)GCManager.GetMemoryTotal() / 1024.0 / 1024.0;
		double num3 = (double)((long)SystemInfo.systemMemorySize);
		return string.Format("CPU Mem.: {0:n} / {1:n} / {2:n}", num, num2, num3);
	}

	// Token: 0x06004442 RID: 17474 RVA: 0x0012B4EF File Offset: 0x001296EF
	public static bool ShouldLog()
	{
		return CheatManager.EnableLogMessages;
	}

	// Token: 0x06004443 RID: 17475 RVA: 0x0012B4F6 File Offset: 0x001296F6
	private void RecordBeginTime(SceneLoad.Phases phase)
	{
		this.phaseInfos[(int)phase].BeginTime = new float?(Time.realtimeSinceStartup);
	}

	// Token: 0x06004444 RID: 17476 RVA: 0x0012B50F File Offset: 0x0012970F
	private void RecordEndTime(SceneLoad.Phases phase)
	{
		this.phaseInfos[(int)phase].EndTime = new float?(Time.realtimeSinceStartup);
	}

	// Token: 0x06004445 RID: 17477 RVA: 0x0012B528 File Offset: 0x00129728
	public float? GetDuration(SceneLoad.Phases phase)
	{
		SceneLoad.PhaseInfo phaseInfo = this.phaseInfos[(int)phase];
		if (phaseInfo.BeginTime != null && phaseInfo.EndTime != null)
		{
			return new float?(phaseInfo.EndTime.Value - phaseInfo.BeginTime.Value);
		}
		return null;
	}

	// Token: 0x06004446 RID: 17478 RVA: 0x0012B57E File Offset: 0x0012977E
	public void Begin()
	{
		this.runner.StartCoroutine(this.BeginRoutine());
	}

	// Token: 0x06004447 RID: 17479 RVA: 0x0012B592 File Offset: 0x00129792
	private IEnumerator BeginRoutine()
	{
		SceneAdditiveLoadConditional.LoadInSequence = true;
		string address = "Scenes/" + this.SceneLoadInfo.SceneName;
		AsyncOperationHandle<SceneInstance>? preLoadOperation = ScenePreloader.TakeSceneLoadOperation(address, LoadSceneMode.Additive);
		bool wasPreloaded = preLoadOperation != null;
		this.RecordBeginTime(SceneLoad.Phases.FetchBlocked);
		while (!this.IsFetchAllowed)
		{
			yield return null;
		}
		this.RecordEndTime(SceneLoad.Phases.FetchBlocked);
		bool hasClearedMemory = false;
		if (SceneLoad.IsClearMemoryRequired())
		{
			GameManager.IsCollectingGarbage = true;
			this.RecordBeginTime(SceneLoad.Phases.ClearMemPreFetch);
			yield return this.LocalTryClearMemory(true, false);
			hasClearedMemory = true;
			this.RecordEndTime(SceneLoad.Phases.ClearMemPreFetch);
		}
		this.RecordBeginTime(SceneLoad.Phases.Fetch);
		int priority = this.SceneLoadInfo.AsyncPriority;
		if (CheatManager.OverrideSceneLoadPriority)
		{
			priority = CheatManager.SceneLoadPriority;
		}
		if (wasPreloaded)
		{
			this.operationHandle = preLoadOperation.Value;
		}
		else if (this.SceneLoadInfo.SceneResourceLocation != null)
		{
			this.operationHandle = Addressables.LoadSceneAsync(this.SceneLoadInfo.SceneResourceLocation, LoadSceneMode.Additive, false, priority);
		}
		else
		{
			this.operationHandle = Addressables.LoadSceneAsync(address, LoadSceneMode.Additive, false, priority, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
		}
		yield return this.operationHandle;
		this.RecordEndTime(SceneLoad.Phases.Fetch);
		if (this.FetchComplete != null)
		{
			try
			{
				this.FetchComplete();
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in responders to SceneLoad.FetchComplete. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex.ToString();
				Debug.LogException(ex);
			}
		}
		this.RecordBeginTime(SceneLoad.Phases.ActivationBlocked);
		if (!wasPreloaded && ScenePreloader.HasPendingOperations)
		{
			yield return this.runner.StartCoroutine(ScenePreloader.ForceEndPendingOperations());
		}
		while (!this.IsActivationAllowed)
		{
			yield return null;
		}
		SceneAdditiveLoadConditional.Unload(SceneManager.GetActiveScene(), SceneLoad._tempOps);
		this.RecordEndTime(SceneLoad.Phases.ActivationBlocked);
		this.RecordBeginTime(SceneLoad.Phases.Activation);
		if (this.WillActivate != null)
		{
			try
			{
				this.WillActivate();
			}
			catch (Exception ex2)
			{
				Debug.LogError("Exception in responders to SceneLoad.WillActivate. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex2.ToString();
				Debug.LogException(ex2);
			}
		}
		if (this.operationHandle.OperationException != null)
		{
			Debug.LogError("Exception in scene load OperationHandle:");
			CheatManager.LastErrorText = this.operationHandle.OperationException.ToString();
			Debug.LogException(this.operationHandle.OperationException);
		}
		yield return this.operationHandle.Result.ActivateAsync();
		this.RecordEndTime(SceneLoad.Phases.Activation);
		if (this.ActivationComplete != null)
		{
			try
			{
				this.ActivationComplete();
			}
			catch (Exception ex3)
			{
				Debug.LogError("Exception in responders to SceneLoad.ActivationComplete. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex3.ToString();
				Debug.LogException(ex3);
			}
		}
		foreach (AsyncOperationHandle<SceneInstance> asyncOperationHandle in SceneLoad._tempOps)
		{
			yield return asyncOperationHandle;
		}
		List<AsyncOperationHandle<SceneInstance>>.Enumerator enumerator = default(List<AsyncOperationHandle<SceneInstance>>.Enumerator);
		SceneLoad._tempOps.Clear();
		while (SceneLoad._assetUnloadOps.Count > 0)
		{
			int index = SceneLoad._assetUnloadOps.Count - 1;
			AsyncOperation assetUnloadOp = SceneLoad._assetUnloadOps[index];
			SceneLoad._assetUnloadOps.RemoveAt(index);
			if (assetUnloadOp != null && !assetUnloadOp.isDone)
			{
				float t = 5f;
				while (!assetUnloadOp.isDone && t > 0f)
				{
					t -= Time.deltaTime;
					yield return null;
				}
				if (!assetUnloadOp.isDone)
				{
					Debug.LogError("Timed out while waiting for asset unload.");
				}
				assetUnloadOp = null;
			}
		}
		if (this.IsUnloadAssetsRequired || SceneLoad.IsClearMemoryRequired())
		{
			GameManager.IsCollectingGarbage = true;
			this.RecordBeginTime(SceneLoad.Phases.ClearMemPostActivation);
			yield return SceneLoad.TryClearMemory(!hasClearedMemory, true);
			this.RecordEndTime(SceneLoad.Phases.ClearMemPostActivation);
		}
		else if (this.IsGarbageCollectRequired)
		{
			GameManager.IsCollectingGarbage = true;
			this.RecordBeginTime(SceneLoad.Phases.GarbageCollect);
			GCManager.Collect();
			this.RecordEndTime(SceneLoad.Phases.GarbageCollect);
		}
		GameManager.IsCollectingGarbage = false;
		if (this.Complete != null)
		{
			try
			{
				this.Complete();
			}
			catch (Exception ex4)
			{
				Debug.LogError("Exception in responders to SceneLoad.Complete. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex4.ToString();
				Debug.LogException(ex4);
			}
		}
		this.RecordBeginTime(SceneLoad.Phases.StartCall);
		yield return null;
		this.RecordEndTime(SceneLoad.Phases.StartCall);
		if (this.StartCalled != null)
		{
			try
			{
				this.StartCalled();
			}
			catch (Exception ex5)
			{
				Debug.LogError("Exception in responders to SceneLoad.StartCalled. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex5.ToString();
				Debug.LogException(ex5);
			}
		}
		if (SceneAdditiveLoadConditional.ShouldLoadBoss)
		{
			this.RecordBeginTime(SceneLoad.Phases.LoadBoss);
			yield return this.runner.StartCoroutine(SceneAdditiveLoadConditional.LoadAll());
			this.RecordEndTime(SceneLoad.Phases.LoadBoss);
			try
			{
				if (this.BossLoaded != null)
				{
					this.BossLoaded();
				}
				if (GameManager.instance)
				{
					GameManager.instance.LoadedBoss();
				}
			}
			catch (Exception ex6)
			{
				Debug.LogError("Exception in responders to SceneLoad.BossLoaded. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex6.ToString();
				Debug.LogException(ex6);
			}
		}
		try
		{
			ScenePreloader.Cleanup();
		}
		catch (Exception ex7)
		{
			Debug.LogError("Exception in responders to ScenePreloader.Cleanup. Attempting to continue load regardless.");
			CheatManager.LastErrorText = ex7.ToString();
			Debug.LogException(ex7);
		}
		this.IsFinished = true;
		if (this.Finish != null)
		{
			try
			{
				this.Finish();
				yield break;
			}
			catch (Exception ex8)
			{
				Debug.LogError("Exception in responders to SceneLoad.Finish. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex8.ToString();
				Debug.LogException(ex8);
				yield break;
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x06004448 RID: 17480 RVA: 0x0012B5A1 File Offset: 0x001297A1
	public static bool IsClearMemoryRequired()
	{
		return CheatManager.IsForcingUnloads || Platform.Current.GetEstimateAllocatableMemoryMB() <= 512;
	}

	// Token: 0x06004449 RID: 17481 RVA: 0x0012B5C0 File Offset: 0x001297C0
	private IEnumerator LocalTryClearMemory(bool revertGlobalPool, bool waitForUnload = true)
	{
		if (this.WaitForFade)
		{
			float t = 0.51f;
			while (this.WaitForFade && t > 0f)
			{
				t -= Time.unscaledDeltaTime;
				yield return null;
			}
		}
		yield return SceneLoad.TryClearMemory(revertGlobalPool, waitForUnload);
		yield break;
	}

	// Token: 0x0600444A RID: 17482 RVA: 0x0012B5DD File Offset: 0x001297DD
	public static IEnumerator TryClearMemory(bool revertGlobalPool, bool waitForUnload = true)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (revertGlobalPool)
		{
			if (SceneLoad.ShouldLog())
			{
				Debug.Log(string.Format("Reverting Object Pool State : T {0:0.00}s", realtimeSinceStartup));
			}
			try
			{
				ObjectPool instance = ObjectPool.instance;
				if (instance)
				{
					instance.RevertToStartState();
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			if (SceneLoad.ShouldLog())
			{
				Debug.Log(string.Format("Finished Reverting Object Pool State : T {0:0.00}s : Time Taken {1:0.00}s", Time.realtimeSinceStartup, Time.realtimeSinceStartup - realtimeSinceStartup));
			}
			yield return null;
		}
		realtimeSinceStartup = Time.realtimeSinceStartup;
		if (SceneLoad.ShouldLog())
		{
			Debug.Log(string.Format("Beginning Asset Unload : T {0:0.00}s", realtimeSinceStartup));
		}
		AsyncOperation unloadOperation = Resources.UnloadUnusedAssets();
		if (waitForUnload)
		{
			if (SceneLoad.ShouldLog())
			{
				Debug.Log("Waiting for asset unload.");
			}
			float timeOut = 5f;
			while (!unloadOperation.isDone && timeOut > 0f)
			{
				timeOut -= Time.unscaledDeltaTime;
				yield return null;
			}
			if (!unloadOperation.isDone && SceneLoad.ShouldLog())
			{
				Debug.LogError("Asset unload operation timed out");
			}
			yield return unloadOperation;
		}
		if (!unloadOperation.isDone)
		{
			SceneLoad._assetUnloadOps.Add(unloadOperation);
		}
		unloadOperation = null;
		if (SceneLoad.ShouldLog())
		{
			Debug.Log(string.Format("Finished Asset Unload : T {0:0.00}s : Time Taken {1:0.00}s", Time.realtimeSinceStartup, Time.realtimeSinceStartup - realtimeSinceStartup));
		}
		realtimeSinceStartup = Time.realtimeSinceStartup;
		if (SceneLoad.ShouldLog())
		{
			Debug.Log(string.Format("Beginning GC : T {0:0.00}s", realtimeSinceStartup));
		}
		GCManager.ForceCollect(true, false);
		if (SceneLoad.ShouldLog())
		{
			Debug.Log(string.Format("Finished GC : T {0:0.00}s : Time Taken {1:0.00}s", Time.realtimeSinceStartup, Time.realtimeSinceStartup - realtimeSinceStartup));
		}
		yield break;
	}

	// Token: 0x140000E4 RID: 228
	// (add) Token: 0x0600444B RID: 17483 RVA: 0x0012B5F4 File Offset: 0x001297F4
	// (remove) Token: 0x0600444C RID: 17484 RVA: 0x0012B62C File Offset: 0x0012982C
	public event SceneLoad.FetchCompleteDelegate FetchComplete;

	// Token: 0x140000E5 RID: 229
	// (add) Token: 0x0600444D RID: 17485 RVA: 0x0012B664 File Offset: 0x00129864
	// (remove) Token: 0x0600444E RID: 17486 RVA: 0x0012B69C File Offset: 0x0012989C
	public event SceneLoad.WillActivateDelegate WillActivate;

	// Token: 0x140000E6 RID: 230
	// (add) Token: 0x0600444F RID: 17487 RVA: 0x0012B6D4 File Offset: 0x001298D4
	// (remove) Token: 0x06004450 RID: 17488 RVA: 0x0012B70C File Offset: 0x0012990C
	public event SceneLoad.ActivationCompleteDelegate ActivationComplete;

	// Token: 0x140000E7 RID: 231
	// (add) Token: 0x06004451 RID: 17489 RVA: 0x0012B744 File Offset: 0x00129944
	// (remove) Token: 0x06004452 RID: 17490 RVA: 0x0012B77C File Offset: 0x0012997C
	public event SceneLoad.CompleteDelegate Complete;

	// Token: 0x140000E8 RID: 232
	// (add) Token: 0x06004453 RID: 17491 RVA: 0x0012B7B4 File Offset: 0x001299B4
	// (remove) Token: 0x06004454 RID: 17492 RVA: 0x0012B7EC File Offset: 0x001299EC
	public event SceneLoad.StartCalledDelegate StartCalled;

	// Token: 0x140000E9 RID: 233
	// (add) Token: 0x06004455 RID: 17493 RVA: 0x0012B824 File Offset: 0x00129A24
	// (remove) Token: 0x06004456 RID: 17494 RVA: 0x0012B85C File Offset: 0x00129A5C
	public event SceneLoad.BossLoadCompleteDelegate BossLoaded;

	// Token: 0x140000EA RID: 234
	// (add) Token: 0x06004457 RID: 17495 RVA: 0x0012B894 File Offset: 0x00129A94
	// (remove) Token: 0x06004458 RID: 17496 RVA: 0x0012B8CC File Offset: 0x00129ACC
	public event SceneLoad.FinishDelegate Finish;

	// Token: 0x04004562 RID: 17762
	public readonly GameManager.SceneLoadInfo SceneLoadInfo;

	// Token: 0x04004563 RID: 17763
	private static readonly List<AsyncOperationHandle<SceneInstance>> _tempOps = new List<AsyncOperationHandle<SceneInstance>>();

	// Token: 0x04004564 RID: 17764
	private AsyncOperationHandle<SceneInstance> operationHandle;

	// Token: 0x04004565 RID: 17765
	private readonly MonoBehaviour runner;

	// Token: 0x04004566 RID: 17766
	public const int PhaseCount = 9;

	// Token: 0x04004567 RID: 17767
	private readonly SceneLoad.PhaseInfo[] phaseInfos;

	// Token: 0x0400456E RID: 17774
	private static readonly List<AsyncOperation> _assetUnloadOps = new List<AsyncOperation>(10);

	// Token: 0x02001A5B RID: 6747
	public enum Phases
	{
		// Token: 0x0400993A RID: 39226
		FetchBlocked,
		// Token: 0x0400993B RID: 39227
		ClearMemPreFetch,
		// Token: 0x0400993C RID: 39228
		Fetch,
		// Token: 0x0400993D RID: 39229
		ActivationBlocked,
		// Token: 0x0400993E RID: 39230
		Activation,
		// Token: 0x0400993F RID: 39231
		ClearMemPostActivation,
		// Token: 0x04009940 RID: 39232
		GarbageCollect,
		// Token: 0x04009941 RID: 39233
		StartCall,
		// Token: 0x04009942 RID: 39234
		LoadBoss
	}

	// Token: 0x02001A5C RID: 6748
	private class PhaseInfo
	{
		// Token: 0x04009943 RID: 39235
		public float? BeginTime;

		// Token: 0x04009944 RID: 39236
		public float? EndTime;
	}

	// Token: 0x02001A5D RID: 6749
	// (Invoke) Token: 0x060096A4 RID: 38564
	public delegate void FetchCompleteDelegate();

	// Token: 0x02001A5E RID: 6750
	// (Invoke) Token: 0x060096A8 RID: 38568
	public delegate void WillActivateDelegate();

	// Token: 0x02001A5F RID: 6751
	// (Invoke) Token: 0x060096AC RID: 38572
	public delegate void ActivationCompleteDelegate();

	// Token: 0x02001A60 RID: 6752
	// (Invoke) Token: 0x060096B0 RID: 38576
	public delegate void CompleteDelegate();

	// Token: 0x02001A61 RID: 6753
	// (Invoke) Token: 0x060096B4 RID: 38580
	public delegate void StartCalledDelegate();

	// Token: 0x02001A62 RID: 6754
	// (Invoke) Token: 0x060096B8 RID: 38584
	public delegate void BossLoadCompleteDelegate();

	// Token: 0x02001A63 RID: 6755
	// (Invoke) Token: 0x060096BC RID: 38588
	public delegate void FinishDelegate();
}
