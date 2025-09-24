using System;
using System.Collections;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004BC RID: 1212
public class CogCylinderPuzzle : MonoBehaviour
{
	// Token: 0x06002BB9 RID: 11193 RVA: 0x000BFA4C File Offset: 0x000BDC4C
	[UsedImplicitly]
	private bool? ValidateFsmBool(string boolName)
	{
		if (!this.notifyCompleted || string.IsNullOrEmpty(boolName))
		{
			return null;
		}
		return new bool?(this.notifyCompleted.FsmVariables.FindFsmBool(boolName) != null);
	}

	// Token: 0x06002BBA RID: 11194 RVA: 0x000BFA94 File Offset: 0x000BDC94
	private void Awake()
	{
		this.readCogs = base.GetComponentsInChildren<CogCylinderPuzzleCog>();
		CogCylinderPuzzleCog[] array = this.readCogs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RotateFinished += this.CheckComplete;
		}
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += this.OnGetSaveState;
			this.persistent.OnSetSaveState += this.OnSetSaveState;
		}
		if (this.scaffoldActive)
		{
			this.scaffoldActive.SetActive(true);
		}
		if (this.scaffoldBreak)
		{
			this.scaffoldBreak.SetActive(false);
		}
	}

	// Token: 0x06002BBB RID: 11195 RVA: 0x000BFB43 File Offset: 0x000BDD43
	private void OnGetSaveState(out bool value)
	{
		value = this.isComplete;
	}

	// Token: 0x06002BBC RID: 11196 RVA: 0x000BFB4D File Offset: 0x000BDD4D
	private void OnSetSaveState(bool value)
	{
		if (!value)
		{
			return;
		}
		this.InternalSetComplete();
	}

	// Token: 0x06002BBD RID: 11197 RVA: 0x000BFB59 File Offset: 0x000BDD59
	private void Start()
	{
		if (!this.isComplete)
		{
			this.stateAnimator.Play("Drop", 0, 0f);
			this.stateAnimator.enabled = false;
			this.stateAnimator.Update(0f);
		}
	}

	// Token: 0x06002BBE RID: 11198 RVA: 0x000BFB98 File Offset: 0x000BDD98
	private void CheckComplete()
	{
		if (this.isComplete)
		{
			return;
		}
		bool flag = true;
		CogCylinderPuzzleCog[] array = this.readCogs;
		for (int i = 0; i < array.Length; i++)
		{
			if (!array[i].IsInTargetPos)
			{
				flag = false;
				break;
			}
		}
		this.isComplete = flag;
		if (this.isComplete)
		{
			base.StartCoroutine(this.Complete());
		}
	}

	// Token: 0x06002BBF RID: 11199 RVA: 0x000BFBEF File Offset: 0x000BDDEF
	private IEnumerator Complete()
	{
		CogCylinderPuzzleCog[] array = this.readCogs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetComplete();
		}
		yield return new WaitForSeconds(this.completionDelay);
		if (this.scaffoldActive)
		{
			this.scaffoldActive.SetActive(false);
		}
		if (this.scaffoldBreak)
		{
			this.scaffoldBreak.SetActive(true);
		}
		this.scaffoldBreakShake.DoShake(this, true);
		this.scaffoldBreakSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.stateAnimator.enabled = true;
		this.stateAnimator.Play("Drop", 0, 0f);
		yield return null;
		yield return new WaitForSeconds(this.stateAnimator.GetCurrentAnimatorStateInfo(0).length);
		this.NotifyComplete(false);
		yield break;
	}

	// Token: 0x06002BC0 RID: 11200 RVA: 0x000BFC00 File Offset: 0x000BDE00
	private void NotifyComplete(bool isInstant)
	{
		if (!string.IsNullOrEmpty(this.fsmBoolName))
		{
			FsmBool fsmBool = this.notifyCompleted.FsmVariables.FindFsmBool(this.fsmBoolName);
			if (fsmBool != null)
			{
				fsmBool.Value = true;
			}
		}
		this.notifyCompleted.SendEvent(isInstant ? "CYLINDER ALREADY COMPLETE" : "CYLINDER COMPLETED");
	}

	// Token: 0x06002BC1 RID: 11201 RVA: 0x000BFC55 File Offset: 0x000BDE55
	public void StartChoir()
	{
		if (this.choirParent)
		{
			this.choirParent.BroadcastMessage("StartJitter");
		}
		this.OnChoirStart.Invoke();
	}

	// Token: 0x06002BC2 RID: 11202 RVA: 0x000BFC7F File Offset: 0x000BDE7F
	public void StopChoir()
	{
		if (this.choirParent)
		{
			this.choirParent.BroadcastMessage("StopJitter");
		}
	}

	// Token: 0x06002BC3 RID: 11203 RVA: 0x000BFCA0 File Offset: 0x000BDEA0
	public void SetComplete()
	{
		if (this.persistent)
		{
			this.persistent.OnGetSaveState -= this.OnGetSaveState;
			this.persistent.OnSetSaveState -= this.OnSetSaveState;
		}
		this.InternalSetComplete();
	}

	// Token: 0x06002BC4 RID: 11204 RVA: 0x000BFCF0 File Offset: 0x000BDEF0
	private void InternalSetComplete()
	{
		this.isComplete = true;
		CogCylinderPuzzleCog[] array = this.readCogs;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetComplete();
		}
		this.stateAnimator.enabled = true;
		this.stateAnimator.Play("Drop", 0, 1f);
		this.NotifyComplete(true);
		if (this.scaffoldActive)
		{
			this.scaffoldActive.SetActive(false);
		}
	}

	// Token: 0x04002D05 RID: 11525
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04002D06 RID: 11526
	[SerializeField]
	private Animator stateAnimator;

	// Token: 0x04002D07 RID: 11527
	[SerializeField]
	private float completionDelay;

	// Token: 0x04002D08 RID: 11528
	[Space]
	[SerializeField]
	private PlayMakerFSM notifyCompleted;

	// Token: 0x04002D09 RID: 11529
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("ValidateFsmBool")]
	private string fsmBoolName;

	// Token: 0x04002D0A RID: 11530
	[Space]
	[SerializeField]
	private GameObject scaffoldActive;

	// Token: 0x04002D0B RID: 11531
	[SerializeField]
	private GameObject scaffoldBreak;

	// Token: 0x04002D0C RID: 11532
	[SerializeField]
	private AudioEventRandom scaffoldBreakSound;

	// Token: 0x04002D0D RID: 11533
	[SerializeField]
	private CameraShakeTarget scaffoldBreakShake;

	// Token: 0x04002D0E RID: 11534
	[Space]
	[SerializeField]
	private GameObject choirParent;

	// Token: 0x04002D0F RID: 11535
	public UnityEvent OnChoirStart;

	// Token: 0x04002D10 RID: 11536
	private bool isComplete;

	// Token: 0x04002D11 RID: 11537
	private CogCylinderPuzzleCog[] readCogs;
}
