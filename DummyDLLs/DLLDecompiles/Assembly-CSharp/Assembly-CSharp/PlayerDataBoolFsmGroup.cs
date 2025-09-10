using System;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200052E RID: 1326
public class PlayerDataBoolFsmGroup : MonoBehaviour
{
	// Token: 0x06002F9A RID: 12186 RVA: 0x000D19C0 File Offset: 0x000CFBC0
	private void Start()
	{
		if (!this.parent)
		{
			base.enabled = false;
			return;
		}
		if (!string.IsNullOrEmpty(this.pdBool) && PlayerData.instance.GetVariable(this.pdBool))
		{
			if (this.parent)
			{
				this.parent.SetActive(false);
			}
			this.Unlock();
			return;
		}
		this.activateObjects.SetAllActive(false);
		foreach (PlayMakerFSM playMakerFSM in this.parent.GetComponentsInChildren<PlayMakerFSM>(true))
		{
			FsmGameObject fsmGameObject = playMakerFSM.FsmVariables.FindFsmGameObject("Notify Group");
			if (fsmGameObject != null)
			{
				fsmGameObject.Value = base.gameObject;
			}
			FsmBool fsmBool = playMakerFSM.FsmVariables.FindFsmBool("Activated");
			if (fsmBool != null)
			{
				this.activatedBools.Add(fsmBool);
			}
		}
	}

	// Token: 0x06002F9B RID: 12187 RVA: 0x000D1A8C File Offset: 0x000CFC8C
	public void UpdateCrustUnlock()
	{
		if (string.IsNullOrEmpty(this.pdBool))
		{
			return;
		}
		using (List<FsmBool>.Enumerator enumerator = this.activatedBools.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (!enumerator.Current.Value)
				{
					return;
				}
			}
		}
		PlayerData.instance.SetVariable(this.pdBool, true);
		this.Unlock();
		if (!string.IsNullOrEmpty(this.sendEventOnUnlock))
		{
			GameObject[] array = this.activateObjects;
			for (int i = 0; i < array.Length; i++)
			{
				FSMUtility.SendEventToGameObject(array[i], this.sendEventOnUnlock, false);
			}
		}
	}

	// Token: 0x06002F9C RID: 12188 RVA: 0x000D1B38 File Offset: 0x000CFD38
	private void Unlock()
	{
		if (this.wasUnlocked)
		{
			return;
		}
		this.wasUnlocked = true;
		this.activateObjects.SetAllActive(true);
	}

	// Token: 0x0400325B RID: 12891
	[SerializeField]
	private GameObject parent;

	// Token: 0x0400325C RID: 12892
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string pdBool;

	// Token: 0x0400325D RID: 12893
	[Space]
	[SerializeField]
	private GameObject[] activateObjects;

	// Token: 0x0400325E RID: 12894
	[Space]
	[SerializeField]
	private string sendEventOnUnlock;

	// Token: 0x0400325F RID: 12895
	private bool wasUnlocked;

	// Token: 0x04003260 RID: 12896
	private readonly List<FsmBool> activatedBools = new List<FsmBool>();
}
