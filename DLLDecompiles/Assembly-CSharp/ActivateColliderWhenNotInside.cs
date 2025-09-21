using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
public class ActivateColliderWhenNotInside : MonoBehaviour
{
	// Token: 0x0600021D RID: 541 RVA: 0x0000D7E9 File Offset: 0x0000B9E9
	private void Awake()
	{
		this.layerMask = Helper.GetCollidingLayerMaskForLayer(this.collider.gameObject.layer);
	}

	// Token: 0x0600021E RID: 542 RVA: 0x0000D806 File Offset: 0x0000BA06
	private void Update()
	{
		if (!this.waitingToEnable || !this.collider)
		{
			return;
		}
		if (!this.IsInsideCollider())
		{
			this.collider.enabled = true;
			this.waitingToEnable = false;
		}
	}

	// Token: 0x0600021F RID: 543 RVA: 0x0000D839 File Offset: 0x0000BA39
	[ContextMenu("Activate", true)]
	[ContextMenu("Deactivate", true)]
	private bool CanTest()
	{
		return Application.isPlaying;
	}

	// Token: 0x06000220 RID: 544 RVA: 0x0000D840 File Offset: 0x0000BA40
	[ContextMenu("Activate")]
	public void ActivateCollider()
	{
		if (!this.collider)
		{
			return;
		}
		if (!this.IsInsideCollider())
		{
			this.collider.enabled = true;
			return;
		}
		this.waitingToEnable = true;
	}

	// Token: 0x06000221 RID: 545 RVA: 0x0000D86C File Offset: 0x0000BA6C
	[ContextMenu("Deactivate")]
	public void DeactivateCollider()
	{
		if (!this.collider)
		{
			return;
		}
		this.collider.enabled = false;
		this.waitingToEnable = false;
	}

	// Token: 0x06000222 RID: 546 RVA: 0x0000D890 File Offset: 0x0000BA90
	private bool IsInsideCollider()
	{
		if (!base.isActiveAndEnabled)
		{
			return false;
		}
		bool result = false;
		BoxCollider2D boxCollider2D = this.collider as BoxCollider2D;
		if (boxCollider2D != null)
		{
			result = Physics2D.OverlapBox(base.transform.TransformPoint(boxCollider2D.offset), boxCollider2D.size, base.transform.rotation.z, this.layerMask);
		}
		else
		{
			CircleCollider2D circleCollider2D = this.collider as CircleCollider2D;
			if (circleCollider2D != null)
			{
				result = Physics2D.OverlapCircle(base.transform.TransformPoint(circleCollider2D.offset), circleCollider2D.radius, this.layerMask);
			}
		}
		return result;
	}

	// Token: 0x040001D0 RID: 464
	[SerializeField]
	private Collider2D collider;

	// Token: 0x040001D1 RID: 465
	private bool waitingToEnable;

	// Token: 0x040001D2 RID: 466
	private int layerMask;
}
