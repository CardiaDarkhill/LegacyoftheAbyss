using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004F0 RID: 1264
public class Grass : MonoBehaviour, IHitResponder
{
	// Token: 0x06002D4D RID: 11597 RVA: 0x000C5BE5 File Offset: 0x000C3DE5
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		Grass.grasses = new List<Grass>();
	}

	// Token: 0x06002D4E RID: 11598 RVA: 0x000C5BF4 File Offset: 0x000C3DF4
	protected void Reset()
	{
		this.inertBackgroundThreshold = 1.8f;
		this.inertForegroundThreshold = -1.8f;
		this.infectedColor = new Color32(byte.MaxValue, 140, 54, byte.MaxValue);
		this.slashImpactRotationMin = 340f;
		this.slashImpactRotationMax = 380f;
		this.slashImpactScale = 0.6f;
		this.preventPushAnimation = false;
		this.childParticleSystemDuration = 5f;
	}

	// Token: 0x06002D4F RID: 11599 RVA: 0x000C5C6B File Offset: 0x000C3E6B
	protected void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.bodyCollider = base.GetComponent<Collider2D>();
		this.audioSource = base.GetComponent<AudioSource>();
		Grass.grasses.Add(this);
	}

	// Token: 0x06002D50 RID: 11600 RVA: 0x000C5C9C File Offset: 0x000C3E9C
	protected void OnDestroy()
	{
		Grass.grasses.Remove(this);
	}

	// Token: 0x06002D51 RID: 11601 RVA: 0x000C5CAC File Offset: 0x000C3EAC
	protected void Start()
	{
		float z = base.transform.position.z;
		if (z > this.inertBackgroundThreshold || z < this.inertForegroundThreshold)
		{
			base.enabled = false;
			return;
		}
		this.isInfected = (this.isInfectable && GameObject.FindGameObjectWithTag("Infected Flag") != null);
		if (this.isInfected)
		{
			FSMActionReplacements.SetMaterialColor(this, this.infectedColor);
		}
		this.animator.Play(Grass.IdleStateId, 0, Random.Range(0f, 1f));
	}

	// Token: 0x06002D52 RID: 11602 RVA: 0x000C5D39 File Offset: 0x000C3F39
	protected void OnTriggerEnter2D(Collider2D other)
	{
		if (this.preventPushAnimation)
		{
			return;
		}
		this.Push(false);
	}

	// Token: 0x06002D53 RID: 11603 RVA: 0x000C5D4B File Offset: 0x000C3F4B
	public void Push(bool isAllGrass)
	{
		if (this.isCut)
		{
			return;
		}
		if (!isAllGrass)
		{
			this.pushAudioClipTable.PlayOneShot(this.audioSource, false);
		}
		this.animator.Play(Grass.PushStateId, 0, 0f);
	}

	// Token: 0x06002D54 RID: 11604 RVA: 0x000C5D84 File Offset: 0x000C3F84
	public static void PushAll()
	{
		if (Grass.grasses != null)
		{
			for (int i = 0; i < Grass.grasses.Count; i++)
			{
				Grass.grasses[i].Push(true);
			}
		}
	}

	// Token: 0x06002D55 RID: 11605 RVA: 0x000C5DC0 File Offset: 0x000C3FC0
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (damageInstance.DamageDealt <= 0)
		{
			return IHitResponder.Response.None;
		}
		if (this.isCut)
		{
			return IHitResponder.Response.None;
		}
		this.isCut = true;
		this.bodyCollider.enabled = false;
		FSMActionReplacements.Directions directions = FSMActionReplacements.CheckDirectionWithBrokenBehaviour(0f);
		GameObject gameObject = this.slashImpactPrefab.Spawn(base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(this.slashImpactRotationMin, this.slashImpactRotationMax)));
		gameObject.transform.localScale = new Vector3((directions == FSMActionReplacements.Directions.Left) ? (-this.slashImpactScale) : this.slashImpactScale, this.slashImpactScale, 1f);
		Vector3 localPosition = gameObject.transform.localPosition;
		localPosition.z = 0f;
		gameObject.transform.localPosition = localPosition;
		Quaternion rotation = Quaternion.Euler(-90f, -90f, -0.01f);
		if (this.isInfected)
		{
			if (this.infectedCutPrefab0 != null)
			{
				this.infectedCutPrefab0.Spawn(base.transform.position, rotation);
			}
			if (this.infectedCutPrefab1 != null)
			{
				this.infectedCutPrefab1.Spawn(base.transform.position, rotation);
			}
		}
		else
		{
			if (this.cutPrefab0 != null)
			{
				this.cutPrefab0.Spawn(base.transform.position, rotation);
			}
			if (this.cutPrefab1 != null)
			{
				this.cutPrefab1.Spawn(base.transform.position, rotation);
			}
		}
		this.cutAudioClipTable.PlayOneShot(this.audioSource, false);
		this.animator.Play(Grass.DeadStateId, 0, 0f);
		if (!this.isInfected && this.childParticleSystem != null)
		{
			this.childParticleSystem.Play();
			this.childParticleSystemTimer = this.childParticleSystemDuration;
			base.enabled = true;
		}
		else
		{
			base.enabled = false;
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06002D56 RID: 11606 RVA: 0x000C5FB4 File Offset: 0x000C41B4
	protected void Update()
	{
		this.childParticleSystemTimer -= Time.deltaTime;
		if (this.childParticleSystemTimer <= 0f)
		{
			if (this.childParticleSystem != null)
			{
				this.childParticleSystem.Stop();
			}
			base.enabled = false;
		}
	}

	// Token: 0x04002EF0 RID: 12016
	private Animator animator;

	// Token: 0x04002EF1 RID: 12017
	private Collider2D bodyCollider;

	// Token: 0x04002EF2 RID: 12018
	private AudioSource audioSource;

	// Token: 0x04002EF3 RID: 12019
	[SerializeField]
	private bool isInfectable;

	// Token: 0x04002EF4 RID: 12020
	[SerializeField]
	private float inertBackgroundThreshold;

	// Token: 0x04002EF5 RID: 12021
	[SerializeField]
	private float inertForegroundThreshold;

	// Token: 0x04002EF6 RID: 12022
	[SerializeField]
	private Color infectedColor;

	// Token: 0x04002EF7 RID: 12023
	[SerializeField]
	private bool preventPushAnimation;

	// Token: 0x04002EF8 RID: 12024
	[SerializeField]
	private GameObject slashImpactPrefab;

	// Token: 0x04002EF9 RID: 12025
	[SerializeField]
	private float slashImpactRotationMin;

	// Token: 0x04002EFA RID: 12026
	[SerializeField]
	private float slashImpactRotationMax;

	// Token: 0x04002EFB RID: 12027
	[SerializeField]
	private float slashImpactScale;

	// Token: 0x04002EFC RID: 12028
	[SerializeField]
	private GameObject infectedCutPrefab0;

	// Token: 0x04002EFD RID: 12029
	[SerializeField]
	private GameObject infectedCutPrefab1;

	// Token: 0x04002EFE RID: 12030
	[SerializeField]
	private GameObject cutPrefab0;

	// Token: 0x04002EFF RID: 12031
	[SerializeField]
	private GameObject cutPrefab1;

	// Token: 0x04002F00 RID: 12032
	[SerializeField]
	private ParticleSystem childParticleSystem;

	// Token: 0x04002F01 RID: 12033
	[SerializeField]
	private float childParticleSystemDuration;

	// Token: 0x04002F02 RID: 12034
	[SerializeField]
	private RandomAudioClipTable pushAudioClipTable;

	// Token: 0x04002F03 RID: 12035
	[SerializeField]
	private RandomAudioClipTable cutAudioClipTable;

	// Token: 0x04002F04 RID: 12036
	private static readonly int IdleStateId = Animator.StringToHash("Idle");

	// Token: 0x04002F05 RID: 12037
	private static readonly int PushStateId = Animator.StringToHash("Push");

	// Token: 0x04002F06 RID: 12038
	private static readonly int DeadStateId = Animator.StringToHash("Dead");

	// Token: 0x04002F07 RID: 12039
	private bool isInfected;

	// Token: 0x04002F08 RID: 12040
	private bool isCut;

	// Token: 0x04002F09 RID: 12041
	private float childParticleSystemTimer;

	// Token: 0x04002F0A RID: 12042
	private static List<Grass> grasses;

	// Token: 0x020017F6 RID: 6134
	public enum GrassTypes
	{
		// Token: 0x0400901C RID: 36892
		White,
		// Token: 0x0400901D RID: 36893
		Green,
		// Token: 0x0400901E RID: 36894
		SimpleType,
		// Token: 0x0400901F RID: 36895
		Rag,
		// Token: 0x04009020 RID: 36896
		ChildType
	}
}
