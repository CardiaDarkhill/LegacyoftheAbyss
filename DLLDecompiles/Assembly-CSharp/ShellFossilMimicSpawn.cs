using System;
using UnityEngine;

// Token: 0x02000319 RID: 793
public class ShellFossilMimicSpawn : MonoBehaviour
{
	// Token: 0x06001BF7 RID: 7159 RVA: 0x00082364 File Offset: 0x00080564
	private void LateUpdate()
	{
		if (!this.didCheck)
		{
			if (this.intItemReference.GetCurrentValue() == -10)
			{
				Object.Instantiate<GameObject>(this.mimicPrefab, base.transform.position, base.transform.rotation).transform.SetParent(base.transform);
			}
			this.didCheck = true;
		}
	}

	// Token: 0x04001AF2 RID: 6898
	public GameObject mimicPrefab;

	// Token: 0x04001AF3 RID: 6899
	public PersistentIntItem intItemReference;

	// Token: 0x04001AF4 RID: 6900
	private bool didCheck;
}
