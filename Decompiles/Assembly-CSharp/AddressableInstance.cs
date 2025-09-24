using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000055 RID: 85
public sealed class AddressableInstance : MonoBehaviour
{
	// Token: 0x06000235 RID: 565 RVA: 0x0000DC68 File Offset: 0x0000BE68
	private void OnDestroy()
	{
		if (this.operationHandle.IsValid())
		{
			Addressables.ReleaseInstance(this.operationHandle);
		}
	}

	// Token: 0x06000236 RID: 566 RVA: 0x0000DC83 File Offset: 0x0000BE83
	public static void AddInstanceHelper(GameObject gameObject, AsyncOperationHandle<GameObject> operationHandle)
	{
		if (gameObject == null)
		{
			return;
		}
		gameObject.AddComponent<AddressableInstance>().operationHandle = operationHandle;
	}

	// Token: 0x040001E8 RID: 488
	private AsyncOperationHandle<GameObject> operationHandle;
}
