using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200116B RID: 4459
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("The normalized scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.")]
	public class UiScrollRectSetNormalizedPosition : ComponentAction<ScrollRect>
	{
		// Token: 0x060077BB RID: 30651 RVA: 0x00245FA4 File Offset: 0x002441A4
		public override void Reset()
		{
			this.gameObject = null;
			this.normalizedPosition = null;
			this.horizontalPosition = new FsmFloat
			{
				UseVariable = true
			};
			this.verticalPosition = new FsmFloat
			{
				UseVariable = true
			};
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060077BC RID: 30652 RVA: 0x00245FF4 File Offset: 0x002441F4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.scrollRect = this.cachedComponent;
			}
			this.originalValue = this.scrollRect.normalizedPosition;
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060077BD RID: 30653 RVA: 0x0024604D File Offset: 0x0024424D
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x060077BE RID: 30654 RVA: 0x00246058 File Offset: 0x00244258
		private void DoSetValue()
		{
			if (this.scrollRect == null)
			{
				return;
			}
			Vector2 value = this.scrollRect.normalizedPosition;
			if (!this.normalizedPosition.IsNone)
			{
				value = this.normalizedPosition.Value;
			}
			if (!this.horizontalPosition.IsNone)
			{
				value.x = this.horizontalPosition.Value;
			}
			if (!this.verticalPosition.IsNone)
			{
				value.y = this.verticalPosition.Value;
			}
			this.scrollRect.normalizedPosition = value;
		}

		// Token: 0x060077BF RID: 30655 RVA: 0x002460E3 File Offset: 0x002442E3
		public override void OnExit()
		{
			if (this.scrollRect == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.scrollRect.normalizedPosition = this.originalValue;
			}
		}

		// Token: 0x04007838 RID: 30776
		[RequiredField]
		[CheckForComponent(typeof(ScrollRect))]
		[Tooltip("The GameObject with the UI ScrollRect component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007839 RID: 30777
		[Tooltip("The position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0.")]
		public FsmVector2 normalizedPosition;

		// Token: 0x0400783A RID: 30778
		[Tooltip("The horizontal position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat horizontalPosition;

		// Token: 0x0400783B RID: 30779
		[Tooltip("The vertical position's value of the UI ScrollRect component. Ranges from 0.0 to 1.0.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat verticalPosition;

		// Token: 0x0400783C RID: 30780
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400783D RID: 30781
		[Tooltip("Repeats every frame")]
		public bool everyFrame;

		// Token: 0x0400783E RID: 30782
		private ScrollRect scrollRect;

		// Token: 0x0400783F RID: 30783
		private Vector2 originalValue;
	}
}
