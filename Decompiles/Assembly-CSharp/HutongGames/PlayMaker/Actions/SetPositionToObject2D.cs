using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D47 RID: 3399
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Position of a Game Object to another Game Object's position")]
	public class SetPositionToObject2D : FsmStateAction
	{
		// Token: 0x060063B4 RID: 25524 RVA: 0x001F6FFB File Offset: 0x001F51FB
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.xOffset = null;
			this.yOffset = null;
		}

		// Token: 0x060063B5 RID: 25525 RVA: 0x001F7019 File Offset: 0x001F5219
		public override void OnEnter()
		{
			this.DoSetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063B6 RID: 25526 RVA: 0x001F702F File Offset: 0x001F522F
		public override void OnUpdate()
		{
			if (this.everyFrame)
			{
				this.DoSetPosition();
			}
		}

		// Token: 0x060063B7 RID: 25527 RVA: 0x001F7040 File Offset: 0x001F5240
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
			position = new Vector3(position.x, position.y, ownerDefaultTarget.transform.position.z);
			ownerDefaultTarget.transform.position = position;
		}

		// Token: 0x04006208 RID: 25096
		[RequiredField]
		[Tooltip("The GameObject to position.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006209 RID: 25097
		public FsmGameObject targetObject;

		// Token: 0x0400620A RID: 25098
		public FsmFloat xOffset;

		// Token: 0x0400620B RID: 25099
		public FsmFloat yOffset;

		// Token: 0x0400620C RID: 25100
		public bool everyFrame;
	}
}
