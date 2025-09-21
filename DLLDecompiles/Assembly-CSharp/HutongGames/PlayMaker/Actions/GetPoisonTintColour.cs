using System;
using GlobalSettings;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C79 RID: 3193
	[ActionCategory(ActionCategory.Color)]
	public class GetPoisonTintColour : FsmStateAction
	{
		// Token: 0x06006039 RID: 24633 RVA: 0x001E771D File Offset: 0x001E591D
		public override void Reset()
		{
			this.colorVariable = null;
		}

		// Token: 0x0600603A RID: 24634 RVA: 0x001E7726 File Offset: 0x001E5926
		public override void OnEnter()
		{
			this.DoSetColorValue();
			base.Finish();
		}

		// Token: 0x0600603B RID: 24635 RVA: 0x001E7734 File Offset: 0x001E5934
		private void DoSetColorValue()
		{
			if (this.colorVariable != null)
			{
				this.colorVariable.Value = Gameplay.PoisonPouchTintColour;
			}
		}

		// Token: 0x04005D90 RID: 23952
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color Variable in which to store poison tint.")]
		public FsmColor colorVariable;
	}
}
