using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001045 RID: 4165
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get the last Loaded Scene Event data when event was sent from the action 'SendSceneLoadedEvent")]
	public class GetSceneLoadedEventData : FsmStateAction
	{
		// Token: 0x06007211 RID: 29201 RVA: 0x00230B94 File Offset: 0x0022ED94
		public override void Reset()
		{
			this.loadedMode = null;
			this.name = null;
			this.path = null;
			this.isValid = null;
			this.buildIndex = null;
			this.isLoaded = null;
			this.rootCount = null;
			this.rootGameObjects = null;
			this.isDirty = null;
		}

		// Token: 0x06007212 RID: 29202 RVA: 0x00230BE0 File Offset: 0x0022EDE0
		public override void OnEnter()
		{
			this.DoGetSceneProperties();
			base.Finish();
		}

		// Token: 0x06007213 RID: 29203 RVA: 0x00230BF0 File Offset: 0x0022EDF0
		private void DoGetSceneProperties()
		{
			this._scene = SendSceneLoadedEvent.lastLoadedScene;
			if (!this.name.IsNone)
			{
				this.loadedMode.Value = SendSceneLoadedEvent.lastLoadedMode;
			}
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

		// Token: 0x040071CC RID: 29132
		[UIHint(UIHint.Variable)]
		[Tooltip("The scene loaded mode")]
		[ObjectType(typeof(LoadSceneMode))]
		public FsmEnum loadedMode;

		// Token: 0x040071CD RID: 29133
		[UIHint(UIHint.Variable)]
		[Tooltip("The scene name")]
		public FsmString name;

		// Token: 0x040071CE RID: 29134
		[Tooltip("The scene path")]
		[UIHint(UIHint.Variable)]
		public FsmString path;

		// Token: 0x040071CF RID: 29135
		[Tooltip("true if the scene is valid.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isValid;

		// Token: 0x040071D0 RID: 29136
		[Tooltip("The scene Build Index")]
		[UIHint(UIHint.Variable)]
		public FsmInt buildIndex;

		// Token: 0x040071D1 RID: 29137
		[Tooltip("true if the scene is loaded.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isLoaded;

		// Token: 0x040071D2 RID: 29138
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the scene is modified.")]
		public FsmBool isDirty;

		// Token: 0x040071D3 RID: 29139
		[Tooltip("The scene RootCount")]
		[UIHint(UIHint.Variable)]
		public FsmInt rootCount;

		// Token: 0x040071D4 RID: 29140
		[Tooltip("The scene Root GameObjects")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray rootGameObjects;

		// Token: 0x040071D5 RID: 29141
		private Scene _scene;
	}
}
