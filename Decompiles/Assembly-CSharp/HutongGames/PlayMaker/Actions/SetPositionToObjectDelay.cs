using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4A RID: 3402
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Position of a Game Object to another Game Object's position")]
	public class SetPositionToObjectDelay : FsmStateAction
	{
		// Token: 0x060063C4 RID: 25540 RVA: 0x001F73CE File Offset: 0x001F55CE
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

		// Token: 0x060063C5 RID: 25541 RVA: 0x001F7405 File Offset: 0x001F5605
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			this.DoSetPosition();
			base.Finish();
		}

		// Token: 0x060063C6 RID: 25542 RVA: 0x001F743C File Offset: 0x001F563C
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

		// Token: 0x04006217 RID: 25111
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006218 RID: 25112
		public FsmGameObject targetObject;

		// Token: 0x04006219 RID: 25113
		public FsmFloat xOffset;

		// Token: 0x0400621A RID: 25114
		public FsmFloat yOffset;

		// Token: 0x0400621B RID: 25115
		public FsmFloat zOffset;

		// Token: 0x0400621C RID: 25116
		public FsmFloat overrideZ;

		// Token: 0x0400621D RID: 25117
		public FsmFloat delay;

		// Token: 0x0400621E RID: 25118
		private float timer;
	}
}
