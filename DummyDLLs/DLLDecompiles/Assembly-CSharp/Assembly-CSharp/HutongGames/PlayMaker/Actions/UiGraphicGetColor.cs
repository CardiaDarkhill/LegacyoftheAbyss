using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001141 RID: 4417
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the color of a UI Graphic component. (E.g. UI Sprite)")]
	public class UiGraphicGetColor : ComponentAction<Graphic>
	{
		// Token: 0x060076E6 RID: 30438 RVA: 0x00243E50 File Offset: 0x00242050
		public override void Reset()
		{
			this.gameObject = null;
			this.color = null;
		}

		// Token: 0x060076E7 RID: 30439 RVA: 0x00243E60 File Offset: 0x00242060
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.uiComponent = this.cachedComponent;
			}
			this.DoGetColorValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060076E8 RID: 30440 RVA: 0x00243EA8 File Offset: 0x002420A8
		public override void OnUpdate()
		{
			this.DoGetColorValue();
		}

		// Token: 0x060076E9 RID: 30441 RVA: 0x00243EB0 File Offset: 0x002420B0
		private void DoGetColorValue()
		{
			if (this.uiComponent != null)
			{
				this.color.Value = this.uiComponent.color;
			}
		}

		// Token: 0x04007761 RID: 30561
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The GameObject with the UI component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007762 RID: 30562
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color of the UI component")]
		public FsmColor color;

		// Token: 0x04007763 RID: 30563
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x04007764 RID: 30564
		private Graphic uiComponent;
	}
}
