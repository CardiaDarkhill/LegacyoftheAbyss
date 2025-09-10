using System;
using UnityEngine;

// Token: 0x020007CA RID: 1994
public abstract class PersistentItem<T> : MonoBehaviour, IPersistentItem where T : IEquatable<T>
{
	// Token: 0x140000EE RID: 238
	// (add) Token: 0x06004623 RID: 17955 RVA: 0x00130B2C File Offset: 0x0012ED2C
	// (remove) Token: 0x06004624 RID: 17956 RVA: 0x00130B64 File Offset: 0x0012ED64
	public event PersistentItem<T>.SetValueEvent OnSetSaveState;

	// Token: 0x140000EF RID: 239
	// (add) Token: 0x06004625 RID: 17957 RVA: 0x00130B9C File Offset: 0x0012ED9C
	// (remove) Token: 0x06004626 RID: 17958 RVA: 0x00130BD4 File Offset: 0x0012EDD4
	public event PersistentItem<T>.GetValueEvent OnGetSaveState;

	// Token: 0x140000F0 RID: 240
	// (add) Token: 0x06004627 RID: 17959 RVA: 0x00130C0C File Offset: 0x0012EE0C
	// (remove) Token: 0x06004628 RID: 17960 RVA: 0x00130C44 File Offset: 0x0012EE44
	public event Action SemiPersistentReset;

	// Token: 0x170007E9 RID: 2025
	// (get) Token: 0x06004629 RID: 17961 RVA: 0x00130C79 File Offset: 0x0012EE79
	public PersistentItemData<T> ItemData
	{
		get
		{
			this.EnsureSetup();
			return this.itemData;
		}
	}

	// Token: 0x170007EA RID: 2026
	// (get) Token: 0x0600462A RID: 17962
	protected abstract T DefaultValue { get; }

	// Token: 0x170007EB RID: 2027
	// (get) Token: 0x0600462B RID: 17963
	protected abstract PersistentItemData<T> SerializedItemData { get; }

	// Token: 0x170007EC RID: 2028
	// (get) Token: 0x0600462C RID: 17964 RVA: 0x00130C87 File Offset: 0x0012EE87
	// (set) Token: 0x0600462D RID: 17965 RVA: 0x00130C8F File Offset: 0x0012EE8F
	public bool HasLoadedValue { get; private set; }

	// Token: 0x170007ED RID: 2029
	// (get) Token: 0x0600462E RID: 17966 RVA: 0x00130C98 File Offset: 0x0012EE98
	// (set) Token: 0x0600462F RID: 17967 RVA: 0x00130CA0 File Offset: 0x0012EEA0
	public T LoadedValue { get; private set; }

	// Token: 0x06004630 RID: 17968 RVA: 0x00130CA9 File Offset: 0x0012EEA9
	protected virtual void Awake()
	{
		if (this.saveCondition == null)
		{
			this.saveCondition = new PlayerDataTest();
		}
		this.gm = GameManager.instance;
		this.gm.SavePersistentObjects += this.SaveState;
	}

	// Token: 0x06004631 RID: 17969 RVA: 0x00130CE0 File Offset: 0x0012EEE0
	private void OnEnable()
	{
		if (this.SerializedItemData.IsSemiPersistent)
		{
			this.gm.ResetSemiPersistentObjects += this.ResetState;
		}
		if (this.OnGetSaveState == null)
		{
			this.fsm = this.LookForMyFSM();
		}
	}

	// Token: 0x06004632 RID: 17970 RVA: 0x00130D1A File Offset: 0x0012EF1A
	private void OnDisable()
	{
		if (this.gm != null)
		{
			this.gm.ResetSemiPersistentObjects -= this.ResetState;
		}
	}

	// Token: 0x06004633 RID: 17971 RVA: 0x00130D41 File Offset: 0x0012EF41
	private void OnDestroy()
	{
		if (this.gm != null)
		{
			this.gm.SavePersistentObjects -= this.SaveState;
		}
	}

	// Token: 0x06004634 RID: 17972 RVA: 0x00130D68 File Offset: 0x0012EF68
	private void Start()
	{
		this.started = true;
		this.EnsureSetup();
		this.CheckIsValid();
		if (this.TryGetValue(ref this.itemData))
		{
			if (this.OnSetSaveState != null)
			{
				this.OnSetSaveState(this.itemData.Value);
			}
			if (this.fsm == null)
			{
				this.fsm = this.LookForMyFSM();
			}
			if (this.fsm != null)
			{
				this.SetValueOnFSM(this.fsm, this.itemData.Value);
			}
			this.HasLoadedValue = true;
			this.LoadedValue = this.itemData.Value;
			return;
		}
		this.UpdateValue();
	}

	// Token: 0x06004635 RID: 17973 RVA: 0x00130E12 File Offset: 0x0012F012
	public void LoadIfNeverStarted()
	{
		if (this.started)
		{
			return;
		}
		this.PreSetup();
	}

	// Token: 0x06004636 RID: 17974 RVA: 0x00130E23 File Offset: 0x0012F023
	private void UpdateValue()
	{
		if (this.isValueOverridden)
		{
			return;
		}
		if (this.OnGetSaveState != null)
		{
			this.OnGetSaveState(out this.ItemData.Value);
			return;
		}
		this.UpdateActivatedFromFSM();
	}

	// Token: 0x06004637 RID: 17975 RVA: 0x00130E53 File Offset: 0x0012F053
	public void SetValueOverride(T value)
	{
		this.ItemData.Value = value;
		this.isValueOverridden = true;
	}

	// Token: 0x06004638 RID: 17976 RVA: 0x00130E68 File Offset: 0x0012F068
	public T GetCurrentValue()
	{
		this.UpdateValue();
		return this.ItemData.Value;
	}

	// Token: 0x06004639 RID: 17977 RVA: 0x00130E7C File Offset: 0x0012F07C
	private void CheckIsValid()
	{
		Type type = base.GetType();
		if (base.GetComponents(type).Length > 1)
		{
			Debug.LogError(string.Format("There is more than one component of type: <b>{0}</b> on <b>{1}</b>, please remove one!", type, base.gameObject.name), this);
		}
	}

	// Token: 0x0600463A RID: 17978 RVA: 0x00130EB8 File Offset: 0x0012F0B8
	public void SaveState()
	{
		if (this.saveCondition.IsDefined && !this.saveCondition.IsFulfilled)
		{
			return;
		}
		this.SaveStateNoCondition();
	}

	// Token: 0x0600463B RID: 17979 RVA: 0x00130EDC File Offset: 0x0012F0DC
	public void SaveStateNoCondition()
	{
		this.EnsureSetup();
		if (!this.isValueOverridden)
		{
			if (this.OnGetSaveState != null)
			{
				this.OnGetSaveState(out this.itemData.Value);
			}
			else
			{
				this.UpdateActivatedFromFSM();
			}
		}
		this.HasLoadedValue = true;
		this.LoadedValue = this.itemData.Value;
		if (this.dontSave)
		{
			return;
		}
		this.SaveValue(this.itemData);
	}

	// Token: 0x0600463C RID: 17980 RVA: 0x00130F4C File Offset: 0x0012F14C
	private void ResetState()
	{
		if (!this.itemData.IsSemiPersistent)
		{
			return;
		}
		this.SaveState();
		if (this.itemData.Value.Equals(this.DefaultValue))
		{
			return;
		}
		this.itemData.Value = this.DefaultValue;
		Action semiPersistentReset = this.SemiPersistentReset;
		if (semiPersistentReset != null)
		{
			semiPersistentReset();
		}
		if (this.fsm != null)
		{
			this.fsm.SendEvent("RESET");
		}
	}

	// Token: 0x0600463D RID: 17981 RVA: 0x00130FCC File Offset: 0x0012F1CC
	private void EnsureSetup()
	{
		if (this.hasSetup)
		{
			return;
		}
		this.hasSetup = true;
		this.itemData = this.SerializedItemData;
		if (string.IsNullOrEmpty(this.itemData.ID))
		{
			this.itemData.ID = base.name;
		}
		if (string.IsNullOrEmpty(this.itemData.SceneName))
		{
			this.itemData.SceneName = GameManager.GetBaseSceneName(base.gameObject.scene.name);
		}
	}

	// Token: 0x0600463E RID: 17982 RVA: 0x0013104D File Offset: 0x0012F24D
	public void PreSetup()
	{
		this.Start();
	}

	// Token: 0x0600463F RID: 17983 RVA: 0x00131055 File Offset: 0x0012F255
	private void UpdateActivatedFromFSM()
	{
		if (this.fsm != null)
		{
			this.itemData.Value = this.GetValueFromFSM(this.fsm);
			return;
		}
		this.fsm = this.LookForMyFSM();
	}

	// Token: 0x06004640 RID: 17984 RVA: 0x00131089 File Offset: 0x0012F289
	protected virtual PlayMakerFSM LookForMyFSM()
	{
		return null;
	}

	// Token: 0x06004641 RID: 17985 RVA: 0x0013108C File Offset: 0x0012F28C
	protected virtual T GetValueFromFSM(PlayMakerFSM fromFsm)
	{
		return this.DefaultValue;
	}

	// Token: 0x06004642 RID: 17986 RVA: 0x00131094 File Offset: 0x0012F294
	protected virtual void SetValueOnFSM(PlayMakerFSM toFsm, T value)
	{
	}

	// Token: 0x06004643 RID: 17987
	protected abstract void SaveValue(PersistentItemData<T> newItemData);

	// Token: 0x06004644 RID: 17988
	protected abstract bool TryGetValue(ref PersistentItemData<T> newItemData);

	// Token: 0x06004645 RID: 17989 RVA: 0x00131096 File Offset: 0x0012F296
	public string GetId()
	{
		return this.SerializedItemData.ID;
	}

	// Token: 0x06004646 RID: 17990 RVA: 0x001310A3 File Offset: 0x0012F2A3
	public string GetSceneName()
	{
		return this.SerializedItemData.SceneName;
	}

	// Token: 0x06004647 RID: 17991 RVA: 0x001310B0 File Offset: 0x0012F2B0
	public string GetValueTypeName()
	{
		return typeof(T).ToString();
	}

	// Token: 0x06004648 RID: 17992 RVA: 0x001310C1 File Offset: 0x0012F2C1
	public bool GetIsSemiPersistent()
	{
		return this.SerializedItemData.IsSemiPersistent;
	}

	// Token: 0x040046B3 RID: 18099
	[SerializeField]
	private PlayerDataTest saveCondition;

	// Token: 0x040046B4 RID: 18100
	[SerializeField]
	[Tooltip("If object is only intended for reading, not saving.")]
	private bool dontSave;

	// Token: 0x040046B5 RID: 18101
	private bool hasSetup;

	// Token: 0x040046B6 RID: 18102
	private bool isValueOverridden;

	// Token: 0x040046B7 RID: 18103
	private GameManager gm;

	// Token: 0x040046B8 RID: 18104
	private PlayMakerFSM fsm;

	// Token: 0x040046B9 RID: 18105
	private PersistentItemData<T> itemData;

	// Token: 0x040046BC RID: 18108
	private bool started;

	// Token: 0x02001A95 RID: 6805
	// (Invoke) Token: 0x06009760 RID: 38752
	public delegate void SetValueEvent(T value);

	// Token: 0x02001A96 RID: 6806
	// (Invoke) Token: 0x06009764 RID: 38756
	public delegate void GetValueEvent(out T value);
}
