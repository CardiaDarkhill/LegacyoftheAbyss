using System;
using System.Collections.Generic;
using GlobalEnums;
using HKMenu;
using TeamCherry.Localization;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000873 RID: 2163
	public class MenuLanguageSetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IMenuOptionListSetting, IPointerClickHandler, ISubmitHandler
	{
		// Token: 0x06004B25 RID: 19237 RVA: 0x001636B3 File Offset: 0x001618B3
		private new void OnEnable()
		{
			this.RefreshControls();
			this.UpdateAlpha();
		}

		// Token: 0x06004B26 RID: 19238 RVA: 0x001636C4 File Offset: 0x001618C4
		public void UpdateAlpha()
		{
			CanvasGroup component = base.GetComponent<CanvasGroup>();
			if (component)
			{
				if (!base.interactable)
				{
					component.alpha = 0.5f;
					return;
				}
				component.alpha = 1f;
			}
		}

		// Token: 0x06004B27 RID: 19239 RVA: 0x001636FF File Offset: 0x001618FF
		public new void OnMove(AxisEventData move)
		{
			if (!base.interactable)
			{
				return;
			}
			if (base.MoveOption(move.moveDir))
			{
				this.UpdateLanguageSetting();
				return;
			}
			base.OnMove(move);
		}

		// Token: 0x06004B28 RID: 19240 RVA: 0x00163726 File Offset: 0x00161926
		public new void OnPointerClick(PointerEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			base.PointerClickCheckArrows(eventData);
			this.UpdateLanguageSetting();
		}

		// Token: 0x06004B29 RID: 19241 RVA: 0x0016373E File Offset: 0x0016193E
		public new void OnSubmit(BaseEventData eventData)
		{
			base.MoveOption(MoveDirection.Right);
			this.UpdateLanguageSetting();
		}

		// Token: 0x06004B2A RID: 19242 RVA: 0x00163750 File Offset: 0x00161950
		public static Rect RectTransformToScreenSpace(RectTransform transform)
		{
			Vector2 vector = Vector2.Scale(transform.rect.size, transform.lossyScale);
			return new Rect(transform.position.x, (float)Screen.height - transform.position.y, vector.x, vector.y);
		}

		// Token: 0x06004B2B RID: 19243 RVA: 0x001637AA File Offset: 0x001619AA
		public void RefreshControls()
		{
			MenuLanguageSetting.RefreshAvailableLanguages();
			this.RefreshCurrentIndex();
			this.PushUpdateOptionList();
			this.UpdateText();
		}

		// Token: 0x06004B2C RID: 19244 RVA: 0x001637C3 File Offset: 0x001619C3
		private void UpdateLanguageSetting()
		{
			GameManager.instance.gameSettings.gameLanguage = MenuLanguageSetting.langs[this.selectedOptionIndex];
			Language.SwitchLanguage((LanguageCode)MenuLanguageSetting.langs[this.selectedOptionIndex]);
			this.gm.RefreshLocalization();
			this.UpdateText();
		}

		// Token: 0x06004B2D RID: 19245 RVA: 0x00163804 File Offset: 0x00161A04
		private static void RefreshAvailableLanguages()
		{
			if (GameManager.instance.gameConfig.hideLanguageOption)
			{
				if (MenuLanguageSetting.languageState != 1)
				{
					MenuLanguageSetting.langs = (Enum.GetValues(typeof(SupportedLanguages)) as SupportedLanguages[]);
					if (MenuLanguageSetting.langs != null && MenuLanguageSetting.langs.Length != 0)
					{
						MenuLanguageSetting.languageState = 1;
						MenuLanguageSetting.CreateLanguageMap();
						MenuLanguageSetting.UpdateLangsArray();
						return;
					}
					MenuLanguageSetting.languageState = 0;
					return;
				}
			}
			else if (MenuLanguageSetting.languageState != 2)
			{
				MenuLanguageSetting.langs = (Enum.GetValues(typeof(SupportedLanguages)) as SupportedLanguages[]);
				if (MenuLanguageSetting.langs != null && MenuLanguageSetting.langs.Length != 0)
				{
					MenuLanguageSetting.languageState = 2;
					MenuLanguageSetting.CreateLanguageMap();
					MenuLanguageSetting.UpdateLangsArray();
					return;
				}
				MenuLanguageSetting.languageState = 0;
			}
		}

		// Token: 0x06004B2E RID: 19246 RVA: 0x001638B4 File Offset: 0x00161AB4
		private static void CreateLanguageMap()
		{
			MenuLanguageSetting.languageCodeToSupportedLanguages.Clear();
			foreach (SupportedLanguages value in MenuLanguageSetting.langs)
			{
				LanguageCode key;
				if (Enum.TryParse<LanguageCode>(value.ToString(), out key))
				{
					MenuLanguageSetting.languageCodeToSupportedLanguages[key] = value;
				}
			}
		}

		// Token: 0x06004B2F RID: 19247 RVA: 0x00163908 File Offset: 0x00161B08
		public override void RefreshCurrentIndex()
		{
			bool flag = false;
			SupportedLanguages supportedLanguages;
			if (MenuLanguageSetting.languageCodeToSupportedLanguages.TryGetValue(Language.CurrentLanguage(), out supportedLanguages))
			{
				for (int i = 0; i < MenuLanguageSetting.langs.Length; i++)
				{
					if (supportedLanguages == MenuLanguageSetting.langs[i])
					{
						this.selectedOptionIndex = i;
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				Debug.LogError("Couldn't find currently active language");
			}
			base.RefreshCurrentIndex();
		}

		// Token: 0x06004B30 RID: 19248 RVA: 0x00163964 File Offset: 0x00161B64
		public void PushUpdateOptionList()
		{
			base.SetOptionList(MenuLanguageSetting.optionList);
		}

		// Token: 0x06004B31 RID: 19249 RVA: 0x00163974 File Offset: 0x00161B74
		private static void UpdateLangsArray()
		{
			MenuLanguageSetting.optionList = new string[MenuLanguageSetting.langs.Length];
			for (int i = 0; i < MenuLanguageSetting.langs.Length; i++)
			{
				MenuLanguageSetting.optionList[i] = MenuLanguageSetting.langs[i].ToString();
			}
		}

		// Token: 0x06004B32 RID: 19250 RVA: 0x001639C4 File Offset: 0x00161BC4
		protected override void UpdateText()
		{
			if (MenuLanguageSetting.optionList != null && this.optionText != null)
			{
				try
				{
					this.optionText.text = Language.Get("LANG_CURRENT", this.sheetTitle);
				}
				catch (Exception ex)
				{
					string[] array = new string[7];
					array[0] = this.optionText.text;
					array[1] = " : ";
					int num = 2;
					string[] array2 = MenuLanguageSetting.optionList;
					array[num] = ((array2 != null) ? array2.ToString() : null);
					array[3] = " : ";
					array[4] = this.selectedOptionIndex.ToString();
					array[5] = " ";
					int num2 = 6;
					Exception ex2 = ex;
					array[num2] = ((ex2 != null) ? ex2.ToString() : null);
					Debug.LogError(string.Concat(array));
				}
				this.optionText.GetComponent<FixVerticalAlign>().AlignText();
			}
		}

		// Token: 0x04004CB4 RID: 19636
		private static Dictionary<LanguageCode, SupportedLanguages> languageCodeToSupportedLanguages = new Dictionary<LanguageCode, SupportedLanguages>();

		// Token: 0x04004CB5 RID: 19637
		private static int languageState;

		// Token: 0x04004CB6 RID: 19638
		private static SupportedLanguages[] langs;

		// Token: 0x04004CB7 RID: 19639
		private new static string[] optionList;

		// Token: 0x04004CB8 RID: 19640
		private GameSettings gs;

		// Token: 0x04004CB9 RID: 19641
		public FixVerticalAlign textAligner;
	}
}
