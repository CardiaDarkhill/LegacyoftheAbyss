using System;
using TeamCherry.Localization;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000874 RID: 2164
	public class MenuOptionHorizontal : MenuSelectable, IMoveHandler, IEventSystemHandler, IPointerClickHandler, ISubmitHandler
	{
		// Token: 0x06004B35 RID: 19253 RVA: 0x00163AA8 File Offset: 0x00161CA8
		private new void Awake()
		{
			this.gm = GameManager.instance;
			this.hasApplyButton = (this.applyButton != null);
		}

		// Token: 0x06004B36 RID: 19254 RVA: 0x00163AC7 File Offset: 0x00161CC7
		private new void OnEnable()
		{
			this.gm.RefreshLanguageText += this.UpdateText;
			this.RefreshMenuControls();
			this.UpdateApplyButton();
		}

		// Token: 0x06004B37 RID: 19255 RVA: 0x00163AED File Offset: 0x00161CED
		private new void OnDisable()
		{
			this.gm.RefreshLanguageText -= this.UpdateText;
		}

		// Token: 0x06004B38 RID: 19256 RVA: 0x00163B07 File Offset: 0x00161D07
		public new void OnMove(AxisEventData move)
		{
			if (!base.interactable)
			{
				return;
			}
			if (!this.MoveOption(move.moveDir))
			{
				base.OnMove(move);
			}
		}

		// Token: 0x06004B39 RID: 19257 RVA: 0x00163B27 File Offset: 0x00161D27
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			this.PointerClickCheckArrows(eventData);
		}

		// Token: 0x06004B3A RID: 19258 RVA: 0x00163B39 File Offset: 0x00161D39
		public void OnSubmit(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Submit)
			{
				this.ApplySettings();
				return;
			}
			this.MoveOption(MoveDirection.Right);
		}

		// Token: 0x06004B3B RID: 19259 RVA: 0x00163B5C File Offset: 0x00161D5C
		protected bool MoveOption(MoveDirection dir)
		{
			if (dir == MoveDirection.Right)
			{
				this.IncrementOption();
			}
			else
			{
				if (dir != MoveDirection.Left)
				{
					return false;
				}
				this.DecrementOption();
			}
			if (this.uiAudioPlayer)
			{
				this.uiAudioPlayer.PlaySlider();
			}
			return true;
		}

		// Token: 0x06004B3C RID: 19260 RVA: 0x00163B90 File Offset: 0x00161D90
		protected void PointerClickCheckArrows(PointerEventData eventData)
		{
			if (this.leftCursor && this.IsInside(this.leftCursor.gameObject, eventData))
			{
				this.MoveOption(MoveDirection.Left);
				return;
			}
			if (this.rightCursor && this.IsInside(this.rightCursor.gameObject, eventData))
			{
				this.MoveOption(MoveDirection.Right);
				return;
			}
			this.MoveOption(MoveDirection.Right);
		}

		// Token: 0x06004B3D RID: 19261 RVA: 0x00163BFC File Offset: 0x00161DFC
		private bool IsInside(GameObject obj, PointerEventData eventData)
		{
			RectTransform component = obj.GetComponent<RectTransform>();
			return component && RectTransformUtility.RectangleContainsScreenPoint(component, eventData.position, Camera.main);
		}

		// Token: 0x06004B3E RID: 19262 RVA: 0x00163C2E File Offset: 0x00161E2E
		public void SetOptionList(string[] optionList)
		{
			this.optionList = optionList;
		}

		// Token: 0x06004B3F RID: 19263 RVA: 0x00163C37 File Offset: 0x00161E37
		public string GetSelectedOptionText()
		{
			if (this.localizeText)
			{
				return Language.Get(this.optionList[this.selectedOptionIndex].ToString(), this.sheetTitle);
			}
			return this.optionList[this.selectedOptionIndex].ToString();
		}

		// Token: 0x06004B40 RID: 19264 RVA: 0x00163C71 File Offset: 0x00161E71
		public string GetSelectedOptionTextRaw()
		{
			return this.optionList[this.selectedOptionIndex].ToString();
		}

		// Token: 0x06004B41 RID: 19265 RVA: 0x00163C88 File Offset: 0x00161E88
		public virtual void SetOptionTo(int optionNumber)
		{
			if (optionNumber >= 0 && optionNumber < this.optionList.Length)
			{
				this.selectedOptionIndex = optionNumber;
				this.UpdateText();
				return;
			}
			Debug.LogErrorFormat("{0} - Trying to select an option outside the list size (index: {1} listsize: {2})", new object[]
			{
				base.name,
				optionNumber,
				this.optionList.Length
			});
		}

		// Token: 0x06004B42 RID: 19266 RVA: 0x00163CE4 File Offset: 0x00161EE4
		protected virtual void UpdateText()
		{
			if (this.optionList != null && this.optionText != null)
			{
				try
				{
					if (this.localizeText)
					{
						this.optionText.text = Language.Get(this.optionList[this.selectedOptionIndex].ToString(), this.sheetTitle);
					}
					else
					{
						this.optionText.text = this.optionList[this.selectedOptionIndex].ToString();
					}
				}
				catch (Exception ex)
				{
					string[] array = new string[7];
					array[0] = this.optionText.text;
					array[1] = " : ";
					int num = 2;
					string[] array2 = this.optionList;
					array[num] = ((array2 != null) ? array2.ToString() : null);
					array[3] = " : ";
					array[4] = this.selectedOptionIndex.ToString();
					array[5] = " ";
					int num2 = 6;
					Exception ex2 = ex;
					array[num2] = ((ex2 != null) ? ex2.ToString() : null);
					Debug.LogError(string.Concat(array));
				}
				FixVerticalAlign component = this.optionText.GetComponent<FixVerticalAlign>();
				if (component)
				{
					component.AlignText();
				}
			}
		}

		// Token: 0x06004B43 RID: 19267 RVA: 0x00163DF4 File Offset: 0x00161FF4
		protected void UpdateSetting()
		{
			if (this.menuSetting)
			{
				this.menuSetting.ChangeSetting(this.selectedOptionIndex);
			}
		}

		// Token: 0x06004B44 RID: 19268 RVA: 0x00163E14 File Offset: 0x00162014
		protected void DecrementOption()
		{
			if (this.selectedOptionIndex > 0)
			{
				this.selectedOptionIndex--;
				if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
				{
					this.UpdateSetting();
				}
				this.UpdateText();
			}
			else if (this.selectedOptionIndex == 0)
			{
				this.selectedOptionIndex = this.optionList.Length - 1;
				if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
				{
					this.UpdateSetting();
				}
				this.UpdateText();
			}
			this.UpdateApplyButton();
		}

		// Token: 0x06004B45 RID: 19269 RVA: 0x00163E80 File Offset: 0x00162080
		protected void IncrementOption()
		{
			if (this.selectedOptionIndex >= 0 && this.selectedOptionIndex < this.optionList.Length - 1)
			{
				this.selectedOptionIndex++;
				if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
				{
					this.UpdateSetting();
				}
				this.UpdateText();
			}
			else if (this.selectedOptionIndex == this.optionList.Length - 1)
			{
				this.selectedOptionIndex = 0;
				if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
				{
					this.UpdateSetting();
				}
				this.UpdateText();
			}
			this.UpdateApplyButton();
		}

		// Token: 0x06004B46 RID: 19270 RVA: 0x00163EFF File Offset: 0x001620FF
		public void RefreshMenuControls()
		{
			this.RefreshCurrentIndex();
			this.UpdateText();
		}

		// Token: 0x06004B47 RID: 19271 RVA: 0x00163F0D File Offset: 0x0016210D
		public virtual void ApplySettings()
		{
			if (this.selectedOptionIndex >= 0)
			{
				this.UpdateSetting();
				this.RefreshCurrentIndex();
				this.HideApplyButton();
			}
		}

		// Token: 0x06004B48 RID: 19272 RVA: 0x00163F2A File Offset: 0x0016212A
		public virtual void UpdateApplyButton()
		{
			if (this.currentActiveIndex == this.selectedOptionIndex)
			{
				this.HideApplyButton();
				return;
			}
			this.ShowApplyButton();
		}

		// Token: 0x06004B49 RID: 19273 RVA: 0x00163F47 File Offset: 0x00162147
		public virtual void RefreshCurrentIndex()
		{
			if (this.menuSetting != null)
			{
				this.menuSetting.RefreshValueFromGameSettings(false);
			}
			this.currentActiveIndex = this.selectedOptionIndex;
		}

		// Token: 0x06004B4A RID: 19274 RVA: 0x00163F6F File Offset: 0x0016216F
		protected void HideApplyButton()
		{
			if (!this.hasApplyButton)
			{
				return;
			}
			this.applyButton.alpha = 0f;
			this.applyButton.interactable = false;
			this.applyButton.blocksRaycasts = false;
		}

		// Token: 0x06004B4B RID: 19275 RVA: 0x00163FA2 File Offset: 0x001621A2
		protected void ShowApplyButton()
		{
			if (this.applySettingOn == MenuOptionHorizontal.ApplyOnType.Scroll)
			{
				return;
			}
			if (!this.hasApplyButton)
			{
				return;
			}
			this.applyButton.alpha = 1f;
			this.applyButton.interactable = true;
			this.applyButton.blocksRaycasts = true;
		}

		// Token: 0x04004CBA RID: 19642
		[Header("Option List Settings")]
		public Text optionText;

		// Token: 0x04004CBB RID: 19643
		public string[] optionList;

		// Token: 0x04004CBC RID: 19644
		public int selectedOptionIndex;

		// Token: 0x04004CBD RID: 19645
		public MenuSetting menuSetting;

		// Token: 0x04004CBE RID: 19646
		[Header("Interaction")]
		public MenuOptionHorizontal.ApplyOnType applySettingOn;

		// Token: 0x04004CBF RID: 19647
		public CanvasGroup applyButton;

		// Token: 0x04004CC0 RID: 19648
		[Header("Localization")]
		public bool localizeText;

		// Token: 0x04004CC1 RID: 19649
		public string sheetTitle;

		// Token: 0x04004CC2 RID: 19650
		protected GameManager gm;

		// Token: 0x04004CC3 RID: 19651
		private bool hasApplyButton;

		// Token: 0x04004CC4 RID: 19652
		private int currentActiveIndex;

		// Token: 0x02001ADE RID: 6878
		public enum ApplyOnType
		{
			// Token: 0x04009AC0 RID: 39616
			Scroll,
			// Token: 0x04009AC1 RID: 39617
			Submit
		}
	}
}
