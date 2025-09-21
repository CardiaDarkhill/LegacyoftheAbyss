using System;
using UnityEngine;

// Token: 0x020007C6 RID: 1990
public class PersistentIntItem : PersistentItem<int>
{
	// Token: 0x170007E7 RID: 2023
	// (get) Token: 0x06004613 RID: 17939 RVA: 0x001309F9 File Offset: 0x0012EBF9
	protected override int DefaultValue
	{
		get
		{
			return -1;
		}
	}

	// Token: 0x170007E8 RID: 2024
	// (get) Token: 0x06004614 RID: 17940 RVA: 0x001309FC File Offset: 0x0012EBFC
	protected override PersistentItemData<int> SerializedItemData
	{
		get
		{
			return this.itemData;
		}
	}

	// Token: 0x06004615 RID: 17941 RVA: 0x00130A04 File Offset: 0x0012EC04
	protected override PlayMakerFSM LookForMyFSM()
	{
		PlayMakerFSM[] components = base.GetComponents<PlayMakerFSM>();
		if (components == null)
		{
			Debug.LogErrorFormat("Persistent Int Item ({0}) does not have a PlayMakerFSM attached to read value from.", new object[]
			{
				base.name
			});
			return null;
		}
		return FSMUtility.FindFSMWithPersistentInt(components);
	}

	// Token: 0x06004616 RID: 17942 RVA: 0x00130A3C File Offset: 0x0012EC3C
	protected override int GetValueFromFSM(PlayMakerFSM fromFsm)
	{
		return fromFsm.FsmVariables.FindFsmInt("Value").Value;
	}

	// Token: 0x06004617 RID: 17943 RVA: 0x00130A53 File Offset: 0x0012EC53
	protected override void SetValueOnFSM(PlayMakerFSM toFsm, int value)
	{
		toFsm.FsmVariables.FindFsmInt("Value").Value = value;
	}

	// Token: 0x06004618 RID: 17944 RVA: 0x00130A6B File Offset: 0x0012EC6B
	protected override void SaveValue(PersistentItemData<int> newItemData)
	{
		SceneData.instance.PersistentInts.SetValue(newItemData);
	}

	// Token: 0x06004619 RID: 17945 RVA: 0x00130A80 File Offset: 0x0012EC80
	protected override bool TryGetValue(ref PersistentItemData<int> newItemData)
	{
		PersistentItemData<int> persistentItemData;
		if (SceneData.instance.PersistentInts.TryGetValue(newItemData.SceneName, newItemData.ID, out persistentItemData))
		{
			newItemData.Value = persistentItemData.Value;
			return true;
		}
		return false;
	}

	// Token: 0x040046A5 RID: 18085
	[SerializeField]
	private PersistentIntItem.PersistentIntData itemData;

	// Token: 0x02001A94 RID: 6804
	[Serializable]
	private class PersistentIntData : PersistentItemData<int>
	{
	}
}
