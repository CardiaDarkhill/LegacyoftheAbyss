using System;
using UnityEngine;

// Token: 0x02000291 RID: 657
public class AlertRange : TrackTriggerObjects
{
	// Token: 0x1700025F RID: 607
	// (get) Token: 0x060016FA RID: 5882 RVA: 0x00067E68 File Offset: 0x00066068
	public bool ChecksLineOfSight
	{
		get
		{
			return this.lineOfSight > AlertRange.LineOfSightChecks.None;
		}
	}

	// Token: 0x060016FB RID: 5883 RVA: 0x00067E73 File Offset: 0x00066073
	private void OnValidate()
	{
		if (this.checkParentSight)
		{
			this.lineOfSight = AlertRange.LineOfSightChecks.Parent;
			this.checkParentSight = false;
		}
	}

	// Token: 0x060016FC RID: 5884 RVA: 0x00067E8C File Offset: 0x0006608C
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
		this.hero = HeroController.instance;
		this.hasHero = (this.hero != null);
		this.initialParent = base.transform.parent;
		HealthManager componentInParent = base.GetComponentInParent<HealthManager>();
		if (componentInParent)
		{
			componentInParent.TookDamage += delegate()
			{
				this.unalertTimer = 0f;
			};
		}
	}

	// Token: 0x060016FD RID: 5885 RVA: 0x00067EF4 File Offset: 0x000660F4
	protected override void OnEnable()
	{
		base.OnEnable();
		this.hero = HeroController.instance;
		this.hasHero = (this.hero != null);
	}

	// Token: 0x060016FE RID: 5886 RVA: 0x00067F19 File Offset: 0x00066119
	protected override void OnDisable()
	{
		base.OnDisable();
		this.haveLineOfSight = false;
		this.isHeroInRange = false;
	}

	// Token: 0x060016FF RID: 5887 RVA: 0x00067F2F File Offset: 0x0006612F
	protected override void OnInsideStateChanged(bool isInside)
	{
		this.isHeroInRange = isInside;
	}

	// Token: 0x06001700 RID: 5888 RVA: 0x00067F38 File Offset: 0x00066138
	private void Update()
	{
		if (this.countUnalertTime)
		{
			if (this.isHeroInRange && (this.haveLineOfSight || this.lineOfSight == AlertRange.LineOfSightChecks.None))
			{
				this.unalertTimer = 0f;
				return;
			}
			this.unalertTimer += Time.deltaTime;
			if (this.unalertTimer > 100f)
			{
				this.unalertTimer = 100f;
			}
		}
	}

	// Token: 0x06001701 RID: 5889 RVA: 0x00067F9B File Offset: 0x0006619B
	private void FixedUpdate()
	{
		if (this.isHeroInRange)
		{
			this.UpdateLineOfSight();
		}
	}

	// Token: 0x06001702 RID: 5890 RVA: 0x00067FAC File Offset: 0x000661AC
	private void UpdateLineOfSight()
	{
		if (!this.ChecksLineOfSight)
		{
			return;
		}
		if (!this.hasHero)
		{
			this.haveLineOfSight = false;
			return;
		}
		AlertRange.LineOfSightChecks lineOfSightChecks = this.lineOfSight;
		Transform transform;
		if (lineOfSightChecks != AlertRange.LineOfSightChecks.Self)
		{
			if (lineOfSightChecks != AlertRange.LineOfSightChecks.Parent)
			{
				throw new NotImplementedException();
			}
			Transform parent = base.transform.parent;
			transform = (parent ? parent : this.initialParent);
		}
		else
		{
			transform = base.transform;
		}
		if (!transform)
		{
			this.haveLineOfSight = false;
			return;
		}
		Vector2 start = transform.position;
		Vector2 end = this.hero.transform.position;
		RaycastHit2D raycastHit2D;
		this.haveLineOfSight = !Helper.LineCast2DHit(start, end, 256, out raycastHit2D);
	}

	// Token: 0x06001703 RID: 5891 RVA: 0x00068060 File Offset: 0x00066260
	public bool IsHeroInRange()
	{
		return (!this.ChecksLineOfSight || this.haveLineOfSight) && this.isHeroInRange;
	}

	// Token: 0x06001704 RID: 5892 RVA: 0x00068089 File Offset: 0x00066289
	public float GetUnalertTime()
	{
		return this.unalertTimer;
	}

	// Token: 0x06001705 RID: 5893 RVA: 0x00068094 File Offset: 0x00066294
	public static AlertRange Find(GameObject root, string childName)
	{
		if (root == null)
		{
			return null;
		}
		bool flag = !string.IsNullOrEmpty(childName);
		Transform transform = root.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			AlertRange component = child.GetComponent<AlertRange>();
			if (!(component == null) && (!flag || !(child.gameObject.name != childName)))
			{
				return component;
			}
		}
		return null;
	}

	// Token: 0x0400158B RID: 5515
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool checkParentSight;

	// Token: 0x0400158C RID: 5516
	[Space]
	[SerializeField]
	private AlertRange.LineOfSightChecks lineOfSight;

	// Token: 0x0400158D RID: 5517
	[Space]
	[SerializeField]
	private bool countUnalertTime;

	// Token: 0x0400158E RID: 5518
	private float unalertTimer;

	// Token: 0x0400158F RID: 5519
	private Transform initialParent;

	// Token: 0x04001590 RID: 5520
	private bool isHeroInRange;

	// Token: 0x04001591 RID: 5521
	private bool haveLineOfSight;

	// Token: 0x04001592 RID: 5522
	private HeroController hero;

	// Token: 0x04001593 RID: 5523
	private bool hasHero;

	// Token: 0x02001561 RID: 5473
	private enum LineOfSightChecks
	{
		// Token: 0x040086F0 RID: 34544
		None,
		// Token: 0x040086F1 RID: 34545
		Self,
		// Token: 0x040086F2 RID: 34546
		Parent
	}
}
