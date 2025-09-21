using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9F RID: 3487
	[ActionCategory(ActionCategory.Transform)]
	public class TranslateRandom : FsmStateAction
	{
		// Token: 0x06006541 RID: 25921 RVA: 0x001FF345 File Offset: 0x001FD545
		public override void Reset()
		{
			this.gameObject = null;
			this.translateMin = null;
			this.translateMax = null;
			this.space = Space.Self;
		}

		// Token: 0x06006542 RID: 25922 RVA: 0x001FF363 File Offset: 0x001FD563
		public override void OnEnter()
		{
			this.DoTranslate();
			base.Finish();
		}

		// Token: 0x06006543 RID: 25923 RVA: 0x001FF374 File Offset: 0x001FD574
		private void DoTranslate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 translation = new Vector3(Random.Range(this.translateMin.Value.x, this.translateMax.Value.x), Random.Range(this.translateMin.Value.y, this.translateMax.Value.y), Random.Range(this.translateMin.Value.z, this.translateMax.Value.z));
			ownerDefaultTarget.transform.Translate(translation, this.space);
		}

		// Token: 0x0400643E RID: 25662
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400643F RID: 25663
		public FsmVector3 translateMin;

		// Token: 0x04006440 RID: 25664
		public FsmVector3 translateMax;

		// Token: 0x04006441 RID: 25665
		[Tooltip("Translate in local or world space.")]
		public Space space;
	}
}
