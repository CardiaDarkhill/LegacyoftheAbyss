using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200019A RID: 410
public class NailSlashTravel : MonoBehaviour
{
	// Token: 0x06000FEA RID: 4074 RVA: 0x0004CEF3 File Offset: 0x0004B0F3
	private void Reset()
	{
		this.slash = base.GetComponent<NailSlash>();
		this.damager = base.GetComponent<DamageEnemies>();
	}

	// Token: 0x06000FEB RID: 4075 RVA: 0x0004CF10 File Offset: 0x0004B110
	private void Awake()
	{
		this.hasSlash = this.slash;
		if (this.hasSlash)
		{
			this.slash.AttackStarting += this.OnSlashStarting;
			this.slash.EndedDamage += this.OnDamageEnded;
			this.slash.SetLongNeedleHandled();
		}
		if (this.damager)
		{
			this.damager.HitResponded += this.OnDamaged;
		}
		this.collider2D = base.GetComponent<Collider2D>();
		this.hasCollider = (this.collider2D != null);
	}

	// Token: 0x06000FEC RID: 4076 RVA: 0x0004CFB1 File Offset: 0x0004B1B1
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.ResetTransform();
		this.SetInitialPos();
	}

	// Token: 0x06000FED RID: 4077 RVA: 0x0004CFC8 File Offset: 0x0004B1C8
	private void Start()
	{
		if (this.hasStarted)
		{
			return;
		}
		Transform transform = base.transform;
		this.initialLocalPos = transform.localPosition;
		this.initialLocalScale = transform.localScale;
		NailSlashTerrainThunk componentInChildren = base.GetComponentInChildren<NailSlashTerrainThunk>(true);
		if (componentInChildren)
		{
			componentInChildren.Thunked += this.OnThunked;
		}
		this.hc = base.GetComponentInParent<HeroController>();
		this.hc.FlippedSprite += this.OnHeroFlipped;
		this.hasStarted = true;
		this.SetInitialPos();
	}

	// Token: 0x06000FEE RID: 4078 RVA: 0x0004D04F File Offset: 0x0004B24F
	private void OnDestroy()
	{
		if (this.hc)
		{
			this.hc.FlippedSprite -= this.OnHeroFlipped;
		}
	}

	// Token: 0x06000FEF RID: 4079 RVA: 0x0004D078 File Offset: 0x0004B278
	private void FixedUpdate()
	{
		if (!this.hasCollider || !this.isSlashActive)
		{
			return;
		}
		if (this.collider2D.enabled)
		{
			this.wasColliderActive = true;
			if (this.queuedThunkerStateChanged)
			{
				this.SetThunkerActive(this.targetThunkerState);
				return;
			}
		}
		else if (this.wasColliderActive && !this.collider2D.enabled)
		{
			if (this.hasSlash)
			{
				this.slash.SetCollidersActive(true);
			}
			this.SetThunkerActive(true);
		}
	}

	// Token: 0x06000FF0 RID: 4080 RVA: 0x0004D0F0 File Offset: 0x0004B2F0
	private void SetInitialPos()
	{
		if (!this.hc.cState.onGround)
		{
			return;
		}
		Vector3 localPosition = this.initialLocalPos;
		localPosition.y += this.groundedYOffset;
		base.transform.localPosition = localPosition;
		if (this.distanceFiller)
		{
			this.distanceFiller.gameObject.SetActive(false);
		}
		this.SetThunkerActive(false);
	}

	// Token: 0x06000FF1 RID: 4081 RVA: 0x0004D159 File Offset: 0x0004B359
	private void OnDisable()
	{
		this.ResetTransform();
		this.ClearTargets();
	}

	// Token: 0x06000FF2 RID: 4082 RVA: 0x0004D167 File Offset: 0x0004B367
	private void ResetTransform()
	{
		if (!this.hasStarted)
		{
			this.Start();
		}
		Transform transform = base.transform;
		transform.localPosition = this.initialLocalPos;
		transform.localScale = this.initialLocalScale;
	}

	// Token: 0x06000FF3 RID: 4083 RVA: 0x0004D194 File Offset: 0x0004B394
	public void OnSlashStarting()
	{
		this.isSlashActive = true;
		this.wasColliderActive = false;
		if (this.travelRoutine != null)
		{
			base.StopCoroutine(this.travelRoutine);
		}
		this.ResetTransform();
		if (this.particles)
		{
			this.particles.Play(true);
		}
		this.hc.AllowRecoil();
		this.travelRoutine = base.StartCoroutine(this.Travel());
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x0004D1FF File Offset: 0x0004B3FF
	private void OnDamageEnded(bool didHit)
	{
		this.isSlashActive = false;
		this.wasColliderActive = false;
		if (this.distanceFiller)
		{
			this.distanceFiller.gameObject.SetActive(false);
		}
		this.SetThunkerActive(false);
		this.ClearTargets();
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0004D23A File Offset: 0x0004B43A
	private void ClearTargets()
	{
		this.impactTargets.Clear();
		this.bounceTargets.Clear();
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x0004D254 File Offset: 0x0004B454
	private void OnDamaged(DamageEnemies.HitResponse hitResponse)
	{
		if (this.hasSlash && (!this.slash.IsDamagerActive || !this.slash.CanDamageEnemies))
		{
			return;
		}
		PhysLayers layerOnHit = hitResponse.LayerOnHit;
		if (layerOnHit != PhysLayers.TERRAIN && layerOnHit != PhysLayers.ENEMIES && !(hitResponse.HealthManager != null))
		{
			return;
		}
		IHitResponder responder = hitResponse.Responder;
		IBreakerBreakable breakerBreakable = responder as IBreakerBreakable;
		if ((breakerBreakable != null && !breakerBreakable.gameObject.CompareTag("Recoiler")) || responder is HitRigidbody2D || responder is ChainAttackForce || responder is ChainInteraction)
		{
			return;
		}
		Vector3 vector = hitResponse.Target.transform.position;
		if (this.hasCollider)
		{
			vector = this.collider2D.ClosestPoint(vector);
		}
		Vector3 position = base.transform.position;
		Vector3 vector2 = vector - position;
		if (this.maxXOffset.IsEnabled)
		{
			if (vector2.x > this.maxXOffset.Value)
			{
				vector2.x -= this.maxXOffset.Value;
				vector.x -= vector2.x;
			}
			if (vector2.x < -this.maxXOffset.Value)
			{
				vector2.x += this.maxXOffset.Value;
				vector.x += vector2.x;
			}
		}
		if (this.maxYOffset.IsEnabled)
		{
			if (vector2.y > this.maxYOffset.Value)
			{
				vector2.y -= this.maxYOffset.Value;
				vector.y -= vector2.y;
			}
			if (vector2.y < -this.maxYOffset.Value)
			{
				vector2.y += this.maxYOffset.Value;
				vector.y += vector2.y;
			}
		}
		if (this.impactTargets.Add(hitResponse.Target))
		{
			this.StopTravelImpact(vector);
		}
		if (this.bounceTargets.Contains(hitResponse.Target))
		{
			return;
		}
		if (hitResponse.Responder is BouncePod)
		{
			return;
		}
		NonBouncer component = hitResponse.Target.GetComponent<NonBouncer>();
		if (component != null && component.active)
		{
			return;
		}
		if (!this.bounceTargets.Add(hitResponse.Target))
		{
			return;
		}
		this.DoBounceEffect(vector);
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x0004D4AE File Offset: 0x0004B6AE
	private void OnThunked(Vector2 hitPoint)
	{
		if (this.onlyThunkCancelIfColliderEnabled && this.hasCollider && !this.collider2D.enabled)
		{
			return;
		}
		this.StopTravelImpact(hitPoint);
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x0004D4DC File Offset: 0x0004B6DC
	private void StopTravelImpact(Vector3 pos)
	{
		if (this.travelRoutine != null)
		{
			base.StopCoroutine(this.travelRoutine);
			this.travelRoutine = null;
		}
		this.SpawnImpactEffect(pos);
		if (this.slash)
		{
			this.slash.CancelAttack();
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x0004D530 File Offset: 0x0004B730
	private void SpawnImpactEffect(Vector3 pos)
	{
		Transform transform = base.transform;
		Vector3 position = transform.InverseTransformPoint(pos);
		position.x += this.impactOffset.x;
		position.y += this.impactOffset.y;
		Vector3 position2 = transform.TransformPoint(position);
		GameObject gameObject = this.impactPrefab.Spawn(position2, transform.rotation);
		Vector3 lossyScale = transform.lossyScale;
		Vector2 b = new Vector2(-Mathf.Sign(lossyScale.x), Mathf.Sign(lossyScale.y));
		gameObject.transform.localScale = this.impactScale * b * this.impactPrefab.transform.localScale;
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x0004D5F0 File Offset: 0x0004B7F0
	private void SetThunkerActive(bool active)
	{
		this.targetThunkerState = active;
		if (active && !this.wasColliderActive)
		{
			this.queuedThunkerStateChanged = true;
			return;
		}
		this.queuedThunkerStateChanged = false;
		if (this.targetThunkerState == this.currentThunkerState)
		{
			return;
		}
		if (this.terrainThunker)
		{
			this.currentThunkerState = active;
			this.terrainThunker.SetActive(active);
		}
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x0004D64D File Offset: 0x0004B84D
	private IEnumerator Travel()
	{
		NailSlashTravel.<>c__DisplayClass51_0 CS$<>8__locals1 = new NailSlashTravel.<>c__DisplayClass51_0();
		CS$<>8__locals1.<>4__this = this;
		yield return null;
		CS$<>8__locals1.trans = base.transform;
		CS$<>8__locals1.worldPos = CS$<>8__locals1.trans.position;
		this.hc.AllowRecoil();
		bool disabledRecoil = false;
		if (this.hc.cState.onGround)
		{
			NailSlashTravel.<>c__DisplayClass51_0 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.worldPos.y = CS$<>8__locals2.worldPos.y + this.groundedYOffset;
		}
		if (this.isSlashActive)
		{
			if (this.distanceFiller)
			{
				this.distanceFiller.gameObject.SetActive(true);
			}
			this.SetThunkerActive(true);
		}
		CS$<>8__locals1.heroVelocity = this.hc.Body.linearVelocity;
		CS$<>8__locals1.distanceMultiplier = (Gameplay.LongNeedleTool.IsEquipped ? Gameplay.LongNeedleMultiplier : Vector2.one);
		this.setPosition = delegate()
		{
			float x = Mathf.Sign(CS$<>8__locals1.trans.lossyScale.x);
			Vector2 self = CS$<>8__locals1.<>4__this.travelDistance.MultiplyElements(new Vector2(x, 1f));
			Vector2 vector = self.MultiplyElements(CS$<>8__locals1.distanceMultiplier);
			if (CS$<>8__locals1.<>4__this.damager)
			{
				Vector2 normalized = self.normalized;
				CS$<>8__locals1.heroVelocity = ((Vector2.Dot(CS$<>8__locals1.heroVelocity.normalized, normalized) > 0f) ? CS$<>8__locals1.heroVelocity.MultiplyElements(normalized.Abs()) : Vector2.zero);
				vector += CS$<>8__locals1.heroVelocity * (0.5f * CS$<>8__locals1.<>4__this.travelDuration);
			}
			float num = CS$<>8__locals1.<>4__this.travelCurve.Evaluate(CS$<>8__locals1.<>4__this.elapsedT);
			CS$<>8__locals1.trans.SetPosition2D(CS$<>8__locals1.worldPos + vector * num);
			if (CS$<>8__locals1.<>4__this.distanceFiller)
			{
				float num2 = Mathf.Abs(vector.x) * num;
				CS$<>8__locals1.<>4__this.distanceFiller.SetScaleX(num2);
				CS$<>8__locals1.<>4__this.distanceFiller.SetLocalPositionX(num2 * 0.5f);
			}
		};
		for (float elapsed = 0f; elapsed < this.travelDuration; elapsed += Time.deltaTime)
		{
			this.elapsedT = elapsed / this.travelDuration;
			this.setPosition();
			if (!disabledRecoil && this.elapsedT >= this.recoilDistance)
			{
				disabledRecoil = true;
				this.hc.PreventRecoil(1f - this.recoilDistance * this.travelDuration);
			}
			yield return null;
		}
		this.travelRoutine = null;
		yield break;
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x0004D65C File Offset: 0x0004B85C
	private void OnHeroFlipped()
	{
		if (this.travelRoutine == null)
		{
			return;
		}
		if (this.hasSlash)
		{
			this.slash.SetCollidersActive(false);
		}
		if (this.distanceFiller)
		{
			this.distanceFiller.gameObject.SetActive(false);
		}
		this.SetThunkerActive(false);
		Action action = this.setPosition;
		if (action != null)
		{
			action();
		}
		float z = base.transform.localEulerAngles.z;
		bool flag = z > -45f && z < 45f;
		base.transform.FlipLocalScale(flag, !flag, false);
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x0004D6F4 File Offset: 0x0004B8F4
	public void DoBounceEffect(Vector2 fromPos)
	{
		if (!this.bouncePrefab)
		{
			return;
		}
		Vector2 b = this.hc.transform.position;
		float z = (fromPos - b).normalized.DirectionToAngle();
		this.bouncePrefab.Spawn(fromPos, Quaternion.Euler(0f, 0f, z));
	}

	// Token: 0x04000F7D RID: 3965
	[SerializeField]
	private NailSlash slash;

	// Token: 0x04000F7E RID: 3966
	[SerializeField]
	private DamageEnemies damager;

	// Token: 0x04000F7F RID: 3967
	[SerializeField]
	private Transform distanceFiller;

	// Token: 0x04000F80 RID: 3968
	[SerializeField]
	private GameObject terrainThunker;

	// Token: 0x04000F81 RID: 3969
	[Space]
	[SerializeField]
	private float groundedYOffset;

	// Token: 0x04000F82 RID: 3970
	[SerializeField]
	private Vector2 travelDistance;

	// Token: 0x04000F83 RID: 3971
	[SerializeField]
	private float travelDuration;

	// Token: 0x04000F84 RID: 3972
	[SerializeField]
	private AnimationCurve travelCurve;

	// Token: 0x04000F85 RID: 3973
	[SerializeField]
	[Range(0f, 1f)]
	private float recoilDistance;

	// Token: 0x04000F86 RID: 3974
	[Space]
	[SerializeField]
	private GameObject impactPrefab;

	// Token: 0x04000F87 RID: 3975
	[SerializeField]
	private Vector2 impactScale = Vector2.one;

	// Token: 0x04000F88 RID: 3976
	[SerializeField]
	private Vector2 impactOffset;

	// Token: 0x04000F89 RID: 3977
	[SerializeField]
	private OverrideFloat maxXOffset;

	// Token: 0x04000F8A RID: 3978
	[SerializeField]
	private OverrideFloat maxYOffset;

	// Token: 0x04000F8B RID: 3979
	[Space]
	[SerializeField]
	private ParticleSystem particles;

	// Token: 0x04000F8C RID: 3980
	[Space]
	[SerializeField]
	private GameObject bouncePrefab;

	// Token: 0x04000F8D RID: 3981
	[SerializeField]
	private bool onlyThunkCancelIfColliderEnabled;

	// Token: 0x04000F8E RID: 3982
	private float elapsedT;

	// Token: 0x04000F8F RID: 3983
	private Action setPosition;

	// Token: 0x04000F90 RID: 3984
	private bool hasStarted;

	// Token: 0x04000F91 RID: 3985
	private HeroController hc;

	// Token: 0x04000F92 RID: 3986
	private bool isSlashActive;

	// Token: 0x04000F93 RID: 3987
	private Coroutine travelRoutine;

	// Token: 0x04000F94 RID: 3988
	private Vector3 initialLocalPos;

	// Token: 0x04000F95 RID: 3989
	private Vector3 initialLocalScale;

	// Token: 0x04000F96 RID: 3990
	private bool hasCollider;

	// Token: 0x04000F97 RID: 3991
	private Collider2D collider2D;

	// Token: 0x04000F98 RID: 3992
	private bool wasColliderActive;

	// Token: 0x04000F99 RID: 3993
	private bool queuedThunkerStateChanged;

	// Token: 0x04000F9A RID: 3994
	private bool targetThunkerState;

	// Token: 0x04000F9B RID: 3995
	private bool currentThunkerState;

	// Token: 0x04000F9C RID: 3996
	private bool hasSlash;

	// Token: 0x04000F9D RID: 3997
	private HashSet<GameObject> impactTargets = new HashSet<GameObject>();

	// Token: 0x04000F9E RID: 3998
	private HashSet<GameObject> bounceTargets = new HashSet<GameObject>();
}
