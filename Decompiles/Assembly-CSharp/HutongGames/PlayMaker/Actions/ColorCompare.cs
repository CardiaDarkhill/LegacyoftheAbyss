using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F44 RID: 3908
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Sends Events based on the comparison of 2 Colors.")]
	public class ColorCompare : FsmStateAction
	{
		// Token: 0x06006CBC RID: 27836 RVA: 0x0021E644 File Offset: 0x0021C844
		public override void Reset()
		{
			this.color1 = Color.white;
			this.color2 = Color.white;
			this.tolerance = 0f;
			this.equal = null;
			this.notEqual = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CBD RID: 27837 RVA: 0x0021E696 File Offset: 0x0021C896
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CBE RID: 27838 RVA: 0x0021E6AC File Offset: 0x0021C8AC
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06006CBF RID: 27839 RVA: 0x0021E6B4 File Offset: 0x0021C8B4
		private void DoCompare()
		{
			if (Mathf.Abs(this.color1.Value.r - this.color2.Value.r) > this.tolerance.Value || Mathf.Abs(this.color1.Value.g - this.color2.Value.g) > this.tolerance.Value || Mathf.Abs(this.color1.Value.b - this.color2.Value.b) > this.tolerance.Value || Mathf.Abs(this.color1.Value.a - this.color2.Value.a) > this.tolerance.Value)
			{
				base.Fsm.Event(this.notEqual);
				return;
			}
			base.Fsm.Event(this.equal);
		}

		// Token: 0x06006CC0 RID: 27840 RVA: 0x0021E7B3 File Offset: 0x0021C9B3
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.notEqual))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x04006C74 RID: 27764
		[RequiredField]
		[Tooltip("The first Color.")]
		public FsmColor color1;

		// Token: 0x04006C75 RID: 27765
		[RequiredField]
		[Tooltip("The second Color.")]
		public FsmColor color2;

		// Token: 0x04006C76 RID: 27766
		[RequiredField]
		[Tooltip("Tolerance of test, to test for 'almost equals' or to ignore small floating point rounding differences.")]
		public FsmFloat tolerance;

		// Token: 0x04006C77 RID: 27767
		[Tooltip("Event sent if Color 1 equals Color 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x04006C78 RID: 27768
		[Tooltip("Event sent if Color 1 does not equal Color 2 (within Tolerance)")]
		public FsmEvent notEqual;

		// Token: 0x04006C79 RID: 27769
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;
	}
}
