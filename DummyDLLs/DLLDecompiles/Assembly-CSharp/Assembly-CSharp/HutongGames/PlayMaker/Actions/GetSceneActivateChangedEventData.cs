using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200103E RID: 4158
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("Get the last activateChanged Scene Event data when event was sent from the action 'SendSceneActiveChangedEvent")]
	public class GetSceneActivateChangedEventData : FsmStateAction
	{
		// Token: 0x060071F1 RID: 29169 RVA: 0x002305E0 File Offset: 0x0022E7E0
		public override void Reset()
		{
			this.newName = null;
			this.newPath = null;
			this.newIsValid = null;
			this.newBuildIndex = null;
			this.newIsLoaded = null;
			this.newRootCount = null;
			this.newRootGameObjects = null;
			this.newIsDirty = null;
			this.previousName = null;
			this.previousPath = null;
			this.previousIsValid = null;
			this.previousBuildIndex = null;
			this.previousIsLoaded = null;
			this.previousRootCount = null;
			this.previousRootGameObjects = null;
			this.previousIsDirty = null;
		}

		// Token: 0x060071F2 RID: 29170 RVA: 0x0023065D File Offset: 0x0022E85D
		public override void OnEnter()
		{
			this.DoGetSceneProperties();
			base.Finish();
		}

		// Token: 0x060071F3 RID: 29171 RVA: 0x0023066B File Offset: 0x0022E86B
		public override void OnUpdate()
		{
			this.DoGetSceneProperties();
		}

		// Token: 0x060071F4 RID: 29172 RVA: 0x00230674 File Offset: 0x0022E874
		private void DoGetSceneProperties()
		{
			this._scene = SendActiveSceneChangedEvent.lastPreviousActiveScene;
			if (!this.previousName.IsNone)
			{
				this.previousName.Value = this._scene.name;
			}
			if (!this.previousBuildIndex.IsNone)
			{
				this.previousBuildIndex.Value = this._scene.buildIndex;
			}
			if (!this.previousPath.IsNone)
			{
				this.previousPath.Value = this._scene.path;
			}
			if (!this.previousIsValid.IsNone)
			{
				this.previousIsValid.Value = this._scene.IsValid();
			}
			if (!this.previousIsDirty.IsNone)
			{
				this.previousIsDirty.Value = this._scene.isDirty;
			}
			if (!this.previousIsLoaded.IsNone)
			{
				this.previousIsLoaded.Value = this._scene.isLoaded;
			}
			if (!this.previousRootCount.IsNone)
			{
				this.previousRootCount.Value = this._scene.rootCount;
			}
			if (!this.previousRootGameObjects.IsNone)
			{
				if (this._scene.IsValid())
				{
					FsmArray fsmArray = this.previousRootGameObjects;
					object[] rootGameObjects = this._scene.GetRootGameObjects();
					fsmArray.Values = rootGameObjects;
				}
				else
				{
					this.previousRootGameObjects.Resize(0);
				}
			}
			this._scene = SendActiveSceneChangedEvent.lastNewActiveScene;
			if (!this.newName.IsNone)
			{
				this.newName.Value = this._scene.name;
			}
			if (!this.newBuildIndex.IsNone)
			{
				this.newBuildIndex.Value = this._scene.buildIndex;
			}
			if (!this.newPath.IsNone)
			{
				this.newPath.Value = this._scene.path;
			}
			if (!this.newIsValid.IsNone)
			{
				this.newIsValid.Value = this._scene.IsValid();
			}
			if (!this.newIsDirty.IsNone)
			{
				this.newIsDirty.Value = this._scene.isDirty;
			}
			if (!this.newIsLoaded.IsNone)
			{
				this.newIsLoaded.Value = this._scene.isLoaded;
			}
			if (!this.newRootCount.IsNone)
			{
				this.newRootCount.Value = this._scene.rootCount;
			}
			if (!this.newRootGameObjects.IsNone)
			{
				if (this._scene.IsValid())
				{
					FsmArray fsmArray2 = this.newRootGameObjects;
					object[] rootGameObjects = this._scene.GetRootGameObjects();
					fsmArray2.Values = rootGameObjects;
					return;
				}
				this.newRootGameObjects.Resize(0);
			}
		}

		// Token: 0x040071AC RID: 29100
		[ActionSection("New Active Scene")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The new active scene name")]
		public FsmString newName;

		// Token: 0x040071AD RID: 29101
		[Tooltip("The new active scene path")]
		[UIHint(UIHint.Variable)]
		public FsmString newPath;

		// Token: 0x040071AE RID: 29102
		[Tooltip("true if the new active scene is valid.")]
		[UIHint(UIHint.Variable)]
		public FsmBool newIsValid;

		// Token: 0x040071AF RID: 29103
		[Tooltip("The new active scene Build Index")]
		[UIHint(UIHint.Variable)]
		public FsmInt newBuildIndex;

		// Token: 0x040071B0 RID: 29104
		[Tooltip("true if the new active scene is loaded.")]
		[UIHint(UIHint.Variable)]
		public FsmBool newIsLoaded;

		// Token: 0x040071B1 RID: 29105
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the new active scene is modified.")]
		public FsmBool newIsDirty;

		// Token: 0x040071B2 RID: 29106
		[Tooltip("The new active scene RootCount")]
		[UIHint(UIHint.Variable)]
		public FsmInt newRootCount;

		// Token: 0x040071B3 RID: 29107
		[Tooltip("The new active scene Root GameObjects")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray newRootGameObjects;

		// Token: 0x040071B4 RID: 29108
		[ActionSection("Previous Active Scene")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The previous active scene name")]
		public FsmString previousName;

		// Token: 0x040071B5 RID: 29109
		[Tooltip("The previous active scene path")]
		[UIHint(UIHint.Variable)]
		public FsmString previousPath;

		// Token: 0x040071B6 RID: 29110
		[Tooltip("true if the previous active scene is valid.")]
		[UIHint(UIHint.Variable)]
		public FsmBool previousIsValid;

		// Token: 0x040071B7 RID: 29111
		[Tooltip("The previous active scene Build Index")]
		[UIHint(UIHint.Variable)]
		public FsmInt previousBuildIndex;

		// Token: 0x040071B8 RID: 29112
		[Tooltip("true if the previous active scene is loaded.")]
		[UIHint(UIHint.Variable)]
		public FsmBool previousIsLoaded;

		// Token: 0x040071B9 RID: 29113
		[UIHint(UIHint.Variable)]
		[Tooltip("true if the previous active scene is modified.")]
		public FsmBool previousIsDirty;

		// Token: 0x040071BA RID: 29114
		[Tooltip("The previous active scene RootCount")]
		[UIHint(UIHint.Variable)]
		public FsmInt previousRootCount;

		// Token: 0x040071BB RID: 29115
		[Tooltip("The previous active scene Root GameObjects")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray previousRootGameObjects;

		// Token: 0x040071BC RID: 29116
		private Scene _scene;
	}
}
