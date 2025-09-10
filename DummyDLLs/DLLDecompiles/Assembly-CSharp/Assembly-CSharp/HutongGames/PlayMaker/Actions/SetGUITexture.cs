using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EDD RID: 3805
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Sets the Texture used by the GUITexture attached to a Game Object.")]
	[Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
	public class SetGUITexture : FsmStateAction
	{
		// Token: 0x04006A5C RID: 27228
		[ActionSection("Obsolete. Use Unity UI instead.")]
		public FsmTexture texture;
	}
}
