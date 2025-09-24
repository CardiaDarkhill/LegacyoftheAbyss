using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC4 RID: 3012
	[ActionCategory("Game Manager")]
	[Tooltip("Perform a generic scene transition.")]
	public class BeginSceneTransition : FsmStateAction
	{
		// Token: 0x06005C9C RID: 23708 RVA: 0x001D274C File Offset: 0x001D094C
		public override void Reset()
		{
			this.sceneName = "";
			this.entryGateName = "left1";
			this.entryDelay = 0f;
			this.visualization = new FsmEnum
			{
				Value = GameManager.SceneLoadVisualizations.Default
			};
			this.preventCameraFadeOut = false;
		}

		// Token: 0x06005C9D RID: 23709 RVA: 0x001D27A8 File Offset: 0x001D09A8
		public override void OnEnter()
		{
			GameManager unsafeInstance = GameManager.UnsafeInstance;
			if (unsafeInstance == null)
			{
				base.LogError("Cannot BeginSceneTransition() before the game manager is loaded.");
			}
			else
			{
				unsafeInstance.BeginSceneTransition(new GameManager.SceneLoadInfo
				{
					SceneName = this.sceneName.Value,
					EntryGateName = this.entryGateName.Value,
					EntryDelay = this.entryDelay.Value,
					Visualization = (GameManager.SceneLoadVisualizations)this.visualization.Value,
					PreventCameraFadeOut = true,
					WaitForSceneTransitionCameraFade = !this.preventCameraFadeOut
				});
			}
			base.Finish();
		}

		// Token: 0x04005830 RID: 22576
		public FsmString sceneName;

		// Token: 0x04005831 RID: 22577
		public FsmString entryGateName;

		// Token: 0x04005832 RID: 22578
		public FsmFloat entryDelay;

		// Token: 0x04005833 RID: 22579
		[ObjectType(typeof(GameManager.SceneLoadVisualizations))]
		public FsmEnum visualization;

		// Token: 0x04005834 RID: 22580
		public bool preventCameraFadeOut;
	}
}
