using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D53 RID: 3411
	[ActionCategory("GameObject")]
	[Tooltip("Set sprite renderer to active or inactive based on the given current color. Can only be one sprite renderer on object. ")]
	public class SetSpriteRendererByColor : FsmStateAction
	{
		// Token: 0x060063EA RID: 25578 RVA: 0x001F7E7F File Offset: 0x001F607F
		public override void Reset()
		{
			this.gameObject = null;
			this.Color = new FsmColor
			{
				UseVariable = true
			};
			this.EveryFrame = new FsmBool
			{
				UseVariable = false,
				Value = true
			};
		}

		// Token: 0x060063EB RID: 25579 RVA: 0x001F7EB4 File Offset: 0x001F60B4
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					this.spriteRenderer = ownerDefaultTarget.GetComponent<SpriteRenderer>();
				}
			}
			if (this.spriteRenderer != null)
			{
				this.Apply();
			}
			if (this.spriteRenderer == null || !this.EveryFrame.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x060063EC RID: 25580 RVA: 0x001F7F25 File Offset: 0x001F6125
		public override void OnUpdate()
		{
			this.Apply();
		}

		// Token: 0x060063ED RID: 25581 RVA: 0x001F7F30 File Offset: 0x001F6130
		private void Apply()
		{
			if (this.spriteRenderer != null)
			{
				bool flag = this.Color.Value.a > Mathf.Epsilon;
				if (this.spriteRenderer.enabled != flag)
				{
					this.spriteRenderer.enabled = flag;
				}
			}
		}

		// Token: 0x0400624D RID: 25165
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400624E RID: 25166
		[UIHint(UIHint.Variable)]
		public FsmColor Color;

		// Token: 0x0400624F RID: 25167
		public FsmBool EveryFrame;

		// Token: 0x04006250 RID: 25168
		private SpriteRenderer spriteRenderer;
	}
}
