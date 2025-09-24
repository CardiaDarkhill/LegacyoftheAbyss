using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFE RID: 3070
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Limits the distance the object can move in X/Y per update. Used to stop Climbers/Laser bugs etc from going into space when framerate dips")]
	public class ConstrainMovement : FsmStateAction
	{
		// Token: 0x06005DDA RID: 24026 RVA: 0x001D98B2 File Offset: 0x001D7AB2
		public override void Reset()
		{
			this.gameObject = null;
			this.xConstrain = new FsmFloat();
			this.yConstrain = new FsmFloat();
			this.xPrev = 0f;
			this.yPrev = 0f;
		}

		// Token: 0x06005DDB RID: 24027 RVA: 0x001D98E8 File Offset: 0x001D7AE8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.xPrev = ownerDefaultTarget.transform.position.x;
			this.yPrev = ownerDefaultTarget.transform.position.y;
		}

		// Token: 0x06005DDC RID: 24028 RVA: 0x001D9934 File Offset: 0x001D7B34
		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			float num = ownerDefaultTarget.transform.position.x;
			float num2 = ownerDefaultTarget.transform.position.y;
			if (num > this.xPrev + this.xConstrain.Value)
			{
				num = this.xPrev + this.xConstrain.Value;
			}
			else if (num < this.xPrev - this.xConstrain.Value)
			{
				num = this.xPrev - this.xConstrain.Value;
			}
			if (num2 > this.yPrev + this.yConstrain.Value)
			{
				num2 = this.yPrev + this.yConstrain.Value;
			}
			else if (num2 < this.yPrev - this.yConstrain.Value)
			{
				num2 = this.yPrev - this.yConstrain.Value;
			}
			ownerDefaultTarget.transform.position = new Vector3(num, num2, ownerDefaultTarget.transform.position.z);
			this.xPrev = ownerDefaultTarget.transform.position.x;
			this.yPrev = ownerDefaultTarget.transform.position.y;
		}

		// Token: 0x06005DDD RID: 24029 RVA: 0x001D9A67 File Offset: 0x001D7C67
		public override void OnLateUpdate()
		{
		}

		// Token: 0x04005A34 RID: 23092
		[RequiredField]
		[Tooltip("The GameObject to constrain.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005A35 RID: 23093
		[Tooltip("Max difference in x pos allowed per update")]
		public FsmFloat xConstrain;

		// Token: 0x04005A36 RID: 23094
		[Tooltip("Max difference in y pos allowed per update")]
		public FsmFloat yConstrain;

		// Token: 0x04005A37 RID: 23095
		private float xPrev;

		// Token: 0x04005A38 RID: 23096
		private float yPrev;
	}
}
