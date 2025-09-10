using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020007C5 RID: 1989
public class PersistentBoolItemResponder : MonoBehaviour
{
	// Token: 0x0600460E RID: 17934 RVA: 0x001308A5 File Offset: 0x0012EAA5
	private void Awake()
	{
		this.InvokeEvents(this.initialValue);
		if (this.persistent)
		{
			this.persistent.OnSetSaveState += this.InvokeEvents;
		}
	}

	// Token: 0x0600460F RID: 17935 RVA: 0x001308D8 File Offset: 0x0012EAD8
	private void Start()
	{
		this.started = true;
		if (!this.persistent)
		{
			return;
		}
		if (!this.persistent.HasLoadedValue && this.forcePersistentLoad)
		{
			this.persistent.LoadIfNeverStarted();
		}
		if (!this.persistent.HasLoadedValue && this.forcePersistentSave)
		{
			this.persistent.SaveState();
		}
		if (this.persistent.HasLoadedValue)
		{
			this.InvokeEvents(this.persistent.LoadedValue);
		}
	}

	// Token: 0x06004610 RID: 17936 RVA: 0x00130958 File Offset: 0x0012EB58
	private void OnEnable()
	{
		if (!this.started)
		{
			return;
		}
		if (!this.persistent)
		{
			return;
		}
		if (!this.persistent.HasLoadedValue && this.forcePersistentSave)
		{
			this.persistent.SaveState();
		}
		if (this.persistent.HasLoadedValue)
		{
			this.InvokeEvents(this.persistent.LoadedValue);
		}
	}

	// Token: 0x06004611 RID: 17937 RVA: 0x001309BA File Offset: 0x0012EBBA
	private void InvokeEvents(bool value)
	{
		this.OnGetValue.Invoke(value);
		this.OnGetValueInverse.Invoke(!value);
		if (value)
		{
			this.OnGetValueTrue.Invoke();
			return;
		}
		this.OnGetValueFalse.Invoke();
	}

	// Token: 0x0400469C RID: 18076
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x0400469D RID: 18077
	[SerializeField]
	private bool initialValue;

	// Token: 0x0400469E RID: 18078
	[SerializeField]
	[Tooltip("If persistent hasn't loaded a save state then get it to save one first.")]
	private bool forcePersistentSave;

	// Token: 0x0400469F RID: 18079
	[SerializeField]
	[Tooltip("If persistent hasn't loaded a save state then get it to try load.")]
	private bool forcePersistentLoad;

	// Token: 0x040046A0 RID: 18080
	[Space]
	public PersistentBoolItemResponder.UnityBoolEvent OnGetValue;

	// Token: 0x040046A1 RID: 18081
	public PersistentBoolItemResponder.UnityBoolEvent OnGetValueInverse;

	// Token: 0x040046A2 RID: 18082
	public UnityEvent OnGetValueTrue;

	// Token: 0x040046A3 RID: 18083
	public UnityEvent OnGetValueFalse;

	// Token: 0x040046A4 RID: 18084
	private bool started;

	// Token: 0x02001A93 RID: 6803
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}
}
