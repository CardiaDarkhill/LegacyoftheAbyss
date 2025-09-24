using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200112C RID: 4396
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets various properties of a UI Layout Element component.")]
	public class UiLayoutElementGetValues : ComponentAction<LayoutElement>
	{
		// Token: 0x06007684 RID: 30340 RVA: 0x002420D4 File Offset: 0x002402D4
		public override void Reset()
		{
			this.gameObject = null;
			this.ignoreLayout = null;
			this.minWidthEnabled = null;
			this.minHeightEnabled = null;
			this.preferredWidthEnabled = null;
			this.preferredHeightEnabled = null;
			this.flexibleWidthEnabled = null;
			this.flexibleHeightEnabled = null;
			this.minWidth = null;
			this.minHeight = null;
			this.preferredWidth = null;
			this.preferredHeight = null;
			this.flexibleWidth = null;
			this.flexibleHeight = null;
		}

		// Token: 0x06007685 RID: 30341 RVA: 0x00242144 File Offset: 0x00240344
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.layoutElement = this.cachedComponent;
			}
			this.DoGetValues();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007686 RID: 30342 RVA: 0x0024218C File Offset: 0x0024038C
		public override void OnUpdate()
		{
			this.DoGetValues();
		}

		// Token: 0x06007687 RID: 30343 RVA: 0x00242194 File Offset: 0x00240394
		private void DoGetValues()
		{
			if (this.layoutElement == null)
			{
				return;
			}
			if (!this.ignoreLayout.IsNone)
			{
				this.ignoreLayout.Value = this.layoutElement.ignoreLayout;
			}
			if (!this.minWidthEnabled.IsNone)
			{
				this.minWidthEnabled.Value = (this.layoutElement.minWidth != 0f);
			}
			if (!this.minWidth.IsNone)
			{
				this.minWidth.Value = this.layoutElement.minWidth;
			}
			if (!this.minHeightEnabled.IsNone)
			{
				this.minHeightEnabled.Value = (this.layoutElement.minHeight != 0f);
			}
			if (!this.minHeight.IsNone)
			{
				this.minHeight.Value = this.layoutElement.minHeight;
			}
			if (!this.preferredWidthEnabled.IsNone)
			{
				this.preferredWidthEnabled.Value = (this.layoutElement.preferredWidth != 0f);
			}
			if (!this.preferredWidth.IsNone)
			{
				this.preferredWidth.Value = this.layoutElement.preferredWidth;
			}
			if (!this.preferredHeightEnabled.IsNone)
			{
				this.preferredHeightEnabled.Value = (this.layoutElement.preferredHeight != 0f);
			}
			if (!this.preferredHeight.IsNone)
			{
				this.preferredHeight.Value = this.layoutElement.preferredHeight;
			}
			if (!this.flexibleWidthEnabled.IsNone)
			{
				this.flexibleWidthEnabled.Value = (this.layoutElement.flexibleWidth != 0f);
			}
			if (!this.flexibleWidth.IsNone)
			{
				this.flexibleWidth.Value = this.layoutElement.flexibleWidth;
			}
			if (!this.flexibleHeightEnabled.IsNone)
			{
				this.flexibleHeightEnabled.Value = (this.layoutElement.flexibleHeight != 0f);
			}
			if (!this.flexibleHeight.IsNone)
			{
				this.flexibleHeight.Value = this.layoutElement.flexibleHeight;
			}
		}

		// Token: 0x040076CC RID: 30412
		[RequiredField]
		[CheckForComponent(typeof(LayoutElement))]
		[Tooltip("The GameObject with the UI LayoutElement component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040076CD RID: 30413
		[ActionSection("Values")]
		[Tooltip("Is this element use Layout constraints")]
		[UIHint(UIHint.Variable)]
		public FsmBool ignoreLayout;

		// Token: 0x040076CE RID: 30414
		[Tooltip("The minimum width enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool minWidthEnabled;

		// Token: 0x040076CF RID: 30415
		[Tooltip("The minimum width this layout element should have.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat minWidth;

		// Token: 0x040076D0 RID: 30416
		[Tooltip("The minimum height enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool minHeightEnabled;

		// Token: 0x040076D1 RID: 30417
		[Tooltip("The minimum height this layout element should have.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat minHeight;

		// Token: 0x040076D2 RID: 30418
		[Tooltip("The preferred width enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool preferredWidthEnabled;

		// Token: 0x040076D3 RID: 30419
		[Tooltip("The preferred width this layout element should have before additional available width is allocated.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat preferredWidth;

		// Token: 0x040076D4 RID: 30420
		[Tooltip("The preferred height enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool preferredHeightEnabled;

		// Token: 0x040076D5 RID: 30421
		[Tooltip("The preferred height this layout element should have before additional available height is allocated.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat preferredHeight;

		// Token: 0x040076D6 RID: 30422
		[Tooltip("The flexible width enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool flexibleWidthEnabled;

		// Token: 0x040076D7 RID: 30423
		[Tooltip("The relative amount of additional available width this layout element should fill out relative to its siblings.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat flexibleWidth;

		// Token: 0x040076D8 RID: 30424
		[Tooltip("The flexible height enabled state")]
		[UIHint(UIHint.Variable)]
		public FsmBool flexibleHeightEnabled;

		// Token: 0x040076D9 RID: 30425
		[Tooltip("The relative amount of additional available height this layout element should fill out relative to its siblings.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat flexibleHeight;

		// Token: 0x040076DA RID: 30426
		[ActionSection("Options")]
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x040076DB RID: 30427
		private LayoutElement layoutElement;
	}
}
