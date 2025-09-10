using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001269 RID: 4713
	public sealed class HeroLockState : FsmStateAction
	{
		// Token: 0x06007C53 RID: 31827 RVA: 0x00252DA0 File Offset: 0x00250FA0
		public override void Reset()
		{
			this.heroLockStates = HeroLockStates.None;
		}

		// Token: 0x06007C54 RID: 31828 RVA: 0x00252DA9 File Offset: 0x00250FA9
		public override void OnEnter()
		{
			this.hc = HeroController.instance;
			if (this.hc != null)
			{
				this.Add();
			}
			base.Finish();
		}

		// Token: 0x06007C55 RID: 31829 RVA: 0x00252DD0 File Offset: 0x00250FD0
		public override void OnExit()
		{
			if (this.setOppositeOnExit.Value && this.hc != null)
			{
				this.Remove();
			}
		}

		// Token: 0x06007C56 RID: 31830 RVA: 0x00252DF4 File Offset: 0x00250FF4
		private void Add()
		{
			HeroLockState.Mode mode = this.mode;
			if (mode == HeroLockState.Mode.Add)
			{
				this.hc.AddLockStates(this.heroLockStates);
				return;
			}
			if (mode != HeroLockState.Mode.Remove)
			{
				return;
			}
			this.hc.RemoveLockStates(this.heroLockStates);
		}

		// Token: 0x06007C57 RID: 31831 RVA: 0x00252E34 File Offset: 0x00251034
		private void Remove()
		{
			HeroLockState.Mode mode = this.mode;
			if (mode == HeroLockState.Mode.Add)
			{
				this.hc.RemoveLockStates(this.heroLockStates);
				return;
			}
			if (mode != HeroLockState.Mode.Remove)
			{
				return;
			}
			this.hc.AddLockStates(this.heroLockStates);
		}

		// Token: 0x04007C65 RID: 31845
		public HeroLockStates heroLockStates;

		// Token: 0x04007C66 RID: 31846
		public HeroLockState.Mode mode;

		// Token: 0x04007C67 RID: 31847
		public FsmBool setOppositeOnExit;

		// Token: 0x04007C68 RID: 31848
		private HeroController hc;

		// Token: 0x02001BE6 RID: 7142
		public enum Mode
		{
			// Token: 0x04009F80 RID: 40832
			Add,
			// Token: 0x04009F81 RID: 40833
			Remove
		}
	}
}
