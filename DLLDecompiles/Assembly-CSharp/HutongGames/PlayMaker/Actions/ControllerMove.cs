using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E57 RID: 3671
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Moves a Game Object with a Character Controller. See also {{Controller Simple Move}}. NOTE: It is recommended that you make only one call to Move or SimpleMove per frame.")]
	public class ControllerMove : FsmStateAction
	{
		// Token: 0x060068E2 RID: 26850 RVA: 0x0020EFD2 File Offset: 0x0020D1D2
		public override void Reset()
		{
			this.gameObject = null;
			this.moveVector = new FsmVector3
			{
				UseVariable = true
			};
			this.space = Space.World;
			this.perSecond = true;
		}

		// Token: 0x060068E3 RID: 26851 RVA: 0x0020F000 File Offset: 0x0020D200
		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.previousGo)
			{
				this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
				this.previousGo = ownerDefaultTarget;
			}
			if (this.controller != null)
			{
				Vector3 vector = (this.space == Space.World) ? this.moveVector.Value : ownerDefaultTarget.transform.TransformDirection(this.moveVector.Value);
				if (this.perSecond.Value)
				{
					this.controller.Move(vector * Time.deltaTime);
					return;
				}
				this.controller.Move(vector);
			}
		}

		// Token: 0x04006826 RID: 26662
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The Game Object that owns the Character Controller component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006827 RID: 26663
		[RequiredField]
		[Tooltip("The movement vector.")]
		public FsmVector3 moveVector;

		// Token: 0x04006828 RID: 26664
		[Tooltip("Move in local or word space.")]
		public Space space;

		// Token: 0x04006829 RID: 26665
		[Tooltip("Apply the move over one second. Makes movement frame rate independent.")]
		public FsmBool perSecond;

		// Token: 0x0400682A RID: 26666
		private GameObject previousGo;

		// Token: 0x0400682B RID: 26667
		private CharacterController controller;
	}
}
