using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F32 RID: 3890
	[ActionCategory(ActionCategory.Level)]
	[Tooltip("Makes the Game Object not be destroyed automatically when loading a new scene.\nSee unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Object.DontDestroyOnLoad.html\">DontDestroyOnLoad</a>.")]
	public class DontDestroyOnLoad : FsmStateAction
	{
		// Token: 0x06006C6F RID: 27759 RVA: 0x0021DB9F File Offset: 0x0021BD9F
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006C70 RID: 27760 RVA: 0x0021DBA8 File Offset: 0x0021BDA8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				Object.DontDestroyOnLoad(ownerDefaultTarget.transform.root.gameObject);
			}
			base.Finish();
		}

		// Token: 0x04006C39 RID: 27705
		[RequiredField]
		[Tooltip("GameObject to mark as DontDestroyOnLoad.")]
		public FsmOwnerDefault gameObject;
	}
}
