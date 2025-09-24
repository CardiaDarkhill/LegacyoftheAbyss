using System;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamCherry.GameCore
{
	// Token: 0x020008B8 RID: 2232
	public sealed class GameCoreSharedData : CustomSharedData
	{
		// Token: 0x1700091B RID: 2331
		// (get) Token: 0x06004D5B RID: 19803 RVA: 0x0016B768 File Offset: 0x00169968
		public bool HasLoaded
		{
			get
			{
				return this.hasLoaded;
			}
		}

		// Token: 0x06004D5C RID: 19804 RVA: 0x0016B772 File Offset: 0x00169972
		public GameCoreSharedData(string container, string fileName)
		{
			this.container = container;
			this.fileName = fileName;
			this.LoadData(null);
		}

		// Token: 0x06004D5D RID: 19805 RVA: 0x0016B79C File Offset: 0x0016999C
		~GameCoreSharedData()
		{
		}

		// Token: 0x06004D5E RID: 19806 RVA: 0x0016B7C4 File Offset: 0x001699C4
		public override void ImportData(Platform.ISharedData otherData)
		{
			object obj = this.loadLock;
			lock (obj)
			{
				if (!this.hasLoaded)
				{
					this.onDataLoaded = (Action)Delegate.Combine(this.onDataLoaded, new Action(delegate()
					{
						this.ImportData(otherData);
					}));
					return;
				}
			}
			base.ImportData(otherData);
		}

		// Token: 0x06004D5F RID: 19807 RVA: 0x0016B84C File Offset: 0x00169A4C
		public void LoadData(Action callback = null)
		{
			object obj = this.loadLock;
			lock (obj)
			{
				if (this.dataRequested)
				{
					this.onDataLoaded = (Action)Delegate.Combine(this.onDataLoaded, callback);
					return;
				}
				this.dataRequested = true;
				this.hasLoaded = false;
			}
			if (string.IsNullOrEmpty(this.container))
			{
				Debug.LogError("Unable to load shared data. Missing container name.");
				return;
			}
			if (string.IsNullOrEmpty(this.fileName))
			{
				Debug.LogError("Unable to load shared data. Missing file name.");
				return;
			}
			GameCoreRuntimeManager.LoadSaveData(this.container, this.fileName, delegate(byte[] byteData)
			{
				if (byteData == null)
				{
					Debug.LogError("Failed to load data " + this.container + " " + this.fileName);
					CoreLoop.InvokeSafe(delegate
					{
						base.SharedData.Clear();
						Action action = null;
						object obj2 = this.loadLock;
						lock (obj2)
						{
							this.hasLoaded = true;
							this.dataRequested = false;
							action = this.onDataLoaded;
							this.onDataLoaded = null;
						}
						if (action != null)
						{
							action();
						}
					});
					return;
				}
				CoreLoop.InvokeSafe(delegate
				{
					this.SharedData.Clear();
					this.LoadFromJSON(CustomSharedData.BytesToJson(byteData, this.useEncryption));
					Action action = null;
					object obj2 = this.loadLock;
					lock (obj2)
					{
						this.hasLoaded = true;
						this.dataRequested = false;
						action = this.onDataLoaded;
						this.onDataLoaded = null;
					}
					if (action != null)
					{
						action();
					}
				});
			});
		}

		// Token: 0x06004D60 RID: 19808 RVA: 0x0016B904 File Offset: 0x00169B04
		public override void Save()
		{
			if (string.IsNullOrEmpty(this.container))
			{
				Debug.LogError("Unable to save shared data. Missing container name.");
				return;
			}
			if (string.IsNullOrEmpty(this.fileName))
			{
				Debug.LogError("Unable to save shared data. Missing file name.");
				return;
			}
			if (GameCoreRuntimeManager.SaveSystemInitialised)
			{
				SaveDataUtility.AddTaskToAsyncQueue(delegate(TaskCompletionSource<string> tcs)
				{
					byte[] data = CustomSharedData.JsonToBytes(base.SaveToJSON(), this.useEncryption);
					GameCoreRuntimeManager.Save(this.container, this.fileName, data, null);
					tcs.SetResult(null);
				}, delegate(bool success, string result)
				{
					if (!success)
					{
						Debug.LogError("Error writing JsonSharedData in thread: " + result);
					}
				});
			}
		}

		// Token: 0x04004E27 RID: 20007
		private volatile bool hasLoaded;

		// Token: 0x04004E28 RID: 20008
		private bool dataRequested;

		// Token: 0x04004E29 RID: 20009
		private string container;

		// Token: 0x04004E2A RID: 20010
		private string fileName;

		// Token: 0x04004E2B RID: 20011
		private bool useEncryption;

		// Token: 0x04004E2C RID: 20012
		private Action onDataLoaded;

		// Token: 0x04004E2D RID: 20013
		private object loadLock = new object();
	}
}
