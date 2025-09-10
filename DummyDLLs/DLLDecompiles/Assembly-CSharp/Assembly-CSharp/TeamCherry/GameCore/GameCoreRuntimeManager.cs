using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;
using XGamingRuntime;
using XGamingRuntime.Interop;

namespace TeamCherry.GameCore
{
	// Token: 0x020008B4 RID: 2228
	public static class GameCoreRuntimeManager
	{
		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06004CFD RID: 19709 RVA: 0x00169AD8 File Offset: 0x00167CD8
		// (set) Token: 0x06004CFE RID: 19710 RVA: 0x00169ADF File Offset: 0x00167CDF
		public static bool Initialized { get; private set; }

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x06004CFF RID: 19711 RVA: 0x00169AE7 File Offset: 0x00167CE7
		public static uint TitleID
		{
			get
			{
				return GameCoreRuntimeManager.s_TitleId;
			}
		}

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06004D00 RID: 19712 RVA: 0x00169AEE File Offset: 0x00167CEE
		// (set) Token: 0x06004D01 RID: 19713 RVA: 0x00169AF5 File Offset: 0x00167CF5
		public static string SCID { get; private set; }

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x06004D02 RID: 19714 RVA: 0x00169AFD File Offset: 0x00167CFD
		// (set) Token: 0x06004D03 RID: 19715 RVA: 0x00169B04 File Offset: 0x00167D04
		public static bool SaveSystemInitialised
		{
			get
			{
				return GameCoreRuntimeManager.saveSystemInitialised;
			}
			private set
			{
				if (GameCoreRuntimeManager.saveSystemInitialised != value)
				{
					GameCoreRuntimeManager.saveSystemInitialised = value;
					Platform.Current.LoadSharedDataAndNotify(value);
				}
			}
		}

		// Token: 0x140000F3 RID: 243
		// (add) Token: 0x06004D04 RID: 19716 RVA: 0x00169B20 File Offset: 0x00167D20
		// (remove) Token: 0x06004D05 RID: 19717 RVA: 0x00169B54 File Offset: 0x00167D54
		public static event Action OnSaveSystemInitialised;

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06004D06 RID: 19718 RVA: 0x00169B87 File Offset: 0x00167D87
		// (set) Token: 0x06004D07 RID: 19719 RVA: 0x00169B8E File Offset: 0x00167D8E
		public static string GamerTag { get; private set; }

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06004D08 RID: 19720 RVA: 0x00169B96 File Offset: 0x00167D96
		// (set) Token: 0x06004D09 RID: 19721 RVA: 0x00169B9D File Offset: 0x00167D9D
		public static Texture2D UserDisplayImage { get; private set; }

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06004D0A RID: 19722 RVA: 0x00169BA8 File Offset: 0x00167DA8
		public static bool UserSignedIn
		{
			get
			{
				if (!GameCoreRuntimeManager.isAddingUser)
				{
					GameCoreRuntimeManager.User user = GameCoreRuntimeManager.activeUser;
					return user != null && user.UserState == GameCoreRuntimeManager.UserState.SignedIn;
				}
				return false;
			}
		}

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06004D0B RID: 19723 RVA: 0x00169BD4 File Offset: 0x00167DD4
		public static bool UserSignInPending
		{
			get
			{
				return GameCoreRuntimeManager.isAddingUser;
			}
		}

		// Token: 0x06004D0C RID: 19724 RVA: 0x00169BE0 File Offset: 0x00167DE0
		public static void InitializeRuntime(bool forceInitialization = false)
		{
			if (GameCoreRuntimeManager.Initialized && !forceInitialization)
			{
				return;
			}
			GameCoreRuntimeManager.InitializeHresultToFriendlyErrorLookup();
			GameCoreRuntimeManager.SCID = GameCoreRuntimeManager.GetSCID();
			if (!GameCoreRuntimeManager.Succeeded(SDK.XGameRuntimeInitialize(), "Initializing Game Core runtime"))
			{
				return;
			}
			GameCoreRuntimeManager.StartAsyncDispatchThread();
			GameCoreRuntimeManager.Initialized = true;
			GameCoreRuntimeManager.Succeeded(SDK.XGameGetXboxTitleId(out GameCoreRuntimeManager.s_TitleId), "Retrieving Title ID");
			GameCoreRuntimeManager.Succeeded(SDK.XBL.XblInitialize(GameCoreRuntimeManager.SCID), "Initialize Xbox Live");
			GameCoreRuntimeManager.RegisterEvents();
			GameCoreRuntimeManager.SignIn();
		}

		// Token: 0x06004D0D RID: 19725 RVA: 0x00169C58 File Offset: 0x00167E58
		public static void CleanUpRuntime()
		{
			if (!GameCoreRuntimeManager.Initialized)
			{
				return;
			}
			GameCoreRuntimeManager.CloseSaveHandler();
			foreach (GameCoreRuntimeManager.User user in GameCoreRuntimeManager.connectedUsers.Values)
			{
				user.Dispose();
			}
			GameCoreRuntimeManager.connectedUsers.Clear();
			if (GameCoreRuntimeManager.dispatchThread != null)
			{
				GameCoreRuntimeManager.threadEnding = true;
				GameCoreRuntimeManager.dispatchThread.Join();
				GameCoreRuntimeManager.dispatchThread = null;
			}
			SDK.XGameRuntimeUninitialize();
			GameCoreRuntimeManager.UnregisterEvents();
			GameCoreRuntimeManager.Initialized = false;
		}

		// Token: 0x06004D0E RID: 19726 RVA: 0x00169CF4 File Offset: 0x00167EF4
		public static void CleanUpUsers()
		{
			List<ulong> list = new List<ulong>();
			foreach (KeyValuePair<ulong, GameCoreRuntimeManager.User> keyValuePair in GameCoreRuntimeManager.connectedUsers)
			{
				if (keyValuePair.Value != GameCoreRuntimeManager.activeUser)
				{
					list.Add(keyValuePair.Key);
					keyValuePair.Value.Dispose();
				}
			}
			foreach (ulong key in list)
			{
				GameCoreRuntimeManager.connectedUsers.Remove(key);
			}
		}

		// Token: 0x06004D0F RID: 19727 RVA: 0x00169DB0 File Offset: 0x00167FB0
		public static void ClearAllUsers()
		{
			foreach (KeyValuePair<ulong, GameCoreRuntimeManager.User> keyValuePair in GameCoreRuntimeManager.connectedUsers)
			{
				GameCoreRuntimeManager.User value = keyValuePair.Value;
				if (value != null)
				{
					value.Dispose();
				}
			}
			GameCoreRuntimeManager.connectedUsers.Clear();
		}

		// Token: 0x06004D10 RID: 19728 RVA: 0x00169E18 File Offset: 0x00168018
		private static void RegisterEvents()
		{
			if (!GameCoreRuntimeManager.registerSuspendEvent)
			{
				GameCoreRuntimeManager.registerSuspendEvent = true;
				GameCoreRuntimeManager.RegisterUserChangeEvents();
			}
		}

		// Token: 0x06004D11 RID: 19729 RVA: 0x00169E2C File Offset: 0x0016802C
		private static void UnregisterEvents()
		{
			if (GameCoreRuntimeManager.registerSuspendEvent)
			{
				GameCoreRuntimeManager.registerSuspendEvent = false;
				GameCoreRuntimeManager.UnregisterUserEvents();
			}
		}

		// Token: 0x06004D12 RID: 19730 RVA: 0x00169E40 File Offset: 0x00168040
		private static string GetSCID()
		{
			return "00000000-0000-0000-0000-0000636f5860";
		}

		// Token: 0x06004D13 RID: 19731 RVA: 0x00169E48 File Offset: 0x00168048
		private static void InitializeHresultToFriendlyErrorLookup()
		{
			if (GameCoreRuntimeManager._hresultToFriendlyErrorLookup != null)
			{
				return;
			}
			GameCoreRuntimeManager._hresultToFriendlyErrorLookup = new Dictionary<int, string>();
			GameCoreRuntimeManager._hresultToFriendlyErrorLookup.Add(-2143330041, "IAP_UNEXPECTED: Does the player you are signed in as have a license for the game? You can get one by downloading your game from the store and purchasing it first. If you can't find your game in the store, have you published it in Partner Center?");
			GameCoreRuntimeManager._hresultToFriendlyErrorLookup.Add(-1994108656, "E_GAMEUSER_NO_PACKAGE_IDENTITY: Are you trying to call GDK APIs from the Unity editor? To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once yourgame is launched.");
			GameCoreRuntimeManager._hresultToFriendlyErrorLookup.Add(-1994129152, "E_GAMERUNTIME_NOT_INITIALIZED: Are you trying to call GDK APIs from the Unity editor? To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once yourgame is launched.");
			GameCoreRuntimeManager._hresultToFriendlyErrorLookup.Add(-2015559675, "AM_E_XAST_UNEXPECTED: Have you added the Windows 10 PC platform on the Xbox Settings page in Partner Center? Learn more: aka.ms/sandboxtroubleshootingguide");
		}

		// Token: 0x06004D14 RID: 19732 RVA: 0x00169EB8 File Offset: 0x001680B8
		private static void StartAsyncDispatchThread()
		{
			if (GameCoreRuntimeManager.dispatchThread == null)
			{
				GameCoreRuntimeManager.dispatchThread = new Thread(new ThreadStart(GameCoreRuntimeManager.DispatchGXDKTaskQueue))
				{
					Name = "GXDK Task Queue Dispatch",
					IsBackground = true
				};
				GameCoreRuntimeManager.threadEnding = false;
				GameCoreRuntimeManager.threadActive = true;
				GameCoreRuntimeManager.dispatchThread.Start();
			}
		}

		// Token: 0x06004D15 RID: 19733 RVA: 0x00169F10 File Offset: 0x00168110
		private static void DispatchGXDKTaskQueue()
		{
			while (GameCoreRuntimeManager.threadActive)
			{
				if (GameCoreRuntimeManager.reconfirmUser)
				{
					GameCoreRuntimeManager.reconfirmUser = false;
					GameCoreRuntimeManager.ConfirmActiveUser();
				}
				if (GameCoreRuntimeManager.activeUser == null && GameCoreRuntimeManager.signInRequested)
				{
					GameCoreRuntimeManager.SignIn();
				}
				if (GameCoreRuntimeManager.achievementUpdateQueued)
				{
					GameCoreRuntimeManager.FetchAchievements();
				}
				GameCoreRuntimeManager.UpdateDeferredSignOuts();
				SDK.XTaskQueueDispatch(0U);
				if (GameCoreRuntimeManager.threadEnding)
				{
					break;
				}
				Thread.Sleep(32);
			}
			GameCoreRuntimeManager.threadActive = false;
		}

		// Token: 0x06004D16 RID: 19734 RVA: 0x00169F82 File Offset: 0x00168182
		private static void OnSuspend()
		{
		}

		// Token: 0x06004D17 RID: 19735 RVA: 0x00169F84 File Offset: 0x00168184
		private static void OnResume(double secondssuspended)
		{
			CoreLoop.InvokeSafe(delegate
			{
				GameCoreRuntimeManager.InitializeRuntime(true);
				GameCoreRuntimeManager.reconfirmUser = true;
				GameCoreRuntimeManager.TrySet120hz();
			});
		}

		// Token: 0x06004D18 RID: 19736 RVA: 0x00169FAC File Offset: 0x001681AC
		private static void ConfirmActiveUser()
		{
			GameCoreRuntimeManager.reconfirmUser = false;
			GameCoreRuntimeManager.User user = GameCoreRuntimeManager.activeUser;
			XUserState xuserState;
			SDK.XUserGetState(user.userHandle, out xuserState);
			object obj;
			if (xuserState == XUserState.SignedIn)
			{
				obj = GameCoreRuntimeManager.saveLock;
				lock (obj)
				{
					GameCoreRuntimeManager.SaveSystemInitialised = false;
				}
				user.InitialiseSaveSystem(true);
				return;
			}
			if (xuserState == XUserState.SigningOut)
			{
				return;
			}
			obj = GameCoreRuntimeManager.userLock;
			lock (obj)
			{
				user.DisposeSave();
				GameCoreRuntimeManager.RemoveUser(user);
				GameCoreRuntimeManager.ClearAllUsers();
			}
		}

		// Token: 0x06004D19 RID: 19737 RVA: 0x0016A050 File Offset: 0x00168250
		public static void TrySet120hz()
		{
			if (Application.platform == RuntimePlatform.GameCoreXboxSeries)
			{
				IEnumerable<Resolution> resolutions = Screen.resolutions;
				Resolution current = Screen.currentResolution;
				List<Resolution> list = (from r in resolutions
				where r.width == current.width && r.height == current.height
				select r).ToList<Resolution>();
				Resolution? resolution = null;
				foreach (Resolution value in list)
				{
					if (Mathf.Approximately((float)value.refreshRateRatio.value, 120f))
					{
						resolution = new Resolution?(value);
						break;
					}
				}
				if (resolution == null)
				{
					using (IEnumerator<Resolution> enumerator2 = (from r in list
					where r.refreshRateRatio.value < 120.0
					orderby r.refreshRateRatio.value descending
					select r).GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							Resolution value2 = enumerator2.Current;
							resolution = new Resolution?(value2);
						}
					}
				}
				if (resolution != null)
				{
					Resolution value3 = resolution.Value;
					if (!Mathf.Approximately((float)value3.refreshRateRatio.value, (float)current.refreshRateRatio.value))
					{
						Debug.Log(string.Format("Setting resolution to {0}x{1} @ {2}Hz", value3.width, value3.height, value3.refreshRateRatio.value));
						Screen.SetResolution(value3.width, value3.height, FullScreenMode.FullScreenWindow, value3.refreshRateRatio);
						return;
					}
					Debug.Log(string.Format("Already at desired resolution: {0}x{1} @ {2}Hz", value3.width, value3.height, value3.refreshRateRatio.value));
					return;
				}
				else
				{
					Debug.LogWarning(string.Format("No matching refresh rate found for current resolution: {0}x{1} @ {2}Hz.", current.width, current.height, current.refreshRateRatio.value));
				}
			}
		}

		// Token: 0x06004D1A RID: 19738 RVA: 0x0016A2B0 File Offset: 0x001684B0
		[Conditional("UNITY_GAMECORE")]
		private static void LogNetworkStatus(string context)
		{
		}

		// Token: 0x06004D1B RID: 19739 RVA: 0x0016A2B4 File Offset: 0x001684B4
		private static void NotifyUserSaveInitialised(GameCoreRuntimeManager.User user, int hresult)
		{
			if (!GameCoreRuntimeManager.Succeeded(hresult, "Initialize game save provider"))
			{
				return;
			}
			if (user == null)
			{
				return;
			}
			if (GameCoreRuntimeManager.activeUser != user)
			{
				return;
			}
			object obj = GameCoreRuntimeManager.saveLock;
			lock (obj)
			{
				GameCoreRuntimeManager.SaveSystemInitialised = false;
				GameCoreRuntimeManager.SaveSystemInitialised = true;
				Action onSaveSystemInitialised = GameCoreRuntimeManager.OnSaveSystemInitialised;
				if (onSaveSystemInitialised != null)
				{
					onSaveSystemInitialised();
				}
				Action action;
				while (user.saveQueue.TryDequeue(out action))
				{
					try
					{
						if (action != null)
						{
							action();
						}
						continue;
					}
					catch (Exception)
					{
						continue;
					}
					break;
				}
			}
		}

		// Token: 0x06004D1C RID: 19740 RVA: 0x0016A350 File Offset: 0x00168550
		private static void CloseSaveHandler()
		{
			object obj = GameCoreRuntimeManager.saveLock;
			lock (obj)
			{
				GameCoreRuntimeManager.SaveSystemInitialised = false;
			}
		}

		// Token: 0x06004D1D RID: 19741 RVA: 0x0016A390 File Offset: 0x00168590
		public static string GetSaveSlotContainerName(int slot)
		{
			return string.Format("save{0}", slot);
		}

		// Token: 0x06004D1E RID: 19742 RVA: 0x0016A3A2 File Offset: 0x001685A2
		public static string GetMainSaveName(int slot)
		{
			return string.Format("user{0}.dat", slot);
		}

		// Token: 0x06004D1F RID: 19743 RVA: 0x0016A3B4 File Offset: 0x001685B4
		public static string GetRestoreContainerName(int slot)
		{
			return string.Format("restore{0}", slot);
		}

		// Token: 0x06004D20 RID: 19744 RVA: 0x0016A3C8 File Offset: 0x001685C8
		public static void Save(string containerName, string blobName, byte[] data, Action<bool> callback)
		{
			GameCoreRuntimeManager.<>c__DisplayClass80_0 CS$<>8__locals1 = new GameCoreRuntimeManager.<>c__DisplayClass80_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.containerName = containerName;
			CS$<>8__locals1.blobName = blobName;
			CS$<>8__locals1.data = data;
			CS$<>8__locals1.operationFriendlyName = string.Concat(new string[]
			{
				"Save : \"",
				CS$<>8__locals1.containerName,
				"\" : \"",
				CS$<>8__locals1.blobName,
				"\""
			});
			CS$<>8__locals1.user = GameCoreRuntimeManager.activeUser;
			if (CS$<>8__locals1.user != null)
			{
				object obj = GameCoreRuntimeManager.saveLock;
				lock (obj)
				{
					if (!GameCoreRuntimeManager.SaveSystemInitialised)
					{
						CS$<>8__locals1.user.saveQueue.Enqueue(new Action(CS$<>8__locals1.<Save>g__DoOperation|1));
						return;
					}
				}
				CS$<>8__locals1.<Save>g__DoOperation|1();
				return;
			}
			Debug.LogError(CS$<>8__locals1.operationFriendlyName + " failed. No user detected.");
			Action<bool> callback2 = CS$<>8__locals1.callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(false);
		}

		// Token: 0x06004D21 RID: 19745 RVA: 0x0016A4C4 File Offset: 0x001686C4
		public static void FileExists(string containerName, string blobName, Action<bool> callback)
		{
			GameCoreRuntimeManager.<>c__DisplayClass81_0 CS$<>8__locals1 = new GameCoreRuntimeManager.<>c__DisplayClass81_0();
			CS$<>8__locals1.blobName = blobName;
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.containerName = containerName;
			CS$<>8__locals1.operationFriendlyName = string.Concat(new string[]
			{
				"Check File Exists : \"",
				CS$<>8__locals1.containerName,
				"\" : \"",
				CS$<>8__locals1.blobName,
				"\""
			});
			if (CS$<>8__locals1.callback == null)
			{
				return;
			}
			CS$<>8__locals1.user = GameCoreRuntimeManager.activeUser;
			if (CS$<>8__locals1.user != null)
			{
				object obj = GameCoreRuntimeManager.saveLock;
				lock (obj)
				{
					if (!GameCoreRuntimeManager.SaveSystemInitialised)
					{
						CS$<>8__locals1.user.saveQueue.Enqueue(new Action(CS$<>8__locals1.<FileExists>g__DoOperation|1));
						return;
					}
				}
				CS$<>8__locals1.<FileExists>g__DoOperation|1();
				return;
			}
			Debug.LogError(CS$<>8__locals1.operationFriendlyName + " failed. No user detected.");
			Action<bool> callback2 = CS$<>8__locals1.callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(false);
		}

		// Token: 0x06004D22 RID: 19746 RVA: 0x0016A5C4 File Offset: 0x001687C4
		public static void EnumerateFiles(string containerName, Action<string[]> callback)
		{
			GameCoreRuntimeManager.<>c__DisplayClass82_0 CS$<>8__locals1 = new GameCoreRuntimeManager.<>c__DisplayClass82_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.containerName = containerName;
			CS$<>8__locals1.operationFriendlyName = "Enumerate \"" + CS$<>8__locals1.containerName + "\"";
			if (CS$<>8__locals1.callback == null)
			{
				return;
			}
			CS$<>8__locals1.user = GameCoreRuntimeManager.activeUser;
			if (CS$<>8__locals1.user != null)
			{
				object obj = GameCoreRuntimeManager.saveLock;
				lock (obj)
				{
					if (!GameCoreRuntimeManager.SaveSystemInitialised)
					{
						CS$<>8__locals1.user.saveQueue.Enqueue(new Action(CS$<>8__locals1.<EnumerateFiles>g__DoOperation|1));
						return;
					}
				}
				CS$<>8__locals1.<EnumerateFiles>g__DoOperation|1();
				return;
			}
			Debug.LogError(CS$<>8__locals1.operationFriendlyName + " failed. No user detected.");
			Action<string[]> callback2 = CS$<>8__locals1.callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(null);
		}

		// Token: 0x06004D23 RID: 19747 RVA: 0x0016A69C File Offset: 0x0016889C
		public static void LoadSaveData(string containerName, string blobName, Action<byte[]> callback)
		{
			GameCoreRuntimeManager.<>c__DisplayClass83_0 CS$<>8__locals1 = new GameCoreRuntimeManager.<>c__DisplayClass83_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.containerName = containerName;
			CS$<>8__locals1.blobName = blobName;
			CS$<>8__locals1.operationFriendlyName = string.Concat(new string[]
			{
				"Loaded Blob : \"",
				CS$<>8__locals1.containerName,
				"\" : \"",
				CS$<>8__locals1.blobName,
				"\""
			});
			if (CS$<>8__locals1.callback == null)
			{
				return;
			}
			CS$<>8__locals1.user = GameCoreRuntimeManager.activeUser;
			if (CS$<>8__locals1.user != null)
			{
				object obj = GameCoreRuntimeManager.saveLock;
				lock (obj)
				{
					if (!GameCoreRuntimeManager.SaveSystemInitialised)
					{
						CS$<>8__locals1.user.saveQueue.Enqueue(new Action(CS$<>8__locals1.<LoadSaveData>g__DoOperation|1));
						return;
					}
				}
				CS$<>8__locals1.<LoadSaveData>g__DoOperation|1();
				return;
			}
			Debug.LogError(CS$<>8__locals1.operationFriendlyName + " failed. No user detected.");
			Action<byte[]> callback2 = CS$<>8__locals1.callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(null);
		}

		// Token: 0x06004D24 RID: 19748 RVA: 0x0016A79C File Offset: 0x0016899C
		public static void DeleteContainer(string containerName, Action<bool> callback)
		{
			GameCoreRuntimeManager.<>c__DisplayClass84_0 CS$<>8__locals1 = new GameCoreRuntimeManager.<>c__DisplayClass84_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.containerName = containerName;
			CS$<>8__locals1.operationFriendlyName = "Deleted Container : \"" + CS$<>8__locals1.containerName + "\"";
			CS$<>8__locals1.user = GameCoreRuntimeManager.activeUser;
			if (CS$<>8__locals1.user != null)
			{
				object obj = GameCoreRuntimeManager.saveLock;
				lock (obj)
				{
					if (!GameCoreRuntimeManager.SaveSystemInitialised)
					{
						CS$<>8__locals1.user.saveQueue.Enqueue(new Action(CS$<>8__locals1.<DeleteContainer>g__DoOperation|1));
						return;
					}
				}
				CS$<>8__locals1.<DeleteContainer>g__DoOperation|1();
				return;
			}
			Debug.LogError(CS$<>8__locals1.operationFriendlyName + " failed. No user detected.");
			Action<bool> callback2 = CS$<>8__locals1.callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(false);
		}

		// Token: 0x06004D25 RID: 19749 RVA: 0x0016A86C File Offset: 0x00168A6C
		public static void DeleteBlob(string containerName, string blobName, Action<bool> callback)
		{
			GameCoreRuntimeManager.<>c__DisplayClass85_0 CS$<>8__locals1 = new GameCoreRuntimeManager.<>c__DisplayClass85_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.containerName = containerName;
			CS$<>8__locals1.blobName = blobName;
			CS$<>8__locals1.operationFriendlyName = string.Concat(new string[]
			{
				"Deleted Blob : \"",
				CS$<>8__locals1.containerName,
				"\" : \"",
				CS$<>8__locals1.blobName,
				"\""
			});
			CS$<>8__locals1.user = GameCoreRuntimeManager.activeUser;
			if (CS$<>8__locals1.user != null)
			{
				object obj = GameCoreRuntimeManager.saveLock;
				lock (obj)
				{
					if (!GameCoreRuntimeManager.SaveSystemInitialised)
					{
						CS$<>8__locals1.user.saveQueue.Enqueue(new Action(CS$<>8__locals1.<DeleteBlob>g__DoOperation|1));
						return;
					}
				}
				CS$<>8__locals1.<DeleteBlob>g__DoOperation|1();
				return;
			}
			Debug.LogError(CS$<>8__locals1.operationFriendlyName + " failed. No user detected.");
			Action<bool> callback2 = CS$<>8__locals1.callback;
			if (callback2 == null)
			{
				return;
			}
			callback2(false);
		}

		// Token: 0x06004D26 RID: 19750 RVA: 0x0016A960 File Offset: 0x00168B60
		private static void RegisterUserChangeEvents()
		{
			if (!GameCoreRuntimeManager.registeredUserEvents)
			{
				GameCoreRuntimeManager.registeredUserEvents = true;
				SDK.XUserRegisterForChangeEvent(new XUserChangeEventCallback(GameCoreRuntimeManager.UserChangeEventCallback), out GameCoreRuntimeManager.userEventToken);
			}
		}

		// Token: 0x06004D27 RID: 19751 RVA: 0x0016A986 File Offset: 0x00168B86
		private static void UnregisterUserEvents()
		{
			if (GameCoreRuntimeManager.registeredUserEvents)
			{
				GameCoreRuntimeManager.registeredUserEvents = false;
				SDK.XUserUnregisterForChangeEvent(GameCoreRuntimeManager.userEventToken);
				GameCoreRuntimeManager.userEventToken = null;
			}
		}

		// Token: 0x06004D28 RID: 19752 RVA: 0x0016A9A8 File Offset: 0x00168BA8
		private static void UserChangeEventCallback(XUserLocalId userLocalId, XUserChangeEvent eventType)
		{
			GameCoreRuntimeManager.User user;
			GameCoreRuntimeManager.connectedUsers.TryGetValue(userLocalId.value, out user);
			switch (eventType)
			{
			case XUserChangeEvent.SignedInAgain:
				if (user != null)
				{
					user.SetUserState(GameCoreRuntimeManager.UserState.SignedIn);
				}
				if (user != null)
				{
					user.InitialiseSaveSystem(true);
					return;
				}
				break;
			case XUserChangeEvent.SigningOut:
				if (user != null)
				{
					user.SetUserState(GameCoreRuntimeManager.UserState.SigningOut);
				}
				if (GameCoreRuntimeManager.activeUser != null && GameCoreRuntimeManager.activeUser.localId.value == userLocalId.value)
				{
					GameCoreRuntimeManager.activeUser.DeferUserSignOut();
					if (GameCoreRuntimeManager.GamerTag != null || GameCoreRuntimeManager.UserDisplayImage != null)
					{
						GameCoreRuntimeManager.GamerTag = null;
						GameCoreRuntimeManager.UserDisplayImage = null;
						GameCoreRuntimeManager.NotifyPlatformEngagedDisplayInfoChanged();
						return;
					}
				}
				break;
			case XUserChangeEvent.SignedOut:
				if (user != null)
				{
					user.SetUserState(GameCoreRuntimeManager.UserState.NotSignedIn);
				}
				GameCoreRuntimeManager.RemoveActiveUser(userLocalId);
				break;
			case XUserChangeEvent.Gamertag:
			case XUserChangeEvent.GamerPicture:
			case XUserChangeEvent.Privileges:
				break;
			default:
				return;
			}
		}

		// Token: 0x06004D29 RID: 19753 RVA: 0x0016AA64 File Offset: 0x00168C64
		public static void UpdateDeferredSignOuts()
		{
			if (GameCoreRuntimeManager.isDelayingSignOut)
			{
				float deltaTimeSeconds = GameCoreRuntimeManager.deltaTimeCalculator.GetDeltaTimeSeconds();
				for (int i = 0; i < GameCoreRuntimeManager.signOutDeferrals.Count; i++)
				{
					if (!GameCoreRuntimeManager.signOutDeferrals[i].UpdateSignOutDeferral(deltaTimeSeconds))
					{
						GameCoreRuntimeManager.signOutDeferrals.RemoveAt(i);
					}
				}
				if (GameCoreRuntimeManager.signOutDeferrals.Count == 0)
				{
					GameCoreRuntimeManager.deltaTimeCalculator.Reset();
					GameCoreRuntimeManager.isDelayingSignOut = false;
				}
			}
		}

		// Token: 0x06004D2A RID: 19754 RVA: 0x0016AAD2 File Offset: 0x00168CD2
		private static void AddDeferredSignOut(GameCoreRuntimeManager.User user)
		{
			if (user == null)
			{
				return;
			}
			if (GameCoreRuntimeManager.signOutDeferrals.Count == 0)
			{
				GameCoreRuntimeManager.deltaTimeCalculator.Reset();
				GameCoreRuntimeManager.deltaTimeCalculator.Start();
			}
			GameCoreRuntimeManager.signOutDeferrals.Add(user);
			GameCoreRuntimeManager.isDelayingSignOut = true;
		}

		// Token: 0x06004D2B RID: 19755 RVA: 0x0016AB0C File Offset: 0x00168D0C
		public static void SwitchUser()
		{
			GameCoreRuntimeManager.RemoveUser(GameCoreRuntimeManager.activeUser);
			object obj = GameCoreRuntimeManager.saveLock;
			lock (obj)
			{
				GameCoreRuntimeManager.SaveSystemInitialised = false;
			}
		}

		// Token: 0x06004D2C RID: 19756 RVA: 0x0016AB58 File Offset: 0x00168D58
		public static void RequestUserSignIn(GameCoreRuntimeManager.UserSignInEvent callback = null)
		{
			object obj = GameCoreRuntimeManager.userLock;
			lock (obj)
			{
				if (!GameCoreRuntimeManager.isAddingUser)
				{
					GameCoreRuntimeManager.userSignInCallback = (GameCoreRuntimeManager.UserSignInEvent)Delegate.Combine(GameCoreRuntimeManager.userSignInCallback, callback);
					GameCoreRuntimeManager.SignIn();
				}
				else if (callback != null)
				{
					callback(false);
				}
			}
		}

		// Token: 0x06004D2D RID: 19757 RVA: 0x0016ABC4 File Offset: 0x00168DC4
		private static void SignIn()
		{
			object obj = GameCoreRuntimeManager.userLock;
			lock (obj)
			{
				if (!GameCoreRuntimeManager.isAddingUser)
				{
					GameCoreRuntimeManager.isAddingUser = true;
					GameCoreRuntimeManager.signInRequested = false;
					if (GameCoreRuntimeManager.activeUser != null)
					{
						GameCoreRuntimeManager.UpdateEngagedUser(null);
					}
					GameCoreRuntimeManager.RegisterUserChangeEvents();
					SDK.XUserAddAsync(GameCoreRuntimeManager.hasSignedInOnce ? XUserAddOptions.AddDefaultUserSilently : XUserAddOptions.AddDefaultUserAllowingUI, new XUserAddCompleted(GameCoreRuntimeManager.AddUserComplete));
				}
			}
		}

		// Token: 0x06004D2E RID: 19758 RVA: 0x0016AC44 File Offset: 0x00168E44
		public static void SignOutUser()
		{
		}

		// Token: 0x06004D2F RID: 19759 RVA: 0x0016AC48 File Offset: 0x00168E48
		private static void RemoveActiveUser(XUserLocalId userLocalId)
		{
			if (GameCoreRuntimeManager.activeUser != null && GameCoreRuntimeManager.activeUser.localId.value == userLocalId.value)
			{
				GameCoreRuntimeManager.UpdateEngagedUser(null);
				GameCoreRuntimeManager.RemoveUser(GameCoreRuntimeManager.activeUser);
				return;
			}
			GameCoreRuntimeManager.User user;
			if (GameCoreRuntimeManager.connectedUsers.TryGetValue(userLocalId.value, out user))
			{
				GameCoreRuntimeManager.RemoveUser(user);
			}
		}

		// Token: 0x06004D30 RID: 19760 RVA: 0x0016ACA0 File Offset: 0x00168EA0
		private static void RemoveUser(GameCoreRuntimeManager.User user)
		{
			if (user == null)
			{
				return;
			}
			user.SetUserState(GameCoreRuntimeManager.UserState.NotSignedIn);
			user.FinishUserSignOut();
			object obj = GameCoreRuntimeManager.userLock;
			lock (obj)
			{
				if (GameCoreRuntimeManager.activeUser == user)
				{
					GameCoreRuntimeManager.activeUser.SetUserActive(false);
					GameCoreRuntimeManager.UpdateEngagedUser(null);
					GameCoreRuntimeManager.activeUser = null;
					object obj2 = GameCoreRuntimeManager.saveLock;
					lock (obj2)
					{
						GameCoreRuntimeManager.SaveSystemInitialised = false;
					}
				}
			}
			GameCoreRuntimeManager.User user2;
			if (GameCoreRuntimeManager.connectedUsers.TryGetValue(user.localId.value, out user2))
			{
				if (user2 != user)
				{
					GameCoreRuntimeManager.connectedUsers.Remove(user.localId.value);
					if (user2 != null)
					{
						user2.Dispose();
					}
					user.Dispose();
					return;
				}
			}
			else
			{
				user.Dispose();
			}
		}

		// Token: 0x06004D31 RID: 19761 RVA: 0x0016AD84 File Offset: 0x00168F84
		private static void AddUserComplete(int hresult, XGamingRuntime.XUserHandle userHandle)
		{
			GameCoreRuntimeManager.UserSignInEvent userSignInEvent = null;
			bool flag = GameCoreRuntimeManager.Succeeded(hresult, "Sign in.");
			GameCoreRuntimeManager.User user = null;
			if (flag)
			{
				XUserLocalId xuserLocalId;
				if (GameCoreRuntimeManager.Succeeded(SDK.XUserGetLocalId(userHandle, out xuserLocalId), "Get Local User ID"))
				{
					if (!GameCoreRuntimeManager.connectedUsers.TryGetValue(xuserLocalId.value, out user) || user.IsDisposed)
					{
						user = new GameCoreRuntimeManager.User(userHandle, xuserLocalId);
						user.Init();
						GameCoreRuntimeManager.connectedUsers[xuserLocalId.value] = user;
					}
				}
				else
				{
					flag = false;
				}
			}
			if (user == null)
			{
				if (userHandle != null)
				{
					SDK.XUserCloseHandle(userHandle);
				}
				flag = false;
			}
			else
			{
				if (userHandle != user.userHandle)
				{
					int num;
					SDK.XUserCompare(userHandle, user.userHandle, out num);
					if (num == 0)
					{
						SDK.XUserCloseHandle(userHandle);
						userHandle = user.userHandle;
					}
					else
					{
						flag = false;
					}
				}
				user.SetUserState(GameCoreRuntimeManager.UserState.SignedIn);
			}
			if (flag)
			{
				if (!GameCoreRuntimeManager.hasSignedInOnce)
				{
					GameCoreRuntimeManager.hasSignedInOnce = true;
				}
				GameCoreRuntimeManager.SetActiveUser(user);
			}
			else if (GameCoreRuntimeManager.hasSignedInOnce && GameCoreRuntimeManager.activeUser != null)
			{
				GameCoreRuntimeManager.SetActiveUser(GameCoreRuntimeManager.activeUser);
			}
			object obj = GameCoreRuntimeManager.userLock;
			lock (obj)
			{
				userSignInEvent = GameCoreRuntimeManager.userSignInCallback;
				GameCoreRuntimeManager.isAddingUser = false;
				GameCoreRuntimeManager.userSignInCallback = null;
			}
			if (userSignInEvent != null)
			{
				userSignInEvent(flag);
			}
		}

		// Token: 0x06004D32 RID: 19762 RVA: 0x0016AEC8 File Offset: 0x001690C8
		private static void SetActiveUser(GameCoreRuntimeManager.User user)
		{
			if (user == null)
			{
				return;
			}
			object obj = GameCoreRuntimeManager.userLock;
			lock (obj)
			{
				bool flag2 = false;
				if (GameCoreRuntimeManager.activeUser != null && GameCoreRuntimeManager.activeUser != user)
				{
					flag2 = true;
					GameCoreRuntimeManager.RemoveUser(GameCoreRuntimeManager.activeUser);
				}
				GameCoreRuntimeManager.activeUser = user;
				user.SetUserActive(true);
				object obj2;
				if (user.SaveInitialised)
				{
					obj2 = GameCoreRuntimeManager.saveLock;
					lock (obj2)
					{
						if (flag2)
						{
							GameCoreRuntimeManager.SaveSystemInitialised = false;
						}
						GameCoreRuntimeManager.SaveSystemInitialised = true;
						goto IL_AA;
					}
				}
				obj2 = GameCoreRuntimeManager.saveLock;
				lock (obj2)
				{
					GameCoreRuntimeManager.SaveSystemInitialised = false;
				}
				user.InitialiseSaveSystem(false);
			}
			IL_AA:
			GameCoreRuntimeManager.FetchAchievements();
			GameCoreRuntimeManager.UpdateEngagedUser(user);
		}

		// Token: 0x06004D33 RID: 19763 RVA: 0x0016AFB4 File Offset: 0x001691B4
		private static void UpdateEngagedUser(GameCoreRuntimeManager.User user)
		{
			if (user != null)
			{
				GameCoreRuntimeManager.GamerTag = user.GamerTag;
				GameCoreRuntimeManager.UserDisplayImage = user.UserDisplayImage;
			}
			else
			{
				GameCoreRuntimeManager.GamerTag = null;
				GameCoreRuntimeManager.UserDisplayImage = null;
			}
			GameCoreRuntimeManager.NotifyPlatformEngagedDisplayInfoChanged();
		}

		// Token: 0x06004D34 RID: 19764 RVA: 0x0016AFE2 File Offset: 0x001691E2
		private static void NotifyPlatformEngagedDisplayInfoChanged()
		{
			CoreLoop.InvokeSafe(delegate
			{
				if (Platform.Current)
				{
					Platform.Current.NotifyEngagedDisplayInfoChanged();
				}
			});
		}

		// Token: 0x06004D35 RID: 19765 RVA: 0x0016B008 File Offset: 0x00169208
		public static void FetchAchievements()
		{
			GameCoreRuntimeManager.hasFetchedSinceUnlock = true;
			if (GameCoreRuntimeManager.activeUser == null)
			{
				GameCoreRuntimeManager.achievementUpdateQueued = false;
				return;
			}
			if (GameCoreRuntimeManager.achivementRateLimiter.CanMakeCall() && GameCoreRuntimeManager.activeUser.FetchRateLimiter.CanMakeCall())
			{
				GameCoreRuntimeManager.activeUser.FetchAchievements();
				GameCoreRuntimeManager.achievementUpdateQueued = false;
				return;
			}
			GameCoreRuntimeManager.achievementUpdateQueued = true;
		}

		// Token: 0x06004D36 RID: 19766 RVA: 0x0016B065 File Offset: 0x00169265
		public static void UpdateAchievementState(string achievementId)
		{
			if (GameCoreRuntimeManager.activeUser == null)
			{
				GameCoreRuntimeManager.achievementUpdateQueued = false;
				return;
			}
			if (GameCoreRuntimeManager.achievementUpdateQueued || GameCoreRuntimeManager.activeUser.IsFetchingAchievementData)
			{
				return;
			}
			GameCoreRuntimeManager.activeUser.UpdateAchievementState(achievementId);
		}

		// Token: 0x06004D37 RID: 19767 RVA: 0x0016B098 File Offset: 0x00169298
		public static bool TryGetAchievement(int achievementId, out XblAchievement xblAchievement)
		{
			if (GameCoreRuntimeManager.activeUser == null)
			{
				xblAchievement = null;
				return false;
			}
			return GameCoreRuntimeManager.activeUser.xblAchievements.TryGetValue(achievementId, out xblAchievement);
		}

		// Token: 0x06004D38 RID: 19768 RVA: 0x0016B0B7 File Offset: 0x001692B7
		public static void UnlockAchievement(int achievementId)
		{
			GameCoreRuntimeManager.UnlockAchievement(achievementId.ToString());
		}

		// Token: 0x06004D39 RID: 19769 RVA: 0x0016B0C5 File Offset: 0x001692C5
		public static void UnlockAchievement(string achievementId)
		{
			GameCoreRuntimeManager.UnlockAchievement(achievementId, 100U);
		}

		// Token: 0x06004D3A RID: 19770 RVA: 0x0016B0CF File Offset: 0x001692CF
		public static void UnlockAchievement(int achievementId, uint progress)
		{
			GameCoreRuntimeManager.UnlockAchievement(achievementId.ToString(), progress);
		}

		// Token: 0x06004D3B RID: 19771 RVA: 0x0016B0E0 File Offset: 0x001692E0
		public static void UnlockAchievement(string achievementId, uint progress)
		{
			if (string.IsNullOrEmpty(achievementId))
			{
				return;
			}
			GameCoreRuntimeManager.User user = GameCoreRuntimeManager.activeUser;
			if (user == null)
			{
				return;
			}
			user.UnlockAchievement(achievementId, progress);
		}

		// Token: 0x06004D3C RID: 19772 RVA: 0x0016B108 File Offset: 0x00169308
		private static void HandleQueryForUpdatesComplete(int hresult, XStorePackageUpdate[] packageUpdates)
		{
			if (GameCoreRuntimeManager.activeUser == null)
			{
				return;
			}
			XStoreContext xblStoreContext = GameCoreRuntimeManager.activeUser.xblStoreContext;
			List<string> list = new List<string>();
			if (hresult >= 0 && packageUpdates != null && packageUpdates.Length != 0)
			{
				foreach (XStorePackageUpdate xstorePackageUpdate in packageUpdates)
				{
					list.Add(xstorePackageUpdate.PackageIdentifier);
				}
				SDK.XStoreDownloadAndInstallPackageUpdatesAsync(xblStoreContext, list.ToArray(), new SDK.XStoreDownloadAndInstallPackageUpdatesCompleted(GameCoreRuntimeManager.DownloadFinishedCallback));
			}
		}

		// Token: 0x06004D3D RID: 19773 RVA: 0x0016B173 File Offset: 0x00169373
		private static void DownloadFinishedCallback(int hresult)
		{
			GameCoreRuntimeManager.Succeeded(hresult, "DownloadAndInstallPackageUpdates callback");
		}

		// Token: 0x06004D3E RID: 19774 RVA: 0x0016B184 File Offset: 0x00169384
		public static bool Succeeded(int hresult, string operationFriendlyName)
		{
			bool result = false;
			if (XGamingRuntime.Interop.HR.SUCCEEDED(hresult))
			{
				result = true;
			}
			return result;
		}

		// Token: 0x04004E00 RID: 19968
		private const int _100PercentAchievementProgress = 100;

		// Token: 0x04004E01 RID: 19969
		private const string DEFAULT_SCID = "00000000-0000-0000-0000-0000636f5860";

		// Token: 0x04004E02 RID: 19970
		private const float SIGN_OUT_DELAY = 2f;

		// Token: 0x04004E03 RID: 19971
		private static uint s_TitleId;

		// Token: 0x04004E04 RID: 19972
		private static volatile bool threadEnding;

		// Token: 0x04004E05 RID: 19973
		private static volatile bool threadActive;

		// Token: 0x04004E06 RID: 19974
		private static Thread dispatchThread;

		// Token: 0x04004E07 RID: 19975
		private static Dictionary<int, string> _hresultToFriendlyErrorLookup;

		// Token: 0x04004E08 RID: 19976
		private static bool saveSystemInitialised;

		// Token: 0x04004E0A RID: 19978
		private static object saveLock = new object();

		// Token: 0x04004E0D RID: 19981
		public static GameCoreRuntimeManager.UserSignInEvent userSignInCallback;

		// Token: 0x04004E0E RID: 19982
		private static readonly List<GameCoreRuntimeManager.User> signOutDeferrals = new List<GameCoreRuntimeManager.User>();

		// Token: 0x04004E0F RID: 19983
		private static readonly Dictionary<ulong, GameCoreRuntimeManager.User> connectedUsers = new Dictionary<ulong, GameCoreRuntimeManager.User>();

		// Token: 0x04004E10 RID: 19984
		private static GameCoreRuntimeManager.User activeUser;

		// Token: 0x04004E11 RID: 19985
		private static readonly object userLock = new object();

		// Token: 0x04004E12 RID: 19986
		private static float timeToSignOut;

		// Token: 0x04004E13 RID: 19987
		private static volatile bool isAddingUser;

		// Token: 0x04004E14 RID: 19988
		private static bool hasSignedInOnce;

		// Token: 0x04004E15 RID: 19989
		private static bool isDelayingSignOut;

		// Token: 0x04004E16 RID: 19990
		private static bool signInRequested;

		// Token: 0x04004E17 RID: 19991
		private static bool registeredUserEvents;

		// Token: 0x04004E18 RID: 19992
		private static volatile bool hasFetchedSinceUnlock;

		// Token: 0x04004E19 RID: 19993
		private static XRegistrationToken userEventToken;

		// Token: 0x04004E1A RID: 19994
		private static DeltaTimeCalculator deltaTimeCalculator = new DeltaTimeCalculator();

		// Token: 0x04004E1B RID: 19995
		private static GameCoreRuntimeManager.RateLimiter achivementRateLimiter = new GameCoreRuntimeManager.RateLimiter(1, TimeSpan.FromSeconds(5.0));

		// Token: 0x04004E1C RID: 19996
		private static volatile bool achievementUpdateQueued;

		// Token: 0x04004E1D RID: 19997
		private static bool registerSuspendEvent;

		// Token: 0x04004E1E RID: 19998
		private static volatile bool reconfirmUser;

		// Token: 0x02001B20 RID: 6944
		public enum UserState
		{
			// Token: 0x04009BB3 RID: 39859
			NotSignedIn,
			// Token: 0x04009BB4 RID: 39860
			SignedIn,
			// Token: 0x04009BB5 RID: 39861
			SigningOut,
			// Token: 0x04009BB6 RID: 39862
			SignedOut
		}

		// Token: 0x02001B21 RID: 6945
		public sealed class User : IDisposable
		{
			// Token: 0x17001190 RID: 4496
			// (get) Token: 0x0600990A RID: 39178 RVA: 0x002B0BB9 File Offset: 0x002AEDB9
			// (set) Token: 0x0600990B RID: 39179 RVA: 0x002B0BC1 File Offset: 0x002AEDC1
			public GameCoreRuntimeManager.UserState UserState { get; private set; }

			// Token: 0x17001191 RID: 4497
			// (get) Token: 0x0600990C RID: 39180 RVA: 0x002B0BCA File Offset: 0x002AEDCA
			// (set) Token: 0x0600990D RID: 39181 RVA: 0x002B0BD2 File Offset: 0x002AEDD2
			public string GamerTag { get; private set; }

			// Token: 0x17001192 RID: 4498
			// (get) Token: 0x0600990E RID: 39182 RVA: 0x002B0BDB File Offset: 0x002AEDDB
			// (set) Token: 0x0600990F RID: 39183 RVA: 0x002B0BE3 File Offset: 0x002AEDE3
			public Texture2D UserDisplayImage { get; private set; }

			// Token: 0x17001193 RID: 4499
			// (get) Token: 0x06009910 RID: 39184 RVA: 0x002B0BEC File Offset: 0x002AEDEC
			// (set) Token: 0x06009911 RID: 39185 RVA: 0x002B0C29 File Offset: 0x002AEE29
			public XGamingRuntime.XblContextHandle XblContextHandle
			{
				get
				{
					if (this.xblContextHandle == null)
					{
						GameCoreRuntimeManager.Succeeded(SDK.XBL.XblContextCreateHandle(this.userHandle, out this.xblContextHandle), "Create Xbox Live context : " + this.GetGamerID());
					}
					return this.xblContextHandle;
				}
				set
				{
					this.xblContextHandle = value;
				}
			}

			// Token: 0x17001194 RID: 4500
			// (get) Token: 0x06009912 RID: 39186 RVA: 0x002B0C32 File Offset: 0x002AEE32
			public GameCoreRuntimeManager.RateLimiter FetchRateLimiter
			{
				get
				{
					return this.fetchRateLimiter;
				}
			}

			// Token: 0x17001195 RID: 4501
			// (get) Token: 0x06009913 RID: 39187 RVA: 0x002B0C3A File Offset: 0x002AEE3A
			// (set) Token: 0x06009914 RID: 39188 RVA: 0x002B0C44 File Offset: 0x002AEE44
			public bool SaveInitialised
			{
				get
				{
					return this.saveInitialised;
				}
				private set
				{
					this.saveInitialised = value;
				}
			}

			// Token: 0x17001196 RID: 4502
			// (get) Token: 0x06009915 RID: 39189 RVA: 0x002B0C4F File Offset: 0x002AEE4F
			public bool IsFetchingAchievementData
			{
				get
				{
					return this.isFetchingAchievementData;
				}
			}

			// Token: 0x17001197 RID: 4503
			// (get) Token: 0x06009916 RID: 39190 RVA: 0x002B0C57 File Offset: 0x002AEE57
			public bool IsDisposed
			{
				get
				{
					return this.isDisposed;
				}
			}

			// Token: 0x06009917 RID: 39191 RVA: 0x002B0C60 File Offset: 0x002AEE60
			public User(XGamingRuntime.XUserHandle userHandle, XUserLocalId localId)
			{
				this.userHandle = userHandle;
				this.localId = localId;
			}

			// Token: 0x06009918 RID: 39192 RVA: 0x002B0CD4 File Offset: 0x002AEED4
			~User()
			{
				this.ReleaseUnmanagedResources();
			}

			// Token: 0x06009919 RID: 39193 RVA: 0x002B0D00 File Offset: 0x002AEF00
			private void ReleaseUnmanagedResources()
			{
				if (this.isDisposed)
				{
					return;
				}
				this.UserState = GameCoreRuntimeManager.UserState.NotSignedIn;
				this.isDisposed = true;
				if (this.userHandle != null)
				{
					SDK.XUserCloseHandle(this.userHandle);
					this.userHandle = null;
				}
				if (this.xblContextHandle != null)
				{
					SDK.XBL.XblContextCloseHandle(this.xblContextHandle);
					this.xblContextHandle = null;
				}
				if (this.xblStoreContext != null)
				{
					SDK.XStoreCloseContextHandle(this.xblStoreContext);
					this.xblStoreContext = null;
				}
				if (this.UserDisplayImage != null)
				{
					Texture2D image = this.UserDisplayImage;
					CoreLoop.InvokeSafe(delegate
					{
						Object.Destroy(image);
					});
					this.UserDisplayImage = null;
				}
				this.FinishUserSignOut();
				GameCoreSaveHandler gameCoreSaveHandler = this.saveHandler;
				if (gameCoreSaveHandler != null)
				{
					gameCoreSaveHandler.Dispose();
				}
				this.saveHandler = null;
			}

			// Token: 0x0600991A RID: 39194 RVA: 0x002B0DD8 File Offset: 0x002AEFD8
			public void Dispose()
			{
				this.ReleaseUnmanagedResources();
				GC.SuppressFinalize(this);
			}

			// Token: 0x0600991B RID: 39195 RVA: 0x002B0DE8 File Offset: 0x002AEFE8
			public void Init()
			{
				if (this.isDisposed)
				{
					return;
				}
				string gamerTag;
				this.hasGamerTag = GameCoreRuntimeManager.Succeeded(SDK.XUserGetGamertag(this.userHandle, XUserGamertagComponent.UniqueModern, out gamerTag), "Get gamertag : " + this.GetGamerID());
				if (this.hasGamerTag)
				{
					this.GamerTag = gamerTag;
					if (this.isActiveUser)
					{
						GameCoreRuntimeManager.UpdateEngagedUser(this);
					}
				}
				SDK.XUserGetGamerPictureAsync(this.userHandle, XUserGamerPictureSize.Small, delegate(int hresult, byte[] buffer)
				{
					if (!GameCoreRuntimeManager.Succeeded(hresult, "Get user display pic."))
					{
						return;
					}
					CoreLoop.InvokeSafe(delegate
					{
						if (this.UserDisplayImage != null)
						{
							Object.Destroy(this.UserDisplayImage);
						}
						this.UserDisplayImage = new Texture2D(64, 64, TextureFormat.BGRA32, false, false);
						this.UserDisplayImage.LoadImage(buffer);
						if (this.isActiveUser)
						{
							GameCoreRuntimeManager.UpdateEngagedUser(this);
						}
					});
				});
				this.InitialiseSaveSystem(false);
				if (this.xblContextHandle == null)
				{
					GameCoreRuntimeManager.Succeeded(SDK.XBL.XblContextCreateHandle(this.userHandle, out this.xblContextHandle), "Create Xbox Live context : " + this.GetGamerID());
				}
				if (this.xblStoreContext == null)
				{
					GameCoreRuntimeManager.Succeeded(SDK.XStoreCreateContext(this.userHandle, out this.xblStoreContext), "Create Xbox store context : " + this.GetGamerID());
				}
			}

			// Token: 0x0600991C RID: 39196 RVA: 0x002B0ECD File Offset: 0x002AF0CD
			public string GetGamerID()
			{
				return string.Format("\"{0}\" - {1}", this.GamerTag, this.localId.value);
			}

			// Token: 0x0600991D RID: 39197 RVA: 0x002B0EEF File Offset: 0x002AF0EF
			public void SetUserState(GameCoreRuntimeManager.UserState userState)
			{
				if (this.isDisposed)
				{
					return;
				}
				this.UserState = userState;
			}

			// Token: 0x0600991E RID: 39198 RVA: 0x002B0F01 File Offset: 0x002AF101
			public void SetUserActive(bool isActive)
			{
				this.isActiveUser = isActive;
			}

			// Token: 0x0600991F RID: 39199 RVA: 0x002B0F0C File Offset: 0x002AF10C
			public void InitialiseSaveSystem(bool reinitialise)
			{
				if (this.isDisposed)
				{
					return;
				}
				object obj = this.saveLock;
				lock (obj)
				{
					if (reinitialise && this.saveHandler != null)
					{
						this.saveHandler.Dispose();
						this.saveHandler = null;
					}
					if (this.saveHandler == null)
					{
						this.saveHandler = new GameCoreSaveHandler();
						this.SaveInitialised = false;
						this.saveHandler.InitializeAsync(this.userHandle, GameCoreRuntimeManager.SCID, delegate(int hresult)
						{
							object obj2 = this.saveLock;
							lock (obj2)
							{
								if (GameCoreRuntimeManager.Succeeded(hresult, "Initialise game save provider : " + this.GetGamerID()))
								{
									this.SaveInitialised = true;
								}
								else
								{
									this.saveHandler = null;
								}
							}
							GameCoreRuntimeManager.NotifyUserSaveInitialised(this, hresult);
						});
					}
				}
			}

			// Token: 0x06009920 RID: 39200 RVA: 0x002B0FA8 File Offset: 0x002AF1A8
			public void DeferUserSignOut()
			{
				if (this.signOutDeferralHandle == null && GameCoreRuntimeManager.Succeeded(SDK.XUserGetSignOutDeferral(out this.signOutDeferralHandle), "Delay user " + this.GetGamerID() + " sign out"))
				{
					this.signOutTimer = 2f;
					GameCoreRuntimeManager.AddDeferredSignOut(this);
				}
			}

			// Token: 0x06009921 RID: 39201 RVA: 0x002B0FFB File Offset: 0x002AF1FB
			public bool UpdateSignOutDeferral(float deltaTime)
			{
				this.signOutTimer -= deltaTime;
				if (this.signOutTimer > 0f)
				{
					return true;
				}
				this.FinishUserSignOut();
				return false;
			}

			// Token: 0x06009922 RID: 39202 RVA: 0x002B1021 File Offset: 0x002AF221
			public void FinishUserSignOut()
			{
				if (this.signOutDeferralHandle != null)
				{
					SDK.XUserCloseSignOutDeferralHandle(this.signOutDeferralHandle);
					this.signOutDeferralHandle = null;
					this.UserState = GameCoreRuntimeManager.UserState.SignedOut;
					this.DisposeSave();
				}
			}

			// Token: 0x06009923 RID: 39203 RVA: 0x002B1054 File Offset: 0x002AF254
			public void DisposeSave()
			{
				object userLock = this.saveLock;
				lock (userLock)
				{
					bool flag2 = this.SaveInitialised;
					if (this.SaveInitialised || this.saveHandler != null)
					{
						this.SaveInitialised = false;
						GameCoreSaveHandler gameCoreSaveHandler = this.saveHandler;
						if (gameCoreSaveHandler != null)
						{
							gameCoreSaveHandler.Dispose();
						}
						flag2 = true;
					}
					this.saveHandler = null;
					if (!flag2)
					{
						return;
					}
				}
				userLock = GameCoreRuntimeManager.userLock;
				lock (userLock)
				{
					if (GameCoreRuntimeManager.activeUser == this)
					{
						object obj = GameCoreRuntimeManager.saveLock;
						lock (obj)
						{
							GameCoreRuntimeManager.SaveSystemInitialised = false;
						}
					}
				}
			}

			// Token: 0x06009924 RID: 39204 RVA: 0x002B112C File Offset: 0x002AF32C
			public void FetchAchievements()
			{
				if (this.isDisposed)
				{
					return;
				}
				if (this.isFetchingAchievementData)
				{
					return;
				}
				if (this.XblContextHandle == null)
				{
					return;
				}
				this.isFetchingAchievementData = true;
				ulong xboxUserId;
				if (!GameCoreRuntimeManager.Succeeded(SDK.XUserGetId(this.userHandle, out xboxUserId), "Get Xbox user ID"))
				{
					this.isFetchingAchievementData = false;
					return;
				}
				SDK.XBL.XblAchievementsGetAchievementsForTitleIdAsync(this.XblContextHandle, xboxUserId, GameCoreRuntimeManager.TitleID, XblAchievementType.All, false, XblAchievementOrderBy.DefaultOrder, 0U, 100U, delegate(int hresult, XblAchievementsResultHandle result)
				{
					if (!GameCoreRuntimeManager.Succeeded(hresult, "Fetching Achievements for " + this.GetGamerID() + "."))
					{
						this.isFetchingAchievementData = false;
						SDK.XBL.XblAchievementsResultCloseHandle(result);
						return;
					}
					XblAchievement[] array;
					SDK.XBL.XblAchievementsResultGetAchievements(result, out array);
					foreach (XblAchievement xblAchievement in array)
					{
						int key;
						if (int.TryParse(xblAchievement.Id, out key))
						{
							this.xblAchievements[key] = xblAchievement;
						}
					}
					SDK.XBL.XblAchievementsResultCloseHandle(result);
				});
			}

			// Token: 0x06009925 RID: 39205 RVA: 0x002B11A8 File Offset: 0x002AF3A8
			public void UpdateAchievementState(string achievementID)
			{
				if (this.isDisposed)
				{
					return;
				}
				if (this.isFetchingAchievementData)
				{
					return;
				}
				if (this.XblContextHandle == null)
				{
					return;
				}
				ulong xboxUserId;
				if (!GameCoreRuntimeManager.Succeeded(SDK.XUserGetId(this.userHandle, out xboxUserId), "Get Xbox user ID"))
				{
					this.isFetchingAchievementData = false;
					return;
				}
				SDK.XBL.XblAchievementsGetAchievementAsync(this.XblContextHandle, xboxUserId, GameCoreRuntimeManager.SCID, achievementID, delegate(int hresult, XblAchievementsResultHandle result)
				{
					if (!GameCoreRuntimeManager.Succeeded(hresult, string.Concat(new string[]
					{
						"Updating Achievement '",
						achievementID,
						"' for ",
						this.GetGamerID(),
						"."
					})))
					{
						SDK.XBL.XblAchievementsResultCloseHandle(result);
						return;
					}
					XblAchievement[] array;
					SDK.XBL.XblAchievementsResultGetAchievements(result, out array);
					if (array != null)
					{
						foreach (XblAchievement xblAchievement in array)
						{
							int key;
							if (int.TryParse(xblAchievement.Id, out key))
							{
								this.xblAchievements[key] = xblAchievement;
							}
						}
					}
					SDK.XBL.XblAchievementsResultCloseHandle(result);
				});
			}

			// Token: 0x06009926 RID: 39206 RVA: 0x002B1230 File Offset: 0x002AF430
			public void UnlockAchievement(string achievementId, uint progress)
			{
				ulong xboxUserId;
				if (!GameCoreRuntimeManager.Succeeded(SDK.XUserGetId(this.userHandle, out xboxUserId), "Get Xbox user ID"))
				{
					return;
				}
				object obj = this.unlockLock;
				lock (obj)
				{
					uint num;
					if (this.activeUnlocks.TryGetValue(achievementId, out num) && num >= progress)
					{
						return;
					}
					this.activeUnlocks[achievementId] = progress;
				}
				GameCoreRuntimeManager.hasFetchedSinceUnlock = false;
				SDK.XBL.XblAchievementsUpdateAchievementAsync(this.XblContextHandle, xboxUserId, achievementId, progress, delegate(int hresult)
				{
					object obj2 = this.unlockLock;
					lock (obj2)
					{
						uint num2;
						if (this.activeUnlocks.TryGetValue(achievementId, out num2) && num2 <= progress)
						{
							this.activeUnlocks.Remove(achievementId);
						}
					}
					if (GameCoreRuntimeManager.Succeeded(hresult, "Unlock achievement " + achievementId) && progress >= 100U && !GameCoreRuntimeManager.hasFetchedSinceUnlock)
					{
						GameCoreRuntimeManager.UpdateAchievementState(achievementId);
					}
				});
			}

			// Token: 0x04009BB7 RID: 39863
			public XGamingRuntime.XUserHandle userHandle;

			// Token: 0x04009BB8 RID: 39864
			public XUserLocalId localId;

			// Token: 0x04009BBC RID: 39868
			private XGamingRuntime.XblContextHandle xblContextHandle;

			// Token: 0x04009BBD RID: 39869
			public XStoreContext xblStoreContext;

			// Token: 0x04009BBE RID: 39870
			public GameCoreSaveHandler saveHandler;

			// Token: 0x04009BBF RID: 39871
			public object saveLock = new object();

			// Token: 0x04009BC0 RID: 39872
			public ConcurrentQueue<Action> saveQueue = new ConcurrentQueue<Action>();

			// Token: 0x04009BC1 RID: 39873
			private GameCoreRuntimeManager.RateLimiter fetchRateLimiter = new GameCoreRuntimeManager.RateLimiter(1, TimeSpan.FromSeconds(30.0));

			// Token: 0x04009BC2 RID: 39874
			private volatile bool saveInitialised;

			// Token: 0x04009BC3 RID: 39875
			public bool HasFetchedAchievement;

			// Token: 0x04009BC4 RID: 39876
			private bool isFetchingAchievementData;

			// Token: 0x04009BC5 RID: 39877
			public Dictionary<int, XblAchievement> xblAchievements = new Dictionary<int, XblAchievement>();

			// Token: 0x04009BC6 RID: 39878
			private bool isActiveUser;

			// Token: 0x04009BC7 RID: 39879
			private bool hasGamerTag;

			// Token: 0x04009BC8 RID: 39880
			private XUserSignOutDeferralHandle signOutDeferralHandle;

			// Token: 0x04009BC9 RID: 39881
			private float signOutTimer;

			// Token: 0x04009BCA RID: 39882
			private bool isDisposed;

			// Token: 0x04009BCB RID: 39883
			private object unlockLock = new object();

			// Token: 0x04009BCC RID: 39884
			private Dictionary<string, uint> activeUnlocks = new Dictionary<string, uint>();
		}

		// Token: 0x02001B22 RID: 6946
		public sealed class RateLimiter
		{
			// Token: 0x0600992A RID: 39210 RVA: 0x002B1428 File Offset: 0x002AF628
			public RateLimiter(int maxCalls, TimeSpan perTimeSpan)
			{
				if (maxCalls <= 0)
				{
					Debug.LogError("Max calls must be at least 1");
					maxCalls = 1;
				}
				this.callInterval = TimeSpan.FromTicks(perTimeSpan.Ticks / (long)maxCalls);
				this.burstLimit = this.callInterval * (double)(maxCalls - 1);
			}

			// Token: 0x0600992B RID: 39211 RVA: 0x002B1484 File Offset: 0x002AF684
			public bool CanMakeCall()
			{
				DateTime utcNow = DateTime.UtcNow;
				if (utcNow + this.burstLimit < this.nextAllowedTime)
				{
					return false;
				}
				this.nextAllowedTime = ((utcNow > this.nextAllowedTime) ? (utcNow + this.callInterval) : (this.nextAllowedTime + this.callInterval));
				return true;
			}

			// Token: 0x04009BCD RID: 39885
			private DateTime nextAllowedTime = DateTime.MinValue;

			// Token: 0x04009BCE RID: 39886
			private readonly TimeSpan callInterval;

			// Token: 0x04009BCF RID: 39887
			private readonly TimeSpan burstLimit;
		}

		// Token: 0x02001B23 RID: 6947
		// (Invoke) Token: 0x0600992D RID: 39213
		public delegate void UserSignInEvent(bool success);
	}
}
