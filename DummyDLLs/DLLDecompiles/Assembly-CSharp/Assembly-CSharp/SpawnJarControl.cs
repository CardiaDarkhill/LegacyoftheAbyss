using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003EE RID: 1006
public class SpawnJarControl : MonoBehaviour
{
	// Token: 0x06002265 RID: 8805 RVA: 0x0009E739 File Offset: 0x0009C939
	private void Awake()
	{
		this.col = base.GetComponent<CircleCollider2D>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.sprite = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06002266 RID: 8806 RVA: 0x0009E75F File Offset: 0x0009C95F
	private void OnEnable()
	{
		this.col.enabled = false;
		this.sprite.enabled = false;
		base.StartCoroutine(this.Behaviour());
	}

	// Token: 0x06002267 RID: 8807 RVA: 0x0009E786 File Offset: 0x0009C986
	private IEnumerator Behaviour()
	{
		base.transform.SetPositionY(this.spawnY);
		base.transform.SetPositionZ(0.01f);
		this.readyDust.Play();
		yield return new WaitForSeconds(0.5f);
		this.col.enabled = true;
		this.body.linearVelocity = new Vector2(0f, -25f);
		this.body.angularVelocity = (float)((Random.Range(0, 2) > 0) ? -300 : 300);
		this.readyDust.Stop();
		this.dustTrail.Play();
		this.sprite.enabled = true;
		while (base.transform.position.y > this.breakY)
		{
			yield return null;
		}
		base.transform.SetPositionY(this.breakY);
		GameCameras.instance.cameraShakeFSM.SendEvent("EnemyKillShake");
		this.dustTrail.Stop();
		this.ptBreakS.Play();
		this.ptBreakL.Play();
		this.strikeNailR.Spawn(base.transform.position);
		this.col.enabled = false;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		this.sprite.enabled = false;
		this.breakSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		if (this.enemyToSpawn)
		{
			GameObject gameObject = this.enemyToSpawn.Spawn(base.transform.position);
			HealthManager component = gameObject.GetComponent<HealthManager>();
			if (component)
			{
				component.hp = this.enemyHealth;
			}
			gameObject.tag = "Boss";
		}
		yield return new WaitForSeconds(2f);
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x06002268 RID: 8808 RVA: 0x0009E795 File Offset: 0x0009C995
	public void SetEnemySpawn(GameObject prefab, int health)
	{
		this.enemyToSpawn = prefab;
		this.enemyHealth = health;
	}

	// Token: 0x0400212B RID: 8491
	public float spawnY = 106.52f;

	// Token: 0x0400212C RID: 8492
	public float breakY = 94.55f;

	// Token: 0x0400212D RID: 8493
	public ParticleSystem readyDust;

	// Token: 0x0400212E RID: 8494
	public ParticleSystem dustTrail;

	// Token: 0x0400212F RID: 8495
	public ParticleSystem ptBreakS;

	// Token: 0x04002130 RID: 8496
	public ParticleSystem ptBreakL;

	// Token: 0x04002131 RID: 8497
	public GameObject strikeNailR;

	// Token: 0x04002132 RID: 8498
	public AudioEventRandom breakSound;

	// Token: 0x04002133 RID: 8499
	public AudioSource audioSourcePrefab;

	// Token: 0x04002134 RID: 8500
	private GameObject enemyToSpawn;

	// Token: 0x04002135 RID: 8501
	private int enemyHealth;

	// Token: 0x04002136 RID: 8502
	private CircleCollider2D col;

	// Token: 0x04002137 RID: 8503
	private Rigidbody2D body;

	// Token: 0x04002138 RID: 8504
	private SpriteRenderer sprite;
}
