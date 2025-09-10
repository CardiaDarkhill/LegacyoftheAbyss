using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001069 RID: 4201
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Position of a Game Object to another Game Object's position")]
	public class SetPositionToObject : FsmStateAction
	{
		// Token: 0x060072C0 RID: 29376 RVA: 0x00234DC3 File Offset: 0x00232FC3
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.xOffset = null;
			this.yOffset = null;
			this.zOffset = null;
			this.overrideZ = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x060072C1 RID: 29377 RVA: 0x00234DFA File Offset: 0x00232FFA
		public override void OnEnter()
		{
			this.DoSetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060072C2 RID: 29378 RVA: 0x00234E10 File Offset: 0x00233010
		public override void OnUpdate()
		{
			if (this.everyFrame)
			{
				this.DoSetPosition();
			}
		}

		// Token: 0x060072C3 RID: 29379 RVA: 0x00234E20 File Offset: 0x00233020
		private void DoSetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null || this.targetObject.IsNone || this.targetObject.Value == null)
			{
				return;
			}
			Vector3 position = this.targetObject.Value.transform.position;
			if (!this.xOffset.IsNone)
			{
				position = new Vector3(position.x + this.xOffset.Value, position.y, position.z);
			}
			if (!this.yOffset.IsNone)
			{
				position = new Vector3(position.x, position.y + this.yOffset.Value, position.z);
			}
			if (!this.zOffset.IsNone)
			{
				position = new Vector3(position.x, position.y, position.z + this.zOffset.Value);
			}
			if (!this.overrideZ.IsNone && this.overrideZ != null)
			{
				position = new Vector3(position.x, position.y, this.overrideZ.Value);
			}
			ownerDefaultTarget.transform.position = position;
		}

		// Token: 0x040072C4 RID: 29380
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072C5 RID: 29381
		public FsmGameObject targetObject;

		// Token: 0x040072C6 RID: 29382
		public FsmFloat xOffset;

		// Token: 0x040072C7 RID: 29383
		public FsmFloat yOffset;

		// Token: 0x040072C8 RID: 29384
		public FsmFloat zOffset;

		// Token: 0x040072C9 RID: 29385
		public FsmFloat overrideZ;

		// Token: 0x040072CA RID: 29386
		public bool everyFrame;
	}
}
