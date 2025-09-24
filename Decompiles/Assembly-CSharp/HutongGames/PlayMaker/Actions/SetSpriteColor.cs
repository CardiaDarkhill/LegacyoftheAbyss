using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001072 RID: 4210
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Sets the color of a sprite renderer")]
	public class SetSpriteColor : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072E2 RID: 29410 RVA: 0x002353C0 File Offset: 0x002335C0
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

		// Token: 0x060072E3 RID: 29411 RVA: 0x00235434 File Offset: 0x00233634
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.originalColor = this.cachedComponent.color;
			this.SetColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060072E4 RID: 29412 RVA: 0x00235480 File Offset: 0x00233680
		public override void OnUpdate()
		{
			this.SetColor();
		}

		// Token: 0x060072E5 RID: 29413 RVA: 0x00235488 File Offset: 0x00233688
		private void SetColor()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			this.newColor = this.cachedComponent.color;
			if (!this.color.IsNone)
			{
				this.newColor = this.color.Value;
			}
			if (!this.red.IsNone)
			{
				this.newColor.r = this.red.Value;
			}
			if (!this.green.IsNone)
			{
				this.newColor.g = this.green.Value;
			}
			if (!this.blue.IsNone)
			{
				this.newColor.b = this.blue.Value;
			}
			if (!this.alpha.IsNone)
			{
				this.newColor.a = this.alpha.Value;
			}
			this.cachedComponent.color = this.newColor;
		}

		// Token: 0x060072E6 RID: 29414 RVA: 0x0023557B File Offset: 0x0023377B
		public override void OnExit()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.cachedComponent.color = this.originalColor;
			}
		}

		// Token: 0x040072E3 RID: 29411
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072E4 RID: 29412
		[Tooltip("Set the Color of the SpriteRenderer component")]
		public FsmColor color;

		// Token: 0x040072E5 RID: 29413
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the red channel")]
		public FsmFloat red;

		// Token: 0x040072E6 RID: 29414
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the green channel")]
		public FsmFloat green;

		// Token: 0x040072E7 RID: 29415
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the blue channel")]
		public FsmFloat blue;

		// Token: 0x040072E8 RID: 29416
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the alpha channel")]
		public FsmFloat alpha;

		// Token: 0x040072E9 RID: 29417
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040072EA RID: 29418
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040072EB RID: 29419
		private Color originalColor;

		// Token: 0x040072EC RID: 29420
		private Color newColor;
	}
}
