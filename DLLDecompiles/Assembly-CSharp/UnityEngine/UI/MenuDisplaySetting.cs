using System;
using System.Collections;
using HKMenu;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000871 RID: 2161
	public class MenuDisplaySetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IMenuOptionListSetting, IPointerClickHandler
	{
		// Token: 0x06004B0D RID: 19213 RVA: 0x00163150 File Offset: 0x00161350
		public new void OnEnable()
		{
			this.RefreshControls();
		}

		// Token: 0x06004B0E RID: 19214 RVA: 0x00163158 File Offset: 0x00161358
		public new void OnMove(AxisEventData move)
		{
			if (base.MoveOption(move.moveDir))
			{
				this.UpdateMonitorSetting();
				return;
			}
			base.OnMove(move);
		}

		// Token: 0x06004B0F RID: 19215 RVA: 0x00163176 File Offset: 0x00161376
		public new void OnPointerClick(PointerEventData eventData)
		{
			base.PointerClickCheckArrows(eventData);
			this.UpdateMonitorSetting();
		}

		// Token: 0x06004B10 RID: 19216 RVA: 0x00163185 File Offset: 0x00161385
		public void RefreshControls()
		{
			this.RefreshCurrentIndex();
			this.PushUpdateOptionList();
			this.UpdateText();
		}

		// Token: 0x06004B11 RID: 19217 RVA: 0x00163199 File Offset: 0x00161399
		public void DisableMonitorSelectSetting()
		{
			this.SetOptionTo(0);
			this.UpdateMonitorSetting();
		}

		// Token: 0x06004B12 RID: 19218 RVA: 0x001631A8 File Offset: 0x001613A8
		private void UpdateMonitorSetting()
		{
			Debug.Log("UpdateMonitorSetting...");
			base.StartCoroutine(this.TargetDisplayHack(this.selectedOptionIndex));
		}

		// Token: 0x06004B13 RID: 19219 RVA: 0x001631C8 File Offset: 0x001613C8
		public override void RefreshCurrentIndex()
		{
			this.availableDisplays = Display.displays;
			bool flag = false;
			for (int i = 0; i < this.availableDisplays.Length; i++)
			{
				if (Display.main == this.availableDisplays[i])
				{
					this.selectedOptionIndex = i;
					flag = true;
				}
			}
			if (!flag)
			{
				Debug.LogError("Could not find currently active display");
			}
			base.RefreshCurrentIndex();
		}

		// Token: 0x06004B14 RID: 19220 RVA: 0x00163220 File Offset: 0x00161420
		public void PushUpdateOptionList()
		{
			string[] array = new string[this.availableDisplays.Length];
			for (int i = 0; i < this.availableDisplays.Length; i++)
			{
				array[i] = (i + 1).ToString();
			}
			base.SetOptionList(array);
		}

		// Token: 0x06004B15 RID: 19221 RVA: 0x00163263 File Offset: 0x00161463
		private IEnumerator TargetDisplayHack(int targetDisplay)
		{
			int screenWidth = Screen.width;
			int screenHeight = Screen.height;
			PlayerPrefs.SetInt("UnitySelectMonitor", targetDisplay);
			Screen.SetResolution(800, 600, Screen.fullScreen);
			yield return null;
			Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreen);
			yield break;
		}

		// Token: 0x04004CAC RID: 19628
		private Display[] availableDisplays;
	}
}
