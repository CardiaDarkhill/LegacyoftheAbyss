using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001355 RID: 4949
	public class ShowCustomNeedolinMsgFromTemplate : FsmStateAction
	{
		// Token: 0x06007FC8 RID: 32712 RVA: 0x0025C79B File Offset: 0x0025A99B
		public override void Reset()
		{
			this.Template = null;
			this.Timer = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007FC9 RID: 32713 RVA: 0x0025C7B8 File Offset: 0x0025A9B8
		public override void OnEnter()
		{
			this.textWrapper = new ShowCustomNeedolinMsgFromTemplate.TextWrapper(this.Template);
			if (this.Template.Key.Value == "")
			{
				base.Finish();
				return;
			}
			NeedolinMsgBox.AddText(this.textWrapper, true, true);
		}

		// Token: 0x06007FCA RID: 32714 RVA: 0x0025C80B File Offset: 0x0025AA0B
		public override void OnUpdate()
		{
			if (this.Timer.Value > 0f && base.State.StateTime >= this.Timer.Value)
			{
				this.End();
			}
		}

		// Token: 0x06007FCB RID: 32715 RVA: 0x0025C83D File Offset: 0x0025AA3D
		public override void OnExit()
		{
			this.End();
		}

		// Token: 0x06007FCC RID: 32716 RVA: 0x0025C845 File Offset: 0x0025AA45
		private void End()
		{
			if (this.textWrapper == null)
			{
				return;
			}
			NeedolinMsgBox.RemoveText(this.textWrapper);
			base.Finish();
		}

		// Token: 0x04007F46 RID: 32582
		public LocalisedFsmString Template;

		// Token: 0x04007F47 RID: 32583
		public FsmFloat Timer;

		// Token: 0x04007F48 RID: 32584
		private ShowCustomNeedolinMsgFromTemplate.TextWrapper textWrapper;

		// Token: 0x02001BFA RID: 7162
		public class TextWrapper : ILocalisedTextCollection
		{
			// Token: 0x06009AA0 RID: 39584 RVA: 0x002B3F86 File Offset: 0x002B2186
			public TextWrapper(LocalisedString template)
			{
				this.data = new LocalisedTextCollectionData(template);
			}

			// Token: 0x170011B8 RID: 4536
			// (get) Token: 0x06009AA1 RID: 39585 RVA: 0x002B3F9A File Offset: 0x002B219A
			public bool IsActive
			{
				get
				{
					return this.data.IsActive;
				}
			}

			// Token: 0x06009AA2 RID: 39586 RVA: 0x002B3FA7 File Offset: 0x002B21A7
			public LocalisedString GetRandom(LocalisedString skipString)
			{
				return this.data.GetRandom(skipString);
			}

			// Token: 0x04009FBD RID: 40893
			private readonly LocalisedTextCollectionData data;
		}
	}
}
