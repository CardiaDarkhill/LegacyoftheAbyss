using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FAE RID: 4014
	[ActionCategory(ActionCategory.Physics)]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=4734.0")]
	[Tooltip("Sets the Drag of a Game Object's Rigid Body.")]
	public class SetDrag : ComponentAction<Rigidbody>
	{
		// Token: 0x06006ED5 RID: 28373 RVA: 0x00224BE4 File Offset: 0x00222DE4
		public override void Reset()
		{
			this.gameObject = null;
			this.drag = 1f;
		}

		// Token: 0x06006ED6 RID: 28374 RVA: 0x00224BFD File Offset: 0x00222DFD
		public override void OnEnter()
		{
			this.DoSetDrag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006ED7 RID: 28375 RVA: 0x00224C13 File Offset: 0x00222E13
		public override void OnUpdate()
		{
			this.DoSetDrag();
		}

		// Token: 0x06006ED8 RID: 28376 RVA: 0x00224C1C File Offset: 0x00222E1C
		private void DoSetDrag()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.linearDamping = this.drag.Value;
			}
		}

		// Token: 0x04006E8F RID: 28303
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The GameObject that owns the RigidBody.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E90 RID: 28304
		[RequiredField]
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Set the Drag.")]
		public FsmFloat drag;

		// Token: 0x04006E91 RID: 28305
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
