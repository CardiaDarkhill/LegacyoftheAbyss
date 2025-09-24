using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012F2 RID: 4850
	public class GetAngleForThreadPosses : FsmStateAction
	{
		// Token: 0x06007E50 RID: 32336 RVA: 0x00258A88 File Offset: 0x00256C88
		public override void Reset()
		{
			this.StoreAngle = null;
		}

		// Token: 0x06007E51 RID: 32337 RVA: 0x00258A94 File Offset: 0x00256C94
		public override void OnEnter()
		{
			GameManager instance = GameManager.instance;
			if (instance)
			{
				CustomSceneManager sm = instance.sm;
				this.StoreAngle.Value = sm.AngleToSilkThread;
			}
			base.Finish();
		}

		// Token: 0x04007E15 RID: 32277
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreAngle;
	}
}
