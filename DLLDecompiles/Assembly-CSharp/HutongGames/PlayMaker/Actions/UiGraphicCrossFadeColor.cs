using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001140 RID: 4416
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Tweens the color of the CanvasRenderer color associated with this Graphic.")]
	public class UiGraphicCrossFadeColor : ComponentAction<Graphic>
	{
		// Token: 0x060076E3 RID: 30435 RVA: 0x00243CBC File Offset: 0x00241EBC
		public override void Reset()
		{
			this.gameObject = null;
			this.color = null;
			this.red = new FsmFloat
			{
				UseVariable = true
			};
			this.green = new FsmFloat
			{
				UseVariable = true
			};
			this.blue = new FsmFloat
			{
				UseVariable = true
			};
			this.alpha = new FsmFloat
			{
				UseVariable = true
			};
			this.useAlpha = null;
			this.duration = 1f;
			this.ignoreTimeScale = null;
		}

		// Token: 0x060076E4 RID: 30436 RVA: 0x00243D40 File Offset: 0x00241F40
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.uiComponent = this.cachedComponent;
			}
			Color value = this.uiComponent.color;
			if (!this.color.IsNone)
			{
				value = this.color.Value;
			}
			if (!this.red.IsNone)
			{
				value.r = this.red.Value;
			}
			if (!this.green.IsNone)
			{
				value.g = this.green.Value;
			}
			if (!this.blue.IsNone)
			{
				value.b = this.blue.Value;
			}
			if (!this.alpha.IsNone)
			{
				value.a = this.alpha.Value;
			}
			this.uiComponent.CrossFadeColor(value, this.duration.Value, this.ignoreTimeScale.Value, this.useAlpha.Value);
			base.Finish();
		}

		// Token: 0x04007757 RID: 30551
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The GameObject with a UI component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007758 RID: 30552
		[Tooltip("The Color target of the UI component. Leave to none and set the individual color values, for example to affect just the alpha channel")]
		public FsmColor color;

		// Token: 0x04007759 RID: 30553
		[Tooltip("The red channel Color target of the UI component. Leave as None for no effect, else it overrides the color property")]
		public FsmFloat red;

		// Token: 0x0400775A RID: 30554
		[Tooltip("The green channel Color target of the UI component. Leave as None for no effect, else it overrides the color property")]
		public FsmFloat green;

		// Token: 0x0400775B RID: 30555
		[Tooltip("The blue channel Color target of the UI component. Leave as None for no effect, else it overrides the color property")]
		public FsmFloat blue;

		// Token: 0x0400775C RID: 30556
		[Tooltip("The alpha channel Color target of the UI component. Leave as None for no effect, else it overrides the color property")]
		public FsmFloat alpha;

		// Token: 0x0400775D RID: 30557
		[Tooltip("The duration of the tween")]
		public FsmFloat duration;

		// Token: 0x0400775E RID: 30558
		[Tooltip("Should ignore Time.scale?")]
		public FsmBool ignoreTimeScale;

		// Token: 0x0400775F RID: 30559
		[Tooltip("Should also Tween the alpha channel?")]
		public FsmBool useAlpha;

		// Token: 0x04007760 RID: 30560
		private Graphic uiComponent;
	}
}
