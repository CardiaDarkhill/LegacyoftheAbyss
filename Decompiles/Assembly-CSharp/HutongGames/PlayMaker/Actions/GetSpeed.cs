using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA5 RID: 4005
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets the Speed of a Game Object and stores it in a Float Variable. NOTE: The Game Object must have a RigidBody component.")]
	public class GetSpeed : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EA7 RID: 28327 RVA: 0x00223F23 File Offset: 0x00222123
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006EA8 RID: 28328 RVA: 0x00223F3A File Offset: 0x0022213A
		public override void OnEnter()
		{
			this.DoGetSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EA9 RID: 28329 RVA: 0x00223F50 File Offset: 0x00222150
		public override void OnUpdate()
		{
			this.DoGetSpeed();
		}

		// Token: 0x06006EAA RID: 28330 RVA: 0x00223F58 File Offset: 0x00222158
		private void DoGetSpeed()
		{
			if (this.storeResult == null)
			{
				return;
			}
			GameObject go = (this.gameObject.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.gameObject.GameObject.Value;
			if (base.UpdateCache(go))
			{
				Vector3 linearVelocity = base.rigidbody.linearVelocity;
				this.storeResult.Value = linearVelocity.magnitude;
			}
		}

		// Token: 0x04006E46 RID: 28230
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The GameObject with a RigidBody component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E47 RID: 28231
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the speed in a float variable.")]
		public FsmFloat storeResult;

		// Token: 0x04006E48 RID: 28232
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
