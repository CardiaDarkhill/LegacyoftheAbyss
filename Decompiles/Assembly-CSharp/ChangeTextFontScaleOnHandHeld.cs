using System;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200040A RID: 1034
public sealed class ChangeTextFontScaleOnHandHeld : MonoBehaviour
{
	// Token: 0x0600231E RID: 8990 RVA: 0x000A0AD4 File Offset: 0x0009ECD4
	private void Awake()
	{
		this.hasText = (this.text != null);
		if (!this.hasText)
		{
			this.text = base.GetComponent<Text>();
			this.hasText = (this.text != null);
		}
		if (!this.hasText)
		{
			this.hasTmpText = (this.tmpText != null);
			if (!this.hasTmpText)
			{
				this.tmpText = base.GetComponent<TMP_Text>();
				this.hasTmpText = (this.tmpText != null);
			}
		}
		if (!this.hasTmpText && !this.hasText)
		{
			base.enabled = false;
			return;
		}
		this.CreateLookup(true);
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x000A0B78 File Offset: 0x0009ED78
	private void Start()
	{
		this.hasStarted = true;
		this.DoUpdate();
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000A0B88 File Offset: 0x0009ED88
	private void OnValidate()
	{
		if (this.text == null)
		{
			this.text = base.GetComponent<Text>();
		}
		if (this.tmpText == null)
		{
			this.tmpText = base.GetComponent<TMP_Text>();
		}
		if (Application.isPlaying && this.hasStarted)
		{
			this.CreateLookup(false);
		}
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000A0BE0 File Offset: 0x0009EDE0
	private void OnEnable()
	{
		if (this.hasStarted)
		{
			this.DoUpdate();
		}
		Platform.Current.OnScreenModeChanged += this.OnScreenModeChanged;
		GameManager instance = GameManager.instance;
		if (instance != null)
		{
			instance.RefreshLanguageText += this.DoUpdate;
		}
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000A0C34 File Offset: 0x0009EE34
	private void OnDisable()
	{
		Platform.Current.OnScreenModeChanged -= this.OnScreenModeChanged;
		GameManager instance = GameManager.instance;
		if (instance != null)
		{
			instance.RefreshLanguageText -= this.DoUpdate;
		}
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000A0C78 File Offset: 0x0009EE78
	private void CreateLookup(bool log = true)
	{
		foreach (ChangeTextFontScaleOnHandHeld.Override @override in this.languageOverrides)
		{
			LanguageCode languageCode = (LanguageCode)@override.languageCode;
			if (!this.languageCodeOverrides.ContainsKey(languageCode))
			{
				this.languageCodeOverrides[languageCode] = @override;
			}
		}
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x000A0CE8 File Offset: 0x0009EEE8
	public void DoUpdate()
	{
		if (!this.hasStarted)
		{
			return;
		}
		bool scale = false;
		if (Platform.Current.IsRunningOnHandHeld)
		{
			scale = true;
		}
		this.SetScale(scale);
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000A0D18 File Offset: 0x0009EF18
	private void SetScale(bool isHandHeld)
	{
		float num = isHandHeld ? this.handHeldSize : this.normalSize;
		ChangeTextFontScaleOnHandHeld.Override @override;
		if (this.languageCodeOverrides.TryGetValue(Language.CurrentLanguage(), out @override))
		{
			num = (isHandHeld ? @override.handHeldSize : @override.normalSize);
		}
		if (this.hasText)
		{
			this.text.fontSize = (int)num;
		}
		if (this.hasTmpText)
		{
			this.tmpText.fontSize = num;
		}
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000A0D86 File Offset: 0x0009EF86
	public void ChangeLanguage(LanguageCode languageCode)
	{
		this.DoUpdate();
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000A0D8E File Offset: 0x0009EF8E
	private void OnScreenModeChanged(Platform.ScreenModeState screenMode)
	{
		this.SetScale(screenMode >= Platform.ScreenModeState.HandHeld);
	}

	// Token: 0x040021CB RID: 8651
	[SerializeField]
	private Text text;

	// Token: 0x040021CC RID: 8652
	[SerializeField]
	private TMP_Text tmpText;

	// Token: 0x040021CD RID: 8653
	[SerializeField]
	private float normalSize;

	// Token: 0x040021CE RID: 8654
	[SerializeField]
	private float handHeldSize;

	// Token: 0x040021CF RID: 8655
	[SerializeField]
	private List<ChangeTextFontScaleOnHandHeld.Override> languageOverrides = new List<ChangeTextFontScaleOnHandHeld.Override>();

	// Token: 0x040021D0 RID: 8656
	private Dictionary<LanguageCode, ChangeTextFontScaleOnHandHeld.Override> languageCodeOverrides = new Dictionary<LanguageCode, ChangeTextFontScaleOnHandHeld.Override>();

	// Token: 0x040021D1 RID: 8657
	private bool hasStarted;

	// Token: 0x040021D2 RID: 8658
	private bool hasText;

	// Token: 0x040021D3 RID: 8659
	private bool hasTmpText;

	// Token: 0x020016A1 RID: 5793
	[Serializable]
	private struct Override
	{
		// Token: 0x04008B90 RID: 35728
		public SupportedLanguages languageCode;

		// Token: 0x04008B91 RID: 35729
		public float normalSize;

		// Token: 0x04008B92 RID: 35730
		public float handHeldSize;
	}
}
