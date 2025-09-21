using System;
using UnityEngine;

// Token: 0x0200053B RID: 1339
public class RepositionFromWalls : MonoBehaviour
{
	// Token: 0x0600300D RID: 12301 RVA: 0x000D3C88 File Offset: 0x000D1E88
	private void OnDrawGizmosSelected()
	{
		if (!this.collider)
		{
			return;
		}
		float num;
		float num2;
		float num3;
		float num4;
		this.GetSideRaysPenetration(out num, out num2, out num3, out num4, true);
		this.GetCornerRaysPenetration(out num, out num2, out num3, out num4, true);
	}

	// Token: 0x0600300E RID: 12302 RVA: 0x000D3CC1 File Offset: 0x000D1EC1
	private void OnEnable()
	{
		this.Reposition();
		this.previousPosition = base.transform.position;
		this.previousRotation = base.transform.eulerAngles.z;
	}

	// Token: 0x0600300F RID: 12303 RVA: 0x000D3CF8 File Offset: 0x000D1EF8
	private void Update()
	{
		if (!this.everyFrame)
		{
			return;
		}
		if ((base.transform.position - this.previousPosition).magnitude > 0.01f || Mathf.Abs(base.transform.eulerAngles.z - this.previousRotation) > 0.1f)
		{
			this.Reposition();
			this.previousPosition = base.transform.position;
		}
	}

	// Token: 0x06003010 RID: 12304 RVA: 0x000D3D78 File Offset: 0x000D1F78
	private void Reposition()
	{
		if (!this.collider || !this.collider.enabled)
		{
			return;
		}
		float distanceInsideTop;
		float distanceInsideBottom;
		float distanceInsideRight;
		float distanceInsideLeft;
		this.GetCornerRaysPenetration(out distanceInsideTop, out distanceInsideBottom, out distanceInsideRight, out distanceInsideLeft, false);
		this.DoMove(distanceInsideTop, distanceInsideBottom, distanceInsideRight, distanceInsideLeft);
		this.GetSideRaysPenetration(out distanceInsideTop, out distanceInsideBottom, out distanceInsideRight, out distanceInsideLeft, false);
		this.DoMove(distanceInsideTop, distanceInsideBottom, distanceInsideRight, distanceInsideLeft);
	}

	// Token: 0x06003011 RID: 12305 RVA: 0x000D3DD4 File Offset: 0x000D1FD4
	private void DoMove(float distanceInsideTop, float distanceInsideBottom, float distanceInsideRight, float distanceInsideLeft)
	{
		Vector3 position = base.transform.position;
		if (distanceInsideTop < Mathf.Epsilon || distanceInsideBottom < Mathf.Epsilon)
		{
			position.y -= distanceInsideTop;
			position.y += distanceInsideBottom;
		}
		if (distanceInsideRight < Mathf.Epsilon || distanceInsideLeft < Mathf.Epsilon)
		{
			position.x -= distanceInsideRight;
			position.x += distanceInsideLeft;
		}
		base.transform.position = position;
	}

	// Token: 0x06003012 RID: 12306 RVA: 0x000D3E4C File Offset: 0x000D204C
	private void GetSideRaysPenetration(out float distanceInsideTop, out float distanceInsideBottom, out float distanceInsideRight, out float distanceInsideLeft, bool isDrawingGizmos)
	{
		Bounds bounds = this.collider.bounds;
		Vector3 center = bounds.center;
		Vector3 size = bounds.size;
		if (isDrawingGizmos)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(center, size);
		}
		float x = size.x;
		float y = size.y;
		Vector2 up = Vector2.up;
		float distance = y * 0.5f + this.rayLengthFromEdge;
		Vector2 down = Vector2.down;
		Vector2 right = Vector2.right;
		float distance2 = x * 0.5f + this.rayLengthFromEdge;
		Vector2 left = Vector2.left;
		distanceInsideTop = this.GetRaycastPenetration(center, up, distance, x, isDrawingGizmos);
		distanceInsideBottom = this.GetRaycastPenetration(center, down, distance, x, isDrawingGizmos);
		distanceInsideRight = this.GetRaycastPenetration(center, right, distance2, y, isDrawingGizmos);
		distanceInsideLeft = this.GetRaycastPenetration(center, left, distance2, y, isDrawingGizmos);
	}

	// Token: 0x06003013 RID: 12307 RVA: 0x000D3F30 File Offset: 0x000D2130
	private void GetCornerRaysPenetration(out float distanceInsideTop, out float distanceInsideBottom, out float distanceInsideRight, out float distanceInsideLeft, bool isDrawingGizmos)
	{
		Bounds bounds = this.collider.bounds;
		Vector3 max = bounds.max;
		Vector3 min = bounds.min;
		Vector2 normalized = new Vector2(1f, 1f).normalized;
		Vector2 normalized2 = new Vector2(-1f, 1f).normalized;
		Vector2 normalized3 = new Vector2(-1f, -1f).normalized;
		Vector2 normalized4 = new Vector2(1f, -1f).normalized;
		float raycastPenetration = this.GetRaycastPenetration(max, normalized, this.rayLengthFromEdge, 0f, isDrawingGizmos);
		float raycastPenetration2 = this.GetRaycastPenetration(new Vector2(min.x, max.y), normalized2, this.rayLengthFromEdge, 0f, isDrawingGizmos);
		float raycastPenetration3 = this.GetRaycastPenetration(min, normalized3, this.rayLengthFromEdge, 0f, isDrawingGizmos);
		float raycastPenetration4 = this.GetRaycastPenetration(new Vector2(max.x, min.y), normalized4, this.rayLengthFromEdge, 0f, isDrawingGizmos);
		Vector2 vector = normalized * raycastPenetration;
		Vector2 vector2 = normalized2 * raycastPenetration2;
		Vector2 vector3 = normalized3 * raycastPenetration3;
		Vector2 vector4 = normalized4 * raycastPenetration4;
		distanceInsideTop = Mathf.Max(Mathf.Abs(vector.y), Mathf.Abs(vector2.y));
		distanceInsideBottom = Mathf.Max(Mathf.Abs(vector4.y), Mathf.Abs(vector3.y));
		distanceInsideRight = Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector4.x));
		distanceInsideLeft = Mathf.Max(Mathf.Abs(vector2.x), Mathf.Abs(vector3.x));
	}

	// Token: 0x06003014 RID: 12308 RVA: 0x000D4110 File Offset: 0x000D2310
	private float GetRaycastPenetration(Vector2 origin, Vector2 direction, float distance, float width, bool isDrawingGizmos)
	{
		int num = (width > 0f) ? Mathf.Max(Mathf.FloorToInt((float)this.raysPerUnitWidth * width), 1) : 1;
		int num2 = num - 1;
		Vector2 b = new Vector2(direction.y, -direction.x) * 0.5f * width;
		Vector2 a = origin - b;
		Vector2 b2 = origin + b;
		float num3 = distance;
		for (int i = 0; i < num; i++)
		{
			Vector2 vector = (num2 > 0) ? Vector2.Lerp(a, b2, (float)i / (float)num2) : origin;
			RaycastHit2D raycastHit2D = Helper.Raycast2D(vector, direction, distance, 256);
			bool flag = raycastHit2D.collider != null;
			if (isDrawingGizmos)
			{
				Gizmos.color = (flag ? new Color(1f, 0f, 0f, 0.5f) : Color.yellow);
				Gizmos.DrawLine(vector, vector + direction * distance);
				if (flag)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(vector, vector + direction * raycastHit2D.distance);
				}
			}
			if (flag)
			{
				num3 = Mathf.Min(num3, raycastHit2D.distance);
			}
		}
		return distance - num3;
	}

	// Token: 0x040032EE RID: 13038
	[SerializeField]
	private Collider2D collider;

	// Token: 0x040032EF RID: 13039
	[SerializeField]
	private float rayLengthFromEdge = 1f;

	// Token: 0x040032F0 RID: 13040
	[SerializeField]
	private int raysPerUnitWidth = 3;

	// Token: 0x040032F1 RID: 13041
	[SerializeField]
	private bool everyFrame;

	// Token: 0x040032F2 RID: 13042
	private Vector2 previousPosition;

	// Token: 0x040032F3 RID: 13043
	private float previousRotation;
}
