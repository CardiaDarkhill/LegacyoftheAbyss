using System;
using UnityEngine;

// Token: 0x02000231 RID: 561
[Serializable]
public class FlingGameObject
{
	// Token: 0x060014A1 RID: 5281 RVA: 0x0005CED3 File Offset: 0x0005B0D3
	public void Fling(Vector3 spawnPosition, float velocityMultiplier)
	{
		this.Fling(spawnPosition, Quaternion.identity, velocityMultiplier);
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x0005CEE4 File Offset: 0x0005B0E4
	public void Fling(Vector3 spawnPosition, Quaternion spawnRotation, float velocityMultiplier)
	{
		int num = Random.Range(this.spawnMin, this.spawnMax + 1);
		for (int i = 1; i <= num; i++)
		{
			GameObject gameObject = this.prefab.Spawn(spawnPosition, spawnRotation);
			float num2 = Random.Range(this.speedMin, this.speedMax);
			float num3 = Random.Range(this.angleMin, this.angleMax);
			Vector2 vector;
			vector.x = num2 * Mathf.Cos(num3 * 0.017453292f);
			vector.y = num2 * Mathf.Sin(num3 * 0.017453292f);
			vector *= Mathf.Abs(velocityMultiplier);
			Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
			if (component)
			{
				component.linearVelocity = vector;
			}
		}
	}

	// Token: 0x040012FB RID: 4859
	public GameObject prefab;

	// Token: 0x040012FC RID: 4860
	[Space]
	public int spawnMin = 5;

	// Token: 0x040012FD RID: 4861
	public int spawnMax = 10;

	// Token: 0x040012FE RID: 4862
	[Space]
	public float speedMin = 10f;

	// Token: 0x040012FF RID: 4863
	public float speedMax = 20f;

	// Token: 0x04001300 RID: 4864
	[Space]
	public float angleMin = -45f;

	// Token: 0x04001301 RID: 4865
	public float angleMax = 45f;
}
