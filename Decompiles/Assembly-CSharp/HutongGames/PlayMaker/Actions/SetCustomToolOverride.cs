using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200132A RID: 4906
	public class SetCustomToolOverride : FsmStateAction
	{
		// Token: 0x06007F0F RID: 32527 RVA: 0x0025A763 File Offset: 0x00258963
		public bool IsToolNotSet()
		{
			return !this.Tool.Value;
		}

		// Token: 0x06007F10 RID: 32528 RVA: 0x0025A778 File Offset: 0x00258978
		public override void Reset()
		{
			this.Tool = null;
			this.Amount = new FsmInt
			{
				UseVariable = true
			};
			this.PromptTextSheet = null;
			this.PromptTextKey = null;
			this.PromptAppearDelay = 1f;
		}

		// Token: 0x06007F11 RID: 32529 RVA: 0x0025A7B4 File Offset: 0x002589B4
		public override void OnEnter()
		{
			ToolItem toolItem = this.Tool.Value as ToolItem;
			if (toolItem)
			{
				LocalisedString promptText = new LocalisedString(this.PromptTextSheet.Value, this.PromptTextKey.Value);
				ToolItemManager.SetCustomToolOverride(toolItem, this.Amount.IsNone ? null : new int?(this.Amount.Value), promptText, this.PromptAppearDelay.Value);
			}
			else
			{
				ToolItemManager.ClearCustomToolOverride();
			}
			base.Finish();
		}

		// Token: 0x04007EA5 RID: 32421
		[ObjectType(typeof(ToolItem))]
		public FsmObject Tool;

		// Token: 0x04007EA6 RID: 32422
		[HideIf("IsToolNotSet")]
		public FsmInt Amount;

		// Token: 0x04007EA7 RID: 32423
		[HideIf("IsToolNotSet")]
		public FsmString PromptTextSheet;

		// Token: 0x04007EA8 RID: 32424
		[HideIf("IsToolNotSet")]
		public FsmString PromptTextKey;

		// Token: 0x04007EA9 RID: 32425
		[HideIf("IsToolNotSet")]
		public FsmFloat PromptAppearDelay;
	}
}
