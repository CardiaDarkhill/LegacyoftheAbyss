using System;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class ThunkTarget : MonoBehaviour
{
	// Token: 0x0600101F RID: 4127 RVA: 0x0004DE1F File Offset: 0x0004C01F
	private void Awake()
	{
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.effectPrefab, 5, true, false, false);
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x0004DE38 File Offset: 0x0004C038
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.GetComponent<NailSlashTerrainThunk>())
		{
			return;
		}
		Vector2 vector = (base.transform.position + other.transform.position) * 0.5f;
		vector = other.bounds.ClosestPoint(vector);
		this.effectPrefab.Spawn(vector.ToVector3(this.effectPrefab.transform.localPosition.z));
		HeroController componentInParent = other.GetComponentInParent<HeroController>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent.cState.upAttacking || componentInParent.cState.downAttacking)
		{
			return;
		}
		componentInParent.SetAllowRecoilWhileRelinquished(true);
		if (componentInParent.cState.facingRight)
		{
			componentInParent.RecoilLeft();
		}
		else
		{
			componentInParent.RecoilRight();
		}
		componentInParent.SetAllowRecoilWhileRelinquished(false);
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x0004DF18 File Offset: 0x0004C118
	private void OnCollisionEnter2D(Collision2D other)
	{
		this.TryThunk(other.collider);
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x0004DF28 File Offset: 0x0004C128
	private void TryThunk(Collider2D other)
	{
		if (!other.GetComponent<NailSlashTerrainThunk>())
		{
			return;
		}
		Vector2 vector = (base.transform.position + other.transform.position) * 0.5f;
		vector = other.bounds.ClosestPoint(vector);
		this.effectPrefab.Spawn(vector.ToVector3(this.effectPrefab.transform.localPosition.z));
		HeroController componentInParent = other.GetComponentInParent<HeroController>();
		if (!componentInParent)
		{
			return;
		}
		if (componentInParent.cState.upAttacking || componentInParent.cState.downAttacking)
		{
			return;
		}
		componentInParent.SetAllowRecoilWhileRelinquished(true);
		if (componentInParent.cState.facingRight)
		{
			componentInParent.RecoilLeft();
		}
		else
		{
			componentInParent.RecoilRight();
		}
		componentInParent.SetAllowRecoilWhileRelinquished(false);
	}

	// Token: 0x04000FB7 RID: 4023
	[SerializeField]
	private GameObject effectPrefab;
}
