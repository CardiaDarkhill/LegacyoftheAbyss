using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF0 RID: 3056
	[ActionCategory(ActionCategory.Transform)]
	public class CheckXPosition : FsmStateAction
	{
		// Token: 0x06005D8C RID: 23948 RVA: 0x001D7F20 File Offset: 0x001D6120
		public override void Reset()
		{
			this.gameObject = null;
			this.compareTo = 0f;
			this.tolerance = 0f;
			this.equal = null;
			this.lessThan = null;
			this.greaterThan = null;
			this.equalBool = null;
			this.lessThanBool = null;
			this.greaterThanBool = null;
			this.everyFrame = false;
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06005D8D RID: 23949 RVA: 0x001D7F97 File Offset: 0x001D6197
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D8E RID: 23950 RVA: 0x001D7FAD File Offset: 0x001D61AD
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06005D8F RID: 23951 RVA: 0x001D7FB8 File Offset: 0x001D61B8
		private void DoCompare()
		{
			if (this.activeBool.IsNone || this.activeBool.Value)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				float num = 0f;
				if (this.space == Space.World)
				{
					num = ownerDefaultTarget.transform.position.x;
				}
				if (this.space == Space.Self)
				{
					num = ownerDefaultTarget.transform.localPosition.x;
				}
				float num2 = this.compareTo.Value + this.compareToOffset.Value;
				if (Mathf.Abs(num - num2) <= this.tolerance.Value)
				{
					base.Fsm.Event(this.equal);
					this.equalBool.Value = true;
					this.greaterThanBool.Value = false;
					this.lessThanBool.Value = false;
					return;
				}
				if (num < num2)
				{
					base.Fsm.Event(this.lessThan);
					this.equalBool.Value = false;
					this.greaterThanBool.Value = false;
					this.lessThanBool.Value = true;
					return;
				}
				if (num > num2)
				{
					base.Fsm.Event(this.greaterThan);
					this.equalBool.Value = false;
					this.greaterThanBool.Value = true;
					this.lessThanBool.Value = false;
				}
			}
		}

		// Token: 0x040059C4 RID: 22980
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040059C5 RID: 22981
		[RequiredField]
		public FsmFloat compareTo;

		// Token: 0x040059C6 RID: 22982
		public FsmFloat compareToOffset;

		// Token: 0x040059C7 RID: 22983
		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).\nNOTE: Floats that look the same are often not exactly the same, so you often need to use a small tolerance.")]
		public FsmFloat tolerance;

		// Token: 0x040059C8 RID: 22984
		[Tooltip("Event sent if Float 1 equals Float 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x040059C9 RID: 22985
		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		// Token: 0x040059CA RID: 22986
		[Tooltip("Event sent if Float 1 is less than Float 2")]
		public FsmEvent lessThan;

		// Token: 0x040059CB RID: 22987
		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		// Token: 0x040059CC RID: 22988
		[Tooltip("Event sent if Float 1 is greater than Float 2")]
		public FsmEvent greaterThan;

		// Token: 0x040059CD RID: 22989
		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		// Token: 0x040059CE RID: 22990
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		// Token: 0x040059CF RID: 22991
		public Space space;

		// Token: 0x040059D0 RID: 22992
		public FsmBool activeBool;
	}
}
