using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB7 RID: 4023
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Sets whether a Game Object's Rigidbody is affected by Gravity.")]
	public class UseGravity : ComponentAction<Rigidbody>
	{
		// Token: 0x06006F0B RID: 28427 RVA: 0x00225600 File Offset: 0x00223800
		public override void Reset()
		{
			this.gameObject = null;
			this.useGravity = true;
		}

		// Token: 0x06006F0C RID: 28428 RVA: 0x00225615 File Offset: 0x00223815
		public override void OnEnter()
		{
			this.DoUseGravity();
			base.Finish();
		}

		// Token: 0x06006F0D RID: 28429 RVA: 0x00225624 File Offset: 0x00223824
		private void DoUseGravity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.useGravity = this.useGravity.Value;
			}
		}

		// Token: 0x04006EB3 RID: 28339
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("A Game Object with a RigidBody component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006EB4 RID: 28340
		[RequiredField]
		[Tooltip("Enable/disable gravity for the Game Object.")]
		public FsmBool useGravity;
	}
}
