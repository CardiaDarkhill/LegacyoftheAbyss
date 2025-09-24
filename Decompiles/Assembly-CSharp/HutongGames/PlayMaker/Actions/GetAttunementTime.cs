using System;
using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F7E RID: 3966
	[ActionCategory(ActionCategory.Math)]
	public class GetAttunementTime : FsmStateAction
	{
		// Token: 0x06006DD4 RID: 28116 RVA: 0x002216D1 File Offset: 0x0021F8D1
		public override void Reset()
		{
			this.storeVariable = null;
			this.baseTime = 0f;
			this.extraTime = 0f;
		}

		// Token: 0x06006DD5 RID: 28117 RVA: 0x002216FA File Offset: 0x0021F8FA
		public override void OnEnter()
		{
			this.GetTime();
		}

		// Token: 0x06006DD6 RID: 28118 RVA: 0x00221704 File Offset: 0x0021F904
		private void GetTime()
		{
			int num = GameManager.instance.playerData.attunementLevel;
			if (Gameplay.MusicianCharmTool.IsEquipped)
			{
				num++;
			}
			float num2 = this.baseTime.Value;
			switch (num)
			{
			case 2:
				num2 *= 0.9f;
				break;
			case 3:
				num2 *= 0.75f;
				break;
			case 4:
				num2 *= 0.6f;
				break;
			case 5:
				num2 *= 0.5f;
				break;
			case 6:
				num2 *= 0.45f;
				break;
			}
			num2 += this.extraTime.Value;
			this.storeVariable.Value = num2;
		}

		// Token: 0x04006D8A RID: 28042
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeVariable;

		// Token: 0x04006D8B RID: 28043
		public FsmFloat baseTime;

		// Token: 0x04006D8C RID: 28044
		public FsmFloat extraTime;
	}
}
