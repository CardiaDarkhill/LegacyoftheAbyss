using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020006D0 RID: 1744
public class MapMarkerButton : MonoBehaviour
{
	// Token: 0x06003EED RID: 16109 RVA: 0x00114FAF File Offset: 0x001131AF
	private void Awake()
	{
		this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		this.fade = base.GetComponent<NestedFadeGroupBase>();
	}

	// Token: 0x06003EEE RID: 16110 RVA: 0x00114FCC File Offset: 0x001131CC
	private void OnEnable()
	{
		PlayerData playerData = GameManager.instance.playerData;
		if (playerData == null)
		{
			return;
		}
		if (this.fade)
		{
			this.fade.AlphaSelf = 1f;
		}
		if ((playerData.hasMarker_a ? 1 : 0) + (playerData.hasMarker_b ? 1 : 0) + (playerData.hasMarker_c ? 1 : 0) + (playerData.hasMarker_d ? 1 : 0) + (playerData.hasMarker_e ? 1 : 0) < this.neededMarkerTypes)
		{
			this.DoDisable();
			this.shouldDisable = true;
			return;
		}
		this.shouldDisable = false;
	}

	// Token: 0x06003EEF RID: 16111 RVA: 0x00115062 File Offset: 0x00113262
	private void Update()
	{
		if (this.keepDisabled && this.shouldDisable)
		{
			this.DoDisable();
		}
	}

	// Token: 0x06003EF0 RID: 16112 RVA: 0x0011507C File Offset: 0x0011327C
	private void DoDisable()
	{
		switch (this.disable)
		{
		case MapMarkerButton.DisableType.GameObject:
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
				return;
			}
			break;
		case MapMarkerButton.DisableType.SpriteRenderer:
			if (this.spriteRenderer && this.spriteRenderer.enabled)
			{
				this.spriteRenderer.enabled = false;
				return;
			}
			break;
		case MapMarkerButton.DisableType.NestedFadeGroup:
			if (this.fade)
			{
				this.fade.AlphaSelf = 0f;
				return;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x04004092 RID: 16530
	public int neededMarkerTypes = 2;

	// Token: 0x04004093 RID: 16531
	public MapMarkerButton.DisableType disable;

	// Token: 0x04004094 RID: 16532
	public bool keepDisabled;

	// Token: 0x04004095 RID: 16533
	private bool shouldDisable;

	// Token: 0x04004096 RID: 16534
	private SpriteRenderer spriteRenderer;

	// Token: 0x04004097 RID: 16535
	private NestedFadeGroupBase fade;

	// Token: 0x020019CF RID: 6607
	public enum DisableType
	{
		// Token: 0x0400973A RID: 38714
		GameObject,
		// Token: 0x0400973B RID: 38715
		SpriteRenderer,
		// Token: 0x0400973C RID: 38716
		NestedFadeGroup
	}
}
