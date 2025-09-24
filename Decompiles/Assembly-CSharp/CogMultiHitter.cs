using System;
using System.Collections;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020004BF RID: 1215
public class CogMultiHitter : MonoBehaviour, ISceneLintUpgrader
{
	// Token: 0x06002BDD RID: 11229 RVA: 0x000C0334 File Offset: 0x000BE534
	private void Awake()
	{
		this.OnSceneLintUpgrade(true);
		EventRegister.GetRegisterGuaranteed(base.gameObject, "COG DAMAGE").ReceivedEvent += delegate()
		{
			this.CancelDelay();
			this.canDamageTime = Time.timeAsDouble + 1.0;
		};
		this.heroGrindEffect.SetParent(null, true);
		Vector3 localScale = this.heroGrindEffect.localScale;
		localScale.x = Mathf.Abs(localScale.x);
		this.heroGrindEffect.localScale = localScale;
	}

	// Token: 0x06002BDE RID: 11230 RVA: 0x000C03A1 File Offset: 0x000BE5A1
	private void OnDisable()
	{
		this.CancelDelay();
	}

	// Token: 0x06002BDF RID: 11231 RVA: 0x000C03AC File Offset: 0x000BE5AC
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (Time.timeAsDouble < this.canDamageTime)
		{
			return;
		}
		if (other.gameObject.layer != 20)
		{
			return;
		}
		HeroController componentInParent = other.GetComponentInParent<HeroController>();
		if (!componentInParent.isHeroInPosition)
		{
			return;
		}
		if (componentInParent.playerData.isInvincible)
		{
			return;
		}
		this.CancelDelay();
		this.damagingHc = componentInParent;
		this.damagingHc.OnHazardRespawn += this.OnHeroHazardRespawn;
		Vector3 position = componentInParent.transform.position;
		float angleToTarget;
		if (this.useSelfForAngle)
		{
			angleToTarget = this.GetAngleToTarget(position, base.transform.position);
		}
		else
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag("Cog Grind Marker");
			Transform transform = null;
			float num = float.PositiveInfinity;
			foreach (GameObject gameObject in array)
			{
				if (!(gameObject == base.gameObject))
				{
					Transform transform2 = gameObject.transform;
					float sqrMagnitude = (position - transform2.position).sqrMagnitude;
					if (sqrMagnitude <= num)
					{
						num = sqrMagnitude;
						transform = transform2;
					}
				}
			}
			angleToTarget = this.GetAngleToTarget(position, transform ? transform.position : base.transform.position);
		}
		EventRegister.SendEvent(EventRegisterEvents.CogDamage, base.gameObject);
		componentInParent.TakeQuickDamage(1, true);
		this.hitEffectPrefab.Spawn(position);
		EventRegister.SendEvent(EventRegisterEvents.HeroDamaged, null);
		StaticVariableList.SetValue("Wound Sender Override", base.gameObject, 0);
		FSMUtility.SendEventToGameObject(componentInParent.gameObject, "WOUND START", false);
		this.multiHitRumble.DoShake(this, false);
		if (this.jitter)
		{
			this.jitter.StartJitter();
		}
		if (this.cogAnimator)
		{
			this.cogAnimator.FreezePosition = true;
		}
		this.heroGrindEffect.SetPosition2D(position);
		this.heroGrindEffect.SetRotation2D(angleToTarget);
		this.heroGrindEffect.gameObject.SetActive(true);
		this.heroDamageAudio.SpawnAndPlayOneShot(position, null);
		this.delayRoutine = base.StartCoroutine(this.DelayEnd(componentInParent));
	}

	// Token: 0x06002BE0 RID: 11232 RVA: 0x000C05C7 File Offset: 0x000BE7C7
	private void OnHeroHazardRespawn()
	{
		if (this.cogAnimator)
		{
			this.cogAnimator.FreezePosition = false;
		}
		this.CancelDelay();
	}

	// Token: 0x06002BE1 RID: 11233 RVA: 0x000C05E8 File Offset: 0x000BE7E8
	private void CancelDelay()
	{
		if (this.damagingHc)
		{
			this.damagingHc.OnHazardRespawn -= this.OnHeroHazardRespawn;
			this.damagingHc = null;
		}
		if (this.delayRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.delayRoutine);
		this.delayRoutine = null;
		this.multiHitRumble.CancelShake();
		if (this.jitter)
		{
			this.jitter.StopJitter();
		}
		this.heroGrindEffect.gameObject.SetActive(false);
	}

	// Token: 0x06002BE2 RID: 11234 RVA: 0x000C0670 File Offset: 0x000BE870
	private IEnumerator DelayEnd(HeroController hc)
	{
		yield return new WaitForSeconds(0.5f);
		this.multiHitRumble.CancelShake();
		if (this.jitter)
		{
			this.jitter.StopJitter();
		}
		this.DamageHeroDirectly(hc);
		yield return new WaitForSeconds(0.2f);
		this.heroGrindEffect.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06002BE3 RID: 11235 RVA: 0x000C0688 File Offset: 0x000BE888
	private float GetAngleToTarget(Vector2 targetPos, Vector2 sourcePos)
	{
		Vector2 vector = targetPos - sourcePos;
		float num;
		for (num = Mathf.Atan2(vector.y, vector.x) * 57.295776f; num < 0f; num += 360f)
		{
		}
		return num;
	}

	// Token: 0x06002BE4 RID: 11236 RVA: 0x000C06C8 File Offset: 0x000BE8C8
	private void DamageHeroDirectly(HeroController hc)
	{
		hc.CancelDownspikeInvulnerability();
		hc.playerData.isInvincible = false;
		hc.cState.parrying = false;
		hc.TakeDamage(base.gameObject, (base.transform.position.x > hc.transform.position.x) ? CollisionSide.right : CollisionSide.left, 1, HazardType.SPIKES, DamagePropertyFlags.None);
	}

	// Token: 0x06002BE5 RID: 11237 RVA: 0x000C0728 File Offset: 0x000BE928
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Multihitter");
		if (!playMakerFSM)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return "cog_multihitter FSM needs upgrading to CogMultiHitter script";
		}
		FsmBool fsmBool = playMakerFSM.FsmVariables.FindFsmBool("Use Self For Angle");
		if (fsmBool != null)
		{
			this.useSelfForAngle = fsmBool.Value;
		}
		Object.DestroyImmediate(playMakerFSM);
		return "cog_multihitter FSM was upgraded to CogMultiHitter script";
	}

	// Token: 0x04002D3F RID: 11583
	[SerializeField]
	private Transform heroGrindEffect;

	// Token: 0x04002D40 RID: 11584
	[SerializeField]
	private bool useSelfForAngle;

	// Token: 0x04002D41 RID: 11585
	[SerializeField]
	private GameObject hitEffectPrefab;

	// Token: 0x04002D42 RID: 11586
	[SerializeField]
	private CameraShakeTarget multiHitRumble;

	// Token: 0x04002D43 RID: 11587
	[SerializeField]
	private JitterSelf jitter;

	// Token: 0x04002D44 RID: 11588
	[SerializeField]
	private AudioEvent heroDamageAudio;

	// Token: 0x04002D45 RID: 11589
	[SerializeField]
	private VectorCurveAnimator cogAnimator;

	// Token: 0x04002D46 RID: 11590
	private double canDamageTime;

	// Token: 0x04002D47 RID: 11591
	private Coroutine delayRoutine;

	// Token: 0x04002D48 RID: 11592
	private HeroController damagingHc;
}
