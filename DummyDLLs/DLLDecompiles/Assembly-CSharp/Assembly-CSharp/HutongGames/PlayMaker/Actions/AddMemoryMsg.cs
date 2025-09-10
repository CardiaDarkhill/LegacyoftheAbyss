using System;
using TeamCherry.Localization;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001350 RID: 4944
	public class AddMemoryMsg : FsmStateAction
	{
		// Token: 0x06007FB6 RID: 32694 RVA: 0x0025C559 File Offset: 0x0025A759
		public override void Reset()
		{
			this.Source = null;
			this.Text = null;
		}

		// Token: 0x06007FB7 RID: 32695 RVA: 0x0025C56C File Offset: 0x0025A76C
		public override void OnEnter()
		{
			GameObject safe = this.Source.GetSafe(this);
			if (safe)
			{
				MemoryMsgBox.AddText(safe, this.Text);
			}
			base.Finish();
		}

		// Token: 0x04007F3B RID: 32571
		public FsmOwnerDefault Source;

		// Token: 0x04007F3C RID: 32572
		public LocalisedFsmString Text;
	}
}
