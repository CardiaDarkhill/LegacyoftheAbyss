using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200106B RID: 4203
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Gets the color of a sprite renderer")]
	public class GetSpriteColor : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072C9 RID: 29385 RVA: 0x00234FB4 File Offset: 0x002331B4
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
			this.everyFrame = false;
		}

		// Token: 0x060072CA RID: 29386 RVA: 0x0023501E File Offset: 0x0023321E
		public override void OnEnter()
		{
			this.GetColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060072CB RID: 29387 RVA: 0x00235034 File Offset: 0x00233234
		public override void OnUpdate()
		{
			this.GetColor();
		}

		// Token: 0x060072CC RID: 29388 RVA: 0x0023503C File Offset: 0x0023323C
		private void GetColor()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (!this.color.IsNone)
			{
				this.color.Value = this.cachedComponent.color;
			}
			if (!this.red.IsNone)
			{
				this.red.Value = this.cachedComponent.color.r;
			}
			if (!this.green.IsNone)
			{
				this.green.Value = this.cachedComponent.color.g;
			}
			if (!this.blue.IsNone)
			{
				this.blue.Value = this.cachedComponent.color.b;
			}
			if (!this.alpha.IsNone)
			{
				this.alpha.Value = this.cachedComponent.color.a;
			}
		}

		// Token: 0x040072CD RID: 29389
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072CE RID: 29390
		[UIHint(UIHint.Variable)]
		[Tooltip("Get The Color of the SpriteRenderer component")]
		public FsmColor color;

		// Token: 0x040072CF RID: 29391
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the red channel in a float variable.")]
		public FsmFloat red;

		// Token: 0x040072D0 RID: 29392
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the green channel in a float variable.")]
		public FsmFloat green;

		// Token: 0x040072D1 RID: 29393
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the blue channel in a float variable.")]
		public FsmFloat blue;

		// Token: 0x040072D2 RID: 29394
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the alpha channel in a float variable.")]
		public FsmFloat alpha;

		// Token: 0x040072D3 RID: 29395
		[Tooltip("Repeat every frame. Useful if the color variable is changing.")]
		public bool everyFrame;
	}
}
