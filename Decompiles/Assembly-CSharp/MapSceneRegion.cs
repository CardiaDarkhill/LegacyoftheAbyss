using System;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000DC RID: 220
public class MapSceneRegion : MonoBehaviour, ISceneLintUpgrader
{
	// Token: 0x060006EE RID: 1774 RVA: 0x00022D9F File Offset: 0x00020F9F
	protected void Awake()
	{
		this.OnSceneLintUpgrade(true);
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x00022DA9 File Offset: 0x00020FA9
	private void OnEnable()
	{
		this.trigger.OnTriggerEntered += this.OnTriggerEnterEvent;
		this.trigger.OnTriggerExited += this.OnTriggerExitEvent;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x00022DD9 File Offset: 0x00020FD9
	private void OnDisable()
	{
		this.trigger.OnTriggerEntered -= this.OnTriggerEnterEvent;
		this.trigger.OnTriggerExited -= this.OnTriggerExitEvent;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x00022E0C File Offset: 0x0002100C
	private void OnTriggerEnterEvent(Collider2D col, GameObject sender)
	{
		GameManager instance = GameManager.instance;
		instance.AddToScenesVisited(this.sceneName);
		if (this.overrideMapZone)
		{
			instance.gameMap.OverrideMapZoneFromScene(this.sceneName);
		}
		if (this.overrideBounds)
		{
			instance.gameMap.OverrideSceneName(this.sceneName);
		}
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x00022E60 File Offset: 0x00021060
	private void OnTriggerExitEvent(Collider2D col, GameObject sender)
	{
		if (this.overrideBounds)
		{
			GameManager silentInstance = GameManager.SilentInstance;
			if (silentInstance)
			{
				silentInstance.gameMap.ClearOverriddenSceneName(this.sceneName);
			}
		}
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00022E94 File Offset: 0x00021094
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Add Map Scene");
		if (!playMakerFSM)
		{
			return null;
		}
		if (string.IsNullOrWhiteSpace(this.sceneName))
		{
			FsmString fsmString = playMakerFSM.FsmVariables.FindFsmString("Scene Name");
			this.sceneName = fsmString.Value;
		}
		Object.DestroyImmediate(playMakerFSM);
		return "Map Scene Region FSM was upgraded to MapSceneRegion script.";
	}

	// Token: 0x040006D5 RID: 1749
	[SerializeField]
	private TriggerEnterEvent trigger;

	// Token: 0x040006D6 RID: 1750
	[SerializeField]
	private string sceneName;

	// Token: 0x040006D7 RID: 1751
	[FormerlySerializedAs("reportGameMapEntered")]
	[SerializeField]
	private bool overrideMapZone;

	// Token: 0x040006D8 RID: 1752
	[SerializeField]
	private bool overrideBounds;
}
