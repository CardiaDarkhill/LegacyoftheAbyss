using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAA RID: 3498
	public abstract class FsmStateActionAdvanced : FsmStateAction
	{
		// Token: 0x06006594 RID: 26004
		public abstract void OnActionUpdate();

		// Token: 0x06006595 RID: 26005 RVA: 0x002011E3 File Offset: 0x001FF3E3
		public override void Reset()
		{
			this.everyFrame = false;
			this.updateType = FsmStateActionAdvanced.FrameUpdateSelector.OnUpdate;
		}

		// Token: 0x06006596 RID: 26006 RVA: 0x002011F3 File Offset: 0x001FF3F3
		public override void OnPreprocess()
		{
			base.OnPreprocess();
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006597 RID: 26007 RVA: 0x00201207 File Offset: 0x001FF407
		public override void OnUpdate()
		{
			if (this.updateType == FsmStateActionAdvanced.FrameUpdateSelector.OnUpdate)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006598 RID: 26008 RVA: 0x00201225 File Offset: 0x001FF425
		public override void OnLateUpdate()
		{
			if (this.updateType == FsmStateActionAdvanced.FrameUpdateSelector.OnLateUpdate)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006599 RID: 26009 RVA: 0x00201244 File Offset: 0x001FF444
		public override void OnFixedUpdate()
		{
			if (this.updateType == FsmStateActionAdvanced.FrameUpdateSelector.OnFixedUpdate)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x040064B5 RID: 25781
		[ActionSection("Update type")]
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040064B6 RID: 25782
		public FsmStateActionAdvanced.FrameUpdateSelector updateType;

		// Token: 0x02001B94 RID: 7060
		public enum FrameUpdateSelector
		{
			// Token: 0x04009DA5 RID: 40357
			OnUpdate,
			// Token: 0x04009DA6 RID: 40358
			OnLateUpdate,
			// Token: 0x04009DA7 RID: 40359
			OnFixedUpdate
		}
	}
}
