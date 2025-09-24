using System;
using System.Collections.Generic;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020004D9 RID: 1241
public class EnviroRegion : DebugDrawColliderRuntimeAdder, ISceneLintUpgrader
{
	// Token: 0x17000518 RID: 1304
	// (get) Token: 0x06002C95 RID: 11413 RVA: 0x000C3335 File Offset: 0x000C1535
	// (set) Token: 0x06002C96 RID: 11414 RVA: 0x000C333D File Offset: 0x000C153D
	public EnvironmentTypes EnvironmentType
	{
		get
		{
			return this.environmentType;
		}
		set
		{
			this.environmentType = value;
		}
	}

	// Token: 0x17000519 RID: 1305
	// (get) Token: 0x06002C97 RID: 11415 RVA: 0x000C3346 File Offset: 0x000C1546
	// (set) Token: 0x06002C98 RID: 11416 RVA: 0x000C334E File Offset: 0x000C154E
	public int Priority
	{
		get
		{
			return this.priority;
		}
		set
		{
			this.priority = value;
		}
	}

	// Token: 0x06002C99 RID: 11417 RVA: 0x000C3358 File Offset: 0x000C1558
	protected override void Awake()
	{
		base.Awake();
		this.OnSceneLintUpgrade(true);
		if (base.gameObject.layer != 21)
		{
			Collider2D component = base.GetComponent<Collider2D>();
			Collider2D collider2D = component;
			collider2D.includeLayers |= 524288;
			if (component.layerOverridePriority <= 0)
			{
				component.layerOverridePriority = 1;
			}
		}
	}

	// Token: 0x06002C9A RID: 11418 RVA: 0x000C33B8 File Offset: 0x000C15B8
	private void OnDisable()
	{
		if (!GameManager.UnsafeInstance)
		{
			return;
		}
		foreach (EnviroRegionListener enviroRegionListener in this.insideListeners)
		{
			enviroRegionListener.RemoveInside(this);
		}
		this.insideListeners.Clear();
	}

	// Token: 0x06002C9B RID: 11419 RVA: 0x000C3424 File Offset: 0x000C1624
	private void OnTriggerEnter2D(Collider2D other)
	{
		EnviroRegionListener component = other.GetComponent<EnviroRegionListener>();
		if (!component)
		{
			return;
		}
		component.AddInside(this);
		this.insideListeners.Add(component);
	}

	// Token: 0x06002C9C RID: 11420 RVA: 0x000C3458 File Offset: 0x000C1658
	private void OnTriggerExit2D(Collider2D other)
	{
		EnviroRegionListener component = other.GetComponent<EnviroRegionListener>();
		if (!component)
		{
			return;
		}
		component.RemoveInside(this);
		this.insideListeners.Remove(component);
	}

	// Token: 0x06002C9D RID: 11421 RVA: 0x000C348C File Offset: 0x000C168C
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Enviro Region");
		if (!playMakerFSM)
		{
			return null;
		}
		if (!doUpgrade)
		{
			return "Enviro Region FSM needs upgrading to EnviroRegion script";
		}
		FsmInt fsmInt = playMakerFSM.FsmVariables.FindFsmInt("Enviro Type");
		this.environmentType = (EnvironmentTypes)fsmInt.Value;
		Object.DestroyImmediate(playMakerFSM);
		return "Enviro Region FSM was upgraded to EnviroRegion script";
	}

	// Token: 0x06002C9E RID: 11422 RVA: 0x000C34E5 File Offset: 0x000C16E5
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.Region, false);
	}

	// Token: 0x04002E39 RID: 11833
	[SerializeField]
	private EnvironmentTypes environmentType;

	// Token: 0x04002E3A RID: 11834
	[SerializeField]
	private int priority;

	// Token: 0x04002E3B RID: 11835
	private readonly HashSet<EnviroRegionListener> insideListeners = new HashSet<EnviroRegionListener>();
}
