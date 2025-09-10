using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000198 RID: 408
public class NailSlashRecoil : MonoBehaviour
{
	// Token: 0x06000FD0 RID: 4048 RVA: 0x0004C26B File Offset: 0x0004A46B
	private void Reset()
	{
		this.enemyDamager = base.GetComponent<DamageEnemies>();
	}

	// Token: 0x06000FD1 RID: 4049 RVA: 0x0004C279 File Offset: 0x0004A479
	private void Awake()
	{
		this.heroCtrl = base.GetComponentInParent<HeroController>();
		this.nailSlash = base.GetComponent<NailAttackBase>();
	}

	// Token: 0x06000FD2 RID: 4050 RVA: 0x0004C293 File Offset: 0x0004A493
	private void Start()
	{
		this.enemyDamager.HitResponded += this.HitResponse;
	}

	// Token: 0x06000FD3 RID: 4051 RVA: 0x0004C2AC File Offset: 0x0004A4AC
	public static void Add(GameObject gameObject, DamageEnemies enemyDamager, bool drillPull)
	{
		if (gameObject.GetComponent<NailSlashRecoil>())
		{
			return;
		}
		NailSlashRecoil nailSlashRecoil = gameObject.AddComponent<NailSlashRecoil>();
		nailSlashRecoil.enemyDamager = enemyDamager;
		nailSlashRecoil.drillPull = drillPull;
	}

	// Token: 0x06000FD4 RID: 4052 RVA: 0x0004C2D0 File Offset: 0x0004A4D0
	private void HitResponse(DamageEnemies.HitResponse response)
	{
		if (!this.heroCtrl)
		{
			return;
		}
		MonoBehaviour monoBehaviour = response.Responder as MonoBehaviour;
		if (monoBehaviour == null)
		{
			return;
		}
		float num = this.GetActualHitDirection(response.Target.transform, this.enemyDamager);
		if (num >= 360f)
		{
			num -= 360f;
		}
		int cardinalDirection = DirectionUtils.GetCardinalDirection(response.Hit.Direction);
		int cardinalDirection2 = DirectionUtils.GetCardinalDirection(this.enemyDamager.GetDirection());
		if (cardinalDirection != cardinalDirection2)
		{
			return;
		}
		if (!this.nailSlash)
		{
			this.heroCtrl.SetAllowRecoilWhileRelinquished(true);
		}
		GameObject gameObject = monoBehaviour.gameObject;
		int cardinalDirection3 = DirectionUtils.GetCardinalDirection(num);
		NonBouncer component = gameObject.GetComponent<NonBouncer>();
		switch (cardinalDirection3)
		{
		case 0:
		{
			PhysLayers layerOnHit = response.LayerOnHit;
			if (layerOnHit != PhysLayers.TERRAIN)
			{
				if (layerOnHit != PhysLayers.ENEMIES)
				{
					if (layerOnHit != PhysLayers.INTERACTIVE_OBJECT)
					{
						return;
					}
				}
				else
				{
					if (!(component == null) && component.active)
					{
						break;
					}
					if (gameObject.GetComponent<BounceShroom>() != null)
					{
						this.heroCtrl.RecoilLeftLong();
						NailSlashRecoil.Bounce(gameObject, false);
						return;
					}
					if (this.drillPull)
					{
						this.heroCtrl.DrillPull(false);
						return;
					}
					this.heroCtrl.RecoilLeft();
					return;
				}
			}
			if (gameObject.CompareTag("Recoiler"))
			{
				this.heroCtrl.RecoilLeft();
				return;
			}
			break;
		}
		case 1:
		{
			PhysLayers layerOnHit = response.LayerOnHit;
			if (layerOnHit != PhysLayers.TERRAIN)
			{
				if (layerOnHit != PhysLayers.ENEMIES)
				{
					if (layerOnHit != PhysLayers.INTERACTIVE_OBJECT)
					{
						return;
					}
				}
				else
				{
					if (!(component == null) && component.active)
					{
						break;
					}
					if (gameObject.GetComponent<BounceShroom>() != null)
					{
						this.heroCtrl.RecoilDown();
						NailSlashRecoil.Bounce(gameObject, false);
						return;
					}
					this.heroCtrl.RecoilDown();
					return;
				}
			}
			if (gameObject.CompareTag("Recoiler"))
			{
				this.heroCtrl.RecoilDown();
			}
			break;
		}
		case 2:
		{
			PhysLayers layerOnHit = response.LayerOnHit;
			if (layerOnHit != PhysLayers.TERRAIN)
			{
				if (layerOnHit != PhysLayers.ENEMIES)
				{
					if (layerOnHit != PhysLayers.INTERACTIVE_OBJECT)
					{
						return;
					}
				}
				else
				{
					if (!(component == null) && component.active)
					{
						break;
					}
					if (gameObject.GetComponent<BounceShroom>() != null)
					{
						this.heroCtrl.RecoilRightLong();
						NailSlashRecoil.Bounce(gameObject, false);
						return;
					}
					if (this.drillPull)
					{
						this.heroCtrl.DrillPull(true);
						return;
					}
					this.heroCtrl.RecoilRight();
					return;
				}
			}
			if (gameObject.CompareTag("Recoiler"))
			{
				this.heroCtrl.RecoilRight();
				return;
			}
			break;
		}
		case 3:
			break;
		default:
			return;
		}
	}

	// Token: 0x06000FD5 RID: 4053 RVA: 0x0004C524 File Offset: 0x0004A724
	private static void Bounce(GameObject obj, bool useEffects)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(obj, "Bounce Shroom");
		if (playMakerFSM)
		{
			playMakerFSM.SendEvent("BOUNCE UPWARD");
			return;
		}
		BounceShroom component = obj.GetComponent<BounceShroom>();
		if (component)
		{
			component.BounceLarge(useEffects);
		}
	}

	// Token: 0x06000FD6 RID: 4054 RVA: 0x0004C568 File Offset: 0x0004A768
	private float GetActualHitDirection(Transform target, DamageEnemies damager)
	{
		if (!damager)
		{
			return 0f;
		}
		if (!damager.CircleDirection)
		{
			return damager.GetDirection();
		}
		Vector2 vector = target.position - damager.transform.position;
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x04000F65 RID: 3941
	[SerializeField]
	private DamageEnemies enemyDamager;

	// Token: 0x04000F66 RID: 3942
	[SerializeField]
	private bool drillPull;

	// Token: 0x04000F67 RID: 3943
	private NailAttackBase nailSlash;

	// Token: 0x04000F68 RID: 3944
	private HeroController heroCtrl;
}
