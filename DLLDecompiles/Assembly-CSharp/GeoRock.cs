using System;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020004EA RID: 1258
public class GeoRock : MonoBehaviour, IBreakerBreakable
{
	// Token: 0x06002D19 RID: 11545 RVA: 0x000C4F14 File Offset: 0x000C3114
	private void Awake()
	{
		this.fsm = base.GetComponent<PlayMakerFSM>();
		this.hitsInt = this.fsm.FsmVariables.GetFsmInt("Hits");
		this.attackDirFloat = this.fsm.FsmVariables.GetFsmFloat("Attack Direction");
	}

	// Token: 0x06002D1A RID: 11546 RVA: 0x000C4F63 File Offset: 0x000C3163
	private void OnEnable()
	{
		SceneManager.activeSceneChanged += this.LevelActivated;
		this.gm = GameManager.instance;
		this.gm.SavePersistentObjects += this.SaveState;
	}

	// Token: 0x06002D1B RID: 11547 RVA: 0x000C4F98 File Offset: 0x000C3198
	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= this.LevelActivated;
		if (this.gm != null)
		{
			this.gm.SavePersistentObjects -= this.SaveState;
		}
	}

	// Token: 0x06002D1C RID: 11548 RVA: 0x000C4FD0 File Offset: 0x000C31D0
	private void Start()
	{
		this.SetMyId();
	}

	// Token: 0x06002D1D RID: 11549 RVA: 0x000C4FD8 File Offset: 0x000C31D8
	private void LevelActivated(Scene sceneFrom, Scene sceneTo)
	{
		this.SetMyId();
		GeoRockData geoRockData = SceneData.instance.FindMyState(this.geoRockData);
		if (geoRockData != null)
		{
			this.geoRockData.hitsLeft = geoRockData.hitsLeft;
			this.hitsInt.Value = geoRockData.hitsLeft;
			return;
		}
		this.UpdateHitsLeftFromFsm();
	}

	// Token: 0x06002D1E RID: 11550 RVA: 0x000C5028 File Offset: 0x000C3228
	private void SaveState()
	{
		this.SetMyId();
		this.UpdateHitsLeftFromFsm();
		SceneData.instance.SaveMyState(this.geoRockData);
	}

	// Token: 0x06002D1F RID: 11551 RVA: 0x000C5048 File Offset: 0x000C3248
	private void SetMyId()
	{
		if (string.IsNullOrEmpty(this.geoRockData.id))
		{
			this.geoRockData.id = base.name;
		}
		if (string.IsNullOrEmpty(this.geoRockData.sceneName))
		{
			this.geoRockData.sceneName = GameManager.GetBaseSceneName(base.gameObject.scene.name);
		}
	}

	// Token: 0x06002D20 RID: 11552 RVA: 0x000C50AD File Offset: 0x000C32AD
	private void UpdateHitsLeftFromFsm()
	{
		this.geoRockData.hitsLeft = this.hitsInt.Value;
	}

	// Token: 0x17000526 RID: 1318
	// (get) Token: 0x06002D21 RID: 11553 RVA: 0x000C50C5 File Offset: 0x000C32C5
	public BreakableBreaker.BreakableTypes BreakableType
	{
		get
		{
			return BreakableBreaker.BreakableTypes.Basic;
		}
	}

	// Token: 0x06002D22 RID: 11554 RVA: 0x000C50C8 File Offset: 0x000C32C8
	public void BreakFromBreaker(BreakableBreaker breaker)
	{
		for (int i = this.hitsInt.Value; i > 1; i--)
		{
			this.fsm.SendEvent("HIT SKIP EFFECTS");
		}
		this.HitFromBreaker(breaker);
	}

	// Token: 0x06002D23 RID: 11555 RVA: 0x000C5104 File Offset: 0x000C3304
	public void HitFromBreaker(BreakableBreaker breaker)
	{
		this.attackDirFloat.Value = (float)((breaker.transform.position.x > base.transform.position.x) ? 180 : 0);
		this.fsm.SendEvent("TAKE DAMAGE");
	}

	// Token: 0x06002D25 RID: 11557 RVA: 0x000C515F File Offset: 0x000C335F
	GameObject IBreakerBreakable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04002EBC RID: 11964
	[SerializeField]
	public GeoRockData geoRockData;

	// Token: 0x04002EBD RID: 11965
	private GameManager gm;

	// Token: 0x04002EBE RID: 11966
	private PlayMakerFSM fsm;

	// Token: 0x04002EBF RID: 11967
	private FsmInt hitsInt;

	// Token: 0x04002EC0 RID: 11968
	private FsmFloat attackDirFloat;
}
