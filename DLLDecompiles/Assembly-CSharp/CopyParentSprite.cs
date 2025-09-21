using System;
using UnityEngine;

// Token: 0x02000620 RID: 1568
public class CopyParentSprite : MonoBehaviour
{
	// Token: 0x060037C2 RID: 14274 RVA: 0x000F5EE7 File Offset: 0x000F40E7
	private void Start()
	{
		base.GetComponent<SpriteRenderer>().sprite = base.transform.parent.gameObject.GetComponent<SpriteRenderer>().sprite;
	}
}
