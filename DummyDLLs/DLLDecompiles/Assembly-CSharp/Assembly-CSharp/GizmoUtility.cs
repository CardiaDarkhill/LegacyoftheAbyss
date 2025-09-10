using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000767 RID: 1895
public static class GizmoUtility
{
	// Token: 0x06004393 RID: 17299 RVA: 0x001291C2 File Offset: 0x001273C2
	public static bool IsChildSelected(Transform transform)
	{
		return false;
	}

	// Token: 0x06004394 RID: 17300 RVA: 0x001291C5 File Offset: 0x001273C5
	public static bool IsTargetSelected(Transform target)
	{
		return false;
	}

	// Token: 0x06004395 RID: 17301 RVA: 0x001291C8 File Offset: 0x001273C8
	public static bool IsSelfOrChildSelected(Transform transform)
	{
		return GizmoUtility.IsTargetSelected(transform) || GizmoUtility.IsChildSelected(transform);
	}

	// Token: 0x06004396 RID: 17302 RVA: 0x001291DA File Offset: 0x001273DA
	public static void DrawSceneLabel(Vector3 position, string label, int fontSize = 30, GUIStyle style = null)
	{
	}

	// Token: 0x06004397 RID: 17303 RVA: 0x001291DC File Offset: 0x001273DC
	public static void DrawCollider2D(Collider2D collider2D)
	{
		if (!collider2D)
		{
			return;
		}
		EdgeCollider2D edgeCollider2D = collider2D as EdgeCollider2D;
		if (edgeCollider2D != null)
		{
			GizmoUtility.DrawEdgeCollider2D(edgeCollider2D);
			return;
		}
		BoxCollider2D boxCollider2D = collider2D as BoxCollider2D;
		if (boxCollider2D != null)
		{
			GizmoUtility.DrawBoxCollider2D(boxCollider2D);
			return;
		}
		PolygonCollider2D polygonCollider2D = collider2D as PolygonCollider2D;
		if (polygonCollider2D != null)
		{
			GizmoUtility.DrawPolygonCollider2D(polygonCollider2D);
			return;
		}
		CircleCollider2D circleCollider2D = collider2D as CircleCollider2D;
		if (circleCollider2D == null)
		{
			string text = string.Format("Draw method for {0} not implemented", collider2D);
			if (GizmoUtility.errors.Add(text))
			{
				Debug.LogError(text);
			}
			return;
		}
		GizmoUtility.DrawCircleCollider2D(circleCollider2D);
	}

	// Token: 0x06004398 RID: 17304 RVA: 0x0012925A File Offset: 0x0012745A
	private static Matrix4x4 UpdateMatrix(Collider2D collider2D)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = collider2D.transform.localToWorldMatrix;
		return matrix;
	}

	// Token: 0x06004399 RID: 17305 RVA: 0x00129271 File Offset: 0x00127471
	private static Matrix4x4 UpdateMatrix(Transform transform)
	{
		Matrix4x4 matrix = Gizmos.matrix;
		Gizmos.matrix = transform.localToWorldMatrix;
		return matrix;
	}

	// Token: 0x0600439A RID: 17306 RVA: 0x00129283 File Offset: 0x00127483
	private static void DrawCircleCollider2D(CircleCollider2D circleCollider2D)
	{
		Matrix4x4 matrix = GizmoUtility.UpdateMatrix(circleCollider2D);
		Gizmos.DrawWireSphere(circleCollider2D.offset, circleCollider2D.radius);
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600439B RID: 17307 RVA: 0x001292A8 File Offset: 0x001274A8
	private static void DrawPolygonCollider2D(PolygonCollider2D polygonCollider2D)
	{
		Matrix4x4 matrix = GizmoUtility.UpdateMatrix(polygonCollider2D);
		Vector2[] points = polygonCollider2D.points;
		Vector2 offset = polygonCollider2D.offset;
		for (int i = 0; i < points.Length - 1; i++)
		{
			Vector2 v = points[i] + offset;
			Vector2 v2 = points[i + 1] + offset;
			Gizmos.DrawLine(v, v2);
		}
		if (points.Length > 2)
		{
			Vector2 v3 = points[points.Length - 1] + offset;
			Vector2 v4 = points[0] + offset;
			Gizmos.DrawLine(v3, v4);
		}
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600439C RID: 17308 RVA: 0x00129348 File Offset: 0x00127548
	public static void DrawPolygonPointList(Transform transform, List<Vector2> points)
	{
		Matrix4x4 matrix = GizmoUtility.UpdateMatrix(transform);
		for (int i = 0; i < points.Count - 1; i++)
		{
			Vector2 v = points[i];
			Vector2 v2 = points[i + 1];
			Gizmos.DrawLine(v, v2);
		}
		if (points.Count > 2)
		{
			Vector2 v3 = points[points.Count - 1];
			Vector2 v4 = points[0];
			Gizmos.DrawLine(v3, v4);
		}
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600439D RID: 17309 RVA: 0x001293C5 File Offset: 0x001275C5
	private static void DrawBoxCollider2D(BoxCollider2D boxCollider2D)
	{
		Matrix4x4 matrix = GizmoUtility.UpdateMatrix(boxCollider2D);
		Gizmos.DrawWireCube(boxCollider2D.offset, boxCollider2D.size);
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600439E RID: 17310 RVA: 0x001293F0 File Offset: 0x001275F0
	private static void DrawEdgeCollider2D(EdgeCollider2D edgeCollider2D)
	{
		Matrix4x4 matrix = GizmoUtility.UpdateMatrix(edgeCollider2D);
		Vector2[] points = edgeCollider2D.points;
		Vector2 offset = edgeCollider2D.offset;
		for (int i = 0; i < points.Length - 1; i++)
		{
			Vector2 v = points[i] + offset;
			Vector2 v2 = points[i + 1] + offset;
			Gizmos.DrawLine(v, v2);
		}
		Gizmos.matrix = matrix;
	}

	// Token: 0x0600439F RID: 17311 RVA: 0x00129457 File Offset: 0x00127657
	public static void DrawWireCircle(Vector3 position, float radius)
	{
	}

	// Token: 0x04004520 RID: 17696
	private static HashSet<string> errors = new HashSet<string>();

	// Token: 0x04004521 RID: 17697
	private static ConditionalWeakTable<PolygonCollider2D, GizmoUtility.MeshHelper> polyMeshMap = new ConditionalWeakTable<PolygonCollider2D, GizmoUtility.MeshHelper>();

	// Token: 0x02001A39 RID: 6713
	private class MeshHelper
	{
		// Token: 0x0600964F RID: 38479 RVA: 0x002A7EAC File Offset: 0x002A60AC
		public MeshHelper()
		{
			this.mesh = new Mesh();
			this.mesh.name = "GizmoUtilityMesh";
			this.mesh.hideFlags = (this.mesh.hideFlags |= HideFlags.DontSave);
		}

		// Token: 0x06009650 RID: 38480 RVA: 0x002A7EFC File Offset: 0x002A60FC
		~MeshHelper()
		{
			if (this.mesh != null)
			{
				Object.Destroy(this.mesh);
			}
		}

		// Token: 0x04009921 RID: 39201
		public Mesh mesh;

		// Token: 0x04009922 RID: 39202
		public uint drawCalls;
	}
}
