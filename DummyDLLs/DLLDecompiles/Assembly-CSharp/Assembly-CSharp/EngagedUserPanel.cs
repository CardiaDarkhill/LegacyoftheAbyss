using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000645 RID: 1605
public class EngagedUserPanel : MonoBehaviour
{
	// Token: 0x06003992 RID: 14738 RVA: 0x000FCCCB File Offset: 0x000FAECB
	protected void Awake()
	{
		Platform.Current.EngagedDisplayInfoChanged += this.UpdateContent;
	}

	// Token: 0x06003993 RID: 14739 RVA: 0x000FCCE3 File Offset: 0x000FAEE3
	protected void OnDestroy()
	{
		Platform.Current.EngagedDisplayInfoChanged -= this.UpdateContent;
	}

	// Token: 0x06003994 RID: 14740 RVA: 0x000FCCFB File Offset: 0x000FAEFB
	protected void Start()
	{
		this.UpdateContent();
		base.enabled = false;
	}

	// Token: 0x06003995 RID: 14741 RVA: 0x000FCD0C File Offset: 0x000FAF0C
	private void UpdateContent()
	{
		if (Platform.Current.EngagementRequirement == Platform.EngagementRequirements.Invisible)
		{
			this.canvasGroup.alpha = 0f;
			this.displayNameText.enabled = false;
			this.displayImageImage.enabled = false;
			return;
		}
		this.canvasGroup.alpha = 1f;
		this.displayNameText.enabled = true;
		this.displayNameText.text = (Platform.Current.EngagedDisplayName ?? "");
		Texture2D engagedDisplayImage = Platform.Current.EngagedDisplayImage;
		this.displayImageImage.enabled = (engagedDisplayImage != null);
		this.displayImageImage.texture = engagedDisplayImage;
	}

	// Token: 0x04003C4B RID: 15435
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x04003C4C RID: 15436
	[SerializeField]
	private Text displayNameText;

	// Token: 0x04003C4D RID: 15437
	[SerializeField]
	private RawImage displayImageImage;
}
