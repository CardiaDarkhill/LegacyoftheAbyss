using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200103F RID: 4159
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Returns the index of a scene in the Build Settings. Always returns -1 if the scene was loaded through an AssetBundle.")]
	public class GetSceneBuildIndex : GetSceneActionBase
	{
		// Token: 0x060071F6 RID: 29174 RVA: 0x00230908 File Offset: 0x0022EB08
		public override void Reset()
		{
			base.Reset();
			this.buildIndex = null;
		}

		// Token: 0x060071F7 RID: 29175 RVA: 0x00230917 File Offset: 0x0022EB17
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetSceneBuildIndex();
			base.Finish();
		}

		// Token: 0x060071F8 RID: 29176 RVA: 0x0023092B File Offset: 0x0022EB2B
		private void DoGetSceneBuildIndex()
		{
			if (!this._sceneFound)
			{
				return;
			}
			if (!this.buildIndex.IsNone)
			{
				this.buildIndex.Value = this._scene.buildIndex;
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071BD RID: 29117
		[ActionSection("Result")]
		[Tooltip("The scene Build Index")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt buildIndex;
	}
}
