using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E9A RID: 3738
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class FlipScale : FsmStateAction
	{
		// Token: 0x06006A13 RID: 27155 RVA: 0x00212B30 File Offset: 0x00210D30
		public override void Reset()
		{
			this.flipHorizontally = false;
			this.flipVertically = false;
			this.everyFrame = false;
		}

		// Token: 0x06006A14 RID: 27156 RVA: 0x00212B47 File Offset: 0x00210D47
		public override void OnEnter()
		{
			this.DoFlipScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A15 RID: 27157 RVA: 0x00212B5D File Offset: 0x00210D5D
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoFlipScale();
			}
		}

		// Token: 0x06006A16 RID: 27158 RVA: 0x00212B6D File Offset: 0x00210D6D
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoFlipScale();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A17 RID: 27159 RVA: 0x00212B8C File Offset: 0x00210D8C
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

		// Token: 0x0400696B RID: 26987
		[RequiredField]
		[Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400696C RID: 26988
		public bool flipHorizontally;

		// Token: 0x0400696D RID: 26989
		public bool flipVertically;

		// Token: 0x0400696E RID: 26990
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400696F RID: 26991
		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
