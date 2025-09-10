using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200104A RID: 4170
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene Root GameObjects.")]
	public class GetSceneRootGameObjects : GetSceneActionBase
	{
		// Token: 0x06007226 RID: 29222 RVA: 0x00231091 File Offset: 0x0022F291
		public override void Reset()
		{
			base.Reset();
			this.rootGameObjects = null;
			this.everyFrame = false;
		}

		// Token: 0x06007227 RID: 29223 RVA: 0x002310A7 File Offset: 0x0022F2A7
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetSceneRootGameObjects();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007228 RID: 29224 RVA: 0x002310C3 File Offset: 0x0022F2C3
		public override void OnUpdate()
		{
			this.DoGetSceneRootGameObjects();
		}

		// Token: 0x06007229 RID: 29225 RVA: 0x002310CC File Offset: 0x0022F2CC
		private void DoGetSceneRootGameObjects()
		{
			if (!this._sceneFound)
			{
				return;
			}
			if (!this.rootGameObjects.IsNone)
			{
				FsmArray fsmArray = this.rootGameObjects;
				object[] values = this._scene.GetRootGameObjects();
				fsmArray.Values = values;
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071E3 RID: 29155
		[ActionSection("Result")]
		[Tooltip("The scene Root GameObjects")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray rootGameObjects;

		// Token: 0x040071E4 RID: 29156
		[Tooltip("Repeat every Frame")]
		public bool everyFrame;
	}
}
