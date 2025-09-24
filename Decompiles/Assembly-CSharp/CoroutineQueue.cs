using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200044E RID: 1102
public class CoroutineQueue
{
	// Token: 0x060026B4 RID: 9908 RVA: 0x000AF1A3 File Offset: 0x000AD3A3
	public CoroutineQueue(MonoBehaviour runner)
	{
		this.runner = runner;
		this.pendingCoroutines = new List<IEnumerator>();
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x000AF1C0 File Offset: 0x000AD3C0
	public void Enqueue(IEnumerator coroutine)
	{
		if (!this.runner || !this.runner.isActiveAndEnabled)
		{
			return;
		}
		this.pendingCoroutines.Add(coroutine);
		if (!this.isRunning)
		{
			this.runner.StartCoroutine(this.Run());
		}
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x000AF20E File Offset: 0x000AD40E
	public void Clear()
	{
		this.pendingCoroutines.Clear();
	}

	// Token: 0x060026B7 RID: 9911 RVA: 0x000AF21B File Offset: 0x000AD41B
	public IEnumerator Run()
	{
		this.isRunning = true;
		while (this.pendingCoroutines.Count > 0)
		{
			IEnumerator coroutine = this.pendingCoroutines[0];
			this.pendingCoroutines.RemoveAt(0);
			for (;;)
			{
				bool flag;
				try
				{
					flag = coroutine.MoveNext();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					break;
				}
				if (!flag)
				{
					break;
				}
				yield return coroutine.Current;
			}
			coroutine = null;
		}
		this.isRunning = false;
		yield break;
	}

	// Token: 0x0400240E RID: 9230
	private readonly List<IEnumerator> pendingCoroutines;

	// Token: 0x0400240F RID: 9231
	private readonly MonoBehaviour runner;

	// Token: 0x04002410 RID: 9232
	private bool isRunning;
}
