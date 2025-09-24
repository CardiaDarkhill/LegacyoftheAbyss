using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001017 RID: 4119
	[ActionCategory("RectTransform")]
	[Tooltip("Flips the horizontal and vertical axes of the RectTransform size and alignment, and optionally its children as well.")]
	public class RectTransformFlipLayoutAxis : FsmStateAction
	{
		// Token: 0x06007131 RID: 28977 RVA: 0x0022D763 File Offset: 0x0022B963
		public override void Reset()
		{
			this.gameObject = null;
			this.axis = RectTransformFlipLayoutAxis.RectTransformFlipOptions.Both;
			this.keepPositioning = null;
			this.recursive = null;
		}

		// Token: 0x06007132 RID: 28978 RVA: 0x0022D781 File Offset: 0x0022B981
		public override void OnEnter()
		{
			this.DoFlip();
			base.Finish();
		}

		// Token: 0x06007133 RID: 28979 RVA: 0x0022D790 File Offset: 0x0022B990
		private void DoFlip()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				RectTransform component = ownerDefaultTarget.GetComponent<RectTransform>();
				if (component != null)
				{
					if (this.axis == RectTransformFlipLayoutAxis.RectTransformFlipOptions.Both)
					{
						RectTransformUtility.FlipLayoutAxes(component, this.keepPositioning.Value, this.recursive.Value);
						return;
					}
					if (this.axis == RectTransformFlipLayoutAxis.RectTransformFlipOptions.Horizontal)
					{
						RectTransformUtility.FlipLayoutOnAxis(component, 0, this.keepPositioning.Value, this.recursive.Value);
						return;
					}
					if (this.axis == RectTransformFlipLayoutAxis.RectTransformFlipOptions.Vertical)
					{
						RectTransformUtility.FlipLayoutOnAxis(component, 1, this.keepPositioning.Value, this.recursive.Value);
					}
				}
			}
		}

		// Token: 0x040070D1 RID: 28881
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070D2 RID: 28882
		[Tooltip("The axis to flip")]
		public RectTransformFlipLayoutAxis.RectTransformFlipOptions axis;

		// Token: 0x040070D3 RID: 28883
		[Tooltip("Flips around the pivot if true. Flips within the parent rect if false.")]
		public FsmBool keepPositioning;

		// Token: 0x040070D4 RID: 28884
		[Tooltip("Flip the children as well?")]
		public FsmBool recursive;

		// Token: 0x02001BB9 RID: 7097
		public enum RectTransformFlipOptions
		{
			// Token: 0x04009E65 RID: 40549
			Horizontal,
			// Token: 0x04009E66 RID: 40550
			Vertical,
			// Token: 0x04009E67 RID: 40551
			Both
		}
	}
}
