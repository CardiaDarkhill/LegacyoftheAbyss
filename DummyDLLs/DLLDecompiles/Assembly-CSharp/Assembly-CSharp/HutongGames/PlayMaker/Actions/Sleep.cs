using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB4 RID: 4020
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Forces a rigid body to sleep at least one frame. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.Sleep.html\">Rigidbody.sleep</a>.")]
	public class Sleep : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EF1 RID: 28401 RVA: 0x0022505D File Offset: 0x0022325D
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006EF2 RID: 28402 RVA: 0x00225066 File Offset: 0x00223266
		public override void OnEnter()
		{
			this.DoSleep();
			base.Finish();
		}

		// Token: 0x06006EF3 RID: 28403 RVA: 0x00225074 File Offset: 0x00223274
		private void DoSleep()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.Sleep();
			}
		}

		// Token: 0x04006EA4 RID: 28324
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("A Game Object with a Rigid Body.")]
		public FsmOwnerDefault gameObject;
	}
}
