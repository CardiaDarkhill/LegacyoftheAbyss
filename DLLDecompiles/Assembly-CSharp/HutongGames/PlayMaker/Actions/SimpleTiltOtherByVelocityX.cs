using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D67 RID: 3431
	[ActionCategory(ActionCategory.Physics2D)]
	public class SimpleTiltOtherByVelocityX : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006449 RID: 25673 RVA: 0x001F97C7 File Offset: 0x001F79C7
		public override void Reset()
		{
			this.objectToTilt = null;
			this.getSpeedFrom = null;
			this.tiltFactor = null;
			this.reverseIfXScaleNegative = null;
		}

		// Token: 0x0600644A RID: 25674 RVA: 0x001F97E5 File Offset: 0x001F79E5
		public override void OnEnter()
		{
			this.go_transform = this.objectToTilt.Value.transform;
			this.go_rb2d = this.getSpeedFrom.Value.GetComponent<Rigidbody2D>();
			this.DoTilt();
		}

		// Token: 0x0600644B RID: 25675 RVA: 0x001F9819 File Offset: 0x001F7A19
		public override void OnUpdate()
		{
			this.DoTilt();
		}

		// Token: 0x0600644C RID: 25676 RVA: 0x001F9824 File Offset: 0x001F7A24
		private void DoTilt()
		{
			float num = this.go_rb2d.linearVelocity.x * this.tiltFactor.Value;
			if (this.reverseIfXScaleNegative.Value && this.go_transform.lossyScale.x < 0f)
			{
				num *= -1f;
			}
			this.go_transform.localEulerAngles = new Vector3(this.go_transform.localEulerAngles.x, this.go_transform.localEulerAngles.y, num);
		}

		// Token: 0x040062C3 RID: 25283
		[RequiredField]
		public FsmGameObject objectToTilt;

		// Token: 0x040062C4 RID: 25284
		[RequiredField]
		public FsmGameObject getSpeedFrom;

		// Token: 0x040062C5 RID: 25285
		public FsmFloat tiltFactor;

		// Token: 0x040062C6 RID: 25286
		public FsmBool reverseIfXScaleNegative;

		// Token: 0x040062C7 RID: 25287
		private Transform go_transform;

		// Token: 0x040062C8 RID: 25288
		private Rigidbody2D go_rb2d;
	}
}
