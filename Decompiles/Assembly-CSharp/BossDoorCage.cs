using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000377 RID: 887
[RequireComponent(typeof(Animator))]
public class BossDoorCage : MonoBehaviour
{
	// Token: 0x06001E54 RID: 7764 RVA: 0x0008BBB7 File Offset: 0x00089DB7
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.cameraShake = base.GetComponent<CameraControlAnimationEvents>();
	}

	// Token: 0x06001E55 RID: 7765 RVA: 0x0008BBD4 File Offset: 0x00089DD4
	private void Start()
	{
		if (GameManager.instance.GetPlayerDataBool(this.playerData))
		{
			base.gameObject.SetActive(false);
			return;
		}
		if (this.unlockTrigger)
		{
			this.unlockTrigger.OnTriggerEntered += delegate(Collider2D collision, GameObject sender)
			{
				this.Unlock();
			};
		}
	}

	// Token: 0x06001E56 RID: 7766 RVA: 0x0008BC24 File Offset: 0x00089E24
	private void Unlock()
	{
		if (GameManager.instance.GetPlayerDataBool(this.playerData))
		{
			return;
		}
		BossSequenceDoor[] array = this.requiredComplete;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].CurrentCompletion.completed)
			{
				return;
			}
		}
		GameManager.instance.SetPlayerDataBool(this.playerData, true);
		base.StartCoroutine(this.UnlockRoutine());
	}

	// Token: 0x06001E57 RID: 7767 RVA: 0x0008BC87 File Offset: 0x00089E87
	private IEnumerator UnlockRoutine()
	{
		this.animator.Play("Unlock");
		yield return null;
		yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		yield break;
	}

	// Token: 0x06001E58 RID: 7768 RVA: 0x0008BC98 File Offset: 0x00089E98
	public void StartShakeLock()
	{
		if (this.cameraShake)
		{
			this.cameraShake.SmallRumble();
		}
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HeroController.instance.gameObject, "Roar and Wound States");
		if (playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Roar Object").Value = base.gameObject;
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR ENTER", false);
	}

	// Token: 0x06001E59 RID: 7769 RVA: 0x0008BD0A File Offset: 0x00089F0A
	public void StopShakeLock()
	{
		if (this.cameraShake)
		{
			this.cameraShake.StopRumble();
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR EXIT", false);
	}

	// Token: 0x04001D58 RID: 7512
	public BossSequenceDoor[] requiredComplete;

	// Token: 0x04001D59 RID: 7513
	public TriggerEnterEvent unlockTrigger;

	// Token: 0x04001D5A RID: 7514
	public string playerData = "bossDoorCageUnlocked";

	// Token: 0x04001D5B RID: 7515
	private Animator animator;

	// Token: 0x04001D5C RID: 7516
	private CameraControlAnimationEvents cameraShake;
}
