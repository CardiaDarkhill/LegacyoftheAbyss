using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BED RID: 3053
	[ActionCategory(ActionCategory.Transform)]
	public class CheckOutOfCamera : FsmStateAction
	{
		// Token: 0x06005D7B RID: 23931 RVA: 0x001D77F3 File Offset: 0x001D59F3
		public override void Reset()
		{
			this.gameObject = null;
			this.margin = 6f;
			this.outsideEvent = null;
			this.insideEvent = null;
			this.insideBool = null;
			this.outsideBool = null;
			this.everyFrame = false;
		}

		// Token: 0x06005D7C RID: 23932 RVA: 0x001D7830 File Offset: 0x001D5A30
		public override void OnEnter()
		{
			this.targetTransform = base.Fsm.GetOwnerDefaultTarget(this.gameObject).transform;
			this.camTransform = GameCameras.instance.mainCamera.gameObject.transform;
			this.DoCheck();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005D7D RID: 23933 RVA: 0x001D7887 File Offset: 0x001D5A87
		public override void OnUpdate()
		{
			this.DoCheck();
		}

		// Token: 0x06005D7E RID: 23934 RVA: 0x001D7890 File Offset: 0x001D5A90
		private void DoCheck()
		{
			float num = this.targetTransform.position.x - this.camTransform.transform.position.x;
			float num2 = this.targetTransform.position.y - this.camTransform.transform.position.y;
			if (num < 0f)
			{
				num *= -1f;
			}
			if (num2 < 0f)
			{
				num2 *= -1f;
			}
			if (num > 15f + this.margin.Value || num2 > 9f + this.margin.Value)
			{
				if (this.outsideEvent != null)
				{
					base.Fsm.Event(this.outsideEvent);
				}
				this.outsideBool.Value = true;
				this.insideBool.Value = false;
				return;
			}
			if (this.insideEvent != null)
			{
				base.Fsm.Event(this.insideEvent);
			}
			this.outsideBool.Value = false;
			this.insideBool.Value = true;
		}

		// Token: 0x040059A1 RID: 22945
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040059A2 RID: 22946
		public FsmFloat margin;

		// Token: 0x040059A3 RID: 22947
		public FsmEvent outsideEvent;

		// Token: 0x040059A4 RID: 22948
		public FsmEvent insideEvent;

		// Token: 0x040059A5 RID: 22949
		public FsmBool insideBool;

		// Token: 0x040059A6 RID: 22950
		public FsmBool outsideBool;

		// Token: 0x040059A7 RID: 22951
		public bool everyFrame;

		// Token: 0x040059A8 RID: 22952
		private Transform targetTransform;

		// Token: 0x040059A9 RID: 22953
		private Transform camTransform;
	}
}
