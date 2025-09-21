using System;
using UnityEngine;

// Token: 0x020004B0 RID: 1200
public class ChainLinkInteraction : MonoBehaviour
{
	// Token: 0x1700050C RID: 1292
	// (get) Token: 0x06002B5A RID: 11098 RVA: 0x000BE524 File Offset: 0x000BC724
	// (set) Token: 0x06002B5B RID: 11099 RVA: 0x000BE52C File Offset: 0x000BC72C
	public ChainPushReaction Chain { get; set; }

	// Token: 0x06002B5C RID: 11100 RVA: 0x000BE535 File Offset: 0x000BC735
	private void Awake()
	{
		this.collider = base.GetComponent<Collider2D>();
	}

	// Token: 0x06002B5D RID: 11101 RVA: 0x000BE544 File Offset: 0x000BC744
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!this.Chain)
		{
			return;
		}
		this.Chain.TouchedLink(collision.GetSafeContact().Point);
		if (!this.Chain.IsPushDisableStarted)
		{
			this.Chain.DisableLinks(collision.transform);
			return;
		}
		this.Chain.AddDisableTracker(collision.transform);
	}

	// Token: 0x06002B5E RID: 11102 RVA: 0x000BE5AA File Offset: 0x000BC7AA
	public void SetActive(bool value)
	{
		if (this.collider)
		{
			this.collider.enabled = value;
		}
	}

	// Token: 0x04002CB1 RID: 11441
	private Collider2D collider;
}
