using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001043 RID: 4163
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene isLoaded flag.")]
	public class GetSceneIsLoaded : GetSceneActionBase
	{
		// Token: 0x06007208 RID: 29192 RVA: 0x00230A6C File Offset: 0x0022EC6C
		public override void Reset()
		{
			base.Reset();
			this.isLoaded = null;
			this.everyFrame = false;
		}

		// Token: 0x06007209 RID: 29193 RVA: 0x00230A82 File Offset: 0x0022EC82
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetSceneIsLoaded();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600720A RID: 29194 RVA: 0x00230A9E File Offset: 0x0022EC9E
		public override void OnUpdate()
		{
			this.DoGetSceneIsLoaded();
		}

		// Token: 0x0600720B RID: 29195 RVA: 0x00230AA6 File Offset: 0x0022ECA6
		private void DoGetSceneIsLoaded()
		{
			if (!this._sceneFound)
			{
				return;
			}
			if (!this.isLoaded.IsNone)
			{
				this.isLoaded.Value = this._scene.isLoaded;
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071C5 RID: 29125
		[ActionSection("Result")]
		[Tooltip("true if the scene is loaded.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isLoaded;

		// Token: 0x040071C6 RID: 29126
		[Tooltip("Event sent if the scene is loaded.")]
		public FsmEvent isLoadedEvent;

		// Token: 0x040071C7 RID: 29127
		[Tooltip("Event sent if the scene is not loaded.")]
		public FsmEvent isNotLoadedEvent;

		// Token: 0x040071C8 RID: 29128
		[Tooltip("Repeat every Frame")]
		public bool everyFrame;
	}
}
