using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200106F RID: 4207
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Get the Sorting Layer name and/or Id of a of a SpriteRenderer component.")]
	public class GetSpriteSortingLayer : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072D9 RID: 29401 RVA: 0x0023527E File Offset: 0x0023347E
		public override void Reset()
		{
			this.gameObject = null;
			this.sortingLayerName = null;
			this.sortingLayerId = null;
		}

		// Token: 0x060072DA RID: 29402 RVA: 0x00235298 File Offset: 0x00233498
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (!this.sortingLayerName.IsNone)
			{
				this.sortingLayerName.Value = this.cachedComponent.sortingLayerName;
			}
			if (!this.sortingLayerId.IsNone)
			{
				this.sortingLayerId.Value = this.cachedComponent.sortingLayerID;
			}
			base.Finish();
		}

		// Token: 0x040072DC RID: 29404
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072DD RID: 29405
		[UIHint(UIHint.Variable)]
		[Tooltip("The sorting layer name")]
		public FsmString sortingLayerName;

		// Token: 0x040072DE RID: 29406
		[UIHint(UIHint.Variable)]
		[Tooltip("The sorting layer id")]
		public FsmInt sortingLayerId;
	}
}
