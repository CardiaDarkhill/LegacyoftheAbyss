using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F60 RID: 3936
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if a String contains another String.")]
	public class StringContains : FsmStateAction
	{
		// Token: 0x06006D49 RID: 27977 RVA: 0x0021FDFB File Offset: 0x0021DFFB
		public override void Reset()
		{
			this.stringVariable = null;
			this.containsString = "";
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D4A RID: 27978 RVA: 0x0021FE30 File Offset: 0x0021E030
		public override void OnEnter()
		{
			this.DoStringContains();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D4B RID: 27979 RVA: 0x0021FE46 File Offset: 0x0021E046
		public override void OnUpdate()
		{
			this.DoStringContains();
		}

		// Token: 0x06006D4C RID: 27980 RVA: 0x0021FE50 File Offset: 0x0021E050
		private void DoStringContains()
		{
			if (this.stringVariable.IsNone || this.containsString.IsNone)
			{
				return;
			}
			bool flag = this.stringVariable.Value.Contains(this.containsString.Value);
			if (this.storeResult != null)
			{
				this.storeResult.Value = flag;
			}
			if (flag && this.trueEvent != null)
			{
				base.Fsm.Event(this.trueEvent);
				return;
			}
			if (!flag && this.falseEvent != null)
			{
				base.Fsm.Event(this.falseEvent);
			}
		}

		// Token: 0x04006D0B RID: 27915
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The String variable to test.")]
		public FsmString stringVariable;

		// Token: 0x04006D0C RID: 27916
		[RequiredField]
		[Tooltip("Test if the String variable contains this string.")]
		public FsmString containsString;

		// Token: 0x04006D0D RID: 27917
		[Tooltip("Event to send if true.")]
		public FsmEvent trueEvent;

		// Token: 0x04006D0E RID: 27918
		[Tooltip("Event to send if false.")]
		public FsmEvent falseEvent;

		// Token: 0x04006D0F RID: 27919
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the true/false result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006D10 RID: 27920
		[Tooltip("Repeat every frame. Useful if any of the strings are changing over time.")]
		public bool everyFrame;
	}
}
