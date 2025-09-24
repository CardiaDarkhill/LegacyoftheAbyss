using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F62 RID: 3938
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the comparison of 2 Vector2 variables.")]
	public class Vector2Compare : FsmStateAction
	{
		// Token: 0x06006D53 RID: 27987 RVA: 0x0021FF99 File Offset: 0x0021E199
		public override void Reset()
		{
			this.vector1 = null;
			this.vector2 = null;
			this.tolerance = 0f;
			this.equal = null;
			this.notEqual = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D54 RID: 27988 RVA: 0x0021FFCE File Offset: 0x0021E1CE
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D55 RID: 27989 RVA: 0x0021FFE4 File Offset: 0x0021E1E4
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06006D56 RID: 27990 RVA: 0x0021FFEC File Offset: 0x0021E1EC
		private void DoCompare()
		{
			base.Fsm.Event((Vector2.Distance(this.vector1.Value, this.vector2.Value) > this.tolerance.Value) ? this.notEqual : this.equal);
		}

		// Token: 0x06006D57 RID: 27991 RVA: 0x0022003A File Offset: 0x0021E23A
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.notEqual))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x04006D15 RID: 27925
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The first Vector2 variable.")]
		public FsmVector2 vector1;

		// Token: 0x04006D16 RID: 27926
		[RequiredField]
		[Tooltip("The second Vector2 variable.")]
		public FsmVector2 vector2;

		// Token: 0x04006D17 RID: 27927
		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).")]
		public FsmFloat tolerance;

		// Token: 0x04006D18 RID: 27928
		[Tooltip("Event sent if Rect 1 equals Rect 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x04006D19 RID: 27929
		[Tooltip("Event sent if Rect 1 does not equal Rect 2 (within Tolerance)")]
		public FsmEvent notEqual;

		// Token: 0x04006D1A RID: 27930
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;
	}
}
