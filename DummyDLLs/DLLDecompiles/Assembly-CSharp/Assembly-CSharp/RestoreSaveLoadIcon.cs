using System;
using System.Collections;
using System.Collections.Generic;
using Coffee.UISoftMask;
using UnityEngine;

// Token: 0x02000707 RID: 1799
public sealed class RestoreSaveLoadIcon : MonoBehaviour
{
	// Token: 0x0600402D RID: 16429 RVA: 0x0011A940 File Offset: 0x00118B40
	private void Awake()
	{
		this.softMasks.RemoveAll((SoftMask o) => o == null);
		foreach (SoftMask softMask in this.softMasks)
		{
			RestoreSaveLoadIcon.ThrobberControl item = new RestoreSaveLoadIcon.ThrobberControl
			{
				softMask = softMask
			};
			this.throbberControls.Add(item);
		}
		if (this.fadeState != RestoreSaveLoadIcon.FadeState.FadeIn)
		{
			this.canvasGroup.alpha = 0f;
		}
	}

	// Token: 0x0600402E RID: 16430 RVA: 0x0011A9EC File Offset: 0x00118BEC
	private void OnValidate()
	{
		if (this.canvasGroup == null)
		{
			this.canvasGroup = base.GetComponent<CanvasGroup>();
		}
	}

	// Token: 0x0600402F RID: 16431 RVA: 0x0011AA08 File Offset: 0x00118C08
	private void OnEnable()
	{
		UIManager instance = UIManager.instance;
		if (instance)
		{
			this.menuFadeSpeed = instance.MENU_FADE_SPEED;
		}
		if (this.fadeInOnEnable || this.fadeInQueued)
		{
			this.StartFadeIn();
		}
		foreach (RestoreSaveLoadIcon.ThrobberControl throbberControl in this.throbberControls)
		{
			throbberControl.Reset(this.startFromZero);
		}
		this.currentThrobberIndex = 0;
		this.throbTimer = 0f;
	}

	// Token: 0x06004030 RID: 16432 RVA: 0x0011AAA0 File Offset: 0x00118CA0
	private void Update()
	{
		if (this.throbberControls.Count == 0)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		foreach (RestoreSaveLoadIcon.ThrobberControl throbberControl in this.throbberControls)
		{
			throbberControl.UpdateThrob(deltaTime, this.throbFromZero);
		}
		this.throbTimer += deltaTime;
		if (this.throbTimer >= this.throbInterval)
		{
			this.throbTimer = 0f;
			this.throbberControls[this.currentThrobberIndex].StartThrob(this.throbDuration);
			this.currentThrobberIndex = (this.currentThrobberIndex + 1) % this.throbberControls.Count;
		}
	}

	// Token: 0x06004031 RID: 16433 RVA: 0x0011AB68 File Offset: 0x00118D68
	public void StartFadeIn()
	{
		if (this.fadeState == RestoreSaveLoadIcon.FadeState.FadeIn)
		{
			return;
		}
		this.fadeState = RestoreSaveLoadIcon.FadeState.FadeIn;
		base.gameObject.SetActive(true);
		if (!base.gameObject.activeInHierarchy)
		{
			this.fadeState = RestoreSaveLoadIcon.FadeState.None;
			this.fadeInQueued = true;
			return;
		}
		this.fadeInQueued = false;
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeInRoutine());
	}

	// Token: 0x06004032 RID: 16434 RVA: 0x0011ABDC File Offset: 0x00118DDC
	public void StartFadeOut()
	{
		if (this.fadeState == RestoreSaveLoadIcon.FadeState.FadeOut)
		{
			return;
		}
		this.fadeState = RestoreSaveLoadIcon.FadeState.FadeOut;
		this.fadeInQueued = false;
		if (!base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(false);
			this.canvasGroup.alpha = 0f;
			return;
		}
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
		}
		this.fadeRoutine = base.StartCoroutine(this.FadeOutRoutine());
	}

	// Token: 0x06004033 RID: 16435 RVA: 0x0011AC51 File Offset: 0x00118E51
	public void HideInstant()
	{
		this.fadeState = RestoreSaveLoadIcon.FadeState.None;
		this.canvasGroup.alpha = 0f;
		base.gameObject.SetActive(false);
	}

	// Token: 0x06004034 RID: 16436 RVA: 0x0011AC76 File Offset: 0x00118E76
	private IEnumerator FadeInRoutine()
	{
		this.fadeState = RestoreSaveLoadIcon.FadeState.FadeIn;
		float loopFailsafe = 0f;
		while (this.canvasGroup.alpha < 1f)
		{
			this.canvasGroup.alpha += Time.unscaledDeltaTime * this.menuFadeSpeed;
			loopFailsafe += Time.unscaledDeltaTime;
			if (this.canvasGroup.alpha >= 0.95f)
			{
				this.canvasGroup.alpha = 1f;
				break;
			}
			if (loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		this.canvasGroup.alpha = 1f;
		yield break;
	}

	// Token: 0x06004035 RID: 16437 RVA: 0x0011AC85 File Offset: 0x00118E85
	private IEnumerator FadeOutRoutine()
	{
		this.fadeState = RestoreSaveLoadIcon.FadeState.FadeOut;
		float loopFailsafe = 0f;
		while (this.canvasGroup.alpha > 0.05f)
		{
			this.canvasGroup.alpha -= Time.unscaledDeltaTime * this.menuFadeSpeed;
			loopFailsafe += Time.unscaledDeltaTime;
			if (this.canvasGroup.alpha <= 0.05f || loopFailsafe >= 2f)
			{
				break;
			}
			yield return null;
		}
		this.canvasGroup.alpha = 0f;
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06004036 RID: 16438 RVA: 0x0011AC94 File Offset: 0x00118E94
	[ContextMenu("Gather SoftMasks")]
	private void GatherSoftMasks()
	{
		this.softMasks.Clear();
		this.softMasks.AddRange(base.gameObject.GetComponentsInChildren<SoftMask>(true));
	}

	// Token: 0x040041D0 RID: 16848
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x040041D1 RID: 16849
	[SerializeField]
	private List<SoftMask> softMasks = new List<SoftMask>();

	// Token: 0x040041D2 RID: 16850
	[SerializeField]
	private bool fadeInOnEnable = true;

	// Token: 0x040041D3 RID: 16851
	[SerializeField]
	private float fadeInDuration = 0.5f;

	// Token: 0x040041D4 RID: 16852
	[SerializeField]
	private float fadeOutDuration = 0.5f;

	// Token: 0x040041D5 RID: 16853
	[SerializeField]
	private float throbInterval = 0.125f;

	// Token: 0x040041D6 RID: 16854
	[SerializeField]
	private float throbDuration = 0.5f;

	// Token: 0x040041D7 RID: 16855
	[SerializeField]
	private bool startFromZero;

	// Token: 0x040041D8 RID: 16856
	[SerializeField]
	private bool throbFromZero = true;

	// Token: 0x040041D9 RID: 16857
	private List<RestoreSaveLoadIcon.ThrobberControl> throbberControls = new List<RestoreSaveLoadIcon.ThrobberControl>();

	// Token: 0x040041DA RID: 16858
	private int currentThrobberIndex;

	// Token: 0x040041DB RID: 16859
	private float throbTimer;

	// Token: 0x040041DC RID: 16860
	private Coroutine fadeRoutine;

	// Token: 0x040041DD RID: 16861
	private RestoreSaveLoadIcon.FadeState fadeState;

	// Token: 0x040041DE RID: 16862
	private float menuFadeSpeed = 3.2f;

	// Token: 0x040041DF RID: 16863
	private bool fadeInQueued;

	// Token: 0x020019F1 RID: 6641
	private sealed class ThrobberControl
	{
		// Token: 0x06009587 RID: 38279 RVA: 0x002A5B37 File Offset: 0x002A3D37
		public void Reset(bool fromZero)
		{
			this.alpha = (fromZero ? 0f : 1f);
			this.softMask.alpha = this.alpha;
			this.isThrobbing = false;
		}

		// Token: 0x06009588 RID: 38280 RVA: 0x002A5B66 File Offset: 0x002A3D66
		public void StartThrob(float duration)
		{
			this.fadeDuration = duration;
			this.fadeElapsed = 0f;
			this.isThrobbing = true;
		}

		// Token: 0x06009589 RID: 38281 RVA: 0x002A5B84 File Offset: 0x002A3D84
		public void UpdateThrob(float deltaTime, bool fromZero)
		{
			if (!this.isThrobbing)
			{
				return;
			}
			this.fadeElapsed += deltaTime;
			this.alpha = Mathf.Lerp(fromZero ? 0f : 1f, fromZero ? 1f : 0f, this.fadeElapsed / this.fadeDuration);
			this.softMask.alpha = this.alpha;
			if (this.fadeElapsed >= this.fadeDuration)
			{
				this.isThrobbing = false;
			}
		}

		// Token: 0x170010E1 RID: 4321
		// (get) Token: 0x0600958A RID: 38282 RVA: 0x002A5C04 File Offset: 0x002A3E04
		public bool IsThrobbing
		{
			get
			{
				return this.isThrobbing;
			}
		}

		// Token: 0x040097D3 RID: 38867
		public SoftMask softMask;

		// Token: 0x040097D4 RID: 38868
		public float alpha;

		// Token: 0x040097D5 RID: 38869
		private float fadeDuration;

		// Token: 0x040097D6 RID: 38870
		private float fadeElapsed;

		// Token: 0x040097D7 RID: 38871
		private bool isThrobbing;
	}

	// Token: 0x020019F2 RID: 6642
	private enum FadeState
	{
		// Token: 0x040097D9 RID: 38873
		None,
		// Token: 0x040097DA RID: 38874
		FadeIn,
		// Token: 0x040097DB RID: 38875
		FadeOut
	}
}
