using System;
using UnityEngine;

// Token: 0x02000556 RID: 1366
[RequireComponent(typeof(Collider2D))]
public class SnapToGround : MonoBehaviour
{
	// Token: 0x060030E3 RID: 12515 RVA: 0x000D86CF File Offset: 0x000D68CF
	private void Awake()
	{
		this.col = base.GetComponent<Collider2D>();
	}

	// Token: 0x060030E4 RID: 12516 RVA: 0x000D86E0 File Offset: 0x000D68E0
	private void OnEnable()
	{
		float y = this.col.bounds.min.y;
		float num = base.transform.position.y - y;
		RaycastHit2D raycastHit2D = Helper.Raycast2D(base.transform.position, Vector2.down, 10f, 256);
		if (raycastHit2D.collider != null)
		{
			base.transform.SetPositionY(raycastHit2D.point.y + num);
		}
	}

	// Token: 0x04003420 RID: 13344
	private Collider2D col;
}
