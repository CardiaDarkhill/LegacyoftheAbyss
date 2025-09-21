using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200052A RID: 1322
public class PersistentActivator : MonoBehaviour
{
	// Token: 0x06002F80 RID: 12160 RVA: 0x000D13B3 File Offset: 0x000CF5B3
	private void Awake()
	{
		this.persistent.OnGetSaveState += delegate(out bool value)
		{
			value = this.isActivated;
		};
		this.persistent.OnSetSaveState += delegate(bool value)
		{
			this.isActivated = value;
			if (this.isActivated)
			{
				this.Activated();
			}
		};
	}

	// Token: 0x06002F81 RID: 12161 RVA: 0x000D13E3 File Offset: 0x000CF5E3
	public void Activate()
	{
		if (this.isActivated)
		{
			return;
		}
		this.isActivated = true;
		this.OnActivate.Invoke();
		base.StartCoroutine(this.UnlockDelayed());
	}

	// Token: 0x06002F82 RID: 12162 RVA: 0x000D1410 File Offset: 0x000CF610
	private void Activated()
	{
		this.OnActivated.Invoke();
		UnlockablePropBase[] array = this.unlockables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Opened();
		}
	}

	// Token: 0x06002F83 RID: 12163 RVA: 0x000D1445 File Offset: 0x000CF645
	private IEnumerator UnlockDelayed()
	{
		yield return new WaitForSeconds(this.unlockDelay);
		UnlockablePropBase[] array = this.unlockables;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Open();
		}
		yield break;
	}

	// Token: 0x04003242 RID: 12866
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04003243 RID: 12867
	[Space]
	[SerializeField]
	private float unlockDelay;

	// Token: 0x04003244 RID: 12868
	[SerializeField]
	private UnlockablePropBase[] unlockables;

	// Token: 0x04003245 RID: 12869
	[Space]
	public UnityEvent OnActivate;

	// Token: 0x04003246 RID: 12870
	public UnityEvent OnActivated;

	// Token: 0x04003247 RID: 12871
	private bool isActivated;
}
