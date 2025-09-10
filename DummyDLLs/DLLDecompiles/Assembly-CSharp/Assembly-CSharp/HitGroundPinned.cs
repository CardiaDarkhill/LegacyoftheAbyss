using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004FE RID: 1278
public class HitGroundPinned : MonoBehaviour
{
	// Token: 0x06002DB7 RID: 11703 RVA: 0x000C7EC0 File Offset: 0x000C60C0
	private void Awake()
	{
		if (!base.transform.IsOnHeroPlane())
		{
			if (this.tinker)
			{
				Object.Destroy(this.tinker);
			}
			if (this.objectBounce)
			{
				Object.Destroy(this.objectBounce);
			}
			Collider2D[] componentsInChildren = base.GetComponentsInChildren<Collider2D>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i]);
			}
			if (this.body)
			{
				Object.Destroy(this.body);
			}
			Object.Destroy(this);
			return;
		}
		if (this.body)
		{
			this.body.bodyType = RigidbodyType2D.Static;
		}
		if (this.tinker)
		{
			this.tinker.HitInDirection += this.OnHitInDirection;
			if (this.fallOverControl)
			{
				this.fallOverControl.Fallen += delegate()
				{
					this.tinker.enabled = false;
				};
			}
		}
		if (this.objectBounce)
		{
			this.objectBounce.StopBounce();
		}
		if (this.terrainDetector)
		{
			this.terrainDetector.SetActive(false);
		}
	}

	// Token: 0x06002DB8 RID: 11704 RVA: 0x000C7FDC File Offset: 0x000C61DC
	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		if (!this.body)
		{
			return;
		}
		Vector2 linearVelocity;
		switch (direction)
		{
		case HitInstance.HitDirection.Left:
			linearVelocity = new Vector2(-Random.Range(this.hitVelocityHMin.x, this.hitVelocityHMax.x), Random.Range(this.hitVelocityHMin.y, this.hitVelocityHMax.y));
			break;
		case HitInstance.HitDirection.Right:
			linearVelocity = new Vector2(Random.Range(this.hitVelocityHMin.x, this.hitVelocityHMax.x), Random.Range(this.hitVelocityHMin.y, this.hitVelocityHMax.y));
			break;
		case HitInstance.HitDirection.Up:
		case HitInstance.HitDirection.Down:
			linearVelocity = new Vector2(Random.Range(this.hitVelocityVMin.x, this.hitVelocityVMax.x), Random.Range(this.hitVelocityVMin.y, this.hitVelocityVMax.y));
			break;
		default:
			throw new NotImplementedException();
		}
		if (this.body.bodyType != RigidbodyType2D.Dynamic)
		{
			this.body.bodyType = RigidbodyType2D.Dynamic;
			Vector2 vector = this.body.position;
			vector += new Vector2(0f, 0.5f);
			this.body.position = vector;
			this.body.transform.SetPosition2D(vector);
			this.body.transform.SetLocalPositionZ(Random.Range(0.003f, 0.006f));
			this.body.linearVelocity = linearVelocity;
			this.OnUnpinned.Invoke();
			if (this.objectBounce)
			{
				this.objectBounce.StartBounce();
			}
			if (this.terrainDetector)
			{
				this.terrainDetector.SetActive(true);
			}
		}
		else
		{
			this.body.linearVelocity = linearVelocity;
		}
		this.OnHit.Invoke();
		if (this.fallOverControl && Random.Range(0f, 1f) < this.fallChancePerHit)
		{
			this.fallOverControl.Activate();
		}
	}

	// Token: 0x04002FA9 RID: 12201
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x04002FAA RID: 12202
	[SerializeField]
	private TinkEffect tinker;

	// Token: 0x04002FAB RID: 12203
	[SerializeField]
	private ObjectBounce objectBounce;

	// Token: 0x04002FAC RID: 12204
	[SerializeField]
	private GameObject terrainDetector;

	// Token: 0x04002FAD RID: 12205
	[Space]
	[SerializeField]
	private Vector2 hitVelocityHMin;

	// Token: 0x04002FAE RID: 12206
	[SerializeField]
	private Vector2 hitVelocityHMax;

	// Token: 0x04002FAF RID: 12207
	[SerializeField]
	private Vector2 hitVelocityVMin;

	// Token: 0x04002FB0 RID: 12208
	[SerializeField]
	private Vector2 hitVelocityVMax;

	// Token: 0x04002FB1 RID: 12209
	[SerializeField]
	private CogRollThenFallOver fallOverControl;

	// Token: 0x04002FB2 RID: 12210
	[SerializeField]
	[Range(0f, 1f)]
	private float fallChancePerHit = 1f;

	// Token: 0x04002FB3 RID: 12211
	[Space]
	public UnityEvent OnUnpinned;

	// Token: 0x04002FB4 RID: 12212
	[Space]
	public UnityEvent OnHit;
}
