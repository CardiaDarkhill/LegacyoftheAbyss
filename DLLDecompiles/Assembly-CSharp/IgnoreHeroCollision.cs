using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200050C RID: 1292
[RequireComponent(typeof(Collider2D))]
public class IgnoreHeroCollision : MonoBehaviour
{
	// Token: 0x1700052E RID: 1326
	// (get) Token: 0x06002E24 RID: 11812 RVA: 0x000CA996 File Offset: 0x000C8B96
	private bool CanIgnore
	{
		get
		{
			return this.hc && (!this.onlyWhileFloating || this.hc.cState.floating);
		}
	}

	// Token: 0x06002E25 RID: 11813 RVA: 0x000CA9C4 File Offset: 0x000C8BC4
	private void Start()
	{
		this.hc = HeroController.instance;
		if (!this.hc)
		{
			return;
		}
		if (this.onlyWhileFloating)
		{
			this.ignoredColliders = new List<ValueTuple<Collider2D, Collider2D>>();
			EventRegister.GetRegisterGuaranteed(base.gameObject, "BROLLY START").ReceivedEvent += this.Ignore;
			EventRegister.GetRegisterGuaranteed(base.gameObject, "BROLLY END").ReceivedEvent += this.Restore;
			return;
		}
		if (this.hc.isHeroInPosition)
		{
			this.Ignore();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate(bool <p0>)
		{
			this.Ignore();
			this.hc.heroInPosition -= temp;
			temp = null;
		};
		this.hc.heroInPosition += temp;
	}

	// Token: 0x06002E26 RID: 11814 RVA: 0x000CAA8B File Offset: 0x000C8C8B
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!this.CanIgnore)
		{
			return;
		}
		if (collision.gameObject.CompareTag("Player"))
		{
			this.IgnoreCollision(collision.collider, collision.otherCollider);
		}
	}

	// Token: 0x06002E27 RID: 11815 RVA: 0x000CAABC File Offset: 0x000C8CBC
	private void Ignore()
	{
		if (!this.CanIgnore)
		{
			return;
		}
		Collider2D component = base.GetComponent<Collider2D>();
		foreach (Collider2D colB in HeroController.instance.GetComponents<Collider2D>())
		{
			this.IgnoreCollision(component, colB);
		}
	}

	// Token: 0x06002E28 RID: 11816 RVA: 0x000CAB00 File Offset: 0x000C8D00
	private void Restore()
	{
		foreach (ValueTuple<Collider2D, Collider2D> valueTuple in this.ignoredColliders)
		{
			Collider2D item = valueTuple.Item1;
			Collider2D item2 = valueTuple.Item2;
			Physics2D.IgnoreCollision(item, item2, false);
		}
		this.ignoredColliders.Clear();
	}

	// Token: 0x06002E29 RID: 11817 RVA: 0x000CAB6C File Offset: 0x000C8D6C
	private void IgnoreCollision(Collider2D colA, Collider2D colB)
	{
		Physics2D.IgnoreCollision(colA, colB);
		if (this.onlyWhileFloating)
		{
			this.ignoredColliders.Add(new ValueTuple<Collider2D, Collider2D>(colA, colB));
		}
	}

	// Token: 0x04003063 RID: 12387
	[SerializeField]
	private bool onlyWhileFloating;

	// Token: 0x04003064 RID: 12388
	private HeroController hc;

	// Token: 0x04003065 RID: 12389
	private List<ValueTuple<Collider2D, Collider2D>> ignoredColliders;
}
