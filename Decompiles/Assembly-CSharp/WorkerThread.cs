using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

// Token: 0x02000475 RID: 1141
public sealed class WorkerThread : IDisposable
{
	// Token: 0x060028BD RID: 10429 RVA: 0x000B3158 File Offset: 0x000B1358
	public WorkerThread(ThreadPriority threadPriority = ThreadPriority.Normal)
	{
		this.workQueue = new ConcurrentQueue<Action>();
		this.workReadyHandle = new ManualResetEvent(false);
		this.workerThread = new Thread(new ThreadStart(this.RunWork))
		{
			IsBackground = true,
			Priority = threadPriority
		};
		this.workerThread.Start();
	}

	// Token: 0x060028BE RID: 10430 RVA: 0x000B31B4 File Offset: 0x000B13B4
	~WorkerThread()
	{
		this.Dispose(false);
	}

	// Token: 0x060028BF RID: 10431 RVA: 0x000B31E4 File Offset: 0x000B13E4
	public void Dispose()
	{
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	// Token: 0x060028C0 RID: 10432 RVA: 0x000B31F3 File Offset: 0x000B13F3
	private void Dispose(bool disposing)
	{
		if (!this.isDisposed)
		{
			this.isDisposed = true;
			this.RequestShutdown();
		}
	}

	// Token: 0x060028C1 RID: 10433 RVA: 0x000B320A File Offset: 0x000B140A
	private void RequestShutdown()
	{
		this.cleanUpRequested = true;
		this.workReadyHandle.Set();
	}

	// Token: 0x060028C2 RID: 10434 RVA: 0x000B321F File Offset: 0x000B141F
	private void CleanupResources()
	{
		this.workReadyHandle.Close();
	}

	// Token: 0x060028C3 RID: 10435 RVA: 0x000B322C File Offset: 0x000B142C
	public void EnqueueWork(Action work)
	{
		if (work == null)
		{
			return;
		}
		if (this.cleanUpRequested)
		{
			return;
		}
		this.workQueue.Enqueue(work);
		this.workReadyHandle.Set();
	}

	// Token: 0x060028C4 RID: 10436 RVA: 0x000B3254 File Offset: 0x000B1454
	private void RunWork()
	{
		Action action = null;
		for (;;)
		{
			if (!this.workQueue.TryDequeue(out action))
			{
				if (this.cleanUpRequested)
				{
					break;
				}
				this.workReadyHandle.Reset();
			}
			if (action != null)
			{
				try
				{
					action();
					continue;
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					continue;
				}
			}
			this.workReadyHandle.WaitOne();
		}
		this.CleanupResources();
	}

	// Token: 0x1700049C RID: 1180
	// (get) Token: 0x060028C5 RID: 10437 RVA: 0x000B32BC File Offset: 0x000B14BC
	// (set) Token: 0x060028C6 RID: 10438 RVA: 0x000B32C9 File Offset: 0x000B14C9
	public ThreadPriority ThreadPriority
	{
		get
		{
			return this.workerThread.Priority;
		}
		set
		{
			this.workerThread.Priority = value;
		}
	}

	// Token: 0x040024BF RID: 9407
	private readonly ConcurrentQueue<Action> workQueue;

	// Token: 0x040024C0 RID: 9408
	private readonly ManualResetEvent workReadyHandle;

	// Token: 0x040024C1 RID: 9409
	private readonly Thread workerThread;

	// Token: 0x040024C2 RID: 9410
	private bool isDisposed;

	// Token: 0x040024C3 RID: 9411
	private bool cleanUpRequested;
}
