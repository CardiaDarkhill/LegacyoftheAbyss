using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020003A7 RID: 935
[ActionCategory("Hollow Knight")]
public class ShowBossDoorChallengeUI : FsmStateAction
{
	// Token: 0x06001F7E RID: 8062 RVA: 0x0008FE5F File Offset: 0x0008E05F
	public override void Reset()
	{
		this.targetDoor = null;
		this.prefab = null;
		this.challengeEvent = null;
		this.cancelEvent = null;
	}

	// Token: 0x06001F7F RID: 8063 RVA: 0x0008FE80 File Offset: 0x0008E080
	public override void OnEnter()
	{
		if (ShowBossDoorChallengeUI.spawnedUI == null && this.prefab.Value)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.prefab.Value);
			ShowBossDoorChallengeUI.spawnedUI = gameObject.GetComponent<BossDoorChallengeUI>();
			gameObject.SetActive(false);
		}
		if (ShowBossDoorChallengeUI.spawnedUI)
		{
			GameObject safe = this.targetDoor.GetSafe(this);
			BossSequenceDoor door = safe ? safe.GetComponent<BossSequenceDoor>() : null;
			ShowBossDoorChallengeUI.spawnedUI.Setup(door);
			ShowBossDoorChallengeUI.spawnedUI.Show();
			BossDoorChallengeUI.HideEvent temp2 = null;
			temp2 = delegate()
			{
				this.Fsm.Event(this.cancelEvent);
				ShowBossDoorChallengeUI.spawnedUI.OnHidden -= temp2;
			};
			ShowBossDoorChallengeUI.spawnedUI.OnHidden += temp2;
			BossDoorChallengeUI.BeginEvent temp = null;
			temp = delegate()
			{
				this.Fsm.Event(this.challengeEvent);
				ShowBossDoorChallengeUI.spawnedUI.OnBegin -= temp;
			};
			ShowBossDoorChallengeUI.spawnedUI.OnBegin += temp;
			return;
		}
	}

	// Token: 0x04001E88 RID: 7816
	private static BossDoorChallengeUI spawnedUI;

	// Token: 0x04001E89 RID: 7817
	public FsmOwnerDefault targetDoor;

	// Token: 0x04001E8A RID: 7818
	public FsmGameObject prefab;

	// Token: 0x04001E8B RID: 7819
	public FsmEvent cancelEvent;

	// Token: 0x04001E8C RID: 7820
	public FsmEvent challengeEvent;
}
