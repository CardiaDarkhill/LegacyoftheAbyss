using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000387 RID: 903
public class BossSequenceBindingsDoor : MonoBehaviour
{
	// Token: 0x06001EB1 RID: 7857 RVA: 0x0008C9BD File Offset: 0x0008ABBD
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x0008C9CC File Offset: 0x0008ABCC
	private void Start()
	{
		if (GameManager.instance.GetPlayerDataBool(this.playerData))
		{
			this.SetUnlocked(true, false);
			return;
		}
		this.SetUnlocked(false, false);
		int num = BossSequenceBindingsDisplay.CountCompletedBindings();
		for (int i = 0; i < this.bindingIcons.Length; i++)
		{
			this.bindingIcons[i].SetActive(i < num);
		}
		if (num >= this.bindingIcons.Length)
		{
			this.shouldBeUnlocked = true;
		}
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x0008CA38 File Offset: 0x0008AC38
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.isUnlocked && this.shouldBeUnlocked)
		{
			GameManager.instance.SetPlayerDataBool(this.playerData, true);
			this.SetUnlocked(true, true);
		}
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x0008CA64 File Offset: 0x0008AC64
	private void SetUnlocked(bool value, bool doUnlockAnimation = false)
	{
		this.isUnlocked = value;
		if (value)
		{
			if (doUnlockAnimation && this.animator)
			{
				base.StartCoroutine(this.DoUnlockAnimation());
				return;
			}
			this.animator.Play(this.unlockedAnimation);
			if (this.transitionPointDoor)
			{
				this.transitionPointDoor.SetActive(true);
				return;
			}
		}
		else if (this.transitionPointDoor)
		{
			this.transitionPointDoor.SetActive(false);
		}
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x0008CADD File Offset: 0x0008ACDD
	private IEnumerator DoUnlockAnimation()
	{
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(HeroController.instance.gameObject, "Roar and Wound States");
		if (playMakerFSM)
		{
			playMakerFSM.FsmVariables.FindFsmGameObject("Roar Object").Value = base.gameObject;
		}
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR ENTER", false);
		this.animator.Play(this.unlockAnimation);
		yield return null;
		yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		FSMUtility.SendEventToGameObject(HeroController.instance.gameObject, "ROAR EXIT", false);
		if (this.transitionPointDoor)
		{
			this.transitionPointDoor.SetActive(true);
		}
		yield break;
	}

	// Token: 0x04001DA0 RID: 7584
	public string playerData = "blueRoomDoorUnlocked";

	// Token: 0x04001DA1 RID: 7585
	public GameObject[] bindingIcons;

	// Token: 0x04001DA2 RID: 7586
	public GameObject transitionPointDoor;

	// Token: 0x04001DA3 RID: 7587
	public float doorEnableAnimDelay = 1f;

	// Token: 0x04001DA4 RID: 7588
	public string unlockAnimation = "Unlock";

	// Token: 0x04001DA5 RID: 7589
	public string unlockedAnimation = "Unlocked";

	// Token: 0x04001DA6 RID: 7590
	private bool isUnlocked;

	// Token: 0x04001DA7 RID: 7591
	private bool shouldBeUnlocked;

	// Token: 0x04001DA8 RID: 7592
	private Animator animator;
}
