using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class CharacterBumpCheck : MonoBehaviour
{
	// Token: 0x06000C83 RID: 3203 RVA: 0x000375BA File Offset: 0x000357BA
	private void Awake()
	{
		this.groundMask = 256;
		this.body = base.GetComponent<Rigidbody2D>();
		this.collider = base.GetComponent<Collider2D>();
		this.getIsFacingRight = (() => base.transform.localScale.x > 0f);
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x000375F6 File Offset: 0x000357F6
	public static CharacterBumpCheck Add(GameObject target, LayerMask groundMask, Rigidbody2D body, Collider2D collider, Func<bool> getIsFacingRight)
	{
		CharacterBumpCheck characterBumpCheck = target.AddComponent<CharacterBumpCheck>();
		characterBumpCheck.groundMask = groundMask;
		characterBumpCheck.body = body;
		characterBumpCheck.collider = collider;
		characterBumpCheck.getIsFacingRight = getIsFacingRight;
		return characterBumpCheck;
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0003761C File Offset: 0x0003581C
	public void CheckForBump(CollisionSide side, out bool hitBump, out bool hitWall, out bool hitHighWall)
	{
		float num = 0.03f;
		Vector2 linearVelocity = this.body.linearVelocity;
		float num2;
		switch (side)
		{
		case CollisionSide.top:
		case CollisionSide.bottom:
			num2 = linearVelocity.y;
			break;
		case CollisionSide.left:
		case CollisionSide.right:
			num2 = linearVelocity.x;
			break;
		default:
			throw new ArgumentOutOfRangeException("side", side, null);
		}
		float num3 = num2;
		float num4 = num + Mathf.Abs(num3 * Time.deltaTime);
		float distance = 0.1f + num4;
		float num5 = Physics2D.defaultContactOffset * 2f + 0.001f;
		Bounds bounds = this.collider.bounds;
		Vector3 min = bounds.min;
		Vector3 center = bounds.center;
		Vector3 max = bounds.max;
		ContactFilter2D contactFilter = new ContactFilter2D
		{
			useTriggers = false,
			useLayerMask = true,
			layerMask = this.groundMask
		};
		bool flag = this.getIsFacingRight();
		if (side != CollisionSide.top)
		{
			if (side - CollisionSide.left > 1)
			{
				throw new ArgumentOutOfRangeException("side", side, null);
			}
			Vector2 origin = new Vector2(min.x + 0.1f, min.y + 0.2f);
			Vector2 origin2 = new Vector2(min.x + 0.1f, max.y + num5);
			Vector2 origin3 = new Vector2(min.x + 0.1f, min.y - num5);
			Vector2 origin4 = new Vector2(max.x - 0.1f, min.y + 0.2f);
			Vector2 origin5 = new Vector2(max.x - 0.1f, max.y + num5);
			Vector2 origin6 = new Vector2(max.x - 0.1f, min.y - num5);
			if (side == CollisionSide.left)
			{
				this.wallHitCount = Physics2D.Raycast(origin, Vector2.left, contactFilter, this.wallHits, distance);
				this.wallHitHighCount = Physics2D.Raycast(origin2, Vector2.left, contactFilter, this.wallHits, distance);
				this.bumpHitCount = Physics2D.Raycast(origin3, Vector2.left, contactFilter, this.bumpHits, distance);
			}
			else
			{
				this.wallHitCount = Physics2D.Raycast(origin4, Vector2.right, contactFilter, this.wallHits, distance);
				this.wallHitHighCount = Physics2D.Raycast(origin5, Vector2.right, contactFilter, this.wallHits, distance);
				this.bumpHitCount = Physics2D.Raycast(origin6, Vector2.right, contactFilter, this.bumpHits, distance);
			}
		}
		else
		{
			Vector2 origin7;
			Vector2 origin8;
			if (flag)
			{
				origin7 = new Vector2(max.x - 0.2f, max.y - 0.1f);
				origin8 = new Vector2(max.x + num5, max.y - 0.1f);
			}
			else
			{
				origin7 = new Vector2(min.x + 0.2f, max.y - 0.1f);
				origin8 = new Vector2(min.x - num5, max.y - 0.1f);
			}
			this.wallHitCount = Physics2D.Raycast(origin7, Vector2.up, contactFilter, this.wallHits, distance);
			this.wallHitHighCount = 0;
			this.bumpHitCount = Physics2D.Raycast(origin8, Vector2.up, contactFilter, this.bumpHits, distance);
		}
		hitBump = false;
		hitHighWall = (this.wallHitHighCount > 0);
		hitWall = (hitHighWall || this.wallHitCount > 0);
		float num6 = 0f;
		int i = 0;
		while (i < this.bumpHitCount)
		{
			RaycastHit2D raycastHit2D = this.bumpHits[i];
			if (!(raycastHit2D.collider != null) || hitWall)
			{
				goto IL_5EF;
			}
			if (!SteepSlope.IsSteepSlope(raycastHit2D.collider))
			{
				Vector2 origin9;
				Vector2 origin10;
				Vector2 direction;
				switch (side)
				{
				case CollisionSide.top:
					if (flag)
					{
						origin9 = raycastHit2D.point + new Vector2(-1f, 0.1f);
						origin10 = new Vector2(raycastHit2D.point.x - 1f, min.y - 0.001f);
						direction = Vector2.right;
					}
					else
					{
						origin9 = raycastHit2D.point + new Vector2(1f, 0.1f);
						origin10 = new Vector2(raycastHit2D.point.x + 1f, center.y - 0.001f);
						direction = Vector2.left;
					}
					break;
				case CollisionSide.left:
					origin9 = raycastHit2D.point + new Vector2(-0.1f, 1f);
					origin10 = new Vector2(max.x - 0.001f, raycastHit2D.point.y + 1f);
					direction = Vector2.down;
					break;
				case CollisionSide.right:
					origin9 = raycastHit2D.point + new Vector2(0.1f, 1f);
					origin10 = new Vector2(min.x + 0.001f, raycastHit2D.point.y + 1f);
					direction = Vector2.down;
					break;
				default:
					throw new ArgumentOutOfRangeException("side", side, null);
				}
				RaycastHit2D hit;
				Helper.IsRayHittingNoTriggers(origin9, direction, 1.2f, this.groundMask, out hit);
				RaycastHit2D hit2;
				Helper.IsRayHittingNoTriggers(origin10, direction, 1.2f, this.groundMask, out hit2);
				if (!hit)
				{
					goto IL_5EF;
				}
				if (hit2)
				{
					if (side != CollisionSide.top)
					{
						if (side - CollisionSide.left > 1)
						{
							throw new ArgumentOutOfRangeException("side", side, null);
						}
						num2 = hit.point.y - hit2.point.y;
					}
					else
					{
						num2 = (flag ? (hit2.point.x - hit.point.x) : (hit.point.x - hit2.point.x));
					}
					float num7 = num2;
					float num8;
					if (num7 >= Physics2D.defaultContactOffset)
					{
						num8 = num7;
					}
					else
					{
						num8 = Physics2D.defaultContactOffset;
					}
					if (num8 > num6)
					{
						num6 = num8;
					}
					hitBump = true;
					goto IL_5EF;
				}
				hitBump = true;
				goto IL_5EF;
			}
			IL_5F3:
			i++;
			continue;
			IL_5EF:
			if (!hitBump)
			{
				goto IL_5F3;
			}
			break;
		}
		if (!hitBump || num6 <= 0f)
		{
			return;
		}
		float y;
		float x;
		if (side != CollisionSide.top)
		{
			if (side - CollisionSide.left > 1)
			{
				throw new ArgumentOutOfRangeException("side", side, null);
			}
			y = num6 + Physics2D.defaultContactOffset;
			x = Physics2D.defaultContactOffset * (float)(flag ? 1 : -1);
		}
		else
		{
			y = Physics2D.defaultContactOffset;
			x = (num6 + Physics2D.defaultContactOffset) * (float)(flag ? -1 : 1);
		}
		Vector2 b = new Vector2(x, y);
		Vector2 position = this.body.position + b;
		this.body.position = position;
		Vector2 linearVelocity2 = this.body.linearVelocity;
		if (linearVelocity2.y < 0f)
		{
			linearVelocity2.y = 0f;
			this.body.linearVelocity = linearVelocity2;
		}
	}

	// Token: 0x04000C02 RID: 3074
	private LayerMask groundMask;

	// Token: 0x04000C03 RID: 3075
	private Rigidbody2D body;

	// Token: 0x04000C04 RID: 3076
	private Collider2D collider;

	// Token: 0x04000C05 RID: 3077
	private Func<bool> getIsFacingRight;

	// Token: 0x04000C06 RID: 3078
	private readonly RaycastHit2D[] bumpHits = new RaycastHit2D[10];

	// Token: 0x04000C07 RID: 3079
	private int bumpHitCount;

	// Token: 0x04000C08 RID: 3080
	private readonly RaycastHit2D[] wallHits = new RaycastHit2D[10];

	// Token: 0x04000C09 RID: 3081
	private int wallHitCount;

	// Token: 0x04000C0A RID: 3082
	private int wallHitHighCount;
}
