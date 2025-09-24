using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A5 RID: 165
public class RandomSprite : MonoBehaviour
{
	// Token: 0x06000504 RID: 1284 RVA: 0x0001A086 File Offset: 0x00018286
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x0001A094 File Offset: 0x00018294
	private void OnEnable()
	{
		this.spriteRenderer.sprite = this.sprites[Random.Range(0, this.sprites.Count)];
		if (this.flipXScale && Random.Range(1, 100) > 50)
		{
			base.transform.localScale = new Vector3(-base.transform.localScale.x, base.transform.localScale.y, base.transform.localScale.z);
		}
		if (this.flipYScale && Random.Range(1, 100) > 50)
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x, -base.transform.localScale.y, base.transform.localScale.z);
		}
	}

	// Token: 0x040004DD RID: 1245
	[SerializeField]
	private List<Sprite> sprites;

	// Token: 0x040004DE RID: 1246
	[SerializeField]
	private bool flipXScale;

	// Token: 0x040004DF RID: 1247
	[SerializeField]
	private bool flipYScale;

	// Token: 0x040004E0 RID: 1248
	private SpriteRenderer spriteRenderer;
}
