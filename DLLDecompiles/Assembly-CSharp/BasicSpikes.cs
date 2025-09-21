using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000492 RID: 1170
public class BasicSpikes : MonoBehaviour, IHitResponder
{
	// Token: 0x06002A36 RID: 10806 RVA: 0x000B7290 File Offset: 0x000B5490
	private bool? IsAnimatorParamValid(string name)
	{
		if (!this.animator || string.IsNullOrEmpty(name))
		{
			return null;
		}
		return new bool?(this.animator.HasParameter(name, null));
	}

	// Token: 0x06002A37 RID: 10807 RVA: 0x000B72D8 File Offset: 0x000B54D8
	private void OnValidate()
	{
		this.activeBoolHash = Animator.StringToHash(this.activeBool);
		this.shineTriggerHash = Animator.StringToHash(this.shineTrigger);
		this.hitTriggerHash = Animator.StringToHash(this.hitTrigger);
		this.idleOffsetFloatHash = Animator.StringToHash(this.idleOffsetFloat);
	}

	// Token: 0x06002A38 RID: 10808 RVA: 0x000B7329 File Offset: 0x000B5529
	private void Awake()
	{
		this.OnValidate();
		if (this.damager)
		{
			this.damageAmount = this.damager.damageDealt;
		}
	}

	// Token: 0x06002A39 RID: 10809 RVA: 0x000B734F File Offset: 0x000B554F
	private void OnEnable()
	{
		if (this.activateTrigger && this.animator)
		{
			base.StartCoroutine(this.Behaviour());
		}
	}

	// Token: 0x06002A3A RID: 10810 RVA: 0x000B7378 File Offset: 0x000B5578
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06002A3B RID: 10811 RVA: 0x000B7380 File Offset: 0x000B5580
	private IEnumerator Behaviour()
	{
		this.DisableSpikeDamager();
		for (;;)
		{
			this.animator.SetFloatIfExists(this.idleOffsetFloatHash, Random.Range(0f, 1f));
			this.animator.SetBoolIfExists(this.activeBoolHash, false);
			this.isOut = false;
			yield return new WaitUntil(() => this.isOut || this.activateTrigger.InsideCount > 0);
			yield return new WaitForSecondsInterruptable(this.activateDelay.GetRandomValue(), () => this.isOut, false);
			this.animator.SetBoolIfExists(this.activeBoolHash, true);
			this.isOut = true;
			float shineDelay = this.shineDelay.GetRandomValue();
			float shineElapsed = 0f;
			float stayOutDuration = this.stayOutDuration.GetRandomValue();
			float noneInsideElapsed = 0f;
			while (noneInsideElapsed < stayOutDuration)
			{
				if (this.didHit || this.activateTrigger.InsideCount > 0)
				{
					noneInsideElapsed = 0f;
				}
				if (shineElapsed > shineDelay)
				{
					shineElapsed = 0f;
					if (!this.didHit)
					{
						this.animator.SetTriggerIfExists(this.shineTriggerHash);
					}
				}
				this.didHit = false;
				yield return null;
				noneInsideElapsed += Time.deltaTime;
				shineElapsed += Time.deltaTime;
			}
		}
		yield break;
	}

	// Token: 0x06002A3C RID: 10812 RVA: 0x000B738F File Offset: 0x000B558F
	public void EnableSpikeDamager()
	{
		if (this.damager)
		{
			this.damager.damageDealt = this.damageAmount;
		}
	}

	// Token: 0x06002A3D RID: 10813 RVA: 0x000B73AF File Offset: 0x000B55AF
	public void DisableSpikeDamager()
	{
		if (this.damager)
		{
			this.damager.damageDealt = 0;
		}
	}

	// Token: 0x06002A3E RID: 10814 RVA: 0x000B73CC File Offset: 0x000B55CC
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!damageInstance.IsNailDamage || damageInstance.DamageDealt <= 0)
		{
			return IHitResponder.Response.None;
		}
		if (this.isOut)
		{
			this.animator.SetTriggerIfExists(this.hitTriggerHash);
			this.didHit = true;
			GameObject[] array = this.hitSpawnPrefabs;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Spawn(base.transform.position);
			}
		}
		else
		{
			this.isOut = true;
		}
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x04002AA7 RID: 10919
	[SerializeField]
	private Animator animator;

	// Token: 0x04002AA8 RID: 10920
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsAnimatorParamValid")]
	private string activeBool;

	// Token: 0x04002AA9 RID: 10921
	private int activeBoolHash;

	// Token: 0x04002AAA RID: 10922
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsAnimatorParamValid")]
	private string shineTrigger;

	// Token: 0x04002AAB RID: 10923
	private int shineTriggerHash;

	// Token: 0x04002AAC RID: 10924
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsAnimatorParamValid")]
	private string hitTrigger;

	// Token: 0x04002AAD RID: 10925
	private int hitTriggerHash;

	// Token: 0x04002AAE RID: 10926
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsAnimatorParamValid")]
	private string idleOffsetFloat;

	// Token: 0x04002AAF RID: 10927
	private int idleOffsetFloatHash;

	// Token: 0x04002AB0 RID: 10928
	[Space]
	[SerializeField]
	private TrackTriggerObjects activateTrigger;

	// Token: 0x04002AB1 RID: 10929
	[SerializeField]
	private MinMaxFloat stayOutDuration;

	// Token: 0x04002AB2 RID: 10930
	[SerializeField]
	private MinMaxFloat activateDelay;

	// Token: 0x04002AB3 RID: 10931
	[SerializeField]
	private MinMaxFloat shineDelay;

	// Token: 0x04002AB4 RID: 10932
	[Space]
	[SerializeField]
	private DamageHero damager;

	// Token: 0x04002AB5 RID: 10933
	private int damageAmount;

	// Token: 0x04002AB6 RID: 10934
	[Space]
	[SerializeField]
	private GameObject[] hitSpawnPrefabs;

	// Token: 0x04002AB7 RID: 10935
	private bool isOut;

	// Token: 0x04002AB8 RID: 10936
	private bool didHit;
}
