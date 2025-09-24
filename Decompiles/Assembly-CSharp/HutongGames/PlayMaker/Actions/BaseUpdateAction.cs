using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E47 RID: 3655
	public abstract class BaseUpdateAction : FsmStateAction
	{
		// Token: 0x0600688B RID: 26763
		public abstract void OnActionUpdate();

		// Token: 0x0600688C RID: 26764 RVA: 0x0020D84E File Offset: 0x0020BA4E
		public override void Reset()
		{
			this.everyFrame = false;
			this.updateType = BaseUpdateAction.UpdateType.OnUpdate;
		}

		// Token: 0x0600688D RID: 26765 RVA: 0x0020D85E File Offset: 0x0020BA5E
		public override void OnPreprocess()
		{
			if (this.updateType == BaseUpdateAction.UpdateType.OnFixedUpdate)
			{
				base.Fsm.HandleFixedUpdate = true;
				return;
			}
			if (this.updateType == BaseUpdateAction.UpdateType.OnLateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x0600688E RID: 26766 RVA: 0x0020D88B File Offset: 0x0020BA8B
		public override void OnUpdate()
		{
			if (this.updateType == BaseUpdateAction.UpdateType.OnUpdate)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600688F RID: 26767 RVA: 0x0020D8A9 File Offset: 0x0020BAA9
		public override void OnLateUpdate()
		{
			if (this.updateType == BaseUpdateAction.UpdateType.OnLateUpdate)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006890 RID: 26768 RVA: 0x0020D8C8 File Offset: 0x0020BAC8
		public override void OnFixedUpdate()
		{
			if (this.updateType == BaseUpdateAction.UpdateType.OnFixedUpdate)
			{
				this.OnActionUpdate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x040067BC RID: 26556
		[ActionSection("Update type")]
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040067BD RID: 26557
		[Tooltip("When to update the action.\nOnUpdate: The most common setting.\nOnLateUpdate: Update after everything else. Useful if dependent on another GameObect, e.g. following.\nOnFixedUpdate: Used to update physics e.g., GameObjects with RigidBody components.")]
		public BaseUpdateAction.UpdateType updateType;

		// Token: 0x02001BA0 RID: 7072
		public enum UpdateType
		{
			// Token: 0x04009DF6 RID: 40438
			OnUpdate,
			// Token: 0x04009DF7 RID: 40439
			OnLateUpdate,
			// Token: 0x04009DF8 RID: 40440
			OnFixedUpdate
		}
	}
}
