using System;
using System.Collections;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200043B RID: 1083
public sealed class LanguageSelector : MonoBehaviour
{
	// Token: 0x170003D8 RID: 984
	// (get) Token: 0x06002544 RID: 9540 RVA: 0x000AAFFE File Offset: 0x000A91FE
	public CanvasGroup CanvasGroup
	{
		get
		{
			return this.canvasGroup;
		}
	}

	// Token: 0x170003D9 RID: 985
	// (get) Token: 0x06002545 RID: 9541 RVA: 0x000AB006 File Offset: 0x000A9206
	public PreselectOption PreselectionOption
	{
		get
		{
			return this.preselectOption;
		}
	}

	// Token: 0x170003DA RID: 986
	// (get) Token: 0x06002546 RID: 9542 RVA: 0x000AB00E File Offset: 0x000A920E
	public CanvasGroup LanguageConfirm
	{
		get
		{
			return this.languageConfirm;
		}
	}

	// Token: 0x170003DB RID: 987
	// (get) Token: 0x06002547 RID: 9543 RVA: 0x000AB016 File Offset: 0x000A9216
	public MenuButton SubmitButton
	{
		get
		{
			return this.submitButton;
		}
	}

	// Token: 0x170003DC RID: 988
	// (get) Token: 0x06002548 RID: 9544 RVA: 0x000AB01E File Offset: 0x000A921E
	public MenuButton CancelButton
	{
		get
		{
			return this.cancelButton;
		}
	}

	// Token: 0x06002549 RID: 9545 RVA: 0x000AB026 File Offset: 0x000A9226
	private void Awake()
	{
		this.hasPreselection = this.preselectOption;
	}

	// Token: 0x0600254A RID: 9546 RVA: 0x000AB039 File Offset: 0x000A9239
	public void SetCamera(Camera camera)
	{
		if (this.canvas)
		{
			this.canvas.worldCamera = camera;
		}
	}

	// Token: 0x0600254B RID: 9547 RVA: 0x000AB054 File Offset: 0x000A9254
	public IEnumerator DoLanguageSelect()
	{
		yield return this.ShowLanguageSelect();
		while (!this.confirmedLanguage)
		{
			yield return null;
		}
		yield return this.LanguageSettingDone();
		yield break;
	}

	// Token: 0x0600254C RID: 9548 RVA: 0x000AB063 File Offset: 0x000A9263
	public void SetLastSelection(Selectable selectable)
	{
		if (this.hasPreselection && selectable)
		{
			this.preselectOption.itemToHighlight = selectable;
		}
	}

	// Token: 0x0600254D RID: 9549 RVA: 0x000AB081 File Offset: 0x000A9281
	public void SetLanguage(LanguageSelectionButton languageButton)
	{
		if (languageButton)
		{
			this.SetLastSelection(languageButton);
			this.SetLanguage(languageButton.Language);
		}
	}

	// Token: 0x0600254E RID: 9550 RVA: 0x000AB0A0 File Offset: 0x000A92A0
	public void SetLanguage(string newLanguage)
	{
		this.oldLanguage = Language.CurrentLanguage().ToString();
		this.selectedLanguage = newLanguage;
		Language.SwitchLanguage(this.selectedLanguage);
		this.languageConfirm.gameObject.SetActive(true);
		this.CancelFade();
		this.fadeRoutine = base.StartCoroutine(this.FadeIn(this.languageConfirm, 0.25f));
		AutoLocalizeTextUI[] componentsInChildren = this.languageConfirm.GetComponentsInChildren<AutoLocalizeTextUI>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RefreshTextFromLocalization();
		}
	}

	// Token: 0x0600254F RID: 9551 RVA: 0x000AB12F File Offset: 0x000A932F
	private void CancelFade()
	{
		if (this.fadeRoutine != null)
		{
			base.StopCoroutine(this.fadeRoutine);
			this.fadeRoutine = null;
		}
	}

	// Token: 0x06002550 RID: 9552 RVA: 0x000AB14C File Offset: 0x000A934C
	private IEnumerator FadeIn(CanvasGroup group, float duration)
	{
		group.alpha = 0f;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			group.alpha = elapsed / duration;
			yield return new WaitForEndOfFrame();
		}
		group.alpha = 1f;
		PreselectOption component = group.GetComponent<PreselectOption>();
		if (component)
		{
			component.HighlightDefault(true);
		}
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x06002551 RID: 9553 RVA: 0x000AB169 File Offset: 0x000A9369
	private IEnumerator FadeOut(CanvasGroup group, float duration)
	{
		group.alpha = 1f;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			group.alpha = 1f - elapsed / duration;
			yield return new WaitForEndOfFrame();
		}
		group.alpha = 0f;
		group.gameObject.SetActive(false);
		this.fadeRoutine = null;
		yield break;
	}

	// Token: 0x06002552 RID: 9554 RVA: 0x000AB188 File Offset: 0x000A9388
	public void ConfirmLanguage()
	{
		Platform.Current.LocalSharedData.SetInt("GameLangSet", 1);
		Platform.Current.LocalSharedData.Save();
		this.CancelFade();
		this.fadeRoutine = base.StartCoroutine(this.FadeOut(this.languageConfirm, 0.25f));
		this.confirmedLanguage = true;
	}

	// Token: 0x06002553 RID: 9555 RVA: 0x000AB1E4 File Offset: 0x000A93E4
	public void CancelLanguage()
	{
		Language.SwitchLanguage(this.oldLanguage);
		this.CancelFade();
		this.fadeRoutine = base.StartCoroutine(this.FadeOut(this.languageConfirm, 0.25f));
		if (this.hasPreselection)
		{
			this.PreselectionOption.HighlightDefault(true);
		}
	}

	// Token: 0x06002554 RID: 9556 RVA: 0x000AB234 File Offset: 0x000A9434
	private IEnumerator ShowLanguageSelect()
	{
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.gameObject.SetActive(true);
		while ((double)this.canvasGroup.alpha < 0.99)
		{
			this.canvasGroup.alpha += Time.smoothDeltaTime * 1.6f;
			if ((double)this.canvasGroup.alpha > 0.99)
			{
				this.canvasGroup.alpha = 1f;
			}
			yield return null;
		}
		Cursor.lockState = CursorLockMode.None;
		this.PreselectionOption.HighlightDefault(false);
		yield return null;
		yield break;
	}

	// Token: 0x06002555 RID: 9557 RVA: 0x000AB243 File Offset: 0x000A9443
	private IEnumerator LanguageSettingDone()
	{
		Cursor.lockState = CursorLockMode.Locked;
		while ((double)this.canvasGroup.alpha > 0.01)
		{
			this.canvasGroup.alpha -= Time.smoothDeltaTime * 1.6f;
			if ((double)this.canvasGroup.alpha < 0.01)
			{
				this.canvasGroup.alpha = 0f;
			}
			yield return null;
		}
		this.canvasGroup.gameObject.SetActive(false);
		ConfigManager.SaveConfig();
		yield break;
	}

	// Token: 0x04002300 RID: 8960
	[SerializeField]
	private Canvas canvas;

	// Token: 0x04002301 RID: 8961
	[SerializeField]
	private GameObject eventSystemObject;

	// Token: 0x04002302 RID: 8962
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x04002303 RID: 8963
	[SerializeField]
	private PreselectOption preselectOption;

	// Token: 0x04002304 RID: 8964
	[SerializeField]
	private CanvasGroup languageConfirm;

	// Token: 0x04002305 RID: 8965
	[SerializeField]
	private MenuButton submitButton;

	// Token: 0x04002306 RID: 8966
	[SerializeField]
	private MenuButton cancelButton;

	// Token: 0x04002307 RID: 8967
	private GameObject lastSelection;

	// Token: 0x04002308 RID: 8968
	private const float FADE_SPEED = 1.6f;

	// Token: 0x04002309 RID: 8969
	private string selectedLanguage;

	// Token: 0x0400230A RID: 8970
	private string oldLanguage;

	// Token: 0x0400230B RID: 8971
	private bool confirmedLanguage;

	// Token: 0x0400230C RID: 8972
	private Coroutine fadeRoutine;

	// Token: 0x0400230D RID: 8973
	private bool hasPreselection;
}
