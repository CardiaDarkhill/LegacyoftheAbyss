using System;
using UnityEngine;

// Token: 0x02000370 RID: 880
public class TriggerActivateGameObject : TrackTriggerObjects
{
	// Token: 0x06001E2C RID: 7724 RVA: 0x0008B4A4 File Offset: 0x000896A4
	protected override void Awake()
	{
		base.Awake();
		if (this.deactivateObjectOnLoad && this.gameObjectToSet)
		{
			this.gameObjectToSet.SetActive(this.invertActivation);
		}
		if (this.activateOncePersistent)
		{
			this.activateOncePersistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.hasActivated;
			};
			this.activateOncePersistent.OnSetSaveState += delegate(bool value)
			{
				this.hasActivated = value;
			};
		}
	}

	// Token: 0x06001E2D RID: 7725 RVA: 0x0008B518 File Offset: 0x00089718
	protected override void OnInsideStateChanged(bool isInside)
	{
		if (this.hasActivated && this.activateOncePersistent)
		{
			return;
		}
		if (!isInside)
		{
			return;
		}
		if (this.gameObjectToSet)
		{
			this.gameObjectToSet.SetActive(!this.invertActivation);
		}
		this.hasActivated = true;
	}

	// Token: 0x04001D44 RID: 7492
	[Space]
	[SerializeField]
	private bool deactivateObjectOnLoad;

	// Token: 0x04001D45 RID: 7493
	[SerializeField]
	private GameObject gameObjectToSet;

	// Token: 0x04001D46 RID: 7494
	[SerializeField]
	private PersistentBoolItem activateOncePersistent;

	// Token: 0x04001D47 RID: 7495
	[SerializeField]
	private bool invertActivation;

	// Token: 0x04001D48 RID: 7496
	private bool hasActivated;
}
