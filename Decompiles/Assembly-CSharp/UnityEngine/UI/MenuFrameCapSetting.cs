using System;
using System.Collections.Generic;
using HKMenu;
using TeamCherry.Localization;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000872 RID: 2162
	public class MenuFrameCapSetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IMenuOptionListSetting, IPointerClickHandler, ISubmitHandler
	{
		// Token: 0x06004B17 RID: 19223 RVA: 0x0016327A File Offset: 0x0016147A
		private new void Awake()
		{
			this.Init();
			this.PushUpdateOptionList();
		}

		// Token: 0x06004B18 RID: 19224 RVA: 0x00163288 File Offset: 0x00161488
		public new void OnEnable()
		{
			this.RefreshControls();
		}

		// Token: 0x06004B19 RID: 19225 RVA: 0x00163290 File Offset: 0x00161490
		public new void OnMove(AxisEventData move)
		{
			if (!base.interactable)
			{
				return;
			}
			if (base.MoveOption(move.moveDir))
			{
				this.UpdateFrameCapSetting();
				return;
			}
			base.OnMove(move);
		}

		// Token: 0x06004B1A RID: 19226 RVA: 0x001632B7 File Offset: 0x001614B7
		public new void OnPointerClick(PointerEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			base.PointerClickCheckArrows(eventData);
			this.UpdateFrameCapSetting();
		}

		// Token: 0x06004B1B RID: 19227 RVA: 0x001632CF File Offset: 0x001614CF
		public new void OnSubmit(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			base.MoveOption(MoveDirection.Right);
			this.UpdateFrameCapSetting();
		}

		// Token: 0x06004B1C RID: 19228 RVA: 0x001632E8 File Offset: 0x001614E8
		public void RefreshControls()
		{
			this.RefreshCurrentIndex();
			if (this.selectedOptionIndex == 0)
			{
				this.optionText.text = this.offOption;
				if (this.fixVerticalAlign)
				{
					this.fixVerticalAlign.AlignText();
					return;
				}
			}
			else
			{
				this.UpdateText();
			}
		}

		// Token: 0x06004B1D RID: 19229 RVA: 0x00163338 File Offset: 0x00161538
		public void DisableFrameCapSetting()
		{
			this.Init();
			this.SetOptionTo(0);
			this.UpdateFrameCapSetting();
			if (base.gameObject.activeInHierarchy)
			{
				this.RefreshControls();
			}
		}

		// Token: 0x06004B1E RID: 19230 RVA: 0x00163360 File Offset: 0x00161560
		public void RefreshValueFromGameSettings()
		{
			if (this.gs == null)
			{
				this.gs = GameManager.instance.gameSettings;
			}
			this.Init();
			int num = Mathf.Min(this.maxFrameRate, this.gs.targetFrameRate);
			if (this.runtimeRefreshRates.IndexOf(this.gs.targetFrameRate) < 0)
			{
				num = 60;
			}
			else if (num >= 0 && num <= 30)
			{
				num = 30;
			}
			if (num >= 0)
			{
				UIManager.instance.DisableVsyncSetting();
			}
			Application.targetFrameRate = num;
			if (base.gameObject.activeInHierarchy)
			{
				this.RefreshControls();
			}
		}

		// Token: 0x06004B1F RID: 19231 RVA: 0x001633F4 File Offset: 0x001615F4
		private void Init()
		{
			if (!this.init)
			{
				this.init = true;
				this.gm = GameManager.instance;
				this.fixVerticalAlign = this.optionText.GetComponent<FixVerticalAlign>();
				this.runtimeRefreshRates = new List<int>();
				this.maxFrameRate = -1;
				foreach (Resolution resolution in Screen.resolutions)
				{
					if (resolution.refreshRateRatio.value > (double)this.maxFrameRate)
					{
						this.maxFrameRate = (int)resolution.refreshRateRatio.value;
					}
				}
				foreach (int item in MenuFrameCapSetting.FRAME_CAP_VALUES)
				{
					this.runtimeRefreshRates.Add(item);
				}
				if (this.maxFrameRate > 120)
				{
					this.runtimeRefreshRates.Add(this.maxFrameRate);
					return;
				}
				this.maxFrameRate = 120;
			}
		}

		// Token: 0x06004B20 RID: 19232 RVA: 0x001634D8 File Offset: 0x001616D8
		private void UpdateFrameCapSetting()
		{
			this.Init();
			if (this.selectedOptionIndex == 0)
			{
				this.optionText.text = this.offOption;
				if (this.fixVerticalAlign)
				{
					this.fixVerticalAlign.AlignText();
				}
			}
			else
			{
				UIManager.instance.DisableVsyncSetting();
			}
			if (this.selectedOptionIndex >= 0 && this.selectedOptionIndex < this.runtimeRefreshRates.Count)
			{
				int targetFrameRate = this.runtimeRefreshRates[this.selectedOptionIndex];
				Application.targetFrameRate = targetFrameRate;
				GameManager.instance.gameSettings.targetFrameRate = targetFrameRate;
				return;
			}
			Debug.LogError("Failed to update frame cap setting. Selected index out of range.", this);
		}

		// Token: 0x06004B21 RID: 19233 RVA: 0x00163580 File Offset: 0x00161780
		public override void RefreshCurrentIndex()
		{
			bool flag = false;
			for (int i = 0; i < this.runtimeRefreshRates.Count; i++)
			{
				if (Application.targetFrameRate == this.runtimeRefreshRates[i])
				{
					this.selectedOptionIndex = i;
					flag = true;
				}
			}
			if (!flag)
			{
				this.selectedOptionIndex = -1;
				Debug.LogError("Couldn't match current Target Frame Rate setting - " + Application.targetFrameRate.ToString());
			}
			base.RefreshCurrentIndex();
		}

		// Token: 0x06004B22 RID: 19234 RVA: 0x001635F0 File Offset: 0x001617F0
		public void PushUpdateOptionList()
		{
			this.Init();
			string[] array = this.optionList;
			if (array.Length != this.runtimeRefreshRates.Count)
			{
				array = new string[this.runtimeRefreshRates.Count];
			}
			for (int i = 0; i < this.runtimeRefreshRates.Count; i++)
			{
				int num = this.runtimeRefreshRates[i];
				if (num <= 0)
				{
					array[i] = this.offOption;
				}
				else
				{
					array[i] = num.ToString();
				}
			}
			base.SetOptionList(array);
		}

		// Token: 0x04004CAD RID: 19629
		[Space]
		[SerializeField]
		private LocalisedString offOption = new LocalisedString("MainMenu", "MOH_OFF");

		// Token: 0x04004CAE RID: 19630
		private static readonly int[] FRAME_CAP_VALUES = new int[]
		{
			-1,
			30,
			60,
			120
		};

		// Token: 0x04004CAF RID: 19631
		private List<int> runtimeRefreshRates = new List<int>();

		// Token: 0x04004CB0 RID: 19632
		private FixVerticalAlign fixVerticalAlign;

		// Token: 0x04004CB1 RID: 19633
		private GameSettings gs;

		// Token: 0x04004CB2 RID: 19634
		private bool init;

		// Token: 0x04004CB3 RID: 19635
		private int maxFrameRate;
	}
}
