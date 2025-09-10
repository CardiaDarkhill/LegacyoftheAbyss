using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.UnityConverters;
using UnityEngine;

// Token: 0x02000780 RID: 1920
public static class SaveDataUtility
{
	// Token: 0x06004420 RID: 17440 RVA: 0x0012B000 File Offset: 0x00129200
	public static string CleanupVersionText(string versionText)
	{
		return Regex.Replace(versionText, "[A-Za-z ]", "");
	}

	// Token: 0x06004421 RID: 17441 RVA: 0x0012B014 File Offset: 0x00129214
	public static bool IsVersionIncompatible(string currentGameVersion, string fileVersionText, int fileRevisionBreak, int manualRevisionBreak = 28104)
	{
		if (string.IsNullOrEmpty(fileVersionText))
		{
			Debug.LogWarning("Save slot has no version in file!");
			return true;
		}
		string text = SaveDataUtility.CleanupVersionText(fileVersionText);
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogWarning("Cleaning up save slot version resulted in empty version text!");
			return true;
		}
		Version version = new Version(text);
		Version version2 = new Version(SaveDataUtility.CleanupVersionText(currentGameVersion));
		return fileRevisionBreak > manualRevisionBreak || (version2.Major <= version.Major && (version2.Major < version.Major || version2.Minor < version.Minor));
	}

	// Token: 0x06004422 RID: 17442 RVA: 0x0012B09C File Offset: 0x0012929C
	private static void CreateJsonObjects()
	{
		if (SaveDataUtility._serializer != null)
		{
			return;
		}
		SaveDataUtility._serializer = JsonSerializer.Create(UnityConverterInitializer.defaultUnityConvertersSettings);
		SaveDataUtility._serializer.DefaultValueHandling = DefaultValueHandling.Populate;
		SaveDataUtility._serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
		SaveDataUtility._serializer.NullValueHandling = NullValueHandling.Ignore;
		SaveDataUtility._serializer.MissingMemberHandling = MissingMemberHandling.Ignore;
	}

	// Token: 0x06004423 RID: 17443 RVA: 0x0012B0EC File Offset: 0x001292EC
	public static string SerializeSaveData<T>(T saveData)
	{
		SaveDataUtility.CreateJsonObjects();
		StringBuilder stringBuilder = new StringBuilder();
		string result;
		using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StringWriter(stringBuilder)))
		{
			SaveDataUtility._serializer.Serialize(jsonTextWriter, saveData);
			result = stringBuilder.ToString();
		}
		return result;
	}

	// Token: 0x06004424 RID: 17444 RVA: 0x0012B148 File Offset: 0x00129348
	public static void SerializeToJsonAsync<T>(T saveData, Action<bool, string> onComplete)
	{
		SaveDataUtility.AddTaskToAsyncQueue(delegate(TaskCompletionSource<string> tcs)
		{
			string result = SaveDataUtility.SerializeSaveData<T>(saveData);
			tcs.SetResult(result);
		}, onComplete);
	}

	// Token: 0x06004425 RID: 17445 RVA: 0x0012B168 File Offset: 0x00129368
	public static T DeserializeSaveData<T>(string json) where T : new()
	{
		SaveDataUtility.CreateJsonObjects();
		JsonTextReader reader = new JsonTextReader(new StringReader(json));
		return SaveDataUtility._serializer.Deserialize<T>(reader);
	}

	// Token: 0x170007A2 RID: 1954
	// (get) Token: 0x06004426 RID: 17446 RVA: 0x0012B191 File Offset: 0x00129391
	// (set) Token: 0x06004427 RID: 17447 RVA: 0x0012B19D File Offset: 0x0012939D
	public static ThreadPriority ThreadPriority
	{
		get
		{
			return SaveDataUtility._saveWorkerThread.ThreadPriority;
		}
		set
		{
			SaveDataUtility._saveWorkerThread.ThreadPriority = value;
		}
	}

	// Token: 0x06004428 RID: 17448 RVA: 0x0012B1AC File Offset: 0x001293AC
	public static void AddTaskToAsyncQueue(Action<TaskCompletionSource<string>> taskToRun, Action<bool, string> onComplete)
	{
		SaveDataUtility.<>c__DisplayClass14_0 CS$<>8__locals1 = new SaveDataUtility.<>c__DisplayClass14_0();
		CS$<>8__locals1.onComplete = onComplete;
		CS$<>8__locals1.taskToRun = taskToRun;
		if (SaveDataUtility._runningSaveTask == null)
		{
			CS$<>8__locals1.<AddTaskToAsyncQueue>g__RunNewTask|0();
			return;
		}
		SaveDataUtility._saveTaskQueue.Enqueue(new Action(CS$<>8__locals1.<AddTaskToAsyncQueue>g__RunNewTask|0));
	}

	// Token: 0x06004429 RID: 17449 RVA: 0x0012B1F1 File Offset: 0x001293F1
	public static void AddTaskToAsyncQueue(Action task)
	{
		if (task == null)
		{
			return;
		}
		SaveDataUtility._saveWorkerThread.EnqueueWork(task);
	}

	// Token: 0x0400455C RID: 17756
	public const int MANUAL_REVISION_BREAK = 28104;

	// Token: 0x0400455D RID: 17757
	private static JsonSerializer _serializer;

	// Token: 0x0400455E RID: 17758
	private static readonly WorkerThread _saveWorkerThread = new WorkerThread(ThreadPriority.Normal);

	// Token: 0x0400455F RID: 17759
	private static readonly Queue<Action> _saveTaskQueue = new Queue<Action>();

	// Token: 0x04004560 RID: 17760
	private static Task _runningSaveTask;
}
