using System;
using System.Collections.Generic;

// Token: 0x02000464 RID: 1124
public sealed class FetchDataRequest
{
	// Token: 0x1700048C RID: 1164
	// (get) Token: 0x06002853 RID: 10323 RVA: 0x000B2144 File Offset: 0x000B0344
	// (set) Token: 0x06002854 RID: 10324 RVA: 0x000B2188 File Offset: 0x000B0388
	public FetchDataRequest.Status State
	{
		get
		{
			object obj = this.statusLock;
			FetchDataRequest.Status result;
			lock (obj)
			{
				result = this.status;
			}
			return result;
		}
		set
		{
			bool flag = false;
			object obj = this.statusLock;
			lock (obj)
			{
				flag = (value == FetchDataRequest.Status.Completed && this.status != FetchDataRequest.Status.Completed);
				if (!flag)
				{
					this.status = value;
				}
			}
			if (flag)
			{
				this.OnFetchCompleted();
				obj = this.statusLock;
				lock (obj)
				{
					this.status = value;
				}
				this.OnFetchCompleted();
				this.RunCallbacks();
			}
		}
	}

	// Token: 0x1700048D RID: 1165
	// (get) Token: 0x06002855 RID: 10325 RVA: 0x000B2228 File Offset: 0x000B0428
	// (set) Token: 0x06002856 RID: 10326 RVA: 0x000B2230 File Offset: 0x000B0430
	public string Name { get; set; }

	// Token: 0x1700048E RID: 1166
	// (get) Token: 0x06002857 RID: 10327 RVA: 0x000B223C File Offset: 0x000B043C
	// (set) Token: 0x06002858 RID: 10328 RVA: 0x000B2280 File Offset: 0x000B0480
	public List<RestorePointFileWrapper> RestorePoints
	{
		get
		{
			object obj = this.listLock;
			List<RestorePointFileWrapper> result;
			lock (obj)
			{
				result = this.restorePoints;
			}
			return result;
		}
		set
		{
			object obj = this.listLock;
			lock (obj)
			{
				this.restorePoints = value;
			}
		}
	}

	// Token: 0x06002859 RID: 10329 RVA: 0x000B22C4 File Offset: 0x000B04C4
	public FetchDataRequest()
	{
		this.statusLock = new object();
		this.listLock = new object();
	}

	// Token: 0x1700048F RID: 1167
	// (get) Token: 0x0600285A RID: 10330 RVA: 0x000B22E2 File Offset: 0x000B04E2
	public static FetchDataRequest Error
	{
		get
		{
			return new FetchDataRequest
			{
				status = FetchDataRequest.Status.Failed
			};
		}
	}

	// Token: 0x0600285B RID: 10331 RVA: 0x000B22F0 File Offset: 0x000B04F0
	public void AddResult(RestorePointFileWrapper data)
	{
		object obj = this.listLock;
		lock (obj)
		{
			for (int i = 0; i < this.restorePoints.Count; i++)
			{
				RestorePointFileWrapper restorePointFileWrapper = this.restorePoints[i];
				if (data.number >= restorePointFileWrapper.number)
				{
					this.restorePoints.Insert(i, data);
					return;
				}
			}
			this.restorePoints.Add(data);
		}
	}

	// Token: 0x0600285C RID: 10332 RVA: 0x000B2378 File Offset: 0x000B0578
	public void RunOnFetchComplete(Action<FetchDataRequest> callback)
	{
		object obj = this.statusLock;
		lock (obj)
		{
			if (this.status == FetchDataRequest.Status.Completed || this.status == FetchDataRequest.Status.Failed)
			{
				if (callback != null)
				{
					callback(this);
				}
			}
			else
			{
				this.callbacks = (Action<FetchDataRequest>)Delegate.Combine(this.callbacks, callback);
			}
		}
	}

	// Token: 0x0600285D RID: 10333 RVA: 0x000B23EC File Offset: 0x000B05EC
	public void RunOnComplete(Action<FetchDataRequest> callback)
	{
		object obj = this.statusLock;
		lock (obj)
		{
			if (this.status == FetchDataRequest.Status.Completed || this.status == FetchDataRequest.Status.Failed)
			{
				if (callback != null)
				{
					callback(this);
				}
			}
			else
			{
				this.callbacks = (Action<FetchDataRequest>)Delegate.Combine(this.callbacks, callback);
			}
		}
	}

	// Token: 0x0600285E RID: 10334 RVA: 0x000B2460 File Offset: 0x000B0660
	private void OnFetchCompleted()
	{
		if (this.fetchCallbacks != null)
		{
			Action<FetchDataRequest> action = this.fetchCallbacks;
			this.fetchCallbacks = null;
			action(this);
		}
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x000B247D File Offset: 0x000B067D
	private void RunCallbacks()
	{
		if (this.callbacks != null)
		{
			Action<FetchDataRequest> action = this.callbacks;
			this.callbacks = null;
			action(this);
		}
	}

	// Token: 0x04002460 RID: 9312
	private FetchDataRequest.Status status;

	// Token: 0x04002461 RID: 9313
	private readonly object statusLock;

	// Token: 0x04002462 RID: 9314
	private readonly object listLock;

	// Token: 0x04002463 RID: 9315
	private Action<FetchDataRequest> fetchCallbacks;

	// Token: 0x04002464 RID: 9316
	private Action<FetchDataRequest> callbacks;

	// Token: 0x04002465 RID: 9317
	private List<RestorePointFileWrapper> restorePoints;

	// Token: 0x0200177C RID: 6012
	public enum Status
	{
		// Token: 0x04008E47 RID: 36423
		NotStarted,
		// Token: 0x04008E48 RID: 36424
		InProgress,
		// Token: 0x04008E49 RID: 36425
		Completed,
		// Token: 0x04008E4A RID: 36426
		Failed
	}
}
