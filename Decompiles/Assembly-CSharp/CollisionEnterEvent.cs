using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200017A RID: 378
public class CollisionEnterEvent : MonoBehaviour
{
	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06000C5A RID: 3162 RVA: 0x00036384 File Offset: 0x00034584
	// (remove) Token: 0x06000C5B RID: 3163 RVA: 0x000363BC File Offset: 0x000345BC
	public event CollisionEnterEvent.DirectionalCollisionEvent CollisionEnteredDirectional;

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06000C5C RID: 3164 RVA: 0x000363F4 File Offset: 0x000345F4
	// (remove) Token: 0x06000C5D RID: 3165 RVA: 0x0003642C File Offset: 0x0003462C
	public event CollisionEnterEvent.CollisionEvent CollisionEntered;

	// Token: 0x1400001B RID: 27
	// (add) Token: 0x06000C5E RID: 3166 RVA: 0x00036464 File Offset: 0x00034664
	// (remove) Token: 0x06000C5F RID: 3167 RVA: 0x0003649C File Offset: 0x0003469C
	public event CollisionEnterEvent.CollisionEvent CollisionExited;

	// Token: 0x17000155 RID: 341
	// (get) Token: 0x06000C60 RID: 3168 RVA: 0x000364D1 File Offset: 0x000346D1
	// (set) Token: 0x06000C61 RID: 3169 RVA: 0x000364D9 File Offset: 0x000346D9
	public bool DoCollisionStay { get; set; }

	// Token: 0x06000C62 RID: 3170 RVA: 0x000364E2 File Offset: 0x000346E2
	private void Awake()
	{
		this.col2d = base.GetComponent<Collider2D>();
	}

	// Token: 0x06000C63 RID: 3171 RVA: 0x000364F0 File Offset: 0x000346F0
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.CollisionEntered != null)
		{
			this.CollisionEntered(collision);
		}
		this.HandleCollision(collision);
	}

	// Token: 0x06000C64 RID: 3172 RVA: 0x0003650D File Offset: 0x0003470D
	private void OnCollisionStay2D(Collision2D collision)
	{
		if (this.DoCollisionStay)
		{
			this.HandleCollision(collision);
		}
	}

	// Token: 0x06000C65 RID: 3173 RVA: 0x0003651E File Offset: 0x0003471E
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (this.CollisionExited != null)
		{
			this.CollisionExited(collision);
		}
		if (this.OnCollisionExited != null)
		{
			this.OnCollisionExited.Invoke();
		}
	}

	// Token: 0x06000C66 RID: 3174 RVA: 0x00036547 File Offset: 0x00034747
	private void HandleCollision(Collision2D collision)
	{
		if (this.OnCollisionEntered != null)
		{
			this.OnCollisionEntered.Invoke();
		}
		if (this.checkDirection)
		{
			this.CheckTouching((int)this.otherLayer, collision);
		}
	}

	// Token: 0x06000C67 RID: 3175 RVA: 0x00036578 File Offset: 0x00034778
	private void CheckTouching(LayerMask layer, Collision2D collision)
	{
		this.topRays.Clear();
		this.topRays.Add(new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.max.y));
		this.topRays.Add(new Vector2(this.col2d.bounds.center.x, this.col2d.bounds.max.y));
		this.topRays.Add(this.col2d.bounds.max);
		this.rightRays.Clear();
		this.rightRays.Add(this.col2d.bounds.max);
		this.rightRays.Add(new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.center.y));
		this.rightRays.Add(new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.min.y));
		this.bottomRays.Clear();
		this.bottomRays.Add(new Vector2(this.col2d.bounds.max.x, this.col2d.bounds.min.y));
		this.bottomRays.Add(new Vector2(this.col2d.bounds.center.x, this.col2d.bounds.min.y));
		this.bottomRays.Add(this.col2d.bounds.min);
		this.leftRays.Clear();
		this.leftRays.Add(this.col2d.bounds.min);
		this.leftRays.Add(new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.center.y));
		this.leftRays.Add(new Vector2(this.col2d.bounds.min.x, this.col2d.bounds.max.y));
		foreach (Vector2 v in this.topRays)
		{
			RaycastHit2D raycastHit2D = Helper.Raycast2D(v, Vector2.up, 0.08f, 1 << layer);
			if (raycastHit2D.collider != null && (!this.ignoreTriggers || !raycastHit2D.collider.isTrigger))
			{
				if (this.CollisionEnteredDirectional != null)
				{
					this.CollisionEnteredDirectional(CollisionEnterEvent.Direction.Top, collision);
					break;
				}
				break;
			}
		}
		foreach (Vector2 v2 in this.rightRays)
		{
			RaycastHit2D raycastHit2D2 = Helper.Raycast2D(v2, Vector2.right, 0.08f, 1 << layer);
			if (raycastHit2D2.collider != null && (!this.ignoreTriggers || !raycastHit2D2.collider.isTrigger))
			{
				if (this.CollisionEnteredDirectional != null)
				{
					this.CollisionEnteredDirectional(CollisionEnterEvent.Direction.Right, collision);
					break;
				}
				break;
			}
		}
		foreach (Vector2 v3 in this.bottomRays)
		{
			RaycastHit2D raycastHit2D3 = Helper.Raycast2D(v3, -Vector2.up, 0.08f, 1 << layer);
			if (raycastHit2D3.collider != null && (!this.ignoreTriggers || !raycastHit2D3.collider.isTrigger))
			{
				if (this.CollisionEnteredDirectional != null)
				{
					this.CollisionEnteredDirectional(CollisionEnterEvent.Direction.Bottom, collision);
					break;
				}
				break;
			}
		}
		foreach (Vector2 v4 in this.leftRays)
		{
			RaycastHit2D raycastHit2D4 = Helper.Raycast2D(v4, -Vector2.right, 0.08f, 1 << layer);
			if (raycastHit2D4.collider != null && (!this.ignoreTriggers || !raycastHit2D4.collider.isTrigger))
			{
				if (this.CollisionEnteredDirectional != null)
				{
					this.CollisionEnteredDirectional(CollisionEnterEvent.Direction.Left, collision);
					break;
				}
				break;
			}
		}
	}

	// Token: 0x04000BD4 RID: 3028
	public bool checkDirection;

	// Token: 0x04000BD5 RID: 3029
	public bool ignoreTriggers;

	// Token: 0x04000BD6 RID: 3030
	public PhysLayers otherLayer = PhysLayers.TERRAIN;

	// Token: 0x04000BD8 RID: 3032
	[Space]
	public UnityEvent OnCollisionEntered;

	// Token: 0x04000BD9 RID: 3033
	public UnityEvent OnCollisionExited;

	// Token: 0x04000BDA RID: 3034
	private Collider2D col2d;

	// Token: 0x04000BDB RID: 3035
	private const float RAYCAST_LENGTH = 0.08f;

	// Token: 0x04000BDC RID: 3036
	private List<Vector2> topRays = new List<Vector2>(3);

	// Token: 0x04000BDD RID: 3037
	private List<Vector2> rightRays = new List<Vector2>(3);

	// Token: 0x04000BDE RID: 3038
	private List<Vector2> bottomRays = new List<Vector2>(3);

	// Token: 0x04000BDF RID: 3039
	private List<Vector2> leftRays = new List<Vector2>(3);

	// Token: 0x020014A9 RID: 5289
	public enum Direction
	{
		// Token: 0x04008424 RID: 33828
		Left,
		// Token: 0x04008425 RID: 33829
		Right,
		// Token: 0x04008426 RID: 33830
		Top,
		// Token: 0x04008427 RID: 33831
		Bottom
	}

	// Token: 0x020014AA RID: 5290
	// (Invoke) Token: 0x06008437 RID: 33847
	public delegate void DirectionalCollisionEvent(CollisionEnterEvent.Direction direction, Collision2D collision);

	// Token: 0x020014AB RID: 5291
	// (Invoke) Token: 0x0600843B RID: 33851
	public delegate void CollisionEvent(Collision2D collision);
}
