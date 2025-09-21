using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200112D RID: 4397
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets various properties of a UI Layout Element component.")]
	public class UiLayoutElementSetValues : ComponentAction<LayoutElement>
	{
		// Token: 0x06007689 RID: 30345 RVA: 0x002423BC File Offset: 0x002405BC
		public override void Reset()
		{
			this.gameObject = null;
			this.minWidth = new FsmFloat
			{
				UseVariable = true
			};
			this.minHeight = new FsmFloat
			{
				UseVariable = true
			};
			this.preferredWidth = new FsmFloat
			{
				UseVariable = true
			};
			this.preferredHeight = new FsmFloat
			{
				UseVariable = true
			};
			this.flexibleWidth = new FsmFloat
			{
				UseVariable = true
			};
			this.flexibleHeight = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x0600768A RID: 30346 RVA: 0x0024243C File Offset: 0x0024063C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.layoutElement = this.cachedComponent;
			}
			this.DoSetValues();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600768B RID: 30347 RVA: 0x00242484 File Offset: 0x00240684
		public override void OnUpdate()
		{
			this.DoSetValues();
		}

		// Token: 0x0600768C RID: 30348 RVA: 0x0024248C File Offset: 0x0024068C
		private void DoSetValues()
		{
			if (this.layoutElement == null)
			{
				return;
			}
			if (!this.minWidth.IsNone)
			{
				this.layoutElement.minWidth = this.minWidth.Value;
			}
			if (!this.minHeight.IsNone)
			{
				this.layoutElement.minHeight = this.minHeight.Value;
			}
			if (!this.preferredWidth.IsNone)
			{
				this.layoutElement.preferredWidth = this.preferredWidth.Value;
			}
			if (!this.preferredHeight.IsNone)
			{
				this.layoutElement.preferredHeight = this.preferredHeight.Value;
			}
			if (!this.flexibleWidth.IsNone)
			{
				this.layoutElement.flexibleWidth = this.flexibleWidth.Value;
			}
			if (!this.flexibleHeight.IsNone)
			{
				this.layoutElement.flexibleHeight = this.flexibleHeight.Value;
			}
		}

		// Token: 0x040076DC RID: 30428
		[RequiredField]
		[CheckForComponent(typeof(LayoutElement))]
		[Tooltip("The GameObject with the UI LayoutElement component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040076DD RID: 30429
		[ActionSection("Values")]
		[Tooltip("The minimum width this layout element should have.")]
		public FsmFloat minWidth;

		// Token: 0x040076DE RID: 30430
		[Tooltip("The minimum height this layout element should have.")]
		public FsmFloat minHeight;

		// Token: 0x040076DF RID: 30431
		[Tooltip("The preferred width this layout element should have before additional available width is allocated.")]
		public FsmFloat preferredWidth;

		// Token: 0x040076E0 RID: 30432
		[Tooltip("The preferred height this layout element should have before additional available height is allocated.")]
		public FsmFloat preferredHeight;

		// Token: 0x040076E1 RID: 30433
		[Tooltip("The relative amount of additional available width this layout element should fill out relative to its siblings.")]
		public FsmFloat flexibleWidth;

		// Token: 0x040076E2 RID: 30434
		[Tooltip("The relative amount of additional available height this layout element should fill out relative to its siblings.")]
		public FsmFloat flexibleHeight;

		// Token: 0x040076E3 RID: 30435
		[ActionSection("Options")]
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x040076E4 RID: 30436
		private LayoutElement layoutElement;
	}
}
