using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D51 RID: 3409
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetScaleDelay : FsmStateAction
	{
		// Token: 0x060063E2 RID: 25570 RVA: 0x001F7C80 File Offset: 0x001F5E80
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.delay = null;
			this.checkBool = null;
		}

		// Token: 0x060063E3 RID: 25571 RVA: 0x001F7CDF File Offset: 0x001F5EDF
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x060063E4 RID: 25572 RVA: 0x001F7CEC File Offset: 0x001F5EEC
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			if (this.checkBool.Value || this.checkBool.IsNone)
			{
				this.DoSetScale();
			}
			base.Finish();
		}

		// Token: 0x060063E5 RID: 25573 RVA: 0x001F7D48 File Offset: 0x001F5F48
		private void DoSetScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 localScale = this.vector.IsNone ? ownerDefaultTarget.transform.localScale : this.vector.Value;
			if (!this.x.IsNone)
			{
				localScale.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				localScale.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				localScale.z = this.z.Value;
			}
			ownerDefaultTarget.transform.localScale = localScale;
		}

		// Token: 0x04006243 RID: 25155
		[RequiredField]
		[Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006244 RID: 25156
		[UIHint(UIHint.Variable)]
		[Tooltip("Use stored Vector3 value, and/or set each axis below.")]
		public FsmVector3 vector;

		// Token: 0x04006245 RID: 25157
		public FsmFloat x;

		// Token: 0x04006246 RID: 25158
		public FsmFloat y;

		// Token: 0x04006247 RID: 25159
		public FsmFloat z;

		// Token: 0x04006248 RID: 25160
		public FsmFloat delay;

		// Token: 0x04006249 RID: 25161
		public FsmBool checkBool;

		// Token: 0x0400624A RID: 25162
		private float timer;
	}
}
