using System;
using System.Globalization;
using System.Text;
using TeamCherry.BuildBot;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005DE RID: 1502
public class SetVersionNumber : MonoBehaviour
{
	// Token: 0x06003544 RID: 13636 RVA: 0x000EC278 File Offset: 0x000EA478
	private void Awake()
	{
		this.textUi = base.GetComponent<Text>();
	}

	// Token: 0x06003545 RID: 13637 RVA: 0x000EC288 File Offset: 0x000EA488
	private void Start()
	{
		if (this.textUi != null)
		{
			StringBuilder stringBuilder = new StringBuilder("1.0.28324");
			if (DemoHelper.IsDemoMode)
			{
				stringBuilder.Append(" (Demo)");
			}
			if (CheatManager.IsCheatsEnabled)
			{
				stringBuilder.Append("\n(CHEATS ENABLED)");
				BuildMetadata embedded = BuildMetadata.Embedded;
				if (embedded != null)
				{
					CultureInfo cultureInfo = CultureInfo.GetCultureInfo("en-AU");
					stringBuilder.Append("\nLast Commit Time: " + embedded.CommitTime.ToString(cultureInfo));
					stringBuilder.Append("\nBuild Time: " + embedded.BuildTime.ToString(cultureInfo));
				}
			}
			this.textUi.text = stringBuilder.ToString();
		}
	}

	// Token: 0x040038B4 RID: 14516
	private Text textUi;
}
