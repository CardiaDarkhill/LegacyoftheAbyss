using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000242 RID: 578
public class HeroTriggerFader : MonoBehaviour
{
	// Token: 0x0600151F RID: 5407 RVA: 0x0005FA70 File Offset: 0x0005DC70
	private void OnEnable()
	{
		if (HeroController.instance.isHeroInPosition)
		{
			base.StartCoroutine(this.MonitorTriggerAndFade());
			return;
		}
		HeroController.instance.heroInPosition += this.Delayed;
	}

	// Token: 0x06001520 RID: 5408 RVA: 0x0005FAA2 File Offset: 0x0005DCA2
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06001521 RID: 5409 RVA: 0x0005FAAC File Offset: 0x0005DCAC
	private void OnDestroy()
	{
		HeroController instance = HeroController.instance;
		if (instance != null)
		{
			instance.heroInPosition -= this.Delayed;
		}
	}

	// Token: 0x06001522 RID: 5410 RVA: 0x0005FADA File Offset: 0x0005DCDA
	private void Delayed(bool _)
	{
		if (base.isActiveAndEnabled)
		{
			base.StartCoroutine(this.MonitorTriggerAndFade());
		}
		HeroController.instance.heroInPosition -= this.Delayed;
	}

	// Token: 0x06001523 RID: 5411 RVA: 0x0005FB07 File Offset: 0x0005DD07
	private IEnumerator MonitorTriggerAndFade()
	{
		this.SetValue(this.trigger.IsInside ? this.valueRange.End : this.valueRange.Start);
		for (;;)
		{
			bool wasShowing = this.ShouldBeVisible();
			float startAlpha = this.currentValue;
			float fadeUpTime = this.fadeTime;
			if (Math.Abs(startAlpha) <= Mathf.Epsilon)
			{
				float randomValue = this.fadeUpFromZeroDelay.GetRandomValue();
				if (randomValue > 0f)
				{
					yield return new WaitForSeconds(randomValue);
				}
				this.OnFadeFromZero.Invoke();
				if (this.fadeUpFromZeroTime.IsEnabled)
				{
					fadeUpTime = this.fadeUpFromZeroTime.Value;
				}
			}
			float targetAlpha = wasShowing ? this.valueRange.End : this.valueRange.Start;
			float num = Mathf.Abs(startAlpha - targetAlpha);
			float num2 = (!wasShowing && this.fadeDownTime.IsEnabled) ? this.fadeDownTime.Value : fadeUpTime;
			float currentFadeTime = num * num2;
			float elapsedTime = 0f;
			while (elapsedTime < currentFadeTime && this.ShouldBeVisible() == wasShowing)
			{
				float t = Mathf.Clamp01(elapsedTime / currentFadeTime);
				this.SetValue(Mathf.Lerp(startAlpha, targetAlpha, t));
				yield return null;
				elapsedTime += Time.deltaTime;
			}
			this.SetValue(targetAlpha);
			while (this.ShouldBeVisible() == wasShowing)
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06001524 RID: 5412 RVA: 0x0005FB16 File Offset: 0x0005DD16
	private bool ShouldBeVisible()
	{
		if (this.fadeDownOnInspect)
		{
			if (InteractManager.BlockingInteractable)
			{
				return false;
			}
			if (InteractManager.IsPromptVisible)
			{
				return false;
			}
		}
		return this.trigger.IsInside;
	}

	// Token: 0x06001525 RID: 5413 RVA: 0x0005FB42 File Offset: 0x0005DD42
	private void SetValue(float value)
	{
		this.currentValue = value;
		if (this.FadeValueChanged != null)
		{
			this.FadeValueChanged.Invoke(value);
		}
	}

	// Token: 0x040013B1 RID: 5041
	[SerializeField]
	private TrackTriggerObjects trigger;

	// Token: 0x040013B2 RID: 5042
	[SerializeField]
	private MinMaxFloat valueRange;

	// Token: 0x040013B3 RID: 5043
	[SerializeField]
	private float fadeTime;

	// Token: 0x040013B4 RID: 5044
	[SerializeField]
	private MinMaxFloat fadeUpFromZeroDelay;

	// Token: 0x040013B5 RID: 5045
	[SerializeField]
	private OverrideFloat fadeUpFromZeroTime;

	// Token: 0x040013B6 RID: 5046
	[SerializeField]
	private OverrideFloat fadeDownTime;

	// Token: 0x040013B7 RID: 5047
	[SerializeField]
	private bool fadeDownOnInspect;

	// Token: 0x040013B8 RID: 5048
	[Space]
	public HeroTriggerFader.UnityFloatEvent FadeValueChanged;

	// Token: 0x040013B9 RID: 5049
	public UnityEvent OnFadeFromZero;

	// Token: 0x040013BA RID: 5050
	private bool hasStarted;

	// Token: 0x040013BB RID: 5051
	private float currentValue;

	// Token: 0x02001549 RID: 5449
	[Serializable]
	public class UnityFloatEvent : UnityEvent<float>
	{
	}
}
