using System;
using System.Collections;
using System.Text;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

// Token: 0x02000619 RID: 1561
public class ContentPackDetailsUI : MonoBehaviour
{
	// Token: 0x06003792 RID: 14226 RVA: 0x000F50F1 File Offset: 0x000F32F1
	private void Awake()
	{
		ContentPackDetailsUI.Instance = this;
	}

	// Token: 0x06003793 RID: 14227 RVA: 0x000F50FC File Offset: 0x000F32FC
	public void ShowPackDetails(int index)
	{
		this.packDetailsIndex = index;
		if (MenuStyles.Instance)
		{
			this.oldMenuStyleIndex = MenuStyles.Instance.CurrentStyle;
			MenuStyles.Instance.SetStyle(this.details[this.packDetailsIndex].menuStyleIndex, true, false);
		}
	}

	// Token: 0x06003794 RID: 14228 RVA: 0x000F514A File Offset: 0x000F334A
	private void OnEnable()
	{
		base.StartCoroutine(this.UpdateDelayed());
	}

	// Token: 0x06003795 RID: 14229 RVA: 0x000F5159 File Offset: 0x000F3359
	private void OnDisable()
	{
		if (this.descriptionText)
		{
			this.descriptionText.text = "";
		}
	}

	// Token: 0x06003796 RID: 14230 RVA: 0x000F5178 File Offset: 0x000F3378
	private IEnumerator UpdateDelayed()
	{
		if (this.packDetailsIndex >= 0 && this.packDetailsIndex < this.details.Length)
		{
			ContentPackDetailsUI.ContentPackDetails contentPackDetails = this.details[this.packDetailsIndex];
			if (this.posterImage)
			{
				this.posterImage.sprite = contentPackDetails.posterSprite;
			}
			if (this.titleText)
			{
				this.titleText.text = Language.Get(contentPackDetails.titleText, this.languageSheet);
			}
			if (this.scrollRect)
			{
				this.scrollRect.verticalNormalizedPosition = 1f;
			}
			if (this.descriptionText)
			{
				string text = Language.Get(contentPackDetails.descriptionText, this.languageSheet);
				this.descriptionText.text = text.Replace("<br>", "\n");
				StringBuilder sb = new StringBuilder();
				sb.Append('\n', this.beforeSpaces);
				sb.Append(this.descriptionText.text);
				this.descriptionText.text = sb.ToString();
				while (!this.descriptionText.gameObject.activeInHierarchy)
				{
					yield return null;
				}
				yield return null;
				float preferredHeight = LayoutUtility.GetPreferredHeight(this.descriptionText.rectTransform);
				float height = this.scrollRect.viewport.rect.height;
				bool flag = preferredHeight > height;
				if (flag)
				{
					sb.Append('\n', this.afterSpaces);
				}
				if (this.softMask)
				{
					this.softMask.HardBlend = !flag;
				}
				this.descriptionText.text = sb.ToString();
				sb = null;
			}
		}
		else
		{
			Debug.LogError("Content Pack Details do not exist for index " + this.packDetailsIndex.ToString());
		}
		yield break;
	}

	// Token: 0x06003797 RID: 14231 RVA: 0x000F5187 File Offset: 0x000F3387
	public void UndoMenuStyle()
	{
		if (MenuStyles.Instance)
		{
			MenuStyles.Instance.SetStyle(this.oldMenuStyleIndex, true, true);
		}
	}

	// Token: 0x04003A85 RID: 14981
	public static ContentPackDetailsUI Instance;

	// Token: 0x04003A86 RID: 14982
	public ContentPackDetailsUI.ContentPackDetails[] details;

	// Token: 0x04003A87 RID: 14983
	[Space]
	public Image posterImage;

	// Token: 0x04003A88 RID: 14984
	public Text titleText;

	// Token: 0x04003A89 RID: 14985
	public Text descriptionText;

	// Token: 0x04003A8A RID: 14986
	public SoftMaskScript softMask;

	// Token: 0x04003A8B RID: 14987
	public ScrollRect scrollRect;

	// Token: 0x04003A8C RID: 14988
	public string languageSheet = "CP3";

	// Token: 0x04003A8D RID: 14989
	public int beforeSpaces = 1;

	// Token: 0x04003A8E RID: 14990
	public int afterSpaces = 3;

	// Token: 0x04003A8F RID: 14991
	private int oldMenuStyleIndex;

	// Token: 0x04003A90 RID: 14992
	private int packDetailsIndex;

	// Token: 0x0200192C RID: 6444
	[Serializable]
	public class ContentPackDetails
	{
		// Token: 0x040094AC RID: 38060
		public Sprite posterSprite;

		// Token: 0x040094AD RID: 38061
		public string titleText;

		// Token: 0x040094AE RID: 38062
		[Multiline]
		public string descriptionText;

		// Token: 0x040094AF RID: 38063
		public int menuStyleIndex;
	}
}
