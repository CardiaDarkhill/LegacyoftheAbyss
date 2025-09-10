using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F5C RID: 3932
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the comparison of 2 Rect variables.")]
	public class RectCompare : FsmStateAction
	{
		// Token: 0x06006D35 RID: 27957 RVA: 0x0021FA27 File Offset: 0x0021DC27
		public override void Reset()
		{
			this.rect1 = null;
			this.rect2 = null;
			this.tolerance = 0f;
			this.equal = null;
			this.notEqual = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D36 RID: 27958 RVA: 0x0021FA5C File Offset: 0x0021DC5C
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D37 RID: 27959 RVA: 0x0021FA72 File Offset: 0x0021DC72
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06006D38 RID: 27960 RVA: 0x0021FA7C File Offset: 0x0021DC7C
		private void DoCompare()
		{
			if (Mathf.Abs(this.rect1.Value.x - this.rect2.Value.x) > this.tolerance.Value || Mathf.Abs(this.rect1.Value.y - this.rect2.Value.y) > this.tolerance.Value || Mathf.Abs(this.rect1.Value.width - this.rect2.Value.width) > this.tolerance.Value || Mathf.Abs(this.rect1.Value.height - this.rect2.Value.height) > this.tolerance.Value)
			{
				base.Fsm.Event(this.notEqual);
				return;
			}
			base.Fsm.Event(this.equal);
		}

		// Token: 0x06006D39 RID: 27961 RVA: 0x0021FB93 File Offset: 0x0021DD93
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.notEqual))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x04006CF8 RID: 27896
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The first Rect variable.")]
		public FsmRect rect1;

		// Token: 0x04006CF9 RID: 27897
		[RequiredField]
		[Tooltip("The second Rect variable.")]
		public FsmRect rect2;

		// Token: 0x04006CFA RID: 27898
		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).")]
		public FsmFloat tolerance;

		// Token: 0x04006CFB RID: 27899
		[Tooltip("Event sent if Rect 1 equals Rect 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x04006CFC RID: 27900
		[Tooltip("Event sent if Rect 1 does not equal Rect 2 (within Tolerance)")]
		public FsmEvent notEqual;

		// Token: 0x04006CFD RID: 27901
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;
	}
}
