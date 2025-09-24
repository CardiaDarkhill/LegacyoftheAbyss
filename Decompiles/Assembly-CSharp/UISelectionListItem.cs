using System;
using TeamCherry.NestedFadeGroup;
using TMProOld;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020006FB RID: 1787
public class UISelectionListItem : MonoBehaviour
{
	// Token: 0x1700074B RID: 1867
	// (get) Token: 0x06003FF1 RID: 16369 RVA: 0x00119FEB File Offset: 0x001181EB
	// (set) Token: 0x06003FF2 RID: 16370 RVA: 0x00119FF3 File Offset: 0x001181F3
	public Func<bool> AutoSelect { get; set; }

	// Token: 0x1700074C RID: 1868
	// (get) Token: 0x06003FF3 RID: 16371 RVA: 0x00119FFC File Offset: 0x001181FC
	// (set) Token: 0x06003FF4 RID: 16372 RVA: 0x0011A004 File Offset: 0x00118204
	public Func<string> InactiveConditionText
	{
		get
		{
			return this._inactiveConditionText;
		}
		set
		{
			this._inactiveConditionText = value;
			this.UpdateAppearance();
		}
	}

	// Token: 0x06003FF5 RID: 16373 RVA: 0x0011A013 File Offset: 0x00118213
	private void OnEnable()
	{
		if (this.submitEffect)
		{
			this.submitEffect.SetActive(false);
		}
		this.UpdateAppearance();
	}

	// Token: 0x06003FF6 RID: 16374 RVA: 0x0011A034 File Offset: 0x00118234
	private void OnDisable()
	{
		this.skipNextSelectSound = false;
	}

	// Token: 0x06003FF7 RID: 16375 RVA: 0x0011A040 File Offset: 0x00118240
	public void SetSelected(bool value, bool isInstant)
	{
		bool? flag = this.previouslySelected;
		if (flag.GetValueOrDefault() == value & flag != null)
		{
			this.skipNextSelectSound = false;
			return;
		}
		this.previouslySelected = new bool?(value);
		string name;
		UnityEvent unityEvent;
		if (value)
		{
			name = this.selectionIndicatorUpAnim;
			unityEvent = this.Selected;
			if (!isInstant && !this.skipNextSelectSound)
			{
				this.selectSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			}
			this.skipNextSelectSound = false;
		}
		else
		{
			name = this.selectionIndicatorDownAnim;
			unityEvent = this.Deselected;
		}
		foreach (tk2dSpriteAnimator tk2dSpriteAnimator in this.selectionIndicators)
		{
			tk2dSpriteAnimationClip clipByName = tk2dSpriteAnimator.GetClipByName(name);
			if (clipByName != null)
			{
				if (isInstant)
				{
					tk2dSpriteAnimator.PlayFromFrame(clipByName, clipByName.frames.Length - 1);
				}
				else
				{
					tk2dSpriteAnimator.Play(clipByName);
				}
			}
		}
		if (this.selectionFade)
		{
			if (this.selectionFadeInitialAlpha == null)
			{
				this.selectionFadeInitialAlpha = new float?(this.selectionFade.AlphaSelf);
			}
			this.selectionFade.FadeTo(value ? this.selectionFadeInitialAlpha.Value : 0f, isInstant ? 0f : this.selectionFadeTime, null, false, null);
		}
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
	}

	// Token: 0x06003FF8 RID: 16376 RVA: 0x0011A190 File Offset: 0x00118390
	public void Submit()
	{
		if (this.InactiveConditionText != null && !string.IsNullOrEmpty(this.InactiveConditionText()))
		{
			this.failSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			this.Failed.Invoke();
			return;
		}
		this.SubmitPressed.Invoke();
		if (this.submitEffect)
		{
			this.submitEffect.SetActive(true);
		}
		this.submitSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		this.skipNextSelectSound = false;
	}

	// Token: 0x06003FF9 RID: 16377 RVA: 0x0011A22A File Offset: 0x0011842A
	public void Cancel()
	{
		this.CancelPressed.Invoke();
		this.cancelSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		this.skipNextSelectSound = false;
	}

	// Token: 0x06003FFA RID: 16378 RVA: 0x0011A25C File Offset: 0x0011845C
	public void SubmitEffect()
	{
		if (this.submitEffect)
		{
			this.submitEffect.SetActive(true);
		}
	}

	// Token: 0x06003FFB RID: 16379 RVA: 0x0011A278 File Offset: 0x00118478
	private void UpdateAppearance()
	{
		string text = (this.InactiveConditionText != null) ? this.InactiveConditionText() : null;
		bool flag = string.IsNullOrEmpty(text);
		if (this.activeAppearance)
		{
			this.activeAppearance.SetActive(flag);
		}
		if (this.inactiveAppearance)
		{
			this.inactiveAppearance.SetActive(!flag);
		}
		if (this.inactiveMessage && !string.IsNullOrEmpty(text))
		{
			this.inactiveMessage.text = text;
		}
	}

	// Token: 0x06003FFC RID: 16380 RVA: 0x0011A2F9 File Offset: 0x001184F9
	public void SkipNextSelectSound()
	{
		this.skipNextSelectSound = true;
	}

	// Token: 0x04004198 RID: 16792
	private Func<string> _inactiveConditionText;

	// Token: 0x04004199 RID: 16793
	[SerializeField]
	private tk2dSpriteAnimator[] selectionIndicators;

	// Token: 0x0400419A RID: 16794
	[SerializeField]
	private string selectionIndicatorUpAnim = "Pointer Up";

	// Token: 0x0400419B RID: 16795
	[SerializeField]
	private string selectionIndicatorDownAnim = "Pointer Down";

	// Token: 0x0400419C RID: 16796
	[SerializeField]
	[Space]
	private NestedFadeGroupBase selectionFade;

	// Token: 0x0400419D RID: 16797
	private float? selectionFadeInitialAlpha;

	// Token: 0x0400419E RID: 16798
	[SerializeField]
	private float selectionFadeTime = 0.2f;

	// Token: 0x0400419F RID: 16799
	[SerializeField]
	private GameObject submitEffect;

	// Token: 0x040041A0 RID: 16800
	[SerializeField]
	private AudioSource audioPlayerPrefab;

	// Token: 0x040041A1 RID: 16801
	[SerializeField]
	private AudioEvent selectSound;

	// Token: 0x040041A2 RID: 16802
	[SerializeField]
	private AudioEvent submitSound;

	// Token: 0x040041A3 RID: 16803
	[SerializeField]
	private AudioEvent cancelSound;

	// Token: 0x040041A4 RID: 16804
	[SerializeField]
	private AudioEvent failSound;

	// Token: 0x040041A5 RID: 16805
	[Space]
	[SerializeField]
	private GameObject activeAppearance;

	// Token: 0x040041A6 RID: 16806
	[SerializeField]
	private GameObject inactiveAppearance;

	// Token: 0x040041A7 RID: 16807
	[SerializeField]
	private TMP_Text inactiveMessage;

	// Token: 0x040041A8 RID: 16808
	[Space]
	[FormerlySerializedAs("submitPressed")]
	public UnityEvent SubmitPressed;

	// Token: 0x040041A9 RID: 16809
	[FormerlySerializedAs("cancelPressed")]
	public UnityEvent CancelPressed;

	// Token: 0x040041AA RID: 16810
	public UnityEvent Selected;

	// Token: 0x040041AB RID: 16811
	public UnityEvent Deselected;

	// Token: 0x040041AC RID: 16812
	public UnityEvent Failed;

	// Token: 0x040041AD RID: 16813
	private bool? previouslySelected;

	// Token: 0x040041AE RID: 16814
	private bool skipNextSelectSound;
}
