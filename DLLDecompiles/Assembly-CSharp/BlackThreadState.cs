using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020002A0 RID: 672
public class BlackThreadState : MonoBehaviour, Recoil.IRecoilMultiplier, IInitialisable
{
	// Token: 0x1700026B RID: 619
	// (get) Token: 0x06001773 RID: 6003 RVA: 0x00069BA8 File Offset: 0x00067DA8
	private bool IsInActiveRange
	{
		get
		{
			if (this.lastActiveRangeCheck == Time.frameCount)
			{
				return this.isInActiveRange;
			}
			bool flag = CameraInfoCache.IsWithinBounds(base.transform.position, this.activityRangeBuffer);
			if (this.isInActiveRange == flag)
			{
				return this.isInActiveRange;
			}
			this.isInActiveRange = flag;
			return this.isInActiveRange;
		}
	}

	// Token: 0x1700026C RID: 620
	// (get) Token: 0x06001774 RID: 6004 RVA: 0x00069C02 File Offset: 0x00067E02
	public bool IsBlackThreaded
	{
		get
		{
			return this.isThreaded;
		}
	}

	// Token: 0x1700026D RID: 621
	// (get) Token: 0x06001775 RID: 6005 RVA: 0x00069C0A File Offset: 0x00067E0A
	private Vector2 CentreOffset
	{
		get
		{
			if (!this.overrideCentreOffset)
			{
				return this.centreOffset;
			}
			return this.centreOffsetOverride;
		}
	}

	// Token: 0x1700026E RID: 622
	// (get) Token: 0x06001776 RID: 6006 RVA: 0x00069C21 File Offset: 0x00067E21
	// (set) Token: 0x06001777 RID: 6007 RVA: 0x00069C29 File Offset: 0x00067E29
	public bool IsInForcedSing { get; private set; }

	// Token: 0x1700026F RID: 623
	// (get) Token: 0x06001778 RID: 6008 RVA: 0x00069C32 File Offset: 0x00067E32
	// (set) Token: 0x06001779 RID: 6009 RVA: 0x00069C3A File Offset: 0x00067E3A
	public bool IsVisiblyThreaded { get; private set; }

	// Token: 0x17000270 RID: 624
	// (get) Token: 0x0600177A RID: 6010 RVA: 0x00069C43 File Offset: 0x00067E43
	private bool IsInAttack
	{
		get
		{
			return this.activeAttack;
		}
	}

	// Token: 0x17000271 RID: 625
	// (get) Token: 0x0600177B RID: 6011 RVA: 0x00069C50 File Offset: 0x00067E50
	private bool IsEnemyHidden
	{
		get
		{
			if (base.gameObject.layer != 11)
			{
				return true;
			}
			if (this.hasCollider && !this.collider.enabled)
			{
				return true;
			}
			MeshRenderer[] array = this.tk2dSpriteRenderers;
			if (array != null && array.Length > 0)
			{
				array = this.tk2dSpriteRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].enabled)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}
	}

	// Token: 0x0600177C RID: 6012 RVA: 0x00069CB9 File Offset: 0x00067EB9
	private void Reset()
	{
		this.GetSingFsm();
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x00069CC4 File Offset: 0x00067EC4
	private void OnDrawGizmos()
	{
		if (this.attacks == null)
		{
			return;
		}
		foreach (BlackThreadAttack blackThreadAttack in this.attacks)
		{
			if (blackThreadAttack)
			{
				Collider2D component = base.GetComponent<Collider2D>();
				blackThreadAttack.DrawAttackRangeGizmos(component ? component.bounds.center : base.transform.position);
			}
		}
		if (this.overrideCentreOffset)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.DrawWireSphere(this.centreOffsetOverride, 0.2f);
		}
	}

	// Token: 0x0600177E RID: 6014 RVA: 0x00069D59 File Offset: 0x00067F59
	private void OnValidate()
	{
		if (this.attack)
		{
			this.attacks = new BlackThreadAttack[]
			{
				this.attack
			};
			this.attack = null;
		}
	}

	// Token: 0x0600177F RID: 6015 RVA: 0x00069D84 File Offset: 0x00067F84
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x06001780 RID: 6016 RVA: 0x00069D8D File Offset: 0x00067F8D
	private void Start()
	{
		this.OnStart();
		if (this.isBlackThreadWorld)
		{
			this.SetupThreaded(true);
		}
	}

	// Token: 0x06001781 RID: 6017 RVA: 0x00069DA8 File Offset: 0x00067FA8
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.OnValidate();
		this.healthManager = base.gameObject.GetComponent<HealthManager>();
		if (!this.healthManager)
		{
			Debug.LogError("Enemy has BlackThreadState but no HealthManager! Disabling script.", this);
			base.enabled = false;
			return true;
		}
		this.healthManager.OnDeath += delegate()
		{
			base.StopAllCoroutines();
			if (!this.IsVisiblyThreaded)
			{
				return;
			}
			for (int k = 0; k < this.tk2dSprites.Length; k++)
			{
				this.tk2dSprites[k].color = this.initialColors[k];
			}
		};
		this.attackEndedFunc = (() => !this.IsInAttack);
		this.collider = base.GetComponent<Collider2D>();
		this.hasCollider = this.collider;
		if (!this.singFsm)
		{
			this.GetSingFsm();
		}
		EnemyHitEffectsRegular component = base.GetComponent<EnemyHitEffectsRegular>();
		if (component)
		{
			component.ReceivedHitEffect += this.OnReceivedHitEffect;
		}
		this.stateReceivers = base.GetComponentsInChildren<BlackThreadState.IBlackThreadStateReceiver>(true).ToList<BlackThreadState.IBlackThreadStateReceiver>();
		EnemyDeathEffects component2 = base.GetComponent<EnemyDeathEffects>();
		if (component2)
		{
			component2.CorpseEmitted += delegate(GameObject corpseObj)
			{
				PassBlackThreadState component3 = corpseObj.GetComponent<PassBlackThreadState>();
				if (component3)
				{
					component3.IsBlackThreaded = this.IsVisiblyThreaded;
					component3.ChosenAttack = this.chosenAttack;
				}
			};
		}
		this.tk2dSprites = base.gameObject.GetComponentsInChildren<tk2dSprite>(true);
		this.tk2dSpriteRenderers = new MeshRenderer[this.tk2dSprites.Length];
		this.initialColors = new Color[this.tk2dSprites.Length];
		this.startColors = new Color[this.tk2dSprites.Length];
		for (int i = 0; i < this.tk2dSpriteRenderers.Length; i++)
		{
			this.tk2dSpriteRenderers[i] = this.tk2dSprites[i].GetComponent<MeshRenderer>();
		}
		foreach (PlayMakerFSM playMakerFSM in base.gameObject.GetComponents<PlayMakerFSM>())
		{
			string fsmName = playMakerFSM.FsmName;
			if (fsmName == "Stun Control" || fsmName == "Stun")
			{
				this.stunControlFsm = playMakerFSM;
				this.stunControlAttackBool = this.stunControlFsm.FsmVariables.FindFsmBool("Abyss Attacking");
				break;
			}
		}
		this.rb2d = base.GetComponent<Rigidbody2D>();
		this.hasRb2d = (this.rb2d != null);
		if (this.enemyDeathEffect == null)
		{
			this.enemyDeathEffect = base.GetComponent<EnemyDeathEffects>();
		}
		return true;
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x00069FC8 File Offset: 0x000681C8
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		if (GameManager.instance.GetCurrentMapZoneEnum() == MapZone.MEMORY)
		{
			return true;
		}
		if (!PlayerData.instance.blackThreadWorld)
		{
			return true;
		}
		this.isBlackThreadWorld = true;
		BlackThreadAttack[] blackThreadAttacksDefault;
		if (this.attacks == null || this.attacks.Length == 0)
		{
			blackThreadAttacksDefault = Effects.BlackThreadAttacksDefault;
		}
		else
		{
			blackThreadAttacksDefault = this.attacks;
		}
		if (blackThreadAttacksDefault != null)
		{
			foreach (BlackThreadAttack blackThreadAttack in blackThreadAttacksDefault)
			{
				if (!(blackThreadAttack == null))
				{
					GameObject gameObject = Object.Instantiate<GameObject>(blackThreadAttack.Prefab, base.transform);
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.SetActive(false);
					this.spawnedAttackObjs[blackThreadAttack] = gameObject;
				}
			}
		}
		this.PreSpawnBlackThreadEffects();
		this.FirstThreadedSetUp();
		return true;
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x0006A094 File Offset: 0x00068294
	private void OnEnable()
	{
		this.recoil = base.GetComponent<Recoil>();
		if (this.recoil)
		{
			this.recoil.AddRecoilMultiplier(this);
		}
		if (this.startAlreadyBlackThreadedOnEnable)
		{
			base.StartCoroutine(this.WaitForActive());
			this.SetBlackThreadAmount(1f);
			this.StartAttackTest();
		}
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x0006A0EC File Offset: 0x000682EC
	private void OnDisable()
	{
		if (this.recoil)
		{
			this.recoil.RemoveRecoilMultiplier(this);
			this.recoil = null;
		}
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x0006A10E File Offset: 0x0006830E
	public void PassState(PassBlackThreadState pass)
	{
		this.force = pass.IsBlackThreaded;
		this.startThreaded = pass.IsBlackThreaded;
		this.attacks = new BlackThreadAttack[]
		{
			pass.ChosenAttack
		};
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x0006A13D File Offset: 0x0006833D
	private void GetSingFsm()
	{
		this.singFsm = base.GetComponents<PlayMakerFSM>().FirstOrDefault((PlayMakerFSM fsmComp) => (fsmComp.FsmTemplate ? fsmComp.FsmTemplate.fsm : fsmComp.Fsm).States.Any((FsmState state) => state.Name.IsAny(BlackThreadState._singingStateNames)));
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x0006A170 File Offset: 0x00068370
	private void OnReceivedHitEffect(HitInstance damageInstance, Vector2 origin)
	{
		if (!this.isThreaded)
		{
			return;
		}
		if (!this.IsVisiblyThreaded)
		{
			if (this.waitRangeRoutine != null)
			{
				base.StopCoroutine(this.waitRangeRoutine);
				this.waitRangeRoutine = null;
			}
			this.BecomeThreaded();
			return;
		}
		Effects.DoBlackThreadHit(base.gameObject, damageInstance, origin);
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x0006A1C4 File Offset: 0x000683C4
	private void SetupThreaded(bool isFirst)
	{
		if (!this.isBlackThreadWorld || this.isThreaded)
		{
			return;
		}
		if (!this.singFsm)
		{
			Debug.LogError("No Sing FSM set for " + base.gameObject.name + "! Will not be black threaded.");
			return;
		}
		bool isAnyActive = BlackThreadCore.IsAnyActive;
		if (this.force || isAnyActive || this.willBeThreaded)
		{
			this.isThreaded = true;
		}
		else
		{
			GameObject gameObject = base.gameObject;
			string sceneName = gameObject.scene.name;
			string id = gameObject.name + "_BlackThread";
			SceneData sd = SceneData.instance;
			PersistentItemData<bool> persistentItemData;
			if (sd.PersistentBools.TryGetValue(sceneName, id, out persistentItemData))
			{
				this.isThreaded = persistentItemData.Value;
			}
			else
			{
				this.isThreaded = Probability.GetRandomItemByProbabilityFair<BlackThreadState.ProbabilityBool, bool>(BlackThreadState._threadProbability, ref BlackThreadState._threadProbOverrides, 2f);
				sd.PersistentBools.SetValue(new PersistentItemData<bool>
				{
					SceneName = sceneName,
					ID = id,
					Value = this.isThreaded,
					IsSemiPersistent = true
				});
			}
			if (isFirst)
			{
				this.healthManager.OnDeath += delegate()
				{
					sd.PersistentBools.SetValue(new PersistentItemData<bool>
					{
						SceneName = sceneName,
						ID = id,
						Value = false,
						IsSemiPersistent = true
					});
				};
			}
		}
		this.PreSpawnBlackThreadEffects();
		if (isFirst)
		{
			this.FirstThreadedSetUp();
		}
		this.SetBlackThreadAmount(0f);
		if (!this.isThreaded)
		{
			return;
		}
		this.ChooseAttack(true);
		float num = 2f;
		if (this.useCustomHPMultiplier)
		{
			num = this.customHPMultiplier;
		}
		this.healthManager.hp = Mathf.FloorToInt((float)this.healthManager.hp * num);
		if (this.startThreaded || isAnyActive)
		{
			this.startAlreadyBlackThreadedOnEnable = true;
			base.StartCoroutine(this.WaitForActive());
			this.SetBlackThreadAmount(1f);
			this.StartAttackTest();
			return;
		}
		if (this.willBeThreaded)
		{
			return;
		}
		this.waitRangeRoutine = base.StartCoroutine(this.WaitForInRange());
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x0006A3C4 File Offset: 0x000685C4
	private void FirstThreadedSetUp()
	{
		if (this.hasDoneFirstSetUp)
		{
			return;
		}
		this.FindVoice();
		this.healthManager.OnDeath += delegate()
		{
			if (!this.IsVisiblyThreaded)
			{
				return;
			}
			GameObject blackThreadEnemyDeathEffect = Effects.BlackThreadEnemyDeathEffect;
			if (blackThreadEnemyDeathEffect)
			{
				blackThreadEnemyDeathEffect.Spawn(base.transform).transform.SetParent(null, true);
			}
		};
		foreach (tk2dSprite tk2dSprite in this.tk2dSprites)
		{
			tk2dSprite.ForceBuild();
			tk2dSprite.EnableKeyword("BLACKTHREAD");
		}
		if (!(this.enemyDeathEffect != null))
		{
			this.enemyDeathEffect = base.GetComponent<EnemyDeathEffects>();
			this.enemyDeathEffect != null;
		}
		this.blackThreadedEffects.AddRange(base.GetComponentsInChildren<BlackThreadedEffect>(true));
		SpriteRenderer[] array2 = this.extraSpriteRenderers;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].material.EnableKeyword("BLACKTHREAD");
		}
		MeshRenderer[] array3 = this.extraMeshRenderers;
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i].material.EnableKeyword("BLACKTHREAD");
		}
		this.hasDoneFirstSetUp = true;
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x0006A4B0 File Offset: 0x000686B0
	public void ChooseAttack(bool force)
	{
		if (force || !this.hasChosenAttack)
		{
			if (CheatManager.ForceChosenBlackThreadAttack && CheatManager.ChosenBlackThreadAttack)
			{
				this.chosenAttack = CheatManager.ChosenBlackThreadAttack;
				this.hasChosenAttack = true;
				return;
			}
			if (this.attacks == null || this.attacks.Length == 0)
			{
				this.chosenAttack = Effects.BlackThreadAttacksDefault.GetRandomElement<BlackThreadAttack>();
			}
			else
			{
				this.chosenAttack = this.attacks.GetRandomElement<BlackThreadAttack>();
			}
			this.hasChosenAttack = true;
		}
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x0006A52C File Offset: 0x0006872C
	private void PreSpawnBlackThreadEffects()
	{
		if (this.hasSpawnedBlackThreadEffects)
		{
			return;
		}
		GameObject blackThreadEnemyEffect = Effects.BlackThreadEnemyEffect;
		if (!blackThreadEnemyEffect)
		{
			return;
		}
		this.blackThreadEffectsObject = Object.Instantiate<GameObject>(blackThreadEnemyEffect, base.transform);
		BlackThreadState.MatchEffectsToObject(base.gameObject, this.blackThreadEffectsObject, out this.centreOffset, false);
		this.hasSpawnedBlackThreadEffects = true;
		this.blackThreadEffectsFader = this.blackThreadEffectsObject.GetComponent<NestedFadeGroupBase>();
		this.blackThreadEffectsFader.AlphaSelf = 0f;
		this.blackThreadEffectsObject.SetActive(false);
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x0006A5B0 File Offset: 0x000687B0
	private void UpdateBlackThreadEffects(bool forceOff = false)
	{
		bool flag = !forceOff && this.IsVisiblyThreaded && this.IsInActiveRange && !this.IsEnemyHidden;
		if (flag == this.blackThreadEffectsIsActive)
		{
			return;
		}
		this.PreSpawnBlackThreadEffects();
		this.blackThreadEffectsIsActive = flag;
		if (this.hasSpawnedBlackThreadEffects)
		{
			if (this.blackThreadEffectsFader)
			{
				if (!this.blackThreadEffectsObject.activeSelf)
				{
					this.blackThreadEffectsObject.SetActive(true);
				}
				if (flag)
				{
					this.blackThreadEffectsFader.FadeTo(1f, 0.2f, null, false, null);
					return;
				}
				float fadeTime = 0.2f;
				if (!this.IsEnemyHidden)
				{
					fadeTime = 1f;
				}
				this.blackThreadEffectsFader.FadeTo(0f, fadeTime, null, false, delegate(bool finished)
				{
					if (finished)
					{
						this.blackThreadEffectsObject.SetActive(false);
					}
				});
				return;
			}
			else
			{
				this.blackThreadEffectsObject.SetActive(this.blackThreadEffectsIsActive);
			}
		}
	}

	// Token: 0x0600178D RID: 6029 RVA: 0x0006A688 File Offset: 0x00068888
	private IEnumerator WaitForActive()
	{
		yield return null;
		while (this.IsEnemyHidden)
		{
			yield return null;
		}
		this.SetVisiblyThreaded();
		yield break;
	}

	// Token: 0x0600178E RID: 6030 RVA: 0x0006A697 File Offset: 0x00068897
	private IEnumerator MonitorActive()
	{
		for (;;)
		{
			this.UpdateBlackThreadEffects(false);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600178F RID: 6031 RVA: 0x0006A6A6 File Offset: 0x000688A6
	private IEnumerator WaitForInRange()
	{
		HeroController hc = HeroController.instance;
		Transform hero = hc.transform;
		Transform self = base.transform;
		while (!hc.isHeroInPosition)
		{
			yield return null;
		}
		WaitForSeconds wait = new WaitForSeconds(0.5f);
		float timeLeft = 2f;
		while (timeLeft > 0f)
		{
			Vector2 a = hero.position;
			Vector2 b = self.position;
			if (Vector2.SqrMagnitude(a - b) <= 64f)
			{
				timeLeft -= 0.5f;
			}
			else
			{
				timeLeft = 2f;
			}
			yield return wait;
		}
		this.waitRangeRoutine = null;
		this.BecomeThreaded();
		yield break;
	}

	// Token: 0x06001790 RID: 6032 RVA: 0x0006A6B8 File Offset: 0x000688B8
	private void SetBlackThreadAmount(float amount)
	{
		if (this.block == null)
		{
			this.block = new MaterialPropertyBlock();
		}
		foreach (MeshRenderer meshRenderer in this.tk2dSpriteRenderers)
		{
			meshRenderer.GetPropertyBlock(this.block);
			this.block.SetFloat(BlackThreadState._blackThreadAmountProp, amount);
			meshRenderer.SetPropertyBlock(this.block);
		}
		foreach (BlackThreadedEffect blackThreadedEffect in this.blackThreadedEffects)
		{
			blackThreadedEffect.SetBlackThreadAmount(amount);
		}
	}

	// Token: 0x06001791 RID: 6033 RVA: 0x0006A75C File Offset: 0x0006895C
	public void BecomeThreaded()
	{
		this.ChooseAttack(false);
		if (this.becomeThreadedRoutine == null)
		{
			this.becomeThreadedRoutine = base.StartCoroutine(this.BecomeThreadedRoutine(true));
		}
	}

	// Token: 0x06001792 RID: 6034 RVA: 0x0006A780 File Offset: 0x00068980
	public void BecomeThreadedNoSing()
	{
		this.ChooseAttack(false);
		if (this.becomeThreadedRoutine == null)
		{
			this.becomeThreadedRoutine = base.StartCoroutine(this.BecomeThreadedRoutine(false));
		}
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x0006A7A4 File Offset: 0x000689A4
	private IEnumerator BecomeThreadedRoutine(bool waitForSing)
	{
		if (waitForSing)
		{
			this.IsInForcedSing = true;
			string currentName = this.singFsm.ActiveStateName;
			if (!currentName.Trim().IsAny(BlackThreadState._singingStateNames))
			{
				for (;;)
				{
					yield return null;
					string activeStateName = this.singFsm.ActiveStateName;
					if (currentName != activeStateName)
					{
						currentName = activeStateName;
						if (currentName.Trim().IsAny(BlackThreadState._singingStateNames))
						{
							break;
						}
					}
				}
			}
			currentName = null;
		}
		this.SetVisiblyThreaded();
		GameObject blackThreadEnemyStartEffect = Effects.BlackThreadEnemyStartEffect;
		if (blackThreadEnemyStartEffect)
		{
			blackThreadEnemyStartEffect.Spawn(base.transform, this.CentreOffset);
		}
		yield return new WaitForSeconds(0.4f);
		for (float elapsed = 0f; elapsed < 0.3f; elapsed += Time.deltaTime)
		{
			this.SetBlackThreadAmount(elapsed / 0.3f);
			yield return null;
		}
		this.SetBlackThreadAmount(1f);
		yield return new WaitForSeconds(0.05f);
		if (waitForSing)
		{
			this.IsInForcedSing = false;
		}
		this.StartAttackTest();
		yield break;
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x0006A7BC File Offset: 0x000689BC
	private void SetVisiblyThreaded()
	{
		this.IsVisiblyThreaded = true;
		for (int i = 0; i < this.tk2dSpriteRenderers.Length; i++)
		{
			this.initialColors[i] = this.tk2dSprites[i].color;
		}
		foreach (BlackThreadState.IBlackThreadStateReceiver blackThreadStateReceiver in this.stateReceivers)
		{
			if (blackThreadStateReceiver != null)
			{
				blackThreadStateReceiver.SetIsBlackThreaded(true);
			}
		}
		this.StartPulseRoutine();
		base.StartCoroutine(this.MonitorActive());
		this.childDamagers = base.gameObject.GetComponentsInChildren<DamageHero>(true);
		DamageHero[] array = this.childDamagers;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].damagePropertyFlags |= DamagePropertyFlags.Void;
		}
		this.ChangeVoiceOutput();
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x0006A89C File Offset: 0x00068A9C
	private void StartAttackTest()
	{
		if (this.chosenAttack)
		{
			if (this.attackTestRoutine != null)
			{
				base.StopCoroutine(this.attackTestRoutine);
			}
			this.attackTestRoutine = base.StartCoroutine(this.ThreadAttackTest());
		}
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x0006A8D1 File Offset: 0x00068AD1
	private void StopPulseRoutine()
	{
		if (this.pulseRoutine != null)
		{
			base.StopCoroutine(this.pulseRoutine);
			this.pulseRoutine = null;
		}
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x0006A8EE File Offset: 0x00068AEE
	private void StartPulseRoutine()
	{
		this.StopPulseRoutine();
		this.pulseRoutine = base.StartCoroutine(this.PulseBlack());
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x0006A908 File Offset: 0x00068B08
	public void ResetThreaded()
	{
		base.StopAllCoroutines();
		if (this.IsVisiblyThreaded)
		{
			this.IsVisiblyThreaded = false;
			for (int i = 0; i < this.tk2dSpriteRenderers.Length; i++)
			{
				this.tk2dSprites[i].color = this.initialColors[i];
			}
			this.UpdateBlackThreadEffects(true);
			foreach (BlackThreadState.IBlackThreadStateReceiver blackThreadStateReceiver in this.stateReceivers)
			{
				if (blackThreadStateReceiver != null)
				{
					blackThreadStateReceiver.SetIsBlackThreaded(false);
				}
			}
		}
		this.childDamagers = base.gameObject.GetComponentsInChildren<DamageHero>(true);
		DamageHero[] array = this.childDamagers;
		for (int j = 0; j < array.Length; j++)
		{
			array[j].damagePropertyFlags &= ~DamagePropertyFlags.Void;
		}
		this.isThreaded = false;
		this.IsInForcedSing = false;
		this.SetupThreaded(false);
	}

	// Token: 0x06001799 RID: 6041 RVA: 0x0006A9F8 File Offset: 0x00068BF8
	private IEnumerator ThreadAttackTest()
	{
		Transform hero = HeroController.instance.transform;
		WaitForSecondsInterruptable threadAttackWait = new WaitForSecondsInterruptable(2f, () => this.queuedForceAttack, false);
		for (;;)
		{
			if ((!this.IsInActiveRange || this.IsEnemyHidden) && !this.queuedForceAttack)
			{
				yield return null;
			}
			else
			{
				if (!this.queuedForceAttack)
				{
					threadAttackWait.ResetTimer();
					yield return threadAttackWait;
					if (this.IsEnemyHidden)
					{
						continue;
					}
					Vector3 v = base.transform.TransformPoint(this.CentreOffset);
					Vector3 position = hero.position;
					if (!this.chosenAttack.IsInRange(position, v) || Physics2D.Linecast(v, position, 256) || !Probability.GetRandomItemByProbabilityFair<BlackThreadState.ProbabilityBool, bool>(this.attackProbability, ref this.attackProbOverrides, 2f))
					{
						continue;
					}
				}
				this.queuedForceAttack = false;
				this.IsInForcedSing = true;
				if (this.stunControlAttackBool != null)
				{
					this.stunControlAttackBool.Value = true;
				}
				string currentName = this.singFsm.ActiveStateName;
				string value = currentName.Trim();
				float timeOutLeft = 5f;
				if (!value.IsAny(BlackThreadState._singingStateNames))
				{
					do
					{
						yield return null;
						string activeStateName = this.singFsm.ActiveStateName;
						if (currentName != activeStateName)
						{
							currentName = activeStateName;
							if (currentName.Trim().IsAny(BlackThreadState._singingStateNames))
							{
								break;
							}
						}
						timeOutLeft -= Time.deltaTime;
					}
					while (timeOutLeft > 0f);
				}
				float duration = Effects.BlackThreadEnemyAttackTintDuration;
				AnimationCurve curve = Effects.BlackThreadEnemyAttackTintCurve;
				if (timeOutLeft > 0f)
				{
					this.StopPulseRoutine();
					for (int i = 0; i < this.tk2dSpriteRenderers.Length; i++)
					{
						this.startColors[i] = this.tk2dSprites[i].color;
					}
					for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
					{
						float t = curve.Evaluate(elapsed / duration);
						for (int j = 0; j < this.tk2dSpriteRenderers.Length; j++)
						{
							this.tk2dSprites[j].color = Color.Lerp(this.startColors[j], Color.black, t);
						}
						yield return null;
					}
					if (this.hasRb2d)
					{
						this.rb2d.linearVelocity = Vector2.zero;
					}
					this.DoAttack(this.chosenAttack);
					yield return new WaitForSecondsInterruptable(this.chosenAttack.Duration, this.attackEndedFunc, false);
					this.activeAttack = null;
					this.IsInForcedSing = false;
					if (this.stunControlAttackBool != null)
					{
						this.stunControlAttackBool.Value = false;
					}
					for (float elapsed = 0f; elapsed <= duration; elapsed += Time.deltaTime)
					{
						float t2 = curve.Evaluate(elapsed / duration);
						for (int k = 0; k < this.tk2dSpriteRenderers.Length; k++)
						{
							this.tk2dSprites[k].color = Color.Lerp(Color.black, this.initialColors[k], t2);
						}
						yield return null;
					}
					this.StartPulseRoutine();
				}
				else
				{
					this.IsInForcedSing = false;
					if (this.stunControlAttackBool != null)
					{
						this.stunControlAttackBool.Value = false;
					}
				}
				currentName = null;
				curve = null;
			}
		}
		yield break;
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x0006AA07 File Offset: 0x00068C07
	private IEnumerator PulseBlack()
	{
		float elapsed = 0f;
		float duration = Effects.BlackThreadEnemyPulseTintDuration;
		AnimationCurve curve = Effects.BlackThreadEnemyPulseTintCurve;
		for (;;)
		{
			if (this.IsInActiveRange)
			{
				float t = curve.Evaluate(elapsed / duration);
				for (int i = 0; i < this.tk2dSpriteRenderers.Length; i++)
				{
					this.tk2dSprites[i].color = Color.Lerp(this.initialColors[i], Color.black, t);
				}
				yield return null;
				elapsed += Time.deltaTime;
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x0006AA18 File Offset: 0x00068C18
	private void DoAttack(BlackThreadAttack currentAttack)
	{
		if (!currentAttack)
		{
			return;
		}
		GameObject gameObject;
		if (!this.spawnedAttackObjs.TryGetValue(currentAttack, out gameObject) || !gameObject)
		{
			return;
		}
		gameObject.SetActive(false);
		this.activeAttack = gameObject;
		Transform transform = gameObject.transform;
		transform.localPosition = this.CentreOffset;
		transform.localScale = currentAttack.Prefab.transform.localScale;
		Vector3 localScale = transform.localScale;
		Vector3 lossyScale = transform.lossyScale;
		Vector3 vector = base.transform.TransformPoint(this.CentreOffset);
		Transform transform2 = HeroController.instance.transform;
		if (lossyScale.x < 0f)
		{
			localScale.x *= -1f;
		}
		if (transform2.position.x < vector.x)
		{
			localScale.x *= -1f;
		}
		if (currentAttack.CounterRotate)
		{
			Vector3 localEulerAngles = transform.localEulerAngles;
			Vector3 localEulerAngles2 = base.transform.localEulerAngles;
			localEulerAngles.z = -localEulerAngles2.z;
			if (lossyScale.x < 0f && Mathf.Abs(Mathf.Abs(Mathf.DeltaAngle(localEulerAngles.z, 0f)) - 90f) < 20f)
			{
				localEulerAngles.z += 180f;
			}
			transform.localEulerAngles = localEulerAngles;
		}
		transform.localScale = localScale;
		gameObject.SetActive(true);
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x0006AB7E File Offset: 0x00068D7E
	public void CancelAttack()
	{
		if (!this.IsInAttack)
		{
			return;
		}
		this.activeAttack.SetActive(false);
		this.activeAttack = null;
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x0006AB9C File Offset: 0x00068D9C
	private static void MatchEffectsToObject(GameObject gameObject, GameObject effectObj, out Vector2 centreOffset, bool recycles = false)
	{
		centreOffset = Vector2.zero;
		Transform transform = gameObject.transform;
		Transform transform2 = effectObj.transform;
		transform2.localPosition = Vector3.zero;
		Collider2D component = gameObject.GetComponent<Collider2D>();
		if (!component)
		{
			return;
		}
		Vector2 self;
		Vector3 vector;
		if (component.enabled)
		{
			Bounds bounds = component.bounds;
			self = bounds.size;
			vector = bounds.center;
		}
		else
		{
			BoxCollider2D boxCollider2D = component as BoxCollider2D;
			if (boxCollider2D == null)
			{
				CircleCollider2D circleCollider2D = component as CircleCollider2D;
				if (circleCollider2D == null)
				{
					Debug.LogError("Black Thread Enemy \"" + gameObject.name + "\" has inactive collider that can't be manually handled!", gameObject);
					return;
				}
				float num = circleCollider2D.radius * 2f;
				self = new Vector2(num, num);
				vector = transform.TransformPoint(circleCollider2D.offset);
			}
			else
			{
				self = boxCollider2D.size;
				vector = transform.TransformPoint(boxCollider2D.offset);
			}
		}
		transform2.SetPosition2D(vector);
		centreOffset = transform.InverseTransformPoint(vector);
		Vector3 localScale = transform2.localScale;
		Vector2 other = localScale;
		Vector2 vector2 = self.DivideElements(other);
		float num2 = localScale.x * localScale.y;
		float num3 = vector2.x * vector2.y;
		localScale.x = vector2.x;
		localScale.y = vector2.y;
		transform2.localScale = localScale;
		float num4 = num3 / num2;
		if (recycles)
		{
			num4 = Mathf.Clamp(num4, 1f, 3f);
		}
		foreach (ParticleSystem particleSystem in effectObj.GetComponentsInChildren<ParticleSystem>(true))
		{
			BlackThreadState.<>c__DisplayClass127_0 CS$<>8__locals1 = new BlackThreadState.<>c__DisplayClass127_0();
			CS$<>8__locals1.main = particleSystem.main;
			CS$<>8__locals1.initialMaxParticles = CS$<>8__locals1.main.maxParticles;
			CS$<>8__locals1.main.maxParticles = Mathf.CeilToInt((float)CS$<>8__locals1.main.maxParticles * num4);
			CS$<>8__locals1.emission = particleSystem.emission;
			CS$<>8__locals1.initialRateOverTimeMultiplier = CS$<>8__locals1.emission.rateOverTimeMultiplier;
			BlackThreadState.<>c__DisplayClass127_0 CS$<>8__locals2 = CS$<>8__locals1;
			CS$<>8__locals2.emission.rateOverTimeMultiplier = CS$<>8__locals2.emission.rateOverTimeMultiplier * num4;
			CS$<>8__locals1.initialRateOverDistanceMultiplier = CS$<>8__locals1.emission.rateOverDistanceMultiplier;
			BlackThreadState.<>c__DisplayClass127_0 CS$<>8__locals3 = CS$<>8__locals1;
			CS$<>8__locals3.emission.rateOverDistanceMultiplier = CS$<>8__locals3.emission.rateOverDistanceMultiplier * num4;
			CS$<>8__locals1.particleFadeGroup = particleSystem.GetComponent<NestedFadeGroupParticleSystem>();
			if (CS$<>8__locals1.particleFadeGroup)
			{
				CS$<>8__locals1.particleFadeGroup.UpdateParticlesArraySize();
			}
			if (recycles)
			{
				RecycleResetHandler.Add(particleSystem.gameObject, delegate()
				{
					CS$<>8__locals1.main.maxParticles = CS$<>8__locals1.initialMaxParticles;
					CS$<>8__locals1.emission.rateOverTimeMultiplier = CS$<>8__locals1.initialRateOverTimeMultiplier;
					CS$<>8__locals1.emission.rateOverDistanceMultiplier = CS$<>8__locals1.initialRateOverDistanceMultiplier;
					if (CS$<>8__locals1.particleFadeGroup)
					{
						CS$<>8__locals1.particleFadeGroup.UpdateParticlesArraySize();
					}
				});
			}
		}
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x0006AE38 File Offset: 0x00069038
	public static void HandleDamagerSpawn(GameObject owner, GameObject spawned)
	{
		BlackThreadState component = owner.GetComponent<BlackThreadState>();
		if (!component || !component.IsVisiblyThreaded)
		{
			return;
		}
		CustomTag component2 = spawned.GetComponent<CustomTag>();
		if (component2 && component2.CustomTagType == CustomTag.CustomTagTypes.PreventBlackThread)
		{
			return;
		}
		if (!spawned.GetComponentInChildren<DamageHero>())
		{
			return;
		}
		if (BlackThreadState._tempSprites == null)
		{
			BlackThreadState._tempSprites = new List<tk2dSprite>();
		}
		try
		{
			spawned.GetComponentsInChildren<tk2dSprite>(BlackThreadState._tempSprites);
			foreach (tk2dSprite tk2dSprite in BlackThreadState._tempSprites)
			{
				tk2dSprite.EnableKeyword("BLACKTHREAD");
			}
		}
		finally
		{
			BlackThreadState._tempSprites.Clear();
		}
		GameObject blackThreadPooledEffect = Effects.BlackThreadPooledEffect;
		GameObject effectObj = blackThreadPooledEffect.Spawn(spawned.transform.position);
		effectObj.transform.localScale = blackThreadPooledEffect.transform.localScale;
		effectObj.transform.SetParent(spawned.transform, true);
		Vector2 vector;
		BlackThreadState.MatchEffectsToObject(spawned, effectObj, out vector, true);
		PlayParticleEffects playParticles = effectObj.GetComponent<PlayParticleEffects>();
		playParticles.ClearParticleSystems();
		playParticles.PlayParticleSystems();
		DamageHero[] heroDamagers = spawned.GetComponentsInChildren<DamageHero>();
		DamageHero[] heroDamagers3 = heroDamagers;
		for (int i = 0; i < heroDamagers3.Length; i++)
		{
			heroDamagers3[i].damagePropertyFlags |= DamagePropertyFlags.Void;
		}
		RecycleResetHandler.Add(spawned, delegate(GameObject self)
		{
			effectObj.transform.SetParent(null, true);
			playParticles.StopParticleSystems();
			DamageHero[] heroDamagers2 = heroDamagers;
			for (int j = 0; j < heroDamagers2.Length; j++)
			{
				heroDamagers2[j].damagePropertyFlags &= ~DamagePropertyFlags.Void;
			}
			try
			{
				self.GetComponentsInChildren<tk2dSprite>(BlackThreadState._tempSprites);
				foreach (tk2dSprite tk2dSprite2 in BlackThreadState._tempSprites)
				{
					tk2dSprite2.DisableKeyword("BLACKTHREAD");
				}
			}
			finally
			{
				BlackThreadState._tempSprites.Clear();
			}
		});
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x0006AFD8 File Offset: 0x000691D8
	public bool CheckIsBlackThreaded()
	{
		return this.IsVisiblyThreaded;
	}

	// Token: 0x060017A0 RID: 6048 RVA: 0x0006AFE0 File Offset: 0x000691E0
	public void SetAttackQueued(bool value)
	{
		this.queuedForceAttack = value;
	}

	// Token: 0x060017A1 RID: 6049 RVA: 0x0006AFE9 File Offset: 0x000691E9
	public float GetRecoilMultiplier()
	{
		if (!this.IsInAttack)
		{
			return 1f;
		}
		return 0.2f;
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x0006AFFE File Offset: 0x000691FE
	public void ReportWillBeThreaded()
	{
		this.willBeThreaded = true;
	}

	// Token: 0x060017A3 RID: 6051 RVA: 0x0006B008 File Offset: 0x00069208
	private void FindVoice()
	{
		if (this.voiceState == BlackThreadState.VoiceState.None)
		{
			bool flag = false;
			foreach (AudioEffectTag audioEffectTag in base.GetComponentsInChildren<AudioEffectTag>(true))
			{
				if (audioEffectTag.EffectType == AudioEffectTag.AudioEffectType.BlackThreadVoice)
				{
					AudioSource audioSource = audioEffectTag.AudioSource;
					if (audioSource)
					{
						if (!flag && audioSource.name == "Audio Loop Voice")
						{
							flag = true;
						}
						this.blackThreadVoiceSources.Add(audioSource);
					}
				}
			}
			if (!flag)
			{
				foreach (AudioSource audioSource2 in base.GetComponentsInChildren<AudioSource>(true))
				{
					if (audioSource2.name == "Audio Loop Voice")
					{
						this.voiceState = BlackThreadState.VoiceState.Found;
						this.blackThreadVoiceSources.Add(audioSource2);
						break;
					}
				}
			}
			if (this.blackThreadVoiceSources.Count == 0)
			{
				this.voiceState = BlackThreadState.VoiceState.NotFound;
				return;
			}
			this.voiceState = BlackThreadState.VoiceState.Found;
		}
	}

	// Token: 0x060017A4 RID: 6052 RVA: 0x0006B0E0 File Offset: 0x000692E0
	private void ChangeVoiceOutput()
	{
		this.FindVoice();
		if (this.voiceState == BlackThreadState.VoiceState.Found)
		{
			AudioMixerGroup blackThreadVoiceMixerGroup = Effects.BlackThreadVoiceMixerGroup;
			if (blackThreadVoiceMixerGroup)
			{
				foreach (AudioSource audioSource in this.blackThreadVoiceSources)
				{
					if (audioSource)
					{
						audioSource.outputAudioMixerGroup = blackThreadVoiceMixerGroup;
					}
				}
				this.voiceState = BlackThreadState.VoiceState.ReplacedMixer;
			}
		}
	}

	// Token: 0x060017A7 RID: 6055 RVA: 0x0006B266 File Offset: 0x00069466
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04001616 RID: 5654
	private const float ACTIVATE_RANGE = 8f;

	// Token: 0x04001617 RID: 5655
	private const float ACTIIVATE_RANGE_SQR = 64f;

	// Token: 0x04001618 RID: 5656
	private const float ACTIVATE_TIMER = 2f;

	// Token: 0x04001619 RID: 5657
	private const float ACTIVATE_RANGE_CHECK_DELAY = 0.5f;

	// Token: 0x0400161A RID: 5658
	private const float HP_MULTIPLIER = 2f;

	// Token: 0x0400161B RID: 5659
	private const float ATTACK_CHECK_TIMER = 2f;

	// Token: 0x0400161C RID: 5660
	private const float ATTACK_PROBABILITY = 0.5f;

	// Token: 0x0400161D RID: 5661
	private const float THREAD_PROBABILITY = 0.15f;

	// Token: 0x0400161E RID: 5662
	private const int TERRAIN_LAYER_MASK = 256;

	// Token: 0x0400161F RID: 5663
	private const float RECOIL_MULTIPLIER = 0.2f;

	// Token: 0x04001620 RID: 5664
	private const float EFFECT_ACTIVE_FADE_TIME = 0.2f;

	// Token: 0x04001621 RID: 5665
	private const float EFFECT_ACTIVE_SLOW_FADE_TIME = 1f;

	// Token: 0x04001622 RID: 5666
	public const string BLACK_THREAD_KEYWORD = "BLACKTHREAD";

	// Token: 0x04001623 RID: 5667
	[SerializeField]
	private PlayMakerFSM singFsm;

	// Token: 0x04001624 RID: 5668
	[SerializeField]
	[HideInInspector]
	[Obsolete("Upgraded, please use attacks array.")]
	private BlackThreadAttack attack;

	// Token: 0x04001625 RID: 5669
	[SerializeField]
	[AssetPickerDropdown]
	private BlackThreadAttack[] attacks;

	// Token: 0x04001626 RID: 5670
	[SerializeField]
	private bool overrideCentreOffset;

	// Token: 0x04001627 RID: 5671
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideCentreOffset", true, false, false)]
	private Vector2 centreOffsetOverride;

	// Token: 0x04001628 RID: 5672
	[SerializeField]
	private bool force;

	// Token: 0x04001629 RID: 5673
	[SerializeField]
	private bool startThreaded;

	// Token: 0x0400162A RID: 5674
	[SerializeField]
	private bool useCustomHPMultiplier;

	// Token: 0x0400162B RID: 5675
	[SerializeField]
	[ModifiableProperty]
	[Conditional("useCustomHPMultiplier", true, false, false)]
	private float customHPMultiplier = 4f;

	// Token: 0x0400162C RID: 5676
	[Space]
	[SerializeField]
	private SpriteRenderer[] extraSpriteRenderers;

	// Token: 0x0400162D RID: 5677
	[SerializeField]
	private MeshRenderer[] extraMeshRenderers;

	// Token: 0x0400162E RID: 5678
	[Space]
	[UnityEngine.Tooltip("Effects / Logic updates will pause when off screen + buffer amount")]
	[SerializeField]
	private Vector2 activityRangeBuffer = new Vector2(4f, 4f);

	// Token: 0x0400162F RID: 5679
	private int lastActiveRangeCheck;

	// Token: 0x04001630 RID: 5680
	private bool isInActiveRange;

	// Token: 0x04001631 RID: 5681
	private bool isBlackThreadWorld;

	// Token: 0x04001632 RID: 5682
	private bool willBeThreaded;

	// Token: 0x04001633 RID: 5683
	private bool isThreaded;

	// Token: 0x04001634 RID: 5684
	private bool queuedForceAttack;

	// Token: 0x04001635 RID: 5685
	private DamageHero[] childDamagers;

	// Token: 0x04001636 RID: 5686
	private tk2dSprite[] tk2dSprites;

	// Token: 0x04001637 RID: 5687
	private MeshRenderer[] tk2dSpriteRenderers;

	// Token: 0x04001638 RID: 5688
	private HealthManager healthManager;

	// Token: 0x04001639 RID: 5689
	private List<BlackThreadState.IBlackThreadStateReceiver> stateReceivers = new List<BlackThreadState.IBlackThreadStateReceiver>();

	// Token: 0x0400163A RID: 5690
	private static List<tk2dSprite> _tempSprites;

	// Token: 0x0400163B RID: 5691
	private bool hasCollider;

	// Token: 0x0400163C RID: 5692
	private Collider2D collider;

	// Token: 0x0400163D RID: 5693
	private Rigidbody2D rb2d;

	// Token: 0x0400163E RID: 5694
	private bool hasRb2d;

	// Token: 0x0400163F RID: 5695
	private PlayMakerFSM stunControlFsm;

	// Token: 0x04001640 RID: 5696
	private FsmBool stunControlAttackBool;

	// Token: 0x04001641 RID: 5697
	private BlackThreadAttack chosenAttack;

	// Token: 0x04001642 RID: 5698
	private readonly Dictionary<BlackThreadAttack, GameObject> spawnedAttackObjs = new Dictionary<BlackThreadAttack, GameObject>();

	// Token: 0x04001643 RID: 5699
	private Color[] initialColors;

	// Token: 0x04001644 RID: 5700
	private Color[] startColors;

	// Token: 0x04001645 RID: 5701
	private Coroutine pulseRoutine;

	// Token: 0x04001646 RID: 5702
	private MaterialPropertyBlock block;

	// Token: 0x04001647 RID: 5703
	private Coroutine becomeThreadedRoutine;

	// Token: 0x04001648 RID: 5704
	private Coroutine waitRangeRoutine;

	// Token: 0x04001649 RID: 5705
	private Recoil recoil;

	// Token: 0x0400164A RID: 5706
	private Vector2 centreOffset;

	// Token: 0x0400164B RID: 5707
	private static readonly string[] _singingStateNames = new string[]
	{
		"Sing",
		"Pray",
		"Needolin",
		"Praying",
		"Sing Ground",
		"Sing Air"
	};

	// Token: 0x0400164C RID: 5708
	private readonly BlackThreadState.ProbabilityBool[] attackProbability = new BlackThreadState.ProbabilityBool[]
	{
		new BlackThreadState.ProbabilityBool(true, 0.5f),
		new BlackThreadState.ProbabilityBool(false, 0.5f)
	};

	// Token: 0x0400164D RID: 5709
	private float[] attackProbOverrides;

	// Token: 0x0400164E RID: 5710
	private static readonly BlackThreadState.ProbabilityBool[] _threadProbability = new BlackThreadState.ProbabilityBool[]
	{
		new BlackThreadState.ProbabilityBool(true, 0.15f),
		new BlackThreadState.ProbabilityBool(false, 0.85f)
	};

	// Token: 0x0400164F RID: 5711
	private static float[] _threadProbOverrides;

	// Token: 0x04001650 RID: 5712
	private static readonly int _blackThreadAmountProp = Shader.PropertyToID("_BlackThreadAmount");

	// Token: 0x04001651 RID: 5713
	private GameObject activeAttack;

	// Token: 0x04001652 RID: 5714
	private Func<bool> attackEndedFunc;

	// Token: 0x04001655 RID: 5717
	private bool blackThreadEffectsIsActive;

	// Token: 0x04001656 RID: 5718
	private bool hasSpawnedBlackThreadEffects;

	// Token: 0x04001657 RID: 5719
	private NestedFadeGroupBase blackThreadEffectsFader;

	// Token: 0x04001658 RID: 5720
	private GameObject blackThreadEffectsObject;

	// Token: 0x04001659 RID: 5721
	private Coroutine attackTestRoutine;

	// Token: 0x0400165A RID: 5722
	private bool hasChosenAttack;

	// Token: 0x0400165B RID: 5723
	private BlackThreadState.VoiceState voiceState;

	// Token: 0x0400165C RID: 5724
	private bool startAlreadyBlackThreadedOnEnable;

	// Token: 0x0400165D RID: 5725
	private List<BlackThreadedEffect> blackThreadedEffects = new List<BlackThreadedEffect>();

	// Token: 0x0400165E RID: 5726
	private bool hasAwaken;

	// Token: 0x0400165F RID: 5727
	private bool hasStarted;

	// Token: 0x04001660 RID: 5728
	private bool hasDoneFirstSetUp;

	// Token: 0x04001661 RID: 5729
	private List<AudioSource> blackThreadVoiceSources = new List<AudioSource>();

	// Token: 0x04001662 RID: 5730
	private EnemyDeathEffects enemyDeathEffect;

	// Token: 0x0200156F RID: 5487
	public interface IBlackThreadStateReceiver
	{
		// Token: 0x060086D9 RID: 34521
		void SetIsBlackThreaded(bool isThreaded);
	}

	// Token: 0x02001570 RID: 5488
	private class ProbabilityBool : Probability.ProbabilityBase<bool>
	{
		// Token: 0x060086DA RID: 34522 RVA: 0x0027413B File Offset: 0x0027233B
		public ProbabilityBool(bool value, float probability)
		{
			this.Item = value;
			this.Probability = probability;
		}

		// Token: 0x17000D9A RID: 3482
		// (get) Token: 0x060086DB RID: 34523 RVA: 0x00274151 File Offset: 0x00272351
		public override bool Item { get; }
	}

	// Token: 0x02001571 RID: 5489
	private enum VoiceState
	{
		// Token: 0x04008728 RID: 34600
		None,
		// Token: 0x04008729 RID: 34601
		NotFound,
		// Token: 0x0400872A RID: 34602
		Found,
		// Token: 0x0400872B RID: 34603
		ReplacedMixer
	}
}
