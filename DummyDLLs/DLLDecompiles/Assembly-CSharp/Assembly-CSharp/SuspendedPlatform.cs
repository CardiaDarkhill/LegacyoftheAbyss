using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000568 RID: 1384
public class SuspendedPlatform : SuspendedPlatformBase
{
	// Token: 0x06003180 RID: 12672 RVA: 0x000DBE3C File Offset: 0x000DA03C
	public override void CutDown()
	{
		base.CutDown();
		this.SetBroken();
	}

	// Token: 0x06003181 RID: 12673 RVA: 0x000DBE4A File Offset: 0x000DA04A
	protected override void OnStartActivated()
	{
		base.OnStartActivated();
		this.SetBroken();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003182 RID: 12674 RVA: 0x000DBE64 File Offset: 0x000DA064
	private void SetBroken()
	{
		if (this.collider)
		{
			this.collider.enabled = false;
		}
		this.onBreak.Invoke();
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x000DBE8A File Offset: 0x000DA08A
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer != 9)
		{
			return;
		}
		this.touchingCount++;
		if (this.touchingCount == 1)
		{
			this.onStartedTouching.Invoke();
		}
	}

	// Token: 0x06003184 RID: 12676 RVA: 0x000DBEBE File Offset: 0x000DA0BE
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (collision.gameObject.layer != 9)
		{
			return;
		}
		this.touchingCount--;
		if (this.touchingCount == 0)
		{
			this.onStoppedTouching.Invoke();
		}
	}

	// Token: 0x040034DB RID: 13531
	[SerializeField]
	protected Collider2D collider;

	// Token: 0x040034DC RID: 13532
	[Space]
	[SerializeField]
	private UnityEvent onBreak;

	// Token: 0x040034DD RID: 13533
	[SerializeField]
	private UnityEvent onStartedTouching;

	// Token: 0x040034DE RID: 13534
	[SerializeField]
	private UnityEvent onStoppedTouching;

	// Token: 0x040034DF RID: 13535
	private int touchingCount;
}
