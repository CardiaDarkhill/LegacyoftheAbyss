using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
[Serializable]
public struct HitInstance
{
	// Token: 0x0600017B RID: 379 RVA: 0x0000860C File Offset: 0x0000680C
	public float GetActualDirection(Transform target, HitInstance.TargetType targetType)
	{
		if (this.Source != null && target != null && this.CircleDirection)
		{
			Vector2 vector = target.position - this.Source.transform.position;
			return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
		}
		if (!(this.Source != null) || !(target != null) || !this.MoveDirection)
		{
			return this.GetDirectionForType(targetType);
		}
		Rigidbody2D component = this.Source.GetComponent<Rigidbody2D>();
		if (component == null)
		{
			component = this.Source.transform.parent.gameObject.GetComponent<Rigidbody2D>();
		}
		if (component == null)
		{
			return this.GetDirectionForType(targetType);
		}
		float num = 0f;
		float num2 = 90f;
		if (component.linearVelocity.x < 0f)
		{
			num = 180f;
		}
		if (component.linearVelocity.y < 0f)
		{
			num2 = 270f;
		}
		float result;
		if (Math.Abs(component.linearVelocity.x) > Math.Abs(component.linearVelocity.y))
		{
			result = num;
		}
		else
		{
			result = num2;
		}
		return result;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x00008758 File Offset: 0x00006958
	public float GetDirectionForType(HitInstance.TargetType targetType)
	{
		switch (targetType)
		{
		case HitInstance.TargetType.Regular:
			return this.Direction;
		case HitInstance.TargetType.Corpse:
			if (!this.UseCorpseDirection)
			{
				return this.Direction;
			}
			return this.CorpseDirection;
		case HitInstance.TargetType.BouncePod:
			if (!this.CanTriggerBouncePod)
			{
				return this.Direction;
			}
			return 270f;
		case HitInstance.TargetType.Currency:
			return this.Direction;
		default:
			throw new ArgumentOutOfRangeException("targetType", targetType, null);
		}
	}

	// Token: 0x0600017D RID: 381 RVA: 0x000087C8 File Offset: 0x000069C8
	public float GetMagnitudeMultForType(HitInstance.TargetType targetType)
	{
		switch (targetType)
		{
		case HitInstance.TargetType.Regular:
			return this.MagnitudeMultiplier;
		case HitInstance.TargetType.Corpse:
			if (!this.UseCorpseMagnitudeMult)
			{
				return this.MagnitudeMultiplier;
			}
			return this.CorpseMagnitudeMultiplier;
		case HitInstance.TargetType.Currency:
			if (!this.UseCurrencyMagnitudeMult)
			{
				return this.MagnitudeMultiplier;
			}
			return this.CurrencyMagnitudeMult;
		}
		return this.MagnitudeMultiplier;
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00008828 File Offset: 0x00006A28
	public float GetOverriddenDirection(Transform target, HitInstance.TargetType targetType)
	{
		float? extraUpDirection = this.ExtraUpDirection;
		if (extraUpDirection == null)
		{
			return this.GetActualDirection(target, targetType);
		}
		return extraUpDirection.GetValueOrDefault();
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00008858 File Offset: 0x00006A58
	public HitInstance.HitDirection GetHitDirection(HitInstance.TargetType targetType)
	{
		float directionForType = this.GetDirectionForType(targetType);
		if (directionForType < 45f)
		{
			return HitInstance.HitDirection.Right;
		}
		if (directionForType < 135f)
		{
			return HitInstance.HitDirection.Up;
		}
		if (directionForType < 225f)
		{
			return HitInstance.HitDirection.Left;
		}
		return HitInstance.HitDirection.Down;
	}

	// Token: 0x06000180 RID: 384 RVA: 0x0000888C File Offset: 0x00006A8C
	public float GetHitDirectionAsAngle(HitInstance.TargetType targetType)
	{
		float directionForType = this.GetDirectionForType(targetType);
		if (directionForType < 45f)
		{
			return 0f;
		}
		if (directionForType < 135f)
		{
			return 90f;
		}
		if (directionForType < 225f)
		{
			return 180f;
		}
		return 270f;
	}

	// Token: 0x06000181 RID: 385 RVA: 0x000088D0 File Offset: 0x00006AD0
	public HitInstance.HitDirection GetActualHitDirection(Transform target, HitInstance.TargetType targetType)
	{
		float num;
		for (num = this.GetActualDirection(target, targetType); num >= 360f; num -= 360f)
		{
		}
		while (num < 0f)
		{
			num += 360f;
		}
		HitInstance.HitDirection result;
		if (num < 135f)
		{
			if (num >= 45f)
			{
				result = HitInstance.HitDirection.Up;
			}
			else
			{
				result = HitInstance.HitDirection.Right;
			}
		}
		else if (num >= 225f)
		{
			result = HitInstance.HitDirection.Down;
		}
		else
		{
			result = HitInstance.HitDirection.Left;
		}
		return result;
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00008938 File Offset: 0x00006B38
	public Vector2 GetHitDirectionAsVector(HitInstance.TargetType targetType)
	{
		switch (this.GetHitDirection(targetType))
		{
		case HitInstance.HitDirection.Left:
			return Vector2.left;
		case HitInstance.HitDirection.Right:
			return Vector2.right;
		case HitInstance.HitDirection.Up:
			return Vector2.up;
		case HitInstance.HitDirection.Down:
			return Vector2.down;
		default:
			return Vector2.zero;
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000183 RID: 387 RVA: 0x00008982 File Offset: 0x00006B82
	public bool IsNailDamage
	{
		get
		{
			return this.IsNailTag || this.AttackType == AttackTypes.Nail;
		}
	}

	// Token: 0x0400010B RID: 267
	public GameObject Source;

	// Token: 0x0400010C RID: 268
	public bool IsFirstHit;

	// Token: 0x0400010D RID: 269
	public AttackTypes AttackType;

	// Token: 0x0400010E RID: 270
	public NailElements NailElement;

	// Token: 0x0400010F RID: 271
	public NailImbuementConfig NailImbuement;

	// Token: 0x04000110 RID: 272
	public bool IsUsingNeedleDamageMult;

	// Token: 0x04000111 RID: 273
	public ToolItem RepresentingTool;

	// Token: 0x04000112 RID: 274
	public int PoisonDamageTicks;

	// Token: 0x04000113 RID: 275
	public int ZapDamageTicks;

	// Token: 0x04000114 RID: 276
	public int DamageScalingLevel;

	// Token: 0x04000115 RID: 277
	public ToolDamageFlags ToolDamageFlags;

	// Token: 0x04000116 RID: 278
	public bool CircleDirection;

	// Token: 0x04000117 RID: 279
	public int DamageDealt;

	// Token: 0x04000118 RID: 280
	public float StunDamage;

	// Token: 0x04000119 RID: 281
	public bool CanWeakHit;

	// Token: 0x0400011A RID: 282
	public float Direction;

	// Token: 0x0400011B RID: 283
	public bool UseCorpseDirection;

	// Token: 0x0400011C RID: 284
	public float CorpseDirection;

	// Token: 0x0400011D RID: 285
	public bool CanTriggerBouncePod;

	// Token: 0x0400011E RID: 286
	public bool UseBouncePodDirection;

	// Token: 0x0400011F RID: 287
	public float BouncePodDirection;

	// Token: 0x04000120 RID: 288
	public float? ExtraUpDirection;

	// Token: 0x04000121 RID: 289
	public bool IgnoreInvulnerable;

	// Token: 0x04000122 RID: 290
	public float MagnitudeMultiplier;

	// Token: 0x04000123 RID: 291
	public bool UseCorpseMagnitudeMult;

	// Token: 0x04000124 RID: 292
	public float CorpseMagnitudeMultiplier;

	// Token: 0x04000125 RID: 293
	public bool UseCurrencyMagnitudeMult;

	// Token: 0x04000126 RID: 294
	public float CurrencyMagnitudeMult;

	// Token: 0x04000127 RID: 295
	public float MoveAngle;

	// Token: 0x04000128 RID: 296
	public bool MoveDirection;

	// Token: 0x04000129 RID: 297
	public float Multiplier;

	// Token: 0x0400012A RID: 298
	public SpecialTypes SpecialType;

	// Token: 0x0400012B RID: 299
	public GameObject[] SlashEffectOverrides;

	// Token: 0x0400012C RID: 300
	public EnemyHitEffectsProfile.EffectsTypes HitEffectsType;

	// Token: 0x0400012D RID: 301
	public HitSilkGeneration SilkGeneration;

	// Token: 0x0400012E RID: 302
	public bool NonLethal;

	// Token: 0x0400012F RID: 303
	public bool RageHit;

	// Token: 0x04000130 RID: 304
	public bool CriticalHit;

	// Token: 0x04000131 RID: 305
	public bool HunterCombo;

	// Token: 0x04000132 RID: 306
	public bool IsManualTrigger;

	// Token: 0x04000133 RID: 307
	public bool ForceNotWeakHit;

	// Token: 0x04000134 RID: 308
	public bool IsHeroDamage;

	// Token: 0x04000135 RID: 309
	public bool IsNailTag;

	// Token: 0x04000136 RID: 310
	public bool IgnoreNailPosition;

	// Token: 0x04000137 RID: 311
	public bool IsHarpoon;

	// Token: 0x020013C9 RID: 5065
	public enum HitDirection
	{
		// Token: 0x040080BA RID: 32954
		Left,
		// Token: 0x040080BB RID: 32955
		Right,
		// Token: 0x040080BC RID: 32956
		Up,
		// Token: 0x040080BD RID: 32957
		Down
	}

	// Token: 0x020013CA RID: 5066
	public enum TargetType
	{
		// Token: 0x040080BF RID: 32959
		Regular,
		// Token: 0x040080C0 RID: 32960
		Corpse,
		// Token: 0x040080C1 RID: 32961
		BouncePod,
		// Token: 0x040080C2 RID: 32962
		Currency
	}
}
