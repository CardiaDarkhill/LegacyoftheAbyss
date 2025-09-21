using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF3 RID: 3059
	[ActionCategory(ActionCategory.Transform)]
	public class CheckZPosition : FsmStateAction
	{
		// Token: 0x06005D9C RID: 23964 RVA: 0x001D84C0 File Offset: 0x001D66C0
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
		}

		// Token: 0x06005D9D RID: 23965 RVA: 0x001D8520 File Offset: 0x001D6720
		public override void OnEnter()
		{
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D9E RID: 23966 RVA: 0x001D8536 File Offset: 0x001D6736
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06005D9F RID: 23967 RVA: 0x001D8540 File Offset: 0x001D6740
		private void DoCompare()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			float num = 0f;
			if (this.space == Space.World)
			{
				num = ownerDefaultTarget.transform.position.z;
			}
			if (this.space == Space.Self)
			{
				num = ownerDefaultTarget.transform.localPosition.z;
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

		// Token: 0x06005DA0 RID: 23968 RVA: 0x001D860C File Offset: 0x001D680C
		public override string ErrorCheck()
		{
			if (FsmEvent.IsNullOrEmpty(this.equal) && FsmEvent.IsNullOrEmpty(this.lessThan) && FsmEvent.IsNullOrEmpty(this.greaterThan))
			{
				return "Action sends no events!";
			}
			return "";
		}

		// Token: 0x040059E8 RID: 23016
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040059E9 RID: 23017
		[RequiredField]
		public FsmFloat compareTo;

		// Token: 0x040059EA RID: 23018
		public FsmFloat compareToOffset;

		// Token: 0x040059EB RID: 23019
		[RequiredField]
		[Tooltip("Tolerance for the Equal test (almost equal).\nNOTE: Floats that look the same are often not exactly the same, so you often need to use a small tolerance.")]
		public FsmFloat tolerance;

		// Token: 0x040059EC RID: 23020
		[Tooltip("Event sent if Float 1 equals Float 2 (within Tolerance)")]
		public FsmEvent equal;

		// Token: 0x040059ED RID: 23021
		[Tooltip("Event sent if Float 1 is less than Float 2")]
		public FsmEvent lessThan;

		// Token: 0x040059EE RID: 23022
		[Tooltip("Event sent if Float 1 is greater than Float 2")]
		public FsmEvent greaterThan;

		// Token: 0x040059EF RID: 23023
		[Tooltip("Repeat every frame. Useful if the variables are changing and you're waiting for a particular result.")]
		public bool everyFrame;

		// Token: 0x040059F0 RID: 23024
		public Space space;
	}
}
