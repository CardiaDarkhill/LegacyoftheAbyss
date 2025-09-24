using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XGamingRuntime;
using XGamingRuntime.Interop;

namespace TeamCherry.GameCore
{
	// Token: 0x020008B7 RID: 2231
	public sealed class GameCoreSaveHandler : IDisposable
	{
		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x06004D46 RID: 19782 RVA: 0x0016B2AF File Offset: 0x001694AF
		// (set) Token: 0x06004D47 RID: 19783 RVA: 0x0016B2B7 File Offset: 0x001694B7
		public bool IsInitialised { get; private set; }

		// Token: 0x1700091A RID: 2330
		// (get) Token: 0x06004D48 RID: 19784 RVA: 0x0016B2C0 File Offset: 0x001694C0
		public int ActiveOperations
		{
			get
			{
				return this.activeOperations;
			}
		}

		// Token: 0x06004D49 RID: 19785 RVA: 0x0016B2C8 File Offset: 0x001694C8
		~GameCoreSaveHandler()
		{
			this.ReleaseUnmanagedResources();
		}

		// Token: 0x06004D4A RID: 19786 RVA: 0x0016B2F4 File Offset: 0x001694F4
		private void ReleaseUnmanagedResources()
		{
			if (this.isDisposed)
			{
				return;
			}
			this.isDisposed = true;
			if (this.m_gameSaveProviderHandle != null)
			{
				SDK.XGameSaveCloseProvider(this.m_gameSaveProviderHandle);
				this.m_gameSaveProviderHandle = null;
			}
			if (this.m_userHandle != null)
			{
				SDK.XUserCloseHandle(this.m_userHandle);
				this.m_userHandle = null;
			}
		}

		// Token: 0x06004D4B RID: 19787 RVA: 0x0016B351 File Offset: 0x00169551
		public void Dispose()
		{
			this.ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		// Token: 0x06004D4C RID: 19788 RVA: 0x0016B360 File Offset: 0x00169560
		public void InitializeAsync(XGamingRuntime.XUserHandle userHandle, string scid, GameCoreSaveHandler.InitializeCallback callback)
		{
			if (this.isInitialising || this.IsInitialised || this.isDisposed)
			{
				return;
			}
			this.isInitialising = true;
			if (this.m_userHandle != null)
			{
				SDK.XUserCloseHandle(this.m_userHandle);
				this.m_userHandle = null;
			}
			if (this.m_gameSaveProviderHandle != null)
			{
				SDK.XGameSaveCloseProvider(this.m_gameSaveProviderHandle);
				this.m_gameSaveProviderHandle = null;
			}
			int num = SDK.XUserDuplicateHandle(userHandle, out this.m_userHandle);
			if (XGamingRuntime.Interop.HR.FAILED(num))
			{
				callback(num);
				return;
			}
			SDK.XGameSaveInitializeProviderAsync(this.m_userHandle, scid, false, delegate(int hresult, XGameSaveProviderHandle gameSaveProviderHandle)
			{
				this.m_gameSaveProviderHandle = gameSaveProviderHandle;
				this.isInitialising = false;
				this.IsInitialised = true;
				callback(hresult);
			});
		}

		// Token: 0x06004D4D RID: 19789 RVA: 0x0016B41C File Offset: 0x0016961C
		private void StartOperation()
		{
			Interlocked.Increment(ref this.activeOperations);
		}

		// Token: 0x06004D4E RID: 19790 RVA: 0x0016B42A File Offset: 0x0016962A
		private void FinishOperation()
		{
			Interlocked.Decrement(ref this.activeOperations);
		}

		// Token: 0x06004D4F RID: 19791 RVA: 0x0016B438 File Offset: 0x00169638
		public void GetQuotaAsync(GameCoreSaveHandler.GetQuotaCallback callback)
		{
			this.StartOperation();
			callback = (GameCoreSaveHandler.GetQuotaCallback)Delegate.Combine(callback, new GameCoreSaveHandler.GetQuotaCallback(delegate(int hresult, long quota)
			{
				this.FinishOperation();
			}));
			SDK.XGameSaveGetRemainingQuotaAsync(this.m_gameSaveProviderHandle, new XGameSaveGetRemainingQuotaCompleted(callback.Invoke));
		}

		// Token: 0x06004D50 RID: 19792 RVA: 0x0016B470 File Offset: 0x00169670
		public void QueryContainers(string containerNamePrefix, GameCoreSaveHandler.QueryContainersCallback callback)
		{
			GameCoreSaveHandler.QueryContainersCallback <>9__1;
			Task.Run(delegate()
			{
				this.StartOperation();
				Delegate callback2 = callback;
				GameCoreSaveHandler.QueryContainersCallback b;
				if ((b = <>9__1) == null)
				{
					b = (<>9__1 = delegate(int hresult, string[] quota)
					{
						this.FinishOperation();
					});
				}
				callback = (GameCoreSaveHandler.QueryContainersCallback)Delegate.Combine(callback2, b);
				XGameSaveContainerInfo[] array;
				int num = SDK.XGameSaveEnumerateContainerInfoByName(this.m_gameSaveProviderHandle, containerNamePrefix, out array);
				string[] array2 = new string[0];
				if (XGamingRuntime.Interop.HR.SUCCEEDED(num))
				{
					array2 = new string[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						array2[i] = array[i].Name;
					}
				}
				callback(num, array2);
			});
		}

		// Token: 0x06004D51 RID: 19793 RVA: 0x0016B49D File Offset: 0x0016969D
		public void QueryContainerBlobs(string containerName, GameCoreSaveHandler.QueryBlobsCallback callback)
		{
			GameCoreSaveHandler.QueryBlobsCallback <>9__1;
			Task.Run(delegate()
			{
				this.StartOperation();
				Delegate callback2 = callback;
				GameCoreSaveHandler.QueryBlobsCallback b;
				if ((b = <>9__1) == null)
				{
					b = (<>9__1 = delegate(int hresult, Dictionary<string, uint> quota)
					{
						this.FinishOperation();
					});
				}
				callback = (GameCoreSaveHandler.QueryBlobsCallback)Delegate.Combine(callback2, b);
				XGameSaveContainerHandle xgameSaveContainerHandle;
				int num = SDK.XGameSaveCreateContainer(this.m_gameSaveProviderHandle, containerName, out xgameSaveContainerHandle);
				if (XGamingRuntime.Interop.HR.FAILED(num))
				{
					callback(num, new Dictionary<string, uint>());
					return;
				}
				XGameSaveBlobInfo[] array;
				num = SDK.XGameSaveEnumerateBlobInfo(xgameSaveContainerHandle, out array);
				Dictionary<string, uint> dictionary = new Dictionary<string, uint>();
				if (XGamingRuntime.Interop.HR.SUCCEEDED(num))
				{
					for (int i = 0; i < array.Length; i++)
					{
						dictionary.Add(array[i].Name, array[i].Size);
					}
				}
				SDK.XGameSaveCloseContainer(xgameSaveContainerHandle);
				callback(num, dictionary);
			});
		}

		// Token: 0x06004D52 RID: 19794 RVA: 0x0016B4CA File Offset: 0x001696CA
		public void Load(string containerName, string blobName, GameCoreSaveHandler.LoadCallback callback)
		{
			GameCoreSaveHandler.LoadCallback <>9__1;
			Task.Run(delegate()
			{
				this.StartOperation();
				Delegate callback2 = callback;
				GameCoreSaveHandler.LoadCallback b;
				if ((b = <>9__1) == null)
				{
					b = (<>9__1 = delegate(int hresult, byte[] quota)
					{
						this.FinishOperation();
					});
				}
				callback = (GameCoreSaveHandler.LoadCallback)Delegate.Combine(callback2, b);
				XGameSaveContainerHandle containerHandle;
				int num = SDK.XGameSaveCreateContainer(this.m_gameSaveProviderHandle, containerName, out containerHandle);
				if (XGamingRuntime.Interop.HR.FAILED(num))
				{
					callback(num, null);
					return;
				}
				string[] blobNames = new string[]
				{
					blobName
				};
				SDK.XGameSaveReadBlobDataAsync(containerHandle, blobNames, delegate(int hresult, XGameSaveBlob[] blobs)
				{
					byte[] blobData = null;
					if (XGamingRuntime.Interop.HR.SUCCEEDED(hresult) && blobs.Length != 0)
					{
						blobData = blobs[0].Data;
					}
					SDK.XGameSaveCloseContainer(containerHandle);
					callback(hresult, blobData);
				});
			});
		}

		// Token: 0x06004D53 RID: 19795 RVA: 0x0016B4FE File Offset: 0x001696FE
		public void Save(string containerName, string blobName, byte[] blobData, GameCoreSaveHandler.SaveCallback callback)
		{
			Task.Run(delegate()
			{
				Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>();
				dictionary.Add(blobName, blobData);
				this.Update(containerName, dictionary, null, new GameCoreSaveHandler.UpdateCallback(callback.Invoke));
			});
		}

		// Token: 0x06004D54 RID: 19796 RVA: 0x0016B53A File Offset: 0x0016973A
		public void Delete(string containerName, GameCoreSaveHandler.DeleteCallback callback)
		{
			this.StartOperation();
			callback = (GameCoreSaveHandler.DeleteCallback)Delegate.Combine(callback, new GameCoreSaveHandler.DeleteCallback(delegate(int hresult)
			{
				this.FinishOperation();
			}));
			SDK.XGameSaveDeleteContainerAsync(this.m_gameSaveProviderHandle, containerName, new XGameSaveDeleteContainerCompleted(callback.Invoke));
		}

		// Token: 0x06004D55 RID: 19797 RVA: 0x0016B573 File Offset: 0x00169773
		public void Delete(string containerName, string blobName, GameCoreSaveHandler.DeleteCallback callback)
		{
			this.Delete(containerName, new string[]
			{
				blobName
			}, callback);
		}

		// Token: 0x06004D56 RID: 19798 RVA: 0x0016B587 File Offset: 0x00169787
		public void Delete(string containerName, string[] blobNames, GameCoreSaveHandler.DeleteCallback callback)
		{
			this.Update(containerName, null, blobNames, new GameCoreSaveHandler.UpdateCallback(callback.Invoke));
		}

		// Token: 0x06004D57 RID: 19799 RVA: 0x0016B5A0 File Offset: 0x001697A0
		private void Update(string containerName, IDictionary<string, byte[]> blobsToSave, IList<string> blobsToDelete, GameCoreSaveHandler.UpdateCallback callback)
		{
			this.StartOperation();
			callback = (GameCoreSaveHandler.UpdateCallback)Delegate.Combine(callback, new GameCoreSaveHandler.UpdateCallback(delegate(int hresult)
			{
				this.FinishOperation();
			}));
			XGameSaveContainerHandle containerHandle;
			int num = SDK.XGameSaveCreateContainer(this.m_gameSaveProviderHandle, containerName, out containerHandle);
			if (XGamingRuntime.Interop.HR.FAILED(num))
			{
				callback(num);
				return;
			}
			XGameSaveUpdateHandle updateHandle;
			num = SDK.XGameSaveCreateUpdate(containerHandle, containerName, out updateHandle);
			if (XGamingRuntime.Interop.HR.FAILED(num))
			{
				SDK.XGameSaveCloseContainer(containerHandle);
				callback(num);
				return;
			}
			if (blobsToSave != null)
			{
				foreach (KeyValuePair<string, byte[]> keyValuePair in blobsToSave)
				{
					num = SDK.XGameSaveSubmitBlobWrite(updateHandle, keyValuePair.Key, keyValuePair.Value);
					if (XGamingRuntime.Interop.HR.FAILED(num))
					{
						SDK.XGameSaveCloseUpdateHandle(updateHandle);
						SDK.XGameSaveCloseContainer(containerHandle);
						callback(num);
						return;
					}
				}
			}
			if (blobsToDelete != null)
			{
				foreach (string blobName in blobsToDelete)
				{
					num = SDK.XGameSaveSubmitBlobDelete(updateHandle, blobName);
					if (XGamingRuntime.Interop.HR.FAILED(num))
					{
						SDK.XGameSaveCloseUpdateHandle(updateHandle);
						SDK.XGameSaveCloseContainer(containerHandle);
						callback(num);
						return;
					}
				}
			}
			SDK.XGameSaveSubmitUpdateAsync(updateHandle, delegate(int hresult)
			{
				SDK.XGameSaveCloseUpdateHandle(updateHandle);
				SDK.XGameSaveCloseContainer(containerHandle);
				callback(hresult);
			});
		}

		// Token: 0x04004E21 RID: 20001
		private XGamingRuntime.XUserHandle m_userHandle;

		// Token: 0x04004E22 RID: 20002
		private XGameSaveProviderHandle m_gameSaveProviderHandle;

		// Token: 0x04004E24 RID: 20004
		private int activeOperations;

		// Token: 0x04004E25 RID: 20005
		private bool isInitialising;

		// Token: 0x04004E26 RID: 20006
		private bool isDisposed;

		// Token: 0x02001B2D RID: 6957
		// (Invoke) Token: 0x0600994D RID: 39245
		public delegate void InitializeCallback(int hresult);

		// Token: 0x02001B2E RID: 6958
		// (Invoke) Token: 0x06009951 RID: 39249
		public delegate void GetQuotaCallback(int hresult, long remainingQuota);

		// Token: 0x02001B2F RID: 6959
		// (Invoke) Token: 0x06009955 RID: 39253
		public delegate void QueryContainersCallback(int hresult, string[] containerNames);

		// Token: 0x02001B30 RID: 6960
		// (Invoke) Token: 0x06009959 RID: 39257
		public delegate void QueryBlobsCallback(int hresult, Dictionary<string, uint> blobInfos);

		// Token: 0x02001B31 RID: 6961
		// (Invoke) Token: 0x0600995D RID: 39261
		public delegate void LoadCallback(int hresult, byte[] blobData);

		// Token: 0x02001B32 RID: 6962
		// (Invoke) Token: 0x06009961 RID: 39265
		public delegate void SaveCallback(int hresult);

		// Token: 0x02001B33 RID: 6963
		// (Invoke) Token: 0x06009965 RID: 39269
		public delegate void DeleteCallback(int hresult);

		// Token: 0x02001B34 RID: 6964
		// (Invoke) Token: 0x06009969 RID: 39273
		private delegate void UpdateCallback(int hresult);
	}
}
