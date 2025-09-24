using System;
using System.Threading;
using UnityEngine;

// Token: 0x0200046E RID: 1134
public static class UnityThreadContext
{
	// Token: 0x17000498 RID: 1176
	// (get) Token: 0x0600289B RID: 10395 RVA: 0x000B2DCA File Offset: 0x000B0FCA
	private static bool IsInitialised
	{
		get
		{
			if (!UnityThreadContext.m_isInitialised)
			{
				UnityThreadContext.SafeInitThreadContext();
			}
			return UnityThreadContext.m_isInitialised;
		}
	}

	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x0600289C RID: 10396 RVA: 0x000B2DDD File Offset: 0x000B0FDD
	// (set) Token: 0x0600289D RID: 10397 RVA: 0x000B2DE4 File Offset: 0x000B0FE4
	private static int MainThreadID { get; set; }

	// Token: 0x1700049A RID: 1178
	// (get) Token: 0x0600289E RID: 10398 RVA: 0x000B2DEC File Offset: 0x000B0FEC
	public static bool IsUnityMainThread
	{
		get
		{
			return UnityThreadContext.IsInitialised && Thread.CurrentThread.ManagedThreadId == UnityThreadContext.MainThreadID;
		}
	}

	// Token: 0x0600289F RID: 10399 RVA: 0x000B2E08 File Offset: 0x000B1008
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		UnityThreadContext.InitThreadContext();
	}

	// Token: 0x060028A0 RID: 10400 RVA: 0x000B2E0F File Offset: 0x000B100F
	private static void InitThreadContext()
	{
		if (!UnityThreadContext.m_isInitialising)
		{
			UnityThreadContext.m_isInitialising = true;
			UnityThreadContext.CreateInitialiser();
		}
	}

	// Token: 0x060028A1 RID: 10401 RVA: 0x000B2E23 File Offset: 0x000B1023
	private static void SafeInitThreadContext()
	{
		if (!UnityThreadContext.m_isInitialising)
		{
			UnityThreadContext.m_isInitialising = true;
			CoreLoop.InvokeSafe(new Action(UnityThreadContext.CreateInitialiser));
		}
	}

	// Token: 0x060028A2 RID: 10402 RVA: 0x000B2E44 File Offset: 0x000B1044
	private static void CreateInitialiser()
	{
		UnityThreadContextInitialiser initialiser = new GameObject("Thread Initialiser").AddComponent<UnityThreadContextInitialiser>();
		initialiser.SetCallback(delegate(int id)
		{
			UnityThreadContext.MainThreadID = id;
			UnityThreadContext.m_isInitialising = false;
			UnityThreadContext.m_isInitialised = true;
			Object.Destroy(initialiser.gameObject);
		});
	}

	// Token: 0x040024AE RID: 9390
	private static bool m_isInitialising;

	// Token: 0x040024AF RID: 9391
	private static bool m_isInitialised;
}
