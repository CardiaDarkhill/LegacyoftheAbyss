using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200055A RID: 1370
public class SpikeCogFall : MonoBehaviour
{
	// Token: 0x060030FF RID: 12543 RVA: 0x000D9327 File Offset: 0x000D7527
	private void OnValidate()
	{
		if (this.foregroundZLimit < 0f)
		{
			this.foregroundZLimit = 0f;
		}
	}

	// Token: 0x06003100 RID: 12544 RVA: 0x000D9344 File Offset: 0x000D7544
	private void Awake()
	{
		if (!this.body)
		{
			this.SetBounced();
		}
		if (this.sprite)
		{
			this.initialColor = this.sprite.color;
		}
		if (this.heroSafeRange)
		{
			this.heroSafeRange.InsideStateChanged += this.OnHeroSafeRangeInsideStateChanged;
		}
	}

	// Token: 0x06003101 RID: 12545 RVA: 0x000D93A6 File Offset: 0x000D75A6
	private void OnDestroy()
	{
		if (this.heroSafeRange)
		{
			this.heroSafeRange.InsideStateChanged -= this.OnHeroSafeRangeInsideStateChanged;
		}
	}

	// Token: 0x06003102 RID: 12546 RVA: 0x000D93CC File Offset: 0x000D75CC
	private void Update()
	{
		Vector3 position = base.transform.position;
		if (position.y < -10f)
		{
			base.gameObject.SetActive(false);
		}
		if (!this.hasBounced)
		{
			return;
		}
		if (!this.hasDisabled && position.y < this.lowerYLimit)
		{
			this.disableOnBounce.SetAllActive(false);
			this.hasDisabled = true;
			if (this.heroDamager)
			{
				this.heroDamager.SetActive(false);
			}
		}
		position.z += this.zSpeed * Time.deltaTime;
		base.transform.position = position;
		if (!this.sprite)
		{
			return;
		}
		float f = position.z - this.bouncedZ;
		if (this.zSpeed < 0f)
		{
			float t = Mathf.Clamp01(Mathf.Abs(f) / this.foregroundZLimit);
			this.sprite.color = Color.Lerp(this.initialColor, Color.black, t);
		}
	}

	// Token: 0x06003103 RID: 12547 RVA: 0x000D94C4 File Offset: 0x000D76C4
	private void OnCollisionEnter2D()
	{
		this.disableOnBounce.SetAllActive(false);
		this.hasDisabled = true;
		if (this.heroDamager)
		{
			this.heroDamager.SetActive(false);
		}
		if (this.body)
		{
			Vector2 linearVelocity = this.body.linearVelocity;
			linearVelocity.y = this.bounceImpulse.GetRandomValue();
			this.body.linearVelocity = linearVelocity;
		}
		this.SetBounced();
	}

	// Token: 0x06003104 RID: 12548 RVA: 0x000D953C File Offset: 0x000D773C
	private void SetBounced()
	{
		this.hasBounced = true;
		this.bouncedZ = base.transform.position.z;
		this.zSpeed = ((Random.Range(0, 2) == 0) ? this.bounceZSpeed.Start : this.bounceZSpeed.End);
	}

	// Token: 0x06003105 RID: 12549 RVA: 0x000D958D File Offset: 0x000D778D
	private void OnHeroSafeRangeInsideStateChanged(bool isInside)
	{
		if (!base.isActiveAndEnabled || this.hasDisabled)
		{
			return;
		}
		if (this.heroDamager)
		{
			this.heroDamager.SetActive(!isInside);
		}
	}

	// Token: 0x04003447 RID: 13383
	[SerializeField]
	private GameObject[] disableOnBounce;

	// Token: 0x04003448 RID: 13384
	[SerializeField]
	private float lowerYLimit;

	// Token: 0x04003449 RID: 13385
	[Space]
	[SerializeField]
	private SpriteRenderer sprite;

	// Token: 0x0400344A RID: 13386
	[SerializeField]
	private MinMaxFloat bounceZSpeed;

	// Token: 0x0400344B RID: 13387
	[SerializeField]
	private float foregroundZLimit;

	// Token: 0x0400344C RID: 13388
	[Space]
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x0400344D RID: 13389
	[SerializeField]
	private MinMaxFloat bounceImpulse;

	// Token: 0x0400344E RID: 13390
	[Space]
	[SerializeField]
	private GameObject heroDamager;

	// Token: 0x0400344F RID: 13391
	[SerializeField]
	private TrackTriggerObjects heroSafeRange;

	// Token: 0x04003450 RID: 13392
	private bool hasBounced;

	// Token: 0x04003451 RID: 13393
	private bool hasDisabled;

	// Token: 0x04003452 RID: 13394
	private float zSpeed;

	// Token: 0x04003453 RID: 13395
	private float bouncedZ;

	// Token: 0x04003454 RID: 13396
	private Color initialColor;
}
