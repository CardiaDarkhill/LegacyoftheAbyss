using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200106C RID: 4204
	[ActionCategory(ActionCategory.SpriteRenderer)]
	[Tooltip("Gets the Flips values of a of a SpriteRenderer component.")]
	public class GetSpriteFlip : ComponentAction<SpriteRenderer>
	{
		// Token: 0x060072CE RID: 29390 RVA: 0x0023512E File Offset: 0x0023332E
		public override void Reset()
		{
			this.gameObject = null;
			this.x = null;
			this.y = null;
		}

		// Token: 0x060072CF RID: 29391 RVA: 0x00235145 File Offset: 0x00233345
		public override void OnEnter()
		{
			this.GetFlip();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060072D0 RID: 29392 RVA: 0x0023515B File Offset: 0x0023335B
		public override void OnUpdate()
		{
			this.GetFlip();
		}

		// Token: 0x060072D1 RID: 29393 RVA: 0x00235164 File Offset: 0x00233364
		private void GetFlip()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (!this.x.IsNone)
			{
				this.x.Value = this.cachedComponent.flipX;
			}
			if (!this.y.IsNone)
			{
				this.y.Value = this.cachedComponent.flipY;
			}
		}

		// Token: 0x040072D4 RID: 29396
		[RequiredField]
		[CheckForComponent(typeof(SpriteRenderer))]
		[Tooltip("The GameObject with the SpriteRenderer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072D5 RID: 29397
		[Tooltip("The X flip value")]
		[UIHint(UIHint.Variable)]
		public FsmBool x;

		// Token: 0x040072D6 RID: 29398
		[Tooltip("The Y flip value")]
		[UIHint(UIHint.Variable)]
		public FsmBool y;

		// Token: 0x040072D7 RID: 29399
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
