using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020003A4 RID: 932
[ActionCategory("Hollow Knight")]
public class ShowBossChallengeUI : FsmStateAction
{
	// Token: 0x06001F66 RID: 8038 RVA: 0x0008F6D5 File Offset: 0x0008D8D5
	public override void Reset()
	{
		this.prefab = null;
		this.bossNameSheet = null;
		this.bossNameKey = null;
		this.descriptionSheet = null;
		this.descriptionKey = null;
		this.levelSelectedEvent = null;
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x0008F704 File Offset: 0x0008D904
	public override void OnEnter()
	{
		if (ShowBossChallengeUI.spawnedUI == null && this.prefab.Value)
		{
			ShowBossChallengeUI.spawnedUI = Object.Instantiate<GameObject>(this.prefab.Value);
			ShowBossChallengeUI.spawnedUI.SetActive(false);
		}
		if (ShowBossChallengeUI.spawnedUI)
		{
			GameObject gameObject = ShowBossChallengeUI.spawnedUI;
			gameObject.transform.position = this.prefab.Value.transform.position;
			gameObject.SetActive(true);
			BossChallengeUI ui = gameObject.GetComponent<BossChallengeUI>();
			if (ui)
			{
				BossStatue componentInParent = base.Owner.GetComponentInParent<BossStatue>();
				BossChallengeUI.HideEvent temp2 = null;
				temp2 = delegate()
				{
					this.Finish();
					ui.OnCancel -= temp2;
				};
				ui.OnCancel += temp2;
				BossChallengeUI.LevelSelectedEvent temp = null;
				temp = delegate()
				{
					this.Fsm.Event(this.levelSelectedEvent);
					ui.OnLevelSelected -= temp;
				};
				ui.OnLevelSelected += temp;
				ui.Setup(componentInParent, this.bossNameSheet.Value, this.bossNameKey.Value, this.descriptionSheet.Value, this.descriptionKey.Value);
				return;
			}
		}
		base.Finish();
	}

	// Token: 0x04001E51 RID: 7761
	private static GameObject spawnedUI;

	// Token: 0x04001E52 RID: 7762
	public FsmGameObject prefab;

	// Token: 0x04001E53 RID: 7763
	public FsmString bossNameSheet;

	// Token: 0x04001E54 RID: 7764
	public FsmString bossNameKey;

	// Token: 0x04001E55 RID: 7765
	public FsmString descriptionSheet;

	// Token: 0x04001E56 RID: 7766
	public FsmString descriptionKey;

	// Token: 0x04001E57 RID: 7767
	public FsmEvent levelSelectedEvent;
}
