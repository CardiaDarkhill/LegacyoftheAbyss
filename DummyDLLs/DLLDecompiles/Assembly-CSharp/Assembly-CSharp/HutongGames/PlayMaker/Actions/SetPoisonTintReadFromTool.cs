using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001297 RID: 4759
	public class SetPoisonTintReadFromTool : FsmStateAction
	{
		// Token: 0x06007CFB RID: 31995 RVA: 0x00255229 File Offset: 0x00253429
		public override void Reset()
		{
			this.Target = null;
			this.Tool = null;
		}

		// Token: 0x06007CFC RID: 31996 RVA: 0x00255239 File Offset: 0x00253439
		public override void OnEnter()
		{
			this.Target.GetSafe(this).GetComponent<PoisonTintBase>().ReadFromTool = (this.Tool.Value as ToolItem);
			base.Finish();
		}

		// Token: 0x04007D0E RID: 32014
		[CheckForComponent(typeof(PoisonTintBase))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007D0F RID: 32015
		[RequiredField]
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;
	}
}
