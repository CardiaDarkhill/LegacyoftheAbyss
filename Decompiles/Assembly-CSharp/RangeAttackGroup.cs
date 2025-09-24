using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000537 RID: 1335
public sealed class RangeAttackGroup : MonoBehaviour
{
	// Token: 0x1700054C RID: 1356
	// (get) Token: 0x06002FF1 RID: 12273 RVA: 0x000D332C File Offset: 0x000D152C
	// (set) Token: 0x06002FF2 RID: 12274 RVA: 0x000D3334 File Offset: 0x000D1534
	public bool GroupEnabled
	{
		get
		{
			return this.groupEnabled;
		}
		set
		{
			this.groupEnabled = value;
			base.gameObject.SetActive(value);
			foreach (RangeAttacker rangeAttacker in this.rangeAttackers)
			{
				if (!(rangeAttacker == null))
				{
					rangeAttacker.gameObject.SetActive(value);
				}
			}
		}
	}

	// Token: 0x06002FF3 RID: 12275 RVA: 0x000D33A8 File Offset: 0x000D15A8
	private void Awake()
	{
		this.groupIDMask = 1 << RangeAttackGroup.nextID++;
		if (RangeAttackGroup.nextID >= 32)
		{
			RangeAttackGroup.nextID = 0;
		}
		this.rangeAttackers.RemoveAll((RangeAttacker o) => o == null);
		foreach (RangeAttacker rangeAttacker in this.rangeAttackers)
		{
			rangeAttacker.SetOrigin(this.appearOriginOffset);
			rangeAttacker.MarkChild();
		}
		if (this.customDamageTrigger != null)
		{
			this.customDamageTrigger.OnTriggerEntered += this.OnCustomDamageTriggerEntered;
		}
		if (this.explosionTrigger)
		{
			this.explosionTrigger.OnTriggerEntered += this.OnExplosionTriggerEntered;
		}
		this.hasChildren = (this.rangeAttackers.Count != 0);
		if (!(this.groupRange != null))
		{
			base.enabled = false;
			return;
		}
		this.groupRange.InsideStateChanged += this.OnInsideStateChanged;
		if (this.hasChildren)
		{
			base.enabled = this.groupRange.IsInside;
			return;
		}
		base.enabled = false;
	}

	// Token: 0x06002FF4 RID: 12276 RVA: 0x000D3504 File Offset: 0x000D1704
	private void OnDestroy()
	{
		if (this.groupRange != null)
		{
			this.groupRange.InsideStateChanged -= this.OnInsideStateChanged;
		}
		if (this.customDamageTrigger != null)
		{
			this.customDamageTrigger.OnTriggerEntered -= this.OnCustomDamageTriggerEntered;
		}
		if (this.explosionTrigger)
		{
			this.explosionTrigger.OnTriggerEntered -= this.OnExplosionTriggerEntered;
		}
	}

	// Token: 0x06002FF5 RID: 12277 RVA: 0x000D357F File Offset: 0x000D177F
	private void OnValidate()
	{
		this.dirty = true;
	}

	// Token: 0x06002FF6 RID: 12278 RVA: 0x000D3588 File Offset: 0x000D1788
	private void OnDisable()
	{
		for (int i = 0; i < this.rangeAttackers.Count; i++)
		{
			this.rangeAttackers[i].SetInsideState(this.groupIDMask, false);
		}
	}

	// Token: 0x06002FF7 RID: 12279 RVA: 0x000D35C4 File Offset: 0x000D17C4
	private void FixedUpdate()
	{
		if (this.groupRange.insideObjectsList.Count == 0)
		{
			return;
		}
		this.targetPositions.Clear();
		this.anyAttackerActive = false;
		for (int i = 0; i < this.groupRange.insideObjectsList.Count; i++)
		{
			GameObject gameObject = this.groupRange.insideObjectsList[i];
			this.targetPositions.Add(gameObject.transform.position);
		}
		float num = this.attackerAppearRadius * this.attackerAppearRadius;
		for (int j = 0; j < this.rangeAttackers.Count; j++)
		{
			RangeAttacker rangeAttacker = this.rangeAttackers[j];
			Vector2 b = rangeAttacker.GetOrigin();
			bool isInside = false;
			for (int k = 0; k < this.targetPositions.Count; k++)
			{
				if ((this.targetPositions[k] - b).sqrMagnitude <= num)
				{
					isInside = true;
					this.anyAttackerActive = true;
					break;
				}
			}
			rangeAttacker.SetInsideState(this.groupIDMask, isInside);
		}
	}

	// Token: 0x06002FF8 RID: 12280 RVA: 0x000D36E0 File Offset: 0x000D18E0
	[ContextMenu("Find Attackers In Region")]
	private void FindAttackersInRegion()
	{
		if (this.groupRange == null)
		{
			return;
		}
		Collider2D[] components = this.groupRange.GetComponents<Collider2D>();
		if (components.Length == 0)
		{
			return;
		}
		HashSet<RangeAttacker> hashSet = new HashSet<RangeAttacker>();
		foreach (RangeAttacker rangeAttacker in Object.FindObjectsByType<RangeAttacker>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			Vector2 point = rangeAttacker.transform.position;
			Collider2D[] array2 = components;
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].OverlapPoint(point))
				{
					hashSet.Add(rangeAttacker);
					break;
				}
			}
		}
		this.rangeAttackers.RemoveAll((RangeAttacker o) => o == null);
		this.rangeAttackers = this.rangeAttackers.Union(hashSet).ToList<RangeAttacker>();
	}

	// Token: 0x06002FF9 RID: 12281 RVA: 0x000D37B4 File Offset: 0x000D19B4
	private void SetAttackersAsChildren()
	{
		this.rangeAttackers.RemoveAll((RangeAttacker o) => o == null);
		foreach (RangeAttacker rangeAttacker in this.rangeAttackers)
		{
		}
	}

	// Token: 0x06002FFA RID: 12282 RVA: 0x000D382C File Offset: 0x000D1A2C
	private void CleanUpChildObjects()
	{
		foreach (RangeAttacker rangeAttacker in this.rangeAttackers)
		{
			rangeAttacker.CleanChild();
		}
	}

	// Token: 0x06002FFB RID: 12283 RVA: 0x000D387C File Offset: 0x000D1A7C
	[ContextMenu("Set Attackers as children and clean up")]
	private void SetAttackersAsChildrenAndClean()
	{
		this.SetAttackersAsChildren();
		this.CleanUpChildObjects();
	}

	// Token: 0x06002FFC RID: 12284 RVA: 0x000D388A File Offset: 0x000D1A8A
	private void OnInsideStateChanged(bool inside)
	{
		if (this.hasChildren)
		{
			base.enabled = inside;
		}
	}

	// Token: 0x06002FFD RID: 12285 RVA: 0x000D389C File Offset: 0x000D1A9C
	private void OnCustomDamageTriggerEntered(Collider2D collision, GameObject sender)
	{
		if (!this.anyAttackerActive)
		{
			return;
		}
		if (collision.gameObject.layer != 20)
		{
			return;
		}
		if (collision.gameObject.GetComponentInParent<HeroController>() && CheatManager.Invincibility == CheatManager.InvincibilityStates.FullInvincible)
		{
			return;
		}
		if (this.sinkTarget)
		{
			Vector2 b = base.transform.position;
			RangeAttacker.LastDamageSinkDirection = (this.sinkTarget.position - b).normalized;
		}
		EventRegister.SendEvent(this.customDamageEventRegister, null);
	}

	// Token: 0x06002FFE RID: 12286 RVA: 0x000D392C File Offset: 0x000D1B2C
	private bool CanDamagerShred(DamageEnemies otherDamager)
	{
		if (otherDamager.attackType == AttackTypes.Explosion || otherDamager.CompareTag("Explosion"))
		{
			return true;
		}
		ToolItem representingTool = otherDamager.RepresentingTool;
		return representingTool && (representingTool.DamageFlags & ToolDamageFlags.Shredding) != ToolDamageFlags.None;
	}

	// Token: 0x06002FFF RID: 12287 RVA: 0x000D3970 File Offset: 0x000D1B70
	private void OnExplosionTriggerEntered(Collider2D collision, GameObject sender)
	{
		DamageEnemies component = collision.GetComponent<DamageEnemies>();
		if (!component)
		{
			return;
		}
		if (!this.CanDamagerShred(component))
		{
			return;
		}
		CircleCollider2D circleCollider2D = collision as CircleCollider2D;
		float num;
		if (circleCollider2D != null)
		{
			num = circleCollider2D.radius * collision.transform.localScale.x;
		}
		else
		{
			num = (collision.bounds.extents.x + collision.bounds.extents.y) * 0.5f;
		}
		num += 2f;
		num *= num;
		Vector3 position = collision.transform.position;
		for (int i = 0; i < this.rangeAttackers.Count; i++)
		{
			RangeAttacker rangeAttacker = this.rangeAttackers[i];
			if (Vector3.SqrMagnitude(rangeAttacker.GetOrigin() - position) <= num)
			{
				rangeAttacker.ReactToExplosion();
			}
		}
	}

	// Token: 0x040032DB RID: 13019
	[SerializeField]
	private TrackTriggerObjects groupRange;

	// Token: 0x040032DC RID: 13020
	[SerializeField]
	private List<RangeAttacker> rangeAttackers = new List<RangeAttacker>();

	// Token: 0x040032DD RID: 13021
	[SerializeField]
	private float attackerAppearRadius = 3.7f;

	// Token: 0x040032DE RID: 13022
	[SerializeField]
	private Vector3 appearOriginOffset = new Vector3(0f, 0.5f, 0f);

	// Token: 0x040032DF RID: 13023
	[SerializeField]
	private TriggerEnterEvent explosionTrigger;

	// Token: 0x040032E0 RID: 13024
	[SerializeField]
	private TriggerEnterEvent customDamageTrigger;

	// Token: 0x040032E1 RID: 13025
	[SerializeField]
	private string customDamageEventRegister;

	// Token: 0x040032E2 RID: 13026
	[SerializeField]
	private Transform sinkTarget;

	// Token: 0x040032E3 RID: 13027
	private bool hasChildren;

	// Token: 0x040032E4 RID: 13028
	private bool anyAttackerActive;

	// Token: 0x040032E5 RID: 13029
	private List<Vector2> targetPositions = new List<Vector2>();

	// Token: 0x040032E6 RID: 13030
	private static int nextID;

	// Token: 0x040032E7 RID: 13031
	private int groupIDMask;

	// Token: 0x040032E8 RID: 13032
	private bool groupEnabled = true;

	// Token: 0x040032E9 RID: 13033
	private bool dirty;
}
