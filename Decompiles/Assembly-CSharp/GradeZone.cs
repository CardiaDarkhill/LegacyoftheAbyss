using System;
using UnityEngine;

// Token: 0x0200023A RID: 570
[RequireComponent(typeof(PolygonCollider2D))]
public class GradeZone : MonoBehaviour
{
	// Token: 0x060014DB RID: 5339 RVA: 0x0005E230 File Offset: 0x0005C430
	private void Start()
	{
		Debug.LogError("GrazeZone has been deprecated, please remove this object: " + base.name);
	}
}
