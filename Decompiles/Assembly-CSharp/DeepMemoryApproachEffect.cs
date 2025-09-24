using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200022C RID: 556
public class DeepMemoryApproachEffect : MonoBehaviour
{
	// Token: 0x06001488 RID: 5256 RVA: 0x0005C82F File Offset: 0x0005AA2F
	private void Awake()
	{
		this.group.AlphaSelf = 0f;
	}

	// Token: 0x06001489 RID: 5257 RVA: 0x0005C841 File Offset: 0x0005AA41
	private void Start()
	{
		this.hc = HeroController.instance;
		this.heartBeatCtrl.Multiplier = 0f;
		this.SetInside(false);
	}

	// Token: 0x0600148A RID: 5258 RVA: 0x0005C865 File Offset: 0x0005AA65
	private void OnDisable()
	{
		this.SetInside(false);
	}

	// Token: 0x0600148B RID: 5259 RVA: 0x0005C870 File Offset: 0x0005AA70
	private void LateUpdate()
	{
		if (!this.insideRange.IsInside)
		{
			if (this.previousValue > Mathf.Epsilon)
			{
				this.UpdatePosition();
			}
			if (!this.wasInside)
			{
				return;
			}
			if (this.fadeRoutine != null)
			{
				base.StopCoroutine(this.fadeRoutine);
			}
			this.fadeRoutine = base.StartCoroutine(this.FadeRoutine(0f, this.fadeOutDuration, this.fadeOutCurve));
			this.SetInside(false);
			EventRegister.SendEvent(this.exitEventId, null);
			this.wasInside = false;
			return;
		}
		else
		{
			this.UpdatePosition();
			if (this.wasInside)
			{
				return;
			}
			if (this.fadeRoutine != null)
			{
				base.StopCoroutine(this.fadeRoutine);
			}
			this.fadeRoutine = base.StartCoroutine(this.FadeRoutine(1f, this.fadeInDuration, this.fadeInCurve));
			this.SetInside(true);
			EventRegister.SendEvent(this.enterEventId, null);
			this.wasInside = true;
			return;
		}
	}

	// Token: 0x0600148C RID: 5260 RVA: 0x0005C958 File Offset: 0x0005AB58
	private void SetInside(bool isInside)
	{
		if (this.activeWhileInside)
		{
			this.activeWhileInside.SetActive(isInside);
		}
		if (isInside)
		{
			NeedolinMsgBox.AddBlocker(this);
			return;
		}
		NeedolinMsgBox.RemoveBlocker(this);
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x0005C984 File Offset: 0x0005AB84
	private void UpdatePosition()
	{
		Vector2 position = this.hc.transform.position;
		this.followHero.SetPosition2D(position);
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x0005C9B3 File Offset: 0x0005ABB3
	private IEnumerator FadeRoutine(float targetValue, float fadeDuration, AnimationCurve curve)
	{
		float initialValue = this.previousValue;
		for (float elapsed = 0f; elapsed < fadeDuration; elapsed += Time.deltaTime)
		{
			float t = curve.Evaluate(elapsed / fadeDuration);
			float value = Mathf.Lerp(initialValue, targetValue, t);
			this.SetValue(value);
			yield return null;
		}
		this.SetValue(targetValue);
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x0005C9D7 File Offset: 0x0005ABD7
	private void SetValue(float value)
	{
		this.previousValue = value;
		this.group.AlphaSelf = value;
		this.heartBeatCtrl.Multiplier = value;
	}

	// Token: 0x040012DC RID: 4828
	[SerializeField]
	private TrackTriggerObjects insideRange;

	// Token: 0x040012DD RID: 4829
	[SerializeField]
	private Transform followHero;

	// Token: 0x040012DE RID: 4830
	[SerializeField]
	private MemoryHeartBeat heartBeatCtrl;

	// Token: 0x040012DF RID: 4831
	[SerializeField]
	private NestedFadeGroupBase group;

	// Token: 0x040012E0 RID: 4832
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x040012E1 RID: 4833
	[SerializeField]
	private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040012E2 RID: 4834
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x040012E3 RID: 4835
	[SerializeField]
	private AnimationCurve fadeOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040012E4 RID: 4836
	[SerializeField]
	private GameObject activeWhileInside;

	// Token: 0x040012E5 RID: 4837
	private bool wasInside;

	// Token: 0x040012E6 RID: 4838
	private Coroutine fadeRoutine;

	// Token: 0x040012E7 RID: 4839
	private float previousValue;

	// Token: 0x040012E8 RID: 4840
	private HeroController hc;

	// Token: 0x040012E9 RID: 4841
	private readonly int enterEventId = EventRegister.GetEventHashCode("ENTER DEEP MEMORY ZONE");

	// Token: 0x040012EA RID: 4842
	private readonly int exitEventId = EventRegister.GetEventHashCode("EXIT DEEP MEMORY ZONE");
}
