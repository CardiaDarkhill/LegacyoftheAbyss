using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001077 RID: 4215
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Set the Sorting Layer of a SpriteRenderer component. by name")]
	public class SetSpriteSortingLayerByName : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072F7 RID: 29431 RVA: 0x00235833 File Offset: 0x00233A33
		public override void Reset()
		{
			this.gameObject = null;
			this.sortingLayerName = null;
			this.setAllSpritesInChildren = false;
		}

		// Token: 0x060072F8 RID: 29432 RVA: 0x00235850 File Offset: 0x00233A50
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
					componentsInChildren[i].sortingLayerName = this.sortingLayerName.Value;
				}
			}
			else
			{
				this.cachedComponent.sortingLayerName = this.sortingLayerName.Value;
			}
			base.Finish();
		}

		// Token: 0x040072FB RID: 29435
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072FC RID: 29436
		[RequiredField]
		[Tooltip("The sorting Layer Name")]
		public FsmString sortingLayerName;

		// Token: 0x040072FD RID: 29437
		[Tooltip("If true, set the sorting layer to all children")]
		public FsmBool setAllSpritesInChildren;
	}
}
