using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001076 RID: 4214
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Set the Sorting Layer of a SpriteRenderer component by Id (by id is faster than by name)")]
	public class SetSpriteSortingLayerById : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072F4 RID: 29428 RVA: 0x0023578F File Offset: 0x0023398F
		public override void Reset()
		{
			this.gameObject = null;
			this.sortingLayerId = null;
			this.setAllSpritesInChildren = false;
		}

		// Token: 0x060072F5 RID: 29429 RVA: 0x002357AC File Offset: 0x002339AC
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (this.setAllSpritesInChildren.Value)
			{
				SpriteRenderer[] componentsInChildren = this.cachedComponent.GetComponentsInChildren<SpriteRenderer>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].sortingLayerID = this.sortingLayerId.Value;
				}
			}
			else
			{
				this.cachedComponent.sortingLayerID = this.sortingLayerId.Value;
			}
			base.Finish();
		}

		// Token: 0x040072F8 RID: 29432
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072F9 RID: 29433
		[RequiredField]
		[Tooltip("The sorting Layer Name")]
		public FsmInt sortingLayerId;

		// Token: 0x040072FA RID: 29434
		[Tooltip("If true, set the sorting layer to all children")]
		public FsmBool setAllSpritesInChildren;
	}
}
