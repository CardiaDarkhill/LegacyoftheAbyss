using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000704 RID: 1796
public sealed class CanvasGroupScrollIndicator : ScrollIndicator
{
	// Token: 0x06004020 RID: 16416 RVA: 0x0011A7EF File Offset: 0x001189EF
	private void Start()
	{
		this.ui = UIManager.instance;
		if (this.canvasGroup)
		{
			this.canvasGroup.alpha = this.transitionOut.alpha;
		}
	}

	// Token: 0x06004021 RID: 16417 RVA: 0x0011A81F File Offset: 0x00118A1F
	private void OnValidate()
	{
		if (!this.canvasGroup)
		{
			this.canvasGroup = base.GetComponent<CanvasGroup>();
		}
	}

	// Token: 0x06004022 RID: 16418 RVA: 0x0011A83A File Offset: 0x00118A3A
	public override void Show()
	{
		this.DoTransition(this.transitionIn);
	}

	// Token: 0x06004023 RID: 16419 RVA: 0x0011A848 File Offset: 0x00118A48
	public override void Hide()
	{
		this.DoTransition(this.transitionOut);
	}

	// Token: 0x06004024 RID: 16420 RVA: 0x0011A856 File Offset: 0x00118A56
	private void DoTransition(CanvasGroupScrollIndicator.Transition transition)
	{
		if (this.coroutine != null)
		{
			this.ui.StopCoroutine(this.coroutine);
		}
		this.coroutine = this.ui.StartCoroutine(this.DoTransitionRoutine(transition));
	}

	// Token: 0x06004025 RID: 16421 RVA: 0x0011A889 File Offset: 0x00118A89
	private IEnumerator DoTransitionRoutine(CanvasGroupScrollIndicator.Transition transition)
	{
		if (this.canvasGroup)
		{
			if (transition.duration > 0f)
			{
				float t = 0f;
				float inverse = 1f / transition.duration;
				float startValue = this.canvasGroup.alpha;
				while (t < 1f)
				{
					yield return null;
					t += Time.deltaTime * inverse;
					this.canvasGroup.alpha = Mathf.Lerp(startValue, transition.alpha, t);
				}
			}
			this.canvasGroup.alpha = transition.alpha;
		}
		yield break;
	}

	// Token: 0x040041CA RID: 16842
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x040041CB RID: 16843
	[SerializeField]
	private CanvasGroupScrollIndicator.Transition transitionIn;

	// Token: 0x040041CC RID: 16844
	[SerializeField]
	private CanvasGroupScrollIndicator.Transition transitionOut;

	// Token: 0x040041CD RID: 16845
	private UIManager ui;

	// Token: 0x040041CE RID: 16846
	private Coroutine coroutine;

	// Token: 0x020019EF RID: 6639
	[Serializable]
	private class Transition
	{
		// Token: 0x040097CA RID: 38858
		public float duration = 0.15f;

		// Token: 0x040097CB RID: 38859
		public float alpha;
	}
}
