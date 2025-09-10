using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001D9 RID: 473
public class TagDamageTaker : MonoBehaviour
{
	// Token: 0x17000207 RID: 519
	// (get) Token: 0x0600127C RID: 4732 RVA: 0x00055E85 File Offset: 0x00054085
	public IReadOnlyDictionary<DamageTag, DamageTagInfo> TaggedDamage
	{
		get
		{
			return this.taggedDamage;
		}
	}

	// Token: 0x17000208 RID: 520
	// (get) Token: 0x0600127D RID: 4733 RVA: 0x00055E8D File Offset: 0x0005408D
	public bool IsTagged
	{
		get
		{
			return this.taggedDamage.Count > 0;
		}
	}

	// Token: 0x0600127E RID: 4734 RVA: 0x00055E9D File Offset: 0x0005409D
	private void Awake()
	{
		this.collider = base.GetComponent<Collider2D>();
		this.isEnemy = (base.gameObject.layer == 11);
	}

	// Token: 0x0600127F RID: 4735 RVA: 0x00055EC0 File Offset: 0x000540C0
	public static TagDamageTaker Add(GameObject gameObject, ITagDamageTakerOwner newOwner)
	{
		TagDamageTaker tagDamageTaker = gameObject.AddComponent<TagDamageTaker>();
		tagDamageTaker.owner = newOwner;
		return tagDamageTaker;
	}

	// Token: 0x06001280 RID: 4736 RVA: 0x00055ED0 File Offset: 0x000540D0
	public void AddDamageTagToStack(DamageTag damageTag, int hitAmountOverride = 0)
	{
		if (!damageTag)
		{
			Debug.LogError("DamageTag was null", this);
			return;
		}
		if (!this.collider)
		{
			this.collider = base.GetComponent<Collider2D>();
		}
		DamageTagInfo damageTagInfo;
		if (this.taggedDamage.TryGetValue(damageTag, out damageTagInfo))
		{
			damageTagInfo.HitsLeft = ((hitAmountOverride > 0) ? Mathf.Max(damageTagInfo.HitsLeft, hitAmountOverride) : damageTag.TotalHitLimit);
			if (damageTagInfo.RemoveAfterNextHit)
			{
				damageTagInfo.RemoveAfterNextHit = false;
				damageTag.OnHit(this.owner);
				damageTagInfo.Stacked--;
			}
			if (damageTagInfo.hasLerpEmission)
			{
				damageTagInfo.RefreshEmission(Mathf.Max(0f, (float)(damageTagInfo.NextHitTime - Time.timeAsDouble)) + damageTag.DelayPerHit * (float)damageTag.TotalHitLimit);
			}
		}
		else
		{
			GameObject spawnedLoopEffect;
			damageTag.OnBegin(this.owner, out spawnedLoopEffect);
			damageTagInfo = new DamageTagInfo
			{
				NextHitTime = Time.timeAsDouble + (double)damageTag.StartDelay,
				SpawnedLoopEffect = spawnedLoopEffect,
				HitsLeft = ((hitAmountOverride > 0) ? hitAmountOverride : damageTag.TotalHitLimit)
			};
			damageTagInfo.CheckLerpEmission();
			this.taggedDamage[damageTag] = damageTagInfo;
		}
		if (damageTag.DamageCooldownTimer)
		{
			damageTagInfo.NextHitTime = Helper.Max(damageTagInfo.NextHitTime, damageTag.DamageCooldownTimer.EndTime);
		}
		damageTagInfo.Stacked++;
	}

	// Token: 0x06001281 RID: 4737 RVA: 0x00056028 File Offset: 0x00054228
	public void RemoveDamageTagFromStack(DamageTag damageTag, bool oneMoreHit = false)
	{
		if (!damageTag)
		{
			Debug.LogError("DamageTag was null", this);
			return;
		}
		DamageTagInfo damageTagInfo;
		if (!this.taggedDamage.TryGetValue(damageTag, out damageTagInfo))
		{
			return;
		}
		if (oneMoreHit)
		{
			damageTagInfo.RemoveAfterNextHit = true;
			return;
		}
		damageTagInfo.Stacked--;
		if (damageTagInfo.Stacked > 0)
		{
			return;
		}
		this.taggedDamageRemove.Enqueue(damageTag);
		damageTagInfo.StopLoopEffect();
	}

	// Token: 0x06001282 RID: 4738 RVA: 0x00056090 File Offset: 0x00054290
	public void Tick(bool canTakeDamage)
	{
		this.ApplyQueued();
		if (this.taggedDamage.Count <= 0)
		{
			return;
		}
		bool flag = !canTakeDamage || (this.isEnemy && base.gameObject.layer != 11) || (!this.ignoreColliderState && this.collider && !this.collider.enabled);
		foreach (KeyValuePair<DamageTag, DamageTagInfo> keyValuePair in this.taggedDamage)
		{
			TagDamageTaker.<>c__DisplayClass14_0 CS$<>8__locals1;
			CS$<>8__locals1.damageTag = keyValuePair.Key;
			CS$<>8__locals1.info = keyValuePair.Value;
			if (flag && (CS$<>8__locals1.damageTag.TotalHitLimit > 0 || CS$<>8__locals1.info.RemoveAfterNextHit))
			{
				this.<Tick>g__Remove|14_0(ref CS$<>8__locals1);
			}
			else if (Time.timeAsDouble >= CS$<>8__locals1.info.NextHitTime)
			{
				CS$<>8__locals1.damageTag.OnHit(this.owner);
				CS$<>8__locals1.info.NextHitTime = Time.timeAsDouble + (double)CS$<>8__locals1.damageTag.DelayPerHit;
				if (CS$<>8__locals1.damageTag.DamageCooldownTimer)
				{
					CS$<>8__locals1.info.NextHitTime = Helper.Max(CS$<>8__locals1.info.NextHitTime, CS$<>8__locals1.damageTag.DamageCooldownTimer.EndTime);
				}
				int hitsLeft = CS$<>8__locals1.info.HitsLeft;
				if (hitsLeft > 0)
				{
					CS$<>8__locals1.info.HitsLeft--;
				}
				if ((CS$<>8__locals1.info.HitsLeft <= 0 && hitsLeft > 0) || CS$<>8__locals1.info.RemoveAfterNextHit)
				{
					this.<Tick>g__Remove|14_0(ref CS$<>8__locals1);
				}
			}
		}
		this.ApplyQueued();
	}

	// Token: 0x06001283 RID: 4739 RVA: 0x00056264 File Offset: 0x00054464
	private void ApplyQueued()
	{
		while (this.taggedDamageRemove.Count > 0)
		{
			this.taggedDamage.Remove(this.taggedDamageRemove.Dequeue());
		}
		this.taggedDamageRemove.Clear();
	}

	// Token: 0x06001284 RID: 4740 RVA: 0x00056298 File Offset: 0x00054498
	public void ClearTagDamage()
	{
		this.taggedDamageRemove.Clear();
		foreach (KeyValuePair<DamageTag, DamageTagInfo> keyValuePair in this.taggedDamage)
		{
			keyValuePair.Value.StopLoopEffect();
		}
		this.taggedDamage.Clear();
	}

	// Token: 0x06001285 RID: 4741 RVA: 0x00056308 File Offset: 0x00054508
	public void SetIgnoreColliderState(bool state)
	{
		this.ignoreColliderState = state;
	}

	// Token: 0x06001287 RID: 4743 RVA: 0x0005632F File Offset: 0x0005452F
	[CompilerGenerated]
	private void <Tick>g__Remove|14_0(ref TagDamageTaker.<>c__DisplayClass14_0 A_1)
	{
		this.taggedDamageRemove.Enqueue(A_1.damageTag);
		A_1.info.StopLoopEffect();
	}

	// Token: 0x0400113D RID: 4413
	private bool ignoreColliderState;

	// Token: 0x0400113E RID: 4414
	private readonly Dictionary<DamageTag, DamageTagInfo> taggedDamage = new Dictionary<DamageTag, DamageTagInfo>();

	// Token: 0x0400113F RID: 4415
	private readonly Queue<DamageTag> taggedDamageRemove = new Queue<DamageTag>();

	// Token: 0x04001140 RID: 4416
	private ITagDamageTakerOwner owner;

	// Token: 0x04001141 RID: 4417
	private Collider2D collider;

	// Token: 0x04001142 RID: 4418
	private bool isEnemy;
}
