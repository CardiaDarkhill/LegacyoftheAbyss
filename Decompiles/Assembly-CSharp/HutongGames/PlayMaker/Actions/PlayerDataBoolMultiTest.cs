using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C74 RID: 3188
	[ActionCategory("PlayerData")]
	[Tooltip("Sends a Message to PlayerData to send and receive data.")]
	public class PlayerDataBoolMultiTest : FsmStateAction
	{
		// Token: 0x0600602A RID: 24618 RVA: 0x001E7433 File Offset: 0x001E5633
		public override void Reset()
		{
			this.boolTests = null;
			this.storeValue = null;
			this.passedEvent = null;
			this.failedEvent = null;
		}

		// Token: 0x0600602B RID: 24619 RVA: 0x001E7454 File Offset: 0x001E5654
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance == null)
			{
				return;
			}
			bool flag = true;
			foreach (PlayerDataBoolMultiTest.BoolTest boolTest in this.boolTests)
			{
				string value = boolTest.boolName.Value;
				bool flag2 = string.IsNullOrEmpty(value);
				if (boolTest.inputBool.IsNone && flag2)
				{
					flag = false;
				}
				else
				{
					bool flag3 = boolTest.inputBool.Value;
					if (!flag2)
					{
						flag3 = instance.GetPlayerDataBool(value);
						boolTest.storeValue.Value = flag3;
					}
					if (boolTest.expectedValue.Value != flag3)
					{
						flag = false;
					}
				}
			}
			this.storeValue.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.passedEvent);
			}
			else
			{
				base.Fsm.Event(this.failedEvent);
			}
			base.Finish();
		}

		// Token: 0x04005D82 RID: 23938
		[RequiredField]
		public PlayerDataBoolMultiTest.BoolTest[] boolTests;

		// Token: 0x04005D83 RID: 23939
		[UIHint(UIHint.Variable)]
		public FsmBool storeValue;

		// Token: 0x04005D84 RID: 23940
		public FsmEvent passedEvent;

		// Token: 0x04005D85 RID: 23941
		public FsmEvent failedEvent;

		// Token: 0x02001B83 RID: 7043
		[Serializable]
		public class BoolTest
		{
			// Token: 0x04009D72 RID: 40306
			public FsmString boolName;

			// Token: 0x04009D73 RID: 40307
			[Tooltip("Will use this value instead if bool name is empty.")]
			[UIHint(UIHint.Variable)]
			public FsmBool inputBool;

			// Token: 0x04009D74 RID: 40308
			[Tooltip("Expected state player data bool needs to be in to pass.")]
			public FsmBool expectedValue;

			// Token: 0x04009D75 RID: 40309
			[Tooltip("Value of player data bool (Independent of expected value).")]
			[UIHint(UIHint.Variable)]
			public FsmBool storeValue;
		}
	}
}
