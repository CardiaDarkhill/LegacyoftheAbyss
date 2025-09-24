using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002E9 RID: 745
public class FlockFlyer : MonoBehaviour
{
	// Token: 0x06001A5E RID: 6750 RVA: 0x000798C4 File Offset: 0x00077AC4
	private void Awake()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.body = base.GetComponent<Rigidbody2D>();
		FlockFlyer.Appearance randomItemByProbability = Probability.GetRandomItemByProbability<FlockFlyer.ProbabilityAppearance, FlockFlyer.Appearance>(this.appearances, null);
		if (randomItemByProbability.AnimLib)
		{
			this.animator.Library = randomItemByProbability.AnimLib;
		}
		base.transform.Translate(randomItemByProbability.Offset, Space.Self);
		this.deathEffects = base.GetComponent<EnemyDeathEffects>();
		if (this.deathEffects)
		{
			this.deathEffects.CorpsePrefab = randomItemByProbability.CorpsePrefab;
			this.deathEffects.PreInstantiate();
		}
		if (this.heroTrigger)
		{
			this.heroTrigger.OnTriggerEntered += this.OnHeroTriggerEntered;
		}
		if (this.enemyTrigger)
		{
			this.enemyTrigger.OnTriggerEntered += this.OnEnemyTriggerEntered;
		}
		this.singReaction = base.GetComponent<HeroPerformanceSingReaction>();
		if (this.singReaction)
		{
			this.singReaction.OnSingStarted.AddListener(new UnityAction(this.StartSing));
			this.singReaction.OnSingEnded.AddListener(new UnityAction(this.EndSing));
			this.singReaction.OnStartleEnded.AddListener(new UnityAction(this.Flee));
		}
	}

	// Token: 0x06001A5F RID: 6751 RVA: 0x00079A18 File Offset: 0x00077C18
	private void OnEnable()
	{
		if (Random.Range(0f, 1f) > this.activeProbability)
		{
			base.gameObject.SetActive(false);
			return;
		}
		float num = (float)((Random.Range(0f, 1f) > 0.5f) ? 1 : -1);
		Vector3 vector = Vector3.Lerp(this.minScale, this.maxScale, Random.Range(0f, 1f));
		vector.x = Mathf.Abs(vector.x) * num;
		base.transform.localScale = vector;
		if (this.wakeOthersCollider)
		{
			foreach (GameObject gameObject in this.wakeOthersCollider.InsideGameObjects)
			{
				FlockFlyer component = gameObject.GetComponent<FlockFlyer>();
				if (component && component != this)
				{
					this.wakeOthers.Add(component);
				}
			}
		}
		this.body.linearVelocity = Vector2.zero;
		this.force = Vector2.zero;
		this.animator.Play("Idle");
		Collider2D component2 = base.GetComponent<Collider2D>();
		if (component2)
		{
			component2.enabled = base.transform.IsOnHeroPlane();
		}
	}

	// Token: 0x06001A60 RID: 6752 RVA: 0x00079B64 File Offset: 0x00077D64
	private void OnDisable()
	{
		this.wakeOthers.Clear();
		this.isFleeing = false;
		base.StopAllCoroutines();
	}

	// Token: 0x06001A61 RID: 6753 RVA: 0x00079B7E File Offset: 0x00077D7E
	private void FixedUpdate()
	{
		if (this.isFleeing)
		{
			this.body.AddForce(this.force);
		}
	}

	// Token: 0x06001A62 RID: 6754 RVA: 0x00079B9C File Offset: 0x00077D9C
	public void StartSing()
	{
		this.isSinging = true;
		if (this.singReaction && this.singReaction.IsForcedAny)
		{
			return;
		}
		foreach (FlockFlyer flockFlyer in this.wakeOthers)
		{
			if (flockFlyer)
			{
				flockFlyer.StartSingForced();
			}
		}
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x00079C18 File Offset: 0x00077E18
	public void EndSing()
	{
		this.isSinging = false;
		if (this.singReaction && this.singReaction.IsForcedAny)
		{
			return;
		}
		this.Flee();
		foreach (FlockFlyer flockFlyer in this.wakeOthers)
		{
			if (flockFlyer)
			{
				flockFlyer.EndSingForced();
			}
		}
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x00079C9C File Offset: 0x00077E9C
	private void StartSingForced()
	{
		if (this.singReaction)
		{
			this.singReaction.IsForcedSoft = true;
		}
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x00079CB7 File Offset: 0x00077EB7
	private void EndSingForced()
	{
		if (this.singReaction)
		{
			this.singReaction.IsForcedSoft = false;
		}
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x00079CD2 File Offset: 0x00077ED2
	private void OnHeroTriggerEntered(Collider2D col, GameObject sender)
	{
		this.Flee();
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x00079CDA File Offset: 0x00077EDA
	private void OnEnemyTriggerEntered(Collider2D col, GameObject sender)
	{
		if (col.GetComponent<FlockFlyer>())
		{
			return;
		}
		this.Flee();
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x00079CF0 File Offset: 0x00077EF0
	private void Flee()
	{
		if (this.isFleeing || !base.isActiveAndEnabled)
		{
			return;
		}
		this.isFleeing = true;
		base.StartCoroutine(this.FlyAway());
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x00079D17 File Offset: 0x00077F17
	private IEnumerator FlyAway()
	{
		if (this.singReaction)
		{
			while (this.isSinging)
			{
				yield return null;
			}
			if (this.singReaction.enabled)
			{
				this.singReaction.enabled = false;
			}
		}
		yield return new WaitForSeconds(this.fleeReactionTime.GetRandomValue());
		foreach (FlockFlyer flockFlyer in this.wakeOthers)
		{
			if (flockFlyer)
			{
				flockFlyer.Flee();
			}
		}
		Transform transform = base.transform;
		Vector3 pos = transform.position;
		HeroController instance = HeroController.instance;
		Vector3 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x) * Mathf.Sign(pos.x - instance.transform.position.x);
		transform.localScale = scale;
		this.alertSound.SpawnAndPlayOneShot(pos, null);
		yield return base.StartCoroutine(this.animator.PlayAnimWait("Fly Antic", null));
		this.flyAwayStartSound.SpawnAndPlayOneShot(pos, null);
		this.takeOffParticles.Play();
		this.force = new Vector2(this.fleeSideForce.GetRandomValue(), this.fleeRiseForce.GetRandomValue());
		this.force.x = this.force.x * Mathf.Sign(scale.x);
		this.animator.Play("Fly");
		yield return new WaitForSeconds(this.fleeDisableTime);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x0400195A RID: 6490
	[SerializeField]
	private FlockFlyer.ProbabilityAppearance[] appearances;

	// Token: 0x0400195B RID: 6491
	[SerializeField]
	[Range(0f, 1f)]
	private float activeProbability;

	// Token: 0x0400195C RID: 6492
	[Space]
	[SerializeField]
	private TriggerEnterEvent heroTrigger;

	// Token: 0x0400195D RID: 6493
	[SerializeField]
	private TriggerEnterEvent enemyTrigger;

	// Token: 0x0400195E RID: 6494
	[SerializeField]
	private TrackTriggerObjects wakeOthersCollider;

	// Token: 0x0400195F RID: 6495
	[SerializeField]
	private ParticleSystem takeOffParticles;

	// Token: 0x04001960 RID: 6496
	[Space]
	[SerializeField]
	private AudioEventRandom alertSound;

	// Token: 0x04001961 RID: 6497
	[SerializeField]
	private AudioEventRandom flyAwayStartSound;

	// Token: 0x04001962 RID: 6498
	[Space]
	[SerializeField]
	private Vector3 minScale = Vector3.one;

	// Token: 0x04001963 RID: 6499
	[SerializeField]
	private Vector3 maxScale = Vector3.one;

	// Token: 0x04001964 RID: 6500
	[SerializeField]
	private MinMaxFloat fleeRiseForce;

	// Token: 0x04001965 RID: 6501
	[SerializeField]
	private MinMaxFloat fleeSideForce;

	// Token: 0x04001966 RID: 6502
	[SerializeField]
	private float fleeDisableTime;

	// Token: 0x04001967 RID: 6503
	[SerializeField]
	private MinMaxFloat fleeReactionTime;

	// Token: 0x04001968 RID: 6504
	private readonly List<FlockFlyer> wakeOthers = new List<FlockFlyer>();

	// Token: 0x04001969 RID: 6505
	private bool isSinging;

	// Token: 0x0400196A RID: 6506
	private bool isFleeing;

	// Token: 0x0400196B RID: 6507
	private Vector2 force;

	// Token: 0x0400196C RID: 6508
	private tk2dSpriteAnimator animator;

	// Token: 0x0400196D RID: 6509
	private HeroPerformanceSingReaction singReaction;

	// Token: 0x0400196E RID: 6510
	private Rigidbody2D body;

	// Token: 0x0400196F RID: 6511
	private EnemyDeathEffects deathEffects;

	// Token: 0x020015C6 RID: 5574
	[Serializable]
	private struct Appearance
	{
		// Token: 0x04008884 RID: 34948
		public tk2dSpriteAnimation AnimLib;

		// Token: 0x04008885 RID: 34949
		public GameObject CorpsePrefab;

		// Token: 0x04008886 RID: 34950
		public Vector2 Offset;
	}

	// Token: 0x020015C7 RID: 5575
	[Serializable]
	private class ProbabilityAppearance : Probability.ProbabilityBase<FlockFlyer.Appearance>
	{
		// Token: 0x17000DEA RID: 3562
		// (get) Token: 0x06008806 RID: 34822 RVA: 0x00278EC3 File Offset: 0x002770C3
		public override FlockFlyer.Appearance Item
		{
			get
			{
				return this.Appearance;
			}
		}

		// Token: 0x04008887 RID: 34951
		public FlockFlyer.Appearance Appearance;
	}
}
