using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FC2 RID: 4034
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets the Mass of a Game Object's Rigid Body 2D.")]
	public class GetMass2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F51 RID: 28497 RVA: 0x002265A6 File Offset: 0x002247A6
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x06006F52 RID: 28498 RVA: 0x002265B6 File Offset: 0x002247B6
		public override void OnEnter()
		{
			this.DoGetMass();
			base.Finish();
		}

		// Token: 0x06006F53 RID: 28499 RVA: 0x002265C4 File Offset: 0x002247C4
		private void DoGetMass()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			this.storeResult.Value = base.rigidbody2d.mass;
		}

		// Token: 0x04006EE7 RID: 28391
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with a Rigidbody2D attached.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006EE8 RID: 28392
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the mass of gameObject.")]
		public FsmFloat storeResult;
	}
}
