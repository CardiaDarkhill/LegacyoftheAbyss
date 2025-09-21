using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB9 RID: 4025
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Forces a Game Object's Rigid Body to wake up. See Unity Docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>.")]
	[SeeAlso("<a href =\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>")]
	public class WakeUp : ComponentAction<Rigidbody>
	{
		// Token: 0x06006F14 RID: 28436 RVA: 0x00225700 File Offset: 0x00223900
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006F15 RID: 28437 RVA: 0x00225709 File Offset: 0x00223909
		public override void OnEnter()
		{
			this.DoWakeUp();
			base.Finish();
		}

		// Token: 0x06006F16 RID: 28438 RVA: 0x00225718 File Offset: 0x00223918
		private void DoWakeUp()
		{
			GameObject go = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (base.UpdateCache(go))
			{
				base.rigidbody.WakeUp();
			}
		}

		// Token: 0x04006EB7 RID: 28343
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The Game Object to wake up.")]
		public FsmOwnerDefault gameObject;
	}
}
