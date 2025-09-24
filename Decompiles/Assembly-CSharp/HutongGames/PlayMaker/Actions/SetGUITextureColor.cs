using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EDF RID: 3807
	[ActionCategory(ActionCategory.GUIElement)]
	[Tooltip("Sets the Color of the GUITexture attached to a Game Object.")]
	[Obsolete("GUITexture is part of the legacy UI system removed in 2019.3")]
	public class SetGUITextureColor : FsmStateAction
	{
		// Token: 0x04006A5E RID: 27230
		[ActionSection("Obsolete. Use Unity UI instead.")]
		public FsmColor color;
	}
}
