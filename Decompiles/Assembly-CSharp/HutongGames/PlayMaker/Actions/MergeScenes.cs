using System;
using UnityEngine.SceneManagement;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200104F RID: 4175
	[ActionCategory(ActionCategory.Scene)]
	[Tooltip("This will merge the source scene into the destinationScene. This function merges the contents of the source scene into the destination scene, and deletes the source scene. All GameObjects at the root of the source scene are moved to the root of the destination scene. NOTE: This function is destructive: The source scene will be destroyed once the merge has been completed.")]
	public class MergeScenes : FsmStateAction
	{
		// Token: 0x0600723D RID: 29245 RVA: 0x002318C8 File Offset: 0x0022FAC8
		public override void Reset()
		{
			this.sourceReference = GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex;
			this.sourceByPath = null;
			this.sourceByName = null;
			this.sourceAtIndex = null;
			this.sourceByGameObject = null;
			this.destinationReference = GetSceneActionBase.SceneAllReferenceOptions.ActiveScene;
			this.destinationByPath = null;
			this.destinationByName = null;
			this.destinationAtIndex = null;
			this.destinationByGameObject = null;
			this.success = null;
			this.successEvent = null;
			this.failureEvent = null;
		}

		// Token: 0x0600723E RID: 29246 RVA: 0x00231930 File Offset: 0x0022FB30
		public override void OnEnter()
		{
			this.GetSourceScene();
			this.GetDestinationScene();
			if (this._destinationFound && this._sourceFound)
			{
				if (this._destinationScene.Equals(this._sourceScene))
				{
					base.LogError("Source and Destination scenes can not be the same");
				}
				else
				{
					SceneManager.MergeScenes(this._sourceScene, this._destinationScene);
				}
				this.success.Value = true;
				base.Fsm.Event(this.successEvent);
			}
			else
			{
				this.success.Value = false;
				base.Fsm.Event(this.failureEvent);
			}
			base.Finish();
		}

		// Token: 0x0600723F RID: 29247 RVA: 0x002319D8 File Offset: 0x0022FBD8
		private void GetSourceScene()
		{
			try
			{
				switch (this.sourceReference)
				{
				case GetSceneActionBase.SceneAllReferenceOptions.ActiveScene:
					this._sourceScene = SceneManager.GetActiveScene();
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex:
					this._sourceScene = SceneManager.GetSceneAt(this.sourceAtIndex.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByName:
					this._sourceScene = SceneManager.GetSceneByName(this.sourceByName.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByPath:
					this._sourceScene = SceneManager.GetSceneByPath(this.sourceByPath.Value);
					break;
				}
			}
			catch (Exception ex)
			{
				base.LogError(ex.Message);
			}
			if (this._sourceScene == default(Scene))
			{
				this._sourceFound = false;
				return;
			}
			this._sourceFound = true;
		}

		// Token: 0x06007240 RID: 29248 RVA: 0x00231AA0 File Offset: 0x0022FCA0
		private void GetDestinationScene()
		{
			try
			{
				switch (this.sourceReference)
				{
				case GetSceneActionBase.SceneAllReferenceOptions.ActiveScene:
					this._destinationScene = SceneManager.GetActiveScene();
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneAtIndex:
					this._destinationScene = SceneManager.GetSceneAt(this.destinationAtIndex.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByName:
					this._destinationScene = SceneManager.GetSceneByName(this.destinationByName.Value);
					break;
				case GetSceneActionBase.SceneAllReferenceOptions.SceneByPath:
					this._destinationScene = SceneManager.GetSceneByPath(this.destinationByPath.Value);
					break;
				}
			}
			catch (Exception ex)
			{
				base.LogError(ex.Message);
			}
			if (this._destinationScene == default(Scene))
			{
				this._destinationFound = false;
				return;
			}
			this._destinationFound = true;
		}

		// Token: 0x06007241 RID: 29249 RVA: 0x00231B68 File Offset: 0x0022FD68
		public override string ErrorCheck()
		{
			if (this.sourceReference == GetSceneActionBase.SceneAllReferenceOptions.ActiveScene && this.destinationReference == GetSceneActionBase.SceneAllReferenceOptions.ActiveScene)
			{
				return "Source and Destination scenes can not be the same";
			}
			return string.Empty;
		}

		// Token: 0x04007212 RID: 29202
		[ActionSection("Source")]
		[Tooltip("The reference options of the source Scene")]
		public GetSceneActionBase.SceneAllReferenceOptions sourceReference;

		// Token: 0x04007213 RID: 29203
		[Tooltip("The source scene Index.")]
		public FsmInt sourceAtIndex;

		// Token: 0x04007214 RID: 29204
		[Tooltip("The source scene Name.")]
		public FsmString sourceByName;

		// Token: 0x04007215 RID: 29205
		[Tooltip("The source scene Path.")]
		public FsmString sourceByPath;

		// Token: 0x04007216 RID: 29206
		[Tooltip("The source scene from GameObject")]
		public FsmOwnerDefault sourceByGameObject;

		// Token: 0x04007217 RID: 29207
		[ActionSection("Destination")]
		[Tooltip("The reference options of the destination Scene")]
		public GetSceneActionBase.SceneAllReferenceOptions destinationReference;

		// Token: 0x04007218 RID: 29208
		[Tooltip("The destination scene Index.")]
		public FsmInt destinationAtIndex;

		// Token: 0x04007219 RID: 29209
		[Tooltip("The destination scene Name.")]
		public FsmString destinationByName;

		// Token: 0x0400721A RID: 29210
		[Tooltip("The destination scene Path.")]
		public FsmString destinationByPath;

		// Token: 0x0400721B RID: 29211
		[Tooltip("The destination scene from GameObject")]
		public FsmOwnerDefault destinationByGameObject;

		// Token: 0x0400721C RID: 29212
		[ActionSection("Result")]
		[Tooltip("True if the merge succeeded")]
		[UIHint(UIHint.Variable)]
		public FsmBool success;

		// Token: 0x0400721D RID: 29213
		[Tooltip("Event sent if merge succeeded")]
		public FsmEvent successEvent;

		// Token: 0x0400721E RID: 29214
		[Tooltip("Event sent if merge failed")]
		public FsmEvent failureEvent;

		// Token: 0x0400721F RID: 29215
		private Scene _sourceScene;

		// Token: 0x04007220 RID: 29216
		private bool _sourceFound;

		// Token: 0x04007221 RID: 29217
		private Scene _destinationScene;

		// Token: 0x04007222 RID: 29218
		private bool _destinationFound;
	}
}
