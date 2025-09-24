using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020003BF RID: 959
public class HeroWispLantern : MonoBehaviour
{
	// Token: 0x06002043 RID: 8259 RVA: 0x000931AC File Offset: 0x000913AC
	private void Awake()
	{
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += this.OnEquipsChanged;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "HERO ENTERED SCENE").ReceivedEvent += this.OnEquipsChanged;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "CLEAR EFFECTS").ReceivedEvent += this.EffectsCleared;
		this.enemyRange.OnTriggerEntered += this.OnEnemyRangeEntered;
		this.enemyRange.OnTriggerExited += this.OnEnemyRangeExited;
	}

	// Token: 0x06002044 RID: 8260 RVA: 0x0009324A File Offset: 0x0009144A
	private void OnEnable()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
		this.OnEquipsChanged();
	}

	// Token: 0x06002045 RID: 8261 RVA: 0x00093263 File Offset: 0x00091463
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
		this.spawnRoutine = null;
	}

	// Token: 0x06002046 RID: 8262 RVA: 0x0009327D File Offset: 0x0009147D
	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		this.isPaused = true;
	}

	// Token: 0x06002047 RID: 8263 RVA: 0x00093288 File Offset: 0x00091488
	private void OnEquipsChanged()
	{
		this.isPaused = false;
		bool isEquipped = this.tool.IsEquipped;
		this.ToggleWispLantern(isEquipped);
	}

	// Token: 0x06002048 RID: 8264 RVA: 0x000932AF File Offset: 0x000914AF
	private void ToggleWispLantern(bool isEquipped)
	{
		this.ToggleLanternEffects(isEquipped);
		this.enemyRange.gameObject.SetActive(isEquipped);
		if (!isEquipped)
		{
			this.trackingEnemies.Clear();
		}
	}

	// Token: 0x06002049 RID: 8265 RVA: 0x000932D7 File Offset: 0x000914D7
	private void ToggleLanternEffects(bool isEquipped)
	{
		if (isEquipped)
		{
			this.idlePt.Play(true);
		}
		else
		{
			this.idlePt.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		this.haze.SetActive(isEquipped);
	}

	// Token: 0x0600204A RID: 8266 RVA: 0x00093303 File Offset: 0x00091503
	private void EffectsCleared()
	{
		this.effectsCleared = true;
		this.ToggleLanternEffects(false);
	}

	// Token: 0x0600204B RID: 8267 RVA: 0x00093314 File Offset: 0x00091514
	private void OnEnemyRangeEntered(Collider2D col, GameObject sender)
	{
		HealthManager item;
		if (!HeroWispLantern.TryGetHealthManager(col, out item))
		{
			return;
		}
		if (!this.trackingEnemies.Contains(item))
		{
			this.trackingEnemies.Add(item);
		}
		if (this.spawnRoutine == null)
		{
			this.spawnRoutine = base.StartCoroutine(this.SpawnWispsRoutine());
		}
	}

	// Token: 0x0600204C RID: 8268 RVA: 0x00093360 File Offset: 0x00091560
	private static bool TryGetHealthManager(Collider2D col, out HealthManager healthManager)
	{
		Rigidbody2D attachedRigidbody = col.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			healthManager = attachedRigidbody.GetComponent<HealthManager>();
			return healthManager != null;
		}
		healthManager = col.GetComponent<HealthManager>();
		return healthManager != null;
	}

	// Token: 0x0600204D RID: 8269 RVA: 0x000933A0 File Offset: 0x000915A0
	private void OnEnemyRangeExited(Collider2D col, GameObject sender)
	{
		HealthManager item;
		if (!HeroWispLantern.TryGetHealthManager(col, out item))
		{
			return;
		}
		this.trackingEnemies.Remove(item);
	}

	// Token: 0x0600204E RID: 8270 RVA: 0x000933C5 File Offset: 0x000915C5
	private IEnumerator SpawnWispsRoutine()
	{
		HeroController hc = HeroController.instance;
		while (this.isPaused)
		{
			yield return null;
		}
		yield return new WaitForSeconds(this.wispSpawnStartDelay.GetRandomValue());
		bool wasPaused = this.isPaused;
		while (this.trackingEnemies.Count > 0)
		{
			if (this.effectsCleared)
			{
				yield return null;
				if (!hc.controlReqlinquished)
				{
					this.ToggleLanternEffects(true);
					this.effectsCleared = false;
				}
			}
			else if (hc.playerData.silk <= 0)
			{
				yield return null;
			}
			else if (this.isPaused)
			{
				yield return null;
			}
			else
			{
				if (wasPaused)
				{
					yield return new WaitForSeconds(this.wispSpawnCooldown.GetRandomValue());
				}
				wasPaused = this.isPaused;
				Vector3 position = base.transform.position;
				HealthManager enemyInRange = this.GetEnemyInRange();
				if (enemyInRange)
				{
					RaycastHit2D raycastHit2D;
					if (global::Helper.LineCast2DHit(position, enemyInRange.TargetPoint, 256, out raycastHit2D))
					{
						yield return null;
					}
					else
					{
						PlayMakerFSM component = this.wispPrefab.Spawn(base.transform.position).GetComponent<PlayMakerFSM>();
						component.FsmVariables.FindFsmGameObject("Target").Value = enemyInRange.gameObject;
						component.FsmVariables.FindFsmGameObject("Spawner").Value = base.gameObject;
						hc.TakeSilk(1, SilkSpool.SilkTakeSource.Wisp);
						yield return new WaitForSeconds(this.wispSpawnCooldown.GetRandomValue());
					}
				}
				else
				{
					yield return null;
				}
			}
		}
		this.spawnRoutine = null;
		yield break;
	}

	// Token: 0x0600204F RID: 8271 RVA: 0x000933D4 File Offset: 0x000915D4
	public HealthManager GetEnemyInRange()
	{
		this.trackingEnemies.RemoveAll((HealthManager o) => o == null);
		if (this.trackingEnemies.Count == 0)
		{
			return null;
		}
		HealthManager healthManager = this.trackingEnemies[Random.Range(0, this.trackingEnemies.Count)];
		if (!healthManager)
		{
			return null;
		}
		return healthManager;
	}

	// Token: 0x04001F41 RID: 8001
	[SerializeField]
	private ToolItem tool;

	// Token: 0x04001F42 RID: 8002
	[Space]
	[SerializeField]
	private ParticleSystem idlePt;

	// Token: 0x04001F43 RID: 8003
	[SerializeField]
	private GameObject haze;

	// Token: 0x04001F44 RID: 8004
	[SerializeField]
	private TriggerEnterEvent enemyRange;

	// Token: 0x04001F45 RID: 8005
	[SerializeField]
	private GameObject wispPrefab;

	// Token: 0x04001F46 RID: 8006
	[Space]
	[SerializeField]
	private MinMaxFloat wispSpawnStartDelay;

	// Token: 0x04001F47 RID: 8007
	[SerializeField]
	private MinMaxFloat wispSpawnCooldown;

	// Token: 0x04001F48 RID: 8008
	private Coroutine spawnRoutine;

	// Token: 0x04001F49 RID: 8009
	private readonly List<HealthManager> trackingEnemies = new List<HealthManager>();

	// Token: 0x04001F4A RID: 8010
	private bool effectsCleared;

	// Token: 0x04001F4B RID: 8011
	private bool isPaused;
}
