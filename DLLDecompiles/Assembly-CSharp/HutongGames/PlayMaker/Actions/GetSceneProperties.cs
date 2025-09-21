using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001048 RID: 4168
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get a scene isDirty flag. true if the scene is modified. ")]
	public class GetSceneProperties : GetSceneActionBase
	{
		// Token: 0x0600721D RID: 29213 RVA: 0x00230E3C File Offset: 0x0022F03C
		public override void Reset()
		{
			base.Reset();
			this.name = null;
			this.path = null;
			this.buildIndex = null;
			this.isValid = null;
			this.isLoaded = null;
			this.rootCount = null;
			this.rootGameObjects = null;
			this.isDirty = null;
			this.everyFrame = false;
		}

		// Token: 0x0600721E RID: 29214 RVA: 0x00230E8E File Offset: 0x0022F08E
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoGetSceneProperties();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600721F RID: 29215 RVA: 0x00230EAC File Offset: 0x0022F0AC
		private void DoGetSceneProperties()
		{
			if (!this._sceneFound)
			{
				return;
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
				}
				else
				{
					this.rootGameObjects.Resize(0);
				}
			}
			base.Fsm.Event(this.sceneFoundEvent);
		}

		// Token: 0x040071D8 RID: 29144
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The scene name")]
		public FsmString name;

		// Token: 0x040071D9 RID: 29145
		[Tooltip("The scene path")]
		[UIHint(UIHint.Variable)]
		public FsmString path;

		// Token: 0x040071DA RID: 29146
		[Tooltip("The scene Build Index")]
		[UIHint(UIHint.Variable)]
		public FsmInt buildIndex;

		// Token: 0x040071DB RID: 29147
		[Tooltip("true if the scene is valid.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isValid;

		// Token: 0x040071DC RID: 29148
		[Tooltip("true if the scene is loaded.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isLoaded;

		// Token: 0x040071DD RID: 29149
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the scene is modified.")]
		public FsmBool isDirty;

		// Token: 0x040071DE RID: 29150
		[Tooltip("The scene RootCount")]
		[UIHint(UIHint.Variable)]
		public FsmInt rootCount;

		// Token: 0x040071DF RID: 29151
		[Tooltip("The scene Root GameObjects")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray rootGameObjects;

		// Token: 0x040071E0 RID: 29152
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
