using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010DB RID: 4315
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Scale of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetScale : FsmStateAction
	{
		// Token: 0x060074C9 RID: 29897 RVA: 0x0023B98B File Offset: 0x00239B8B
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.xScale = null;
			this.yScale = null;
			this.zScale = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x060074CA RID: 29898 RVA: 0x0023B9BE File Offset: 0x00239BBE
		public override void OnEnter()
		{
			this.DoGetScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074CB RID: 29899 RVA: 0x0023B9D4 File Offset: 0x00239BD4
		public override void OnUpdate()
		{
			this.DoGetScale();
		}

		// Token: 0x060074CC RID: 29900 RVA: 0x0023B9DC File Offset: 0x00239BDC
		private void DoGetScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = (this.space == Space.World) ? ownerDefaultTarget.transform.lossyScale : ownerDefaultTarget.transform.localScale;
			this.vector.Value = vector;
			this.xScale.Value = vector.x;
			this.yScale.Value = vector.y;
			this.zScale.Value = vector.z;
		}

		// Token: 0x04007517 RID: 29975
		[RequiredField]
		[Tooltip("The Game Object.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007518 RID: 29976
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the scale in a Vector3 variable.")]
		public FsmVector3 vector;

		// Token: 0x04007519 RID: 29977
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X scale in a Float variable.")]
		public FsmFloat xScale;

		// Token: 0x0400751A RID: 29978
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y scale in a Float variable.")]
		public FsmFloat yScale;

		// Token: 0x0400751B RID: 29979
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Z scale in a Float variable.")]
		public FsmFloat zScale;

		// Token: 0x0400751C RID: 29980
		[Tooltip("The coordinate space to get the rotation in.")]
		public Space space;

		// Token: 0x0400751D RID: 29981
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
