using System;
using GlobalEnums;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004AC RID: 1196
public class BreakOnHazard : MonoBehaviour
{
	// Token: 0x06002B47 RID: 11079 RVA: 0x000BDD48 File Offset: 0x000BBF48
	private void Start()
	{
		if (!this.migrateUpOnStart)
		{
			return;
		}
		GameObject gameObject = base.transform.parent.gameObject;
		if (!gameObject.GetComponent<BreakOnHazard>())
		{
			BreakOnHazard breakOnHazard = gameObject.AddComponent<BreakOnHazard>();
			breakOnHazard.hazardMask = this.hazardMask;
			breakOnHazard.breakEffectPrefab = this.breakEffectPrefab;
			breakOnHazard.breakAudioClipTable = this.breakAudioClipTable;
			breakOnHazard.OnBreak = this.OnBreak;
		}
		Object.Destroy(this);
	}

	// Token: 0x06002B48 RID: 11080 RVA: 0x000BDDB7 File Offset: 0x000BBFB7
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Geo"))
		{
			return;
		}
		if (BreakOnHazard.IsCogDamager(collision.gameObject))
		{
			this.Break();
			return;
		}
		this.HandleCollision(collision);
	}

	// Token: 0x06002B49 RID: 11081 RVA: 0x000BDDE2 File Offset: 0x000BBFE2
	private void OnCollisionEnter2D(Collision2D collision)
	{
		this.HandleCollision(collision.collider);
	}

	// Token: 0x06002B4A RID: 11082 RVA: 0x000BDDF0 File Offset: 0x000BBFF0
	private void HandleCollision(Collider2D col)
	{
		if (col.gameObject.CompareTag("Geo"))
		{
			return;
		}
		DamageHero damageHero;
		if (!DamageHero.TryGet(col.gameObject, out damageHero))
		{
			return;
		}
		HazardType hazardType = damageHero.hazardType;
		if (!this.hazardMask.IsBitSet((int)hazardType))
		{
			return;
		}
		this.Break();
	}

	// Token: 0x06002B4B RID: 11083 RVA: 0x000BDE3C File Offset: 0x000BC03C
	public void Break()
	{
		if (this.breakEffectPrefab)
		{
			this.breakEffectPrefab.Spawn(base.transform.position);
		}
		if (this.breakAudioClipTable)
		{
			this.breakAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		if (!this.doNotRecycle)
		{
			base.gameObject.Recycle();
		}
		this.OnBreak.Invoke();
	}

	// Token: 0x06002B4C RID: 11084 RVA: 0x000BDEB0 File Offset: 0x000BC0B0
	public static bool IsCogDamager(GameObject target)
	{
		DamageEnemies component = target.GetComponent<DamageEnemies>();
		return component && (component.multiHitter && component.attackType == AttackTypes.Generic) && (component.contactFSMEvent == "COG ENTER" || target.CompareTag("Breaker"));
	}

	// Token: 0x04002C92 RID: 11410
	[SerializeField]
	[EnumPickerBitmask(typeof(HazardType))]
	private int hazardMask;

	// Token: 0x04002C93 RID: 11411
	[Space]
	[SerializeField]
	private GameObject breakEffectPrefab;

	// Token: 0x04002C94 RID: 11412
	[SerializeField]
	private RandomAudioClipTable breakAudioClipTable;

	// Token: 0x04002C95 RID: 11413
	[SerializeField]
	private bool doNotRecycle;

	// Token: 0x04002C96 RID: 11414
	[SerializeField]
	private bool migrateUpOnStart;

	// Token: 0x04002C97 RID: 11415
	[Space]
	public UnityEvent OnBreak;
}
