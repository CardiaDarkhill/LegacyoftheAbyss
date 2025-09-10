using System;
using UnityEngine;

// Token: 0x020003FC RID: 1020
public class KillOnContact : MonoBehaviour
{
	// Token: 0x060022BC RID: 8892 RVA: 0x0009F950 File Offset: 0x0009DB50
	private void OnCollisionEnter2D(Collision2D collision)
	{
		HealthManager component = collision.gameObject.GetComponent<HealthManager>();
		if (component)
		{
			component.Die(null, AttackTypes.Generic, true);
			return;
		}
		ICurrencyObject component2 = collision.gameObject.GetComponent<ICurrencyObject>();
		if (component2 != null)
		{
			component2.Disable(0f);
			return;
		}
		HeroController component3 = collision.gameObject.GetComponent<HeroController>();
		if (component3 && component3.isHeroInPosition)
		{
			base.StartCoroutine(component3.HazardRespawn());
		}
	}
}
