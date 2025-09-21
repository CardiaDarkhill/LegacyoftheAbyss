using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031B RID: 795
public struct Sweep
{
	// Token: 0x06001C03 RID: 7171 RVA: 0x00082766 File Offset: 0x00080966
	public Sweep(Collider2D collider, int cardinalDirection, int rayCount, float skinThickness = 0.1f, float skinWideThickness = 0.01f)
	{
		this.Direction = new Vector2((float)DirectionUtils.GetX(cardinalDirection), (float)DirectionUtils.GetY(cardinalDirection));
		this.Collider = collider;
		this.RayCount = rayCount;
		this.SkinThickness = skinThickness;
		this.SkinWideThickness = skinWideThickness;
	}

	// Token: 0x06001C04 RID: 7172 RVA: 0x000827A0 File Offset: 0x000809A0
	public bool Check(float distance, int layerMask)
	{
		float num;
		return this.Check(distance, layerMask, out num);
	}

	// Token: 0x06001C05 RID: 7173 RVA: 0x000827B8 File Offset: 0x000809B8
	public bool Check(float distance, int layerMask, bool useTriggers)
	{
		float num;
		return this.Check(distance, layerMask, out num, useTriggers);
	}

	// Token: 0x06001C06 RID: 7174 RVA: 0x000827D0 File Offset: 0x000809D0
	public bool Check(float distance, int layerMask, out float clippedDistance)
	{
		return this.Check(distance, layerMask, out clippedDistance, false);
	}

	// Token: 0x06001C07 RID: 7175 RVA: 0x000827DC File Offset: 0x000809DC
	public bool Check(float distance, int layerMask, out float clippedDistance, bool useTriggers)
	{
		return this.Check(distance, layerMask, out clippedDistance, useTriggers, Vector2.zero);
	}

	// Token: 0x06001C08 RID: 7176 RVA: 0x000827F0 File Offset: 0x000809F0
	public bool Check(float distance, int layerMask, out float clippedDistance, bool useTriggers, Vector2 offset)
	{
		if (distance <= 0f)
		{
			clippedDistance = 0f;
			return false;
		}
		Bounds bounds = this.GetBounds();
		Vector3 extents = bounds.extents;
		Vector2 b = Vector2.Scale(extents, this.Direction);
		Vector2 a = bounds.center + b;
		Vector2 a2 = Vector2.Scale(extents, new Vector2(Mathf.Abs(this.Direction.y), Mathf.Abs(this.Direction.x)));
		float num = distance;
		float num2 = Mathf.Max(b.magnitude * 2f - this.SkinThickness, this.SkinThickness);
		Vector2 b2 = this.Direction * -num2;
		Vector2 a3 = Vector3.Cross(this.Direction, -Vector3.forward);
		for (int i = 0; i < this.RayCount; i++)
		{
			float num3 = 2f * ((float)i / (float)(this.RayCount - 1)) - 1f;
			Vector2 b3 = a + a2 * num3 + b2 + a3 * (num3 * this.SkinWideThickness);
			Vector2 origin = offset + b3;
			RaycastHit2D hit;
			bool flag;
			if (useTriggers)
			{
				hit = Helper.Raycast2D(origin, this.Direction, num + num2, layerMask);
				flag = hit;
			}
			else
			{
				flag = Helper.IsRayHittingNoTriggers(origin, this.Direction, num + num2, layerMask, out hit);
			}
			float num4 = hit.distance - num2;
			if (flag && num4 < num)
			{
				num = num4;
			}
		}
		clippedDistance = num;
		return distance - num > Mathf.Epsilon;
	}

	// Token: 0x06001C09 RID: 7177 RVA: 0x00082998 File Offset: 0x00080B98
	private Bounds GetBounds()
	{
		if (!this.Collider.enabled)
		{
			BoxCollider2D boxCollider2D = this.Collider as BoxCollider2D;
			if (boxCollider2D != null)
			{
				return Sweep.CalculateBoxCollider2DBounds(boxCollider2D);
			}
			Rigidbody2D attachedRigidbody = this.Collider.attachedRigidbody;
			if (attachedRigidbody)
			{
				int attachedColliders = attachedRigidbody.GetAttachedColliders(Sweep.buffer);
				for (int i = 0; i < attachedColliders; i++)
				{
					Collider2D collider2D = Sweep.buffer[i];
					if (!collider2D.isTrigger && collider2D.enabled)
					{
						Sweep.buffer.Clear();
						return collider2D.bounds;
					}
				}
				Sweep.buffer.Clear();
			}
		}
		return this.Collider.bounds;
	}

	// Token: 0x06001C0A RID: 7178 RVA: 0x00082A3C File Offset: 0x00080C3C
	private static Bounds CalculateBoxCollider2DBounds(BoxCollider2D boxCollider)
	{
		Transform transform = boxCollider.transform;
		Vector2 offset = boxCollider.offset;
		Vector2 vector = boxCollider.size * 0.5f;
		Vector2[] array = new Vector2[]
		{
			offset + new Vector2(-vector.x, -vector.y),
			offset + new Vector2(vector.x, -vector.y),
			offset + new Vector2(vector.x, vector.y),
			offset + new Vector2(-vector.x, vector.y)
		};
		Vector3[] array2 = new Vector3[4];
		for (int i = 0; i < 4; i++)
		{
			array2[i] = transform.TransformPoint(array[i]);
		}
		Vector3 vector2 = array2[0];
		Vector3 vector3 = array2[0];
		foreach (Vector3 rhs in array2)
		{
			vector2 = Vector3.Min(vector2, rhs);
			vector3 = Vector3.Max(vector3, rhs);
		}
		Bounds result = default(Bounds);
		result.SetMinMax(vector2, vector3);
		return result;
	}

	// Token: 0x04001B04 RID: 6916
	public Vector2 Direction;

	// Token: 0x04001B05 RID: 6917
	public Collider2D Collider;

	// Token: 0x04001B06 RID: 6918
	public float SkinThickness;

	// Token: 0x04001B07 RID: 6919
	public float SkinWideThickness;

	// Token: 0x04001B08 RID: 6920
	public int RayCount;

	// Token: 0x04001B09 RID: 6921
	private const float DefaultSkinThickness = 0.1f;

	// Token: 0x04001B0A RID: 6922
	private const float DefaultSkinWideThickness = 0.01f;

	// Token: 0x04001B0B RID: 6923
	public const int DefaultRayCount = 3;

	// Token: 0x04001B0C RID: 6924
	private static List<Collider2D> buffer = new List<Collider2D>();
}
