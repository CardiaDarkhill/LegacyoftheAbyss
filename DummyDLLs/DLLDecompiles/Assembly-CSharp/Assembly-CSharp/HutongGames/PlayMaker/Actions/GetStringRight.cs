using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C4 RID: 4292
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Gets the Right n characters from a String.")]
	public class GetStringRight : FsmStateAction
	{
		// Token: 0x0600745C RID: 29788 RVA: 0x0023A35E File Offset: 0x0023855E
		public override void Reset()
		{
			this.stringVariable = null;
			this.charCount = 0;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600745D RID: 29789 RVA: 0x0023A381 File Offset: 0x00238581
		public override void OnEnter()
		{
			this.DoGetStringRight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600745E RID: 29790 RVA: 0x0023A397 File Offset: 0x00238597
		public override void OnUpdate()
		{
			this.DoGetStringRight();
		}

		// Token: 0x0600745F RID: 29791 RVA: 0x0023A3A0 File Offset: 0x002385A0
		private void DoGetStringRight()
		{
			if (this.stringVariable.IsNone)
			{
				return;
			}
			if (this.storeResult.IsNone)
			{
				return;
			}
			string value = this.stringVariable.Value;
			int num = Mathf.Clamp(this.charCount.Value, 0, value.Length);
			this.storeResult.Value = value.Substring(value.Length - num, num);
		}

		// Token: 0x0400749C RID: 29852
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The string to get characters from.")]
		public FsmString stringVariable;

		// Token: 0x0400749D RID: 29853
		[Tooltip("Number of characters to get.")]
		public FsmInt charCount;

		// Token: 0x0400749E RID: 29854
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a string variable.")]
		public FsmString storeResult;

		// Token: 0x0400749F RID: 29855
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
