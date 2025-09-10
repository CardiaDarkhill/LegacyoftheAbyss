using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000045 RID: 69
[RequireComponent(typeof(BoxCollider2D))]
public class CollisionDepress : DebugDrawColliderRuntimeAdder
{
	// Token: 0x060001EE RID: 494 RVA: 0x0000C495 File Offset: 0x0000A695
	protected override void Awake()
	{
		base.Awake();
		this.collider = base.GetComponent<BoxCollider2D>();
		if (this.childToMove)
		{
			this.initialPosition = this.childToMove.localPosition;
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x0000C4C8 File Offset: 0x0000A6C8
	private void OnEnable()
	{
		foreach (CollisionDepress.Rotator rotator in this.rotators)
		{
			if (rotator.Target)
			{
				rotator.InitialRotation = rotator.Target.localEulerAngles.z;
			}
		}
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x0000C511 File Offset: 0x0000A711
	private void OnDisable()
	{
		if (this.isDepressed)
		{
			this.SetNotDepressed();
		}
		this.collided.Clear();
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x0000C52C File Offset: 0x0000A72C
	private void FixedUpdate()
	{
		if (!this.isDepressed && this.collided.Count > 0)
		{
			foreach (Collider2D otherCollider in this.collided)
			{
				if (this.CanColliderDepress(otherCollider))
				{
					this.SetDepressed();
				}
			}
		}
		if (this.depressReturnStepsLeft > 0)
		{
			this.depressReturnStepsLeft--;
			if (this.depressReturnStepsLeft <= 0)
			{
				this.SetNotDepressed();
			}
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x0000C5C4 File Offset: 0x0000A7C4
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!this.useTrigger)
		{
			this.HandleEnter(collision.collider);
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x0000C5DA File Offset: 0x0000A7DA
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (!this.useTrigger)
		{
			this.HandleExit(collision.collider);
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0000C5F0 File Offset: 0x0000A7F0
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (this.useTrigger)
		{
			this.HandleEnter(other);
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x0000C601 File Offset: 0x0000A801
	private void OnTriggerExit2D(Collider2D other)
	{
		if (this.useTrigger)
		{
			this.HandleExit(other);
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x0000C614 File Offset: 0x0000A814
	private void HandleEnter(Collider2D other)
	{
		if ((1 << other.gameObject.layer & this.layerMask) == 0)
		{
			return;
		}
		this.collided.AddIfNotPresent(other);
		if (this.CanColliderDepress(other))
		{
			this.depressReturnStepsLeft = 0;
			if (!this.isDepressed)
			{
				this.SetDepressed();
			}
		}
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x0000C66B File Offset: 0x0000A86B
	private void HandleExit(Collider2D other)
	{
		if (this.collided.Count == 0)
		{
			return;
		}
		this.collided.Remove(other);
		if (this.isDepressed && this.collided.Count == 0)
		{
			this.depressReturnStepsLeft = 5;
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x0000C6A4 File Offset: 0x0000A8A4
	private bool CanColliderDepress(Collider2D otherCollider)
	{
		if (this.isDisabled)
		{
			return false;
		}
		if (this.useTrigger)
		{
			return true;
		}
		float y = this.collider.bounds.max.y;
		return otherCollider.bounds.min.y >= y;
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x0000C6F8 File Offset: 0x0000A8F8
	private void SetDepressed()
	{
		this.isDepressed = true;
		if (this.childToMove)
		{
			this.childToMove.localPosition = this.initialPosition + new Vector3(0f, -this.depressDistance, 0f);
		}
		this.onDepress.Invoke();
		foreach (CollisionDepress.Rotator rotator in this.rotators)
		{
			if (rotator.Target)
			{
				rotator.Target.SetLocalRotation2D(rotator.InitialRotation + rotator.Offset);
			}
		}
	}

	// Token: 0x060001FA RID: 506 RVA: 0x0000C790 File Offset: 0x0000A990
	private void SetNotDepressed()
	{
		this.isDepressed = false;
		if (this.childToMove)
		{
			this.childToMove.localPosition = this.initialPosition;
		}
		this.depressReturnStepsLeft = 0;
		this.onRise.Invoke();
		foreach (CollisionDepress.Rotator rotator in this.rotators)
		{
			if (rotator.Target)
			{
				rotator.Target.SetLocalRotation2D(rotator.InitialRotation);
			}
		}
	}

	// Token: 0x060001FB RID: 507 RVA: 0x0000C80C File Offset: 0x0000AA0C
	public void SetActive(bool value)
	{
		if (value)
		{
			this.isDisabled = false;
			if (this.childToMove)
			{
				this.childToMove.localPosition = this.initialPosition;
			}
			if (!this.isDepressed && this.collided.Count > 0)
			{
				this.SetDepressed();
				return;
			}
		}
		else
		{
			this.isDisabled = true;
			if (this.isDepressed)
			{
				this.SetNotDepressed();
			}
			if (this.childToMove)
			{
				this.childToMove.localPosition = this.initialPosition + new Vector3(0f, -this.depressDistance, 0f);
			}
		}
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000C8AC File Offset: 0x0000AAAC
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x04000190 RID: 400
	[SerializeField]
	private LayerMask layerMask;

	// Token: 0x04000191 RID: 401
	[SerializeField]
	private Transform childToMove;

	// Token: 0x04000192 RID: 402
	[SerializeField]
	private float depressDistance;

	// Token: 0x04000193 RID: 403
	[SerializeField]
	private bool useTrigger;

	// Token: 0x04000194 RID: 404
	[Space]
	[SerializeField]
	private UnityEvent onDepress;

	// Token: 0x04000195 RID: 405
	[SerializeField]
	private UnityEvent onRise;

	// Token: 0x04000196 RID: 406
	[Space]
	[SerializeField]
	private CollisionDepress.Rotator[] rotators;

	// Token: 0x04000197 RID: 407
	private Vector3 initialPosition;

	// Token: 0x04000198 RID: 408
	private int depressReturnStepsLeft;

	// Token: 0x04000199 RID: 409
	private readonly List<Collider2D> collided = new List<Collider2D>();

	// Token: 0x0400019A RID: 410
	private bool isDepressed;

	// Token: 0x0400019B RID: 411
	private bool isDisabled;

	// Token: 0x0400019C RID: 412
	private BoxCollider2D collider;

	// Token: 0x020013D3 RID: 5075
	[Serializable]
	public class Rotator
	{
		// Token: 0x040080CB RID: 32971
		public Transform Target;

		// Token: 0x040080CC RID: 32972
		public float Offset;

		// Token: 0x040080CD RID: 32973
		[NonSerialized]
		public float InitialRotation;
	}
}
