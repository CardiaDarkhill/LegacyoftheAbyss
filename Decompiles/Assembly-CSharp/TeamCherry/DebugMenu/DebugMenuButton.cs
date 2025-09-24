using System;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

namespace TeamCherry.DebugMenu
{
	// Token: 0x020008BB RID: 2235
	public class DebugMenuButton : MonoBehaviour
	{
		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06004D7D RID: 19837 RVA: 0x0016C0D3 File Offset: 0x0016A2D3
		// (set) Token: 0x06004D7E RID: 19838 RVA: 0x0016C0DB File Offset: 0x0016A2DB
		public TextMeshProUGUI Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
			}
		}

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x06004D7F RID: 19839 RVA: 0x0016C0E4 File Offset: 0x0016A2E4
		// (set) Token: 0x06004D80 RID: 19840 RVA: 0x0016C0EC File Offset: 0x0016A2EC
		public Button Button
		{
			get
			{
				return this.button;
			}
			set
			{
				this.button = value;
			}
		}

		// Token: 0x06004D81 RID: 19841 RVA: 0x0016C0F5 File Offset: 0x0016A2F5
		private void OnValidate()
		{
			if (!this.text)
			{
				this.text = base.gameObject.GetComponentInChildren<TextMeshProUGUI>();
			}
			if (!this.button)
			{
				this.button = base.gameObject.GetComponentInChildren<Button>();
			}
		}

		// Token: 0x06004D82 RID: 19842 RVA: 0x0016C134 File Offset: 0x0016A334
		public void DoButton(string text, Action onClick)
		{
			this.text.text = text;
			this.button.onClick.AddListener(delegate()
			{
				Action onClick2 = onClick;
				if (onClick2 == null)
				{
					return;
				}
				onClick2();
			});
		}

		// Token: 0x04004E41 RID: 20033
		[SerializeField]
		private TextMeshProUGUI text;

		// Token: 0x04004E42 RID: 20034
		[SerializeField]
		private Button button;
	}
}
