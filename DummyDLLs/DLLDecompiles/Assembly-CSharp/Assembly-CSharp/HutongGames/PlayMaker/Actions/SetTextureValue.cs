using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F6E RID: 3950
	[ActionCategory(ActionCategory.Material)]
	[Tooltip("Sets the value of a Texture Variable.")]
	public class SetTextureValue : FsmStateAction
	{
		// Token: 0x06006D8A RID: 28042 RVA: 0x00220D82 File Offset: 0x0021EF82
		public override void Reset()
		{
			this.textureVariable = null;
			this.textureValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06006D8B RID: 28043 RVA: 0x00220D99 File Offset: 0x0021EF99
		public override void OnEnter()
		{
			this.textureVariable.Value = this.textureValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D8C RID: 28044 RVA: 0x00220DBF File Offset: 0x0021EFBF
		public override void OnUpdate()
		{
			this.textureVariable.Value = this.textureValue.Value;
		}

		// Token: 0x04006D4F RID: 27983
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Texture Variable.")]
		public FsmTexture textureVariable;

		// Token: 0x04006D50 RID: 27984
		[RequiredField]
		[Tooltip("Texture Value.")]
		public FsmTexture textureValue;

		// Token: 0x04006D51 RID: 27985
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
