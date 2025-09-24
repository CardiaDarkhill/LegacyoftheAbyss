using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF2 RID: 3058
	[ActionCategory(ActionCategory.Transform)]
	public class CheckYPositionV2 : FsmStateAction
	{
		// Token: 0x06005D97 RID: 23959 RVA: 0x001D82D0 File Offset: 0x001D64D0
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

		// Token: 0x06005D98 RID: 23960 RVA: 0x001D8342 File Offset: 0x001D6542
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D99 RID: 23961 RVA: 0x001D8358 File Offset: 0x001D6558
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06005D9A RID: 23962 RVA: 0x001D8360 File Offset: 0x001D6560
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

		// Token: 0x040059DB RID: 23003
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040059DC RID: 23004
		[RequiredField]
		public FsmFloat compareTo;

		// Token: 0x040059DD RID: 23005
		public FsmFloat compareToOffset;

		// Token: 0x040059DE RID: 23006
		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).\nNOTE: Floats that look the same are often not exactly the same, so you often need to use a small tolerance.")]
		public FsmFloat tolerance;

		// Token: 0x040059DF RID: 23007
		[Tooltip("Event sent if Float 1 equals Float 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x040059E0 RID: 23008
		[UIHint(UIHint.Variable)]
		public FsmBool equalBool;

		// Token: 0x040059E1 RID: 23009
		[Tooltip("Event sent if Float 1 is less than Float 2")]
		public FsmEvent lessThan;

		// Token: 0x040059E2 RID: 23010
		[UIHint(UIHint.Variable)]
		public FsmBool lessThanBool;

		// Token: 0x040059E3 RID: 23011
		[Tooltip("Event sent if Float 1 is greater than Float 2")]
		public FsmEvent greaterThan;

		// Token: 0x040059E4 RID: 23012
		[UIHint(UIHint.Variable)]
		public FsmBool greaterThanBool;

		// Token: 0x040059E5 RID: 23013
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		// Token: 0x040059E6 RID: 23014
		public Space space;

		// Token: 0x040059E7 RID: 23015
		public FsmBool activeBool;
	}
}
