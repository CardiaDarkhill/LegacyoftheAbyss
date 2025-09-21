using System;
using System.Collections;
using HKMenu;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000876 RID: 2166
	public class MenuResolutionSetting : MenuOptionHorizontal, ISubmitHandler, IEventSystemHandler, IMoveHandler, IPointerClickHandler, IMenuOptionListSetting
	{
		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x06004B59 RID: 19289 RVA: 0x001642D5 File Offset: 0x001624D5
		// (set) Token: 0x06004B5A RID: 19290 RVA: 0x001642DD File Offset: 0x001624DD
		public Resolution currentRes { get; private set; }

		// Token: 0x170008EC RID: 2284
		// (get) Token: 0x06004B5B RID: 19291 RVA: 0x001642E6 File Offset: 0x001624E6
		// (set) Token: 0x06004B5C RID: 19292 RVA: 0x001642EE File Offset: 0x001624EE
		public Resolution screenRes { get; private set; }

		// Token: 0x06004B5D RID: 19293 RVA: 0x001642F7 File Offset: 0x001624F7
		public new void OnEnable()
		{
			this.RefreshControls();
			this.UpdateApplyButton();
		}

		// Token: 0x06004B5E RID: 19294 RVA: 0x00164305 File Offset: 0x00162505
		public new void OnSubmit(BaseEventData eventData)
		{
			if (this.currentlyActiveResIndex != this.selectedOptionIndex)
			{
				base.ForceDeselect();
				this.uiAudioPlayer.PlaySubmit();
				this.ApplySettings();
			}
		}

		// Token: 0x06004B5F RID: 19295 RVA: 0x0016432C File Offset: 0x0016252C
		public new void OnMove(AxisEventData move)
		{
			if (base.MoveOption(move.moveDir))
			{
				this.UpdateApplyButton();
				return;
			}
			base.OnMove(move);
		}

		// Token: 0x06004B60 RID: 19296 RVA: 0x0016434A File Offset: 0x0016254A
		public new void OnPointerClick(PointerEventData eventData)
		{
			base.OnPointerClick(eventData);
			if (eventData.button == PointerEventData.InputButton.Left || eventData.button == PointerEventData.InputButton.Right)
			{
				this.UpdateApplyButton();
			}
		}

		// Token: 0x06004B61 RID: 19297 RVA: 0x0016436C File Offset: 0x0016256C
		public override void ApplySettings()
		{
			if (this.selectedOptionIndex >= 0)
			{
				this.previousRes = this.currentRes;
				this.previousResIndex = this.currentlyActiveResIndex;
				Resolution currentRes = this.availableResolutions[this.selectedOptionIndex];
				Screen.SetResolution(currentRes.width, currentRes.height, Screen.fullScreen, currentRes.refreshRate);
				this.currentRes = currentRes;
				this.currentlyActiveResIndex = this.selectedOptionIndex;
				base.HideApplyButton();
				UIManager.instance.UIShowResolutionPrompt(true);
			}
		}

		// Token: 0x06004B62 RID: 19298 RVA: 0x001643EF File Offset: 0x001625EF
		public override void UpdateApplyButton()
		{
			if (this.currentlyActiveResIndex == this.selectedOptionIndex)
			{
				base.HideApplyButton();
				return;
			}
			base.ShowApplyButton();
		}

		// Token: 0x06004B63 RID: 19299 RVA: 0x0016440C File Offset: 0x0016260C
		public void ResetToDefaultResolution()
		{
			Screen.SetResolution(1920, 1080, Screen.fullScreen);
			this.currentRes = Screen.currentResolution;
			base.StartCoroutine(this.RefreshOnNextFrame());
		}

		// Token: 0x06004B64 RID: 19300 RVA: 0x0016443A File Offset: 0x0016263A
		public void RefreshControls()
		{
			this.RefreshAvailableResolutions();
			this.RefreshCurrentIndex();
			this.PushUpdateOptionList();
			this.UpdateText();
		}

		// Token: 0x06004B65 RID: 19301 RVA: 0x00164454 File Offset: 0x00162654
		public void RollbackResolution()
		{
			Screen.SetResolution(this.previousRes.width, this.previousRes.height, Screen.fullScreen);
			this.currentRes = Screen.currentResolution;
			base.StartCoroutine(this.RefreshOnNextFrame());
		}

		// Token: 0x06004B66 RID: 19302 RVA: 0x00164490 File Offset: 0x00162690
		private string GetResolutionString(Resolution resolution)
		{
			return string.Format("{0} x {1} @ {2:0.##}Hz", resolution.width, resolution.height, resolution.refreshRateRatio.value);
		}

		// Token: 0x06004B67 RID: 19303 RVA: 0x001644D4 File Offset: 0x001626D4
		public override void RefreshCurrentIndex()
		{
			this.foundResolutionInList = false;
			for (int i = 0; i < this.availableResolutions.Length; i++)
			{
				if (this.currentRes.Equals(this.availableResolutions[i]))
				{
					this.selectedOptionIndex = i;
					this.currentlyActiveResIndex = i;
					this.foundResolutionInList = true;
					break;
				}
			}
			if (!this.foundResolutionInList)
			{
				Resolution[] array = new Resolution[this.availableResolutions.Length + 1];
				array[0] = this.currentRes;
				for (int j = 0; j < this.availableResolutions.Length; j++)
				{
					array[j + 1] = this.availableResolutions[j];
				}
				this.availableResolutions = array;
				this.selectedOptionIndex = 0;
				this.currentlyActiveResIndex = 0;
			}
		}

		// Token: 0x06004B68 RID: 19304 RVA: 0x0016459C File Offset: 0x0016279C
		public void PushUpdateOptionList()
		{
			string[] array = new string[this.availableResolutions.Length];
			for (int i = 0; i < this.availableResolutions.Length; i++)
			{
				Resolution resolution = this.availableResolutions[i];
				array[i] = this.GetResolutionString(resolution);
			}
			base.SetOptionList(array);
		}

		// Token: 0x06004B69 RID: 19305 RVA: 0x001645E8 File Offset: 0x001627E8
		private void RefreshAvailableResolutions()
		{
			this.screenRes = Screen.currentResolution;
			if (!Screen.fullScreen)
			{
				Resolution currentRes = new Resolution
				{
					width = Screen.width,
					height = Screen.height,
					refreshRateRatio = this.screenRes.refreshRateRatio
				};
				this.currentRes = currentRes;
			}
			else
			{
				this.currentRes = this.screenRes;
			}
			this.availableResolutions = Screen.resolutions;
		}

		// Token: 0x06004B6A RID: 19306 RVA: 0x0016465E File Offset: 0x0016285E
		private IEnumerator RefreshOnNextFrame()
		{
			yield return null;
			this.RefreshAvailableResolutions();
			this.RefreshCurrentIndex();
			this.PushUpdateOptionList();
			this.UpdateApplyButton();
			this.UpdateText();
			yield break;
		}

		// Token: 0x04004CD1 RID: 19665
		private Resolution[] availableResolutions;

		// Token: 0x04004CD2 RID: 19666
		private Resolution previousRes;

		// Token: 0x04004CD3 RID: 19667
		private bool foundResolutionInList;

		// Token: 0x04004CD4 RID: 19668
		private int currentlyActiveResIndex = -1;

		// Token: 0x04004CD5 RID: 19669
		private int previousResIndex;
	}
}
