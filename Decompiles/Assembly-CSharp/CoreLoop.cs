using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200044D RID: 1101
public class CoreLoop : MonoBehaviour
{
	// Token: 0x060026AB RID: 9899 RVA: 0x000AEF43 File Offset: 0x000AD143
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		GameObject gameObject = new GameObject("CoreLoop");
		CoreLoop.instance = gameObject.AddComponent<CoreLoop>();
		Object.DontDestroyOnLoad(gameObject);
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000AEF5F File Offset: 0x000AD15F
	public static void InvokeNext(Action action)
	{
		CoreLoop.invokeNextActions.Add(action);
		CoreLoop.EnqueueInvokeNext();
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000AEF71 File Offset: 0x000AD171
	public static void InvokeSafe(Action action)
	{
		if (action == null)
		{
			return;
		}
		CoreLoop.InvokeOnGameThread(action);
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000AEF7D File Offset: 0x000AD17D
	private static void EnqueueInvokeNext()
	{
		if (!CoreLoop.isFiringInvokeNext)
		{
			CoreLoop.isFiringInvokeNext = true;
			CoreLoop.instance.Invoke("FireInvokeNext", 0f);
		}
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x000AEFA0 File Offset: 0x000AD1A0
	protected void FireInvokeNext()
	{
		CoreLoop.isFiringInvokeNext = false;
		List<Action> list = CoreLoop.invokeNextActions;
		List<Action> list2 = CoreLoop.invokeNextActionsBuffer;
		CoreLoop.invokeNextActionsBuffer = list;
		CoreLoop.invokeNextActions = list2;
		for (int i = 0; i < CoreLoop.invokeNextActionsBuffer.Count; i++)
		{
			Action action = CoreLoop.invokeNextActionsBuffer[i];
			if (action != null)
			{
				try
				{
					action();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
		}
		CoreLoop.invokeNextActionsBuffer.Clear();
	}

	// Token: 0x060026B0 RID: 9904 RVA: 0x000AF018 File Offset: 0x000AD218
	public static void InvokeOnGameThread(Action action)
	{
		object obj = CoreLoop.invokeOnGameThreadMutex;
		lock (obj)
		{
			CoreLoop.pendingActions.Add(action);
		}
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x000AF05C File Offset: 0x000AD25C
	protected void Update()
	{
		object obj = CoreLoop.invokeOnGameThreadMutex;
		lock (obj)
		{
			List<Action> list = CoreLoop.pendingActions;
			List<Action> list2 = CoreLoop.executingActions;
			CoreLoop.executingActions = list;
			CoreLoop.pendingActions = list2;
		}
		if (CoreLoop.executingActions.Count > 0)
		{
			for (int i = 0; i < CoreLoop.executingActions.Count; i++)
			{
				CoreLoop.InvokeNext(CoreLoop.executingActions[i]);
			}
			CoreLoop.executingActions.Clear();
		}
		for (int j = 0; j < CoreLoop.delayedInvokes.Count; j++)
		{
			CoreLoop.DelayedInvoke delayedInvoke = CoreLoop.delayedInvokes[j];
			delayedInvoke.TimeRemaining -= Time.unscaledDeltaTime;
			if (delayedInvoke.TimeRemaining <= 0f)
			{
				CoreLoop.delayedInvokes.RemoveAt(j--);
				CoreLoop.InvokeNext(delayedInvoke.Action);
			}
		}
	}

	// Token: 0x04002406 RID: 9222
	private static CoreLoop instance;

	// Token: 0x04002407 RID: 9223
	private static List<Action> invokeNextActions = new List<Action>();

	// Token: 0x04002408 RID: 9224
	private static List<Action> invokeNextActionsBuffer = new List<Action>();

	// Token: 0x04002409 RID: 9225
	private static bool isFiringInvokeNext = false;

	// Token: 0x0400240A RID: 9226
	private static List<CoreLoop.DelayedInvoke> delayedInvokes = new List<CoreLoop.DelayedInvoke>();

	// Token: 0x0400240B RID: 9227
	private static object invokeOnGameThreadMutex = new object();

	// Token: 0x0400240C RID: 9228
	private static List<Action> pendingActions = new List<Action>();

	// Token: 0x0400240D RID: 9229
	private static List<Action> executingActions = new List<Action>();

	// Token: 0x02001735 RID: 5941
	private class DelayedInvoke
	{
		// Token: 0x04008DA1 RID: 36257
		public float TimeRemaining;

		// Token: 0x04008DA2 RID: 36258
		public Action Action;
	}
}
