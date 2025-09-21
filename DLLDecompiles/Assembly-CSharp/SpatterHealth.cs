using System;
using UnityEngine;

// Token: 0x02000557 RID: 1367
public class SpatterHealth : MonoBehaviour
{
	// Token: 0x060030E6 RID: 12518 RVA: 0x000D876E File Offset: 0x000D696E
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x060030E7 RID: 12519 RVA: 0x000D877C File Offset: 0x000D697C
	private void OnEnable()
	{
		this.scaleModifier = Random.Range(1.3f, 1.6f);
		this.currentState = SpatterHealth.States.Init;
		this.fallTimeLeft = Random.Range(0.25f, 0.45f);
		this.fireSpeed = 0.1f;
		if (this.healthGetEffect)
		{
			this.healthGetEffect.SetActive(false);
		}
		this.hc = HeroController.instance;
	}

	// Token: 0x060030E8 RID: 12520 RVA: 0x000D87EC File Offset: 0x000D69EC
	private void FixedUpdate()
	{
		this.FaceAngle();
		this.ProjectileSquash();
		switch (this.currentState)
		{
		case SpatterHealth.States.Init:
			base.transform.SetParent(null, true);
			this.currentState = SpatterHealth.States.Fall;
			return;
		case SpatterHealth.States.Fall:
			this.fallTimeLeft -= Time.deltaTime;
			if (this.fallTimeLeft <= 0f)
			{
				this.currentState = SpatterHealth.States.Decel;
				this.body.gravityScale = 0f;
				return;
			}
			break;
		case SpatterHealth.States.Decel:
		{
			Vector2 vector = this.body.linearVelocity;
			vector *= 0.75f;
			this.body.linearVelocity = vector;
			if (vector.magnitude <= 0.3f)
			{
				this.currentState = SpatterHealth.States.Attract;
				return;
			}
			break;
		}
		case SpatterHealth.States.Attract:
			if (Vector2.Distance(this.hc.transform.position, base.transform.position) <= 0.6f)
			{
				this.Collect();
				return;
			}
			this.FireAtTarget();
			return;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060030E9 RID: 12521 RVA: 0x000D88F8 File Offset: 0x000D6AF8
	private void FaceAngle()
	{
		Vector2 linearVelocity = this.body.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x060030EA RID: 12522 RVA: 0x000D8944 File Offset: 0x000D6B44
	private void ProjectileSquash()
	{
		float num = 1f - this.body.linearVelocity.magnitude * 1.4f * 0.01f;
		float num2 = 1f + this.body.linearVelocity.magnitude * 1.4f * 0.01f;
		if (num2 < 0.6f)
		{
			num2 = 0.6f;
		}
		if (num > 1.75f)
		{
			num = 1.75f;
		}
		num *= this.scaleModifier;
		num2 *= this.scaleModifier;
		base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
	}

	// Token: 0x060030EB RID: 12523 RVA: 0x000D89F0 File Offset: 0x000D6BF0
	private void FireAtTarget()
	{
		Vector3 position = this.hc.transform.position;
		float x = position.x - base.transform.position.x;
		float num = Mathf.Atan2(position.y + -0.5f - base.transform.position.y, x) * 57.295776f;
		this.fireSpeed += 0.85f;
		if (this.fireSpeed > 30f)
		{
			this.fireSpeed = 30f;
		}
		float x2 = this.fireSpeed * Mathf.Cos(num * 0.017453292f);
		float y = this.fireSpeed * Mathf.Sin(num * 0.017453292f);
		this.body.linearVelocity = new Vector2(x2, y);
	}

	// Token: 0x060030EC RID: 12524 RVA: 0x000D8AB4 File Offset: 0x000D6CB4
	private void Collect()
	{
		this.getAudio.SpawnAndPlayOneShot(base.transform.position, null);
		this.hc.SpriteFlash.flashHealBlue();
		if (this.healthGetEffect)
		{
			this.healthGetEffect.SetActive(true);
			Transform transform = this.healthGetEffect.transform;
			transform.SetRotationZ(Random.Range(0f, 360f));
			Vector3 localScale = transform.localScale;
			localScale.x = (localScale.y = Random.Range(0.5f, 0.7f));
			transform.localScale = localScale;
			transform.SetParent(null, true);
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x04003421 RID: 13345
	[SerializeField]
	private GameObject healthGetEffect;

	// Token: 0x04003422 RID: 13346
	[SerializeField]
	private AudioEventRandom getAudio;

	// Token: 0x04003423 RID: 13347
	private SpatterHealth.States currentState;

	// Token: 0x04003424 RID: 13348
	private float scaleModifier;

	// Token: 0x04003425 RID: 13349
	private float fallTimeLeft;

	// Token: 0x04003426 RID: 13350
	private float fireSpeed;

	// Token: 0x04003427 RID: 13351
	private Rigidbody2D body;

	// Token: 0x04003428 RID: 13352
	private HeroController hc;

	// Token: 0x0200185E RID: 6238
	private enum States
	{
		// Token: 0x040091BF RID: 37311
		Init,
		// Token: 0x040091C0 RID: 37312
		Fall,
		// Token: 0x040091C1 RID: 37313
		Decel,
		// Token: 0x040091C2 RID: 37314
		Attract
	}
}
