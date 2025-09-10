using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020000BC RID: 188
public class SpriteFlashDistanceSilhouette : MonoBehaviour
{
	// Token: 0x060005F6 RID: 1526 RVA: 0x0001EC2C File Offset: 0x0001CE2C
	private void OnValidate()
	{
		if (this.distanceRange.Start < 0f)
		{
			this.distanceRange.Start = 0f;
		}
		if (this.distanceRange.End < this.distanceRange.Start)
		{
			this.distanceRange.End = this.distanceRange.Start;
		}
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x0001EC8C File Offset: 0x0001CE8C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.distanceRange.Start);
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.distanceRange.End);
		if (!this.hero)
		{
			HeroController silentInstance = HeroController.SilentInstance;
			if (silentInstance)
			{
				this.hero = silentInstance.transform;
			}
		}
		if (this.hero)
		{
			float t = this.GetT();
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, this.distanceRange.GetLerpedValue(t));
		}
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x0001ED44 File Offset: 0x0001CF44
	private void OnEnable()
	{
		if (!this.spriteFlash)
		{
			base.enabled = false;
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x0001ED5A File Offset: 0x0001CF5A
	private void Start()
	{
		this.hero = HeroController.instance.transform;
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x0001ED6C File Offset: 0x0001CF6C
	private void Update()
	{
		float t = this.GetT();
		this.spriteFlash.ExtraMixColor = this.color;
		this.spriteFlash.ExtraMixAmount = this.colorMixCurve.Evaluate(t);
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x0001EDA8 File Offset: 0x0001CFA8
	private float GetT()
	{
		Vector2 a = this.hero.position;
		Vector2 b = base.transform.position;
		return (Mathf.Clamp(Vector2.Distance(a, b), this.distanceRange.Start, this.distanceRange.End) - this.distanceRange.Start) / (this.distanceRange.End - this.distanceRange.Start);
	}

	// Token: 0x040005C6 RID: 1478
	[SerializeField]
	private SpriteFlash spriteFlash;

	// Token: 0x040005C7 RID: 1479
	[SerializeField]
	private MinMaxFloat distanceRange;

	// Token: 0x040005C8 RID: 1480
	[SerializeField]
	private Color color;

	// Token: 0x040005C9 RID: 1481
	[SerializeField]
	private AnimationCurve colorMixCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040005CA RID: 1482
	private Transform hero;
}
