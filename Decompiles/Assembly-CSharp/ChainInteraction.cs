using System;
using System.Linq;
using UnityEngine;

// Token: 0x020004AF RID: 1199
public class ChainInteraction : ChainPushReaction, IHitResponder
{
	// Token: 0x06002B57 RID: 11095 RVA: 0x000BE1A0 File Offset: 0x000BC3A0
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		bool flag;
		if (damageInstance.IsNailDamage)
		{
			if (damageInstance.DamageDealt <= 0)
			{
				if (!damageInstance.CanWeakHit)
				{
					return IHitResponder.Response.None;
				}
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		if (Time.timeAsDouble < this.nextCutTime)
		{
			return IHitResponder.Response.None;
		}
		this.nextCutTime = Time.timeAsDouble + (double)this.cutDelay;
		if (!this.player)
		{
			this.player = HeroController.instance.transform;
		}
		Vector3 position = damageInstance.Source.transform.position;
		float y = position.y;
		Vector3 position2 = base.transform.position;
		if (this.cutEffectPrefab && !flag && (!this.hitEffectTimer || this.hitEffectTimer.HasEnded))
		{
			Vector3 position3 = new Vector3(position2.x, y, position2.z);
			this.cutEffectPrefab.Spawn(position3);
			if (this.hitEffectTimer)
			{
				this.hitEffectTimer.ResetTimer();
			}
		}
		if (this.brokenParticle && !flag)
		{
			this.brokenParticle.gameObject.SetActive(false);
			Transform transform = this.brokenParticle.transform;
			Vector3 position4 = this.positionAtNail ? new Vector3(position2.x, y, position2.z) : transform.position;
			Vector3 eulerAngles = transform.eulerAngles;
			if (Math.Abs(this.startAngle) < Mathf.Epsilon)
			{
				this.startAngle = eulerAngles.z;
			}
			eulerAngles.z = this.startAngle;
			if (base.transform.position.x > this.player.position.x)
			{
				eulerAngles.z += 180f;
			}
			transform.eulerAngles = eulerAngles;
			transform.position = position4;
			this.brokenParticle.gameObject.SetActive(true);
		}
		if (this.canBreak && !flag)
		{
			if (this.brokenObject)
			{
				this.brokenObject.SetActive(true);
				this.ApplyCollisionForce(this.brokenObject, damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular), this.cutApplyForce, y);
			}
			base.gameObject.SetActive(false);
			if (base.Sound)
			{
				base.Sound.PlayHitSound(position);
			}
		}
		else
		{
			this.ApplyCollisionForce(base.gameObject, damageInstance.GetHitDirectionAsVector(HitInstance.TargetType.Regular), this.cutApplyForce, y);
			base.DisableLinks(this.player);
			if (base.Sound)
			{
				base.Sound.PlayBrokenHitSound(position);
			}
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06002B58 RID: 11096 RVA: 0x000BE434 File Offset: 0x000BC634
	public void ApplyCollisionForce(GameObject parent, Vector2 hitDirection, float cutForce, float hitYPos)
	{
		Rigidbody2D rigidbody2D = (from b in parent.GetComponentsInChildren<Rigidbody2D>()
		where !b.isKinematic
		orderby b.GetComponent<ChainLinkInteraction>() != null descending, Mathf.Abs(b.position.y - hitYPos)
		select b).FirstOrDefault<Rigidbody2D>();
		if (rigidbody2D == null)
		{
			return;
		}
		rigidbody2D.linearVelocity = Vector2.zero;
		rigidbody2D.angularVelocity = 0f;
		rigidbody2D.AddForceAtPosition(hitDirection * cutForce, new Vector2(rigidbody2D.position.x, hitYPos), ForceMode2D.Impulse);
	}

	// Token: 0x04002CA6 RID: 11430
	[Space]
	[SerializeField]
	private GameObject cutEffectPrefab;

	// Token: 0x04002CA7 RID: 11431
	[SerializeField]
	private float cutDelay = 0.5f;

	// Token: 0x04002CA8 RID: 11432
	[SerializeField]
	private TimerGroup hitEffectTimer;

	// Token: 0x04002CA9 RID: 11433
	[Space]
	[SerializeField]
	private ParticleSystem brokenParticle;

	// Token: 0x04002CAA RID: 11434
	[SerializeField]
	private bool positionAtNail = true;

	// Token: 0x04002CAB RID: 11435
	[SerializeField]
	private float cutApplyForce = 10f;

	// Token: 0x04002CAC RID: 11436
	[SerializeField]
	private GameObject brokenObject;

	// Token: 0x04002CAD RID: 11437
	[SerializeField]
	private bool canBreak = true;

	// Token: 0x04002CAE RID: 11438
	private double nextCutTime;

	// Token: 0x04002CAF RID: 11439
	private Transform player;

	// Token: 0x04002CB0 RID: 11440
	private float startAngle;
}
