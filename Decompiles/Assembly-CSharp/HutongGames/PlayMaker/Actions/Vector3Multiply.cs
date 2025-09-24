using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A7 RID: 4519
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Multiplies a Vector3 variable by a Float.")]
	public class Vector3Multiply : FsmStateAction
	{
		// Token: 0x060078D3 RID: 30931 RVA: 0x00248E75 File Offset: 0x00247075
		public override void Reset()
		{
			this.vector3Variable = null;
			this.multiplyBy = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060078D4 RID: 30932 RVA: 0x00248E95 File Offset: 0x00247095
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * this.multiplyBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078D5 RID: 30933 RVA: 0x00248ECB File Offset: 0x002470CB
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * this.multiplyBy.Value;
		}

		// Token: 0x04007936 RID: 31030
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector3 variable to multiply.")]
		public FsmVector3 vector3Variable;

		// Token: 0x04007937 RID: 31031
		[RequiredField]
		[Tooltip("The float to multiply each axis of the Vector3 variable by.")]
		public FsmFloat multiplyBy;

		// Token: 0x04007938 RID: 31032
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
