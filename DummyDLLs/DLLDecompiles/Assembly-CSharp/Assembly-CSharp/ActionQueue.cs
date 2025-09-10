using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200044C RID: 1100
public class ActionQueue
{
	// Token: 0x060026A8 RID: 9896 RVA: 0x000AEE98 File Offset: 0x000AD098
	public ActionQueue()
	{
		this.pendingActions = new List<ActionQueue.ActionQueueCallback>();
		this.isRunning = false;
	}

	// Token: 0x060026A9 RID: 9897 RVA: 0x000AEEB4 File Offset: 0x000AD0B4
	public void Next()
	{
		while (this.pendingActions.Count > 0)
		{
			ActionQueue.ActionQueueCallback actionQueueCallback = this.pendingActions[0];
			this.pendingActions.RemoveAt(0);
			try
			{
				actionQueueCallback(new Action(this.Next));
				return;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}
		this.isRunning = false;
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x000AEF1C File Offset: 0x000AD11C
	public void Enqueue(ActionQueue.ActionQueueCallback action)
	{
		if (action == null)
		{
			return;
		}
		this.pendingActions.Add(action);
		if (!this.isRunning)
		{
			this.isRunning = true;
			this.Next();
		}
	}

	// Token: 0x04002404 RID: 9220
	private readonly List<ActionQueue.ActionQueueCallback> pendingActions;

	// Token: 0x04002405 RID: 9221
	private bool isRunning;

	// Token: 0x02001734 RID: 5940
	// (Invoke) Token: 0x06008D10 RID: 36112
	public delegate void ActionQueueCallback(Action next);
}
