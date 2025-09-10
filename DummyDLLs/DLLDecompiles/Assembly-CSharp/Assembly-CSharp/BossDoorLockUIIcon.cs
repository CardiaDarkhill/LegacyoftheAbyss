using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020003AA RID: 938
[RequireComponent(typeof(Animator))]
public class BossDoorLockUIIcon : MonoBehaviour
{
	// Token: 0x06001F96 RID: 8086 RVA: 0x000906EE File Offset: 0x0008E8EE
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06001F97 RID: 8087 RVA: 0x000906FC File Offset: 0x0008E8FC
	public void SetUnlocked(bool unlocked, bool doUnlockAnim = false, int indexAnimOffset = 0)
	{
		if (unlocked)
		{
			base.StartCoroutine(this.PlayAnimWithDelay("Unlock", doUnlockAnim, doUnlockAnim ? (this.unlockAnimDelay + this.indexOffsetDelay * (float)indexAnimOffset) : 0f));
			return;
		}
		this.animator.Play("Locked");
	}

	// Token: 0x06001F98 RID: 8088 RVA: 0x0009074A File Offset: 0x0008E94A
	private IEnumerator PlayAnimWithDelay(string anim, bool doAnim, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (doAnim)
		{
			this.animator.Play(anim);
			this.unlockSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		else
		{
			this.animator.Play(anim, 0, 1f);
		}
		yield break;
	}

	// Token: 0x04001EA7 RID: 7847
	public Image bossIcon;

	// Token: 0x04001EA8 RID: 7848
	public float unlockAnimDelay = 1f;

	// Token: 0x04001EA9 RID: 7849
	public float indexOffsetDelay = 0.25f;

	// Token: 0x04001EAA RID: 7850
	public AudioSource audioSourcePrefab;

	// Token: 0x04001EAB RID: 7851
	public AudioEvent unlockSound;

	// Token: 0x04001EAC RID: 7852
	private Animator animator;
}
