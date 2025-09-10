using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC5 RID: 3013
	[ActionCategory("Game Manager")]
	[Tooltip("Perform a generic scene transition.")]
	public class BeginSceneTransitionV2 : FsmStateAction
	{
		// Token: 0x06005C9F RID: 23711 RVA: 0x001D284C File Offset: 0x001D0A4C
		public override void Reset()
		{
			this.SceneName = "";
			this.EntryGateName = "left1";
			this.EntryDelay = 0f;
			this.Visualization = new FsmEnum
			{
				Value = GameManager.SceneLoadVisualizations.Default
			};
			this.PreventCameraFadeOut = false;
			this.TryClearMemory = false;
		}

		// Token: 0x06005CA0 RID: 23712 RVA: 0x001D28B8 File Offset: 0x001D0AB8
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
					SceneName = this.SceneName.Value,
					EntryGateName = this.EntryGateName.Value,
					EntryDelay = this.EntryDelay.Value,
					Visualization = (GameManager.SceneLoadVisualizations)this.Visualization.Value,
					PreventCameraFadeOut = true,
					WaitForSceneTransitionCameraFade = !this.PreventCameraFadeOut.Value,
					AlwaysUnloadUnusedAssets = this.TryClearMemory.Value
				});
			}
			base.Finish();
		}

		// Token: 0x04005835 RID: 22581
		public FsmString SceneName;

		// Token: 0x04005836 RID: 22582
		public FsmString EntryGateName;

		// Token: 0x04005837 RID: 22583
		public FsmFloat EntryDelay;

		// Token: 0x04005838 RID: 22584
		[ObjectType(typeof(GameManager.SceneLoadVisualizations))]
		public FsmEnum Visualization;

		// Token: 0x04005839 RID: 22585
		public FsmBool PreventCameraFadeOut;

		// Token: 0x0400583A RID: 22586
		public FsmBool TryClearMemory;
	}
}
