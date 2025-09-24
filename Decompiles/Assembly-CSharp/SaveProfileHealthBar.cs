using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200070F RID: 1807
public class SaveProfileHealthBar : MonoBehaviour
{
	// Token: 0x0600407F RID: 16511 RVA: 0x0011BBD8 File Offset: 0x00119DD8
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<SaveProfileHealthBar.CrestTypeInfo>(ref this.crests, typeof(SaveProfileHealthBar.CrestTypes));
	}

	// Token: 0x06004080 RID: 16512 RVA: 0x0011BBEF File Offset: 0x00119DEF
	private void Awake()
	{
		this.OnValidate();
		this.healthTemplate.gameObject.SetActive(false);
	}

	// Token: 0x06004081 RID: 16513 RVA: 0x0011BC08 File Offset: 0x00119E08
	public void ShowHealth(int numberToShow, bool steelsoulMode, string crestId)
	{
		SaveProfileHealthBar.CrestTypes crestTypes;
		if (Enum.TryParse<SaveProfileHealthBar.CrestTypes>(crestId, out crestTypes))
		{
			SaveProfileHealthBar.CrestTypeInfo crestTypeInfo = this.crests[(int)crestTypes];
			this.spoolImage.sprite = (steelsoulMode ? crestTypeInfo.SpoolImageSteel : crestTypeInfo.SpoolImage);
		}
		else
		{
			Debug.LogError("Could not parse crest id " + crestId, this);
		}
		for (int i = numberToShow - this.healthImages.Count; i > 0; i--)
		{
			this.healthImages.Add(Object.Instantiate<Image>(this.healthTemplate, this.healthTemplate.transform.parent));
		}
		for (int j = 0; j < this.healthImages.Count; j++)
		{
			this.healthImages[j].gameObject.SetActive(j < numberToShow);
		}
	}

	// Token: 0x04004213 RID: 16915
	[SerializeField]
	[ArrayForEnum(typeof(SaveProfileHealthBar.CrestTypes))]
	private SaveProfileHealthBar.CrestTypeInfo[] crests;

	// Token: 0x04004214 RID: 16916
	[Space]
	[SerializeField]
	private Image spoolImage;

	// Token: 0x04004215 RID: 16917
	[SerializeField]
	private Image healthTemplate;

	// Token: 0x04004216 RID: 16918
	private readonly List<Image> healthImages = new List<Image>(10);

	// Token: 0x020019FC RID: 6652
	private enum CrestTypes
	{
		// Token: 0x040097FD RID: 38909
		Hunter,
		// Token: 0x040097FE RID: 38910
		Hunter_v2,
		// Token: 0x040097FF RID: 38911
		Hunter_v3,
		// Token: 0x04009800 RID: 38912
		Cloakless,
		// Token: 0x04009801 RID: 38913
		Cursed,
		// Token: 0x04009802 RID: 38914
		Reaper,
		// Token: 0x04009803 RID: 38915
		Spell,
		// Token: 0x04009804 RID: 38916
		Toolmaster,
		// Token: 0x04009805 RID: 38917
		Wanderer,
		// Token: 0x04009806 RID: 38918
		Warrior,
		// Token: 0x04009807 RID: 38919
		Witch
	}

	// Token: 0x020019FD RID: 6653
	[Serializable]
	private struct CrestTypeInfo
	{
		// Token: 0x04009808 RID: 38920
		public Sprite SpoolImage;

		// Token: 0x04009809 RID: 38921
		public Sprite SpoolImageSteel;
	}
}
