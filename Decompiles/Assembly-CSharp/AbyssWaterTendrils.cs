using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
public class AbyssWaterTendrils : MonoBehaviour
{
	// Token: 0x06000699 RID: 1689 RVA: 0x000215EE File Offset: 0x0001F7EE
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		Gizmos.color = Color.grey;
		Gizmos.DrawWireSphere(position, this.appearRadius);
		Gizmos.color = Color.white;
		Gizmos.DrawWireSphere(position, this.flowerDisappearRadius);
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00021628 File Offset: 0x0001F828
	private void Start()
	{
		this.hc = HeroController.instance;
		this.worldPos = base.transform.position;
		this.appearRadiusSqr = this.appearRadius * this.appearRadius;
		this.flowerDisappearRadiusSqr = this.flowerDisappearRadius * this.flowerDisappearRadius;
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x00021677 File Offset: 0x0001F877
	private void OnValidate()
	{
		this.appearRadiusSqr = this.appearRadius * this.appearRadius;
		this.flowerDisappearRadiusSqr = this.flowerDisappearRadius * this.flowerDisappearRadius;
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x000216A0 File Offset: 0x0001F8A0
	private void Update()
	{
		if (Time.frameCount != AbyssWaterTendrils._heroPosFrame)
		{
			AbyssWaterTendrils._heroPos = this.hc.transform.position;
			AbyssWaterTendrils._hasWhiteFlower = this.hc.playerData.HasWhiteFlower;
			AbyssWaterTendrils._heroPosFrame = Time.frameCount;
		}
		float num = Vector2.SqrMagnitude(this.worldPos - AbyssWaterTendrils._heroPos);
		bool flag = num <= this.appearRadiusSqr && (!AbyssWaterTendrils._hasWhiteFlower || num >= this.flowerDisappearRadiusSqr);
		this.dropAnimator.SetBool(AbyssWaterTendrils._shouldAppearId, flag);
		if (flag && !this.wasAppeared && this.dropAnimator.cullingMode == AnimatorCullingMode.CullCompletely)
		{
			this.dropAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			if (Random.Range(0, 2) == 0)
			{
				base.transform.FlipLocalScale(true, false, false);
			}
		}
		this.wasAppeared = flag;
	}

	// Token: 0x04000669 RID: 1641
	[SerializeField]
	private float appearRadius;

	// Token: 0x0400066A RID: 1642
	[SerializeField]
	private float flowerDisappearRadius;

	// Token: 0x0400066B RID: 1643
	[SerializeField]
	private Animator dropAnimator;

	// Token: 0x0400066C RID: 1644
	private HeroController hc;

	// Token: 0x0400066D RID: 1645
	private Vector3 worldPos;

	// Token: 0x0400066E RID: 1646
	private bool wasAppeared;

	// Token: 0x0400066F RID: 1647
	private static Vector3 _heroPos;

	// Token: 0x04000670 RID: 1648
	private static bool _hasWhiteFlower;

	// Token: 0x04000671 RID: 1649
	private static int _heroPosFrame;

	// Token: 0x04000672 RID: 1650
	private static readonly int _shouldAppearId = Animator.StringToHash("ShouldAppear");

	// Token: 0x04000673 RID: 1651
	private float appearRadiusSqr;

	// Token: 0x04000674 RID: 1652
	private float flowerDisappearRadiusSqr;
}
