using System;
using UnityEngine;

// Token: 0x020007C4 RID: 1988
public class PersistentBoolItem : PersistentItem<bool>
{
	// Token: 0x170007E5 RID: 2021
	// (get) Token: 0x06004602 RID: 17922 RVA: 0x00130766 File Offset: 0x0012E966
	protected override bool DefaultValue
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170007E6 RID: 2022
	// (get) Token: 0x06004603 RID: 17923 RVA: 0x00130769 File Offset: 0x0012E969
	protected override PersistentItemData<bool> SerializedItemData
	{
		get
		{
			return this.itemData;
		}
	}

	// Token: 0x06004604 RID: 17924 RVA: 0x00130774 File Offset: 0x0012E974
	protected override void Awake()
	{
		if (this.itemData == null)
		{
			this.itemData = new PersistentBoolItem.PersistentBoolData();
		}
		base.Awake();
		if (this.disablePrefabIfActivated)
		{
			base.OnSetSaveState += delegate(bool value)
			{
				if (value)
				{
					this.disablePrefabIfActivated.SetActive(false);
				}
			};
		}
		if (this.disableIfActivated)
		{
			base.OnSetSaveState += delegate(bool value)
			{
				if (value)
				{
					base.gameObject.SetActive(false);
				}
			};
		}
	}

	// Token: 0x06004605 RID: 17925 RVA: 0x001307D4 File Offset: 0x0012E9D4
	protected override PlayMakerFSM LookForMyFSM()
	{
		PlayMakerFSM[] components = base.GetComponents<PlayMakerFSM>();
		if (components == null)
		{
			return null;
		}
		return FSMUtility.FindFSMWithPersistentBool(components);
	}

	// Token: 0x06004606 RID: 17926 RVA: 0x001307F3 File Offset: 0x0012E9F3
	protected override bool GetValueFromFSM(PlayMakerFSM fromFsm)
	{
		return fromFsm.FsmVariables.FindFsmBool("Activated").Value;
	}

	// Token: 0x06004607 RID: 17927 RVA: 0x0013080A File Offset: 0x0012EA0A
	protected override void SetValueOnFSM(PlayMakerFSM toFsm, bool value)
	{
		toFsm.FsmVariables.FindFsmBool("Activated").Value = value;
	}

	// Token: 0x06004608 RID: 17928 RVA: 0x00130822 File Offset: 0x0012EA22
	protected override void SaveValue(PersistentItemData<bool> newItemData)
	{
		SceneData.instance.PersistentBools.SetValue(newItemData);
	}

	// Token: 0x06004609 RID: 17929 RVA: 0x00130834 File Offset: 0x0012EA34
	protected override bool TryGetValue(ref PersistentItemData<bool> newItemData)
	{
		PersistentItemData<bool> persistentItemData;
		if (SceneData.instance.PersistentBools.TryGetValue(newItemData.SceneName, newItemData.ID, out persistentItemData))
		{
			newItemData.Value = persistentItemData.Value;
			return true;
		}
		return false;
	}

	// Token: 0x0600460A RID: 17930 RVA: 0x00130872 File Offset: 0x0012EA72
	public new void SetValueOverride(bool value)
	{
		base.SetValueOverride(value);
	}

	// Token: 0x04004699 RID: 18073
	[SerializeField]
	private PersistentBoolItem.PersistentBoolData itemData;

	// Token: 0x0400469A RID: 18074
	[Space]
	[SerializeField]
	private bool disableIfActivated;

	// Token: 0x0400469B RID: 18075
	[SerializeField]
	private GameObject disablePrefabIfActivated;

	// Token: 0x02001A92 RID: 6802
	[Serializable]
	private class PersistentBoolData : PersistentItemData<bool>
	{
	}
}
