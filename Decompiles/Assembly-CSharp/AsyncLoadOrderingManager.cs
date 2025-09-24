using System;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

// Token: 0x02000413 RID: 1043
public static class AsyncLoadOrderingManager
{
	// Token: 0x06002352 RID: 9042 RVA: 0x000A16BC File Offset: 0x0009F8BC
	public static void OnStartedLoad(AsyncOperationHandle loadQueueItem, out int loadHandle)
	{
		AsyncLoadOrderingManager._lastLoadHandle++;
		loadHandle = AsyncLoadOrderingManager._lastLoadHandle;
		if (AsyncLoadOrderingManager._orderedLoads == null)
		{
			AsyncLoadOrderingManager._orderedLoads = new List<ValueTuple<int, AsyncOperationHandle>>();
		}
		AsyncLoadOrderingManager._orderedLoads.Add(new ValueTuple<int, AsyncOperationHandle>(loadHandle, loadQueueItem));
	}

	// Token: 0x06002353 RID: 9043 RVA: 0x000A16F4 File Offset: 0x0009F8F4
	public static void CompleteUpTo(AsyncOperationHandle loadQueueItem, int loadHandle)
	{
		if (AsyncLoadOrderingManager._orderedLoads == null)
		{
			return;
		}
		if (AsyncLoadOrderingManager._tempList == null)
		{
			AsyncLoadOrderingManager._tempList = new List<ValueTuple<int, AsyncOperationHandle>>(AsyncLoadOrderingManager._orderedLoads.Count);
		}
		foreach (ValueTuple<int, AsyncOperationHandle> valueTuple in AsyncLoadOrderingManager._orderedLoads)
		{
			int item = valueTuple.Item1;
			AsyncOperationHandle item2 = valueTuple.Item2;
			if (item == loadHandle)
			{
				break;
			}
			AsyncLoadOrderingManager._tempList.Add(new ValueTuple<int, AsyncOperationHandle>(item, item2));
		}
		foreach (ValueTuple<int, AsyncOperationHandle> valueTuple2 in AsyncLoadOrderingManager._tempList)
		{
			AsyncOperationHandle item3 = valueTuple2.Item2;
			item3.WaitForCompletion();
		}
		AsyncLoadOrderingManager._tempList.Clear();
	}

	// Token: 0x06002354 RID: 9044 RVA: 0x000A17D8 File Offset: 0x0009F9D8
	public static void OnCompletedLoad(AsyncOperationHandle loadQueueItem, int loadHandle)
	{
		if (AsyncLoadOrderingManager._orderedLoads == null)
		{
			return;
		}
		for (int i = AsyncLoadOrderingManager._orderedLoads.Count - 1; i >= 0; i--)
		{
			if (AsyncLoadOrderingManager._orderedLoads[i].Item1 == loadHandle)
			{
				AsyncLoadOrderingManager._orderedLoads.RemoveAt(i);
			}
		}
		if (AsyncLoadOrderingManager._orderedLoads.Count != 0)
		{
			return;
		}
		AsyncLoadOrderingManager._orderedLoads = null;
		if (AsyncLoadOrderingManager._onLoadsCompleteActionQueue == null)
		{
			return;
		}
		Action action;
		while (AsyncLoadOrderingManager._onLoadsCompleteActionQueue.TryDequeue(out action))
		{
			action();
		}
		AsyncLoadOrderingManager._onLoadsCompleteActionQueue = null;
	}

	// Token: 0x06002355 RID: 9045 RVA: 0x000A185C File Offset: 0x0009FA5C
	public static void DoActionAfterAllLoadsComplete(Action action)
	{
		List<ValueTuple<int, AsyncOperationHandle>> orderedLoads = AsyncLoadOrderingManager._orderedLoads;
		if (orderedLoads == null || orderedLoads.Count <= 0)
		{
			action();
			return;
		}
		if (AsyncLoadOrderingManager._onLoadsCompleteActionQueue == null)
		{
			AsyncLoadOrderingManager._onLoadsCompleteActionQueue = new Queue<Action>();
		}
		AsyncLoadOrderingManager._onLoadsCompleteActionQueue.Enqueue(action);
	}

	// Token: 0x040021EE RID: 8686
	private static List<ValueTuple<int, AsyncOperationHandle>> _orderedLoads;

	// Token: 0x040021EF RID: 8687
	private static List<ValueTuple<int, AsyncOperationHandle>> _tempList;

	// Token: 0x040021F0 RID: 8688
	private static int _lastLoadHandle;

	// Token: 0x040021F1 RID: 8689
	private static Queue<Action> _onLoadsCompleteActionQueue;
}
