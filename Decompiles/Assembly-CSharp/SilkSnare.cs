using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000550 RID: 1360
public class SilkSnare : MonoBehaviour
{
	// Token: 0x06003098 RID: 12440 RVA: 0x000D6A60 File Offset: 0x000D4C60
	private void Awake()
	{
		this.particleSystems = base.GetComponentsInChildren<ParticleSystem>();
		this.groundTrigger.OnTriggerEntered += this.OnGroundTriggerEntered;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "BENCHREST").ReceivedEvent += this.End;
		this.CalculateSqureValues();
		this.canUpdateFade = (this.groundFadeGroup != null && this.distantGroundFadeGroup != null);
	}

	// Token: 0x06003099 RID: 12441 RVA: 0x000D6ADA File Offset: 0x000D4CDA
	private void OnValidate()
	{
		this.CalculateSqureValues();
	}

	// Token: 0x0600309A RID: 12442 RVA: 0x000D6AE4 File Offset: 0x000D4CE4
	private void OnEnable()
	{
		SilkSnare._activeSnares.Add(this);
		this.appearRoutine = base.StartCoroutine(this.Appear());
		this.hc = HeroController.instance;
		this.foundHero = (this.hc != null);
		this.UpdateFade();
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x000D6B34 File Offset: 0x000D4D34
	private void OnDisable()
	{
		if (this.appearRoutine != null)
		{
			base.StopCoroutine(this.appearRoutine);
			this.appearRoutine = null;
		}
		SilkSnare._activeSnares.Remove(this);
		if (this.endRoutine != null)
		{
			base.StopCoroutine(this.endRoutine);
			this.endRoutine = null;
		}
	}

	// Token: 0x0600309C RID: 12444 RVA: 0x000D6B83 File Offset: 0x000D4D83
	private void Update()
	{
		if (Time.timeAsDouble > this.nextUpdate)
		{
			this.UpdateFade();
		}
	}

	// Token: 0x0600309D RID: 12445 RVA: 0x000D6B98 File Offset: 0x000D4D98
	private void OnGroundTriggerEntered(Collider2D col, GameObject sender)
	{
		if (this.endRoutine != null)
		{
			return;
		}
		if (!col.GetComponentInParent<HealthManager>())
		{
			return;
		}
		this.endRoutine = base.StartCoroutine(this.Blast());
	}

	// Token: 0x0600309E RID: 12446 RVA: 0x000D6BC4 File Offset: 0x000D4DC4
	private bool IsAnyParticles()
	{
		ParticleSystem[] array = this.particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600309F RID: 12447 RVA: 0x000D6BF4 File Offset: 0x000D4DF4
	private void StopAllParticles()
	{
		ParticleSystem[] array = this.particleSystems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(false, ParticleSystemStopBehavior.StopEmitting);
		}
	}

	// Token: 0x060030A0 RID: 12448 RVA: 0x000D6C20 File Offset: 0x000D4E20
	private IEnumerator Blast()
	{
		this.StopAllParticles();
		this.animator.Play(SilkSnare._blastAnim);
		yield return new WaitForSeconds(this.blastRecycleTimer);
		while (this.IsAnyParticles())
		{
			yield return null;
		}
		base.gameObject.Recycle();
		this.endRoutine = null;
		yield break;
	}

	// Token: 0x060030A1 RID: 12449 RVA: 0x000D6C2F File Offset: 0x000D4E2F
	public void End()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.endRoutine != null)
		{
			return;
		}
		this.endRoutine = base.StartCoroutine(this.EndRoutine());
	}

	// Token: 0x060030A2 RID: 12450 RVA: 0x000D6C5A File Offset: 0x000D4E5A
	private IEnumerator Appear()
	{
		this.animator.Play(SilkSnare._appearAnim, 0, 0f);
		yield return null;
		Vector2 b = base.transform.position;
		using (List<SilkSnare>.Enumerator enumerator = SilkSnare._activeSnares.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				SilkSnare silkSnare = enumerator.Current;
				if (!(silkSnare == this) && Vector2.Distance(silkSnare.transform.position, b) < 2f)
				{
					silkSnare.End();
				}
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x060030A3 RID: 12451 RVA: 0x000D6C69 File Offset: 0x000D4E69
	private IEnumerator EndRoutine()
	{
		this.StopAllParticles();
		this.animator.Play(SilkSnare._disappearAnim);
		yield return new WaitForSeconds(this.disappearRecycleTimer);
		while (this.IsAnyParticles())
		{
			yield return null;
		}
		base.gameObject.Recycle();
		this.endRoutine = null;
		yield break;
	}

	// Token: 0x060030A4 RID: 12452 RVA: 0x000D6C78 File Offset: 0x000D4E78
	private void CalculateSqureValues()
	{
		this.fullEffectRadiusSqr = this.fullEffectRadius * this.fullEffectRadius;
		this.falloffRadiusSqr = this.falloffRadius * this.falloffRadius;
	}

	// Token: 0x060030A5 RID: 12453 RVA: 0x000D6CA0 File Offset: 0x000D4EA0
	public float CalculateFalloff(Vector2 targetPosition, Vector2 originPosition)
	{
		this.insideRange = false;
		float sqrMagnitude = (targetPosition - originPosition).sqrMagnitude;
		if (sqrMagnitude >= this.falloffRadiusSqr)
		{
			return 0f;
		}
		this.insideRange = true;
		if (sqrMagnitude <= this.fullEffectRadiusSqr)
		{
			return 1f;
		}
		float num = sqrMagnitude - this.fullEffectRadiusSqr;
		float num2 = this.falloffRadiusSqr - this.fullEffectRadiusSqr;
		return 1f - num / num2;
	}

	// Token: 0x060030A6 RID: 12454 RVA: 0x000D6D0C File Offset: 0x000D4F0C
	private void UpdateFade()
	{
		if (!this.canUpdateFade)
		{
			return;
		}
		if (!this.foundHero)
		{
			this.nextUpdate = Time.timeAsDouble + (double)(this.blastRecycleTimer * 2f);
			return;
		}
		float num = this.CalculateFalloff(this.hc.transform.position, base.transform.position);
		this.groundFadeGroup.AlphaSelf = num;
		this.distantGroundFadeGroup.AlphaSelf = 1f - num;
		if (!this.insideRange)
		{
			this.nextUpdate = Time.timeAsDouble + 0.10000000149011612;
		}
	}

	// Token: 0x060030A7 RID: 12455 RVA: 0x000D6DAC File Offset: 0x000D4FAC
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, this.fullEffectRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.position, this.falloffRadius);
	}

	// Token: 0x0400339A RID: 13210
	private static readonly int _appearAnim = Animator.StringToHash("Appear");

	// Token: 0x0400339B RID: 13211
	private static readonly int _disappearAnim = Animator.StringToHash("Disappear");

	// Token: 0x0400339C RID: 13212
	private static readonly int _blastAnim = Animator.StringToHash("Blast");

	// Token: 0x0400339D RID: 13213
	[SerializeField]
	private Animator animator;

	// Token: 0x0400339E RID: 13214
	[SerializeField]
	private TriggerEnterEvent groundTrigger;

	// Token: 0x0400339F RID: 13215
	[SerializeField]
	private float blastRecycleTimer;

	// Token: 0x040033A0 RID: 13216
	[SerializeField]
	private float disappearRecycleTimer;

	// Token: 0x040033A1 RID: 13217
	[Header("Fade Settings")]
	[SerializeField]
	private NestedFadeGroup groundFadeGroup;

	// Token: 0x040033A2 RID: 13218
	[SerializeField]
	private NestedFadeGroup distantGroundFadeGroup;

	// Token: 0x040033A3 RID: 13219
	[Space]
	[SerializeField]
	private float fullEffectRadius = 5f;

	// Token: 0x040033A4 RID: 13220
	[SerializeField]
	private float falloffRadius = 10f;

	// Token: 0x040033A5 RID: 13221
	private float fullEffectRadiusSqr;

	// Token: 0x040033A6 RID: 13222
	private float falloffRadiusSqr;

	// Token: 0x040033A7 RID: 13223
	private double nextUpdate;

	// Token: 0x040033A8 RID: 13224
	private bool foundHero;

	// Token: 0x040033A9 RID: 13225
	private HeroController hc;

	// Token: 0x040033AA RID: 13226
	private Coroutine appearRoutine;

	// Token: 0x040033AB RID: 13227
	private Coroutine endRoutine;

	// Token: 0x040033AC RID: 13228
	private ParticleSystem[] particleSystems;

	// Token: 0x040033AD RID: 13229
	private static List<SilkSnare> _activeSnares = new List<SilkSnare>();

	// Token: 0x040033AE RID: 13230
	private bool canUpdateFade;

	// Token: 0x040033AF RID: 13231
	private bool insideRange;
}
