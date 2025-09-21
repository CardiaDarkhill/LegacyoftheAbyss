using System;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000739 RID: 1849
public class ToolTutorialMsg : UIMsgBase<ToolItem>
{
	// Token: 0x06004204 RID: 16900 RVA: 0x0012262F File Offset: 0x0012082F
	private void Awake()
	{
		this.rootFader.AlphaSelf = 0f;
	}

	// Token: 0x06004205 RID: 16901 RVA: 0x00122644 File Offset: 0x00120844
	public static void Spawn(ToolItem tool, Action afterMsg = null)
	{
		ToolTutorialMsg.<>c__DisplayClass4_0 CS$<>8__locals1 = new ToolTutorialMsg.<>c__DisplayClass4_0();
		CS$<>8__locals1.afterMsg = afterMsg;
		CS$<>8__locals1.msg = null;
		CS$<>8__locals1.msg = (UIMsgBase<ToolItem>.Spawn(tool, UI.ToolTutorialMsgPrefab, new Action(CS$<>8__locals1.<Spawn>g__AfterMsg|0)) as ToolTutorialMsg);
		if (!CS$<>8__locals1.msg)
		{
			return;
		}
		GameCameras.instance.HUDOut();
		HeroController.instance.AddInputBlocker(CS$<>8__locals1.msg);
	}

	// Token: 0x06004206 RID: 16902 RVA: 0x001226B0 File Offset: 0x001208B0
	protected override void Setup(ToolItem tool)
	{
		if (this.toolIcon)
		{
			this.toolIcon.sprite = tool.InventorySpriteBase;
		}
		if (this.ring)
		{
			this.ring.color = UI.GetToolTypeColor(tool.Type);
		}
	}

	// Token: 0x04004389 RID: 17289
	[SerializeField]
	private NestedFadeGroupBase rootFader;

	// Token: 0x0400438A RID: 17290
	[SerializeField]
	private SpriteRenderer toolIcon;

	// Token: 0x0400438B RID: 17291
	[SerializeField]
	private SpriteRenderer ring;
}
