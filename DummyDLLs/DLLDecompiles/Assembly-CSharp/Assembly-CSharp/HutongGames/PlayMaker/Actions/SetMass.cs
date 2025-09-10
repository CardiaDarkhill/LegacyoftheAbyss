using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB2 RID: 4018
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Sets the mass of a game object's rigid body. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody-mass.html\">Rigidbody.Mass</a>")]
	public class SetMass : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EE6 RID: 28390 RVA: 0x00224E46 File Offset: 0x00223046
		public override void Reset()
		{
			this.gameObject = null;
			this.mass = 1f;
		}

		// Token: 0x06006EE7 RID: 28391 RVA: 0x00224E5F File Offset: 0x0022305F
		public override void OnEnter()
		{
			this.DoSetMass();
			base.Finish();
		}

		// Token: 0x06006EE8 RID: 28392 RVA: 0x00224E70 File Offset: 0x00223070
		private void DoSetMass()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.mass = this.mass.Value;
			}
		}

		// Token: 0x04006E9B RID: 28315
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("A GameObject with a RigidBody component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E9C RID: 28316
		[RequiredField]
		[HasFloatSlider(0.1f, 10f)]
		[Tooltip("Set the mass. Unity recommends a mass between 0.1 and 10.")]
		public FsmFloat mass;
	}
}
