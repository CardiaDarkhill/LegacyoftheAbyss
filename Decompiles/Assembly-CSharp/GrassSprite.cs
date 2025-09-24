using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x0200023D RID: 573
[ExecuteInEditMode]
[DisallowMultipleComponent]
public class GrassSprite : MonoBehaviour
{
	// Token: 0x060014FC RID: 5372 RVA: 0x0005EF95 File Offset: 0x0005D195
	private void OnValidate()
	{
		this.UpdateMesh();
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x0005EF9D File Offset: 0x0005D19D
	private void Awake()
	{
		this.Upgrade();
		this.UpdateMesh();
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x0005EFAB File Offset: 0x0005D1AB
	private void OnEnable()
	{
		if (base.enabled)
		{
			this.meshRenderer.enabled = true;
		}
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x0005EFC1 File Offset: 0x0005D1C1
	private void OnDisable()
	{
		if (!base.enabled)
		{
			this.meshRenderer.enabled = false;
		}
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x0005EFD7 File Offset: 0x0005D1D7
	private void OnDestroy()
	{
		if (this.mesh)
		{
			Object.DestroyImmediate(this.mesh);
		}
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x0005EFF4 File Offset: 0x0005D1F4
	public void Upgrade()
	{
		int sortingLayerID;
		int sortingOrder;
		if (this.meshRenderer)
		{
			sortingLayerID = this.meshRenderer.sortingLayerID;
			sortingOrder = this.meshRenderer.sortingOrder;
		}
		else
		{
			sortingLayerID = 0;
			sortingOrder = 0;
		}
		SpriteRenderer component = base.GetComponent<SpriteRenderer>();
		if (component)
		{
			this.sprite = component.sprite;
			this.color = component.color;
			sortingLayerID = component.sortingLayerID;
			sortingOrder = component.sortingOrder;
			this.DestroyComponent(component);
		}
		this.meshFilter = base.gameObject.AddComponentIfNotPresent<MeshFilter>();
		this.meshFilter.hideFlags |= HideFlags.HideInInspector;
		this.meshRenderer = base.gameObject.AddComponentIfNotPresent<MeshRenderer>();
		this.meshRenderer.hideFlags |= HideFlags.HideInInspector;
		this.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		this.meshRenderer.receiveShadows = false;
		this.meshRenderer.sortingLayerID = sortingLayerID;
		this.meshRenderer.sortingOrder = sortingOrder;
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x0005F0E0 File Offset: 0x0005D2E0
	public void RemoveAndAddSpriteRenderer()
	{
		int sortingLayerID = this.meshRenderer.sortingLayerID;
		int sortingOrder = this.meshRenderer.sortingOrder;
		this.DestroyComponent(this.meshFilter);
		this.DestroyComponent(this.meshRenderer);
		SpriteRenderer spriteRenderer = base.gameObject.AddComponentIfNotPresent<SpriteRenderer>();
		if (spriteRenderer)
		{
			spriteRenderer.sprite = this.sprite;
			spriteRenderer.color = this.color;
			spriteRenderer.sortingLayerID = sortingLayerID;
			spriteRenderer.sortingOrder = sortingOrder;
		}
		this.DestroyComponent(this);
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x0005F15E File Offset: 0x0005D35E
	private void DestroyComponent(Component component)
	{
		if (!component)
		{
			return;
		}
		Object.DestroyImmediate(component, true);
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x0005F170 File Offset: 0x0005D370
	public void UpdateMesh()
	{
		if (this.sprite == null || this.meshFilter == null)
		{
			return;
		}
		Animator componentInParent = base.GetComponentInParent<Animator>();
		if (Application.isPlaying && !componentInParent && !this.appliedFlip)
		{
			Transform transform = base.transform;
			Vector3 lossyScale = transform.lossyScale;
			this.appliedFlip = true;
			this.flipX = (Mathf.Sign(lossyScale.x) < 0f);
			this.flipY = (Mathf.Sign(lossyScale.y) < 0f);
			bool flag = lossyScale.z < 0f;
			if (this.flipX || this.flipY || flag)
			{
				transform.FlipLocalScale(this.flipX, this.flipY, flag);
			}
		}
		Vector3[] array = (from v in this.sprite.vertices
		select v.ToVector3(0f)).ToArray<Vector3>();
		int[] triangles = this.sprite.triangles.Select(new Func<ushort, int>(Convert.ToInt32)).ToArray<int>();
		List<Vector2> source = this.sprite.uv.ToList<Vector2>();
		Color[] array2 = new Color[array.Length];
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i] = this.color;
		}
		Vector3 position = base.transform.position;
		Vector4 vector = new Vector4(position.x, position.y, position.z);
		Vector4[] array3 = new Vector4[array.Length];
		for (int j = 0; j < array3.Length; j++)
		{
			Vector3 vector2 = array[j];
			if (this.flipX)
			{
				vector2.x *= -1f;
			}
			if (this.flipY)
			{
				vector2.y *= -1f;
			}
			float y = vector2.y;
			vector.w = y;
			array[j] = vector2;
			array3[j] = vector;
		}
		this.mesh = this.meshFilter.sharedMesh;
		if (this.mesh)
		{
			Object.DestroyImmediate(this.mesh);
		}
		this.mesh = new Mesh();
		this.mesh.name = "GrassSprite";
		this.mesh.hideFlags |= HideFlags.DontSave;
		this.meshFilter.sharedMesh = this.mesh;
		this.mesh.vertices = array;
		this.mesh.triangles = triangles;
		this.mesh.colors = array2;
		this.mesh.SetUVs(0, source.ToList<Vector2>());
		this.mesh.SetUVs(3, array3.ToList<Vector4>());
		this.mesh.RecalculateBounds();
		this.mesh.RecalculateNormals();
		MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
		this.meshRenderer.GetPropertyBlock(materialPropertyBlock);
		materialPropertyBlock.SetTexture(GrassSprite._mainTexProp, this.sprite.texture);
		this.meshRenderer.SetPropertyBlock(materialPropertyBlock);
	}

	// Token: 0x04001384 RID: 4996
	[SerializeField]
	private Sprite sprite;

	// Token: 0x04001385 RID: 4997
	[SerializeField]
	private Color color;

	// Token: 0x04001386 RID: 4998
	private MeshFilter meshFilter;

	// Token: 0x04001387 RID: 4999
	private MeshRenderer meshRenderer;

	// Token: 0x04001388 RID: 5000
	private Mesh mesh;

	// Token: 0x04001389 RID: 5001
	private bool appliedFlip;

	// Token: 0x0400138A RID: 5002
	private bool flipX;

	// Token: 0x0400138B RID: 5003
	private bool flipY;

	// Token: 0x0400138C RID: 5004
	private static readonly int _mainTexProp = Shader.PropertyToID("_MainTex");
}
