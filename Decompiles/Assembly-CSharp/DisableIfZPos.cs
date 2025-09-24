using System;
using UnityEngine;

// Token: 0x0200020B RID: 523
public class DisableIfZPos : MonoBehaviour
{
	// Token: 0x06001399 RID: 5017 RVA: 0x0005955F File Offset: 0x0005775F
	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > this.limitZ)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040011FC RID: 4604
	[Tooltip("If further than this distance from the hero on Z, will be disabled.")]
	public float limitZ = 1.8f;
}
