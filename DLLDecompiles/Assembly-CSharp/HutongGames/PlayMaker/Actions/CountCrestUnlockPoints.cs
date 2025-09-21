using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001322 RID: 4898
	public class CountCrestUnlockPoints : FsmStateAction
	{
		// Token: 0x06007EF9 RID: 32505 RVA: 0x0025A3BB File Offset: 0x002585BB
		public override void Reset()
		{
			base.Reset();
			this.CrestList = null;
			this.StoreCurrentPoints = null;
			this.StoreMaxPoints = null;
		}

		// Token: 0x06007EFA RID: 32506 RVA: 0x0025A3D8 File Offset: 0x002585D8
		public override void OnEnter()
		{
			int num = 0;
			int num2 = 0;
			ToolCrestList toolCrestList = this.CrestList.Value as ToolCrestList;
			if (toolCrestList != null)
			{
				foreach (ToolCrest toolCrest in toolCrestList)
				{
					if (!toolCrest.IsHidden && toolCrest.IsBaseVersion && !toolCrest.IsUpgradedVersionUnlocked)
					{
						ToolCrest.SlotInfo[] slots = toolCrest.Slots;
						for (int i = 0; i < slots.Length; i++)
						{
							num2++;
						}
						if (toolCrest.IsUnlocked)
						{
							ToolCrest.SlotInfo[] slots2 = toolCrest.Slots;
							List<ToolCrestsData.SlotData> slots3 = toolCrest.SaveData.Slots;
							for (int j = 0; j < slots2.Length; j++)
							{
								if (!slots2[j].IsLocked || (slots3 != null && j < slots3.Count && slots3[j].IsUnlocked))
								{
									num++;
								}
							}
						}
					}
				}
			}
			this.StoreCurrentPoints.Value = num;
			this.StoreMaxPoints.Value = num2;
			base.Finish();
		}

		// Token: 0x04007E99 RID: 32409
		[ObjectType(typeof(ToolCrestList))]
		public FsmObject CrestList;

		// Token: 0x04007E9A RID: 32410
		[UIHint(UIHint.Variable)]
		public FsmInt StoreCurrentPoints;

		// Token: 0x04007E9B RID: 32411
		[UIHint(UIHint.Variable)]
		public FsmInt StoreMaxPoints;
	}
}
