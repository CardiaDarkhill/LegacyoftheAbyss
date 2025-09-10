using System;
using GlobalEnums;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000712 RID: 1810
public class SaveSlotBackgrounds : MonoBehaviour
{
	// Token: 0x06004088 RID: 16520 RVA: 0x0011BE48 File Offset: 0x0011A048
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<SaveSlotBackgrounds.AreaBackground>(ref this.areaBackgrounds, typeof(MapZone));
		ArrayForEnumAttribute.EnsureArraySize<SaveSlotBackgrounds.AreaBackground>(ref this.extraAreaBackgrounds, typeof(ExtraRestZones));
		ArrayForEnumAttribute.EnsureArraySize<Sprite>(ref this.bellhomeBackgrounds, typeof(BellhomePaintColours));
	}

	// Token: 0x06004089 RID: 16521 RVA: 0x0011BE94 File Offset: 0x0011A094
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x0600408A RID: 16522 RVA: 0x0011BE9C File Offset: 0x0011A09C
	public SaveSlotBackgrounds.AreaBackground GetBackground(SaveStats currentSaveStats)
	{
		ExtraRestZones extraRestZone = currentSaveStats.ExtraRestZone;
		SaveSlotBackgrounds.AreaBackground[] array;
		if (extraRestZone > ExtraRestZones.None)
		{
			array = this.extraAreaBackgrounds;
			if (array != null && array.Length > 0)
			{
				SaveSlotBackgrounds.AreaBackground extraBackground = this.GetExtraBackground((int)extraRestZone);
				if (extraRestZone == ExtraRestZones.Bellhome)
				{
					extraBackground.BackgroundImage = this.bellhomeBackgrounds[(int)currentSaveStats.BellhomePaintColour];
				}
				if (extraBackground != null && extraBackground.BackgroundImage)
				{
					return extraBackground;
				}
			}
		}
		MapZone mapZone = currentSaveStats.MapZone;
		array = this.areaBackgrounds;
		if (array == null || array.Length <= 0)
		{
			Debug.LogError("No background images have been created in this prefab.");
			return null;
		}
		SaveSlotBackgrounds.AreaBackground background = this.GetBackground((int)mapZone);
		if (background != null && background.BackgroundImage)
		{
			return background;
		}
		return this.GetBackground(13);
	}

	// Token: 0x0600408B RID: 16523 RVA: 0x0011BF3F File Offset: 0x0011A13F
	public SaveSlotBackgrounds.AreaBackground GetBackground(MapZone mapZone)
	{
		return this.GetBackground((int)mapZone);
	}

	// Token: 0x0600408C RID: 16524 RVA: 0x0011BF48 File Offset: 0x0011A148
	private SaveSlotBackgrounds.AreaBackground GetBackground(int i)
	{
		if (i < 0 || i >= this.areaBackgrounds.Length)
		{
			return null;
		}
		return this.areaBackgrounds[i];
	}

	// Token: 0x0600408D RID: 16525 RVA: 0x0011BF63 File Offset: 0x0011A163
	private SaveSlotBackgrounds.AreaBackground GetExtraBackground(int i)
	{
		if (i < 0 || i >= this.extraAreaBackgrounds.Length)
		{
			return null;
		}
		return this.extraAreaBackgrounds[i];
	}

	// Token: 0x04004221 RID: 16929
	[SerializeField]
	[ArrayForEnum(typeof(MapZone))]
	private SaveSlotBackgrounds.AreaBackground[] areaBackgrounds;

	// Token: 0x04004222 RID: 16930
	[SerializeField]
	[ArrayForEnum(typeof(ExtraRestZones))]
	private SaveSlotBackgrounds.AreaBackground[] extraAreaBackgrounds;

	// Token: 0x04004223 RID: 16931
	[SerializeField]
	[ArrayForEnum(typeof(BellhomePaintColours))]
	private Sprite[] bellhomeBackgrounds;

	// Token: 0x020019FE RID: 6654
	[Serializable]
	public class AreaBackground
	{
		// Token: 0x0400980A RID: 38922
		[FormerlySerializedAs("backgroundImage")]
		public Sprite BackgroundImage;

		// Token: 0x0400980B RID: 38923
		public Sprite Act3BackgroundImage;

		// Token: 0x0400980C RID: 38924
		[LocalisedString.NotRequiredAttribute]
		public LocalisedString NameOverride;

		// Token: 0x0400980D RID: 38925
		public bool Act3OverlayOptOut;
	}
}
