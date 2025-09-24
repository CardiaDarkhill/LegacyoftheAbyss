using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000495 RID: 1173
public class BellShrineTuningFork : MonoBehaviour
{
	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x06002A61 RID: 10849 RVA: 0x000B7C73 File Offset: 0x000B5E73
	public bool IsBellShrineCompleted
	{
		get
		{
			return PlayerData.instance.GetVariable(this.bellShrineBool);
		}
	}

	// Token: 0x06002A62 RID: 10850 RVA: 0x000B7C88 File Offset: 0x000B5E88
	public void SetInitialState(BellShrineTuningFork.States state)
	{
		switch (state)
		{
		case BellShrineTuningFork.States.Dormant:
			this.animator.SetBool(BellShrineTuningFork._activatedId, false);
			this.animator.SetBool(BellShrineTuningFork._completedId, false);
			return;
		case BellShrineTuningFork.States.Activated:
			this.animator.SetBool(BellShrineTuningFork._activatedId, true);
			this.animator.Play(BellShrineTuningFork._activatedId, 0, Random.Range(0f, 1f));
			return;
		case BellShrineTuningFork.States.AllComplete:
			this.animator.SetBool(BellShrineTuningFork._completedId, true);
			this.animator.Play(BellShrineTuningFork._completedId, 0, Random.Range(0f, 1f));
			return;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
	}

	// Token: 0x06002A63 RID: 10851 RVA: 0x000B7D44 File Offset: 0x000B5F44
	public IEnumerator DoActivate()
	{
		this.endWait = false;
		this.animator.SetBool(BellShrineTuningFork._activatedId, true);
		yield return null;
		yield return new WaitForSecondsInterruptable(this.animator.GetCurrentAnimatorStateInfo(0).length, () => this.endWait, false);
		yield break;
	}

	// Token: 0x06002A64 RID: 10852 RVA: 0x000B7D53 File Offset: 0x000B5F53
	public void FinishActivate()
	{
		this.endWait = true;
	}

	// Token: 0x06002A65 RID: 10853 RVA: 0x000B7D5C File Offset: 0x000B5F5C
	public void DoComplete(float delay)
	{
		base.StartCoroutine(this.CompleteRoutine(delay));
	}

	// Token: 0x06002A66 RID: 10854 RVA: 0x000B7D6C File Offset: 0x000B5F6C
	public void DoPulse()
	{
		this.animator.SetTrigger(BellShrineTuningFork._pulseId);
	}

	// Token: 0x06002A67 RID: 10855 RVA: 0x000B7D7E File Offset: 0x000B5F7E
	private IEnumerator CompleteRoutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.animator.SetBool(BellShrineTuningFork._completedId, true);
		yield break;
	}

	// Token: 0x06002A68 RID: 10856 RVA: 0x000B7D94 File Offset: 0x000B5F94
	public void SetSinging(bool value)
	{
		this.animator.SetBool(BellShrineTuningFork._singingId, value);
	}

	// Token: 0x04002AFC RID: 11004
	private static readonly int _activatedId = Animator.StringToHash("Activated");

	// Token: 0x04002AFD RID: 11005
	private static readonly int _completedId = Animator.StringToHash("Completed");

	// Token: 0x04002AFE RID: 11006
	private static readonly int _singingId = Animator.StringToHash("Singing");

	// Token: 0x04002AFF RID: 11007
	private static readonly int _pulseId = Animator.StringToHash("Pulse");

	// Token: 0x04002B00 RID: 11008
	[SerializeField]
	[PlayerDataField(typeof(bool), true)]
	private string bellShrineBool;

	// Token: 0x04002B01 RID: 11009
	[Space]
	[SerializeField]
	private Animator animator;

	// Token: 0x04002B02 RID: 11010
	private bool endWait;

	// Token: 0x0200179F RID: 6047
	public enum States
	{
		// Token: 0x04008EBA RID: 36538
		Dormant,
		// Token: 0x04008EBB RID: 36539
		Activated,
		// Token: 0x04008EBC RID: 36540
		AllComplete
	}
}
