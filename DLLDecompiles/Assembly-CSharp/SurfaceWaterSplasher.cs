using System;
using UnityEngine;

// Token: 0x02000566 RID: 1382
public class SurfaceWaterSplasher : MonoBehaviour
{
	// Token: 0x06003173 RID: 12659 RVA: 0x000DBB4C File Offset: 0x000D9D4C
	private void Awake()
	{
		Collider2D component = base.GetComponent<Collider2D>();
		if (component)
		{
			Vector3 center = component.bounds.center;
			this.splashOffset = center - base.transform.position;
			this.splashOffset.z = 0f;
		}
	}

	// Token: 0x06003174 RID: 12660 RVA: 0x000DBB9E File Offset: 0x000D9D9E
	private void OnEnable()
	{
		this.isRecycled = false;
		this.insideCount = 0;
	}

	// Token: 0x06003175 RID: 12661 RVA: 0x000DBBAE File Offset: 0x000D9DAE
	private void OnDisable()
	{
		this.insideRegion = null;
	}

	// Token: 0x06003176 RID: 12662 RVA: 0x000DBBB8 File Offset: 0x000D9DB8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.isRecycled)
		{
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		SurfaceWaterRegion componentInParent = collision.gameObject.GetComponentInParent<SurfaceWaterRegion>();
		if (componentInParent)
		{
			this.insideCount++;
			if (this.insideCount == 1)
			{
				this.insideRegion = componentInParent;
				this.DoSplashIn();
			}
		}
	}

	// Token: 0x06003177 RID: 12663 RVA: 0x000DBC14 File Offset: 0x000D9E14
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.insideCount == 0)
		{
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		SurfaceWaterRegion componentInParent = collision.gameObject.GetComponentInParent<SurfaceWaterRegion>();
		if (!componentInParent)
		{
			return;
		}
		this.insideCount--;
		if (this.insideCount != 0)
		{
			return;
		}
		componentInParent.DoSplashOut(base.transform, Vector2.zero);
		this.insideRegion = null;
	}

	// Token: 0x06003178 RID: 12664 RVA: 0x000DBC7C File Offset: 0x000D9E7C
	public void DoSplashIn()
	{
		if (this.isRecycled)
		{
			return;
		}
		if (!this.insideRegion)
		{
			return;
		}
		if (!this.smallSplash)
		{
			this.insideRegion.DoSplashIn(base.transform, this.splashOffset, false);
		}
		else
		{
			this.insideRegion.DoSplashInSmall(base.transform, this.splashOffset);
		}
		if (this.recycleOnSplash)
		{
			this.isRecycled = true;
			base.gameObject.Recycle();
		}
	}

	// Token: 0x040034C8 RID: 13512
	private Vector3 splashOffset;

	// Token: 0x040034C9 RID: 13513
	private int insideCount;

	// Token: 0x040034CA RID: 13514
	private SurfaceWaterRegion insideRegion;

	// Token: 0x040034CB RID: 13515
	public bool smallSplash;

	// Token: 0x040034CC RID: 13516
	public bool recycleOnSplash;

	// Token: 0x040034CD RID: 13517
	private bool isRecycled;
}
