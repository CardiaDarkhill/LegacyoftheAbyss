using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class DebugDrawColliderRuntime : MonoBehaviour
{
	// Token: 0x1700022F RID: 559
	// (get) Token: 0x0600137E RID: 4990 RVA: 0x00058CA3 File Offset: 0x00056EA3
	// (set) Token: 0x0600137F RID: 4991 RVA: 0x00058CAC File Offset: 0x00056EAC
	public static bool IsShowing
	{
		get
		{
			return DebugDrawColliderRuntime._isShowing;
		}
		set
		{
			DebugDrawColliderRuntime._isShowing = value;
			if (!DebugDrawColliderRuntime._isShowing)
			{
				return;
			}
			foreach (DebugDrawColliderRuntime debugDrawColliderRuntime in DebugDrawColliderRuntime._actives)
			{
				debugDrawColliderRuntime.Init();
			}
		}
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x00058D0C File Offset: 0x00056F0C
	private void OnEnable()
	{
		DebugDrawColliderRuntime._actives.Add(this);
		if (DebugDrawColliderRuntime.IsShowing)
		{
			this.Init();
		}
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x00058D26 File Offset: 0x00056F26
	private void OnDisable()
	{
		DebugDrawColliderRuntime._actives.Remove(this);
		this.DeInit();
	}

	// Token: 0x06001382 RID: 4994 RVA: 0x00058D3C File Offset: 0x00056F3C
	private void Init()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.isInitialized = true;
		this.damageHero = base.GetComponent<DamageHero>();
		this.damageEnemies = base.GetComponent<DamageEnemies>();
		CameraRenderHooks.CameraPostRender += this.OnPostRenderCamera;
		this.material = new Material(Shader.Find("Sprites/Default"));
		this.GetColliders();
	}

	// Token: 0x06001383 RID: 4995 RVA: 0x00058D9D File Offset: 0x00056F9D
	private void GetColliders()
	{
		this.edgeCollider2Ds = base.GetComponents<EdgeCollider2D>();
		this.polygonCollider2Ds = base.GetComponents<PolygonCollider2D>();
		this.boxCollider2Ds = base.GetComponents<BoxCollider2D>();
		this.circleCollider2Ds = base.GetComponents<CircleCollider2D>();
	}

	// Token: 0x06001384 RID: 4996 RVA: 0x00058DD0 File Offset: 0x00056FD0
	private void DeInit()
	{
		if (!this.isInitialized)
		{
			return;
		}
		this.isInitialized = false;
		CameraRenderHooks.CameraPostRender -= this.OnPostRenderCamera;
		if (this.material != null)
		{
			Object.Destroy(this.material);
			this.material = null;
		}
		this.edgeCollider2Ds = null;
		this.polygonCollider2Ds = null;
		this.boxCollider2Ds = null;
		this.currentBoxColliderPoints = null;
	}

	// Token: 0x06001385 RID: 4997 RVA: 0x00058E3C File Offset: 0x0005703C
	private void OnPostRenderCamera(CameraRenderHooks.CameraSource source)
	{
		if (source != CameraRenderHooks.CameraSource.MainCamera)
		{
			return;
		}
		if (!DebugDrawColliderRuntime.IsShowing)
		{
			return;
		}
		if (!this.damageEnemies && this.damageHero && this.damageHero.damageDealt <= 0)
		{
			return;
		}
		GL.PushMatrix();
		this.material.SetPass(0);
		foreach (EdgeCollider2D edgeCollider2D in this.edgeCollider2Ds)
		{
			if (edgeCollider2D.enabled)
			{
				this.DrawLines(edgeCollider2D.offset, edgeCollider2D.points);
			}
		}
		foreach (PolygonCollider2D polygonCollider2D in this.polygonCollider2Ds)
		{
			if (polygonCollider2D.enabled)
			{
				this.DrawLines(polygonCollider2D.offset, polygonCollider2D.points);
			}
		}
		foreach (BoxCollider2D boxCollider2D in this.boxCollider2Ds)
		{
			if (boxCollider2D.enabled)
			{
				Vector2 size = boxCollider2D.size;
				Vector2 offset = boxCollider2D.offset;
				Vector2 b = size * 0.5f;
				Vector2 vector = offset - b;
				Vector2 vector2 = offset + b;
				if (this.currentBoxColliderPoints == null)
				{
					this.currentBoxColliderPoints = new Vector2[4];
				}
				this.currentBoxColliderPoints[0] = vector;
				this.currentBoxColliderPoints[1] = new Vector2(vector.x, vector2.y);
				this.currentBoxColliderPoints[2] = vector2;
				this.currentBoxColliderPoints[3] = new Vector2(vector2.x, vector.y);
				this.DrawLines(Vector2.zero, this.currentBoxColliderPoints);
			}
		}
		foreach (CircleCollider2D circleCollider2D in this.circleCollider2Ds)
		{
			if (circleCollider2D.enabled)
			{
				this.DrawCircle(circleCollider2D.offset, circleCollider2D.radius);
			}
		}
		GL.PopMatrix();
	}

	// Token: 0x06001386 RID: 4998 RVA: 0x0005901C File Offset: 0x0005721C
	private void SetGlColour()
	{
		switch (this.type)
		{
		case DebugDrawColliderRuntime.ColorType.Tilemap:
			GL.Color(new Color(0f, 0.44f, 0f));
			return;
		case DebugDrawColliderRuntime.ColorType.TerrainCollider:
			GL.Color(Color.green);
			return;
		case DebugDrawColliderRuntime.ColorType.Danger:
			GL.Color(Color.red);
			return;
		case DebugDrawColliderRuntime.ColorType.Roof:
			GL.Color(new Color(0.8f, 1f, 0f));
			return;
		case DebugDrawColliderRuntime.ColorType.Region:
			GL.Color(new Color(0.4f, 0.75f, 1f));
			return;
		case DebugDrawColliderRuntime.ColorType.Enemy:
			GL.Color(new Color(1f, 0.7f, 0f));
			return;
		case DebugDrawColliderRuntime.ColorType.Water:
			GL.Color(new Color(0.2f, 0.5f, 1f));
			return;
		case DebugDrawColliderRuntime.ColorType.TransitionPoint:
			GL.Color(Color.magenta);
			return;
		case DebugDrawColliderRuntime.ColorType.SandRegion:
			GL.Color(new Color(1f, 0.7f, 0.7f));
			return;
		case DebugDrawColliderRuntime.ColorType.ShardRegion:
			GL.Color(Color.grey);
			return;
		case DebugDrawColliderRuntime.ColorType.CameraLock:
			GL.Color(new Color(0.16f, 0.17f, 0.28f));
			return;
		default:
			GL.Color(Color.white);
			return;
		}
	}

	// Token: 0x06001387 RID: 4999 RVA: 0x00059154 File Offset: 0x00057354
	private void DrawLines(Vector2 offset, Vector2[] points)
	{
		GL.Begin(1);
		this.SetGlColour();
		for (int i = 0; i < points.Length; i++)
		{
			GL.Vertex(base.transform.TransformPoint(points[i] + offset));
			GL.Vertex((i == 0) ? base.transform.TransformPoint(points[points.Length - 1] + offset) : base.transform.TransformPoint(points[i - 1] + offset));
		}
		GL.End();
	}

	// Token: 0x06001388 RID: 5000 RVA: 0x00059208 File Offset: 0x00057408
	private void DrawCircle(Vector2 offset, float radius)
	{
		DebugDrawColliderRuntime.<>c__DisplayClass24_0 CS$<>8__locals1;
		CS$<>8__locals1.radius = radius;
		CS$<>8__locals1.offset = offset;
		GL.Begin(1);
		GL.PushMatrix();
		GL.MultMatrix(base.transform.localToWorldMatrix);
		this.SetGlColour();
		Vector3 lossyScale = base.transform.lossyScale;
		CS$<>8__locals1.points = Mathf.RoundToInt(Mathf.Log(CS$<>8__locals1.radius * Mathf.Max(lossyScale.x, lossyScale.y) * 100f) * 10f);
		for (int i = 0; i < CS$<>8__locals1.points; i++)
		{
			DebugDrawColliderRuntime.<DrawCircle>g__DrawCircleSection|24_0(i, ref CS$<>8__locals1);
			if (i == 0)
			{
				DebugDrawColliderRuntime.<DrawCircle>g__DrawCircleSection|24_0(CS$<>8__locals1.points - 1, ref CS$<>8__locals1);
			}
			else
			{
				DebugDrawColliderRuntime.<DrawCircle>g__DrawCircleSection|24_0(i - 1, ref CS$<>8__locals1);
			}
		}
		GL.End();
		GL.PopMatrix();
	}

	// Token: 0x06001389 RID: 5001 RVA: 0x000592C8 File Offset: 0x000574C8
	public static void AddOrUpdate(GameObject gameObject, DebugDrawColliderRuntime.ColorType type = DebugDrawColliderRuntime.ColorType.None, bool forceAdd = false)
	{
		if (!DebugDrawColliderRuntime._isShowing && !forceAdd)
		{
			return;
		}
		DebugDrawColliderRuntime component = gameObject.GetComponent<DebugDrawColliderRuntime>();
		if (component)
		{
			component.GetColliders();
			return;
		}
		if (type == DebugDrawColliderRuntime.ColorType.None)
		{
			Object component2 = gameObject.GetComponent<HealthManager>();
			DamageHero component3 = gameObject.GetComponent<DamageHero>();
			if (component2 || component3)
			{
				type = DebugDrawColliderRuntime.ColorType.Enemy;
			}
		}
		gameObject.AddComponent<DebugDrawColliderRuntime>().type = type;
	}

	// Token: 0x0600138C RID: 5004 RVA: 0x0005933C File Offset: 0x0005753C
	[CompilerGenerated]
	internal static void <DrawCircle>g__DrawCircleSection|24_0(int i, ref DebugDrawColliderRuntime.<>c__DisplayClass24_0 A_1)
	{
		float f = (float)i / (float)A_1.points * 3.1415927f * 2f;
		Vector3 b = new Vector3(Mathf.Cos(f) * A_1.radius, Mathf.Sin(f) * A_1.radius, 0f);
		GL.Vertex(A_1.offset.ToVector3(0f) + b);
	}

	// Token: 0x040011E7 RID: 4583
	[SerializeField]
	private DebugDrawColliderRuntime.ColorType type;

	// Token: 0x040011E8 RID: 4584
	private bool isInitialized;

	// Token: 0x040011E9 RID: 4585
	private Material material;

	// Token: 0x040011EA RID: 4586
	private EdgeCollider2D[] edgeCollider2Ds;

	// Token: 0x040011EB RID: 4587
	private PolygonCollider2D[] polygonCollider2Ds;

	// Token: 0x040011EC RID: 4588
	private BoxCollider2D[] boxCollider2Ds;

	// Token: 0x040011ED RID: 4589
	private CircleCollider2D[] circleCollider2Ds;

	// Token: 0x040011EE RID: 4590
	private Vector2[] currentBoxColliderPoints;

	// Token: 0x040011EF RID: 4591
	private DamageHero damageHero;

	// Token: 0x040011F0 RID: 4592
	private DamageEnemies damageEnemies;

	// Token: 0x040011F1 RID: 4593
	private static readonly List<DebugDrawColliderRuntime> _actives = new List<DebugDrawColliderRuntime>();

	// Token: 0x040011F2 RID: 4594
	private static bool _isShowing;

	// Token: 0x02001534 RID: 5428
	public enum ColorType
	{
		// Token: 0x04008643 RID: 34371
		None = -1,
		// Token: 0x04008644 RID: 34372
		Tilemap,
		// Token: 0x04008645 RID: 34373
		TerrainCollider,
		// Token: 0x04008646 RID: 34374
		Danger,
		// Token: 0x04008647 RID: 34375
		Roof,
		// Token: 0x04008648 RID: 34376
		Region,
		// Token: 0x04008649 RID: 34377
		Enemy,
		// Token: 0x0400864A RID: 34378
		Water,
		// Token: 0x0400864B RID: 34379
		TransitionPoint,
		// Token: 0x0400864C RID: 34380
		SandRegion,
		// Token: 0x0400864D RID: 34381
		ShardRegion,
		// Token: 0x0400864E RID: 34382
		CameraLock
	}
}
