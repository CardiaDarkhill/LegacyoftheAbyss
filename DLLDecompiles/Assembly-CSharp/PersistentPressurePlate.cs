using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200052B RID: 1323
public class PersistentPressurePlate : PressurePlateBase
{
	// Token: 0x17000544 RID: 1348
	// (get) Token: 0x06002F87 RID: 12167 RVA: 0x000D147D File Offset: 0x000CF67D
	protected override bool CanDepress
	{
		get
		{
			return !this.activated;
		}
	}

	// Token: 0x06002F88 RID: 12168 RVA: 0x000D1488 File Offset: 0x000CF688
	private void Start()
	{
		if (!string.IsNullOrEmpty(this.playerDataBool))
		{
			this.activated = PlayerData.instance.GetVariable(this.playerDataBool);
			if (this.activated)
			{
				this.StartActivated();
				return;
			}
		}
		else if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.activated;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.activated = value;
				if (this.activated)
				{
					this.StartActivated();
				}
			};
		}
	}

	// Token: 0x06002F89 RID: 12169 RVA: 0x000D1504 File Offset: 0x000CF704
	private void StartActivated()
	{
		base.SetDepressed();
		foreach (UnlockablePropBase unlockablePropBase in this.connectedGates)
		{
			if (unlockablePropBase)
			{
				unlockablePropBase.Opened();
			}
		}
		foreach (PlayMakerFSM playMakerFSM in this.fsmGates)
		{
			if (playMakerFSM)
			{
				playMakerFSM.SendEvent("ACTIVATE");
			}
		}
		this.OnActivated.Invoke();
	}

	// Token: 0x06002F8A RID: 12170 RVA: 0x000D1578 File Offset: 0x000CF778
	protected override void Activate()
	{
		this.activated = true;
		if (this.persistent)
		{
			this.persistent.SaveState();
		}
		foreach (UnlockablePropBase unlockablePropBase in this.connectedGates)
		{
			if (unlockablePropBase)
			{
				unlockablePropBase.Open();
			}
		}
		foreach (PlayMakerFSM playMakerFSM in this.fsmGates)
		{
			if (playMakerFSM)
			{
				playMakerFSM.SendEvent("OPEN");
			}
		}
		if (!string.IsNullOrEmpty(this.playerDataBool))
		{
			PlayerData.instance.SetVariable(this.playerDataBool, true);
		}
		this.OnActivate.Invoke();
	}

	// Token: 0x04003248 RID: 12872
	[Space]
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04003249 RID: 12873
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	// Token: 0x0400324A RID: 12874
	[Space]
	[SerializeField]
	private UnlockablePropBase[] connectedGates;

	// Token: 0x0400324B RID: 12875
	public PlayMakerFSM[] fsmGates;

	// Token: 0x0400324C RID: 12876
	[Space]
	public UnityEvent OnActivate;

	// Token: 0x0400324D RID: 12877
	public UnityEvent OnActivated;

	// Token: 0x0400324E RID: 12878
	private bool activated;
}
