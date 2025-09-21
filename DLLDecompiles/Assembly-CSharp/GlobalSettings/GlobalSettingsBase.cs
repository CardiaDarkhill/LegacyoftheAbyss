using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GlobalSettings
{
	// Token: 0x020008CC RID: 2252
	public abstract class GlobalSettingsBase<T> : ScriptableObject where T : GlobalSettingsBase<T>
	{
		// Token: 0x06004EA6 RID: 20134 RVA: 0x0016DA94 File Offset: 0x0016BC94
		protected static T Get(string fileName)
		{
			if (!GlobalSettingsBase<T>._foundInstance)
			{
				if (GlobalSettingsBase<T>._delayedLoader.Item2 != null)
				{
					if (GlobalSettingsBase<T>._delayedLoader.Item1)
					{
						GlobalSettingsBase<T>._delayedLoader.Item1.StopCoroutine(GlobalSettingsBase<T>._delayedLoader.Item2);
						Object.Destroy(GlobalSettingsBase<T>._delayedLoader.Item1.gameObject);
					}
					GlobalSettingsBase<T>._delayedLoader.Item2 = null;
					GlobalSettingsBase<T>._delayedLoader.Item1 = null;
				}
				AsyncOperationHandle<T> value;
				if (GlobalSettingsBase<T>._loadHandle == null)
				{
					GlobalSettingsBase<T>._loadHandle = new AsyncOperationHandle<T>?(Addressables.LoadAssetAsync<T>("GlobalSettings/" + fileName + ".asset"));
					AsyncLoadOrderingManager.OnStartedLoad(GlobalSettingsBase<T>._loadHandle.Value, out GlobalSettingsBase<T>._orderHandle);
					value = GlobalSettingsBase<T>._loadHandle.Value;
					value.Completed += delegate(AsyncOperationHandle<T> handle)
					{
						AsyncLoadOrderingManager.OnCompletedLoad(handle, GlobalSettingsBase<T>._orderHandle);
					};
				}
				AsyncLoadOrderingManager.CompleteUpTo(GlobalSettingsBase<T>._loadHandle.Value, GlobalSettingsBase<T>._orderHandle);
				value = GlobalSettingsBase<T>._loadHandle.Value;
				GlobalSettingsBase<T>._instance = value.WaitForCompletion();
				if (!GlobalSettingsBase<T>._instance)
				{
					GlobalSettingsBase<T>._instance = ScriptableObject.CreateInstance<T>();
				}
				GlobalSettingsBase<T>._foundInstance = true;
			}
			return GlobalSettingsBase<T>._instance;
		}

		// Token: 0x06004EA7 RID: 20135 RVA: 0x0016DBD8 File Offset: 0x0016BDD8
		protected static void StartPreloadAddressable(string fileName)
		{
			if (GlobalSettingsBase<T>._loadHandle != null || GlobalSettingsBase<T>._delayedLoader.Item2 != null)
			{
				return;
			}
			GlobalSettingsBase<T>._delayedLoader = GlobalSettings.StartLoad<T>(fileName, delegate(AsyncOperationHandle<T>? value)
			{
				GlobalSettingsBase<T>._loadHandle = value;
			}, delegate(T value)
			{
				GlobalSettingsBase<T>._instance = value;
				GlobalSettingsBase<T>._foundInstance = true;
				GlobalSettingsBase<T>._delayedLoader.Item2 = null;
				GlobalSettingsBase<T>._delayedLoader.Item1 = null;
			});
		}

		// Token: 0x06004EA8 RID: 20136 RVA: 0x0016DC48 File Offset: 0x0016BE48
		protected static void StartUnload()
		{
			if (GlobalSettingsBase<T>._loadHandle == null)
			{
				return;
			}
			GlobalSettingsBase<T>._loadHandle.Value.Release();
			GlobalSettingsBase<T>._loadHandle = null;
			GlobalSettingsBase<T>._foundInstance = false;
			GlobalSettingsBase<T>._instance = default(T);
		}

		// Token: 0x06004EA9 RID: 20137 RVA: 0x0016DC90 File Offset: 0x0016BE90
		private void OnDestroy()
		{
			if (GlobalSettingsBase<T>._instance == this)
			{
				GlobalSettingsBase<T>._foundInstance = false;
				GlobalSettingsBase<T>._instance = default(T);
			}
		}

		// Token: 0x04004F54 RID: 20308
		private static bool _foundInstance;

		// Token: 0x04004F55 RID: 20309
		private static T _instance;

		// Token: 0x04004F56 RID: 20310
		private static AsyncOperationHandle<T>? _loadHandle;

		// Token: 0x04004F57 RID: 20311
		private static int _orderHandle;

		// Token: 0x04004F58 RID: 20312
		[TupleElementNames(new string[]
		{
			"Runner",
			"Routine"
		})]
		private static ValueTuple<GlobalSettings, Coroutine> _delayedLoader;
	}
}
