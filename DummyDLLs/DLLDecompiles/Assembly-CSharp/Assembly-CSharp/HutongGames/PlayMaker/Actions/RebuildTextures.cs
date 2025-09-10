using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FF9 RID: 4089
	[ActionCategory("Substance")]
	[Tooltip("Rebuilds all dirty textures. By default the rebuild is spread over multiple frames so it won't halt the game. Check Immediately to rebuild all textures in a single frame.")]
	[Obsolete("Built-in support for Substance Designer materials has been deprecated and will be removed in Unity 2018.1. To continue using Substance Designer materials in Unity 2018.1, you will need to install a suitable third-party external importer from the Asset Store.")]
	public class RebuildTextures : FsmStateAction
	{
		// Token: 0x06007089 RID: 28809 RVA: 0x0022BFE7 File Offset: 0x0022A1E7
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.immediately = false;
			this.everyFrame = false;
		}

		// Token: 0x0600708A RID: 28810 RVA: 0x0022C003 File Offset: 0x0022A203
		public override void OnEnter()
		{
			this.DoRebuildTextures();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600708B RID: 28811 RVA: 0x0022C019 File Offset: 0x0022A219
		public override void OnUpdate()
		{
			this.DoRebuildTextures();
		}

		// Token: 0x0600708C RID: 28812 RVA: 0x0022C021 File Offset: 0x0022A221
		private void DoRebuildTextures()
		{
		}

		// Token: 0x04007059 RID: 28761
		[RequiredField]
		[Tooltip("The Substance material.")]
		public FsmMaterial substanceMaterial;

		// Token: 0x0400705A RID: 28762
		[RequiredField]
		[Tooltip("Rebuild now!")]
		public FsmBool immediately;

		// Token: 0x0400705B RID: 28763
		[Tooltip("Repeat every frame. Useful if you're animating Substance parameters.")]
		public bool everyFrame;
	}
}
