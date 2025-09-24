using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Token: 0x020007CF RID: 1999
public class ScenePreloader : MonoBehaviour
{
	// Token: 0x0600466B RID: 18027 RVA: 0x001316B2 File Offset: 0x0012F8B2
	private void Start()
	{
		if (this.activateTrigger)
		{
			this.activateTrigger.OnTriggerEntered += delegate(Collider2D _, GameObject _)
			{
				this.StartPreload(LoadSceneMode.Additive);
			};
			return;
		}
		this.StartPreload(LoadSceneMode.Additive);
	}

	// Token: 0x0600466C RID: 18028 RVA: 0x001316E0 File Offset: 0x0012F8E0
	private void StartPreload(LoadSceneMode mode = LoadSceneMode.Additive)
	{
		if (this.startedLoad)
		{
			return;
		}
		if (!Platform.Current.FetchScenesBeforeFade)
		{
			return;
		}
		if (!this.test.IsFulfilled)
		{
			return;
		}
		string entryGateName = GameManager.instance.entryGateName;
		if (!string.IsNullOrEmpty(entryGateName))
		{
			TransitionPoint[] array = this.entryGateWhiteList;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].gameObject.name.Equals(entryGateName))
				{
					return;
				}
			}
			array = this.entryGateBlackList;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].gameObject.name.Equals(entryGateName))
				{
					return;
				}
			}
		}
		this.startedLoad = true;
		base.StartCoroutine(this.LoadRoutine(mode));
	}

	// Token: 0x0600466D RID: 18029 RVA: 0x0013178F File Offset: 0x0012F98F
	private IEnumerator LoadRoutine(LoadSceneMode mode)
	{
		yield return null;
		string text = "Scenes/" + this.sceneNameToLoad;
		AsyncOperationHandle<SceneInstance> operation = Addressables.LoadSceneAsync(text, mode, false, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
		ScenePreloader._pendingOperations.Add(new ScenePreloader.SceneLoadOp(text, operation, mode));
		yield break;
	}

	// Token: 0x0600466E RID: 18030 RVA: 0x001317A8 File Offset: 0x0012F9A8
	public static AsyncOperationHandle<SceneInstance>? TakeSceneLoadOperation(string sceneName, LoadSceneMode expectedMode)
	{
		if (ScenePreloader._pendingOperations == null)
		{
			return null;
		}
		for (int i = ScenePreloader._pendingOperations.Count - 1; i >= 0; i--)
		{
			ScenePreloader.SceneLoadOp sceneLoadOp = ScenePreloader._pendingOperations[i];
			if (sceneLoadOp.Address.Equals(sceneName))
			{
				if (sceneLoadOp.Mode != expectedMode)
				{
					Debug.LogErrorFormat("Preloaded scene was not loaded with the expected load method! Expected: {0}, Was: {1}", new object[]
					{
						expectedMode.ToString(),
						sceneLoadOp.Mode.ToString()
					});
				}
				ScenePreloader._pendingOperations.RemoveAt(i);
				return new AsyncOperationHandle<SceneInstance>?(sceneLoadOp.Operation);
			}
		}
		return null;
	}

	// Token: 0x170007F5 RID: 2037
	// (get) Token: 0x0600466F RID: 18031 RVA: 0x00131857 File Offset: 0x0012FA57
	public static bool HasPendingOperations
	{
		get
		{
			return ScenePreloader._pendingOperations != null && ScenePreloader._pendingOperations.Count > 0;
		}
	}

	// Token: 0x06004670 RID: 18032 RVA: 0x0013186F File Offset: 0x0012FA6F
	public static IEnumerator ForceEndPendingOperations()
	{
		if (ScenePreloader._pendingOperations == null)
		{
			yield break;
		}
		foreach (ScenePreloader.SceneLoadOp op in ScenePreloader._pendingOperations)
		{
			if (!op.Operation.IsDone)
			{
				yield return op.Operation;
			}
			ScenePreloader._forceEndedOperations.Add(op);
			yield return op.Operation.Result.ActivateAsync();
			op = null;
		}
		List<ScenePreloader.SceneLoadOp>.Enumerator enumerator = default(List<ScenePreloader.SceneLoadOp>.Enumerator);
		ScenePreloader._pendingOperations.Clear();
		yield break;
		yield break;
	}

	// Token: 0x06004671 RID: 18033 RVA: 0x00131878 File Offset: 0x0012FA78
	public static void Cleanup()
	{
		if (ScenePreloader._forceEndedOperations != null)
		{
			foreach (ScenePreloader.SceneLoadOp sceneLoadOp in ScenePreloader._forceEndedOperations)
			{
				SceneManager.UnloadSceneAsync(sceneLoadOp.Address);
			}
			ScenePreloader._forceEndedOperations.Clear();
		}
	}

	// Token: 0x06004672 RID: 18034 RVA: 0x001318E0 File Offset: 0x0012FAE0
	public static IEnumerable<ScenePreloader.SceneLoadOp> GetOperations()
	{
		return ScenePreloader._pendingOperations;
	}

	// Token: 0x06004673 RID: 18035 RVA: 0x001318E8 File Offset: 0x0012FAE8
	public static void SpawnPreloader(string sceneName, LoadSceneMode mode)
	{
		if (string.IsNullOrEmpty(sceneName))
		{
			Debug.LogError("Cannot preload scene with empty name!");
			return;
		}
		if (SceneAdditiveLoadConditional.IsAnyLoaded)
		{
			return;
		}
		ScenePreloader component = new GameObject("Scene Preloader", new Type[]
		{
			typeof(ScenePreloader)
		}).GetComponent<ScenePreloader>();
		component.sceneNameToLoad = sceneName;
		component.test = new PlayerDataTest();
		component.entryGateWhiteList = new TransitionPoint[0];
		component.entryGateBlackList = new TransitionPoint[0];
		component.StartPreload(mode);
	}

	// Token: 0x040046DD RID: 18141
	[SerializeField]
	private string sceneNameToLoad = "";

	// Token: 0x040046DE RID: 18142
	[SerializeField]
	private PlayerDataTest test;

	// Token: 0x040046DF RID: 18143
	[SerializeField]
	private TriggerEnterEvent activateTrigger;

	// Token: 0x040046E0 RID: 18144
	[SerializeField]
	private TransitionPoint[] entryGateWhiteList;

	// Token: 0x040046E1 RID: 18145
	[SerializeField]
	private TransitionPoint[] entryGateBlackList;

	// Token: 0x040046E2 RID: 18146
	private bool startedLoad;

	// Token: 0x040046E3 RID: 18147
	private static readonly List<ScenePreloader.SceneLoadOp> _pendingOperations = new List<ScenePreloader.SceneLoadOp>();

	// Token: 0x040046E4 RID: 18148
	private static readonly List<ScenePreloader.SceneLoadOp> _forceEndedOperations = new List<ScenePreloader.SceneLoadOp>();

	// Token: 0x02001AA2 RID: 6818
	public class SceneLoadOp
	{
		// Token: 0x1700113A RID: 4410
		// (get) Token: 0x06009787 RID: 38791 RVA: 0x002AAACB File Offset: 0x002A8CCB
		// (set) Token: 0x06009788 RID: 38792 RVA: 0x002AAAD3 File Offset: 0x002A8CD3
		public AsyncOperationHandle<SceneInstance> Operation { get; private set; }

		// Token: 0x1700113B RID: 4411
		// (get) Token: 0x06009789 RID: 38793 RVA: 0x002AAADC File Offset: 0x002A8CDC
		// (set) Token: 0x0600978A RID: 38794 RVA: 0x002AAAE4 File Offset: 0x002A8CE4
		public string Address { get; private set; }

		// Token: 0x1700113C RID: 4412
		// (get) Token: 0x0600978B RID: 38795 RVA: 0x002AAAED File Offset: 0x002A8CED
		// (set) Token: 0x0600978C RID: 38796 RVA: 0x002AAAF5 File Offset: 0x002A8CF5
		public LoadSceneMode Mode { get; private set; }

		// Token: 0x0600978D RID: 38797 RVA: 0x002AAAFE File Offset: 0x002A8CFE
		public SceneLoadOp(string address, AsyncOperationHandle<SceneInstance> operation, LoadSceneMode mode)
		{
			this.Address = address;
			this.Operation = operation;
			this.Mode = mode;
		}
	}
}
