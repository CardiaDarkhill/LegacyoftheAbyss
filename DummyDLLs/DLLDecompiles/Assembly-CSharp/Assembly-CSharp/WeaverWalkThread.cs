using System;
using UnityEngine;

// Token: 0x020000F2 RID: 242
public class WeaverWalkThread : MonoBehaviour
{
	// Token: 0x060007AB RID: 1963 RVA: 0x0002506F File Offset: 0x0002326F
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x060007AC RID: 1964 RVA: 0x0002507D File Offset: 0x0002327D
	private void OnEnable()
	{
		base.transform.localEulerAngles = new Vector3(0f, 0f, Random.Range(-1.5f, 1.5f));
		this.fullAlpha = Random.Range(0.5f, 0.8f);
	}

	// Token: 0x060007AD RID: 1965 RVA: 0x000250C0 File Offset: 0x000232C0
	private void Update()
	{
		Vector2 a = new Vector2(this.weaver.position.x, this.weaver.position.y);
		Vector2 b = new Vector2(base.transform.position.x + this.offsetX, base.transform.position.y + this.offsetY);
		float num = Vector2.Distance(a, b);
		if (num > this.falloffRange)
		{
			if (this.spriteRenderer.enabled)
			{
				this.spriteRenderer.enabled = false;
			}
			if (this.visible)
			{
				this.visible = false;
				return;
			}
		}
		else
		{
			if (num > this.visibleRange)
			{
				if (!this.spriteRenderer.enabled)
				{
					this.spriteRenderer.enabled = true;
				}
				if (this.visible)
				{
					this.visible = false;
				}
				float a2 = this.fullAlpha - (num - this.visibleRange) / (this.falloffRange - this.visibleRange);
				this.spriteRenderer.color = new Color(1f, 1f, 1f, a2);
				return;
			}
			if (!this.visible)
			{
				this.spriteRenderer.enabled = true;
				this.spriteRenderer.color = new Color(1f, 1f, 1f, this.fullAlpha);
				this.visible = true;
			}
		}
	}

	// Token: 0x04000777 RID: 1911
	public Transform weaver;

	// Token: 0x04000778 RID: 1912
	private float visibleRange = 1f;

	// Token: 0x04000779 RID: 1913
	private float falloffRange = 3f;

	// Token: 0x0400077A RID: 1914
	private float fullAlpha;

	// Token: 0x0400077B RID: 1915
	private SpriteRenderer spriteRenderer;

	// Token: 0x0400077C RID: 1916
	private float offsetX;

	// Token: 0x0400077D RID: 1917
	private float offsetY;

	// Token: 0x0400077E RID: 1918
	private bool visible;
}
