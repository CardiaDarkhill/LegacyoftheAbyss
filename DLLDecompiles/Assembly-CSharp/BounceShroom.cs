using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class BounceShroom : MonoBehaviour
{
	// Token: 0x06000A06 RID: 2566 RVA: 0x0002D6C6 File Offset: 0x0002B8C6
	private void Awake()
	{
		this.anim = base.GetComponentInChildren<tk2dSpriteAnimator>();
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x0002D6D4 File Offset: 0x0002B8D4
	private void Start()
	{
		if (!this.active)
		{
			return;
		}
		GameObject gameObject = Object.Instantiate<GameObject>(this.idleParticlePrefab);
		if (gameObject)
		{
			gameObject.transform.SetPositionX(base.transform.position.x);
			gameObject.transform.SetPositionY(base.transform.position.y);
		}
		this.idleRoutine = base.StartCoroutine(this.Idle());
		CollisionEnterEvent[] componentsInChildren = base.GetComponentsInChildren<CollisionEnterEvent>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].CollisionEnteredDirectional += delegate(CollisionEnterEvent.Direction direction, Collision2D collision)
			{
				switch (direction)
				{
				case CollisionEnterEvent.Direction.Left:
					HeroController.instance.SendMessage("RecoilLeft");
					break;
				case CollisionEnterEvent.Direction.Right:
					HeroController.instance.SendMessage("RecoilRight");
					break;
				case CollisionEnterEvent.Direction.Top:
					HeroController.instance.SendMessage("Bounce");
					HeroController.instance.SendMessage((Random.Range(0, 2) == 0) ? "RecoilLeft" : "RecoilRight");
					break;
				}
				this.BounceSmall();
			};
		}
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x0002D76F File Offset: 0x0002B96F
	private IEnumerator Idle()
	{
		for (;;)
		{
			this.PlayAnims(BounceShroom.AnimType.Idle);
			yield return new WaitForSeconds(Random.Range(2f, 8f));
			yield return new WaitForSeconds(this.PlayAnims(BounceShroom.AnimType.Bob));
		}
		yield break;
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x0002D780 File Offset: 0x0002B980
	private float PlayAnims(BounceShroom.AnimType animType)
	{
		tk2dSpriteAnimationClip clipByName = this.anim.GetClipByName(this.GetAnimName(animType));
		this.anim.Play(clipByName);
		return clipByName.Duration;
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x0002D7B4 File Offset: 0x0002B9B4
	private string GetAnimName(BounceShroom.AnimType animType)
	{
		string result = "NONE";
		switch (animType)
		{
		case BounceShroom.AnimType.Idle:
			result = this.idleAnim;
			break;
		case BounceShroom.AnimType.Bob:
			result = this.bobAnim;
			break;
		case BounceShroom.AnimType.Bounce:
			result = this.bounceAnim;
			break;
		}
		return result;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x0002D7F8 File Offset: 0x0002B9F8
	protected void BounceSmall()
	{
		if (!this.active)
		{
			return;
		}
		if (this.bounceSmallPrefab)
		{
			this.bounceSmallPrefab.Spawn(base.transform.position).transform.SetPositionZ(-0.001f);
		}
		if (this.bounceRoutine == null)
		{
			this.bounceRoutine = base.StartCoroutine(this.Bounce());
		}
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x0002D85A File Offset: 0x0002BA5A
	private IEnumerator Bounce()
	{
		if (this.idleRoutine != null)
		{
			base.StopCoroutine(this.idleRoutine);
		}
		this.hitSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, false, 1f, null);
		yield return new WaitForSeconds(this.PlayAnims(BounceShroom.AnimType.Bounce));
		this.bounceRoutine = null;
		this.idleRoutine = base.StartCoroutine(this.Idle());
		yield break;
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x0002D86C File Offset: 0x0002BA6C
	public void BounceLarge(bool useEffects = true)
	{
		if (!this.active)
		{
			return;
		}
		if (useEffects)
		{
			if (Time.timeAsDouble >= BounceShroom.nextBounceParticleTime)
			{
				BounceShroom.nextBounceParticleTime = Time.timeAsDouble + 0.25;
			}
			else
			{
				useEffects = false;
			}
		}
		if (this.bounceLargePrefab && useEffects)
		{
			this.bounceLargePrefab.Spawn(base.transform.position).transform.SetPositionZ(-0.001f);
		}
		if (this.bounceRoutine == null)
		{
			this.bounceRoutine = base.StartCoroutine(this.Bounce());
		}
		if (Time.timeAsDouble > BounceShroom.nextCamShakeTime)
		{
			GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			BounceShroom.nextCamShakeTime = Time.timeAsDouble + 0.25;
		}
		if (useEffects)
		{
			if (this.hitEffect)
			{
				this.hitEffect.SetActive(true);
			}
			if (this.heroParticlePrefab)
			{
				ParticleSystem particleSystem = BounceShroom.heroParticles ? BounceShroom.heroParticles.GetComponent<ParticleSystem>() : null;
				if (!BounceShroom.heroParticles || !BounceShroom.heroParticles.activeSelf || (particleSystem && !particleSystem.isEmitting))
				{
					BounceShroom.heroParticles = this.heroParticlePrefab.Spawn(HeroController.instance.transform, new Vector3(0f, -1.5f, -0.002f));
				}
			}
		}
	}

	// Token: 0x0400098F RID: 2447
	[Tooltip("Active false by default since this script may be used elsewhere as just a flag")]
	public bool active;

	// Token: 0x04000990 RID: 2448
	[Space]
	public GameObject idleParticlePrefab;

	// Token: 0x04000991 RID: 2449
	public GameObject bounceSmallPrefab;

	// Token: 0x04000992 RID: 2450
	public GameObject bounceLargePrefab;

	// Token: 0x04000993 RID: 2451
	public GameObject heroParticlePrefab;

	// Token: 0x04000994 RID: 2452
	private const float bounceParticleDelay = 0.25f;

	// Token: 0x04000995 RID: 2453
	private static double nextBounceParticleTime;

	// Token: 0x04000996 RID: 2454
	[Header("Animations")]
	public string idleAnim = "Idle 1";

	// Token: 0x04000997 RID: 2455
	public string bobAnim = "Bob 1";

	// Token: 0x04000998 RID: 2456
	public string bounceAnim = "Bounce 1";

	// Token: 0x04000999 RID: 2457
	[Space]
	public GameObject hitEffect;

	// Token: 0x0400099A RID: 2458
	[Space]
	public AudioSource audioSourcePrefab;

	// Token: 0x0400099B RID: 2459
	public RandomAudioClipTable hitSound;

	// Token: 0x0400099C RID: 2460
	private tk2dSpriteAnimator anim;

	// Token: 0x0400099D RID: 2461
	private Coroutine idleRoutine;

	// Token: 0x0400099E RID: 2462
	private Coroutine bounceRoutine;

	// Token: 0x0400099F RID: 2463
	private static GameObject heroParticles;

	// Token: 0x040009A0 RID: 2464
	private static double nextCamShakeTime;

	// Token: 0x0200147B RID: 5243
	private enum AnimType
	{
		// Token: 0x04008364 RID: 33636
		Idle,
		// Token: 0x04008365 RID: 33637
		Bob,
		// Token: 0x04008366 RID: 33638
		Bounce
	}
}
