using System;
using UnityEngine;

// Token: 0x02000267 RID: 615
public class PositionToHeroClamped : MonoBehaviour
{
	// Token: 0x0600160F RID: 5647 RVA: 0x00062E68 File Offset: 0x00061068
	private void OnDrawGizmosSelected()
	{
		Vector3 center;
		Vector3 size;
		this.GetPositionAndSize(out center, out size);
		Gizmos.DrawWireCube(center, size);
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x00062E86 File Offset: 0x00061086
	private void Start()
	{
		this.hero = HeroController.instance.transform;
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x00062E98 File Offset: 0x00061098
	private void Update()
	{
		Vector2 vector = this.hero.position;
		Vector2 vector2 = base.transform.position;
		Vector3 v;
		Vector3 v2;
		this.GetPositionAndSize(out v, out v2);
		Vector2 a = v;
		Vector2 b = v2 / 2f;
		Vector2 vector3 = a - b;
		Vector2 vector4 = a + b;
		if (this.PositionX)
		{
			vector2.x = vector.x;
			vector2.x = Mathf.Clamp(vector2.x, vector3.x, vector4.x);
		}
		if (this.PositionY)
		{
			vector2.y = vector.y;
			vector2.y = Mathf.Clamp(vector2.y, vector3.y, vector4.y);
		}
		base.transform.position = vector2;
	}

	// Token: 0x06001612 RID: 5650 RVA: 0x00062F78 File Offset: 0x00061178
	private void GetPositionAndSize(out Vector3 pos, out Vector3 size)
	{
		Transform parent = base.transform.parent;
		if (parent == null)
		{
			pos = this.ClampAreaOffset.ToVector3(base.transform.position.z);
			size = this.ClampAreaSize.ToVector3(1f);
			return;
		}
		Vector3 original = parent.TransformPoint(this.ClampAreaOffset);
		float? z = new float?(base.transform.position.z);
		pos = original.Where(null, null, z);
		Vector3 original2 = parent.TransformVector(this.ClampAreaSize);
		z = new float?(1f);
		size = original2.Where(null, null, z);
	}

	// Token: 0x0400147E RID: 5246
	public Vector2 ClampAreaOffset;

	// Token: 0x0400147F RID: 5247
	public Vector2 ClampAreaSize;

	// Token: 0x04001480 RID: 5248
	public bool PositionX;

	// Token: 0x04001481 RID: 5249
	public bool PositionY;

	// Token: 0x04001482 RID: 5250
	private Transform hero;
}
