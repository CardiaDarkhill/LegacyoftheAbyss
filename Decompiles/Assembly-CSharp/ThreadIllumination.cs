using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class ThreadIllumination : MonoBehaviour, IUpdateBatchableUpdate
{
	// Token: 0x1700009E RID: 158
	// (get) Token: 0x06000771 RID: 1905 RVA: 0x0002441A File Offset: 0x0002261A
	public bool ShouldUpdate
	{
		get
		{
			return !this.spriteRenderer || this.spriteRenderer.isVisible;
		}
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00024438 File Offset: 0x00022638
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.initialColor = this.spriteRenderer.color;
		this.fadeBridge = base.GetComponent<NestedFadeGroupSpriteRenderer>();
		if (this.fadeBridge)
		{
			this.initialColor.a = this.fadeBridge.AlphaSelf;
		}
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x00024494 File Offset: 0x00022694
	private void OnEnable()
	{
		this.mainCamera = GameCameras.instance.mainCamera.gameObject.transform;
		this.offsetX = Random.Range(-8f, 8f);
		this.offsetY = Random.Range(-8f, 8f);
		GameManager instance = GameManager.instance;
		if (instance)
		{
			instance.GetComponent<UpdateBatcher>().Add(this);
		}
	}

	// Token: 0x06000774 RID: 1908 RVA: 0x00024500 File Offset: 0x00022700
	private void OnDisable()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance)
		{
			silentInstance.GetComponent<UpdateBatcher>().Remove(this);
		}
	}

	// Token: 0x06000775 RID: 1909 RVA: 0x00024528 File Offset: 0x00022728
	public void BatchedUpdate()
	{
		Vector2 a = this.mainCamera.position;
		Vector3 position = base.transform.position;
		Vector2 b = new Vector2(position.x + this.offsetX, position.y + this.offsetY);
		float num = Vector2.Distance(a, b);
		if (num > 12f)
		{
			this.SetAlpha(0.1f);
			if (this.visible)
			{
				this.visible = false;
				return;
			}
		}
		else
		{
			if (num > 2f)
			{
				if (this.visible)
				{
					this.visible = false;
				}
				float alpha = 1f - (num - 2f) / 10f * 0.9f;
				this.SetAlpha(alpha);
				return;
			}
			if (!this.visible)
			{
				this.SetAlpha(1f);
				this.visible = true;
			}
		}
	}

	// Token: 0x06000776 RID: 1910 RVA: 0x000245F0 File Offset: 0x000227F0
	private void SetAlpha(float alpha)
	{
		if (this.fadeBridge)
		{
			this.fadeBridge.AlphaSelf = alpha;
			return;
		}
		this.spriteRenderer.color = new Color(this.initialColor.r, this.initialColor.g, this.initialColor.b, alpha);
	}

	// Token: 0x0400073B RID: 1851
	private const float RANDOMISATION_RANGE = 8f;

	// Token: 0x0400073C RID: 1852
	private const float VISIBLE_RANGE = 2f;

	// Token: 0x0400073D RID: 1853
	private const float FALLOFF_RANGE = 12f;

	// Token: 0x0400073E RID: 1854
	private SpriteRenderer spriteRenderer;

	// Token: 0x0400073F RID: 1855
	private NestedFadeGroupSpriteRenderer fadeBridge;

	// Token: 0x04000740 RID: 1856
	private Transform mainCamera;

	// Token: 0x04000741 RID: 1857
	private Color initialColor;

	// Token: 0x04000742 RID: 1858
	private float offsetX;

	// Token: 0x04000743 RID: 1859
	private float offsetY;

	// Token: 0x04000744 RID: 1860
	private bool visible;
}
