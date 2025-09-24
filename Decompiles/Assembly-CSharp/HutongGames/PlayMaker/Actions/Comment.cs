using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E76 RID: 3702
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Adds a text area to the action list for notes etc. Use this to document your project.")]
	public class Comment : FsmStateAction
	{
		// Token: 0x0600697D RID: 27005 RVA: 0x00210B04 File Offset: 0x0020ED04
		public override void Reset()
		{
			this.comment = "Double-Click To Edit";
		}

		// Token: 0x0600697E RID: 27006 RVA: 0x00210B11 File Offset: 0x0020ED11
		public override void OnEnter()
		{
			base.Finish();
		}

		// Token: 0x040068B8 RID: 26808
		[UIHint(UIHint.Comment)]
		[Tooltip("Any comment you care to make...")]
		public string comment;
	}
}
