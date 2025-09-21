using System;
using UnityEngine;

// Token: 0x020004A7 RID: 1191
public class BreakablePoleSimple : MonoBehaviour, IHitResponder
{
	// Token: 0x06002B37 RID: 11063 RVA: 0x000BD6EA File Offset: 0x000BB8EA
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06002B38 RID: 11064 RVA: 0x000BD6F8 File Offset: 0x000BB8F8
	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > 1f)
		{
			if (this.source)
			{
				this.source.enabled = false;
			}
			Collider2D component = base.GetComponent<Collider2D>();
			if (component)
			{
				component.enabled = false;
			}
			base.enabled = false;
		}
	}

	// Token: 0x06002B39 RID: 11065 RVA: 0x000BD760 File Offset: 0x000BB960
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.activated)
		{
			return IHitResponder.Response.None;
		}
		bool flag = false;
		float num = 1f;
		if (damageInstance.IsNailDamage)
		{
			float overriddenDirection = damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular);
			if (overriddenDirection < 45f)
			{
				flag = true;
				num = 1f;
			}
			else if (overriddenDirection < 135f)
			{
				flag = false;
			}
			else if (overriddenDirection < 225f)
			{
				flag = true;
				num = -1f;
			}
			else if (overriddenDirection < 360f)
			{
				flag = false;
			}
		}
		else if (damageInstance.AttackType == AttackTypes.Spell)
		{
			flag = true;
		}
		if (!flag)
		{
			return IHitResponder.Response.None;
		}
		SpriteRenderer component = base.GetComponent<SpriteRenderer>();
		if (component)
		{
			component.enabled = false;
		}
		if (this.bottom)
		{
			this.bottom.SetActive(true);
		}
		if (this.top)
		{
			this.top.SetActive(true);
			float num2 = Random.Range(this.angleMin, this.angleMax);
			Vector2 linearVelocity;
			linearVelocity.x = this.speed * Mathf.Cos(num2 * 0.017453292f) * num;
			linearVelocity.y = this.speed * Mathf.Sin(num2 * 0.017453292f);
			Rigidbody2D component2 = this.top.GetComponent<Rigidbody2D>();
			if (component2)
			{
				component2.linearVelocity = linearVelocity;
			}
		}
		if (this.slashEffectPrefab)
		{
			GameObject gameObject = this.slashEffectPrefab.Spawn(base.transform.position);
			gameObject.transform.SetScaleX(num);
			gameObject.transform.SetRotationZ(Random.Range(this.slashAngleMin, this.slashAngleMax));
		}
		if (this.source)
		{
			this.source.pitch = Random.Range(this.audioPitchMin, this.audioPitchMax);
			this.source.Play();
		}
		this.activated = true;
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x04002C79 RID: 11385
	[SerializeField]
	private GameObject bottom;

	// Token: 0x04002C7A RID: 11386
	[SerializeField]
	private GameObject top;

	// Token: 0x04002C7B RID: 11387
	[SerializeField]
	private float speed = 17f;

	// Token: 0x04002C7C RID: 11388
	[SerializeField]
	private float angleMin = 40f;

	// Token: 0x04002C7D RID: 11389
	[SerializeField]
	private float angleMax = 60f;

	// Token: 0x04002C7E RID: 11390
	[Space]
	[SerializeField]
	private GameObject slashEffectPrefab;

	// Token: 0x04002C7F RID: 11391
	[SerializeField]
	private float slashAngleMin = 340f;

	// Token: 0x04002C80 RID: 11392
	[SerializeField]
	private float slashAngleMax = 380f;

	// Token: 0x04002C81 RID: 11393
	[Space]
	[SerializeField]
	private float audioPitchMin = 0.85f;

	// Token: 0x04002C82 RID: 11394
	[SerializeField]
	private float audioPitchMax = 1.15f;

	// Token: 0x04002C83 RID: 11395
	private bool activated;

	// Token: 0x04002C84 RID: 11396
	private AudioSource source;
}
