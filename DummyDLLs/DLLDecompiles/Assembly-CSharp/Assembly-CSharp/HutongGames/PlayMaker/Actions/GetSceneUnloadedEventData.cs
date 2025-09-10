using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200104B RID: 4171
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get the last Unloaded Scene Event data when event was sent from the action 'SendSceneUnloadedEvent")]
	public class GetSceneUnloadedEventData : FsmStateAction
	{
		// Token: 0x0600722B RID: 29227 RVA: 0x00231120 File Offset: 0x0022F320
		public override void Reset()
		{
			this.name = null;
			this.path = null;
			this.buildIndex = null;
			this.isLoaded = null;
			this.rootCount = null;
			this.rootGameObjects = null;
			this.isDirty = null;
			this.everyFrame = false;
		}

		// Token: 0x0600722C RID: 29228 RVA: 0x0023115A File Offset: 0x0022F35A
		public override void OnEnter()
		{
			this.DoGetSceneProperties();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600722D RID: 29229 RVA: 0x00231170 File Offset: 0x0022F370
		public override void OnUpdate()
		{
			this.DoGetSceneProperties();
		}

		// Token: 0x0600722E RID: 29230 RVA: 0x00231178 File Offset: 0x0022F378
		private void DoGetSceneProperties()
		{
			this._scene = SendSceneUnloadedEvent.lastUnLoadedScene;
			if (!this.name.IsNone)
			{
				this.name.Value = this._scene.name;
			}
			if (!this.buildIndex.IsNone)
			{
				this.buildIndex.Value = this._scene.buildIndex;
			}
			if (!this.path.IsNone)
			{
				this.path.Value = this._scene.path;
			}
			if (!this.isValid.IsNone)
			{
				this.isValid.Value = this._scene.IsValid();
			}
			if (!this.isDirty.IsNone)
			{
				this.isDirty.Value = this._scene.isDirty;
			}
			if (!this.isLoaded.IsNone)
			{
				this.isLoaded.Value = this._scene.isLoaded;
			}
			if (!this.rootCount.IsNone)
			{
				this.rootCount.Value = this._scene.rootCount;
			}
			if (!this.rootGameObjects.IsNone)
			{
				if (this._scene.IsValid())
				{
					FsmArray fsmArray = this.rootGameObjects;
					object[] values = this._scene.GetRootGameObjects();
					fsmArray.Values = values;
					return;
				}
				this.rootGameObjects.Resize(0);
			}
		}

		// Token: 0x040071E5 RID: 29157
		[UIHint(UIHint.Variable)]
		[Tooltip("The scene name")]
		public FsmString name;

		// Token: 0x040071E6 RID: 29158
		[Tooltip("The scene path")]
		[UIHint(UIHint.Variable)]
		public FsmString path;

		// Token: 0x040071E7 RID: 29159
		[Tooltip("The scene Build Index")]
		[UIHint(UIHint.Variable)]
		public FsmInt buildIndex;

		// Token: 0x040071E8 RID: 29160
		[Tooltip("true if the scene is valid.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isValid;

		// Token: 0x040071E9 RID: 29161
		[Tooltip("true if the scene is loaded.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isLoaded;

		// Token: 0x040071EA RID: 29162
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the scene is modified.")]
		public FsmBool isDirty;

		// Token: 0x040071EB RID: 29163
		[Tooltip("The scene RootCount")]
		[UIHint(UIHint.Variable)]
		public FsmInt rootCount;

		// Token: 0x040071EC RID: 29164
		[Tooltip("The scene Root GameObjects")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray rootGameObjects;

		// Token: 0x040071ED RID: 29165
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x040071EE RID: 29166
		private Scene _scene;
	}
}
