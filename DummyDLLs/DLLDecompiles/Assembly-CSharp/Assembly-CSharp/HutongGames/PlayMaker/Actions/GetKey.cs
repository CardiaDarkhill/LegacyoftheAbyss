using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F06 RID: 3846
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets the pressed state of a Key.")]
	public class GetKey : FsmStateAction
	{
		// Token: 0x06006B92 RID: 27538 RVA: 0x0021790A File Offset: 0x00215B0A
		public override void Reset()
		{
			this.key = KeyCode.None;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B93 RID: 27539 RVA: 0x00217921 File Offset: 0x00215B21
		public override void OnEnter()
		{
			this.DoGetKey();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B94 RID: 27540 RVA: 0x00217937 File Offset: 0x00215B37
		public override void OnUpdate()
		{
			this.DoGetKey();
		}

		// Token: 0x06006B95 RID: 27541 RVA: 0x0021793F File Offset: 0x00215B3F
		private void DoGetKey()
		{
			this.storeResult.Value = Input.GetKey(this.key);
		}

		// Token: 0x04006AE6 RID: 27366
		[RequiredField]
		[Tooltip("The key to detect.")]
		public KeyCode key;

		// Token: 0x04006AE7 RID: 27367
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store if the key is down (True) or up (False).")]
		public FsmBool storeResult;

		// Token: 0x04006AE8 RID: 27368
		[Tooltip("Repeat every frame. Useful if you're waiting for a key press/release.")]
		public bool everyFrame;
	}
}
