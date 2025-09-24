using System;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000057 RID: 87
public static class AddressableInstanceExtensions
{
	// Token: 0x06000245 RID: 581 RVA: 0x0000DFBE File Offset: 0x0000C1BE
	public static void AddInstanceHelper(this GameObject gameObject, AsyncOperationHandle<GameObject> operationHandle)
	{
		AddressableInstance.AddInstanceHelper(gameObject, operationHandle);
	}
}
