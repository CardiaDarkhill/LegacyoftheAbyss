using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E5D RID: 3677
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Gets a CharacterController's velocity.")]
	public class GetControllerVelocity : ComponentAction<CharacterController>
	{
		// Token: 0x17000BEE RID: 3054
		// (get) Token: 0x060068FD RID: 26877 RVA: 0x0020F75E File Offset: 0x0020D95E
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068FE RID: 26878 RVA: 0x0020F766 File Offset: 0x0020D966
		public override void Reset()
		{
			this.gameObject = null;
			this.storeVelocity = null;
			this.storeX = null;
			this.storeY = null;
			this.storeZ = null;
		}

		// Token: 0x060068FF RID: 26879 RVA: 0x0020F78B File Offset: 0x0020D98B
		public override void OnEnter()
		{
			this.DoGetControllerVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006900 RID: 26880 RVA: 0x0020F7A1 File Offset: 0x0020D9A1
		public override void OnUpdate()
		{
			this.DoGetControllerVelocity();
		}

		// Token: 0x06006901 RID: 26881 RVA: 0x0020F7AC File Offset: 0x0020D9AC
		private void DoGetControllerVelocity()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 velocity = this.controller.velocity;
			this.storeVelocity.Value = velocity;
			this.storeX.Value = this.controller.velocity.x;
			this.storeY.Value = this.controller.velocity.y;
			this.storeZ.Value = this.controller.velocity.z;
		}

		// Token: 0x0400684C RID: 26700
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject with a CharacterController.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400684D RID: 26701
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the velocity in Vector3 variable.")]
		public FsmVector3 storeVelocity;

		// Token: 0x0400684E RID: 26702
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the x component of the velocity in a Float variable.")]
		public FsmFloat storeX;

		// Token: 0x0400684F RID: 26703
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the y component of the velocity in a Float variable.")]
		public FsmFloat storeY;

		// Token: 0x04006850 RID: 26704
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the z component of the velocity in a Float variable.")]
		public FsmFloat storeZ;

		// Token: 0x04006851 RID: 26705
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
