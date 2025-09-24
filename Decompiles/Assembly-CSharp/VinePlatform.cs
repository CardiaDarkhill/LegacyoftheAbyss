using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000582 RID: 1410
public class VinePlatform : SuspendedPlatformBase
{
	// Token: 0x06003279 RID: 12921 RVA: 0x000E0AEC File Offset: 0x000DECEC
	protected override void Awake()
	{
		base.Awake();
		this.audioSource = base.GetComponent<AudioSource>();
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x0600327A RID: 12922 RVA: 0x000E0B0C File Offset: 0x000DED0C
	private void Start()
	{
		if (this.landingDetector && !this.acidLander)
		{
			this.landingDetector.OnTriggerEntered += delegate(Collider2D collider, GameObject sender)
			{
				this.Land();
			};
		}
		if (this.enemyDetector)
		{
			this.enemyDetector.OnTriggerEntered += delegate(Collider2D collider, GameObject sender)
			{
				HealthManager component = collider.GetComponent<HealthManager>();
				if (component)
				{
					component.Die(new float?(0f), AttackTypes.Splatter, false);
				}
			};
			this.enemyDetector.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600327B RID: 12923 RVA: 0x000E0B90 File Offset: 0x000DED90
	private void Update()
	{
		if (this.acidLander && !this.activated && this.collider.bounds.min.y <= this.acidTargetY)
		{
			this.Land();
		}
	}

	// Token: 0x0600327C RID: 12924 RVA: 0x000E0BD4 File Offset: 0x000DEDD4
	private void Land()
	{
		this.PlaySound(this.landSound);
		if (!this.acidLander)
		{
			GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
			if (gameCameras)
			{
				gameCameras.cameraShakeFSM.SendEvent("AverageShake");
			}
			foreach (ParticleSystem particleSystem in this.landParticles)
			{
				if (particleSystem.gameObject.activeInHierarchy)
				{
					particleSystem.Play();
				}
			}
			if (this.slamEffect)
			{
				this.slamEffect.SetActive(true);
			}
		}
		else
		{
			this.PlaySound(this.acidSplashSound);
			if (this.acidSplashPrefab)
			{
				Object.Instantiate<GameObject>(this.acidSplashPrefab, new Vector3(base.transform.position.x, this.collider.bounds.min.y, base.transform.position.z), Quaternion.identity);
			}
			float num = base.transform.position.y - this.collider.bounds.min.y;
			base.transform.SetPositionY(this.acidTargetY + num);
		}
		if (this.body)
		{
			this.body.isKinematic = true;
			this.body.linearVelocity = Vector2.zero;
		}
		if (this.enemyDetector)
		{
			this.enemyDetector.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600327D RID: 12925 RVA: 0x000E0D50 File Offset: 0x000DEF50
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.respondOnLand && collision.gameObject.layer == 9 && collision.collider.bounds.min.y >= this.collider.bounds.max.y)
		{
			if (this.landRoutine != null)
			{
				base.StopCoroutine(this.landRoutine);
			}
			if (this.landReturnAction != null)
			{
				this.landReturnAction();
			}
			this.landRoutine = base.StartCoroutine(this.PlayerLand());
			return;
		}
		if (!this.body.isKinematic && collision.gameObject.layer != 8 && collision.gameObject.layer != 9)
		{
			Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
		}
	}

	// Token: 0x0600327E RID: 12926 RVA: 0x000E0E1B File Offset: 0x000DF01B
	private void PlaySound(AudioClip clip)
	{
		if (this.audioSource && clip)
		{
			this.audioSource.PlayOneShot(clip);
		}
	}

	// Token: 0x0600327F RID: 12927 RVA: 0x000E0E3E File Offset: 0x000DF03E
	private IEnumerator PlayerLand()
	{
		this.PlaySound(this.playerLandSound);
		if (this.playerLandParticles)
		{
			this.playerLandParticles.Play();
		}
		if (this.platformSprite)
		{
			Vector3 initialPos = this.platformSprite.transform.position;
			this.landReturnAction = delegate()
			{
				this.platformSprite.transform.position = initialPos;
			};
			for (float elapsed = 0f; elapsed < this.playerLandAnimLength; elapsed += Time.deltaTime)
			{
				Vector3 initialPos2 = initialPos;
				initialPos2.y += this.playerLandAnimCurve.Evaluate(elapsed / this.playerLandAnimLength);
				this.platformSprite.transform.position = initialPos2;
				yield return null;
			}
		}
		if (this.landReturnAction != null)
		{
			this.landReturnAction();
		}
		this.landRoutine = null;
		yield break;
	}

	// Token: 0x06003280 RID: 12928 RVA: 0x000E0E50 File Offset: 0x000DF050
	private void OnDrawGizmosSelected()
	{
		if (this.acidLander)
		{
			Vector3 position = base.transform.position;
			position.y = this.acidTargetY;
			Gizmos.DrawWireSphere(position, 0.5f);
		}
	}

	// Token: 0x06003281 RID: 12929 RVA: 0x000E0E8C File Offset: 0x000DF08C
	protected override void OnStartActivated()
	{
		base.OnStartActivated();
		this.platformSprite.SetActive(false);
		this.activatedSprite.SetActive(true);
		this.collider.enabled = false;
		if (this.landingDetector)
		{
			this.landingDetector.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003282 RID: 12930 RVA: 0x000E0EE4 File Offset: 0x000DF0E4
	public override void CutDown()
	{
		base.CutDown();
		if (this.enemyDetector)
		{
			this.enemyDetector.gameObject.SetActive(true);
		}
		this.respondOnLand = false;
		if (this.landRoutine != null)
		{
			base.StopCoroutine(this.landRoutine);
		}
		if (this.body)
		{
			this.body.isKinematic = false;
		}
	}

	// Token: 0x0400363C RID: 13884
	[SerializeField]
	protected Collider2D collider;

	// Token: 0x0400363D RID: 13885
	[SerializeField]
	private GameObject platformSprite;

	// Token: 0x0400363E RID: 13886
	[SerializeField]
	private GameObject activatedSprite;

	// Token: 0x0400363F RID: 13887
	[Space]
	[SerializeField]
	private AudioClip playerLandSound;

	// Token: 0x04003640 RID: 13888
	[SerializeField]
	private ParticleSystem playerLandParticles;

	// Token: 0x04003641 RID: 13889
	[SerializeField]
	private AnimationCurve playerLandAnimCurve = new AnimationCurve(new Keyframe[]
	{
		new Keyframe(0f, 0f),
		new Keyframe(0.5f, 1f),
		new Keyframe(1f, 0f)
	});

	// Token: 0x04003642 RID: 13890
	[SerializeField]
	private float playerLandAnimLength = 0.5f;

	// Token: 0x04003643 RID: 13891
	[Space]
	[SerializeField]
	private TriggerEnterEvent landingDetector;

	// Token: 0x04003644 RID: 13892
	[SerializeField]
	private AudioClip landSound;

	// Token: 0x04003645 RID: 13893
	[SerializeField]
	private ParticleSystem[] landParticles;

	// Token: 0x04003646 RID: 13894
	[SerializeField]
	private GameObject slamEffect;

	// Token: 0x04003647 RID: 13895
	[Space]
	[SerializeField]
	private TriggerEnterEvent enemyDetector;

	// Token: 0x04003648 RID: 13896
	[Space]
	[SerializeField]
	private bool acidLander;

	// Token: 0x04003649 RID: 13897
	[SerializeField]
	private float acidTargetY;

	// Token: 0x0400364A RID: 13898
	[SerializeField]
	private AudioClip acidSplashSound;

	// Token: 0x0400364B RID: 13899
	[SerializeField]
	private GameObject acidSplashPrefab;

	// Token: 0x0400364C RID: 13900
	private Coroutine landRoutine;

	// Token: 0x0400364D RID: 13901
	private bool respondOnLand = true;

	// Token: 0x0400364E RID: 13902
	private Action landReturnAction;

	// Token: 0x0400364F RID: 13903
	private AudioSource audioSource;

	// Token: 0x04003650 RID: 13904
	private Rigidbody2D body;
}
