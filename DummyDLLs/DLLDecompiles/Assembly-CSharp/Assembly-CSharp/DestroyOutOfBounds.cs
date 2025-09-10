using System;
using UnityEngine;

// Token: 0x020002C9 RID: 713
public class DestroyOutOfBounds : MonoBehaviour
{
	// Token: 0x06001989 RID: 6537 RVA: 0x00075127 File Offset: 0x00073327
	private void Update()
	{
		if (base.transform.position.y < -1f)
		{
			base.gameObject.SetActive(false);
		}
	}
}
