using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D66 RID: 3430
	[ActionCategory(ActionCategory.Physics2D)]
	public class SimpleTiltByVelocityX : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006444 RID: 25668 RVA: 0x001F96B2 File Offset: 0x001F78B2
		public override void Reset()
		{
			this.gameObject = null;
			this.tiltFactor = null;
			this.reverseIfXScaleNegative = null;
		}

		// Token: 0x06006445 RID: 25669 RVA: 0x001F96CC File Offset: 0x001F78CC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			this.go_transform = ownerDefaultTarget.transform;
			this.go_rb2d = ownerDefaultTarget.GetComponent<Rigidbody2D>();
			this.DoTilt();
		}

		// Token: 0x06006446 RID: 25670 RVA: 0x001F9713 File Offset: 0x001F7913
		public override void OnUpdate()
		{
			this.DoTilt();
		}

		// Token: 0x06006447 RID: 25671 RVA: 0x001F971C File Offset: 0x001F791C
		private void DoTilt()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			float num = this.go_rb2d.linearVelocity.x * this.tiltFactor.Value;
			if (this.reverseIfXScaleNegative.Value && this.go_transform.lossyScale.x < 0f)
			{
				num *= -1f;
			}
			this.go_transform.localEulerAngles = new Vector3(this.go_transform.localEulerAngles.x, this.go_transform.localEulerAngles.y, num);
		}

		// Token: 0x040062BE RID: 25278
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040062BF RID: 25279
		public FsmFloat tiltFactor;

		// Token: 0x040062C0 RID: 25280
		public FsmBool reverseIfXScaleNegative;

		// Token: 0x040062C1 RID: 25281
		private Transform go_transform;

		// Token: 0x040062C2 RID: 25282
		private Rigidbody2D go_rb2d;
	}
}
