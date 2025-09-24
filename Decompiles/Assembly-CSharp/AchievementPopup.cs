using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000603 RID: 1539
public class AchievementPopup : MonoBehaviour
{
	// Token: 0x140000B2 RID: 178
	// (add) Token: 0x060036F6 RID: 14070 RVA: 0x000F2544 File Offset: 0x000F0744
	// (remove) Token: 0x060036F7 RID: 14071 RVA: 0x000F257C File Offset: 0x000F077C
	public event AchievementPopup.SelfEvent OnFinish;

	// Token: 0x060036F8 RID: 14072 RVA: 0x000F25B1 File Offset: 0x000F07B1
	private void Awake()
	{
		this.group = base.GetComponent<CanvasGroup>();
	}

	// Token: 0x060036F9 RID: 14073 RVA: 0x000F25C0 File Offset: 0x000F07C0
	public void Setup(Sprite icon, string name, string description)
	{
		if (this.image)
		{
			this.image.sprite = icon;
		}
		if (this.nameText)
		{
			this.nameText.text = name;
		}
		if (this.descriptionText)
		{
			this.descriptionText.text = description;
		}
		this.sound.SpawnAndPlayOneShot(this.audioPlayerPrefab, Vector3.zero, null);
		this.currentState = AchievementPopup.FadeState.WaitAppear;
	}

	// Token: 0x060036FA RID: 14074 RVA: 0x000F2638 File Offset: 0x000F0838
	private void Update()
	{
		switch (this.currentState)
		{
		case AchievementPopup.FadeState.None:
			break;
		case AchievementPopup.FadeState.WaitAppear:
			if (this.currentState != this.previousState)
			{
				this.elapsed = 0f;
				this.previousState = this.currentState;
				this.group.alpha = 0f;
			}
			this.elapsed += Time.unscaledDeltaTime;
			if (this.elapsed >= this.appearDelay)
			{
				this.currentState = AchievementPopup.FadeState.FadeUp;
				return;
			}
			break;
		case AchievementPopup.FadeState.FadeUp:
			if (this.currentState != this.previousState)
			{
				this.elapsed = 0f;
				this.previousState = this.currentState;
			}
			this.group.alpha = Mathf.Lerp(0f, 1f, this.elapsed / this.fadeInTime);
			this.elapsed += Time.unscaledDeltaTime;
			if (this.elapsed >= this.fadeInTime)
			{
				this.group.alpha = 1f;
				this.currentState = AchievementPopup.FadeState.WaitHold;
				return;
			}
			break;
		case AchievementPopup.FadeState.WaitHold:
			if (this.currentState != this.previousState)
			{
				this.elapsed = 0f;
				this.previousState = this.currentState;
			}
			this.elapsed += Time.unscaledDeltaTime;
			if (this.elapsed >= this.holdTime)
			{
				this.currentState = AchievementPopup.FadeState.FadeDown;
				return;
			}
			break;
		case AchievementPopup.FadeState.FadeDown:
			if (this.currentState != this.previousState)
			{
				this.elapsed = 0f;
				this.previousState = this.currentState;
				if (this.fluerAnimator)
				{
					this.fluerAnimator.Play(this.fluerCloseName);
				}
			}
			this.group.alpha = Mathf.Lerp(1f, 0f, this.elapsed / this.fadeOutTime);
			this.elapsed += Time.unscaledDeltaTime;
			if (this.elapsed >= this.fadeOutTime)
			{
				this.group.alpha = 0f;
				this.currentState = AchievementPopup.FadeState.Finish;
				return;
			}
			break;
		case AchievementPopup.FadeState.Finish:
			if (this.currentState != this.previousState)
			{
				this.previousState = this.currentState;
				if (this.OnFinish != null)
				{
					this.OnFinish(this);
					return;
				}
				base.gameObject.SetActive(false);
				return;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x040039BA RID: 14778
	public Image image;

	// Token: 0x040039BB RID: 14779
	public Text nameText;

	// Token: 0x040039BC RID: 14780
	public Text descriptionText;

	// Token: 0x040039BD RID: 14781
	private CanvasGroup group;

	// Token: 0x040039BE RID: 14782
	[Space]
	public float appearDelay;

	// Token: 0x040039BF RID: 14783
	public float fadeInTime = 0.25f;

	// Token: 0x040039C0 RID: 14784
	public float holdTime = 3f;

	// Token: 0x040039C1 RID: 14785
	public float fadeOutTime = 0.5f;

	// Token: 0x040039C2 RID: 14786
	[Space]
	public AudioSource audioPlayerPrefab;

	// Token: 0x040039C3 RID: 14787
	public AudioEvent sound;

	// Token: 0x040039C4 RID: 14788
	[Space]
	public Animator fluerAnimator;

	// Token: 0x040039C5 RID: 14789
	public string fluerCloseName = "Close";

	// Token: 0x040039C6 RID: 14790
	private AchievementPopup.FadeState currentState;

	// Token: 0x040039C7 RID: 14791
	private AchievementPopup.FadeState previousState;

	// Token: 0x040039C8 RID: 14792
	private float elapsed;

	// Token: 0x0200190D RID: 6413
	// (Invoke) Token: 0x06009300 RID: 37632
	public delegate void SelfEvent(AchievementPopup sender);

	// Token: 0x0200190E RID: 6414
	private enum FadeState
	{
		// Token: 0x04009430 RID: 37936
		None,
		// Token: 0x04009431 RID: 37937
		WaitAppear,
		// Token: 0x04009432 RID: 37938
		FadeUp,
		// Token: 0x04009433 RID: 37939
		WaitHold,
		// Token: 0x04009434 RID: 37940
		FadeDown,
		// Token: 0x04009435 RID: 37941
		Finish
	}
}
