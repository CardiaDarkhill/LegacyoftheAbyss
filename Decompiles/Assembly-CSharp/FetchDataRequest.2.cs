using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000465 RID: 1125
public sealed class FetchDataRequest<T> where T : new()
{
	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x06002860 RID: 10336 RVA: 0x000B249A File Offset: 0x000B069A
	public FetchDataRequest.Status State
	{
		get
		{
			if (this.dataSource.State == FetchDataRequest.Status.Completed && !this.isComplete)
			{
				return FetchDataRequest.Status.InProgress;
			}
			return this.dataSource.State;
		}
	}

	// Token: 0x06002861 RID: 10337 RVA: 0x000B24C0 File Offset: 0x000B06C0
	public FetchDataRequest(FetchDataRequest dataSource)
	{
		FetchDataRequest<T> <>4__this = this;
		this.dataSource = dataSource;
		if (dataSource != null)
		{
			this.results = new List<FetchDataRequest<T>.FetchResult>();
			dataSource.RunOnFetchComplete(delegate(FetchDataRequest fetchResult)
			{
				try
				{
					if (dataSource.RestorePoints != null)
					{
						foreach (RestorePointFileWrapper restorePointFileWrapper in fetchResult.RestorePoints)
						{
							if (restorePointFileWrapper == null)
							{
								Debug.LogError(dataSource.Name + " failed to load restore point");
							}
							else if (restorePointFileWrapper.data == null)
							{
								Debug.LogError(dataSource.Name + " is missing data");
							}
							else
							{
								FetchDataRequest<T>.FetchResult fetchResult2 = new FetchDataRequest<T>.FetchResult(restorePointFileWrapper);
								if (fetchResult2.loadedObject != null)
								{
									<>4__this.results.Add(fetchResult2);
								}
							}
						}
					}
				}
				finally
				{
					<>4__this.isComplete = true;
				}
			});
			return;
		}
		Debug.LogError("Data source is null");
	}

	// Token: 0x04002467 RID: 9319
	public readonly FetchDataRequest dataSource;

	// Token: 0x04002468 RID: 9320
	public readonly List<FetchDataRequest<T>.FetchResult> results;

	// Token: 0x04002469 RID: 9321
	private bool isComplete;

	// Token: 0x0200177D RID: 6013
	public sealed class FetchResult
	{
		// Token: 0x06008DC3 RID: 36291 RVA: 0x0028A2F8 File Offset: 0x002884F8
		public FetchResult(RestorePointFileWrapper sourceData)
		{
			this.sourceData = sourceData;
			try
			{
				string jsonForSaveBytesStatic = GameManager.GetJsonForSaveBytesStatic(sourceData.data);
				if (string.IsNullOrEmpty(jsonForSaveBytesStatic))
				{
					Debug.LogError("Failed to load json from bytes.");
				}
				else
				{
					this.loadedObject = SaveDataUtility.DeserializeSaveData<T>(jsonForSaveBytesStatic);
					if (this.loadedObject == null)
					{
						Debug.LogError(string.Concat(new string[]
						{
							"Failed to load ",
							typeof(T).Name,
							" from ",
							jsonForSaveBytesStatic,
							"."
						}));
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		// Token: 0x04008E4B RID: 36427
		public readonly RestorePointFileWrapper sourceData;

		// Token: 0x04008E4C RID: 36428
		public T loadedObject;
	}
}
