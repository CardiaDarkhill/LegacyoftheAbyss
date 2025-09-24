using System;
using System.Collections.Generic;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

// Token: 0x020002CB RID: 715
public class EnemyDeathEffects : MonoBehaviour, IInitialisable, BlackThreadState.IBlackThreadStateReceiver
{
	// Token: 0x170002A2 RID: 674
	// (get) Token: 0x0600198D RID: 6541 RVA: 0x000751E8 File Offset: 0x000733E8
	protected virtual Color? OverrideBloodColor
	{
		get
		{
			return null;
		}
	}

	// Token: 0x14000048 RID: 72
	// (add) Token: 0x0600198E RID: 6542 RVA: 0x00075200 File Offset: 0x00073400
	// (remove) Token: 0x0600198F RID: 6543 RVA: 0x00075238 File Offset: 0x00073438
	public event Action<GameObject> CorpseEmitted;

	// Token: 0x170002A3 RID: 675
	// (get) Token: 0x06001990 RID: 6544 RVA: 0x00075270 File Offset: 0x00073470
	private bool IsVisible
	{
		get
		{
			Renderer renderer = base.GetComponent<Renderer>();
			if (renderer == null)
			{
				renderer = base.GetComponentInChildren<Renderer>();
			}
			return renderer != null && renderer.isVisible;
		}
	}

	// Token: 0x170002A4 RID: 676
	// (get) Token: 0x06001991 RID: 6545 RVA: 0x000752A5 File Offset: 0x000734A5
	// (set) Token: 0x06001992 RID: 6546 RVA: 0x000752AD File Offset: 0x000734AD
	public bool SkipKillFreeze { get; set; }

	// Token: 0x170002A5 RID: 677
	// (get) Token: 0x06001993 RID: 6547 RVA: 0x000752B6 File Offset: 0x000734B6
	// (set) Token: 0x06001994 RID: 6548 RVA: 0x000752C0 File Offset: 0x000734C0
	public GameObject CorpsePrefab
	{
		get
		{
			return this.corpsePrefab;
		}
		set
		{
			if (this.corpsePrefab != value)
			{
				this.corpsePrefab = value;
				if (this.preInstantiated)
				{
					this.preInstantiated = false;
					if (this.instantiatedCorpses != null)
					{
						GameObject[] array = this.instantiatedCorpses;
						for (int i = 0; i < array.Length; i++)
						{
							Object.Destroy(array[i]);
						}
						this.instantiatedCorpses = null;
					}
					this.PreInstantiate();
				}
			}
		}
	}

	// Token: 0x170002A6 RID: 678
	// (get) Token: 0x06001995 RID: 6549 RVA: 0x00075323 File Offset: 0x00073523
	protected bool IsCorpseRecyclable
	{
		get
		{
			return this.isCorpseRecyclable;
		}
	}

	// Token: 0x06001996 RID: 6550 RVA: 0x0007532C File Offset: 0x0007352C
	private void OnValidate()
	{
		if (this.enemyDeathSwordAudio.Clip)
		{
			this.deathSounds.Add(this.enemyDeathSwordAudio);
			this.enemyDeathSwordAudio = default(AudioEvent);
		}
		if (this.enemyDamageAudio.Clip)
		{
			this.deathSounds.Add(this.enemyDamageAudio);
			this.enemyDamageAudio = default(AudioEvent);
		}
	}

	// Token: 0x06001997 RID: 6551 RVA: 0x00075398 File Offset: 0x00073598
	public virtual bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		this.OnValidate();
		if (!this.manualPreInstantiate)
		{
			this.PreInstantiate();
		}
		if (this.isCorpseRecyclable && this.corpsePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.corpsePrefab, 1, true, true, false);
		}
		return true;
	}

	// Token: 0x06001998 RID: 6552 RVA: 0x000753F5 File Offset: 0x000735F5
	public virtual bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x06001999 RID: 6553 RVA: 0x00075410 File Offset: 0x00073610
	protected void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x0600199A RID: 6554 RVA: 0x0007541C File Offset: 0x0007361C
	public void PreInstantiate()
	{
		if (this.isCorpseRecyclable)
		{
			return;
		}
		if (this.preInstantiated)
		{
			return;
		}
		this.preInstantiated = true;
		if (this.instantiatedCorpses != null || (!this.corpsePrefab && this.altCorpses.Length == 0))
		{
			return;
		}
		Transform transform = base.transform;
		this.instantiatedCorpses = new GameObject[this.altCorpses.Length + 1];
		for (int i = 0; i < this.instantiatedCorpses.Length; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>((i == 0) ? this.corpsePrefab : this.altCorpses[i - 1].Prefab, transform.TransformPoint(this.corpseSpawnPoint), Quaternion.identity, transform);
			tk2dSprite[] componentsInChildren = gameObject.GetComponentsInChildren<tk2dSprite>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].ForceBuild();
			}
			IInitialisable.DoFullInit(gameObject);
			gameObject.SetActive(false);
			this.instantiatedCorpses[i] = gameObject;
			if (this.disabledSpecialQuestDrops)
			{
				this.RemoveQuestDropsFromGameObject(gameObject);
			}
		}
		this.disabledSpecialQuestDrops = false;
	}

	// Token: 0x0600199B RID: 6555 RVA: 0x0007551C File Offset: 0x0007371C
	private GameObject GetInstantiatedCorpse(AttackTypes attackType)
	{
		if (this.instantiatedCorpses == null)
		{
			return null;
		}
		for (int i = 0; i < this.altCorpses.Length; i++)
		{
			if (this.altCorpses[i].AttackTypeMask.IsBitSet((int)attackType))
			{
				return this.instantiatedCorpses[i + 1];
			}
		}
		return this.instantiatedCorpses[0];
	}

	// Token: 0x0600199C RID: 6556 RVA: 0x00075574 File Offset: 0x00073774
	public void DisableSpecialQuestDrops()
	{
		if (this.isCorpseRecyclable)
		{
			this.disabledSpecialQuestDrops = true;
			return;
		}
		if (this.preInstantiated)
		{
			if (this.instantiatedCorpses != null)
			{
				foreach (GameObject gameObject in this.instantiatedCorpses)
				{
					if (!(gameObject == null))
					{
						this.RemoveQuestDropsFromGameObject(gameObject);
					}
				}
				return;
			}
		}
		else
		{
			this.disabledSpecialQuestDrops = true;
		}
	}

	// Token: 0x0600199D RID: 6557 RVA: 0x000755D4 File Offset: 0x000737D4
	private void RemoveQuestDropsFromGameObject(GameObject gameObject)
	{
		SpecialQuestItemVariant[] componentsInChildren = gameObject.GetComponentsInChildren<SpecialQuestItemVariant>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].SetInactive();
		}
	}

	// Token: 0x0600199E RID: 6558 RVA: 0x000755FF File Offset: 0x000737FF
	public void ReceiveDeathEvent(float attackDirection)
	{
		this.ReceiveDeathEvent(new float?(attackDirection), AttackTypes.Generic, false);
	}

	// Token: 0x0600199F RID: 6559 RVA: 0x0007560F File Offset: 0x0007380F
	public void ReceiveDeathEvent(float? attackDirection, AttackTypes attackType, bool resetDeathEvent = false)
	{
		this.ReceiveDeathEvent(attackDirection, attackType, 1f, resetDeathEvent);
	}

	// Token: 0x060019A0 RID: 6560 RVA: 0x00075620 File Offset: 0x00073820
	public void ReceiveDeathEvent(float? attackDirection, AttackTypes attackType, float corpseFlingMultiplier, bool resetDeathEvent = false)
	{
		bool flag;
		GameObject gameObject;
		this.ReceiveDeathEvent(attackDirection, attackType, NailElements.None, null, corpseFlingMultiplier, resetDeathEvent, null, out flag, out gameObject);
	}

	// Token: 0x060019A1 RID: 6561 RVA: 0x00075640 File Offset: 0x00073840
	public void ReceiveDeathEvent(float? attackDirection, AttackTypes attackType, NailElements nailElement, GameObject damageSource, float corpseFlingMultiplier, bool resetDeathEvent, Action<Transform> onCorpseBegin, out bool didCallCorpseBegin, out GameObject corpseObj)
	{
		didCallCorpseBegin = false;
		corpseObj = null;
		if (this.didFire && !this.isCorpseRecyclable)
		{
			return;
		}
		this.didFire = true;
		this.RecordKillForJournal();
		if (corpseFlingMultiplier > 1.35f)
		{
			corpseFlingMultiplier = 1.35f;
		}
		if (attackType == AttackTypes.Lava)
		{
			this.ShakeCameraIfVisible();
			if (GlobalSettings.Corpse.EnemyLavaDeath)
			{
				GlobalSettings.Corpse.EnemyLavaDeath.Spawn().transform.SetPosition2D(base.transform.TransformPoint(this.effectOrigin));
			}
		}
		else
		{
			bool flag = attackType == AttackTypes.RuinsWater || attackType == AttackTypes.Acid;
			corpseObj = this.EmitCorpse(attackDirection, corpseFlingMultiplier, attackType, nailElement, damageSource, onCorpseBegin, out didCallCorpseBegin);
			if (!flag)
			{
				this.EmitEffects(corpseObj);
			}
		}
		GameManager instance = GameManager.instance;
		if (!string.IsNullOrEmpty(this.setPlayerDataBool))
		{
			instance.playerData.SetBool(this.setPlayerDataBool, true);
		}
		if (!string.IsNullOrWhiteSpace(this.awardAchievement))
		{
			instance.AwardAchievement(this.awardAchievement);
		}
		if (!this.doNotSetHasKilled)
		{
			instance.playerData.SetBool("hasKilled", true);
		}
		if (this.audioSnapshotOnDeath != null)
		{
			this.audioSnapshotOnDeath.TransitionTo(2f);
		}
		if (!string.IsNullOrEmpty(this.sendEventRegister))
		{
			EventRegister.SendEvent(this.sendEventRegister, null);
		}
		if (resetDeathEvent)
		{
			FSMUtility.SendEventToGameObject(base.gameObject, "CENTIPEDE DEATH", false);
			this.didFire = false;
			return;
		}
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (component)
		{
			component.SaveState();
		}
		if (this.recycle)
		{
			PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "health_manager_enemy");
			if (playMakerFSM != null)
			{
				playMakerFSM.FsmVariables.GetFsmBool("Activated").Value = false;
			}
			HealthManager component2 = base.GetComponent<HealthManager>();
			if (component2 != null)
			{
				component2.SetIsDead(false);
			}
			this.didFire = false;
			base.gameObject.Recycle();
			return;
		}
		Object.Destroy(base.gameObject);
	}

	// Token: 0x060019A2 RID: 6562 RVA: 0x00075820 File Offset: 0x00073A20
	public void RecordKillForJournal()
	{
		if (this.journalRecord == null)
		{
			return;
		}
		HealthManager component = base.GetComponent<HealthManager>();
		if (component == null || !component.WillAwardJournalKill)
		{
			return;
		}
		if (this.awardFullJournalEntry)
		{
			while (this.journalRecord.KillCount < this.journalRecord.KillsRequired - 1)
			{
				EnemyJournalManager.RecordKill(this.journalRecord, false);
			}
		}
		EnemyJournalManager.RecordKill(this.journalRecord, true);
	}

	// Token: 0x060019A3 RID: 6563 RVA: 0x00075894 File Offset: 0x00073A94
	protected GameObject EmitCorpse(float? attackDirection, float flingMultiplier, AttackTypes attackType, NailElements nailElement, GameObject damageSource, Action<Transform> onCorpseBegin, out bool didCallCorpseBegin)
	{
		didCallCorpseBegin = false;
		if (this.doNotSpawnCorpse)
		{
			return null;
		}
		bool flag = attackType == AttackTypes.RuinsWater || attackType == AttackTypes.Acid;
		float num = Random.Range(0.008f, 0.009f);
		Transform parent = base.GetComponentInParent<PersistentFolderReset>() ? base.transform.parent : null;
		GameObject gameObject;
		if (this.isCorpseRecyclable)
		{
			Transform transform = base.transform;
			Vector3 position = transform.TransformPoint(this.corpseSpawnPoint);
			position.z = num;
			gameObject = ObjectPool.Spawn(this.corpsePrefab, parent, position, Quaternion.identity, true);
			if (this.disabledSpecialQuestDrops)
			{
				this.RemoveQuestDropsFromGameObject(gameObject);
			}
			Vector3 localScale = transform.localScale;
			Vector3 localScale2 = gameObject.transform.localScale;
			localScale2.x = Mathf.Abs(localScale2.x) * Mathf.Sign(localScale.x);
			localScale2.y = Mathf.Abs(localScale2.y) * Mathf.Sign(localScale.y);
			localScale2.z = Mathf.Abs(localScale2.z) * Mathf.Sign(localScale.z);
			gameObject.transform.localScale = localScale2;
			DropRecycle.AddInactive(gameObject);
		}
		else
		{
			GameObject instantiatedCorpse = this.GetInstantiatedCorpse(attackType);
			if (!instantiatedCorpse)
			{
				return null;
			}
			instantiatedCorpse.transform.SetParent(parent);
			instantiatedCorpse.transform.SetPositionZ(num);
			instantiatedCorpse.SetActive(true);
			gameObject = instantiatedCorpse;
		}
		if (!gameObject)
		{
			return null;
		}
		if (this.GetBlackThreadAmount() > 0f)
		{
			BlackThreadState.IBlackThreadStateReceiver[] blackThreadEffects = gameObject.GetComponents<BlackThreadState.IBlackThreadStateReceiver>();
			if (blackThreadEffects != null && blackThreadEffects.Length > 0)
			{
				BlackThreadState.IBlackThreadStateReceiver[] blackThreadEffects3 = blackThreadEffects;
				for (int i = 0; i < blackThreadEffects3.Length; i++)
				{
					blackThreadEffects3[i].SetIsBlackThreaded(true);
				}
				if (this.IsCorpseRecyclable)
				{
					RecycleResetHandler.Add(gameObject, delegate()
					{
						if (blackThreadEffects != null && blackThreadEffects.Length > 0)
						{
							foreach (BlackThreadState.IBlackThreadStateReceiver blackThreadStateReceiver in blackThreadEffects)
							{
								if (blackThreadStateReceiver != null)
								{
									blackThreadStateReceiver.SetIsBlackThreaded(false);
								}
							}
						}
					});
				}
			}
		}
		Action<GameObject> corpseEmitted = this.CorpseEmitted;
		if (corpseEmitted != null)
		{
			corpseEmitted(gameObject);
		}
		CorpseItems component = gameObject.GetComponent<CorpseItems>();
		ActiveCorpse component2 = gameObject.GetComponent<ActiveCorpse>();
		HealthManager component3 = base.GetComponent<HealthManager>();
		if (component3)
		{
			if (component3.GetLastAttackType() == AttackTypes.Explosion)
			{
				FSMUtility.SendEventToGameObject(gameObject, "EXPLOSION DEATH", false);
			}
			if (component3.HasClearedItemDrops && component)
			{
				component.ClearPickupItems();
			}
		}
		tk2dSprite component4 = base.GetComponent<tk2dSprite>();
		PlayMakerFSM[] components = gameObject.GetComponents<PlayMakerFSM>();
		for (int i = 0; i < components.Length; i++)
		{
			foreach (FsmString fsmString in components[i].FsmVariables.StringVariables)
			{
				if (fsmString.Name == "Owner Name")
				{
					fsmString.Value = base.gameObject.name;
				}
			}
		}
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(gameObject, "corpse");
		if (playMakerFSM != null)
		{
			FsmBool fsmBool = playMakerFSM.FsmVariables.GetFsmBool("spellBurn");
			if (fsmBool != null)
			{
				fsmBool.Value = false;
			}
		}
		global::Corpse component5 = gameObject.GetComponent<global::Corpse>();
		if (component5)
		{
			component5.Setup(this.OverrideBloodColor, onCorpseBegin, this.isCorpseRecyclable);
			didCallCorpseBegin = true;
		}
		TagDamageTaker component6 = base.GetComponent<TagDamageTaker>();
		bool flag2 = false;
		if (component2)
		{
			component2.QueueBurnEffects(component4, attackType, nailElement, damageSource, 1f, component6);
			flag2 = true;
		}
		else
		{
			CorpseRegular corpseRegular = component5 as CorpseRegular;
			if (corpseRegular != null)
			{
				if (attackType == AttackTypes.Fire || attackType == AttackTypes.Explosion || nailElement == NailElements.Fire)
				{
					corpseRegular.SpawnElementalEffects(ElementalEffectType.Fire);
				}
				else if (attackType == AttackTypes.Lightning)
				{
					corpseRegular.SpawnElementalEffects(ElementalEffectType.Lightning);
				}
				else if (damageSource != null)
				{
					DamageEnemies component7 = damageSource.GetComponent<DamageEnemies>();
					if (component7)
					{
						ToolItem representingTool = component7.RepresentingTool;
						if (component7.ZapDamageTicks > 0)
						{
							corpseRegular.SpawnElementalEffects(ElementalEffectType.Lightning);
						}
					}
				}
			}
			ActiveCorpse[] componentsInChildren = gameObject.GetComponentsInChildren<ActiveCorpse>(true);
			float[] array = new float[componentsInChildren.Length];
			float num2 = 0f;
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				Collider2D component8 = componentsInChildren[k].GetComponent<Collider2D>();
				if (component8)
				{
					float magnitude = component8.bounds.size.magnitude;
					array[k] = magnitude;
					num2 += magnitude;
				}
				else
				{
					array[k] = 0f;
				}
			}
			for (int l = 0; l < componentsInChildren.Length; l++)
			{
				ActiveCorpse activeCorpse = componentsInChildren[l];
				float scale = array[l] / num2;
				activeCorpse.QueueBurnEffects(component4, attackType, nailElement, damageSource, scale, component6);
				flag2 = true;
			}
		}
		if (!flag2 && component6 != null)
		{
			foreach (DamageTag damageTag in component6.TaggedDamage.Keys)
			{
				if (damageTag.CorpseBurnEffect)
				{
					damageTag.SpawnDeathEffects(gameObject.transform.position);
					break;
				}
			}
		}
		if (flag)
		{
			return gameObject;
		}
		Rigidbody2D component9 = gameObject.GetComponent<Rigidbody2D>();
		float rotation = this.rotateCorpse ? base.transform.GetLocalRotation2D() : 0f;
		gameObject.transform.SetRotation2D(rotation);
		if (component9)
		{
			component9.rotation = rotation;
		}
		if (this.corpseMatchesEnemyScale)
		{
			gameObject.transform.localScale = base.transform.localScale;
			return gameObject;
		}
		if (Mathf.Abs(base.transform.eulerAngles.z) >= 45f)
		{
			Collider2D component10 = base.GetComponent<Collider2D>();
			Collider2D component11 = gameObject.GetComponent<Collider2D>();
			if (!this.rotateCorpse && component10 && component11)
			{
				Vector3 b = component10.bounds.center - component11.bounds.center;
				b.z = 0f;
				gameObject.transform.position += b;
			}
		}
		float num3 = 1f;
		if (attackDirection == null)
		{
			flingMultiplier = 0f;
			num3 = Mathf.Sign(base.transform.lossyScale.x);
		}
		int num4 = DirectionUtils.GetCardinalDirection(attackDirection.GetValueOrDefault());
		float num5 = gameObject.transform.localScale.x * (this.corpseFacesRight ? 1f : -1f) * num3;
		if (this.overrideDeathDirection)
		{
			num4 = ((num5 < 0f) ? 0 : 2);
		}
		if (component9 == null)
		{
			return gameObject;
		}
		float num6 = this.corpseFlingSpeed;
		float num7 = 60f;
		float num8 = 120f;
		if (flingMultiplier > 1.25f)
		{
			num7 = 45f;
			num8 = 135f;
		}
		float num9;
		switch (num4)
		{
		case 0:
			num9 = (this.lowCorpseArc ? 10f : num7);
			gameObject.transform.SetScaleX(-num5 * Mathf.Sign(base.transform.localScale.x));
			break;
		case 1:
			num9 = Random.Range(75f, 105f);
			num6 *= 1.3f;
			break;
		case 2:
			num9 = (this.lowCorpseArc ? 170f : num8);
			gameObject.transform.SetScaleX(num5 * Mathf.Sign(base.transform.localScale.x));
			break;
		case 3:
			num9 = 270f;
			break;
		default:
			num9 = 90f;
			break;
		}
		if (flingMultiplier < 0.5f && Math.Abs(flingMultiplier) > Mathf.Epsilon)
		{
			flingMultiplier = 0.5f;
		}
		if (flingMultiplier > 1.5f)
		{
			flingMultiplier = 1.5f;
		}
		component9.linearVelocity = new Vector2(Mathf.Cos(num9 * 0.017453292f), Mathf.Sin(num9 * 0.017453292f)) * (num6 * flingMultiplier);
		return gameObject;
	}

	// Token: 0x060019A4 RID: 6564 RVA: 0x00076030 File Offset: 0x00074230
	protected virtual void EmitEffects(GameObject corpseObj)
	{
		Debug.Log("EnemyDeathEffects EmitEffects not overidden!", this);
	}

	// Token: 0x060019A5 RID: 6565 RVA: 0x0007603D File Offset: 0x0007423D
	public void EmitSound()
	{
	}

	// Token: 0x060019A6 RID: 6566 RVA: 0x0007603F File Offset: 0x0007423F
	protected void ShakeCameraIfVisible(string eventName)
	{
		if (this.IsVisible)
		{
			GameCameras.instance.cameraShakeFSM.SendEvent(eventName);
		}
	}

	// Token: 0x060019A7 RID: 6567 RVA: 0x00076059 File Offset: 0x00074259
	protected void ShakeCameraIfVisible()
	{
		if (this.IsVisible)
		{
			this.deathCameraShake.DoShake(this, !this.SkipKillFreeze);
		}
	}

	// Token: 0x060019A8 RID: 6568 RVA: 0x00076078 File Offset: 0x00074278
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.corpseSpawnPoint, 0.25f);
		Gizmos.DrawWireSphere(this.effectOrigin, 0.25f);
	}

	// Token: 0x060019A9 RID: 6569 RVA: 0x000760AA File Offset: 0x000742AA
	public float GetBlackThreadAmount()
	{
		return (float)(this.isBlackThreaded ? 1 : 0);
	}

	// Token: 0x060019AA RID: 6570 RVA: 0x000760B9 File Offset: 0x000742B9
	public void SetIsBlackThreaded(bool isThreaded)
	{
		if (isThreaded)
		{
			this.isBlackThreaded = true;
			return;
		}
		this.isBlackThreaded = false;
	}

	// Token: 0x060019AC RID: 6572 RVA: 0x000760E0 File Offset: 0x000742E0
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0400188B RID: 6283
	[SerializeField]
	private GameObject corpsePrefab;

	// Token: 0x0400188C RID: 6284
	[SerializeField]
	private EnemyDeathEffects.AltCorpse[] altCorpses;

	// Token: 0x0400188D RID: 6285
	[SerializeField]
	private bool isCorpseRecyclable;

	// Token: 0x0400188E RID: 6286
	[SerializeField]
	[ModifiableProperty]
	[Conditional("isCorpseRecyclable", false, false, false)]
	private bool manualPreInstantiate;

	// Token: 0x0400188F RID: 6287
	[SerializeField]
	private bool corpseFacesRight;

	// Token: 0x04001890 RID: 6288
	[SerializeField]
	private bool overrideDeathDirection;

	// Token: 0x04001891 RID: 6289
	[SerializeField]
	private float corpseFlingSpeed;

	// Token: 0x04001892 RID: 6290
	[SerializeField]
	public Vector3 corpseSpawnPoint;

	// Token: 0x04001893 RID: 6291
	[SerializeField]
	public Vector3 effectOrigin;

	// Token: 0x04001894 RID: 6292
	[SerializeField]
	private bool lowCorpseArc;

	// Token: 0x04001895 RID: 6293
	[SerializeField]
	private bool recycle;

	// Token: 0x04001896 RID: 6294
	[SerializeField]
	private bool rotateCorpse;

	// Token: 0x04001897 RID: 6295
	[SerializeField]
	private bool corpseMatchesEnemyScale;

	// Token: 0x04001898 RID: 6296
	[SerializeField]
	private AudioMixerSnapshot audioSnapshotOnDeath;

	// Token: 0x04001899 RID: 6297
	[SerializeField]
	[FormerlySerializedAs("deathBroadcastEvent")]
	private string sendEventRegister;

	// Token: 0x0400189A RID: 6298
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private List<AudioEvent> deathSounds = new List<AudioEvent>();

	// Token: 0x0400189B RID: 6299
	[SerializeField]
	[HideInInspector]
	private AudioEvent enemyDeathSwordAudio;

	// Token: 0x0400189C RID: 6300
	[SerializeField]
	[HideInInspector]
	private AudioEvent enemyDamageAudio;

	// Token: 0x0400189D RID: 6301
	[SerializeField]
	private CameraShakeTarget deathCameraShake;

	// Token: 0x0400189E RID: 6302
	[SerializeField]
	private EnemyJournalRecord journalRecord;

	// Token: 0x0400189F RID: 6303
	[SerializeField]
	private bool awardFullJournalEntry;

	// Token: 0x040018A0 RID: 6304
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	public string setPlayerDataBool;

	// Token: 0x040018A1 RID: 6305
	[SerializeField]
	private string awardAchievement;

	// Token: 0x040018A2 RID: 6306
	public bool doNotSetHasKilled;

	// Token: 0x040018A3 RID: 6307
	public bool doNotSpawnCorpse;

	// Token: 0x040018A4 RID: 6308
	private bool didFire;

	// Token: 0x040018A5 RID: 6309
	private GameObject[] instantiatedCorpses;

	// Token: 0x040018A6 RID: 6310
	private bool preInstantiated;

	// Token: 0x040018A8 RID: 6312
	private bool hasAwaken;

	// Token: 0x040018A9 RID: 6313
	private bool hasStarted;

	// Token: 0x040018AA RID: 6314
	private bool disabledSpecialQuestDrops;

	// Token: 0x040018AB RID: 6315
	private bool isBlackThreaded;

	// Token: 0x020015B3 RID: 5555
	[Serializable]
	private struct AltCorpse
	{
		// Token: 0x04008847 RID: 34887
		[EnumPickerBitmask(typeof(AttackTypes))]
		public int AttackTypeMask;

		// Token: 0x04008848 RID: 34888
		public GameObject Prefab;
	}
}
