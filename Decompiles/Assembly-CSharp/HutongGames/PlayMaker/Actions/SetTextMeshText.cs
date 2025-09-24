using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5A RID: 3418
	[ActionCategory("TextMesh")]
	[Tooltip("Set TextMesh text.")]
	public class SetTextMeshText : FsmStateAction
	{
		// Token: 0x06006405 RID: 25605 RVA: 0x001F84B9 File Offset: 0x001F66B9
		public override void Reset()
		{
			this.gameObject = null;
			this.textString = null;
		}

		// Token: 0x06006406 RID: 25606 RVA: 0x001F84CC File Offset: 0x001F66CC
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				this.textMesh = ownerDefaultTarget.GetComponent<TextMesh>();
				if (this.textMesh != null)
				{
					this.textMesh.text = this.textString.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04006272 RID: 25202
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006273 RID: 25203
		public FsmString textString;

		// Token: 0x04006274 RID: 25204
		private TextMesh textMesh;
	}
}
