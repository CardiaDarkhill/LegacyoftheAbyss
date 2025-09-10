using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D28 RID: 3368
	public class SetDarknessLevel : FsmStateAction
	{
		// Token: 0x0600633B RID: 25403 RVA: 0x001F5B42 File Offset: 0x001F3D42
		public override void Reset()
		{
			this.SetLevel = null;
			this.StoreCurrentLevel = null;
		}

		// Token: 0x0600633C RID: 25404 RVA: 0x001F5B54 File Offset: 0x001F3D54
		public override void OnEnter()
		{
			if (!base.Owner.activeInHierarchy)
			{
				return;
			}
			this.StoreCurrentLevel.Value = DarknessRegion.GetDarknessLevel();
			if (!this.SetLevel.IsNone)
			{
				DarknessRegion.SetDarknessLevel(this.SetLevel.Value);
			}
			base.Finish();
		}

		// Token: 0x040061A6 RID: 24998
		public FsmInt SetLevel;

		// Token: 0x040061A7 RID: 24999
		[UIHint(UIHint.Variable)]
		public FsmInt StoreCurrentLevel;
	}
}
