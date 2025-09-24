using System;

namespace UnityEngine.UI
{
	// Token: 0x0200086D RID: 2157
	public class ControllerProfileButtons : MonoBehaviour
	{
		// Token: 0x06004AE5 RID: 19173 RVA: 0x00162B64 File Offset: 0x00160D64
		public void SelectItem(int num)
		{
			switch (num)
			{
			case 1:
				this.profileHighlight1.gameObject.SetActive(true);
				this.profileHighlight2.gameObject.SetActive(false);
				this.profileHighlight3.gameObject.SetActive(false);
				this.profileHighlight4.gameObject.SetActive(false);
				return;
			case 2:
				this.profileHighlight1.gameObject.SetActive(false);
				this.profileHighlight2.gameObject.SetActive(true);
				this.profileHighlight3.gameObject.SetActive(false);
				this.profileHighlight4.gameObject.SetActive(false);
				return;
			case 3:
				this.profileHighlight1.gameObject.SetActive(false);
				this.profileHighlight2.gameObject.SetActive(false);
				this.profileHighlight3.gameObject.SetActive(true);
				this.profileHighlight4.gameObject.SetActive(false);
				return;
			case 4:
				this.profileHighlight1.gameObject.SetActive(false);
				this.profileHighlight2.gameObject.SetActive(false);
				this.profileHighlight3.gameObject.SetActive(false);
				this.profileHighlight4.gameObject.SetActive(true);
				return;
			default:
				Debug.LogError("Invalid profile button ID");
				return;
			}
		}

		// Token: 0x04004C8F RID: 19599
		public Image profileHighlight1;

		// Token: 0x04004C90 RID: 19600
		public Image profileHighlight2;

		// Token: 0x04004C91 RID: 19601
		public Image profileHighlight3;

		// Token: 0x04004C92 RID: 19602
		public Image profileHighlight4;
	}
}
