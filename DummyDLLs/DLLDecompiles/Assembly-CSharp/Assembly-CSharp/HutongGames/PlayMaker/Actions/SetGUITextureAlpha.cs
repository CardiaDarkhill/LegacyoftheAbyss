using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EDE RID: 3806
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Sets the Alpha of the GUITexture attached to a Game Object. Useful for fading GUI elements in/out.")]
	[Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
	public class SetGUITextureAlpha : FsmStateAction
	{
		// Token: 0x04006A5D RID: 27229
		[ActionSection("Obsolete. Use Unity UI instead.")]
		public FsmFloat alpha;
	}
}
