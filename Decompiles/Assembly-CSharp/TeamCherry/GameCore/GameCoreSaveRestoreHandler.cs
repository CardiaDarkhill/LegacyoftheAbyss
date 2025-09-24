using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamCherry.GameCore
{
	// Token: 0x020008B3 RID: 2227
	public sealed class GameCoreSaveRestoreHandler : SaveRestoreHandler
	{
		// Token: 0x06004CF2 RID: 19698 RVA: 0x001696A7 File Offset: 0x001678A7
		protected override string GetRestoreDirectory(int slot)
		{
			return GameCoreRuntimeManager.GetRestoreContainerName(slot);
		}

		// Token: 0x06004CF3 RID: 19699 RVA: 0x001696B0 File Offset: 0x001678B0
		public override void WriteSaveRestorePoint(int slot, string identifier, bool noDelete, byte[] bytes, Action<bool> callback)
		{
			GameCoreSaveRestoreHandler.<>c__DisplayClass2_0 CS$<>8__locals1 = new GameCoreSaveRestoreHandler.<>c__DisplayClass2_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.identifier = identifier;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.noDelete = noDelete;
			CS$<>8__locals1.bytes = bytes;
			try
			{
				string directory = this.GetRestoreDirectory(slot);
				this.GetRestorePointList(slot, delegate(GameCoreSaveRestoreHandler.RestorePointList pointList)
				{
					pointList.TrimAll(19, CS$<>8__locals1.identifier, CS$<>8__locals1.<>4__this);
					int num = pointList.max + 1;
					string fileName = SaveRestoreHandler.GetFileName(num, CS$<>8__locals1.noDelete);
					RestorePointFileWrapper restorePointFileWrapper = new RestorePointFileWrapper(CS$<>8__locals1.bytes, num, CS$<>8__locals1.identifier);
					restorePointFileWrapper.SetVersion();
					restorePointFileWrapper.SetDateString();
					byte[] bytesForSaveData = SaveRestoreHandler.GetBytesForSaveData<RestorePointFileWrapper>(restorePointFileWrapper);
					GameCoreRuntimeManager.Save(directory, fileName, bytesForSaveData, new Action<bool>(CS$<>8__locals1.<WriteSaveRestorePoint>g__SafeInvoke|0));
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				CS$<>8__locals1.<WriteSaveRestorePoint>g__SafeInvoke|0(false);
			}
		}

		// Token: 0x06004CF4 RID: 19700 RVA: 0x00169738 File Offset: 0x00167938
		public override void WriteVersionBackup(int slot, byte[] bytes, Action<bool> callback)
		{
			GameCoreSaveRestoreHandler.<>c__DisplayClass3_0 CS$<>8__locals1 = new GameCoreSaveRestoreHandler.<>c__DisplayClass3_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.bytes = bytes;
			try
			{
				string directory = this.GetRestoreDirectory(slot);
				string backupName = base.GetFullVersionBackupName(slot);
				this.TrimVersionBackups(slot, 2, delegate
				{
					GameCoreRuntimeManager.Save(directory, backupName, CS$<>8__locals1.bytes, new Action<bool>(CS$<>8__locals1.<WriteVersionBackup>g__SafeInvoke|0));
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				Action<bool> callback2 = CS$<>8__locals1.callback;
				if (callback2 != null)
				{
					callback2(false);
				}
			}
		}

		// Token: 0x06004CF5 RID: 19701 RVA: 0x001697C0 File Offset: 0x001679C0
		public override FetchDataRequest FetchRestorePoints(int slot)
		{
			FetchDataRequest request = new FetchDataRequest();
			try
			{
				string directory = this.GetRestoreDirectory(slot);
				request.State = FetchDataRequest.Status.InProgress;
				GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
				{
					if (files == null || files.Length == 0)
					{
						request.State = FetchDataRequest.Status.Completed;
						return;
					}
					int returnCount = 0;
					request.RestorePoints = new List<RestorePointFileWrapper>();
					request.State = FetchDataRequest.Status.InProgress;
					Action<RestorePointFileWrapper> <>9__1;
					foreach (string text in files)
					{
						GameCoreSaveRestoreHandler <>4__this = this;
						string directory = directory;
						string file = text;
						Action<RestorePointFileWrapper> callback;
						if ((callback = <>9__1) == null)
						{
							callback = (<>9__1 = delegate(RestorePointFileWrapper data)
							{
								if (data != null)
								{
									request.AddResult(data);
								}
								if (Interlocked.Increment(ref returnCount) == files.Length)
								{
									request.State = FetchDataRequest.Status.Completed;
								}
							});
						}
						<>4__this.LoadRestorePoint(directory, file, callback);
					}
				});
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Error while trying to fetch restore points for Slot #{0} : {1}", slot, arg));
				request.State = FetchDataRequest.Status.Failed;
			}
			return request;
		}

		// Token: 0x06004CF6 RID: 19702 RVA: 0x00169854 File Offset: 0x00167A54
		public override FetchDataRequest FetchVersionBackupPoints(int slot)
		{
			FetchDataRequest request = new FetchDataRequest();
			try
			{
				string directory = this.GetRestoreDirectory(slot);
				string searchPattern = this.GetVersionedBackupName(slot) + "*.dat";
				request.State = FetchDataRequest.Status.InProgress;
				GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
				{
					if (files == null || files.Length == 0)
					{
						request.State = FetchDataRequest.Status.Completed;
						return;
					}
					files = SaveRestoreHandler.FilterResults(files, searchPattern);
					if (files == null || files.Length == 0)
					{
						request.State = FetchDataRequest.Status.Completed;
						return;
					}
					int returnCount = 0;
					request.RestorePoints = new List<RestorePointFileWrapper>();
					request.State = FetchDataRequest.Status.InProgress;
					Action<RestorePointFileWrapper> <>9__1;
					foreach (string text in files)
					{
						GameCoreSaveRestoreHandler <>4__this = this;
						string directory = directory;
						string fileName = text;
						Action<RestorePointFileWrapper> callback;
						if ((callback = <>9__1) == null)
						{
							callback = (<>9__1 = delegate(RestorePointFileWrapper data)
							{
								if (data != null)
								{
									request.AddResult(data);
								}
								if (Interlocked.Increment(ref returnCount) == files.Length)
								{
									request.State = FetchDataRequest.Status.Completed;
								}
							});
						}
						<>4__this.LoadBackupPoint(directory, fileName, callback);
					}
				});
			}
			catch (Exception arg)
			{
				Debug.LogError(string.Format("Error while trying to load versioned backups for Slot # {0} : {1}", slot, arg));
				request.State = FetchDataRequest.Status.Failed;
			}
			return request;
		}

		// Token: 0x06004CF7 RID: 19703 RVA: 0x00169900 File Offset: 0x00167B00
		public override void DeleteRestorePoints(int slot, Action<bool> callback)
		{
			string restoreDirectory = this.GetRestoreDirectory(slot);
			try
			{
				GameCoreRuntimeManager.DeleteContainer(restoreDirectory, callback);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				callback(false);
			}
		}

		// Token: 0x06004CF8 RID: 19704 RVA: 0x0016993C File Offset: 0x00167B3C
		public override void DeleteVersionBackups(int slot, Action<bool> callback)
		{
			try
			{
				string directory = this.GetRestoreDirectory(slot);
				string searchPattern = this.GetVersionedBackupName(slot) + "*.dat";
				GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
				{
					if (files == null || files.Length == 0)
					{
						return;
					}
					files = SaveRestoreHandler.FilterResults(files, searchPattern);
					if (files == null || files.Length == 0)
					{
						return;
					}
					foreach (string blobName in files)
					{
						try
						{
							GameCoreRuntimeManager.DeleteBlob(directory, blobName, null);
						}
						catch (Exception message2)
						{
							Debug.LogError(message2);
						}
					}
					Action<bool> callback3 = callback;
					if (callback3 == null)
					{
						return;
					}
					callback3.SafeInvoke(true);
				});
			}
			catch (Exception message)
			{
				Action<bool> callback2 = callback;
				if (callback2 != null)
				{
					callback2(false);
				}
				Debug.LogError(message);
			}
		}

		// Token: 0x06004CF9 RID: 19705 RVA: 0x001699BC File Offset: 0x00167BBC
		private void TrimVersionBackups(int slot, int count, Action callback)
		{
			GameCoreSaveRestoreHandler.<>c__DisplayClass8_0 CS$<>8__locals1 = new GameCoreSaveRestoreHandler.<>c__DisplayClass8_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.slot = slot;
			CS$<>8__locals1.count = count;
			Task.Run(delegate()
			{
				try
				{
					string directory = CS$<>8__locals1.<>4__this.GetRestoreDirectory(CS$<>8__locals1.slot);
					string versionedBackupName = CS$<>8__locals1.<>4__this.GetVersionedBackupName(CS$<>8__locals1.slot);
					string searchFilter = versionedBackupName + "*.dat";
					GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
					{
						if (files == null || files.Length == 0)
						{
							CS$<>8__locals1.<TrimVersionBackups>g__SafeInvoke|0();
							return;
						}
						files = SaveRestoreHandler.FilterResults(files, searchFilter);
						if (files == null || files.Length == 0)
						{
							CS$<>8__locals1.<TrimVersionBackups>g__SafeInvoke|0();
							return;
						}
						List<GameCoreSaveRestoreHandler.VersionInfo> list = new List<GameCoreSaveRestoreHandler.VersionInfo>();
						foreach (string text in files)
						{
							GameCoreSaveRestoreHandler.VersionInfo versionInfo = default(GameCoreSaveRestoreHandler.VersionInfo);
							versionInfo.file = text;
							string text2 = Path.GetFileNameWithoutExtension(text);
							text2 = text2.Replace(versionedBackupName, "");
							text2 = SaveDataUtility.CleanupVersionText(text2);
							try
							{
								versionInfo.version = new Version(text2);
							}
							catch (Exception message2)
							{
								Debug.LogError(message2);
								versionInfo.version = new Version(0, 0, 0, 0);
							}
							bool flag = false;
							for (int j = 0; j < list.Count; j++)
							{
								GameCoreSaveRestoreHandler.VersionInfo versionInfo2 = list[j];
								if (versionInfo.version >= versionInfo2.version)
								{
									list.Insert(j, versionInfo);
									flag = true;
									break;
								}
							}
							if (!flag)
							{
								list.Add(versionInfo);
							}
						}
						int num = files.Length - CS$<>8__locals1.count;
						if (num > 0)
						{
							for (int k = list.Count - 1; k >= 0; k--)
							{
								GameCoreSaveRestoreHandler.VersionInfo versionInfo3 = list[k];
								try
								{
									GameCoreRuntimeManager.DeleteBlob(directory, versionInfo3.file, null);
								}
								catch (Exception message3)
								{
									Debug.LogError(message3);
								}
								if (--num == 0)
								{
									break;
								}
							}
						}
						CS$<>8__locals1.<TrimVersionBackups>g__SafeInvoke|0();
					});
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					base.<TrimVersionBackups>g__SafeInvoke|0();
				}
			});
		}

		// Token: 0x06004CFA RID: 19706 RVA: 0x001699F0 File Offset: 0x00167BF0
		private void GetRestorePointList(int slot, Action<GameCoreSaveRestoreHandler.RestorePointList> callback)
		{
			GameCoreSaveRestoreHandler.<>c__DisplayClass9_0 CS$<>8__locals1 = new GameCoreSaveRestoreHandler.<>c__DisplayClass9_0();
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.slot = slot;
			CS$<>8__locals1.result = new GameCoreSaveRestoreHandler.RestorePointList();
			Task.Run(delegate()
			{
				try
				{
					string directory = CS$<>8__locals1.<>4__this.GetRestoreDirectory(CS$<>8__locals1.slot);
					GameCoreRuntimeManager.EnumerateFiles(directory, delegate(string[] files)
					{
						if (files == null || files.Length == 0)
						{
							CS$<>8__locals1.<GetRestorePointList>g__SafeInvoke|0();
							return;
						}
						CS$<>8__locals1.result.infos = new List<GameCoreSaveRestoreHandler.GameCoreInfo>(files.Length);
						CS$<>8__locals1.result.noDeleteInfo = new List<GameCoreSaveRestoreHandler.GameCoreInfo>(files.Length);
						foreach (string text in files)
						{
							GameCoreSaveRestoreHandler.GameCoreInfo gameCoreInfo = new GameCoreSaveRestoreHandler.GameCoreInfo();
							gameCoreInfo.file = text;
							gameCoreInfo.directory = directory;
							SaveRestoreHandler.RestorePointFileInfo restorePointFileInfo = new SaveRestoreHandler.RestorePointFileInfo(text);
							gameCoreInfo.number = restorePointFileInfo.number;
							if (restorePointFileInfo.number > CS$<>8__locals1.result.max)
							{
								CS$<>8__locals1.result.max = restorePointFileInfo.number;
							}
							if (restorePointFileInfo.isNoDelete)
							{
								CS$<>8__locals1.result.noDeleteInfo.Add(gameCoreInfo);
							}
							else
							{
								CS$<>8__locals1.result.infos.Add(gameCoreInfo);
							}
						}
						CS$<>8__locals1.result.CompleteLoad(CS$<>8__locals1.<>4__this, new Action(CS$<>8__locals1.<GetRestorePointList>g__SafeInvoke|0));
					});
				}
				catch (Exception ex)
				{
					base.<GetRestorePointList>g__SafeInvoke|0();
					Debug.LogError(ex.ToString());
				}
			});
		}

		// Token: 0x06004CFB RID: 19707 RVA: 0x00169A28 File Offset: 0x00167C28
		private void LoadRestorePoint(string directory, string file, Action<RestorePointFileWrapper> callback)
		{
			try
			{
				GameCoreRuntimeManager.LoadSaveData(directory, file, delegate(byte[] bytes)
				{
					if (bytes != null)
					{
						RestorePointFileWrapper obj = SaveDataUtility.DeserializeSaveData<RestorePointFileWrapper>(SaveRestoreHandler.GetJsonForSaveBytes(bytes));
						Action<RestorePointFileWrapper> callback3 = callback;
						if (callback3 == null)
						{
							return;
						}
						callback3(obj);
						return;
					}
					else
					{
						Action<RestorePointFileWrapper> callback4 = callback;
						if (callback4 == null)
						{
							return;
						}
						callback4(null);
						return;
					}
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				Action<RestorePointFileWrapper> callback2 = callback;
				if (callback2 != null)
				{
					callback2(null);
				}
			}
		}

		// Token: 0x06004CFC RID: 19708 RVA: 0x00169A80 File Offset: 0x00167C80
		private void LoadBackupPoint(string directory, string fileName, Action<RestorePointFileWrapper> callback)
		{
			try
			{
				GameCoreRuntimeManager.LoadSaveData(directory, fileName, delegate(byte[] byteData)
				{
					if (byteData != null)
					{
						RestorePointFileWrapper obj = new RestorePointFileWrapper(byteData);
						Action<RestorePointFileWrapper> callback3 = callback;
						if (callback3 == null)
						{
							return;
						}
						callback3(obj);
						return;
					}
					else
					{
						Action<RestorePointFileWrapper> callback4 = callback;
						if (callback4 == null)
						{
							return;
						}
						callback4(null);
						return;
					}
				});
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				Action<RestorePointFileWrapper> callback2 = callback;
				if (callback2 != null)
				{
					callback2(null);
				}
			}
		}

		// Token: 0x02001B0C RID: 6924
		private sealed class RestorePointList : SaveRestoreHandler.RestorePointList<GameCoreSaveRestoreHandler.GameCoreInfo, GameCoreSaveRestoreHandler>
		{
			// Token: 0x060098DF RID: 39135 RVA: 0x002B026D File Offset: 0x002AE46D
			public override void PrepareDelete()
			{
			}

			// Token: 0x060098E0 RID: 39136 RVA: 0x002B026F File Offset: 0x002AE46F
			public override void Commit()
			{
			}
		}

		// Token: 0x02001B0D RID: 6925
		private sealed class GameCoreInfo : SaveRestoreHandler.Info<GameCoreSaveRestoreHandler>
		{
			// Token: 0x060098E2 RID: 39138 RVA: 0x002B0279 File Offset: 0x002AE479
			public override void Delete()
			{
				GameCoreRuntimeManager.DeleteBlob(this.directory, this.file, null);
			}

			// Token: 0x060098E3 RID: 39139 RVA: 0x002B028D File Offset: 0x002AE48D
			public override void Commit()
			{
			}

			// Token: 0x060098E4 RID: 39140 RVA: 0x002B0290 File Offset: 0x002AE490
			public override void LoadData(GameCoreSaveRestoreHandler handler, Action callback)
			{
				base.LoadState = SaveRestoreHandler.InfoLoadState.Loading;
				handler.LoadRestorePoint(this.directory, this.file, delegate(RestorePointFileWrapper wrapper)
				{
					this.restorePointWrapper = wrapper;
					this.LoadState = SaveRestoreHandler.InfoLoadState.LoadComplete;
					Action callback2 = callback;
					if (callback2 == null)
					{
						return;
					}
					callback2();
				});
			}

			// Token: 0x04009B7D RID: 39805
			public string directory;
		}

		// Token: 0x02001B0E RID: 6926
		private struct VersionInfo
		{
			// Token: 0x04009B7E RID: 39806
			public string file;

			// Token: 0x04009B7F RID: 39807
			public Version version;
		}
	}
}
