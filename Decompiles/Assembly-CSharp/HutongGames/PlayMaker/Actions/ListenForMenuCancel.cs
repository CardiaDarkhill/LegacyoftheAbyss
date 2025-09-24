using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAB RID: 3243
	public class ListenForMenuCancel : ListenForMenuButton
	{
		// Token: 0x17000BC2 RID: 3010
		// (get) Token: 0x06006125 RID: 24869 RVA: 0x001EC69D File Offset: 0x001EA89D
		protected override Platform.MenuActions MenuAction
		{
			get
			{
				return Platform.MenuActions.Cancel;
			}
		}
	}
}
