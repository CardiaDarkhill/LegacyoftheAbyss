using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000302 RID: 770
public class LifebloodState : MonoBehaviour
{
	// Token: 0x06001B76 RID: 7030 RVA: 0x0008042A File Offset: 0x0007E62A
	private void Awake()
	{
		this.persistent = (base.GetComponent<PersistentBoolItem>() ?? base.gameObject.AddComponent<PersistentBoolItem>());
	}

	// Token: 0x06001B77 RID: 7031 RVA: 0x00080448 File Offset: 0x0007E648
	private void Start()
	{
		this.healthManager = base.gameObject.GetComponent<HealthManager>();
		this.spriteFlash = base.gameObject.GetComponent<SpriteFlash>();
		this.healthManager.TookDamage += this.TakeDamage;
		this.tagDamageTaker = base.GetComponent<TagDamageTaker>();
		Color lifebloodTintColour = Effects.LifebloodTintColour;
		this.healEffect = Object.Instantiate<GameObject>(Effects.LifebloodHealEffect, base.transform.position, base.transform.rotation, base.transform);
		this.healEffect.transform.position = new Vector3(this.healEffect.transform.position.x, this.healEffect.transform.position.y, 0.0011f);
		EnemyHitEffectsRegular component = base.gameObject.GetComponent<EnemyHitEffectsRegular>();
		if (component != null)
		{
			component.OverrideBloodColor = true;
			component.BloodColorOverride = lifebloodTintColour;
		}
		this.healthManager.hp = (int)((float)this.healthManager.hp * 1f);
		this.maxHP = this.healthManager.hp;
		this.healthManager.SetShellShards(0);
	}

	// Token: 0x06001B78 RID: 7032 RVA: 0x00080570 File Offset: 0x0007E770
	private void Update()
	{
		if (this.dead || !this.healingIsActive || (this.tagDamageTaker && this.tagDamageTaker.IsTagged))
		{
			return;
		}
		int num = this.healthManager.hp;
		if (num >= this.maxHP)
		{
			this.timer = 0f;
			return;
		}
		if (this.timer < 0.75f)
		{
			this.timer += Time.deltaTime;
			return;
		}
		num += this.healAmount;
		if (num > this.maxHP)
		{
			num = this.maxHP;
		}
		this.healthManager.hp = num;
		this.healEffect.SetActive(true);
		this.spriteFlash.flashHealBlue();
		this.timer -= 0.75f;
	}

	// Token: 0x06001B79 RID: 7033 RVA: 0x00080637 File Offset: 0x0007E837
	private void TakeDamage()
	{
		this.timer = 0f;
	}

	// Token: 0x06001B7A RID: 7034 RVA: 0x00080644 File Offset: 0x0007E844
	private void OnDeath()
	{
		LifebloodGlob lifebloodGlob = Effects.LifebloodGlob.Spawn(base.transform.position);
		this.dead = true;
		PersistentItemData<bool> itemData = this.persistent.ItemData;
		lifebloodGlob.SetTempQuestHandler(delegate
		{
			itemData.IsSemiPersistent = false;
			itemData.Value = true;
			SceneData.instance.PersistentBools.SetValue(itemData);
		});
	}

	// Token: 0x06001B7B RID: 7035 RVA: 0x00080695 File Offset: 0x0007E895
	public void SetIsLifebloodHealing(bool set)
	{
		this.timer = 0f;
		this.healingIsActive = set;
	}

	// Token: 0x06001B7C RID: 7036 RVA: 0x000806A9 File Offset: 0x0007E8A9
	public void UpdateMaxHP(int new_maxHP)
	{
		this.maxHP = new_maxHP;
	}

	// Token: 0x04001A79 RID: 6777
	public int healAmount = 5;

	// Token: 0x04001A7A RID: 6778
	private const float HEAL_TIME = 0.75f;

	// Token: 0x04001A7B RID: 6779
	private const float HP_MULTIPLIER = 1f;

	// Token: 0x04001A7C RID: 6780
	private bool dead;

	// Token: 0x04001A7D RID: 6781
	private bool healingIsActive = true;

	// Token: 0x04001A7E RID: 6782
	private int maxHP;

	// Token: 0x04001A7F RID: 6783
	private float timer;

	// Token: 0x04001A80 RID: 6784
	private HealthManager healthManager;

	// Token: 0x04001A81 RID: 6785
	private SpriteFlash spriteFlash;

	// Token: 0x04001A82 RID: 6786
	private GameObject healEffect;

	// Token: 0x04001A83 RID: 6787
	private TagDamageTaker tagDamageTaker;

	// Token: 0x04001A84 RID: 6788
	private PersistentBoolItem persistent;
}
