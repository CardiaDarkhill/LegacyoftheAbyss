using System;
using UnityEngine;

// Token: 0x020005B1 RID: 1457
public class RandomScale : MonoBehaviour, IExternalDebris
{
	// Token: 0x06003460 RID: 13408 RVA: 0x000E8D35 File Offset: 0x000E6F35
	private void Start()
	{
		this.ApplyScale();
	}

	// Token: 0x06003461 RID: 13409 RVA: 0x000E8D3D File Offset: 0x000E6F3D
	private void OnEnable()
	{
		if (this.scaleOnEnable)
		{
			this.ApplyScale();
		}
	}

	// Token: 0x06003462 RID: 13410 RVA: 0x000E8D50 File Offset: 0x000E6F50
	private void ApplyScale()
	{
		float num = Random.Range(this.minScale, this.maxScale);
		float y = num;
		if (this.randomlyFlipX && (float)Random.Range(1, 100) > 50f)
		{
			num = -num;
		}
		base.transform.localScale = new Vector3(num, y, 1f);
	}

	// Token: 0x06003463 RID: 13411 RVA: 0x000E8DA3 File Offset: 0x000E6FA3
	public void InitExternalDebris()
	{
		this.ApplyScale();
	}

	// Token: 0x040037DF RID: 14303
	[SerializeField]
	private float minScale;

	// Token: 0x040037E0 RID: 14304
	[SerializeField]
	private float maxScale;

	// Token: 0x040037E1 RID: 14305
	[SerializeField]
	private bool randomlyFlipX;

	// Token: 0x040037E2 RID: 14306
	[SerializeField]
	private bool scaleOnEnable;
}
