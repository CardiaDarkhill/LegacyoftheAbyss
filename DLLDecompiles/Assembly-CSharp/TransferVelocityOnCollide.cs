using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000288 RID: 648
public class TransferVelocityOnCollide : MonoBehaviour
{
	// Token: 0x060016CE RID: 5838 RVA: 0x000669B3 File Offset: 0x00064BB3
	[UsedImplicitly]
	private bool IsSource()
	{
		return this.type == TransferVelocityOnCollide.Types.Source;
	}

	// Token: 0x060016CF RID: 5839 RVA: 0x000669BE File Offset: 0x00064BBE
	private void Awake()
	{
		this.box = base.GetComponent<BoxCollider2D>();
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x000669CC File Offset: 0x00064BCC
	private void FixedUpdate()
	{
		this.cooldownTicks--;
		if (!this.body)
		{
			return;
		}
		bool flag = this.DetectTerrain();
		if (flag && !this.didLastRayHit)
		{
			Vector2 linearVelocity = -this.lastVelocity.normalized * this.wallBounceMagnitude;
			this.body.linearVelocity = linearVelocity;
			this.OnBounce.Invoke();
		}
		this.lastVelocity = this.body.linearVelocity;
		this.didLastRayHit = flag;
	}

	// Token: 0x060016D1 RID: 5841 RVA: 0x00066A54 File Offset: 0x00064C54
	private bool DetectTerrain()
	{
		if (Mathf.Abs(this.wallBounceMagnitude) <= Mathf.Epsilon)
		{
			return false;
		}
		if (Mathf.Abs(this.lastVelocity.x) <= Mathf.Epsilon)
		{
			return false;
		}
		float distance = this.wallRayDistance - -0.1f;
		Bounds bounds = this.box.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		Vector2 direction;
		Vector2 origin;
		Vector2 origin2;
		if (this.lastVelocity.x > 0f)
		{
			direction = Vector2.right;
			origin = new Vector2(max.x + -0.1f, max.y + 0.001f);
			origin2 = new Vector2(max.x + -0.1f, min.y - 0.001f);
		}
		else
		{
			direction = Vector2.left;
			origin = new Vector2(min.x - -0.1f, max.y + 0.001f);
			origin2 = new Vector2(min.x - -0.1f, min.y - 0.001f);
		}
		RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, direction, distance, 256);
		if (raycastHit2D.collider && !raycastHit2D.collider.GetComponent<TransferVelocityOnCollide>())
		{
			return true;
		}
		RaycastHit2D raycastHit2D2 = Helper.Raycast2D(origin2, direction, distance, 256);
		return raycastHit2D2.collider && !raycastHit2D2.collider.GetComponent<TransferVelocityOnCollide>();
	}

	// Token: 0x060016D2 RID: 5842 RVA: 0x00066BC0 File Offset: 0x00064DC0
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.type != TransferVelocityOnCollide.Types.Source)
		{
			return;
		}
		if (this.cooldownTicks > 0)
		{
			return;
		}
		TransferVelocityOnCollide component = collision.gameObject.GetComponent<TransferVelocityOnCollide>();
		if (!component)
		{
			return;
		}
		if (component.type != TransferVelocityOnCollide.Types.Target)
		{
			return;
		}
		if (this.body && this.lastVelocity.magnitude <= Mathf.Epsilon)
		{
			return;
		}
		Vector2 a;
		if (this.body)
		{
			a = this.lastVelocity.normalized;
			this.body.linearVelocity = Vector2.zero;
		}
		else
		{
			Vector2 vector = component.lastVelocity;
			a = -vector.normalized;
		}
		Vector2 linearVelocity = a * this.minMagnitude;
		component.body.linearVelocity = linearVelocity;
		if (component.linkedSource)
		{
			component.linkedSource.cooldownTicks = 2;
		}
		component.OnBounce.Invoke();
	}

	// Token: 0x0400154A RID: 5450
	[SerializeField]
	private TransferVelocityOnCollide.Types type;

	// Token: 0x0400154B RID: 5451
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x0400154C RID: 5452
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsSource", true, true, false)]
	private float minMagnitude;

	// Token: 0x0400154D RID: 5453
	[SerializeField]
	[ModifiableProperty]
	[Conditional("IsSource", false, true, false)]
	private TransferVelocityOnCollide linkedSource;

	// Token: 0x0400154E RID: 5454
	[Space]
	public UnityEvent OnBounce;

	// Token: 0x0400154F RID: 5455
	[Space]
	[SerializeField]
	private float wallBounceMagnitude;

	// Token: 0x04001550 RID: 5456
	[SerializeField]
	private float wallRayDistance;

	// Token: 0x04001551 RID: 5457
	private Vector2 lastVelocity;

	// Token: 0x04001552 RID: 5458
	private int cooldownTicks;

	// Token: 0x04001553 RID: 5459
	private bool didLastRayHit;

	// Token: 0x04001554 RID: 5460
	private BoxCollider2D box;

	// Token: 0x0200155D RID: 5469
	private enum Types
	{
		// Token: 0x040086E4 RID: 34532
		Source,
		// Token: 0x040086E5 RID: 34533
		Target
	}
}
