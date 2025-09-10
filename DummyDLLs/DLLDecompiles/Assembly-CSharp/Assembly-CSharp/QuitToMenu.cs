using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Token: 0x020005AF RID: 1455
public class QuitToMenu : MonoBehaviour
{
	// Token: 0x06003457 RID: 13399 RVA: 0x000E8B29 File Offset: 0x000E6D29
	protected IEnumerator Start()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		yield return null;
		Platform.Current.SetSceneLoadState(true, true);
		UIManager instance = UIManager.instance;
		if (instance != null)
		{
			Object.Destroy(instance.gameObject);
		}
		HeroController instance2 = HeroController.instance;
		if (instance2 != null)
		{
			Object.Destroy(instance2.gameObject);
		}
		GameCameras instance3 = GameCameras.instance;
		if (instance3 != null)
		{
			Object.Destroy(instance3.gameObject);
		}
		GameManager instance4 = GameManager.instance;
		if (instance4 != null)
		{
			if (DemoHelper.IsExhibitionMode)
			{
				StaticVariableList.SetValue("ExhibitionModeProfileId", instance4.profileID, 0);
			}
			try
			{
				PersonalObjectPool.ForceReleasePoolManagers();
			}
			catch (Exception exception)
			{
				Debug.LogErrorFormat("Error while cleaning personal object pools as part of quit, attempting to continue regardless.", Array.Empty<object>());
				Debug.LogException(exception);
			}
			try
			{
				ObjectPool.RecycleAll();
			}
			catch (Exception exception2)
			{
				Debug.LogErrorFormat("Error while recycling all as part of quit, attempting to continue regardless.", Array.Empty<object>());
				Debug.LogException(exception2);
			}
			instance4.playerData = PlayerData.CreateNewSingleton(false);
			instance4.sceneData.Reset();
			instance4.UnloadGlobalPoolPrefab();
			instance4.UnloadHeroPrefab();
			Object.Destroy(instance4.gameObject);
			ObjectPool instance5 = ObjectPool.instance;
			if (instance5)
			{
				Object.Destroy(instance5.gameObject);
			}
		}
		TimeManager.Reset();
		BossSequenceController.Reset();
		QuestTargetCounter.ClearStatic();
		PerformanceHud.ReInit();
		CheatManager.ReInit();
		yield return null;
		yield return null;
		if (QuitToMenu._loadedAssets != null)
		{
			while (QuitToMenu._loadedAssets.Count > 0)
			{
				Addressables.Release(QuitToMenu._loadedAssets.Dequeue());
			}
		}
		yield return null;
		ToolItemLimiter.ClearStatic();
		CollectableItemManager.ClearStatic();
		TweenExtensions.ClenaupInactiveCoroutines();
		QuitToMenu.ValidatePlayMaker();
		GCManager.ForceCollect(true, false);
		yield return Resources.UnloadUnusedAssets();
		QuitToMenu.StartLoadCoreManagers();
		yield return null;
		bool finishedPrewarm = false;
		AsyncLoadOrderingManager.DoActionAfterAllLoadsComplete(delegate
		{
			finishedPrewarm = true;
		});
		while (!finishedPrewarm)
		{
			yield return null;
		}
		AsyncOperationHandle<SceneInstance> asyncOperationHandle = Addressables.LoadSceneAsync("Scenes/Menu_Title", LoadSceneMode.Single, true, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
		yield return asyncOperationHandle;
		GCManager.ForceCollect(true, false);
		yield return Resources.UnloadUnusedAssets();
		Object.Destroy(base.gameObject);
		GCManager.ForceCollect(true, false);
		Platform.Current.SetSceneLoadState(false, false);
		yield break;
	}

	// Token: 0x06003458 RID: 13400 RVA: 0x000E8B38 File Offset: 0x000E6D38
	private static void ValidatePlayMaker()
	{
		List<string> list = PlayMakerValidator.ValidatePlayMakerState(PlayMakerValidator.FixMode.All);
		if (list != null && list.Count > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Invalid PlayMaker state! Issues were fixed automatically. Look at the errors below to find out what went wrong.");
			stringBuilder.AppendLine();
			foreach (string value in list)
			{
				stringBuilder.AppendLine(value);
				stringBuilder.AppendLine();
			}
			Debug.LogError(stringBuilder.ToString());
			stringBuilder.Clear();
		}
	}

	// Token: 0x06003459 RID: 13401 RVA: 0x000E8BCC File Offset: 0x000E6DCC
	public static void StartLoadCoreManagers()
	{
		if (QuitToMenu._loadedAssets == null)
		{
			QuitToMenu._loadedAssets = new Queue<AsyncOperationHandle>();
		}
		QuitToMenu.StartLoadCoreManager("_GameManager");
		QuitToMenu.StartLoadCoreManager("_UIManager");
		QuitToMenu.StartLoadCoreManager("_GameCameras");
	}

	// Token: 0x0600345A RID: 13402 RVA: 0x000E8C00 File Offset: 0x000E6E00
	private static void StartLoadCoreManager(string address)
	{
		AsyncOperationHandle<GameObject> obj = Addressables.LoadAssetAsync<GameObject>(address);
		int loadHandle;
		AsyncLoadOrderingManager.OnStartedLoad(obj, out loadHandle);
		obj.Completed += delegate(AsyncOperationHandle<GameObject> handle)
		{
			QuitToMenu._loadedAssets.Enqueue(handle);
			AsyncLoadOrderingManager.OnCompletedLoad(handle, loadHandle);
		};
	}

	// Token: 0x040037D8 RID: 14296
	private static Queue<AsyncOperationHandle> _loadedAssets;

	// Token: 0x040037D9 RID: 14297
	public const string EXHIBITION_PROFILE_VAR = "ExhibitionModeProfileId";
}
