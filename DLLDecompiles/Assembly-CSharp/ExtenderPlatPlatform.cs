using System;
using UnityEngine;

// Token: 0x020004DC RID: 1244
public class ExtenderPlatPlatform : MonoBehaviour
{
	// Token: 0x06002CB5 RID: 11445 RVA: 0x000C3998 File Offset: 0x000C1B98
	private void Awake()
	{
		if (!base.GetComponentInParent<ExtenderPlatsController>())
		{
			TiltPlat component = base.GetComponent<TiltPlat>();
			if (component)
			{
				component.ActivateTiltPlat(true);
			}
		}
	}
}
