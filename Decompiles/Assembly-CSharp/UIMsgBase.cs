using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000740 RID: 1856
public abstract class UIMsgBase<TTargetObject> : UIMsgProxy
{
	// Token: 0x06004237 RID: 16951 RVA: 0x00124A96 File Offset: 0x00122C96
	protected virtual int GetHideAnimId()
	{
		return UIMsgBase<TTargetObject>._hideProp;
	}

	// Token: 0x06004238 RID: 16952 RVA: 0x00124A9D File Offset: 0x00122C9D
	protected static UIMsgBase<TTargetObject> Spawn(TTargetObject item, UIMsgBase<TTargetObject> prefab, Action afterMsg)
	{
		if (!prefab)
		{
			return null;
		}
		UIMsgBase<TTargetObject> uimsgBase = Object.Instantiate<UIMsgBase<TTargetObject>>(prefab);
		uimsgBase.StartCoroutine(uimsgBase.DoMsg(item, afterMsg));
		return uimsgBase;
	}

	// Token: 0x06004239 RID: 16953 RVA: 0x00124ABE File Offset: 0x00122CBE
	private IEnumerator DoMsg(TTargetObject item, Action afterMsg)
	{
		base.SetIsInMsg(true);
		this.Setup(item);
		VibrationManager.FadeVibration(0f, 0.25f);
		if (this.stop)
		{
			this.stop.SetActive(false);
		}
		Color colour = ScreenFaderUtils.GetColour();
		bool flag = colour.r < 0.01f && colour.g < 0.01f && colour.b < 0.01f && colour.a > 0.99f;
		if (this.animator)
		{
			if (!flag && this.animator.HasState(0, UIMsgBase<TTargetObject>._fadeUpProp))
			{
				this.animator.Play(UIMsgBase<TTargetObject>._fadeUpProp);
				yield return null;
				if (this.startPauseTime > 0f)
				{
					this.animator.SetFloat(UIMsgBase<TTargetObject>._appearSpeedProp, 0f);
					yield return new WaitForSeconds(this.startPauseTime);
					this.animator.SetFloat(UIMsgBase<TTargetObject>._appearSpeedProp, 1f);
					yield return null;
				}
				float length = this.animator.GetCurrentAnimatorStateInfo(0).length;
				yield return new WaitForSeconds(length);
			}
			this.animator.Play(UIMsgBase<TTargetObject>._showProp);
			yield return null;
			float length2 = this.animator.GetCurrentAnimatorStateInfo(0).length;
			yield return new WaitForSeconds(length2);
		}
		if (this.stop)
		{
			this.stop.SetActive(true);
		}
		bool wasPressed = false;
		while (!wasPressed)
		{
			wasPressed = GameManager.instance.inputHandler.WasSkipButtonPressed;
			yield return null;
		}
		Audio.StopConfirmSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		if (this.stop)
		{
			this.stop.SetActive(false);
		}
		VibrationManager.FadeVibration(1f, 0.25f);
		if (this.animator)
		{
			this.animator.Play(this.GetHideAnimId());
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		base.SetIsInMsg(false);
		if (afterMsg != null)
		{
			afterMsg();
		}
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x0600423A RID: 16954
	protected abstract void Setup(TTargetObject targetObject);

	// Token: 0x040043D8 RID: 17368
	protected const float VIBRATION_FADE_DURATION = 0.25f;

	// Token: 0x040043D9 RID: 17369
	[SerializeField]
	private Animator animator;

	// Token: 0x040043DA RID: 17370
	[SerializeField]
	private float startPauseTime;

	// Token: 0x040043DB RID: 17371
	[SerializeField]
	private GameObject stop;

	// Token: 0x040043DC RID: 17372
	private static readonly int _appearSpeedProp = Animator.StringToHash("Appear Speed");

	// Token: 0x040043DD RID: 17373
	private static readonly int _fadeUpProp = Animator.StringToHash("Fade Up");

	// Token: 0x040043DE RID: 17374
	private static readonly int _showProp = Animator.StringToHash("Show");

	// Token: 0x040043DF RID: 17375
	private static readonly int _hideProp = Animator.StringToHash("Hide");
}
