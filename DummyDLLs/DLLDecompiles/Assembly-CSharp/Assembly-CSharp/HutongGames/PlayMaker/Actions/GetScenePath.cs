using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001047 RID: 4167
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene path.")]
	public class GetScenePath : GetSceneActionBase
	{
		// Token: 0x06007219 RID: 29209 RVA: 0x00230DD0 File Offset: 0x0022EFD0
		public override void Reset()
		{
			base.Reset();
			this.path = null;
		}

		// Token: 0x0600721A RID: 29210 RVA: 0x00230DDF File Offset: 0x0022EFDF
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetScenePath();
			base.Finish();
		}

		// Token: 0x0600721B RID: 29211 RVA: 0x00230DF3 File Offset: 0x0022EFF3
		private void DoGetScenePath()
		{
			if (!this._sceneFound)
			{
				return;
			}
			if (!this.path.IsNone)
			{
				this.path.Value = this._scene.path;
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071D7 RID: 29143
		[ActionSection("Result")]
		[Tooltip("The scene path")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmString path;
	}
}
