using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x020006DE RID: 1758
[RequireComponent(typeof(SpriteRenderer))]
public class MenuButtonIcon : ActionButtonIconBase
{
	// Token: 0x17000746 RID: 1862
	// (get) Token: 0x06003F6E RID: 16238 RVA: 0x00117FC8 File Offset: 0x001161C8
	public override HeroActionButton Action
	{
		get
		{
			if (Platform.Current.WasLastInputKeyboard)
			{
				switch (this.menuAction)
				{
				case Platform.MenuActions.Submit:
					return HeroActionButton.JUMP;
				case Platform.MenuActions.Cancel:
					return HeroActionButton.CAST;
				case Platform.MenuActions.Extra:
					return HeroActionButton.DASH;
				case Platform.MenuActions.Super:
					return HeroActionButton.DREAM_NAIL;
				}
			}
			else
			{
				switch (this.menuAction)
				{
				case Platform.MenuActions.Submit:
					return HeroActionButton.MENU_SUBMIT;
				case Platform.MenuActions.Cancel:
					return HeroActionButton.MENU_CANCEL;
				case Platform.MenuActions.Extra:
					return HeroActionButton.MENU_EXTRA;
				case Platform.MenuActions.Super:
					return HeroActionButton.MENU_SUPER;
				}
			}
			return HeroActionButton.MENU_CANCEL;
		}
	}

	// Token: 0x0400412F RID: 16687
	public Platform.MenuActions menuAction;
}
