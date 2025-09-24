using System;
using UnityEngine;

// Token: 0x020004A5 RID: 1189
public class BreakableObject : MonoBehaviour
{
	// Token: 0x06002B2F RID: 11055 RVA: 0x000BD0B0 File Offset: 0x000BB2B0
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06002B30 RID: 11056 RVA: 0x000BD0C0 File Offset: 0x000BB2C0
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

	// Token: 0x06002B31 RID: 11057 RVA: 0x000BD128 File Offset: 0x000BB328
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.activated)
		{
			return;
		}
		bool flag = false;
		int num = (int)Mathf.Sign(base.transform.position.x - collision.transform.position.x);
		float num2 = 1f;
		BreakableObject.Direction direction = null;
		if (collision.tag == "Nail Attack")
		{
			flag = true;
			num2 = this.attackMagnitude;
			if (this.objectNailEffectPrefab)
			{
				GameObject gameObject = this.objectNailEffectPrefab.Spawn(base.transform.position);
				Vector3 localScale = gameObject.transform.localScale;
				localScale.x = Mathf.Abs(localScale.x) * (float)num;
				gameObject.transform.localScale = localScale;
			}
			if (this.midpointNailEffectPrefab)
			{
				GameObject gameObject2 = this.midpointNailEffectPrefab.Spawn((collision.transform.position + base.transform.position) / 2f);
				Vector3 localScale2 = gameObject2.transform.localScale;
				localScale2.x = Mathf.Abs(localScale2.x) * (float)num;
				gameObject2.transform.localScale = localScale2;
			}
			float value = PlayMakerFSM.FindFsmOnGameObject(collision.gameObject, "damages_enemy").FsmVariables.FindFsmFloat("direction").Value;
			if (value < 45f)
			{
				direction = this.right;
			}
			else if (value < 135f)
			{
				direction = this.up;
			}
			else if (value < 225f)
			{
				direction = this.left;
			}
			else if (value < 360f)
			{
				direction = this.down;
			}
			if (direction != null && direction.effectPrefab)
			{
				GameObject gameObject3 = direction.effectPrefab.Spawn(base.transform.position);
				if (gameObject3)
				{
					gameObject3.transform.localScale = direction.scale;
					gameObject3.transform.localEulerAngles = direction.rotation;
				}
			}
		}
		else if (collision.tag == "Hero Spell")
		{
			flag = true;
			if (this.spellHitEffectPrefab)
			{
				this.spellHitEffectPrefab.Spawn(base.transform.position);
			}
		}
		if (flag)
		{
			if (this.containingParticles.Length != 0)
			{
				GameObject gameObject4 = Probability.GetRandomGameObjectByProbability(this.containingParticles);
				if (gameObject4)
				{
					if (gameObject4.transform.parent != base.transform)
					{
						BreakableObject.FlingObject flingObject = null;
						foreach (BreakableObject.FlingObject flingObject2 in this.flingObjectRegister)
						{
							if (flingObject2.referenceObject == gameObject4)
							{
								flingObject = flingObject2;
								break;
							}
						}
						if (flingObject != null)
						{
							flingObject.Fling(base.transform.position);
						}
						else
						{
							gameObject4 = gameObject4.Spawn(base.transform.position);
						}
					}
					gameObject4.SetActive(true);
				}
			}
			foreach (GameObject gameObject5 in this.flingDebris)
			{
				if (gameObject5)
				{
					gameObject5.SetActive(true);
					float num3 = Random.Range(direction.minFlingSpeed, direction.maxFlingSpeed) * num2;
					float num4 = Random.Range(direction.minFlingAngle, direction.maxFlingAngle);
					float x = num3 * Mathf.Cos(num4 * 0.017453292f);
					float y = num3 * Mathf.Sin(num4 * 0.017453292f);
					Vector2 force = new Vector2(x, y);
					Rigidbody2D component = gameObject5.GetComponent<Rigidbody2D>();
					if (component)
					{
						component.AddForce(force, ForceMode2D.Impulse);
					}
				}
			}
			if (this.source && this.cutSound.Length != 0)
			{
				this.source.clip = this.cutSound[Random.Range(0, this.cutSound.Length)];
				this.source.pitch = Random.Range(this.pitchMin, this.pitchMax);
				this.source.Play();
			}
			GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
			if (gameCameras)
			{
				gameCameras.cameraShakeFSM.SendEvent("EnemyKillShake");
			}
			SpriteRenderer component2 = base.GetComponent<SpriteRenderer>();
			if (component2)
			{
				component2.enabled = false;
			}
			this.activated = true;
		}
	}

	// Token: 0x04002C61 RID: 11361
	public GameObject[] flingDebris;

	// Token: 0x04002C62 RID: 11362
	public float attackMagnitude = 6f;

	// Token: 0x04002C63 RID: 11363
	[Space]
	public BreakableObject.Direction right;

	// Token: 0x04002C64 RID: 11364
	public BreakableObject.Direction left;

	// Token: 0x04002C65 RID: 11365
	public BreakableObject.Direction up;

	// Token: 0x04002C66 RID: 11366
	public BreakableObject.Direction down;

	// Token: 0x04002C67 RID: 11367
	[Space]
	public Probability.ProbabilityGameObject[] containingParticles;

	// Token: 0x04002C68 RID: 11368
	public BreakableObject.FlingObject[] flingObjectRegister;

	// Token: 0x04002C69 RID: 11369
	[Space]
	public GameObject objectNailEffectPrefab;

	// Token: 0x04002C6A RID: 11370
	public GameObject midpointNailEffectPrefab;

	// Token: 0x04002C6B RID: 11371
	public GameObject spellHitEffectPrefab;

	// Token: 0x04002C6C RID: 11372
	[Space]
	public AudioClip[] cutSound;

	// Token: 0x04002C6D RID: 11373
	public float pitchMin = 0.9f;

	// Token: 0x04002C6E RID: 11374
	public float pitchMax = 1.1f;

	// Token: 0x04002C6F RID: 11375
	private AudioSource source;

	// Token: 0x04002C70 RID: 11376
	private bool activated;

	// Token: 0x020017BE RID: 6078
	[Serializable]
	public class Direction
	{
		// Token: 0x04008F3A RID: 36666
		public GameObject effectPrefab;

		// Token: 0x04008F3B RID: 36667
		public Vector3 scale = Vector3.one;

		// Token: 0x04008F3C RID: 36668
		public Vector3 rotation;

		// Token: 0x04008F3D RID: 36669
		[Space]
		public float minFlingSpeed = 4f;

		// Token: 0x04008F3E RID: 36670
		public float maxFlingSpeed = 4f;

		// Token: 0x04008F3F RID: 36671
		public float minFlingAngle = 5f;

		// Token: 0x04008F40 RID: 36672
		public float maxFlingAngle = 5f;
	}

	// Token: 0x020017BF RID: 6079
	[Serializable]
	public class FlingObject
	{
		// Token: 0x06008E94 RID: 36500 RVA: 0x0028DCF4 File Offset: 0x0028BEF4
		public void Fling(Vector3 origin)
		{
			if (!this.referenceObject)
			{
				return;
			}
			int num = Random.Range(this.spawnMin, this.spawnMax + 1);
			for (int i = 0; i < num; i++)
			{
				GameObject gameObject = this.referenceObject.Spawn();
				if (gameObject)
				{
					gameObject.transform.position = origin + new Vector3(Random.Range(-this.originVariation.x, this.originVariation.x), Random.Range(-this.originVariation.y, this.originVariation.y), 0f);
					float num2 = Random.Range(this.speedMin, this.speedMax);
					float num3 = Random.Range(this.angleMin, this.angleMax);
					float x = num2 * Mathf.Cos(num3 * 0.017453292f);
					float y = num2 * Mathf.Sin(num3 * 0.017453292f);
					Vector2 force = new Vector2(x, y);
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					if (component)
					{
						component.AddForce(force, ForceMode2D.Impulse);
					}
				}
			}
		}

		// Token: 0x04008F41 RID: 36673
		public GameObject referenceObject;

		// Token: 0x04008F42 RID: 36674
		[Space]
		public int spawnMin = 25;

		// Token: 0x04008F43 RID: 36675
		public int spawnMax = 30;

		// Token: 0x04008F44 RID: 36676
		public float speedMin = 9f;

		// Token: 0x04008F45 RID: 36677
		public float speedMax = 20f;

		// Token: 0x04008F46 RID: 36678
		public float angleMin = 20f;

		// Token: 0x04008F47 RID: 36679
		public float angleMax = 160f;

		// Token: 0x04008F48 RID: 36680
		public Vector2 originVariation = new Vector2(0.5f, 0.5f);
	}
}
