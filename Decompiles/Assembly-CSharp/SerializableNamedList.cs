using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x020001F6 RID: 502
[JsonObject(MemberSerialization.OptIn)]
[Serializable]
public abstract class SerializableNamedList<TData, TContainer> : ISerializationCallbackReceiver where TContainer : SerializableNamedData<TData>, new()
{
	// Token: 0x0600133D RID: 4925 RVA: 0x000582BA File Offset: 0x000564BA
	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		this.OnAfterDeserialize();
	}

	// Token: 0x0600133E RID: 4926 RVA: 0x000582C4 File Offset: 0x000564C4
	public void OnAfterDeserialize()
	{
		this.RuntimeData = (from named in this.savedData
		group named by named.Name).ToDictionary((IGrouping<string, TContainer> group) => group.Key, (IGrouping<string, TContainer> group) => group.FirstOrDefault<TContainer>().Data);
		if (this.RuntimeData.Count < this.savedData.Count && !this.RuntimeData.ContainsKey(string.Empty))
		{
			this.RuntimeData[string.Empty] = default(TData);
		}
	}

	// Token: 0x0600133F RID: 4927 RVA: 0x00058387 File Offset: 0x00056587
	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		this.OnBeforeSerialize();
	}

	// Token: 0x06001340 RID: 4928 RVA: 0x0005838F File Offset: 0x0005658F
	public void OnBeforeSerialize()
	{
		this.savedData = this.RuntimeData.Select(delegate(KeyValuePair<string, TData> kvp)
		{
			TContainer tcontainer = Activator.CreateInstance<TContainer>();
			tcontainer.Name = kvp.Key;
			tcontainer.Data = kvp.Value;
			return tcontainer;
		}).ToList<TContainer>();
	}

	// Token: 0x06001341 RID: 4929 RVA: 0x000583C8 File Offset: 0x000565C8
	public void SetData(string itemName, TData data)
	{
		ISerializableNamedDataRedundancy serializableNamedDataRedundancy = data as ISerializableNamedDataRedundancy;
		if (serializableNamedDataRedundancy != null && serializableNamedDataRedundancy.IsDataRedundant)
		{
			if (this.RuntimeData.ContainsKey(itemName))
			{
				this.RuntimeData.Remove(itemName);
				return;
			}
		}
		else
		{
			this.RuntimeData[itemName] = data;
		}
	}

	// Token: 0x06001342 RID: 4930 RVA: 0x00058415 File Offset: 0x00056615
	public TData GetData(string itemName)
	{
		return this.RuntimeData.GetValueOrDefault(itemName);
	}

	// Token: 0x06001343 RID: 4931 RVA: 0x00058424 File Offset: 0x00056624
	public List<string> GetValidNames(Func<TData, bool> predicate = null)
	{
		if (predicate == null)
		{
			return this.RuntimeData.Keys.ToList<string>();
		}
		return (from name in this.RuntimeData.Keys
		where predicate(this.GetData(name))
		select name).ToList<string>();
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x0005847F File Offset: 0x0005667F
	public List<TData> GetValidDatas(Func<TData, bool> predicate = null)
	{
		if (predicate == null)
		{
			return this.RuntimeData.Values.ToList<TData>();
		}
		return this.RuntimeData.Values.Where(predicate).ToList<TData>();
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x000584AC File Offset: 0x000566AC
	public bool IsAnyMatching(Func<TData, bool> predicate)
	{
		if (predicate == null)
		{
			return false;
		}
		foreach (TData arg in this.RuntimeData.Values)
		{
			if (predicate(arg))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x00058514 File Offset: 0x00056714
	public IEnumerable<KeyValuePair<string, TData>> Enumerate()
	{
		foreach (KeyValuePair<string, TData> keyValuePair in this.RuntimeData)
		{
			yield return keyValuePair;
		}
		Dictionary<string, TData>.Enumerator enumerator = default(Dictionary<string, TData>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x040011B4 RID: 4532
	[SerializeField]
	[JsonProperty]
	private List<TContainer> savedData = new List<TContainer>();

	// Token: 0x040011B5 RID: 4533
	[NonSerialized]
	protected Dictionary<string, TData> RuntimeData = new Dictionary<string, TData>();
}
