using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF5 RID: 2805
	[ActionCategory("UI")]
	[Tooltip("Marks a UI GameObject for rebuild, either for graphic updates or layout updates.")]
	public sealed class MarkUIForRebuild : FsmStateAction
	{
		// Token: 0x060058F9 RID: 22777 RVA: 0x001C3923 File Offset: 0x001C1B23
		public override void Reset()
		{
			this.gameObject = null;
			this.markGraphicRebuild = false;
			this.markLayoutRebuild = true;
		}

		// Token: 0x060058FA RID: 22778 RVA: 0x001C3944 File Offset: 0x001C1B44
		public override void OnEnter()
		{
			this.DoMarkForRebuild();
			base.Finish();
		}

		// Token: 0x060058FB RID: 22779 RVA: 0x001C3954 File Offset: 0x001C1B54
		private void DoMarkForRebuild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			RectTransform component = ownerDefaultTarget.GetComponent<RectTransform>();
			if (component == null)
			{
				return;
			}
			if (this.markGraphicRebuild.Value)
			{
				Graphic component2 = ownerDefaultTarget.GetComponent<Graphic>();
				if (component2 != null)
				{
					CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(component2);
				}
				else
				{
					Debug.LogWarning("No Graphic component found on the specified GameObject for graphic rebuild.");
				}
			}
			if (this.markLayoutRebuild.Value)
			{
				LayoutRebuilder.MarkLayoutForRebuild(component);
			}
		}

		// Token: 0x0400541E RID: 21534
		[RequiredField]
		[Tooltip("The GameObject to mark for rebuild.")]
		[CheckForComponent(typeof(RectTransform))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400541F RID: 21535
		[Tooltip("Mark for graphic rebuild (e.g., visual updates like color or material changes).")]
		public FsmBool markGraphicRebuild;

		// Token: 0x04005420 RID: 21536
		[Tooltip("Mark for layout rebuild (e.g., recalculating position, size, and layout).")]
		public FsmBool markLayoutRebuild;
	}
}
