using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A8 RID: 4520
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Normalizes a Vector3 Variable. A normalized vector has a length of 1.")]
	public class Vector3Normalize : FsmStateAction
	{
		// Token: 0x060078D7 RID: 30935 RVA: 0x00248EFB File Offset: 0x002470FB
		public override void Reset()
		{
			this.vector3Variable = null;
			this.everyFrame = false;
		}

		// Token: 0x060078D8 RID: 30936 RVA: 0x00248F0C File Offset: 0x0024710C
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value.normalized;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078D9 RID: 30937 RVA: 0x00248F48 File Offset: 0x00247148
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value.normalized;
		}

		// Token: 0x04007939 RID: 31033
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 Variable to normalize.")]
		public FsmVector3 vector3Variable;

		// Token: 0x0400793A RID: 31034
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
