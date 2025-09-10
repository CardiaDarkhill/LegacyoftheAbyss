using System;
using UnityEngine;

// Token: 0x020003C4 RID: 964
public class HeroBouncer : MonoBehaviour
{
	// Token: 0x060020A4 RID: 8356 RVA: 0x000960D8 File Offset: 0x000942D8
	private void OnCollisionStay2D(Collision2D other)
	{
		HeroController component = other.gameObject.GetComponent<HeroController>();
		if (!component)
		{
			return;
		}
		switch (this.behaviour)
		{
		case HeroBouncer.Behaviours.BounceUp:
			component.BounceShort();
			if (Random.Range(0, 2) == 0)
			{
				component.RecoilLeft();
			}
			else
			{
				component.RecoilRight();
			}
			break;
		case HeroBouncer.Behaviours.RecoilLeft:
			component.RecoilLeft();
			break;
		case HeroBouncer.Behaviours.RecoilRight:
			component.RecoilRight();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		if (this.bounceEffectPrefab)
		{
			this.bounceEffectPrefab.Spawn().transform.SetPosition2D(component.transform.TransformPoint(this.bounceEffectOffset));
		}
	}

	// Token: 0x04001FDF RID: 8159
	[SerializeField]
	private GameObject bounceEffectPrefab;

	// Token: 0x04001FE0 RID: 8160
	[SerializeField]
	private Vector2 bounceEffectOffset;

	// Token: 0x04001FE1 RID: 8161
	[SerializeField]
	private HeroBouncer.Behaviours behaviour;

	// Token: 0x0200167D RID: 5757
	private enum Behaviours
	{
		// Token: 0x04008B04 RID: 35588
		BounceUp,
		// Token: 0x04008B05 RID: 35589
		RecoilLeft,
		// Token: 0x04008B06 RID: 35590
		RecoilRight
	}
}
