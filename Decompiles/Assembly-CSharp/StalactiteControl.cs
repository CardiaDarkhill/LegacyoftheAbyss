using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000560 RID: 1376
public class StalactiteControl : MonoBehaviour
{
	// Token: 0x06003122 RID: 12578 RVA: 0x000D9FD4 File Offset: 0x000D81D4
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.source = base.GetComponent<AudioSource>();
		this.heroDamage = base.GetComponent<DamageHero>();
		this.damageEnemies = base.GetComponent<DamageEnemies>();
	}

	// Token: 0x06003123 RID: 12579 RVA: 0x000DA008 File Offset: 0x000D8208
	private void Start()
	{
		this.trigger = base.GetComponentInChildren<TriggerEnterEvent>();
		if (this.trigger)
		{
			this.trigger.OnTriggerEntered += this.HandleTriggerEnter;
		}
		if (this.heroDamage)
		{
			this.heroDamage.damageDealt = 0;
		}
		this.body.isKinematic = true;
		if (this.damageEnemies)
		{
			this.damageEnemies.enabled = false;
		}
	}

	// Token: 0x06003124 RID: 12580 RVA: 0x000DA084 File Offset: 0x000D8284
	private void HandleTriggerEnter(Collider2D collider, GameObject sender)
	{
		if (collider.tag == "Player" && Physics2D.Linecast(base.transform.position, collider.transform.position, 256).collider == null)
		{
			base.StartCoroutine(this.Fall(this.fallDelay));
			this.trigger.OnTriggerEntered -= this.HandleTriggerEnter;
			sender.SetActive(false);
		}
	}

	// Token: 0x06003125 RID: 12581 RVA: 0x000DA10E File Offset: 0x000D830E
	private IEnumerator Fall(float delay)
	{
		if (this.top)
		{
			this.top.transform.SetParent(base.transform.parent);
		}
		base.transform.position += Vector3.down * this.startFallOffset;
		if (this.startFallEffect)
		{
			this.startFallEffect.SetActive(true);
			this.startFallEffect.transform.SetParent(base.transform.parent);
		}
		if (this.source && this.startFallSound)
		{
			this.source.PlayOneShot(this.startFallSound);
		}
		yield return new WaitForSeconds(delay);
		if (this.fallEffect)
		{
			this.fallEffect.SetActive(true);
			this.fallEffect.transform.SetParent(base.transform.parent);
		}
		if (this.trailEffect)
		{
			this.trailEffect.SetActive(true);
		}
		if (this.heroDamage)
		{
			this.heroDamage.damageDealt = 1;
		}
		if (this.damageEnemies)
		{
			this.damageEnemies.enabled = true;
		}
		this.body.isKinematic = false;
		this.fallen = true;
		yield break;
	}

	// Token: 0x06003126 RID: 12582 RVA: 0x000DA124 File Offset: 0x000D8324
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.fallen && collision.gameObject.layer == 8)
		{
			this.body.isKinematic = true;
			if (this.trailEffect)
			{
				this.trailEffect.transform.parent = null;
			}
			this.trailEffect.GetComponent<ParticleSystem>().Stop();
			if (this.embeddedVersion)
			{
				this.embeddedVersion.SetActive(true);
				this.embeddedVersion.transform.SetParent(base.transform.parent, true);
			}
			RaycastHit2D raycastHit2D = Helper.Raycast2D(base.transform.position, Vector2.down, 10f, 256);
			foreach (GameObject gameObject in this.landEffectPrefabs)
			{
				Vector3 vector = new Vector3(raycastHit2D.point.x, raycastHit2D.point.y, gameObject.transform.position.z);
				gameObject.Spawn((raycastHit2D.collider != null) ? vector : base.transform.position);
			}
			base.gameObject.SetActive(false);
			return;
		}
		if (collision.tag == "Nail Attack")
		{
			if (!this.fallen)
			{
				base.StartCoroutine(this.Fall(0f));
			}
			if (this.heroDamage)
			{
				this.heroDamage.damageDealt = 0;
				this.heroDamage = null;
			}
			float value = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
			float num = 0f;
			if (value < 45f)
			{
				num = 45f;
			}
			else
			{
				if (value < 135f)
				{
					GameObject[] array = this.hitUpEffectPrefabs;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Spawn(base.transform.position);
					}
					this.FlingObjects();
					if (this.source && this.breakSound)
					{
						AudioSource audioSource = new GameObject("StalactiteBreakEffect").AddComponent<AudioSource>();
						audioSource.outputAudioMixerGroup = this.source.outputAudioMixerGroup;
						audioSource.loop = false;
						audioSource.playOnAwake = false;
						audioSource.rolloffMode = this.source.rolloffMode;
						audioSource.minDistance = this.source.minDistance;
						audioSource.maxDistance = this.source.maxDistance;
						audioSource.clip = this.breakSound;
						audioSource.volume = this.source.volume;
						audioSource.Play();
					}
					base.gameObject.SetActive(false);
					return;
				}
				if (value < 225f)
				{
					num = -45f;
				}
				else if (value < 360f)
				{
					num = 0f;
				}
			}
			this.body.linearVelocity;
			Vector3 v = Quaternion.Euler(0f, 0f, num) * Vector3.down * this.hitVelocity;
			this.body.rotation = num;
			this.body.gravityScale = 0f;
			this.body.linearVelocity = v;
			this.nailStrikePrefab.Spawn(base.transform.position);
			if (this.source && this.hitSound)
			{
				this.source.PlayOneShot(this.hitSound);
			}
		}
	}

	// Token: 0x06003127 RID: 12583 RVA: 0x000DA4A4 File Offset: 0x000D86A4
	private void FlingObjects()
	{
		int num = Random.Range(this.spawnMin, this.spawnMax + 1);
		for (int i = 1; i <= num; i++)
		{
			GameObject gameObject = this.hitUpRockPrefabs.Spawn(base.transform.position, base.transform.rotation);
			Vector3 position = gameObject.transform.position;
			Vector3 position2 = gameObject.transform.position;
			Vector3 position3 = gameObject.transform.position;
			float num2 = (float)Random.Range(this.speedMin, this.speedMax);
			float num3 = Random.Range(0f, 360f);
			float x = num2 * Mathf.Cos(num3 * 0.017453292f);
			float y = num2 * Mathf.Sin(num3 * 0.017453292f);
			Vector2 linearVelocity;
			linearVelocity.x = x;
			linearVelocity.y = y;
			Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
			if (component)
			{
				component.linearVelocity = linearVelocity;
			}
		}
	}

	// Token: 0x04003478 RID: 13432
	public GameObject top;

	// Token: 0x04003479 RID: 13433
	[Space]
	public float startFallOffset = 0.1f;

	// Token: 0x0400347A RID: 13434
	public GameObject startFallEffect;

	// Token: 0x0400347B RID: 13435
	public AudioClip startFallSound;

	// Token: 0x0400347C RID: 13436
	public float fallDelay = 0.25f;

	// Token: 0x0400347D RID: 13437
	[Space]
	public GameObject fallEffect;

	// Token: 0x0400347E RID: 13438
	public GameObject trailEffect;

	// Token: 0x0400347F RID: 13439
	public GameObject nailStrikePrefab;

	// Token: 0x04003480 RID: 13440
	[Space]
	public GameObject embeddedVersion;

	// Token: 0x04003481 RID: 13441
	public GameObject[] landEffectPrefabs;

	// Token: 0x04003482 RID: 13442
	[Space]
	public float hitVelocity = 40f;

	// Token: 0x04003483 RID: 13443
	[Space]
	public GameObject[] hitUpEffectPrefabs;

	// Token: 0x04003484 RID: 13444
	public AudioClip hitSound;

	// Token: 0x04003485 RID: 13445
	public GameObject hitUpRockPrefabs;

	// Token: 0x04003486 RID: 13446
	public int spawnMin = 10;

	// Token: 0x04003487 RID: 13447
	public int spawnMax = 12;

	// Token: 0x04003488 RID: 13448
	public int speedMin = 15;

	// Token: 0x04003489 RID: 13449
	public int speedMax = 20;

	// Token: 0x0400348A RID: 13450
	public AudioClip breakSound;

	// Token: 0x0400348B RID: 13451
	private TriggerEnterEvent trigger;

	// Token: 0x0400348C RID: 13452
	private DamageHero heroDamage;

	// Token: 0x0400348D RID: 13453
	private Rigidbody2D body;

	// Token: 0x0400348E RID: 13454
	private AudioSource source;

	// Token: 0x0400348F RID: 13455
	private DamageEnemies damageEnemies;

	// Token: 0x04003490 RID: 13456
	private bool fallen;
}
