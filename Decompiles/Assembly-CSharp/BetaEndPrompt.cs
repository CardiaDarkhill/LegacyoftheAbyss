using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200060A RID: 1546
public class BetaEndPrompt : MonoBehaviour
{
	// Token: 0x0600372F RID: 14127 RVA: 0x000F3857 File Offset: 0x000F1A57
	protected void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06003730 RID: 14128 RVA: 0x000F3865 File Offset: 0x000F1A65
	protected IEnumerator Start()
	{
		yield return new WaitForSeconds(this.delayDuration);
		this.canEnd = true;
		yield break;
	}

	// Token: 0x06003731 RID: 14129 RVA: 0x000F3874 File Offset: 0x000F1A74
	protected void Update()
	{
		if (this.canEnd && (GameManager.instance.inputHandler.inputActions.MenuSubmit.IsPressed || GameManager.instance.inputHandler.inputActions.MenuCancel.IsPressed))
		{
			this.canEnd = false;
			base.StartCoroutine(this.BeginEnd());
		}
	}

	// Token: 0x06003732 RID: 14130 RVA: 0x000F38D3 File Offset: 0x000F1AD3
	protected IEnumerator BeginEnd()
	{
		if (this.audioSource != null)
		{
			this.audioSource.Play();
		}
		this.blackFade.FadeIn();
		yield return new WaitForSeconds(this.blackFade.fadeDuration);
		GameManager.instance.StartCoroutine(GameManager.instance.ReturnToMainMenu(true, null, false, false));
		yield break;
	}

	// Token: 0x04003A03 RID: 14851
	private AudioSource audioSource;

	// Token: 0x04003A04 RID: 14852
	[SerializeField]
	private float delayDuration;

	// Token: 0x04003A05 RID: 14853
	[SerializeField]
	private SimpleSpriteFade blackFade;

	// Token: 0x04003A06 RID: 14854
	private bool canEnd;
}
