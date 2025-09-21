using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF1 RID: 3057
	[ActionCategory(ActionCategory.Transform)]
	public class CheckYPosition : FsmStateAction
	{
		// Token: 0x06005D91 RID: 23953 RVA: 0x001D8118 File Offset: 0x001D6318
		public override void Reset()
		{
			this.gameObject = null;
			this.compareTo = 0f;
			this.compareToOffset = 0f;
			this.tolerance = 0f;
			this.equal = null;
			this.lessThan = null;
			this.greaterThan = null;
			this.everyFrame = false;
			this.activeBool = new FsmBool
			{
				UseVariable = true
			};
		}

		// Token: 0x06005D92 RID: 23954 RVA: 0x001D818A File Offset: 0x001D638A
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D93 RID: 23955 RVA: 0x001D81A0 File Offset: 0x001D63A0
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06005D94 RID: 23956 RVA: 0x001D81A8 File Offset: 0x001D63A8
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
					num = ownerDefaultTarget.transform.position.y;
				}
				if (this.space == Space.Self)
				{
					num = ownerDefaultTarget.transform.localPosition.y;
				}
				float num2 = this.compareTo.Value + this.compareToOffset.Value;
				if (Mathf.Abs(num - num2) <= this.tolerance.Value)
				{
					base.Fsm.Event(this.equal);
					return;
				}
				if (num < num2)
				{
					base.Fsm.Event(this.lessThan);
					return;
				}
				if (num > num2)
				{
					base.Fsm.Event(this.greaterThan);
				}
			}
		}

		// Token: 0x06005D95 RID: 23957 RVA: 0x001D8291 File Offset: 0x001D6491
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x040059D1 RID: 22993
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040059D2 RID: 22994
		[RequiredField]
		public FsmFloat compareTo;

		// Token: 0x040059D3 RID: 22995
		public FsmFloat compareToOffset;

		// Token: 0x040059D4 RID: 22996
		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).\nNOTE: Floats that look the same are often not exactly the same, so you often need to use a small tolerance.")]
		public FsmFloat tolerance;

		// Token: 0x040059D5 RID: 22997
		[Tooltip("Event sent if Float 1 equals Float 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x040059D6 RID: 22998
		[Tooltip("Event sent if Float 1 is less than Float 2")]
		public FsmEvent lessThan;

		// Token: 0x040059D7 RID: 22999
		[Tooltip("Event sent if Float 1 is greater than Float 2")]
		public FsmEvent greaterThan;

		// Token: 0x040059D8 RID: 23000
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		// Token: 0x040059D9 RID: 23001
		public Space space;

		// Token: 0x040059DA RID: 23002
		public FsmBool activeBool;
	}
}
