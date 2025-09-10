using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GlobalSettings
{
	// Token: 0x020008CB RID: 2251
	public class GlobalSettings : MonoBehaviour
	{
		// Token: 0x06004EA3 RID: 20131 RVA: 0x0016DA14 File Offset: 0x0016BC14
		public static ValueTuple<GlobalSettings, Coroutine> StartLoad<T>(string fileName, Action<AsyncOperationHandle<T>?> onLoadStarted, Action<T> onComplete)
		{
			GlobalSettings component = new GameObject("GlobalSettings Loader " + fileName, new Type[]
			{
				typeof(GlobalSettings)
			}).GetComponent<GlobalSettings>();
			Object.DontDestroyOnLoad(component);
			Coroutine item = component.StartCoroutine(component.Load<T>(fileName, onLoadStarted, onComplete));
			return new ValueTuple<GlobalSettings, Coroutine>(component, item);
		}

		// Token: 0x06004EA4 RID: 20132 RVA: 0x0016DA65 File Offset: 0x0016BC65
		private IEnumerator Load<T>(string fileName, Action<AsyncOperationHandle<T>?> onLoadStarted, Action<T> onComplete)
		{
			yield return new WaitForEndOfFrame();
			AsyncOperationHandle<T> asyncOperationHandle = Addressables.LoadAssetAsync<T>("GlobalSettings/" + fileName + ".asset");
			int orderHandle;
			AsyncLoadOrderingManager.OnStartedLoad(asyncOperationHandle, out orderHandle);
			onLoadStarted(new AsyncOperationHandle<T>?(asyncOperationHandle));
			asyncOperationHandle.Completed += delegate(AsyncOperationHandle<T> handle)
			{
				AsyncLoadOrderingManager.OnCompletedLoad(handle, orderHandle);
				onComplete(handle.Result);
			};
			Object.Destroy(base.gameObject);
			yield break;
		}
	}
}
