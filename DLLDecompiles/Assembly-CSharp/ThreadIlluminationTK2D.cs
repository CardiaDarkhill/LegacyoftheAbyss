using System;
using UnityEngine;

// Token: 0x020000EC RID: 236
public class ThreadIlluminationTK2D : MonoBehaviour
{
	// Token: 0x06000778 RID: 1912 RVA: 0x00024651 File Offset: 0x00022851
	private void Awake()
	{
		this.sprite = base.GetComponent<tk2dSprite>();
		this.initialColor = this.sprite.color;
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x00024670 File Offset: 0x00022870
	private void OnEnable()
	{
		this.mainCamera = GameCameras.instance.mainCamera.gameObject.transform;
		this.offsetX = Random.Range(-8f, 8f);
		this.offsetY = Random.Range(-8f, 8f);
	}

	// Token: 0x0600077A RID: 1914 RVA: 0x000246C4 File Offset: 0x000228C4
	private void Update()
	{
		if (this.active)
		{
			Vector2 a = this.mainCamera.position;
			Vector3 position = base.transform.position;
			Vector2 b = new Vector2(position.x + this.offsetX, position.y + this.offsetY);
			float num = Vector2.Distance(a, b);
			if (num > 10f)
			{
				this.SetAlpha(0f);
				if (this.visible)
				{
					this.visible = false;
				}
			}
			else if (num > 1f)
			{
				if (this.visible)
				{
					this.visible = false;
				}
				float alpha = 1f - (num - 1f) / 9f;
				this.SetAlpha(alpha);
			}
			else if (!this.visible)
			{
				this.SetAlpha(1f);
				this.visible = true;
			}
		}
		if (this.revertAlpha)
		{
			float num2 = this.sprite.color.a;
			if (num2 < 1f)
			{
				num2 += Time.deltaTime * 4f;
				if (num2 > 1f)
				{
					num2 = 1f;
				}
				this.sprite.color = new Color(this.initialColor.r, this.initialColor.g, this.initialColor.b, num2);
			}
			else
			{
				this.revertAlpha = false;
			}
		}
		if (this.fadeOut)
		{
			float num3 = this.sprite.color.a;
			if (num3 > 0f)
			{
				num3 -= Time.deltaTime * 4f;
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				this.sprite.color = new Color(this.initialColor.r, this.initialColor.g, this.initialColor.b, num3);
				return;
			}
			this.fadeOut = false;
		}
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x0002488F File Offset: 0x00022A8F
	private void SetAlpha(float alpha)
	{
		this.sprite.color = new Color(this.initialColor.r, this.initialColor.g, this.initialColor.b, alpha);
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x000248C3 File Offset: 0x00022AC3
	public void StopIllumination()
	{
		this.active = false;
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x000248CC File Offset: 0x00022ACC
	public void ThreadStrum()
	{
		this.StopIllumination();
		this.revertAlpha = true;
		tk2dSpriteAnimator component = base.GetComponent<tk2dSpriteAnimator>();
		if (component)
		{
			component.PlayFromFrame("Strum", Random.Range(0, 3));
		}
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00024907 File Offset: 0x00022B07
	public void ThreadEnd()
	{
		this.fadeOut = true;
	}

	// Token: 0x04000745 RID: 1861
	private const float RANDOMISATION_RANGE = 8f;

	// Token: 0x04000746 RID: 1862
	private const float VISIBLE_RANGE = 1f;

	// Token: 0x04000747 RID: 1863
	private const float FALLOFF_RANGE = 10f;

	// Token: 0x04000748 RID: 1864
	private tk2dSprite sprite;

	// Token: 0x04000749 RID: 1865
	private Transform mainCamera;

	// Token: 0x0400074A RID: 1866
	private Color initialColor;

	// Token: 0x0400074B RID: 1867
	private float offsetX;

	// Token: 0x0400074C RID: 1868
	private float offsetY;

	// Token: 0x0400074D RID: 1869
	private bool visible;

	// Token: 0x0400074E RID: 1870
	private bool revertAlpha;

	// Token: 0x0400074F RID: 1871
	private bool fadeOut;

	// Token: 0x04000750 RID: 1872
	public bool active = true;
}
