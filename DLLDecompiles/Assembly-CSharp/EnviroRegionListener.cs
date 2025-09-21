using System;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x020004DA RID: 1242
public class EnviroRegionListener : MonoBehaviour
{
	// Token: 0x1700051A RID: 1306
	// (get) Token: 0x06002CA0 RID: 11424 RVA: 0x000C3507 File Offset: 0x000C1707
	public bool IsSprinting
	{
		get
		{
			return !this.hc || this.hc.cState.isSprinting;
		}
	}

	// Token: 0x1700051B RID: 1307
	// (get) Token: 0x06002CA1 RID: 11425 RVA: 0x000C3528 File Offset: 0x000C1728
	public EnvironmentTypes CurrentEnvironmentType
	{
		get
		{
			if (!this.overrideEnvironment)
			{
				return this.currentEnvironmentType;
			}
			return this.overrideEnvironmentType;
		}
	}

	// Token: 0x1700051C RID: 1308
	// (get) Token: 0x06002CA2 RID: 11426 RVA: 0x000C353F File Offset: 0x000C173F
	public string CurrentEnvironmentTypeString
	{
		get
		{
			if (!this.overrideEnvironment)
			{
				return this.currentEnvironmentType.ToString();
			}
			return this.overrideEnvironmentType.ToString();
		}
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x000C356C File Offset: 0x000C176C
	private void Awake()
	{
		this.hc = base.GetComponent<HeroController>();
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x000C357A File Offset: 0x000C177A
	private void Start()
	{
		this.Refresh(false);
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x000C3583 File Offset: 0x000C1783
	public void AddInside(EnviroRegion region)
	{
		this.insideRegions.AddIfNotPresent(region);
		this.Refresh(false);
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x000C3599 File Offset: 0x000C1799
	public void RemoveInside(EnviroRegion region)
	{
		this.insideRegions.Remove(region);
		this.Refresh(false);
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x000C35B0 File Offset: 0x000C17B0
	public void Refresh(bool fixRecursion = false)
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		CustomSceneManager sm = silentInstance.sm;
		if (!sm)
		{
			silentInstance.GetSceneManager();
			sm = silentInstance.sm;
			if (!sm)
			{
				return;
			}
		}
		EnvironmentTypes environmentType = sm.environmentType;
		int num = int.MinValue;
		for (int i = this.insideRegions.Count - 1; i >= 0; i--)
		{
			EnviroRegion enviroRegion = this.insideRegions[i];
			if (enviroRegion.Priority > num)
			{
				num = enviroRegion.Priority;
				environmentType = enviroRegion.EnvironmentType;
			}
		}
		this.currentEnvironmentType = environmentType;
		if (this.hc)
		{
			PlayerData.instance.environmentType = environmentType;
			if (!fixRecursion)
			{
				this.hc.checkEnvironment();
			}
		}
		EventRegister.SendEvent(EventRegisterEvents.EnviroUpdate, null);
		Action<EnvironmentTypes> currentEnvironmentTypeChanged = this.CurrentEnvironmentTypeChanged;
		if (currentEnvironmentTypeChanged == null)
		{
			return;
		}
		currentEnvironmentTypeChanged(this.currentEnvironmentType);
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x000C3692 File Offset: 0x000C1892
	public void SetOverride(EnvironmentTypes type)
	{
		this.overrideEnvironment = true;
		this.overrideEnvironmentType = type;
		this.Refresh(false);
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x000C36A9 File Offset: 0x000C18A9
	public void ClearOverride()
	{
		this.overrideEnvironment = false;
		this.Refresh(false);
	}

	// Token: 0x06002CAA RID: 11434 RVA: 0x000C36B9 File Offset: 0x000C18B9
	public string GetCurrentEnvironmentTypeString()
	{
		this.Refresh(false);
		return this.CurrentEnvironmentTypeString;
	}

	// Token: 0x04002E3C RID: 11836
	public Action<EnvironmentTypes> CurrentEnvironmentTypeChanged;

	// Token: 0x04002E3D RID: 11837
	private readonly List<EnviroRegion> insideRegions = new List<EnviroRegion>();

	// Token: 0x04002E3E RID: 11838
	private HeroController hc;

	// Token: 0x04002E3F RID: 11839
	private bool overrideEnvironment;

	// Token: 0x04002E40 RID: 11840
	private EnvironmentTypes overrideEnvironmentType;

	// Token: 0x04002E41 RID: 11841
	private EnvironmentTypes currentEnvironmentType;
}
