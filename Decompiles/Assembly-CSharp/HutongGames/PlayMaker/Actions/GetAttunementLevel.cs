using System;
using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F7D RID: 3965
	[ActionCategory(ActionCategory.Math)]
	public class GetAttunementLevel : FsmStateAction
	{
		// Token: 0x06006DD0 RID: 28112 RVA: 0x0022167E File Offset: 0x0021F87E
		public override void Reset()
		{
			this.storeVariable = null;
		}

		// Token: 0x06006DD1 RID: 28113 RVA: 0x00221687 File Offset: 0x0021F887
		public override void OnEnter()
		{
			this.GetLevel();
		}

		// Token: 0x06006DD2 RID: 28114 RVA: 0x00221690 File Offset: 0x0021F890
		private void GetLevel()
		{
			int num = GameManager.instance.playerData.attunementLevel;
			if (Gameplay.MusicianCharmTool.IsEquipped)
			{
				num++;
			}
			this.storeVariable.Value = num;
		}

		// Token: 0x04006D89 RID: 28041
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt storeVariable;
	}
}
