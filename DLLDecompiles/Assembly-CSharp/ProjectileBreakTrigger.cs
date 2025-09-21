using System;
using UnityEngine;

// Token: 0x02000361 RID: 865
public sealed class ProjectileBreakTrigger : MonoBehaviour
{
	// Token: 0x06001DEF RID: 7663 RVA: 0x0008A778 File Offset: 0x00088978
	private void OnTriggerEnter2D(Collider2D other)
	{
		IBreakableProjectile componentInParent = other.GetComponentInParent<IBreakableProjectile>();
		if (componentInParent != null)
		{
			componentInParent.QueueBreak(new IBreakableProjectile.HitInfo
			{
				isWall = this.isWall
			});
		}
	}

	// Token: 0x04001D14 RID: 7444
	[SerializeField]
	private bool isWall;
}
