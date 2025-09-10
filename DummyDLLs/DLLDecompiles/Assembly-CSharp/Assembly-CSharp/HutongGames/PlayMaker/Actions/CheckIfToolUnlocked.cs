using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001325 RID: 4901
	public class CheckIfToolUnlocked : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007F01 RID: 32513 RVA: 0x0025A5CF File Offset: 0x002587CF
		public override void Reset()
		{
			base.Reset();
			this.Tool = null;
		}

		// Token: 0x17000C37 RID: 3127
		// (get) Token: 0x06007F02 RID: 32514 RVA: 0x0025A5DE File Offset: 0x002587DE
		public override bool IsTrue
		{
			get
			{
				return !this.Tool.IsNone && ((ToolItem)this.Tool.Value).IsUnlocked;
			}
		}

		// Token: 0x04007E9E RID: 32414
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;
	}
}
