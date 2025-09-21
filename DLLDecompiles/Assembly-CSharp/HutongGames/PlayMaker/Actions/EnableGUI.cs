using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EC8 RID: 3784
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("Enables/Disables the PlayMakerGUI component in the scene. Note, you need a PlayMakerGUI component in the scene to see OnGUI actions. However, OnGUI can be very expensive on mobile devices. This action lets you turn OnGUI on/off (e.g., turn it on for a menu, and off during gameplay).")]
	public class EnableGUI : FsmStateAction
	{
		// Token: 0x06006AD5 RID: 27349 RVA: 0x00215165 File Offset: 0x00213365
		public override void Reset()
		{
			this.enableGUI = true;
		}

		// Token: 0x06006AD6 RID: 27350 RVA: 0x00215173 File Offset: 0x00213373
		public override void OnEnter()
		{
			PlayMakerGUI.Instance.enabled = this.enableGUI.Value;
			base.Finish();
		}

		// Token: 0x04006A1B RID: 27163
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enableGUI;
	}
}
