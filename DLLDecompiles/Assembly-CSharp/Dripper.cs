using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004D4 RID: 1236
public class Dripper : MonoBehaviour
{
	// Token: 0x1400008E RID: 142
	// (add) Token: 0x06002C71 RID: 11377 RVA: 0x000C2840 File Offset: 0x000C0A40
	// (remove) Token: 0x06002C72 RID: 11378 RVA: 0x000C2878 File Offset: 0x000C0A78
	public event Action<List<GameObject>> OnSpawned;

	// Token: 0x06002C73 RID: 11379 RVA: 0x000C28B0 File Offset: 0x000C0AB0
	public void StartDripper(Transform target)
	{
		if (!target || !this.spatterPrefab)
		{
			return;
		}
		this.flickingAway = false;
		base.transform.SetParent(target);
		base.transform.localPosition = new Vector3(0f, -0.5f, 0.001f);
		this.rb = target.GetComponent<Rigidbody2D>();
		AreaEffectTint component = base.GetComponent<AreaEffectTint>();
		if (component)
		{
			component.DoTint();
		}
		this.routine = base.StartCoroutine(this.Behaviour());
	}

	// Token: 0x06002C74 RID: 11380 RVA: 0x000C2938 File Offset: 0x000C0B38
	public void StartDripper(GameObject target)
	{
		this.StartDripper(target.transform);
	}

	// Token: 0x06002C75 RID: 11381 RVA: 0x000C2946 File Offset: 0x000C0B46
	private void OnDisable()
	{
		if (this.routine != null)
		{
			base.StopCoroutine(this.routine);
		}
	}

	// Token: 0x06002C76 RID: 11382 RVA: 0x000C295C File Offset: 0x000C0B5C
	private void OnEnable()
	{
		this.skipFlickAway = false;
	}

	// Token: 0x06002C77 RID: 11383 RVA: 0x000C2968 File Offset: 0x000C0B68
	private void Update()
	{
		if (this.rb != null && !this.flickingAway && Math.Abs(this.rb.linearVelocity.x) >= 20f)
		{
			if (this.routine != null)
			{
				base.StopCoroutine(this.routine);
			}
			this.routine = base.StartCoroutine(this.FlickAway());
			this.flickingAway = true;
		}
	}

	// Token: 0x06002C78 RID: 11384 RVA: 0x000C29D4 File Offset: 0x000C0BD4
	private IEnumerator Behaviour()
	{
		yield return new WaitForSeconds(0.04f);
		WaitForSeconds frequency = new WaitForSeconds(0.025f);
		float elapsed = 0f;
		while (elapsed <= 0.7f)
		{
			yield return frequency;
			elapsed += 0.025f;
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.spatterPrefab,
				SpeedMin = 0f,
				SpeedMax = 1f,
				AmountMin = 1,
				AmountMax = 1,
				AngleMin = 90f,
				AngleMax = 90f,
				OriginVariationX = 0.5f,
				OriginVariationY = 0.8f
			}, base.transform, Vector3.zero, this.spawnedFlingTracker, -1f);
			if (this.OnSpawned != null)
			{
				this.OnSpawned(this.spawnedFlingTracker);
			}
			this.spawnedFlingTracker.Clear();
		}
		this.routine = null;
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x06002C79 RID: 11385 RVA: 0x000C29E3 File Offset: 0x000C0BE3
	private IEnumerator FlickAway()
	{
		if (!this.skipFlickAway)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			this.flickOffParticle.Play();
			float angleMin;
			float angleMax;
			if (base.transform.lossyScale.x > 0f)
			{
				angleMin = 20f;
				angleMax = 60f;
			}
			else
			{
				angleMin = 120f;
				angleMax = 160f;
			}
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.spatterPrefab,
				SpeedMin = 10f,
				SpeedMax = 20f,
				AmountMin = 4,
				AmountMax = 5,
				AngleMin = angleMin,
				AngleMax = angleMax,
				OriginVariationX = 0.5f,
				OriginVariationY = 0.8f
			}, base.transform, Vector3.zero, this.spawnedFlingTracker, -1f);
			if (this.OnSpawned != null)
			{
				this.OnSpawned(this.spawnedFlingTracker);
			}
			this.spawnedFlingTracker.Clear();
		}
		yield return new WaitForSeconds(1f);
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x06002C7A RID: 11386 RVA: 0x000C29F2 File Offset: 0x000C0BF2
	public void SkipFlickaway()
	{
		this.skipFlickAway = true;
	}

	// Token: 0x04002E0D RID: 11789
	private const float FLICK_AWAY_SPEED = 20f;

	// Token: 0x04002E0F RID: 11791
	[SerializeField]
	private GameObject spatterPrefab;

	// Token: 0x04002E10 RID: 11792
	[SerializeField]
	private ParticleSystem flickOffParticle;

	// Token: 0x04002E11 RID: 11793
	private bool flickingAway;

	// Token: 0x04002E12 RID: 11794
	private bool skipFlickAway;

	// Token: 0x04002E13 RID: 11795
	private List<GameObject> spawnedFlingTracker = new List<GameObject>();

	// Token: 0x04002E14 RID: 11796
	private Coroutine routine;

	// Token: 0x04002E15 RID: 11797
	private Rigidbody2D rb;
}
