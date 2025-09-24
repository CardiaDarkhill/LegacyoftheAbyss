using System;
using UnityEngine;

// Token: 0x020002A6 RID: 678
public class ContinuousTrigger : MonoBehaviour
{
	// Token: 0x06001813 RID: 6163 RVA: 0x0006DFBC File Offset: 0x0006C1BC
	private void Awake()
	{
		this.selfCollider = base.GetComponent<Collider2D>();
		this.selfColliderSize = this.selfCollider.bounds.size.y;
		this.childCollider.enabled = false;
	}

	// Token: 0x06001814 RID: 6164 RVA: 0x0006DFFF File Offset: 0x0006C1FF
	private void OnEnable()
	{
		this.previousPos = base.transform.position;
		this.UpdateChildCollider();
	}

	// Token: 0x06001815 RID: 6165 RVA: 0x0006E01D File Offset: 0x0006C21D
	private void FixedUpdate()
	{
		this.UpdateChildCollider();
		this.previousPos = base.transform.position;
	}

	// Token: 0x06001816 RID: 6166 RVA: 0x0006E03C File Offset: 0x0006C23C
	private void UpdateChildCollider()
	{
		Vector2 vector = base.transform.position - this.previousPos;
		float magnitude = vector.magnitude;
		if (magnitude <= Mathf.Epsilon)
		{
			this.childCollider.enabled = false;
			return;
		}
		float num = 1f / Mathf.Abs(base.transform.lossyScale.y);
		float num2 = magnitude * num;
		this.childCollider.enabled = true;
		this.childCollider.size = new Vector2(num2, this.selfColliderSize * num);
		this.childCollider.offset = new Vector2(num2 / 2f, 0f);
		this.childCollider.transform.SetRotation2D(vector.normalized.DirectionToAngle());
	}

	// Token: 0x040016E7 RID: 5863
	[SerializeField]
	private BoxCollider2D childCollider;

	// Token: 0x040016E8 RID: 5864
	private Vector2 previousPos;

	// Token: 0x040016E9 RID: 5865
	private Collider2D selfCollider;

	// Token: 0x040016EA RID: 5866
	private float selfColliderSize;
}
