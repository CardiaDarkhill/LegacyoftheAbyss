using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001073 RID: 4211
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Sets the Flips values of a of a SpriteRenderer component.")]
	public class SetSpriteFlip : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072E8 RID: 29416 RVA: 0x002355BD File Offset: 0x002337BD
		public override void Reset()
		{
			this.gameObject = null;
			this.x = null;
			this.y = null;
			this.resetOnExit = false;
		}

		// Token: 0x060072E9 RID: 29417 RVA: 0x002355E0 File Offset: 0x002337E0
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.x_orig = this.cachedComponent.flipX;
				this.y_orig = this.cachedComponent.flipY;
			}
			this.FlipSprites();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060072EA RID: 29418 RVA: 0x0023564A File Offset: 0x0023384A
		public override void OnUpdate()
		{
			this.FlipSprites();
		}

		// Token: 0x060072EB RID: 29419 RVA: 0x00235652 File Offset: 0x00233852
		public override void OnExit()
		{
			if (this.resetOnExit.Value)
			{
				this.cachedComponent.flipX = this.x_orig;
				this.cachedComponent.flipY = this.y_orig;
			}
		}

		// Token: 0x060072EC RID: 29420 RVA: 0x00235684 File Offset: 0x00233884
		private void FlipSprites()
		{
			if (!this.x.IsNone)
			{
				this.cachedComponent.flipX = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				this.cachedComponent.flipY = this.y.Value;
			}
		}

		// Token: 0x040072ED RID: 29421
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072EE RID: 29422
		[Tooltip("The X Flip value")]
		public FsmBool x;

		// Token: 0x040072EF RID: 29423
		[Tooltip("The Y Flip value")]
		public FsmBool y;

		// Token: 0x040072F0 RID: 29424
		[Tooltip("Reset flip values when state exits")]
		public FsmBool resetOnExit;

		// Token: 0x040072F1 RID: 29425
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040072F2 RID: 29426
		private bool x_orig;

		// Token: 0x040072F3 RID: 29427
		private bool y_orig;
	}
}
