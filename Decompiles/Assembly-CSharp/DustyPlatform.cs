using System;
using UnityEngine;

// Token: 0x020004D7 RID: 1239
[RequireComponent(typeof(BoxCollider2D))]
public class DustyPlatform : DebugDrawColliderRuntimeAdder
{
	// Token: 0x06002C8A RID: 11402 RVA: 0x000C2DE8 File Offset: 0x000C0FE8
	protected void Reset()
	{
		this.inset = 0.3f;
		this.dustIgnoredLayers.value = 327680;
		this.dustRateAreaFactor = 10f;
		this.dustRateConstant = 5f;
		this.streamOffset = new Vector3(0f, 0.1f, 0.01f);
		this.rocksChance = 0.5f;
		this.cooldownDuration = 0.45f;
	}

	// Token: 0x06002C8B RID: 11403 RVA: 0x000C2E56 File Offset: 0x000C1056
	protected override void Awake()
	{
		base.Awake();
		this.bodyCollider = base.GetComponent<BoxCollider2D>();
	}

	// Token: 0x06002C8C RID: 11404 RVA: 0x000C2E6A File Offset: 0x000C106A
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TerrainCollider, false);
	}

	// Token: 0x06002C8D RID: 11405 RVA: 0x000C2E7C File Offset: 0x000C107C
	protected void Update()
	{
		if (!this.isRunning)
		{
			return;
		}
		bool flag = true;
		if (this.cooldownTimer > 0f)
		{
			this.cooldownTimer -= Time.deltaTime;
			if (this.cooldownTimer > 0f)
			{
				flag = false;
			}
		}
		if (flag)
		{
			this.isRunning = false;
		}
	}

	// Token: 0x06002C8E RID: 11406 RVA: 0x000C2ECC File Offset: 0x000C10CC
	protected void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.isRunning)
		{
			return;
		}
		int layer = collision.collider.gameObject.layer;
		if ((this.dustIgnoredLayers.value & 1 << layer) != 0)
		{
			return;
		}
		Vector2 vector = Vector2.zero;
		if (collision.contacts.Length != 0)
		{
			vector = collision.contacts[0].normal;
		}
		if (Mathf.Abs(vector.y - -1f) > 0.1f)
		{
			return;
		}
		Vector3 position = base.transform.position;
		this.dustFallClips.SpawnAndPlayOneShot(this.dustFallSourcePrefab, position, false, 1f, null);
		Vector2 vector2 = this.bodyCollider.size - new Vector2(this.inset, this.inset);
		Vector3 vector3 = position;
		vector3.z = -0.1f;
		if (this.dustPrefab != null)
		{
			ParticleSystem particleSystem = this.dustPrefab.Spawn(vector3);
			this.SetRateOverTime(particleSystem, vector2.x * vector2.y * this.dustRateAreaFactor + this.dustRateConstant);
			Transform transform = particleSystem.transform;
			transform.localScale = new Vector3(vector2.x, vector2.y, transform.localScale.z);
		}
		if (this.streamPrefab != null)
		{
			GameObject gameObject = this.streamPrefab.Spawn(vector3 + new Vector3(0f, -this.bodyCollider.size.y * 0.5f, 0.01f) + this.streamOffset);
			Vector3 localScale = gameObject.transform.localScale;
			localScale.x = vector2.x;
			gameObject.transform.localScale = localScale;
		}
		if (Random.value < this.rocksChance && this.rocksPrefab != null)
		{
			Transform transform2 = this.rocksPrefab.Spawn(vector3).transform;
			Vector3 position2 = transform2.position;
			transform2.position = new Vector3(position2.x, position2.y, 0.003f);
			transform2.localScale = new Vector3(vector2.x, vector2.y, transform2.localScale.z);
		}
		this.cooldownTimer = this.cooldownDuration;
		this.isRunning = true;
	}

	// Token: 0x06002C8F RID: 11407 RVA: 0x000C310C File Offset: 0x000C130C
	private void SetRateOverTime(ParticleSystem ps, float rateOverTime)
	{
		ps.emission.rateOverTime = rateOverTime;
	}

	// Token: 0x04002E28 RID: 11816
	private BoxCollider2D bodyCollider;

	// Token: 0x04002E29 RID: 11817
	[SerializeField]
	private float inset;

	// Token: 0x04002E2A RID: 11818
	[SerializeField]
	private LayerMask dustIgnoredLayers;

	// Token: 0x04002E2B RID: 11819
	[SerializeField]
	private RandomAudioClipTable dustFallClips;

	// Token: 0x04002E2C RID: 11820
	[SerializeField]
	private AudioSource dustFallSourcePrefab;

	// Token: 0x04002E2D RID: 11821
	[SerializeField]
	private ParticleSystem dustPrefab;

	// Token: 0x04002E2E RID: 11822
	[SerializeField]
	private ParticleSystem rocksPrefab;

	// Token: 0x04002E2F RID: 11823
	[SerializeField]
	private float dustRateAreaFactor;

	// Token: 0x04002E30 RID: 11824
	[SerializeField]
	private float dustRateConstant;

	// Token: 0x04002E31 RID: 11825
	[SerializeField]
	private GameObject streamPrefab;

	// Token: 0x04002E32 RID: 11826
	[SerializeField]
	private Vector3 streamOffset;

	// Token: 0x04002E33 RID: 11827
	[SerializeField]
	private float rocksChance;

	// Token: 0x04002E34 RID: 11828
	[SerializeField]
	private float cooldownDuration;

	// Token: 0x04002E35 RID: 11829
	private float rocksDelayTimer;

	// Token: 0x04002E36 RID: 11830
	private float cooldownTimer;

	// Token: 0x04002E37 RID: 11831
	private bool isRunning;
}
