using System;
using UnityEngine;

// Token: 0x02000233 RID: 563
public class FollowCamera : MonoBehaviour
{
	// Token: 0x060014AA RID: 5290 RVA: 0x0005D0E7 File Offset: 0x0005B2E7
	private void Awake()
	{
		if (this.enableOnCameraPosStart)
		{
			this.enableOnCameraPosStart.SetActive(false);
		}
	}

	// Token: 0x060014AB RID: 5291 RVA: 0x0005D102 File Offset: 0x0005B302
	private void OnEnable()
	{
		this.gc = GameCameras.instance;
		this.gameCamera = this.gc.mainCamera.gameObject;
		this.gc.cameraController.PositionedAtHero += this.OnPositionedAtHero;
	}

	// Token: 0x060014AC RID: 5292 RVA: 0x0005D141 File Offset: 0x0005B341
	private void OnDisable()
	{
		if (this.gc != null)
		{
			this.gc.cameraController.PositionedAtHero -= this.OnPositionedAtHero;
			this.gc = null;
		}
	}

	// Token: 0x060014AD RID: 5293 RVA: 0x0005D174 File Offset: 0x0005B374
	private void Update()
	{
		this.UpdatePosition();
	}

	// Token: 0x060014AE RID: 5294 RVA: 0x0005D17C File Offset: 0x0005B37C
	private void OnPositionedAtHero()
	{
		this.UpdatePosition();
		if (this.enableOnCameraPosStart)
		{
			this.enableOnCameraPosStart.SetActive(true);
		}
	}

	// Token: 0x060014AF RID: 5295 RVA: 0x0005D1A0 File Offset: 0x0005B3A0
	private void UpdatePosition()
	{
		Vector3 position = base.transform.position;
		Vector3 position2 = this.gameCamera.transform.position;
		if (this.followX)
		{
			position.x = position2.x + this.offsetX;
		}
		if (this.followY)
		{
			position.y = position2.y + this.offsetY;
		}
		if (this.clampToCollider)
		{
			Bounds bounds = this.clampToCollider.bounds;
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			if (position.x < min.x)
			{
				position.x = min.x;
			}
			else if (position.x > max.x)
			{
				position.x = max.x;
			}
			if (position.y < min.y)
			{
				position.y = min.y;
			}
			else if (position.y > max.y)
			{
				position.y = max.y;
			}
		}
		if (this.clampToRange)
		{
			if (position.x < this.xRange.x)
			{
				position.x = this.xRange.x;
			}
			else if (position.x > this.xRange.y)
			{
				position.x = this.xRange.y;
			}
			if (position.y < this.yRange.x)
			{
				position.y = this.yRange.x;
			}
			else if (position.y > this.yRange.y)
			{
				position.y = this.yRange.y;
			}
		}
		base.transform.position = position;
	}

	// Token: 0x060014B0 RID: 5296 RVA: 0x0005D352 File Offset: 0x0005B552
	public void SetClampRangeX(float rangeMin, float rangeMax)
	{
		this.xRange = new Vector2(rangeMin, rangeMax);
	}

	// Token: 0x060014B1 RID: 5297 RVA: 0x0005D361 File Offset: 0x0005B561
	public void SetClampRangeY(float rangeMin, float rangeMax)
	{
		this.yRange = new Vector2(rangeMin, rangeMax);
	}

	// Token: 0x04001309 RID: 4873
	[SerializeField]
	private bool followX;

	// Token: 0x0400130A RID: 4874
	[SerializeField]
	[ModifiableProperty]
	[Conditional("followX", true, false, false)]
	private float offsetX;

	// Token: 0x0400130B RID: 4875
	[SerializeField]
	private bool followY;

	// Token: 0x0400130C RID: 4876
	[SerializeField]
	[ModifiableProperty]
	[Conditional("followY", true, false, false)]
	private float offsetY;

	// Token: 0x0400130D RID: 4877
	[SerializeField]
	private BoxCollider2D clampToCollider;

	// Token: 0x0400130E RID: 4878
	[SerializeField]
	private bool clampToRange;

	// Token: 0x0400130F RID: 4879
	[SerializeField]
	[ModifiableProperty]
	[Conditional("clampToRange", true, false, true)]
	private Vector2 xRange = new Vector2(0f, 9999f);

	// Token: 0x04001310 RID: 4880
	[SerializeField]
	[ModifiableProperty]
	[Conditional("clampToRange", true, false, true)]
	private Vector2 yRange = new Vector2(0f, 9999f);

	// Token: 0x04001311 RID: 4881
	[SerializeField]
	private GameObject enableOnCameraPosStart;

	// Token: 0x04001312 RID: 4882
	private GameCameras gc;

	// Token: 0x04001313 RID: 4883
	private GameObject gameCamera;
}
