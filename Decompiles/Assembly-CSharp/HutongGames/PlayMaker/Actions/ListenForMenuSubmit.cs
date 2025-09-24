using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CAD RID: 3245
	public class ListenForMenuSubmit : ListenForMenuButton
	{
		// Token: 0x17000BC4 RID: 3012
		// (get) Token: 0x0600612C RID: 24876 RVA: 0x001EC7D3 File Offset: 0x001EA9D3
		protected override Platform.MenuActions MenuAction
		{
			get
			{
				return Platform.MenuActions.Submit;
			}
		}
	}
}
