using System;
using UnityEngine;

// Token: 0x020004E7 RID: 1255
public class GameObjectMatchActiveState : MonoBehaviour
{
	// Token: 0x06002D01 RID: 11521 RVA: 0x000C49D0 File Offset: 0x000C2BD0
	private void OnEnable()
	{
		if (this.readStateFrom && !this.readStateFrom.activeInHierarchy)
		{
			base.gameObject.SetActive(false);
		}
		if (this.setStateOn)
		{
			this.setStateOn.SetActive(true);
		}
	}

	// Token: 0x06002D02 RID: 11522 RVA: 0x000C4A1C File Offset: 0x000C2C1C
	private void OnDisable()
	{
		if (this.setStateOn)
		{
			this.setStateOn.SetActive(false);
		}
	}

	// Token: 0x04002EA7 RID: 11943
	[SerializeField]
	private GameObject setStateOn;

	// Token: 0x04002EA8 RID: 11944
	[SerializeField]
	private GameObject readStateFrom;
}
