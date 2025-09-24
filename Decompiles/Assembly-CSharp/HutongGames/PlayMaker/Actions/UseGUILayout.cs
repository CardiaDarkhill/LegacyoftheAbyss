using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EFF RID: 3839
	[ActionCategory(ActionCategory.GUILayout)]
	[Tooltip("Turn GUILayout on/off. If you don't use GUILayout actions you can get some performance back by turning GUILayout off. This can make a difference on iOS platforms.")]
	public class UseGUILayout : FsmStateAction
	{
		// Token: 0x06006B79 RID: 27513 RVA: 0x00217480 File Offset: 0x00215680
		public override void Reset()
		{
			this.turnOffGUIlayout = true;
		}

		// Token: 0x06006B7A RID: 27514 RVA: 0x00217489 File Offset: 0x00215689
		public override void OnEnter()
		{
			base.Fsm.Owner.useGUILayout = !this.turnOffGUIlayout;
			base.Finish();
		}

		// Token: 0x04006ACE RID: 27342
		[RequiredField]
		[Tooltip("Enable/disable GUILayout.")]
		public bool turnOffGUIlayout;
	}
}
