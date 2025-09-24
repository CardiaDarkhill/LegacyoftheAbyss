using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x020001A4 RID: 420
public class DamageEffectTicker : MonoBehaviour
{
	// Token: 0x06001046 RID: 4166 RVA: 0x0004E641 File Offset: 0x0004C841
	private void OnEnable()
	{
		this.enemyList.Clear();
		this.timeAlive = 0f;
	}

	// Token: 0x06001047 RID: 4167 RVA: 0x0004E65C File Offset: 0x0004C85C
	private void Update()
	{
		this.timeAlive += Time.deltaTime;
		if (this.damageStopTime > 0f && this.timeAlive >= this.damageStopTime)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer >= this.damageInterval)
		{
			for (int i = 0; i < this.enemyList.Count; i++)
			{
				GameObject gameObject = this.enemyList[i];
				if (!(gameObject == null))
				{
					HealthManager component = gameObject.GetComponent<HealthManager>();
					if (component)
					{
						component.ApplyExtraDamage(this.damageAmount);
						this.DoFlashOnEnemy(gameObject);
					}
				}
			}
			this.timer -= this.damageInterval;
		}
	}

	// Token: 0x06001048 RID: 4168 RVA: 0x0004E718 File Offset: 0x0004C918
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (this.enemySpriteFlash == DamageEffectTicker.SpriteFlashMethods.Coal && otherCollider.GetComponent<HealthManager>().ImmuneToCoal)
		{
			return;
		}
		this.enemyList.AddIfNotPresent(otherCollider.gameObject);
	}

	// Token: 0x06001049 RID: 4169 RVA: 0x0004E743 File Offset: 0x0004C943
	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		this.enemyList.Remove(otherCollider.gameObject);
	}

	// Token: 0x0600104A RID: 4170 RVA: 0x0004E757 File Offset: 0x0004C957
	public void EmptyDamageList()
	{
		this.enemyList.Clear();
	}

	// Token: 0x0600104B RID: 4171 RVA: 0x0004E764 File Offset: 0x0004C964
	public void SetDamageInterval(float newInterval)
	{
		this.damageInterval = newInterval;
	}

	// Token: 0x0600104C RID: 4172 RVA: 0x0004E770 File Offset: 0x0004C970
	private void DoFlashOnEnemy(GameObject enemy)
	{
		Effects.EnemyDamageTickSoundTable.SpawnAndPlayOneShot(enemy.transform.position, false);
		if (this.enemySpriteFlash == DamageEffectTicker.SpriteFlashMethods.None)
		{
			return;
		}
		SpriteFlash component = enemy.GetComponent<SpriteFlash>();
		if (!component)
		{
			return;
		}
		switch (this.enemySpriteFlash)
		{
		case DamageEffectTicker.SpriteFlashMethods.Curse:
			component.FlashWitchPoison();
			return;
		case DamageEffectTicker.SpriteFlashMethods.Dazzle:
			component.FlashDazzleQuick();
			return;
		case DamageEffectTicker.SpriteFlashMethods.Coal:
			component.FlashCoal();
			return;
		default:
			throw new NotImplementedException();
		}
	}

	// Token: 0x04000FC3 RID: 4035
	[SerializeField]
	private float damageInterval = 0.2f;

	// Token: 0x04000FC4 RID: 4036
	[SerializeField]
	private int damageAmount = 1;

	// Token: 0x04000FC5 RID: 4037
	[SerializeField]
	private DamageEffectTicker.SpriteFlashMethods enemySpriteFlash;

	// Token: 0x04000FC6 RID: 4038
	[SerializeField]
	private float damageStopTime;

	// Token: 0x04000FC7 RID: 4039
	private float timeAlive;

	// Token: 0x04000FC8 RID: 4040
	private float timer;

	// Token: 0x04000FC9 RID: 4041
	private List<GameObject> enemyList = new List<GameObject>();

	// Token: 0x020014E3 RID: 5347
	private enum SpriteFlashMethods
	{
		// Token: 0x04008516 RID: 34070
		None,
		// Token: 0x04008517 RID: 34071
		Curse,
		// Token: 0x04008518 RID: 34072
		Dazzle,
		// Token: 0x04008519 RID: 34073
		Coal
	}
}
