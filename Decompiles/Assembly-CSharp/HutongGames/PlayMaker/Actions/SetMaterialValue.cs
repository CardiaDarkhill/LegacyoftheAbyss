using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F6A RID: 3946
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the value of a Material Variable.")]
	public class SetMaterialValue : FsmStateAction
	{
		// Token: 0x06006D78 RID: 28024 RVA: 0x00220915 File Offset: 0x0021EB15
		public override void Reset()
		{
			this.materialVariable = null;
			this.materialValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D79 RID: 28025 RVA: 0x0022092C File Offset: 0x0021EB2C
		public override void OnEnter()
		{
			this.materialVariable.Value = this.materialValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D7A RID: 28026 RVA: 0x00220952 File Offset: 0x0021EB52
		public override void OnUpdate()
		{
			this.materialVariable.Value = this.materialValue.Value;
		}

		// Token: 0x04006D3D RID: 27965
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Material Variable.")]
		public FsmMaterial materialVariable;

		// Token: 0x04006D3E RID: 27966
		[RequiredField]
		[Tooltip("Material Value.")]
		public FsmMaterial materialValue;

		// Token: 0x04006D3F RID: 27967
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
