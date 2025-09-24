using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x020007CD RID: 1997
[JsonObject(MemberSerialization.Fields)]
[Serializable]
public class SceneData
{
	// Token: 0x170007F2 RID: 2034
	// (get) Token: 0x0600465E RID: 18014 RVA: 0x001314E5 File Offset: 0x0012F6E5
	public SceneData.PersistentItemDataCollection<bool, SceneData.SerializableBoolData> PersistentBools
	{
		get
		{
			return this.persistentBools;
		}
	}

	// Token: 0x170007F3 RID: 2035
	// (get) Token: 0x0600465F RID: 18015 RVA: 0x001314ED File Offset: 0x0012F6ED
	public SceneData.PersistentItemDataCollection<int, SceneData.SerializableIntData> PersistentInts
	{
		get
		{
			return this.persistentInts;
		}
	}

	// Token: 0x170007F4 RID: 2036
	// (get) Token: 0x06004660 RID: 18016 RVA: 0x001314F5 File Offset: 0x0012F6F5
	// (set) Token: 0x06004661 RID: 18017 RVA: 0x0013150D File Offset: 0x0012F70D
	public static SceneData instance
	{
		get
		{
			if (SceneData._instance == null)
			{
				SceneData._instance = new SceneData();
			}
			return SceneData._instance;
		}
		set
		{
			SceneData._instance = value;
		}
	}

	// Token: 0x06004662 RID: 18018 RVA: 0x00131515 File Offset: 0x0012F715
	public SceneData()
	{
		this.SetupNewSceneData();
	}

	// Token: 0x06004663 RID: 18019 RVA: 0x00131523 File Offset: 0x0012F723
	public void Reset()
	{
		this.SetupNewSceneData();
	}

	// Token: 0x06004664 RID: 18020 RVA: 0x0013152B File Offset: 0x0012F72B
	public void SaveMyState(GeoRockData geoRockData)
	{
		this.geoRocks.SetValue(new PersistentItemData<int>
		{
			SceneName = geoRockData.sceneName,
			ID = geoRockData.id,
			Value = geoRockData.hitsLeft
		});
	}

	// Token: 0x06004665 RID: 18021 RVA: 0x00131564 File Offset: 0x0012F764
	public GeoRockData FindMyState(GeoRockData grd)
	{
		PersistentItemData<int> persistentItemData;
		if (this.geoRocks.TryGetValue(grd.sceneName, grd.id, out persistentItemData))
		{
			return new GeoRockData
			{
				sceneName = persistentItemData.SceneName,
				id = persistentItemData.ID,
				hitsLeft = persistentItemData.Value
			};
		}
		return null;
	}

	// Token: 0x06004666 RID: 18022 RVA: 0x001315B7 File Offset: 0x0012F7B7
	public void ResetSemiPersistentItems()
	{
		this.persistentBools.ResetSemiPersistent();
		this.persistentInts.ResetSemiPersistent();
		this.geoRocks.ResetSemiPersistent();
	}

	// Token: 0x06004667 RID: 18023 RVA: 0x001315DA File Offset: 0x0012F7DA
	private void SetupNewSceneData()
	{
		this.persistentBools = new SceneData.PersistentBoolCollection();
		this.persistentInts = new SceneData.PersistentIntCollection();
		this.geoRocks = new SceneData.PersistentIntCollection();
	}

	// Token: 0x06004668 RID: 18024 RVA: 0x001315FD File Offset: 0x0012F7FD
	public void MimicShuffle()
	{
		this.persistentInts.Mutate(delegate(PersistentItemData<int> data)
		{
			if (data.Mutator != SceneData.PersistentMutatorTypes.Mimic)
			{
				return;
			}
			if (data.Value != 0)
			{
				return;
			}
			if (Random.Range(0, 50) == 0)
			{
				data.Value = -10;
			}
		});
	}

	// Token: 0x040046CE RID: 18126
	[SerializeField]
	private SceneData.PersistentBoolCollection persistentBools;

	// Token: 0x040046CF RID: 18127
	[SerializeField]
	private SceneData.PersistentIntCollection persistentInts;

	// Token: 0x040046D0 RID: 18128
	[SerializeField]
	private SceneData.PersistentIntCollection geoRocks;

	// Token: 0x040046D1 RID: 18129
	private static SceneData _instance;

	// Token: 0x02001A99 RID: 6809
	public enum PersistentMutatorTypes
	{
		// Token: 0x040099FC RID: 39420
		None,
		// Token: 0x040099FD RID: 39421
		Mimic
	}

	// Token: 0x02001A9A RID: 6810
	[Serializable]
	public class SerializableItemData<T>
	{
		// Token: 0x040099FE RID: 39422
		public string SceneName;

		// Token: 0x040099FF RID: 39423
		public string ID;

		// Token: 0x04009A00 RID: 39424
		public T Value;

		// Token: 0x04009A01 RID: 39425
		public SceneData.PersistentMutatorTypes Mutator;
	}

	// Token: 0x02001A9B RID: 6811
	[JsonObject(MemberSerialization.Fields)]
	[Serializable]
	public class PersistentItemDataCollection<TValue, TContainer> : ISerializationCallbackReceiver where TContainer : SceneData.SerializableItemData<TValue>, new()
	{
		// Token: 0x06009775 RID: 38773 RVA: 0x002AA4E1 File Offset: 0x002A86E1
		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			this.OnAfterDeserialize();
		}

		// Token: 0x06009776 RID: 38774 RVA: 0x002AA4EC File Offset: 0x002A86EC
		public void OnAfterDeserialize()
		{
			if (this.scenes == null)
			{
				this.scenes = new Dictionary<string, Dictionary<string, PersistentItemData<TValue>>>();
			}
			else
			{
				Dictionary<string, Dictionary<string, PersistentItemData<TValue>>> dictionary = new Dictionary<string, Dictionary<string, PersistentItemData<TValue>>>();
				foreach (KeyValuePair<string, Dictionary<string, PersistentItemData<TValue>>> keyValuePair in this.scenes)
				{
					foreach (KeyValuePair<string, PersistentItemData<TValue>> keyValuePair2 in keyValuePair.Value)
					{
						if (keyValuePair2.Value.IsSemiPersistent)
						{
							if (!dictionary.ContainsKey(keyValuePair.Key))
							{
								dictionary[keyValuePair.Key] = new Dictionary<string, PersistentItemData<TValue>>();
							}
							dictionary[keyValuePair.Key][keyValuePair2.Key] = keyValuePair2.Value;
						}
					}
				}
				this.scenes = dictionary;
			}
			foreach (TContainer tcontainer in this.serializedList)
			{
				if (!this.scenes.ContainsKey(tcontainer.SceneName))
				{
					this.scenes[tcontainer.SceneName] = new Dictionary<string, PersistentItemData<TValue>>();
				}
				this.scenes[tcontainer.SceneName][tcontainer.ID] = new PersistentItemData<TValue>
				{
					SceneName = tcontainer.SceneName,
					ID = tcontainer.ID,
					Value = tcontainer.Value,
					Mutator = tcontainer.Mutator
				};
			}
		}

		// Token: 0x06009777 RID: 38775 RVA: 0x002AA6DC File Offset: 0x002A88DC
		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			this.OnBeforeSerialize();
		}

		// Token: 0x06009778 RID: 38776 RVA: 0x002AA6E4 File Offset: 0x002A88E4
		public void OnBeforeSerialize()
		{
			this.serializedList = new List<TContainer>();
			foreach (KeyValuePair<string, Dictionary<string, PersistentItemData<TValue>>> keyValuePair in this.scenes)
			{
				foreach (KeyValuePair<string, PersistentItemData<TValue>> keyValuePair2 in keyValuePair.Value)
				{
					if (!keyValuePair2.Value.IsSemiPersistent)
					{
						List<TContainer> list = this.serializedList;
						TContainer tcontainer = Activator.CreateInstance<TContainer>();
						tcontainer.SceneName = keyValuePair2.Value.SceneName;
						tcontainer.ID = keyValuePair2.Value.ID;
						tcontainer.Value = keyValuePair2.Value.Value;
						tcontainer.Mutator = keyValuePair2.Value.Mutator;
						list.Add(tcontainer);
					}
				}
			}
		}

		// Token: 0x06009779 RID: 38777 RVA: 0x002AA800 File Offset: 0x002A8A00
		public void SetValue(PersistentItemData<TValue> itemData)
		{
			if (!this.scenes.ContainsKey(itemData.SceneName))
			{
				this.scenes[itemData.SceneName] = new Dictionary<string, PersistentItemData<TValue>>();
			}
			this.scenes[itemData.SceneName][itemData.ID] = itemData;
		}

		// Token: 0x0600977A RID: 38778 RVA: 0x002AA853 File Offset: 0x002A8A53
		public bool TryGetValue(string sceneName, string id, out PersistentItemData<TValue> value)
		{
			if (this.scenes.ContainsKey(sceneName) && this.scenes[sceneName].ContainsKey(id))
			{
				value = this.scenes[sceneName][id];
				return true;
			}
			value = null;
			return false;
		}

		// Token: 0x0600977B RID: 38779 RVA: 0x002AA894 File Offset: 0x002A8A94
		public TValue GetValueOrDefault(string sceneName, string id)
		{
			PersistentItemData<TValue> persistentItemData;
			if (!this.TryGetValue(sceneName, id, out persistentItemData))
			{
				return default(TValue);
			}
			return persistentItemData.Value;
		}

		// Token: 0x0600977C RID: 38780 RVA: 0x002AA8C0 File Offset: 0x002A8AC0
		public void ResetSemiPersistent()
		{
			Dictionary<string, Dictionary<string, PersistentItemData<TValue>>> dictionary = new Dictionary<string, Dictionary<string, PersistentItemData<TValue>>>();
			foreach (KeyValuePair<string, Dictionary<string, PersistentItemData<TValue>>> keyValuePair in this.scenes)
			{
				Dictionary<string, PersistentItemData<TValue>> dictionary2 = new Dictionary<string, PersistentItemData<TValue>>();
				foreach (KeyValuePair<string, PersistentItemData<TValue>> keyValuePair2 in keyValuePair.Value)
				{
					if (!keyValuePair2.Value.IsSemiPersistent)
					{
						dictionary2.Add(keyValuePair2.Key, keyValuePair2.Value);
					}
				}
				dictionary.Add(keyValuePair.Key, dictionary2);
			}
			this.scenes = dictionary;
		}

		// Token: 0x0600977D RID: 38781 RVA: 0x002AA990 File Offset: 0x002A8B90
		public void Mutate(Action<PersistentItemData<TValue>> mutateAction)
		{
			if (mutateAction == null)
			{
				return;
			}
			foreach (KeyValuePair<string, Dictionary<string, PersistentItemData<TValue>>> keyValuePair in this.scenes)
			{
				foreach (KeyValuePair<string, PersistentItemData<TValue>> keyValuePair2 in keyValuePair.Value)
				{
					mutateAction(keyValuePair2.Value);
				}
			}
		}

		// Token: 0x0600977E RID: 38782 RVA: 0x002AAA2C File Offset: 0x002A8C2C
		public bool Remove(string sceneName, string id)
		{
			Dictionary<string, PersistentItemData<TValue>> dictionary;
			return this.scenes.TryGetValue(sceneName, out dictionary) && dictionary.Remove(id);
		}

		// Token: 0x04009A02 RID: 39426
		[SerializeField]
		private List<TContainer> serializedList = new List<TContainer>();

		// Token: 0x04009A03 RID: 39427
		[NonSerialized]
		private Dictionary<string, Dictionary<string, PersistentItemData<TValue>>> scenes = new Dictionary<string, Dictionary<string, PersistentItemData<TValue>>>();
	}

	// Token: 0x02001A9C RID: 6812
	[Serializable]
	public class SerializableBoolData : SceneData.SerializableItemData<bool>
	{
	}

	// Token: 0x02001A9D RID: 6813
	[Serializable]
	private class PersistentBoolCollection : SceneData.PersistentItemDataCollection<bool, SceneData.SerializableBoolData>
	{
	}

	// Token: 0x02001A9E RID: 6814
	[Serializable]
	public class SerializableIntData : SceneData.SerializableItemData<int>
	{
	}

	// Token: 0x02001A9F RID: 6815
	[Serializable]
	private class PersistentIntCollection : SceneData.PersistentItemDataCollection<int, SceneData.SerializableIntData>
	{
	}
}
