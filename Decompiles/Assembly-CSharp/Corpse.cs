using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020002A7 RID: 679
public class Corpse : MonoBehaviour, IInitialisable, BlackThreadState.IBlackThreadStateReceiver
{
	// Token: 0x17000278 RID: 632
	// (get) Token: 0x06001818 RID: 6168 RVA: 0x0006E107 File Offset: 0x0006C307
	protected virtual bool DoLandEffectsInstantly
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000279 RID: 633
	// (get) Token: 0x06001819 RID: 6169 RVA: 0x0006E10A File Offset: 0x0006C30A
	protected virtual bool DesaturateOnLand
	{
		get
		{
			return false;
		}
	}

	// Token: 0x0600181A RID: 6170 RVA: 0x0006E10D File Offset: 0x0006C30D
	public static bool TryGetCorpse(GameObject gameObject, out Corpse corpse)
	{
		return Corpse.corpses.TryGetValue(gameObject, out corpse);
	}

	// Token: 0x0600181B RID: 6171 RVA: 0x0006E11B File Offset: 0x0006C31B
	private void OnDrawGizmosSelected()
	{
		if (!this.resetRotation)
		{
			return;
		}
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.rotateAroundLocal, 0.2f);
	}

	// Token: 0x0600181C RID: 6172 RVA: 0x0006E14C File Offset: 0x0006C34C
	public virtual bool OnAwake()
	{
		if (this.destroyed)
		{
			return false;
		}
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.OnValidate();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.sprite = base.GetComponent<tk2dSprite>();
		this.spriteAnimator = base.GetComponent<tk2dSpriteAnimator>();
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.bodyCollider = base.GetComponent<Collider2D>();
		this.corpseItems = base.gameObject.GetComponent<CorpseItems>();
		this.landTint = base.gameObject.AddComponent<CorpseLandTint>();
		return true;
	}

	// Token: 0x0600181D RID: 6173 RVA: 0x0006E1E5 File Offset: 0x0006C3E5
	public virtual bool OnStart()
	{
		if (this.destroyed)
		{
			return false;
		}
		this.OnAwake();
		if (this.startCalled)
		{
			return false;
		}
		this.startCalled = true;
		return true;
	}

	// Token: 0x0600181E RID: 6174 RVA: 0x0006E20A File Offset: 0x0006C40A
	protected virtual void Awake()
	{
		this.OnAwake();
		Corpse.corpses[base.gameObject] = this;
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x0006E224 File Offset: 0x0006C424
	private void OnDestroy()
	{
		Corpse.corpses.Remove(base.gameObject);
		this.destroyed = true;
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x0006E240 File Offset: 0x0006C440
	private void OnValidate()
	{
		if (this.startAudio.Clip)
		{
			this.startAudios = new AudioEventRandom
			{
				Clips = new AudioClip[]
				{
					this.startAudio.Clip
				},
				PitchMin = this.startAudio.PitchMin,
				PitchMax = this.startAudio.PitchMax
			};
			this.startAudio.Clip = null;
		}
	}

	// Token: 0x06001821 RID: 6177 RVA: 0x0006E2B9 File Offset: 0x0006C4B9
	public void Setup(Color? bloodColorOverride, Action<Transform> onCorpseBegin, bool isCorpseRecyclable)
	{
		this.bloodColorOverride = bloodColorOverride;
		this.onCorpseBegin = onCorpseBegin;
		this.isRecyclable = isCorpseRecyclable;
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x0006E2D0 File Offset: 0x0006C4D0
	private void OnEnable()
	{
		if (this.hasStarted)
		{
			this.bounceCount = 0;
			if (this.sprite)
			{
				this.sprite.color = this.initialSpriteColor;
			}
			this.Begin();
		}
		else if (this.sprite)
		{
			this.initialSpriteColor = this.sprite.color;
		}
		this.landTimer = 0f;
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x0006E33B File Offset: 0x0006C53B
	protected virtual void OnDisable()
	{
		this.splashed = false;
		this.hitAcid = false;
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x0006E34B File Offset: 0x0006C54B
	protected virtual void Start()
	{
		if (this.hasStarted)
		{
			return;
		}
		this.OnStart();
		this.Begin();
		this.hasStarted = true;
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x0006E36A File Offset: 0x0006C56A
	protected void PlayStartAudio()
	{
		if (this.hasPlayedStartAudio)
		{
			return;
		}
		this.hasPlayedStartAudio = true;
		this.startAudios.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x0006E39C File Offset: 0x0006C59C
	protected virtual void Begin()
	{
		Transform transform = base.transform;
		if (this.meshRenderer)
		{
			this.meshRenderer.enabled = true;
		}
		if (this.onCorpseBegin != null)
		{
			this.onCorpseBegin(transform);
			this.onCorpseBegin = null;
		}
		this.PlayStartAudio();
		if (this.resetRotation)
		{
			Vector3 point = transform.TransformPoint(this.rotateAroundLocal);
			transform.RotateAround(point, Vector3.forward, -transform.eulerAngles.z);
			transform.SetScaleY(Mathf.Abs(transform.localScale.y));
		}
		if (this.spriteFlash != null)
		{
			this.spriteFlash.flashWhiteQuick();
		}
		if (this.massless)
		{
			this.state = Corpse.States.DeathAnimation;
		}
		else
		{
			this.state = Corpse.States.InAir;
			if (this.spriteAnimator != null)
			{
				this.spriteAnimator.TryPlay("Death Air");
			}
		}
		if (this.DoLandEffectsInstantly)
		{
			this.Land();
		}
		base.StartCoroutine(this.DisableFlame());
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x0006E4A0 File Offset: 0x0006C6A0
	protected void Update()
	{
		if (this.state == Corpse.States.DeathAnimation)
		{
			if (this.spriteAnimator == null || !this.spriteAnimator.Playing)
			{
				this.Complete(true, true);
				return;
			}
		}
		else if (this.state == Corpse.States.InAir)
		{
			this.bouncedThisFrame = false;
			if (base.transform.position.y < -10f)
			{
				this.Complete(true, true);
			}
			float y = this.body.linearVelocity.y;
			if (y < 0.1f && y > -0.1f)
			{
				this.landTimer += Time.deltaTime;
				if (this.landTimer > 0.1f)
				{
					this.Land();
					return;
				}
			}
		}
		else if (this.state == Corpse.States.PendingLandEffects)
		{
			this.landEffectsDelayRemaining -= Time.deltaTime;
			if (this.landEffectsDelayRemaining <= 0f)
			{
				this.Complete(false, false);
			}
		}
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x0006E583 File Offset: 0x0006C783
	private void Complete(bool detachChildren, bool destroyMe)
	{
		this.state = Corpse.States.Complete;
		base.enabled = false;
		if (detachChildren)
		{
			base.transform.DetachChildren();
		}
		if (destroyMe)
		{
			if (this.isRecyclable)
			{
				base.gameObject.Recycle();
				return;
			}
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x0006E5C3 File Offset: 0x0006C7C3
	private void OnCollisionEnter2D()
	{
		this.OnCollision();
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x0006E5CB File Offset: 0x0006C7CB
	private void OnCollisionStay2D()
	{
		this.OnCollision();
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x0006E5D4 File Offset: 0x0006C7D4
	private void OnCollision()
	{
		if (this.state != Corpse.States.InAir)
		{
			return;
		}
		Sweep sweep = new Sweep(this.bodyCollider, 3, 3, 0.1f, 0.01f);
		float num;
		if (sweep.Check(0.1f, 256, out num))
		{
			this.Land();
		}
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x0006E620 File Offset: 0x0006C820
	private void Land()
	{
		if (this.breaker)
		{
			if (this.bouncedThisFrame)
			{
				return;
			}
			this.bounceCount++;
			this.bouncedThisFrame = true;
			if (this.bounceCount >= this.smashBounces)
			{
				this.Smash();
			}
		}
		else
		{
			if (this.spriteAnimator != null && (!this.hitAcid || this.splashed))
			{
				this.spriteAnimator.TryPlay("Death Land");
			}
			this.landEffectsDelayRemaining = 1f;
			if (this.activateOnLand != null)
			{
				this.activateOnLand.SetActive(true);
			}
			this.state = Corpse.States.PendingLandEffects;
			if (!this.hitAcid || this.splashed)
			{
				this.LandEffects();
			}
		}
		if (this.landEventTarget)
		{
			this.landEventTarget.SendEvent("LAND");
		}
		if (this.corpseItems)
		{
			this.corpseItems.ActivatePickup();
		}
		this.splashed = false;
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x0006E716 File Offset: 0x0006C916
	protected virtual void LandEffects()
	{
		this.landTint.Landed(this.DesaturateOnLand);
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x0006E72C File Offset: 0x0006C92C
	protected virtual void Smash()
	{
		if (!this.hitAcid)
		{
			BloodSpawner.SpawnBlood(base.transform.position, 6, 8, 10f, 20f, 75f, 105f, null, 0f);
		}
		this.splatAudioClipTable.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, false, 1f, null);
		if (this.spriteAnimator != null)
		{
			this.spriteAnimator.Play("Death Land");
		}
		this.body.linearVelocity = Vector2.zero;
		this.state = Corpse.States.DeathAnimation;
		if (this.bigBreaker)
		{
			if (!this.hitAcid)
			{
				BloodSpawner.SpawnBlood(base.transform.position, 30, 30, 20f, 30f, 80f, 100f, null, 0f);
			}
			GameCameras instance = GameCameras.instance;
			if (instance)
			{
				instance.cameraShakeFSM.SendEvent("EnemyKillShake");
			}
		}
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x0006E834 File Offset: 0x0006CA34
	public void Acid()
	{
		this.hitAcid = true;
		this.Splashed();
		this.Land();
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x0006E849 File Offset: 0x0006CA49
	public void Splashed()
	{
		if (this.doLandOnSplash)
		{
			this.splashed = true;
		}
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x0006E85A File Offset: 0x0006CA5A
	public void DropThroughFloor(bool waitToDrop)
	{
		if (this.dropRoutine != null)
		{
			base.StopCoroutine(this.dropRoutine);
		}
		this.dropRoutine = base.StartCoroutine(this.DropThroughFloorRoutine(waitToDrop));
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x0006E883 File Offset: 0x0006CA83
	private IEnumerator DropThroughFloorRoutine(bool waitToDrop)
	{
		if (waitToDrop)
		{
			yield return new WaitForSeconds(Random.Range(3f, 6f));
		}
		Collider2D[] componentsInChildren = base.GetComponentsInChildren<Collider2D>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		if (this.body)
		{
			this.body.isKinematic = false;
		}
		yield return new WaitForSeconds(1f);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x0006E899 File Offset: 0x0006CA99
	private IEnumerator DisableFlame()
	{
		yield return new WaitForSeconds(5f);
		yield break;
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x0006E8A1 File Offset: 0x0006CAA1
	public float GetBlackThreadAmount()
	{
		return (float)(this.isBlackThreaded ? 1 : 0);
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x0006E8B0 File Offset: 0x0006CAB0
	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			this.isBlackThreaded = true;
			BlackThreadEffectRendererGroup component = base.GetComponent<BlackThreadEffectRendererGroup>();
			if (component != null)
			{
				component.SetBlackThreadAmount(1f);
				return;
			}
		}
		else
		{
			this.isBlackThreaded = false;
			BlackThreadEffectRendererGroup component2 = base.GetComponent<BlackThreadEffectRendererGroup>();
			if (component2 != null)
			{
				component2.OnRecycled();
			}
		}
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x0006E914 File Offset: 0x0006CB14
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x040016EB RID: 5867
	[SerializeField]
	[FormerlySerializedAs("landEffects")]
	protected GameObject activateOnLand;

	// Token: 0x040016EC RID: 5868
	[SerializeField]
	protected RandomAudioClipTable splatAudioClipTable;

	// Token: 0x040016ED RID: 5869
	[SerializeField]
	private int smashBounces;

	// Token: 0x040016EE RID: 5870
	[SerializeField]
	private bool breaker;

	// Token: 0x040016EF RID: 5871
	[SerializeField]
	private bool bigBreaker;

	// Token: 0x040016F0 RID: 5872
	[SerializeField]
	private bool massless;

	// Token: 0x040016F1 RID: 5873
	[SerializeField]
	private bool resetRotation;

	// Token: 0x040016F2 RID: 5874
	[SerializeField]
	private Vector2 rotateAroundLocal;

	// Token: 0x040016F3 RID: 5875
	[SerializeField]
	protected AudioSource audioPlayerPrefab;

	// Token: 0x040016F4 RID: 5876
	[SerializeField]
	[HideInInspector]
	private AudioEvent startAudio;

	// Token: 0x040016F5 RID: 5877
	[SerializeField]
	private AudioEventRandom startAudios;

	// Token: 0x040016F6 RID: 5878
	[SerializeField]
	private PlayMakerFSM landEventTarget;

	// Token: 0x040016F7 RID: 5879
	[SerializeField]
	private bool doLandOnSplash;

	// Token: 0x040016F8 RID: 5880
	[SerializeField]
	protected float splashLandDelay;

	// Token: 0x040016F9 RID: 5881
	protected bool hitAcid;

	// Token: 0x040016FA RID: 5882
	private bool isRecyclable;

	// Token: 0x040016FB RID: 5883
	protected bool splashed;

	// Token: 0x040016FC RID: 5884
	protected Color? bloodColorOverride;

	// Token: 0x040016FD RID: 5885
	private Action<Transform> onCorpseBegin;

	// Token: 0x040016FE RID: 5886
	private Corpse.States state;

	// Token: 0x040016FF RID: 5887
	private bool bouncedThisFrame;

	// Token: 0x04001700 RID: 5888
	private int bounceCount;

	// Token: 0x04001701 RID: 5889
	private float landEffectsDelayRemaining;

	// Token: 0x04001702 RID: 5890
	private float landTimer;

	// Token: 0x04001703 RID: 5891
	private Coroutine dropRoutine;

	// Token: 0x04001704 RID: 5892
	private bool hasStarted;

	// Token: 0x04001705 RID: 5893
	private Color initialSpriteColor;

	// Token: 0x04001706 RID: 5894
	private bool hasPlayedStartAudio;

	// Token: 0x04001707 RID: 5895
	protected MeshRenderer meshRenderer;

	// Token: 0x04001708 RID: 5896
	protected tk2dSprite sprite;

	// Token: 0x04001709 RID: 5897
	protected tk2dSpriteAnimator spriteAnimator;

	// Token: 0x0400170A RID: 5898
	protected SpriteFlash spriteFlash;

	// Token: 0x0400170B RID: 5899
	protected Rigidbody2D body;

	// Token: 0x0400170C RID: 5900
	protected Collider2D bodyCollider;

	// Token: 0x0400170D RID: 5901
	private CorpseItems corpseItems;

	// Token: 0x0400170E RID: 5902
	private CorpseLandTint landTint;

	// Token: 0x0400170F RID: 5903
	private static Dictionary<GameObject, Corpse> corpses = new Dictionary<GameObject, Corpse>();

	// Token: 0x04001710 RID: 5904
	private bool hasAwaken;

	// Token: 0x04001711 RID: 5905
	private bool startCalled;

	// Token: 0x04001712 RID: 5906
	private bool destroyed;

	// Token: 0x04001713 RID: 5907
	private bool isBlackThreaded;

	// Token: 0x0200158D RID: 5517
	private enum States
	{
		// Token: 0x040087B8 RID: 34744
		NotStarted,
		// Token: 0x040087B9 RID: 34745
		InAir,
		// Token: 0x040087BA RID: 34746
		DeathAnimation,
		// Token: 0x040087BB RID: 34747
		Complete,
		// Token: 0x040087BC RID: 34748
		PendingLandEffects
	}
}
