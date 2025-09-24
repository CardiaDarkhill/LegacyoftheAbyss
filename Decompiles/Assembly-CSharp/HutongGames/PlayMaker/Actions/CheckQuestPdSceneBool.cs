using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200131E RID: 4894
	public class CheckQuestPdSceneBool : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007EED RID: 32493 RVA: 0x0025A283 File Offset: 0x00258483
		public override void Reset()
		{
			base.Reset();
			this.QuestTarget = null;
			this.ExpectedValue = null;
		}

		// Token: 0x17000C33 RID: 3123
		// (get) Token: 0x06007EEE RID: 32494 RVA: 0x0025A29C File Offset: 0x0025849C
		public override bool IsTrue
		{
			get
			{
				QuestTargetPlayerDataBools questTargetPlayerDataBools = this.QuestTarget.Value as QuestTargetPlayerDataBools;
				return !(questTargetPlayerDataBools == null) && questTargetPlayerDataBools.GetSceneBoolValue() == this.ExpectedValue.Value;
			}
		}

		// Token: 0x04007E94 RID: 32404
		[ObjectType(typeof(QuestTargetPlayerDataBools))]
		[RequiredField]
		public FsmObject QuestTarget;

		// Token: 0x04007E95 RID: 32405
		[RequiredField]
		public FsmBool ExpectedValue;
	}
}
