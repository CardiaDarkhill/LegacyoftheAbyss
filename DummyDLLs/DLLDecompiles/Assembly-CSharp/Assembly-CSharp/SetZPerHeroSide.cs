using System;
using UnityEngine;

// Token: 0x02000279 RID: 633
public class SetZPerHeroSide : MonoBehaviour
{
	// Token: 0x1700025E RID: 606
	// (get) Token: 0x0600167C RID: 5756 RVA: 0x000652AB File Offset: 0x000634AB
	// (set) Token: 0x0600167D RID: 5757 RVA: 0x000652B3 File Offset: 0x000634B3
	public float ExpectedZ { get; private set; }

	// Token: 0x0600167E RID: 5758 RVA: 0x000652BC File Offset: 0x000634BC
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.origin, 0.2f);
	}

	// Token: 0x0600167F RID: 5759 RVA: 0x000652E3 File Offset: 0x000634E3
	private void Start()
	{
		this.hc = HeroController.instance;
		this.wasHeroRight = true;
		this.SetZ(true);
	}

	// Token: 0x06001680 RID: 5760 RVA: 0x00065300 File Offset: 0x00063500
	private void LateUpdate()
	{
		Vector2 vector = base.transform.TransformPoint(this.origin);
		bool flag = this.hc.transform.position.x > vector.x;
		if (this.checkScale && base.transform.lossyScale.x < 0f)
		{
			flag = !flag;
		}
		if (flag != this.wasHeroRight)
		{
			this.wasHeroRight = flag;
			this.SetZ(flag);
		}
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x00065382 File Offset: 0x00063582
	private void SetZ(bool isRight)
	{
		this.ExpectedZ = (isRight ? this.heroRightZ : this.heroLeftZ);
		base.transform.SetLocalPositionZ(this.ExpectedZ);
	}

	// Token: 0x040014EF RID: 5359
	[SerializeField]
	private Vector2 origin;

	// Token: 0x040014F0 RID: 5360
	[SerializeField]
	private bool checkScale;

	// Token: 0x040014F1 RID: 5361
	[Space]
	[SerializeField]
	private float heroLeftZ;

	// Token: 0x040014F2 RID: 5362
	[SerializeField]
	private float heroRightZ;

	// Token: 0x040014F3 RID: 5363
	private HeroController hc;

	// Token: 0x040014F4 RID: 5364
	private bool wasHeroRight;
}
