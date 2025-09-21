using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB1 RID: 4017
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Connect a joint to a game object.")]
	public class SetJointConnectedBody : FsmStateAction
	{
		// Token: 0x06006EE3 RID: 28387 RVA: 0x00224DC2 File Offset: 0x00222FC2
		public override void Reset()
		{
			this.joint = null;
			this.rigidBody = null;
		}

		// Token: 0x06006EE4 RID: 28388 RVA: 0x00224DD4 File Offset: 0x00222FD4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.joint);
			if (ownerDefaultTarget != null)
			{
				Joint component = ownerDefaultTarget.GetComponent<Joint>();
				if (component != null)
				{
					component.connectedBody = ((this.rigidBody.Value == null) ? null : this.rigidBody.Value.GetComponent<Rigidbody>());
				}
			}
			base.Finish();
		}

		// Token: 0x04006E99 RID: 28313
		[RequiredField]
		[CheckForComponent(typeof(Joint))]
		[Tooltip("The joint to connect. Requires a Joint component.")]
		public FsmOwnerDefault joint;

		// Token: 0x04006E9A RID: 28314
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The game object to connect to the Joint. Set to none to connect the Joint to the world.")]
		public FsmGameObject rigidBody;
	}
}
