using System;
using System.Collections;
using Coffee.UISoftMask;
using UnityEngine;

// Token: 0x0200070C RID: 1804
public sealed class SoftMaskScrollIndicator : ScrollIndicator
{
	// Token: 0x06004066 RID: 16486 RVA: 0x0011B52A File Offset: 0x0011972A
	private void Start()
	{
		this.ui = UIManager.instance;
		if (this.softMask)
		{
			this.softMask.alpha = this.transitionOut.alpha;
		}
	}

	// Token: 0x06004067 RID: 16487 RVA: 0x0011B55A File Offset: 0x0011975A
	private void OnValidate()
	{
		if (!this.softMask)
		{
			this.softMask = base.GetComponent<SoftMask>();
		}
	}

	// Token: 0x06004068 RID: 16488 RVA: 0x0011B575 File Offset: 0x00119775
	public override void Show()
	{
		this.DoTransition(this.transitionIn);
	}

	// Token: 0x06004069 RID: 16489 RVA: 0x0011B583 File Offset: 0x00119783
	public override void Hide()
	{
		this.DoTransition(this.transitionOut);
	}

	// Token: 0x0600406A RID: 16490 RVA: 0x0011B591 File Offset: 0x00119791
	private void DoTransition(SoftMaskScrollIndicator.Transition transition)
	{
		if (this.coroutine != null)
		{
			this.ui.StopCoroutine(this.coroutine);
		}
		this.coroutine = this.ui.StartCoroutine(this.DoTransitionRoutine(transition));
	}

	// Token: 0x0600406B RID: 16491 RVA: 0x0011B5C4 File Offset: 0x001197C4
	private IEnumerator DoTransitionRoutine(SoftMaskScrollIndicator.Transition transition)
	{
		if (this.softMask)
		{
			if (transition.duration > 0f)
			{
				float t = 0f;
				float inverse = 1f / transition.duration;
				float startValue = this.softMask.alpha;
				while (t < 1f)
				{
					yield return null;
					t += Time.deltaTime * inverse;
					this.softMask.alpha = Mathf.Lerp(startValue, transition.alpha, t);
				}
			}
			this.softMask.alpha = transition.alpha;
		}
		yield break;
	}

	// Token: 0x040041FC RID: 16892
	[SerializeField]
	private SoftMask softMask;

	// Token: 0x040041FD RID: 16893
	[SerializeField]
	private SoftMaskScrollIndicator.Transition transitionIn;

	// Token: 0x040041FE RID: 16894
	[SerializeField]
	private SoftMaskScrollIndicator.Transition transitionOut;

	// Token: 0x040041FF RID: 16895
	private UIManager ui;

	// Token: 0x04004200 RID: 16896
	private Coroutine coroutine;

	// Token: 0x020019FA RID: 6650
	[Serializable]
	private class Transition
	{
		// Token: 0x040097F3 RID: 38899
		public float duration = 0.15f;

		// Token: 0x040097F4 RID: 38900
		public float alpha;
	}
}
