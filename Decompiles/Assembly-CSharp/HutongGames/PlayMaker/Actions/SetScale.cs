using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E7 RID: 4327
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetScale : FsmStateAction
	{
		// Token: 0x06007514 RID: 29972 RVA: 0x0023CC48 File Offset: 0x0023AE48
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
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x06007515 RID: 29973 RVA: 0x0023CCA7 File Offset: 0x0023AEA7
		public override void OnPreprocess()
		{
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x06007516 RID: 29974 RVA: 0x0023CCBD File Offset: 0x0023AEBD
		public override void OnEnter()
		{
			this.DoSetScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007517 RID: 29975 RVA: 0x0023CCD3 File Offset: 0x0023AED3
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetScale();
			}
		}

		// Token: 0x06007518 RID: 29976 RVA: 0x0023CCE3 File Offset: 0x0023AEE3
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetScale();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007519 RID: 29977 RVA: 0x0023CD04 File Offset: 0x0023AF04
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

		// Token: 0x0400756E RID: 30062
		[RequiredField]
		[Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400756F RID: 30063
		[UIHint(UIHint.Variable)]
		[Tooltip("Use stored Vector3 value, and/or set each axis below.")]
		public FsmVector3 vector;

		// Token: 0x04007570 RID: 30064
		[Tooltip("Scale along the X axis (1 = normal).")]
		public FsmFloat x;

		// Token: 0x04007571 RID: 30065
		[Tooltip("Scale along the Y axis (1 = normal).")]
		public FsmFloat y;

		// Token: 0x04007572 RID: 30066
		[Tooltip("Scale along the Z axis (1 = normal).")]
		public FsmFloat z;

		// Token: 0x04007573 RID: 30067
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007574 RID: 30068
		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
