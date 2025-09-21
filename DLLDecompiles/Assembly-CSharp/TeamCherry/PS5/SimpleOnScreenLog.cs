using System;
using System.Collections.Generic;
using TMProOld;
using UnityEngine;
using UnityEngine.Pool;

namespace TeamCherry.PS5
{
	// Token: 0x020008A8 RID: 2216
	public class SimpleOnScreenLog : MonoBehaviour, IMessagePrinter
	{
		// Token: 0x06004CBA RID: 19642 RVA: 0x001688A4 File Offset: 0x00166AA4
		private void Awake()
		{
			PlaystationLogHandler.Printer = this;
			this.textPool = new ObjectPool<TextMeshProUGUI>(new Func<TextMeshProUGUI>(this.CreateFunc), new Action<TextMeshProUGUI>(this.ActionOnGet), new Action<TextMeshProUGUI>(this.ActionOnRelease), null, true, 10, 10000);
			if (this.text)
			{
				this.text.gameObject.SetActive(false);
			}
		}

		// Token: 0x06004CBB RID: 19643 RVA: 0x0016890D File Offset: 0x00166B0D
		private void OnDestroy()
		{
			if ((SimpleOnScreenLog)PlaystationLogHandler.Printer == this)
			{
				PlaystationLogHandler.Printer = null;
			}
		}

		// Token: 0x06004CBC RID: 19644 RVA: 0x00168927 File Offset: 0x00166B27
		private TextMeshProUGUI CreateFunc()
		{
			if (this.text)
			{
				return Object.Instantiate<TextMeshProUGUI>(this.text, base.transform);
			}
			return new GameObject().AddComponent<TextMeshProUGUI>();
		}

		// Token: 0x06004CBD RID: 19645 RVA: 0x00168952 File Offset: 0x00166B52
		private void ActionOnRelease(TextMeshProUGUI obj)
		{
			obj.gameObject.SetActive(false);
			this.activeList.Remove(obj);
		}

		// Token: 0x06004CBE RID: 19646 RVA: 0x0016896D File Offset: 0x00166B6D
		private void ActionOnGet(TextMeshProUGUI obj)
		{
			obj.gameObject.SetActive(true);
			obj.gameObject.transform.SetAsLastSibling();
			this.activeList.Add(obj);
		}

		// Token: 0x06004CBF RID: 19647 RVA: 0x00168997 File Offset: 0x00166B97
		private TextMeshProUGUI GetText()
		{
			if (this.activeList.Count >= this.maxLines)
			{
				this.textPool.Release(this.activeList[0]);
			}
			return this.textPool.Get();
		}

		// Token: 0x06004CC0 RID: 19648 RVA: 0x001689CE File Offset: 0x00166BCE
		public void PrintMessage(Message message)
		{
			TextMeshProUGUI textMeshProUGUI = this.GetText();
			textMeshProUGUI.text = message.message;
			textMeshProUGUI.color = message.color;
		}

		// Token: 0x04004DD3 RID: 19923
		public TextMeshProUGUI text;

		// Token: 0x04004DD4 RID: 19924
		private ObjectPool<TextMeshProUGUI> textPool;

		// Token: 0x04004DD5 RID: 19925
		public int maxLines = 40;

		// Token: 0x04004DD6 RID: 19926
		private List<TextMeshProUGUI> activeList = new List<TextMeshProUGUI>();
	}
}
