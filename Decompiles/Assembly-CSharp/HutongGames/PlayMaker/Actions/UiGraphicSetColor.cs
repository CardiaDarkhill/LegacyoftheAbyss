using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001142 RID: 4418
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Set Graphic Color. E.g. to set Sprite Color.")]
	public class UiGraphicSetColor : ComponentAction<Graphic>
	{
		// Token: 0x060076EB RID: 30443 RVA: 0x00243EE0 File Offset: 0x002420E0
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
			this.resetOnExit = null;
			this.everyFrame = false;
		}

		// Token: 0x060076EC RID: 30444 RVA: 0x00243F54 File Offset: 0x00242154
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.uiComponent = this.cachedComponent;
			}
			this.originalColor = this.uiComponent.color;
			this.DoSetColorValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060076ED RID: 30445 RVA: 0x00243FAD File Offset: 0x002421AD
		public override void OnUpdate()
		{
			this.DoSetColorValue();
		}

		// Token: 0x060076EE RID: 30446 RVA: 0x00243FB8 File Offset: 0x002421B8
		private void DoSetColorValue()
		{
			if (this.uiComponent == null)
			{
				return;
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
			this.uiComponent.color = value;
		}

		// Token: 0x060076EF RID: 30447 RVA: 0x00244081 File Offset: 0x00242281
		public override void OnExit()
		{
			if (this.uiComponent == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.uiComponent.color = this.originalColor;
			}
		}

		// Token: 0x04007765 RID: 30565
		[RequiredField]
		[CheckForComponent(typeof(Graphic))]
		[Tooltip("The GameObject with a UI component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007766 RID: 30566
		[Tooltip("The Color of the UI component. Leave to none and set the individual color values, for example to affect just the alpha channel")]
		public FsmColor color;

		// Token: 0x04007767 RID: 30567
		[Tooltip("The red channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
		public FsmFloat red;

		// Token: 0x04007768 RID: 30568
		[Tooltip("The green channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
		public FsmFloat green;

		// Token: 0x04007769 RID: 30569
		[Tooltip("The blue channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
		public FsmFloat blue;

		// Token: 0x0400776A RID: 30570
		[Tooltip("The alpha channel Color of the UI component. Leave to none for no effect, else it overrides the color property")]
		public FsmFloat alpha;

		// Token: 0x0400776B RID: 30571
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400776C RID: 30572
		[Tooltip("Repeats every frame, useful for animation")]
		public bool everyFrame;

		// Token: 0x0400776D RID: 30573
		private Graphic uiComponent;

		// Token: 0x0400776E RID: 30574
		private Color originalColor;
	}
}
