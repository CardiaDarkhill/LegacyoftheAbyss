using System;
using System.Collections;
using UnityEngine;

// Token: 0x020005FE RID: 1534
public class TransitionPointDoorAnim : MonoBehaviour, ITransitionPointDoorAnim
{
	// Token: 0x060036CB RID: 14027 RVA: 0x000F1D13 File Offset: 0x000EFF13
	private void Awake()
	{
		this.transitionPoint.DoorAnimHandler = this;
	}

	// Token: 0x060036CC RID: 14028 RVA: 0x000F1D24 File Offset: 0x000EFF24
	private void Start()
	{
		GameManager gm = GameManager.instance;
		string name = this.transitionPoint.gameObject.name;
		string entryGateName = gm.GetEntryGateName();
		if (!gm.HasFinishedEnteringScene && entryGateName == name)
		{
			this.animator.Play(TransitionPointDoorAnim._openAnim, 0, 1f);
			GameManager.EnterSceneEvent temp = null;
			temp = delegate()
			{
				this.closeSound.SpawnAndPlayOneShot(this.transform.position, null);
				this.animator.Play(TransitionPointDoorAnim._closeAnim, 0, 0f);
				gm.OnFinishedEnteringScene -= temp;
			};
			gm.OnFinishedEnteringScene += temp;
			return;
		}
		this.animator.Play(TransitionPointDoorAnim._closeAnim, 0, 1f);
	}

	// Token: 0x060036CD RID: 14029 RVA: 0x000F1DD3 File Offset: 0x000EFFD3
	public Coroutine GetDoorAnimRoutine()
	{
		return base.StartCoroutine(this.DoorEnterAnimRoutine());
	}

	// Token: 0x060036CE RID: 14030 RVA: 0x000F1DE1 File Offset: 0x000EFFE1
	private IEnumerator DoorEnterAnimRoutine()
	{
		this.openSound.SpawnAndPlayOneShot(base.transform.position, null);
		this.animator.Play(TransitionPointDoorAnim._openAnim, 0, 0f);
		yield return null;
		yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		yield break;
	}

	// Token: 0x0400399B RID: 14747
	private static readonly int _openAnim = Animator.StringToHash("Open");

	// Token: 0x0400399C RID: 14748
	private static readonly int _closeAnim = Animator.StringToHash("Close");

	// Token: 0x0400399D RID: 14749
	[SerializeField]
	private TransitionPoint transitionPoint;

	// Token: 0x0400399E RID: 14750
	[SerializeField]
	private Animator animator;

	// Token: 0x0400399F RID: 14751
	[SerializeField]
	private AudioEvent openSound;

	// Token: 0x040039A0 RID: 14752
	[SerializeField]
	private AudioEvent closeSound;
}
