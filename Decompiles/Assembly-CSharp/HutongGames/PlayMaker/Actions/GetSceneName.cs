using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001046 RID: 4166
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene name.")]
	public class GetSceneName : GetSceneActionBase
	{
		// Token: 0x06007215 RID: 29205 RVA: 0x00230D66 File Offset: 0x0022EF66
		public override void Reset()
		{
			base.Reset();
			this.name = null;
		}

		// Token: 0x06007216 RID: 29206 RVA: 0x00230D75 File Offset: 0x0022EF75
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetSceneName();
			base.Finish();
		}

		// Token: 0x06007217 RID: 29207 RVA: 0x00230D89 File Offset: 0x0022EF89
		private void DoGetSceneName()
		{
			if (!this._sceneFound)
			{
				return;
			}
			if (!this.name.IsNone)
			{
				this.name.Value = this._scene.name;
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071D6 RID: 29142
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The scene name")]
		public FsmString name;
	}
}
