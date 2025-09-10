using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001049 RID: 4169
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene RootCount, the number of root transforms of this scene.")]
	public class GetSceneRootCount : GetSceneActionBase
	{
		// Token: 0x06007221 RID: 29217 RVA: 0x00231010 File Offset: 0x0022F210
		public override void Reset()
		{
			base.Reset();
			this.rootCount = null;
			this.everyFrame = false;
		}

		// Token: 0x06007222 RID: 29218 RVA: 0x00231026 File Offset: 0x0022F226
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetSceneRootCount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007223 RID: 29219 RVA: 0x00231042 File Offset: 0x0022F242
		public override void OnUpdate()
		{
			this.DoGetSceneRootCount();
		}

		// Token: 0x06007224 RID: 29220 RVA: 0x0023104A File Offset: 0x0022F24A
		private void DoGetSceneRootCount()
		{
			if (!this._sceneFound)
			{
				return;
			}
			if (!this.rootCount.IsNone)
			{
				this.rootCount.Value = this._scene.rootCount;
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071E1 RID: 29153
		[ActionSection("Result")]
		[Tooltip("The scene RootCount")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt rootCount;

		// Token: 0x040071E2 RID: 29154
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
