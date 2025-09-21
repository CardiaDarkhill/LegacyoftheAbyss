using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FDE RID: 4062
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Sets the Mass of a Game Object's Rigid Body 2D.")]
	public class SetMass2d : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006FDB RID: 28635 RVA: 0x002293A3 File Offset: 0x002275A3
		public override void Reset()
		{
			this.gameObject = null;
			this.mass = 1f;
		}

		// Token: 0x06006FDC RID: 28636 RVA: 0x002293BC File Offset: 0x002275BC
		public override void OnEnter()
		{
			this.DoSetMass();
			base.Finish();
		}

		// Token: 0x06006FDD RID: 28637 RVA: 0x002293CC File Offset: 0x002275CC
		private void DoSetMass()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			base.rigidbody2d.mass = this.mass.Value;
		}

		// Token: 0x04006FD4 RID: 28628
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FD5 RID: 28629
		[RequiredField]
		[HasFloatSlider(0.1f, 10f)]
		[Tooltip("The Mass")]
		public FsmFloat mass;
	}
}
