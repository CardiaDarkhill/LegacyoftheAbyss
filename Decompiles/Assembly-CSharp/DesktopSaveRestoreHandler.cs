using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

// Token: 0x02000454 RID: 1108
public sealed class DesktopSaveRestoreHandler : SaveRestoreHandler
{
	// Token: 0x06002721 RID: 10017 RVA: 0x000AFFED File Offset: 0x000AE1ED
	public DesktopSaveRestoreHandler(string dataPath)
	{
		this.DATA_PATH = dataPath;
	}

	// Token: 0x06002722 RID: 10018 RVA: 0x000AFFFC File Offset: 0x000AE1FC
	public override void WriteSaveRestorePoint(int slot, string identifier, bool noDelete, byte[] bytes, Action<bool> callback)
	{
		Task.Run(delegate()
		{
			try
			{
				string directoryPath = this.GetDirectoryPath(slot);
				if (!Directory.Exists(directoryPath))
				{
					Directory.CreateDirectory(directoryPath);
					if (!Directory.Exists(directoryPath))
					{
						Debug.LogError("Failed to create directory " + directoryPath);
						Action<bool> callback2 = callback;
						if (callback2 != null)
						{
							callback2(false);
						}
						return;
					}
				}
				DesktopSaveRestoreHandler.RestorePointList restorePointList = this.GetRestorePointList(slot);
				restorePointList.TrimAll(19, identifier, this);
				int num = restorePointList.max + 1;
				string fileName = SaveRestoreHandler.GetFileName(num, noDelete);
				string path = Path.Combine(this.GetDirectoryPath(slot), fileName);
				RestorePointFileWrapper restorePointFileWrapper = new RestorePointFileWrapper(bytes, num, identifier);
				restorePointFileWrapper.SetVersion();
				restorePointFileWrapper.SetDateString();
				byte[] bytesForSaveData = SaveRestoreHandler.GetBytesForSaveData<RestorePointFileWrapper>(restorePointFileWrapper);
				File.WriteAllBytes(path, bytesForSaveData);
				Action<bool> callback3 = callback;
				if (callback3 != null)
				{
					callback3(true);
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				Action<bool> callback4 = callback;
				if (callback4 != null)
				{
					callback4(false);
				}
			}
		});
	}

	// Token: 0x06002723 RID: 10019 RVA: 0x000B004B File Offset: 0x000AE24B
	public override void WriteVersionBackup(int slot, byte[] bytes, Action<bool> callback)
	{
		Task.Run(delegate()
		{
			try
			{
				string fullVersionBackupPath = this.GetFullVersionBackupPath(slot);
				this.TrimVersionBackups(slot, 2);
				File.WriteAllBytes(fullVersionBackupPath, bytes);
				Action<bool> callback2 = callback;
				if (callback2 != null)
				{
					callback2(true);
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				Action<bool> callback3 = callback;
				if (callback3 != null)
				{
					callback3(false);
				}
			}
		});
	}

	// Token: 0x06002724 RID: 10020 RVA: 0x000B0080 File Offset: 0x000AE280
	public override FetchDataRequest FetchRestorePoints(int slot)
	{
		FetchDataRequest request = new FetchDataRequest();
		request.State = FetchDataRequest.Status.InProgress;
		Task.Run(delegate()
		{
			string directoryPath = this.GetDirectoryPath(slot);
			if (!Directory.Exists(directoryPath))
			{
				request.State = FetchDataRequest.Status.Completed;
				return;
			}
			string[] files = Directory.GetFiles(directoryPath, "*.dat");
			if (files.Length == 0)
			{
				request.State = FetchDataRequest.Status.Completed;
				return;
			}
			int returnCount = 0;
			request.RestorePoints = new List<RestorePointFileWrapper>();
			Action<RestorePointFileWrapper> <>9__1;
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i];
				SaveRestoreHandler <>4__this = this;
				string path = text;
				Action<RestorePointFileWrapper> callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate(RestorePointFileWrapper data)
					{
						int returnCount = returnCount;
						returnCount++;
						if (data != null)
						{
							request.AddResult(data);
						}
						if (returnCount == files.Length)
						{
							request.State = FetchDataRequest.Status.Completed;
						}
					});
				}
				<>4__this.LoadRestorePoint(path, callback);
			}
		});
		return request;
	}

	// Token: 0x06002725 RID: 10021 RVA: 0x000B00D0 File Offset: 0x000AE2D0
	public override FetchDataRequest FetchVersionBackupPoints(int slot)
	{
		FetchDataRequest request = new FetchDataRequest();
		request.State = FetchDataRequest.Status.InProgress;
		Task.Run(delegate()
		{
			string data_PATH = this.DATA_PATH;
			if (!Directory.Exists(data_PATH))
			{
				request.State = FetchDataRequest.Status.Completed;
				return;
			}
			string searchPattern = this.GetVersionedBackupName(slot) + "*.dat";
			string[] files = Directory.GetFiles(data_PATH, searchPattern);
			if (files.Length == 0)
			{
				request.State = FetchDataRequest.Status.Completed;
				return;
			}
			int returnCount = 0;
			request.RestorePoints = new List<RestorePointFileWrapper>();
			Action<RestorePointFileWrapper> <>9__1;
			for (int i = 0; i < files.Length; i++)
			{
				string text = files[i];
				DesktopSaveRestoreHandler <>4__this = this;
				string path = text;
				Action<RestorePointFileWrapper> callback;
				if ((callback = <>9__1) == null)
				{
					callback = (<>9__1 = delegate(RestorePointFileWrapper data)
					{
						int returnCount = returnCount;
						returnCount++;
						if (data != null)
						{
							request.AddResult(data);
						}
						if (returnCount == files.Length)
						{
							request.State = FetchDataRequest.Status.Completed;
						}
					});
				}
				<>4__this.LoadBackupPoint(path, callback);
			}
		});
		return request;
	}

	// Token: 0x06002726 RID: 10022 RVA: 0x000B011E File Offset: 0x000AE31E
	public override void DeleteRestorePoints(int slot, Action<bool> callback)
	{
		Task.Run(delegate()
		{
			string directoryPath = this.GetDirectoryPath(slot);
			if (!Directory.Exists(directoryPath))
			{
				callback(true);
				return;
			}
			try
			{
				Directory.Delete(directoryPath, true);
				callback(true);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				callback(false);
			}
		});
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x000B014B File Offset: 0x000AE34B
	public override void DeleteVersionBackups(int slot, Action<bool> callback)
	{
		Task.Run(delegate()
		{
			string data_PATH = this.DATA_PATH;
			if (!Directory.Exists(data_PATH))
			{
				return;
			}
			string searchPattern = this.GetVersionedBackupName(slot) + "*.dat";
			string[] files = Directory.GetFiles(data_PATH, searchPattern);
			if (files.Length == 0)
			{
				return;
			}
			foreach (string path in files)
			{
				try
				{
					File.Delete(path);
				}
				catch (Exception message)
				{
					Debug.LogError(message);
				}
			}
		});
	}

	// Token: 0x06002728 RID: 10024 RVA: 0x000B0171 File Offset: 0x000AE371
	private string GetDirectoryPath(int slot)
	{
		return Path.Combine(this.DATA_PATH, SaveRestoreHandler.GetDirectoryName(slot));
	}

	// Token: 0x06002729 RID: 10025 RVA: 0x000B0184 File Offset: 0x000AE384
	private string GetFullVersionBackupPath(int slot)
	{
		return Path.Combine(this.DATA_PATH, base.GetFullVersionBackupName(slot));
	}

	// Token: 0x0600272A RID: 10026 RVA: 0x000B0198 File Offset: 0x000AE398
	private void TrimVersionBackups(int slot, int count)
	{
		string data_PATH = this.DATA_PATH;
		if (!Directory.Exists(data_PATH))
		{
			return;
		}
		string versionedBackupName = this.GetVersionedBackupName(slot);
		string[] files = Directory.GetFiles(data_PATH, versionedBackupName + "*.dat");
		if (files.Length <= count)
		{
			return;
		}
		List<DesktopSaveRestoreHandler.VersionInfo> list = new List<DesktopSaveRestoreHandler.VersionInfo>();
		foreach (string text in files)
		{
			DesktopSaveRestoreHandler.VersionInfo versionInfo = default(DesktopSaveRestoreHandler.VersionInfo);
			versionInfo.file = text;
			string text2 = Path.GetFileNameWithoutExtension(text);
			text2 = text2.Replace(versionedBackupName, "");
			text2 = SaveDataUtility.CleanupVersionText(text2);
			try
			{
				versionInfo.version = new Version(text2);
			}
			catch (Exception message)
			{
				Debug.LogError(message);
				versionInfo.version = new Version(0, 0, 0, 0);
			}
			bool flag = false;
			for (int j = 0; j < list.Count; j++)
			{
				DesktopSaveRestoreHandler.VersionInfo versionInfo2 = list[j];
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
		int num = files.Length - count;
		if (num > 0)
		{
			for (int k = list.Count - 1; k >= 0; k--)
			{
				DesktopSaveRestoreHandler.VersionInfo versionInfo3 = list[k];
				try
				{
					File.Delete(versionInfo3.file);
				}
				catch (Exception message2)
				{
					Debug.LogError(message2);
				}
				if (--num <= 0)
				{
					break;
				}
			}
		}
	}

	// Token: 0x0600272B RID: 10027 RVA: 0x000B0314 File Offset: 0x000AE514
	private DesktopSaveRestoreHandler.RestorePointList GetRestorePointList(int slot)
	{
		DesktopSaveRestoreHandler.RestorePointList restorePointList = new DesktopSaveRestoreHandler.RestorePointList();
		string directoryPath = this.GetDirectoryPath(slot);
		if (!Directory.Exists(directoryPath))
		{
			Debug.LogError("Directory: \"" + directoryPath + "\" does not exist");
			return restorePointList;
		}
		string[] files = Directory.GetFiles(directoryPath, "*.dat");
		if (files.Length == 0)
		{
			return restorePointList;
		}
		restorePointList.infos = new List<DesktopSaveRestoreHandler.DesktopInfo>(files.Length);
		restorePointList.noDeleteInfo = new List<DesktopSaveRestoreHandler.DesktopInfo>(files.Length);
		foreach (string text in files)
		{
			DesktopSaveRestoreHandler.DesktopInfo desktopInfo = new DesktopSaveRestoreHandler.DesktopInfo();
			desktopInfo.file = text;
			SaveRestoreHandler.RestorePointFileInfo restorePointFileInfo = new SaveRestoreHandler.RestorePointFileInfo(text);
			desktopInfo.number = restorePointFileInfo.number;
			if (restorePointFileInfo.number > restorePointList.max)
			{
				restorePointList.max = restorePointFileInfo.number;
			}
			if (restorePointFileInfo.isNoDelete)
			{
				restorePointList.noDeleteInfo.Add(desktopInfo);
			}
			else
			{
				restorePointList.infos.Add(desktopInfo);
			}
		}
		restorePointList.CompleteLoad(this, null);
		return restorePointList;
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x000B0400 File Offset: 0x000AE600
	protected override void LoadRestorePoint(string path, Action<RestorePointFileWrapper> callback)
	{
		try
		{
			RestorePointFileWrapper obj = SaveDataUtility.DeserializeSaveData<RestorePointFileWrapper>(SaveRestoreHandler.GetJsonForSaveBytes(File.ReadAllBytes(path)));
			if (callback != null)
			{
				callback(obj);
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			if (callback != null)
			{
				callback(null);
			}
		}
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x000B044C File Offset: 0x000AE64C
	private void LoadBackupPoint(string path, Action<RestorePointFileWrapper> callback)
	{
		try
		{
			RestorePointFileWrapper obj = new RestorePointFileWrapper(File.ReadAllBytes(path));
			if (callback != null)
			{
				callback(obj);
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			if (callback != null)
			{
				callback(null);
			}
		}
	}

	// Token: 0x0400241C RID: 9244
	private readonly string DATA_PATH;

	// Token: 0x02001741 RID: 5953
	private sealed class DesktopInfo : SaveRestoreHandler.Info<DesktopSaveRestoreHandler>
	{
		// Token: 0x06008D2F RID: 36143 RVA: 0x002891EE File Offset: 0x002873EE
		public override void Delete()
		{
			if (string.IsNullOrEmpty(this.file))
			{
				return;
			}
			File.Delete(this.file);
		}

		// Token: 0x06008D30 RID: 36144 RVA: 0x00289209 File Offset: 0x00287409
		public override void Commit()
		{
		}
	}

	// Token: 0x02001742 RID: 5954
	private sealed class RestorePointList : SaveRestoreHandler.RestorePointList<DesktopSaveRestoreHandler.DesktopInfo, DesktopSaveRestoreHandler>
	{
		// Token: 0x06008D32 RID: 36146 RVA: 0x00289213 File Offset: 0x00287413
		public override void PrepareDelete()
		{
		}

		// Token: 0x06008D33 RID: 36147 RVA: 0x00289215 File Offset: 0x00287415
		public override void Delete(DesktopSaveRestoreHandler.DesktopInfo info)
		{
			base.Delete(info);
		}

		// Token: 0x06008D34 RID: 36148 RVA: 0x0028921E File Offset: 0x0028741E
		public override void Commit()
		{
		}
	}

	// Token: 0x02001743 RID: 5955
	private struct VersionInfo
	{
		// Token: 0x04008DB5 RID: 36277
		public string file;

		// Token: 0x04008DB6 RID: 36278
		public Version version;
	}
}
