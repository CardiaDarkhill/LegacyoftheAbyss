using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C5 RID: 4293
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Gets a sub-string from a String Variable.")]
	public class GetSubstring : FsmStateAction
	{
		// Token: 0x06007461 RID: 29793 RVA: 0x0023A40F File Offset: 0x0023860F
		public override void Reset()
		{
			this.stringVariable = null;
			this.startIndex = 0;
			this.length = 1;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06007462 RID: 29794 RVA: 0x0023A43E File Offset: 0x0023863E
		public override void OnEnter()
		{
			this.DoGetSubstring();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007463 RID: 29795 RVA: 0x0023A454 File Offset: 0x00238654
		public override void OnUpdate()
		{
			this.DoGetSubstring();
		}

		// Token: 0x06007464 RID: 29796 RVA: 0x0023A45C File Offset: 0x0023865C
		private void DoGetSubstring()
		{
			if (this.stringVariable == null)
			{
				return;
			}
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.stringVariable.Value.Substring(this.startIndex.Value, this.length.Value);
		}

		// Token: 0x040074A0 RID: 29856
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The string to get characters from.")]
		public FsmString stringVariable;

		// Token: 0x040074A1 RID: 29857
		[RequiredField]
		[Tooltip("The start of the substring (0 = first character).")]
		public FsmInt startIndex;

		// Token: 0x040074A2 RID: 29858
		[RequiredField]
		[Tooltip("The number of characters to get.")]
		public FsmInt length;

		// Token: 0x040074A3 RID: 29859
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a string variable.")]
		public FsmString storeResult;

		// Token: 0x040074A4 RID: 29860
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
