using System;
using UnityEngine;

// Token: 0x0200075B RID: 1883
public static class EdgeAdjustHelper
{
	// Token: 0x060042B3 RID: 17075 RVA: 0x00125ED4 File Offset: 0x001240D4
	public static float GetEdgeAdjustX(Collider2D col2d, bool facingRight, float forwardEdgeFudge = 0f, float backwardEdgeFudge = 0f)
	{
		Bounds bounds = col2d.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		if (facingRight)
		{
			max.x -= forwardEdgeFudge;
			min.x += backwardEdgeFudge;
		}
		else
		{
			max.x -= backwardEdgeFudge;
			min.x += forwardEdgeFudge;
		}
		float edgeAdjust = EdgeAdjustHelper.GetEdgeAdjust(min, max, backwardEdgeFudge, facingRight, true);
		if (Math.Abs(edgeAdjust) <= Mathf.Epsilon)
		{
			edgeAdjust = EdgeAdjustHelper.GetEdgeAdjust(min, max, forwardEdgeFudge, facingRight, false);
		}
		return edgeAdjust;
	}

	// Token: 0x060042B4 RID: 17076 RVA: 0x00125F68 File Offset: 0x00124168
	private static float GetEdgeAdjust(Vector2 min, Vector2 max, float edgeFudge, bool facingRight, bool isForward)
	{
		float y = min.y + 0.1f;
		Vector2 vector;
		Vector2 direction;
		if (facingRight == isForward)
		{
			vector = new Vector2(max.x, y);
			direction = Vector2.left;
		}
		else
		{
			vector = new Vector2(min.x, y);
			direction = Vector2.right;
		}
		if (Helper.IsRayHittingNoTriggers(vector, Vector2.down, 0.3f, 256))
		{
			return 0f;
		}
		Vector2 vector2 = vector + Vector2.down * 0.3f;
		float num;
		float num2;
		if (facingRight == isForward)
		{
			num = vector2.x - min.x;
			num2 = -1f;
		}
		else
		{
			num = max.x - vector2.x;
			num2 = 1f;
		}
		num += edgeFudge;
		RaycastHit2D raycastHit2D;
		if (!Helper.IsRayHittingNoTriggers(vector2, direction, num, 256, out raycastHit2D))
		{
			return 0f;
		}
		return raycastHit2D.distance * num2;
	}
}
