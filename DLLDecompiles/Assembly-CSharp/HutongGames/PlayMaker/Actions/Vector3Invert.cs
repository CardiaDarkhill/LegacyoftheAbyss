using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A4 RID: 4516
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Reverses the direction of a Vector3 Variable. Same as multiplying by -1.")]
	public class Vector3Invert : FsmStateAction
	{
		// Token: 0x060078C6 RID: 30918 RVA: 0x00248C13 File Offset: 0x00246E13
		public override void Reset()
		{
			this.vector3Variable = null;
			this.everyFrame = false;
		}

		// Token: 0x060078C7 RID: 30919 RVA: 0x00248C23 File Offset: 0x00246E23
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * -1f;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078C8 RID: 30920 RVA: 0x00248C53 File Offset: 0x00246E53
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * -1f;
		}

		// Token: 0x0400792C RID: 31020
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector3 variable to invert.")]
		public FsmVector3 vector3Variable;

		// Token: 0x0400792D RID: 31021
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
