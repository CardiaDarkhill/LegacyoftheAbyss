using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C46 RID: 3142
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class FlipScaleDelay : FsmStateAction
	{
		// Token: 0x06005F55 RID: 24405 RVA: 0x001E4B15 File Offset: 0x001E2D15
		public override void Reset()
		{
			this.flipHorizontally = false;
			this.flipVertically = false;
			this.delay = null;
			this.checkBool = null;
		}

		// Token: 0x06005F56 RID: 24406 RVA: 0x001E4B33 File Offset: 0x001E2D33
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x06005F57 RID: 24407 RVA: 0x001E4B40 File Offset: 0x001E2D40
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			if (this.checkBool.Value || this.checkBool.IsNone)
			{
				if (this.checkBool.Value)
				{
					this.checkBool.Value = false;
				}
				this.DoFlipScale();
			}
			base.Finish();
		}

		// Token: 0x06005F58 RID: 24408 RVA: 0x001E4BB4 File Offset: 0x001E2DB4
		private void DoFlipScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 localScale = ownerDefaultTarget.transform.localScale;
			if (this.flipHorizontally)
			{
				localScale.x = -localScale.x;
			}
			if (this.flipVertically)
			{
				localScale.y = -localScale.y;
			}
			ownerDefaultTarget.transform.localScale = localScale;
		}

		// Token: 0x04005CB6 RID: 23734
		[RequiredField]
		[Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005CB7 RID: 23735
		public bool flipHorizontally;

		// Token: 0x04005CB8 RID: 23736
		public bool flipVertically;

		// Token: 0x04005CB9 RID: 23737
		public FsmFloat delay;

		// Token: 0x04005CBA RID: 23738
		public FsmBool checkBool;

		// Token: 0x04005CBB RID: 23739
		private float timer;
	}
}
