using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001042 RID: 4162
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene isDirty flag. True if the scene is modified.")]
	public class GetSceneIsDirty : GetSceneActionBase
	{
		// Token: 0x06007203 RID: 29187 RVA: 0x002309EB File Offset: 0x0022EBEB
		public override void Reset()
		{
			base.Reset();
			this.isDirty = null;
			this.everyFrame = false;
		}

		// Token: 0x06007204 RID: 29188 RVA: 0x00230A01 File Offset: 0x0022EC01
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetSceneIsDirty();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007205 RID: 29189 RVA: 0x00230A1D File Offset: 0x0022EC1D
		public override void OnUpdate()
		{
			this.DoGetSceneIsDirty();
		}

		// Token: 0x06007206 RID: 29190 RVA: 0x00230A25 File Offset: 0x0022EC25
		private void DoGetSceneIsDirty()
		{
			if (!this._sceneFound)
			{
				return;
			}
			if (!this.isDirty.IsNone)
			{
				this.isDirty.Value = this._scene.isDirty;
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071C1 RID: 29121
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the scene is modified.")]
		public FsmBool isDirty;

		// Token: 0x040071C2 RID: 29122
		[Tooltip("Event sent if the scene is modified.")]
		public FsmEvent isDirtyEvent;

		// Token: 0x040071C3 RID: 29123
		[Tooltip("Event sent if the scene is unmodified.")]
		public FsmEvent isNotDirtyEvent;

		// Token: 0x040071C4 RID: 29124
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
