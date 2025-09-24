using System;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class SilkAcidRegion : MonoBehaviour
{
	// Token: 0x1700009A RID: 154
	// (get) Token: 0x0600074C RID: 1868 RVA: 0x00023D70 File Offset: 0x00021F70
	public HeroSilkAcid.SizzleTypes SizzleType
	{
		get
		{
			return this.sizzleType;
		}
	}

	// Token: 0x1700009B RID: 155
	// (get) Token: 0x0600074D RID: 1869 RVA: 0x00023D78 File Offset: 0x00021F78
	public float SizzleStartDelay
	{
		get
		{
			return this.sizzleStartDelay;
		}
	}

	// Token: 0x1700009C RID: 156
	// (get) Token: 0x0600074E RID: 1870 RVA: 0x00023D80 File Offset: 0x00021F80
	public float SizzleTime
	{
		get
		{
			return this.sizzleTime;
		}
	}

	// Token: 0x1700009D RID: 157
	// (get) Token: 0x0600074F RID: 1871 RVA: 0x00023D88 File Offset: 0x00021F88
	public bool IsProtected
	{
		get
		{
			return this.protectCondition.IsDefined && this.protectCondition.IsFulfilled;
		}
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00023DA4 File Offset: 0x00021FA4
	private void OnEnable()
	{
		this.sizzledSilk = 0;
		if (base.transform.parent)
		{
			this.Entered(base.transform.parent.gameObject);
		}
		foreach (EventBase eventBase in this.dispelEvents)
		{
			if (eventBase)
			{
				eventBase.ReceivedEvent += this.Dispel;
			}
		}
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x00023E14 File Offset: 0x00022014
	private void OnDisable()
	{
		foreach (EventBase eventBase in this.dispelEvents)
		{
			if (eventBase)
			{
				eventBase.ReceivedEvent -= this.Dispel;
			}
		}
		this.Dispel();
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00023E5A File Offset: 0x0002205A
	private void OnTriggerEnter2D(Collider2D collision)
	{
		this.Entered(collision.gameObject);
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x00023E68 File Offset: 0x00022068
	private void Entered(GameObject obj)
	{
		if (this.hero)
		{
			return;
		}
		this.hero = obj.GetComponent<HeroSilkAcid>();
		if (this.hero)
		{
			this.hero.AddInside(this);
		}
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00023E9D File Offset: 0x0002209D
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!this.hero)
		{
			return;
		}
		if (collision.GetComponent<HeroSilkAcid>() == this.hero)
		{
			this.Dispel();
		}
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00023EC6 File Offset: 0x000220C6
	public void Dispel()
	{
		if (!this.hero)
		{
			return;
		}
		this.hero.RemoveInside(this);
		this.hero = null;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00023EE9 File Offset: 0x000220E9
	public void OnTakenSilk(bool isAnySilkLeft)
	{
		this.sizzledSilk++;
		if (this.dispelAfterSilk > 0 && (this.sizzledSilk >= this.dispelAfterSilk || !isAnySilkLeft))
		{
			this.Dispel();
		}
	}

	// Token: 0x04000714 RID: 1812
	[SerializeField]
	private HeroSilkAcid.SizzleTypes sizzleType;

	// Token: 0x04000715 RID: 1813
	[Space]
	[SerializeField]
	private float sizzleStartDelay;

	// Token: 0x04000716 RID: 1814
	[SerializeField]
	private float sizzleTime = 1f;

	// Token: 0x04000717 RID: 1815
	[Space]
	[SerializeField]
	private int dispelAfterSilk;

	// Token: 0x04000718 RID: 1816
	[SerializeField]
	private EventBase[] dispelEvents;

	// Token: 0x04000719 RID: 1817
	[SerializeField]
	private PlayerDataTest protectCondition;

	// Token: 0x0400071A RID: 1818
	private HeroSilkAcid hero;

	// Token: 0x0400071B RID: 1819
	private int sizzledSilk;
}
