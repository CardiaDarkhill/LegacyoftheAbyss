using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004F9 RID: 1273
public class HealthFlyer : MonoBehaviour, IHitResponder
{
	// Token: 0x06002D92 RID: 11666 RVA: 0x000C7174 File Offset: 0x000C5374
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		if (this.animator)
		{
			tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
			tk2dSpriteAnimator.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
		}
		if (this.landDetector)
		{
			this.landDetector.CollisionEnteredDirectional += delegate(CollisionEnterEvent.Direction direction, Collision2D _)
			{
				if (!this.landed && direction == CollisionEnterEvent.Direction.Bottom)
				{
					this.DoLand();
				}
			};
		}
	}

	// Token: 0x06002D93 RID: 11667 RVA: 0x000C71F4 File Offset: 0x000C53F4
	private void OnEnable()
	{
		base.transform.SetScaleMatching(Random.Range(1.35f, 1.5f));
		this.activateTime = Time.timeAsDouble + 0.25;
		this.alive = true;
		this.landed = false;
		this.body.gravityScale = 1f;
		if (this.animator)
		{
			this.animator.Play(this.fallAnim);
		}
	}

	// Token: 0x06002D94 RID: 11668 RVA: 0x000C726C File Offset: 0x000C546C
	private IEnumerator Heal()
	{
		Action doHeal = null;
		doHeal = delegate()
		{
			GameManager instance = GameManager.instance;
			instance.AddBlueHealthQueued();
			instance.UnloadingLevel -= doHeal;
			doHeal = null;
		};
		GameManager.instance.UnloadingLevel += doHeal;
		if (HeroController.instance && Vector2.Distance(base.transform.position, HeroController.instance.transform.position) > 40f)
		{
			base.gameObject.SetActive(false);
		}
		yield return new WaitForSeconds(1.2f);
		if (this.screenFlash)
		{
			GameObject gameObject = this.screenFlash.Spawn();
			gameObject.GetComponent<Renderer>().material.SetColor(HealthFlyer._colorId, new Color(0f, 0.7f, 1f));
			PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "Fade Away");
			if (playMakerFSM)
			{
				FSMUtility.SetFloat(playMakerFSM, "Alpha", 0.75f);
			}
		}
		Action doHeal2 = doHeal;
		if (doHeal2 != null)
		{
			doHeal2();
		}
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06002D95 RID: 11669 RVA: 0x000C727C File Offset: 0x000C547C
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (Time.timeAsDouble < this.activateTime)
		{
			return IHitResponder.Response.None;
		}
		if (!this.alive)
		{
			return IHitResponder.Response.None;
		}
		this.alive = false;
		if (this.corpsePrefab)
		{
			Object.Instantiate<GameObject>(this.corpsePrefab, base.transform.position, base.transform.rotation);
		}
		if (this.splatEffectChild)
		{
			this.splatEffectChild.SetActive(true);
		}
		if (this.journalData)
		{
			this.journalData.Get(true);
		}
		bool flag = false;
		if (damageInstance.IsNailDamage)
		{
			flag = true;
			if (this.strikeNailPrefab)
			{
				this.strikeNailPrefab.Spawn(base.transform.position);
			}
			if (this.slashImpactPrefab)
			{
				GameObject gameObject = this.slashImpactPrefab.Spawn(base.transform.position);
				switch (DirectionUtils.GetCardinalDirection(damageInstance.GetOverriddenDirection(base.transform, HitInstance.TargetType.Regular)))
				{
				case 0:
					gameObject.transform.SetRotation2D(Random.Range(340f, 380f));
					gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
					break;
				case 1:
					gameObject.transform.SetRotation2D(Random.Range(70f, 110f));
					gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
					break;
				case 2:
					gameObject.transform.SetRotation2D(Random.Range(340f, 380f));
					gameObject.transform.localScale = new Vector3(-0.9f, 0.9f, 1f);
					break;
				case 3:
					gameObject.transform.SetRotation2D(Random.Range(250f, 290f));
					gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
					break;
				}
			}
		}
		else if (damageInstance.AttackType == AttackTypes.Spell || damageInstance.AttackType == AttackTypes.NailBeam)
		{
			flag = true;
			if (this.fireballHitPrefab)
			{
				GameObject gameObject2 = this.fireballHitPrefab.Spawn(base.transform.position);
				gameObject2.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
				gameObject2.transform.SetPositionZ(0.0031f);
			}
		}
		else if (damageInstance.AttackType == AttackTypes.Generic)
		{
			flag = true;
		}
		this.deathSound1.SpawnAndPlayOneShot(base.transform.position, null);
		this.deathSound2.SpawnAndPlayOneShot(base.transform.position, null);
		this.killShake.DoShake(this, true);
		BloodSpawner.SpawnBlood(base.transform.position, 12, 18, 4f, 22f, 30f, 150f, new Color?(this.bloodColor), 0f);
		Renderer component = base.GetComponent<Renderer>();
		if (component)
		{
			component.enabled = false;
		}
		if (flag)
		{
			if (this.pool)
			{
				this.pool.transform.SetPositionZ(-0.2f);
				FlingUtils.FlingChildren(new FlingUtils.ChildrenConfig
				{
					Parent = this.pool,
					AmountMin = 8,
					AmountMax = 10,
					SpeedMin = 15f,
					SpeedMax = 20f,
					AngleMin = 30f,
					AngleMax = 150f,
					OriginVariationX = 0f,
					OriginVariationY = 0f
				}, base.transform, Vector3.zero, null);
			}
			base.StartCoroutine(this.Heal());
		}
		else
		{
			base.gameObject.SetActive(false);
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x06002D96 RID: 11670 RVA: 0x000C7667 File Offset: 0x000C5867
	public void DoLand()
	{
		this.landed = true;
		if (this.animator)
		{
			this.animator.Play(this.landAnim);
		}
		this.landAudio.SpawnAndPlayOneShot(base.transform.position, false);
	}

	// Token: 0x06002D97 RID: 11671 RVA: 0x000C76A8 File Offset: 0x000C58A8
	private void StartFly()
	{
		if (this.animator)
		{
			this.animator.Play(this.flyAnim);
		}
		this.body.gravityScale = 0f;
		if (this.flyBehaviour)
		{
			this.flyBehaviour.SendEvent("FLY");
		}
		if (this.flyLoopAudio)
		{
			this.flyLoopAudio.Play();
		}
	}

	// Token: 0x06002D98 RID: 11672 RVA: 0x000C7718 File Offset: 0x000C5918
	private void OnAnimationCompleted(tk2dSpriteAnimator _, tk2dSpriteAnimationClip clip)
	{
		if (clip.name == this.landAnim)
		{
			this.StartFly();
		}
	}

	// Token: 0x04002F5C RID: 12124
	[SerializeField]
	private string fallAnim;

	// Token: 0x04002F5D RID: 12125
	[SerializeField]
	private string landAnim;

	// Token: 0x04002F5E RID: 12126
	[SerializeField]
	private string flyAnim;

	// Token: 0x04002F5F RID: 12127
	[Space]
	[SerializeField]
	private CollisionEnterEvent landDetector;

	// Token: 0x04002F60 RID: 12128
	[SerializeField]
	private RandomAudioClipTable landAudio;

	// Token: 0x04002F61 RID: 12129
	[Space]
	[SerializeField]
	private GameObject corpsePrefab;

	// Token: 0x04002F62 RID: 12130
	[SerializeField]
	private GameObject splatEffectChild;

	// Token: 0x04002F63 RID: 12131
	[SerializeField]
	private GameObject strikeNailPrefab;

	// Token: 0x04002F64 RID: 12132
	[SerializeField]
	private GameObject slashImpactPrefab;

	// Token: 0x04002F65 RID: 12133
	[SerializeField]
	private GameObject fireballHitPrefab;

	// Token: 0x04002F66 RID: 12134
	[SerializeField]
	private AudioEvent deathSound1;

	// Token: 0x04002F67 RID: 12135
	[SerializeField]
	private AudioEvent deathSound2;

	// Token: 0x04002F68 RID: 12136
	[SerializeField]
	private GameObject pool;

	// Token: 0x04002F69 RID: 12137
	[SerializeField]
	private GameObject screenFlash;

	// Token: 0x04002F6A RID: 12138
	[SerializeField]
	private Color bloodColor;

	// Token: 0x04002F6B RID: 12139
	[SerializeField]
	private CameraShakeTarget killShake;

	// Token: 0x04002F6C RID: 12140
	[SerializeField]
	private EnemyJournalRecord journalData;

	// Token: 0x04002F6D RID: 12141
	[Space]
	[SerializeField]
	private PlayMakerFSM flyBehaviour;

	// Token: 0x04002F6E RID: 12142
	[SerializeField]
	private AudioSource flyLoopAudio;

	// Token: 0x04002F6F RID: 12143
	private tk2dSpriteAnimator animator;

	// Token: 0x04002F70 RID: 12144
	private Rigidbody2D body;

	// Token: 0x04002F71 RID: 12145
	private bool alive;

	// Token: 0x04002F72 RID: 12146
	private bool landed;

	// Token: 0x04002F73 RID: 12147
	private const float ACTIVATE_DELAY = 0.25f;

	// Token: 0x04002F74 RID: 12148
	private double activateTime;

	// Token: 0x04002F75 RID: 12149
	private static readonly int _colorId = Shader.PropertyToID("_Color");
}
