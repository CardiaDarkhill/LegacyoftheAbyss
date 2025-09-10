using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EBE RID: 3774
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets a Game Object's Layer.")]
	public class SetLayer : FsmStateAction
	{
		// Token: 0x06006AAF RID: 27311 RVA: 0x00214916 File Offset: 0x00212B16
		public override void Reset()
		{
			this.gameObject = null;
			this.layer = 0;
		}

		// Token: 0x06006AB0 RID: 27312 RVA: 0x00214926 File Offset: 0x00212B26
		public override void OnEnter()
		{
			this.DoSetLayer();
			base.Finish();
		}

		// Token: 0x06006AB1 RID: 27313 RVA: 0x00214934 File Offset: 0x00212B34
		private void DoSetLayer()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			ownerDefaultTarget.layer = this.layer;
		}

		// Token: 0x040069F6 RID: 27126
		[RequiredField]
		[Tooltip("The Game Object to set.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040069F7 RID: 27127
		[Tooltip("The new layer.")]
		[UIHint(UIHint.Layer)]
		public int layer;
	}
}
