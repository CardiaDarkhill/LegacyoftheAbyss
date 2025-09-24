using System;
using UnityEngine;

// Token: 0x0200047F RID: 1151
public class PreInstantiateGameObject : MonoBehaviour
{
	// Token: 0x170004F5 RID: 1269
	// (get) Token: 0x060029A0 RID: 10656 RVA: 0x000B5356 File Offset: 0x000B3556
	// (set) Token: 0x060029A1 RID: 10657 RVA: 0x000B535E File Offset: 0x000B355E
	public GameObject InstantiatedGameObject { get; private set; }

	// Token: 0x060029A2 RID: 10658 RVA: 0x000B5367 File Offset: 0x000B3567
	private void Awake()
	{
		if (!this.prefab)
		{
			return;
		}
		this.InstantiatedGameObject = Object.Instantiate<GameObject>(this.prefab);
		this.InstantiatedGameObject.SetActive(false);
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000B5394 File Offset: 0x000B3594
	public void Activate()
	{
		this.InstantiatedGameObject.SetActive(true);
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x000B53A2 File Offset: 0x000B35A2
	public void ActivateWithFsmEvent(string eventName)
	{
		this.InstantiatedGameObject.SetActive(true);
		this.InstantiatedGameObject.GetComponent<PlayMakerFSM>().SendEvent(eventName);
	}

	// Token: 0x04002A36 RID: 10806
	[SerializeField]
	private GameObject prefab;
}
