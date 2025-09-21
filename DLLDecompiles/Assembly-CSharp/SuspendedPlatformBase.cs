using System;
using UnityEngine;

// Token: 0x02000569 RID: 1385
public abstract class SuspendedPlatformBase : MonoBehaviour
{
	// Token: 0x06003186 RID: 12678 RVA: 0x000DBEF9 File Offset: 0x000DA0F9
	protected virtual void Awake()
	{
		if (this.persistent)
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
					this.OnStartActivated();
				}
			};
		}
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x000DBF36 File Offset: 0x000DA136
	protected virtual void OnStartActivated()
	{
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x000DBF38 File Offset: 0x000DA138
	public virtual void CutDown()
	{
		this.activated = true;
	}

	// Token: 0x040034E0 RID: 13536
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x040034E1 RID: 13537
	protected bool activated;
}
