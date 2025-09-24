using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000463 RID: 1123
public abstract class SaveRestoreHandler
{
	// Token: 0x1700048A RID: 1162
	// (get) Token: 0x0600283D RID: 10301 RVA: 0x000B1F23 File Offset: 0x000B0123
	public static int TotalFileLimit
	{
		get
		{
			return 30;
		}
	}

	// Token: 0x1700048B RID: 1163
	// (get) Token: 0x0600283E RID: 10302 RVA: 0x000B1F27 File Offset: 0x000B0127
	public virtual int FileLimit
	{
		get
		{
			return 5;
		}
	}

	// Token: 0x0600283F RID: 10303 RVA: 0x000B1F2A File Offset: 0x000B012A
	protected virtual string GetRestoreDirectory(int slot)
	{
		return SaveRestoreHandler.GetDirectoryName(slot);
	}

	// Token: 0x06002840 RID: 10304 RVA: 0x000B1F32 File Offset: 0x000B0132
	public static string GetTitle(int slot)
	{
		return string.Format("Restore Data #{0}", slot);
	}

	// Token: 0x06002841 RID: 10305 RVA: 0x000B1F44 File Offset: 0x000B0144
	public static string GetFileName(int slot, bool noDelete = false)
	{
		if (noDelete)
		{
			return SaveRestoreHandler.GetNoDeleteFileName(slot);
		}
		return string.Format("{0}{1}{2}", "restoreData", slot, ".dat");
	}

	// Token: 0x06002842 RID: 10306 RVA: 0x000B1F6A File Offset: 0x000B016A
	public static string GetNoDeleteFileName(int slot)
	{
		return "NODEL" + SaveRestoreHandler.GetFileName(slot, false);
	}

	// Token: 0x06002843 RID: 10307 RVA: 0x000B1F7D File Offset: 0x000B017D
	public static string GetDirectoryName(int slot)
	{
		return string.Format("{0}{1}", "Restore_Points", slot);
	}

	// Token: 0x06002844 RID: 10308 RVA: 0x000B1F94 File Offset: 0x000B0194
	protected virtual string GetVersionedBackupName(int slot)
	{
		return string.Format("user{0}_", slot);
	}

	// Token: 0x06002845 RID: 10309 RVA: 0x000B1FA6 File Offset: 0x000B01A6
	protected string GetFullVersionBackupName(int slot)
	{
		return this.GetVersionedBackupName(slot) + "1.0.28324.dat";
	}

	// Token: 0x06002846 RID: 10310 RVA: 0x000B1FBC File Offset: 0x000B01BC
	protected static string GetJsonForSaveBytes(byte[] fileBytes)
	{
		string result;
		if (GameManager.instance.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(fileBytes);
			result = Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
		}
		else
		{
			result = Encoding.UTF8.GetString(fileBytes);
		}
		return result;
	}

	// Token: 0x06002847 RID: 10311 RVA: 0x000B2018 File Offset: 0x000B0218
	protected static byte[] GetBytesForSaveJson(string jsonData)
	{
		byte[] result;
		if (GameManager.instance.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
		{
			string graph = Encryption.Encrypt(jsonData);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, graph);
			result = memoryStream.ToArray();
			memoryStream.Close();
		}
		else
		{
			result = Encoding.UTF8.GetBytes(jsonData);
		}
		return result;
	}

	// Token: 0x06002848 RID: 10312 RVA: 0x000B207D File Offset: 0x000B027D
	protected static byte[] GetBytesForSaveData<T>(T data)
	{
		return SaveRestoreHandler.GetBytesForSaveJson(SaveDataUtility.SerializeSaveData<T>(data));
	}

	// Token: 0x06002849 RID: 10313 RVA: 0x000B208A File Offset: 0x000B028A
	public virtual void WriteSaveRestorePoint(int slot, string identifier, bool noDelete, byte[] bytes, Action<bool> callback)
	{
		if (callback != null)
		{
			callback(false);
		}
	}

	// Token: 0x0600284A RID: 10314
	public abstract void WriteVersionBackup(int slot, byte[] bytes, Action<bool> callback);

	// Token: 0x0600284B RID: 10315
	public abstract FetchDataRequest FetchRestorePoints(int slot);

	// Token: 0x0600284C RID: 10316
	public abstract FetchDataRequest FetchVersionBackupPoints(int slot);

	// Token: 0x0600284D RID: 10317 RVA: 0x000B2098 File Offset: 0x000B0298
	protected virtual void LoadRestorePoint(string path, Action<RestorePointFileWrapper> callback)
	{
		if (callback != null)
		{
			callback(null);
		}
	}

	// Token: 0x0600284E RID: 10318
	public abstract void DeleteRestorePoints(int slot, Action<bool> callback);

	// Token: 0x0600284F RID: 10319
	public abstract void DeleteVersionBackups(int slot, Action<bool> callback);

	// Token: 0x06002850 RID: 10320 RVA: 0x000B20A4 File Offset: 0x000B02A4
	protected static string[] FilterResults(string[] source, string searchPattern)
	{
		if (source == null || string.IsNullOrEmpty(searchPattern))
		{
			return source;
		}
		string pattern = SaveRestoreHandler.WildcardToRegex(searchPattern);
		List<string> list = new List<string>();
		foreach (string text in source)
		{
			if (Regex.IsMatch(text, pattern))
			{
				list.Add(text);
			}
			else if (text.Contains(searchPattern))
			{
				list.Add(text);
			}
		}
		return list.ToArray();
	}

	// Token: 0x06002851 RID: 10321 RVA: 0x000B2105 File Offset: 0x000B0305
	private static string WildcardToRegex(string pattern)
	{
		return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
	}

	// Token: 0x04002459 RID: 9305
	public const string FOLDER_NAME = "Restore_Points";

	// Token: 0x0400245A RID: 9306
	public const string FILE_NAME = "restoreData";

	// Token: 0x0400245B RID: 9307
	public const string NO_DELETE = "NODEL";

	// Token: 0x0400245C RID: 9308
	public const string SAVE_FILE_EXT = ".dat";

	// Token: 0x0400245D RID: 9309
	protected const int FILE_LIMIT = 20;

	// Token: 0x0400245E RID: 9310
	protected const int NO_DELETE_TRIM_THRESHOLD = 2;

	// Token: 0x0400245F RID: 9311
	public const int VERSION_BACKUP_LIMIT = 3;

	// Token: 0x02001778 RID: 6008
	protected enum InfoLoadState
	{
		// Token: 0x04008E38 RID: 36408
		None,
		// Token: 0x04008E39 RID: 36409
		Loading,
		// Token: 0x04008E3A RID: 36410
		LoadComplete
	}

	// Token: 0x02001779 RID: 6009
	protected class Info<T> where T : SaveRestoreHandler
	{
		// Token: 0x06008DB3 RID: 36275 RVA: 0x00289E25 File Offset: 0x00288025
		public virtual void Delete()
		{
		}

		// Token: 0x06008DB4 RID: 36276 RVA: 0x00289E27 File Offset: 0x00288027
		public virtual void Commit()
		{
		}

		// Token: 0x17000F17 RID: 3863
		// (get) Token: 0x06008DB5 RID: 36277 RVA: 0x00289E29 File Offset: 0x00288029
		// (set) Token: 0x06008DB6 RID: 36278 RVA: 0x00289E33 File Offset: 0x00288033
		public SaveRestoreHandler.InfoLoadState LoadState
		{
			get
			{
				return this.loadState;
			}
			protected set
			{
				this.loadState = value;
			}
		}

		// Token: 0x06008DB7 RID: 36279 RVA: 0x00289E40 File Offset: 0x00288040
		public virtual void LoadData(T handler, Action callback = null)
		{
			this.LoadState = SaveRestoreHandler.InfoLoadState.Loading;
			handler.LoadRestorePoint(this.file, delegate(RestorePointFileWrapper wrapper)
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

		// Token: 0x04008E3B RID: 36411
		public string file;

		// Token: 0x04008E3C RID: 36412
		public int number;

		// Token: 0x04008E3D RID: 36413
		public RestorePointFileWrapper restorePointWrapper;

		// Token: 0x04008E3E RID: 36414
		private volatile SaveRestoreHandler.InfoLoadState loadState;
	}

	// Token: 0x0200177A RID: 6010
	protected abstract class RestorePointList<T, TV> where T : SaveRestoreHandler.Info<TV> where TV : SaveRestoreHandler
	{
		// Token: 0x06008DB9 RID: 36281 RVA: 0x00289E90 File Offset: 0x00288090
		public void TrimAll(int limit, string identifier, TV handler)
		{
			bool flag = false;
			if (this.Trim(limit, false))
			{
				flag = true;
			}
			if (this.TrimNoDelete(identifier, handler, false))
			{
				flag = true;
			}
			if (flag)
			{
				this.Commit();
			}
		}

		// Token: 0x06008DBA RID: 36282 RVA: 0x00289EC4 File Offset: 0x002880C4
		public bool Trim(int limit, bool commit = true)
		{
			if (this.infos == null)
			{
				return false;
			}
			if (this.infos.Count < limit)
			{
				return false;
			}
			int num = this.infos.Count - limit;
			if (num <= 0)
			{
				return false;
			}
			this.infos.Sort(delegate(T a, T b)
			{
				if (a.number > b.number)
				{
					return 1;
				}
				if (a.number < b.number)
				{
					return -1;
				}
				return 0;
			});
			bool flag = false;
			this.PrepareDelete();
			foreach (T t in this.infos)
			{
				if (t.number >= 0)
				{
					try
					{
						this.Delete(t);
						flag = true;
						if (--num <= 0)
						{
							break;
						}
					}
					catch (Exception message)
					{
						Debug.LogError(message);
					}
				}
			}
			if (flag && commit)
			{
				this.Commit();
			}
			return flag;
		}

		// Token: 0x06008DBB RID: 36283 RVA: 0x00289FB4 File Offset: 0x002881B4
		public bool TrimNoDelete(string identifier, TV handler, bool commit = true)
		{
			if (this.noDeleteInfo == null)
			{
				return false;
			}
			bool flag = false;
			if (this.noDeleteInfo.Count >= 2)
			{
				string.IsNullOrEmpty(identifier);
				int num = 0;
				for (int i = 0; i < this.noDeleteInfo.Count; i++)
				{
					T t = this.noDeleteInfo[i];
					if (t.restorePointWrapper != null)
					{
						string text = t.restorePointWrapper.identifier;
						if (string.IsNullOrEmpty(text))
						{
							try
							{
								RestorePointData restorePointData = SaveDataUtility.DeserializeSaveData<RestorePointData>(GameManager.GetJsonForSaveBytesStatic(t.restorePointWrapper.data));
								if (restorePointData == null)
								{
									goto IL_CF;
								}
								text = restorePointData.autoSaveName.ToString();
								t.restorePointWrapper.identifier = text;
							}
							catch (Exception)
							{
								goto IL_CF;
							}
						}
						if (!(text != identifier))
						{
							this.noDeleteInfo[num++] = t;
						}
					}
					IL_CF:;
				}
				if (num < this.noDeleteInfo.Count)
				{
					this.noDeleteInfo.RemoveRange(num, this.noDeleteInfo.Count - num);
				}
				if (this.noDeleteInfo.Count >= 2)
				{
					this.noDeleteInfo = (from e in this.noDeleteInfo
					orderby e.restorePointWrapper.date descending, e.number descending
					select e).ToList<T>();
					for (int j = 1; j < this.noDeleteInfo.Count; j++)
					{
						this.noDeleteInfo[j].Delete();
						flag = true;
					}
					if (flag && commit)
					{
						this.Commit();
					}
				}
			}
			return flag;
		}

		// Token: 0x06008DBC RID: 36284
		public abstract void PrepareDelete();

		// Token: 0x06008DBD RID: 36285 RVA: 0x0028A188 File Offset: 0x00288388
		public virtual void Delete(T info)
		{
			T t = info;
			if (t == null)
			{
				return;
			}
			t.Delete();
		}

		// Token: 0x06008DBE RID: 36286
		public abstract void Commit();

		// Token: 0x06008DBF RID: 36287 RVA: 0x0028A19C File Offset: 0x0028839C
		public virtual void CompleteLoad(TV handler, Action callback)
		{
			SaveRestoreHandler.RestorePointList<T, TV>.<>c__DisplayClass9_0 CS$<>8__locals1 = new SaveRestoreHandler.RestorePointList<T, TV>.<>c__DisplayClass9_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.callback = callback;
			if (this.noDeleteInfo.Count > 0)
			{
				CS$<>8__locals1.count = 0;
				CS$<>8__locals1.expected = this.noDeleteInfo.Count;
				using (List<T>.Enumerator enumerator = this.noDeleteInfo.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						T t = enumerator.Current;
						t.LoadData(handler, new Action(CS$<>8__locals1.<CompleteLoad>g__Complete|0));
					}
					return;
				}
			}
			Action callback2 = CS$<>8__locals1.callback;
			if (callback2 == null)
			{
				return;
			}
			callback2();
		}

		// Token: 0x04008E3F RID: 36415
		public int max;

		// Token: 0x04008E40 RID: 36416
		public List<T> infos;

		// Token: 0x04008E41 RID: 36417
		public List<T> noDeleteInfo;
	}

	// Token: 0x0200177B RID: 6011
	protected readonly struct RestorePointFileInfo
	{
		// Token: 0x06008DC1 RID: 36289 RVA: 0x0028A250 File Offset: 0x00288450
		public RestorePointFileInfo(string path)
		{
			string text = Path.GetFileNameWithoutExtension(path);
			this.isNoDelete = text.StartsWith("NODEL");
			if (this.isNoDelete)
			{
				text = text.Substring(SaveRestoreHandler.RestorePointFileInfo.NO_DELETE_LENGTH, text.Length - SaveRestoreHandler.RestorePointFileInfo.NO_DELETE_LENGTH);
			}
			text = text.Substring(SaveRestoreHandler.RestorePointFileInfo.FILE_NAME_LENGTH, text.Length - SaveRestoreHandler.RestorePointFileInfo.FILE_NAME_LENGTH);
			if (!int.TryParse(text, out this.number))
			{
				Debug.LogError("Failed to parse number from \"" + path + "\"");
				this.number = -1;
			}
		}

		// Token: 0x04008E42 RID: 36418
		public readonly int number;

		// Token: 0x04008E43 RID: 36419
		public readonly bool isNoDelete;

		// Token: 0x04008E44 RID: 36420
		private static readonly int FILE_NAME_LENGTH = "restoreData".Length;

		// Token: 0x04008E45 RID: 36421
		private static readonly int NO_DELETE_LENGTH = "NODEL".Length;
	}
}
