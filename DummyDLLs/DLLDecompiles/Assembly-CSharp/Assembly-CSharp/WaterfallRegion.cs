using System;
using UnityEngine;

// Token: 0x02000585 RID: 1413
public class WaterfallRegion : MonoBehaviour
{
	// Token: 0x0600328C RID: 12940 RVA: 0x000E108E File Offset: 0x000DF28E
	private void OnValidate()
	{
		if (this.downVelocity < 0f)
		{
			this.downVelocity = 0f;
		}
	}

	// Token: 0x0600328D RID: 12941 RVA: 0x000E10A8 File Offset: 0x000DF2A8
	private void FixedUpdate()
	{
		if (!this.heroInside)
		{
			return;
		}
		if (Time.timeAsDouble >= this.nextHeroDropletFlingTime)
		{
			this.FlingHeroDroplets();
		}
		if (this.hc.cState.swimming)
		{
			return;
		}
		if (this.hc.cState.onGround)
		{
			return;
		}
		Vector2 linearVelocity = this.body.linearVelocity;
		linearVelocity.y -= this.downVelocity * Time.fixedDeltaTime;
		float num = -this.hc.GetMaxFallVelocity();
		if (linearVelocity.y < num)
		{
			linearVelocity.y = num;
		}
		this.body.linearVelocity = linearVelocity;
	}

	// Token: 0x0600328E RID: 12942 RVA: 0x000E1144 File Offset: 0x000DF344
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (this.hc)
		{
			return;
		}
		this.hc = other.GetComponent<HeroController>();
		if (!this.hc)
		{
			return;
		}
		this.body = other.GetComponent<Rigidbody2D>();
		this.heroInside = true;
		this.FlingHeroDroplets();
		if (!this.hc.controlReqlinquished)
		{
			this.hc.RecoilDown();
		}
	}

	// Token: 0x0600328F RID: 12943 RVA: 0x000E11AA File Offset: 0x000DF3AA
	private void OnTriggerExit2D(Collider2D other)
	{
		if (!other.GetComponent<HeroController>())
		{
			return;
		}
		this.hc = null;
		this.body = null;
		this.heroInside = false;
	}

	// Token: 0x06003290 RID: 12944 RVA: 0x000E11CF File Offset: 0x000DF3CF
	private void FlingHeroDroplets()
	{
		this.nextHeroDropletFlingTime = Time.timeAsDouble + (double)this.heroDropletFlingDelay;
		FlingUtils.SpawnAndFling(this.heroDropletFling, this.hc.transform, this.heroDropletFlingOffset, null, -1f);
	}

	// Token: 0x0400365C RID: 13916
	[SerializeField]
	private float downVelocity;

	// Token: 0x0400365D RID: 13917
	[SerializeField]
	private FlingUtils.Config heroDropletFling;

	// Token: 0x0400365E RID: 13918
	[SerializeField]
	private Vector3 heroDropletFlingOffset;

	// Token: 0x0400365F RID: 13919
	[SerializeField]
	private float heroDropletFlingDelay;

	// Token: 0x04003660 RID: 13920
	private bool heroInside;

	// Token: 0x04003661 RID: 13921
	private double nextHeroDropletFlingTime;

	// Token: 0x04003662 RID: 13922
	private HeroController hc;

	// Token: 0x04003663 RID: 13923
	private Rigidbody2D body;
}
