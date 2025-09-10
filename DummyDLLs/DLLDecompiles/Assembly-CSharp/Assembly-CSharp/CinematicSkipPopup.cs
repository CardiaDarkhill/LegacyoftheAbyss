using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000617 RID: 1559
public class CinematicSkipPopup : MonoBehaviour
{
	// Token: 0x06003787 RID: 14215 RVA: 0x000F4DB0 File Offset: 0x000F2FB0
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<GameObject>(ref this.textGroups, typeof(CinematicSkipPopup.Texts));
	}

	// Token: 0x06003788 RID: 14216 RVA: 0x000F4DC7 File Offset: 0x000F2FC7
	private void Awake()
	{
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x06003789 RID: 14217 RVA: 0x000F4DD8 File Offset: 0x000F2FD8
	public void Show(CinematicSkipPopup.Texts text)
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(true);
			this.canvasGroup.alpha = 0f;
		}
		for (int i = 0; i < this.textGroups.Length; i++)
		{
			this.textGroups[i].SetActive(i == (int)text);
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeToAlpha(1f, this.fadeInDuration, false, null));
	}

	// Token: 0x0600378A RID: 14218 RVA: 0x000F4E68 File Offset: 0x000F3068
	public void Hide(bool isInstant, Action onEnd)
	{
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		if (isInstant)
		{
			if (onEnd != null)
			{
				onEnd();
			}
			base.gameObject.SetActive(false);
			return;
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeToAlpha(0f, this.fadeOutDuration, true, onEnd));
	}

	// Token: 0x0600378B RID: 14219 RVA: 0x000F4EC1 File Offset: 0x000F30C1
	private IEnumerator FadeToAlpha(float targetAlpha, float duration, bool disableOnEnd, Action onEnd)
	{
		float elapsed = 0f;
		float startAlpha = this.canvasGroup.alpha;
		while (elapsed < duration)
		{
			this.canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
			yield return null;
			elapsed += Time.unscaledDeltaTime;
		}
		if (onEnd != null)
		{
			onEnd();
		}
		if (disableOnEnd)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			this.canvasGroup.alpha = targetAlpha;
		}
		yield break;
	}

	// Token: 0x04003A7E RID: 14974
	[SerializeField]
	[ArrayForEnum(typeof(CinematicSkipPopup.Texts))]
	private GameObject[] textGroups;

	// Token: 0x04003A7F RID: 14975
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x04003A80 RID: 14976
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x04003A81 RID: 14977
	private float showTimer;

	// Token: 0x04003A82 RID: 14978
	private Coroutine fadeRoutine;

	// Token: 0x04003A83 RID: 14979
	private CanvasGroup canvasGroup;

	// Token: 0x02001929 RID: 6441
	public enum Texts
	{
		// Token: 0x0400949A RID: 38042
		Skip,
		// Token: 0x0400949B RID: 38043
		Loading
	}
}
