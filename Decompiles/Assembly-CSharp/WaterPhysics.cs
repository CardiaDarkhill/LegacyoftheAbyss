using System;
using UnityEngine;

// Token: 0x0200028D RID: 653
public class WaterPhysics : MonoBehaviour
{
	// Token: 0x060016E5 RID: 5861 RVA: 0x0006724A File Offset: 0x0006544A
	private void Awake()
	{
		this.lineRenderer = base.gameObject.GetComponent<LineRenderer>();
	}

	// Token: 0x060016E6 RID: 5862 RVA: 0x00067260 File Offset: 0x00065460
	private void Start()
	{
		Bounds bounds = base.GetComponent<MeshRenderer>().bounds;
		float num = bounds.max.x - bounds.min.x;
		this.top = bounds.max.y;
		this.left = bounds.min.x;
		this.bottom = bounds.min.y;
		this.edgeCount = Mathf.RoundToInt(num) * this.resolution;
		int num2 = this.edgeCount + 1;
		this.lineRenderer.positionCount = num2;
		this.lineRenderer.useWorldSpace = true;
		this.positions = new Vector2[num2];
		this.velocities = new float[num2];
		this.accelerations = new float[num2];
		this.colliders = new GameObject[num2];
		this.mesh = new Mesh();
		this.mesh.name = "WaterPhysicsMesh";
		this.meshObject = new GameObject("Water Mesh");
		this.meshObject.transform.SetParent(base.transform);
		this.meshObject.transform.localPosition = Vector2.zero;
		this.meshObject.AddComponent<MeshFilter>().sharedMesh = this.mesh;
		Material material = base.GetComponent<MeshRenderer>().material;
		this.meshObject.AddComponent<MeshRenderer>().sharedMaterial = material;
		Object.Destroy(base.GetComponent<MeshFilter>());
		Object.Destroy(base.GetComponent<MeshRenderer>());
		for (int i = 0; i < num2; i++)
		{
			this.positions[i] = new Vector2(this.left + num * ((float)i / (float)this.edgeCount), this.top);
			this.accelerations[i] = 0f;
			this.velocities[i] = 0f;
			this.lineRenderer.SetPosition(i, this.positions[i]);
			this.colliders[i] = new GameObject("Trigger");
			this.colliders[i].AddComponent<BoxCollider2D>().isTrigger = true;
			this.colliders[i].transform.position = new Vector3(this.left + num * (float)i / (float)this.edgeCount, this.top - this.detectorHeight / 2f, 0f);
			this.colliders[i].transform.localScale = new Vector3(num / (float)this.edgeCount, this.detectorHeight, 1f);
			this.colliders[i].AddComponent<WaterDetector>();
			this.colliders[i].transform.SetParent(base.transform);
		}
		this.vertices = new Vector3[this.edgeCount * 4];
		Vector2[] array = new Vector2[this.edgeCount * 4];
		int[] array2 = new int[this.edgeCount * 6];
		for (int j = 0; j < this.edgeCount; j++)
		{
			int num3 = j * 4;
			float x = (j == 0) ? 0f : ((float)(j - 1) / (float)this.edgeCount);
			float x2 = (float)j / (float)this.edgeCount;
			array[num3] = new Vector2(x, 1f);
			array[num3 + 1] = new Vector2(x2, 1f);
			array[num3 + 2] = new Vector2(x, 0f);
			array[num3 + 3] = new Vector2(x2, 0f);
			int num4 = j * 6;
			array2[num4] = num3;
			array2[num4 + 1] = num3 + 1;
			array2[num4 + 2] = num3 + 3;
			array2[num4 + 3] = num3 + 3;
			array2[num4 + 4] = num3 + 2;
			array2[num4 + 5] = num3;
		}
		this.UpdateVertexPositions();
		this.mesh.uv = array;
		this.mesh.triangles = array2;
		this.mesh.RecalculateBounds();
		this.mesh.RecalculateNormals();
		if (this.reflections)
		{
			float orthographicSize = base.transform.localScale.y / 2f;
			this.cameraObj = new GameObject("WaterCamera");
			this.cameraObj.transform.SetParent(base.transform);
			this.cameraObj.transform.localScale = Vector3.one;
			this.cameraObj.transform.localPosition = new Vector3(0f, 1f, this.zOffset);
			Camera camera = this.cameraObj.AddComponent<Camera>();
			camera.orthographic = true;
			camera.orthographicSize = orthographicSize;
			camera.nearClipPlane = 0f;
			camera.farClipPlane = this.depth;
			int num5 = this.reflectionResolution;
			int width = Mathf.RoundToInt((float)num5 * (base.transform.localScale.x / base.transform.localScale.y));
			this.texture = new RenderTexture(width, num5, 32, RenderTextureFormat.ARGB32);
			this.texture.name = "WaterPhysics" + base.GetInstanceID().ToString();
			camera.targetTexture = this.texture;
			material.SetTexture("_ReflectionTex", this.texture);
			this.cameraObj.SetActive(false);
		}
	}

	// Token: 0x060016E7 RID: 5863 RVA: 0x000677A0 File Offset: 0x000659A0
	private void OnDestroy()
	{
		if (this.texture != null)
		{
			this.texture.Release();
			Object.Destroy(this.texture);
			this.texture = null;
		}
		if (this.mesh)
		{
			Object.Destroy(this.mesh);
			this.mesh = null;
		}
	}

	// Token: 0x060016E8 RID: 5864 RVA: 0x000677F7 File Offset: 0x000659F7
	private void Update()
	{
		this.UpdateVertexPositions();
	}

	// Token: 0x060016E9 RID: 5865 RVA: 0x00067800 File Offset: 0x00065A00
	private void FixedUpdate()
	{
		for (int i = 0; i < this.positions.Length; i++)
		{
			float num = this.spring * (this.positions[i].y - this.top) + this.velocities[i] * this.damping;
			this.accelerations[i] = -num;
			Vector2[] array = this.positions;
			int num2 = i;
			array[num2].y = array[num2].y + this.velocities[i];
			this.velocities[i] += this.accelerations[i];
			this.lineRenderer.SetPosition(i, new Vector3(this.positions[i].x, this.positions[i].y, base.transform.position.z + this.lineZ));
		}
		float[] array2 = new float[this.positions.Length];
		float[] array3 = new float[this.positions.Length];
		for (int j = 0; j < 8; j++)
		{
			for (int k = 0; k < this.positions.Length; k++)
			{
				if (k > 0)
				{
					array2[k] = this.spread * (this.positions[k].y - this.positions[k - 1].y);
					this.velocities[k - 1] += array2[k];
				}
				if (k < this.positions.Length - 1)
				{
					array3[k] = this.spread * (this.positions[k].y - this.positions[k + 1].y);
					this.velocities[k + 1] += array3[k];
				}
			}
		}
		for (int l = 0; l < this.positions.Length; l++)
		{
			if (l > 0)
			{
				Vector2[] array4 = this.positions;
				int num3 = l - 1;
				array4[num3].y = array4[num3].y + array2[l];
			}
			if (l < this.positions.Length - 1)
			{
				Vector2[] array5 = this.positions;
				int num4 = l + 1;
				array5[num4].y = array5[num4].y + array3[l];
			}
		}
	}

	// Token: 0x060016EA RID: 5866 RVA: 0x00067A38 File Offset: 0x00065C38
	private void UpdateVertexPositions()
	{
		for (int i = 0; i < this.edgeCount; i++)
		{
			int num = i * 4;
			this.vertices[num] = new Vector2(this.positions[i].x, this.positions[i].y) - this.meshObject.transform.position;
			this.vertices[num + 1] = new Vector2(this.positions[i + 1].x, this.positions[i + 1].y) - this.meshObject.transform.position;
			this.vertices[num + 2] = new Vector2(this.positions[i].x, this.bottom) - this.meshObject.transform.position;
			this.vertices[num + 3] = new Vector2(this.positions[i + 1].x, this.bottom) - this.meshObject.transform.position;
		}
		this.mesh.vertices = this.vertices;
	}

	// Token: 0x060016EB RID: 5867 RVA: 0x00067BB0 File Offset: 0x00065DB0
	public void Splash(float xPos, float velocity)
	{
		if (xPos >= this.positions[0].x && xPos <= this.positions[this.positions.Length - 1].x)
		{
			xPos -= this.positions[0].x;
			int num = Mathf.RoundToInt((float)(this.positions.Length - 1) * (xPos / (this.positions[this.positions.Length - 1].x - this.positions[0].x)));
			float num2 = this.velocities[num];
			this.velocities[num] = velocity * ((velocity > 0f) ? this.velocityUpMultiplier : this.velocityDownMultiplier);
		}
	}

	// Token: 0x0400156B RID: 5483
	public float spring = 0.02f;

	// Token: 0x0400156C RID: 5484
	public float damping = 0.04f;

	// Token: 0x0400156D RID: 5485
	public float spread = 0.05f;

	// Token: 0x0400156E RID: 5486
	[Space]
	public float detectorHeight = 2f;

	// Token: 0x0400156F RID: 5487
	[Space]
	public float velocityUpMultiplier = 1f;

	// Token: 0x04001570 RID: 5488
	public float velocityDownMultiplier = 1f;

	// Token: 0x04001571 RID: 5489
	[Space]
	[Tooltip("Amount of nodes per unit width.")]
	public int resolution = 5;

	// Token: 0x04001572 RID: 5490
	public float lineZ = -0.1f;

	// Token: 0x04001573 RID: 5491
	[Header("Reflections")]
	public bool reflections;

	// Token: 0x04001574 RID: 5492
	public float zOffset = -5f;

	// Token: 0x04001575 RID: 5493
	public float depth = 10f;

	// Token: 0x04001576 RID: 5494
	[Space]
	public int reflectionResolution = 256;

	// Token: 0x04001577 RID: 5495
	private GameObject cameraObj;

	// Token: 0x04001578 RID: 5496
	private int edgeCount;

	// Token: 0x04001579 RID: 5497
	private Vector2[] positions;

	// Token: 0x0400157A RID: 5498
	private float[] velocities;

	// Token: 0x0400157B RID: 5499
	private float[] accelerations;

	// Token: 0x0400157C RID: 5500
	private GameObject meshObject;

	// Token: 0x0400157D RID: 5501
	private Mesh mesh;

	// Token: 0x0400157E RID: 5502
	private Vector3[] vertices;

	// Token: 0x0400157F RID: 5503
	private float top;

	// Token: 0x04001580 RID: 5504
	private float left;

	// Token: 0x04001581 RID: 5505
	private float bottom;

	// Token: 0x04001582 RID: 5506
	private GameObject[] colliders;

	// Token: 0x04001583 RID: 5507
	private LineRenderer lineRenderer;

	// Token: 0x04001584 RID: 5508
	private RenderTexture texture;

	// Token: 0x0200155F RID: 5471
	public enum SplashDirection
	{
		// Token: 0x040086EA RID: 34538
		Down,
		// Token: 0x040086EB RID: 34539
		Up,
		// Token: 0x040086EC RID: 34540
		Right,
		// Token: 0x040086ED RID: 34541
		Left
	}
}
