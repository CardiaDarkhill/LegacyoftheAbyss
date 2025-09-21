using System;
using TeamCherry.Localization;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001354 RID: 4948
	public class ShowCustomNeedolinMsg : FsmStateAction
	{
		// Token: 0x06007FC2 RID: 32706 RVA: 0x0025C6F7 File Offset: 0x0025A8F7
		public override void Reset()
		{
			this.Text = null;
			this.Timer = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06007FC3 RID: 32707 RVA: 0x0025C712 File Offset: 0x0025A912
		public override void OnEnter()
		{
			this.textWrapper = new ShowCustomNeedolinMsg.TextWrapper
			{
				Text = this.Text
			};
			NeedolinMsgBox.AddText(this.textWrapper, true, true);
		}

		// Token: 0x06007FC4 RID: 32708 RVA: 0x0025C73D File Offset: 0x0025A93D
		public override void OnUpdate()
		{
			if (this.Timer.Value > 0f && base.State.StateTime >= this.Timer.Value)
			{
				this.End();
			}
		}

		// Token: 0x06007FC5 RID: 32709 RVA: 0x0025C76F File Offset: 0x0025A96F
		public override void OnExit()
		{
			this.End();
		}

		// Token: 0x06007FC6 RID: 32710 RVA: 0x0025C777 File Offset: 0x0025A977
		private void End()
		{
			if (this.textWrapper == null)
			{
				return;
			}
			NeedolinMsgBox.RemoveText(this.textWrapper);
			base.Finish();
		}

		// Token: 0x04007F43 RID: 32579
		public LocalisedFsmString Text;

		// Token: 0x04007F44 RID: 32580
		public FsmFloat Timer;

		// Token: 0x04007F45 RID: 32581
		private ShowCustomNeedolinMsg.TextWrapper textWrapper;

		// Token: 0x02001BF9 RID: 7161
		public class TextWrapper : ILocalisedTextCollection
		{
			// Token: 0x170011B7 RID: 4535
			// (get) Token: 0x06009A9D RID: 39581 RVA: 0x002B3F73 File Offset: 0x002B2173
			public bool IsActive
			{
				get
				{
					return true;
				}
			}

			// Token: 0x06009A9E RID: 39582 RVA: 0x002B3F76 File Offset: 0x002B2176
			public LocalisedString GetRandom(LocalisedString skipString)
			{
				return this.Text;
			}

			// Token: 0x04009FBC RID: 40892
			public LocalisedString Text;
		}
	}
}
