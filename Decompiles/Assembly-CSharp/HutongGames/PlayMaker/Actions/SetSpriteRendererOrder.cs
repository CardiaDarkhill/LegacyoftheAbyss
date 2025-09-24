using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D54 RID: 3412
	[ActionCategory("Sprite Renderer")]
	public class SetSpriteRendererOrder : FsmStateAction
	{
		// Token: 0x060063EF RID: 25583 RVA: 0x001F7F85 File Offset: 0x001F6185
		public override void Reset()
		{
			this.gameObject = null;
			this.order = null;
			this.delay = null;
			this.timer = 0f;
		}

		// Token: 0x060063F0 RID: 25584 RVA: 0x001F7FA8 File Offset: 0x001F61A8
		public override void OnEnter()
		{
			this.timer = 0f;
			if ((this.delay.IsNone || this.delay.Value == 0f) && this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.sortingOrder = this.order.Value;
						return;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x060063F1 RID: 25585 RVA: 0x001F802C File Offset: 0x001F622C
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget)
				{
					SpriteRenderer component = ownerDefaultTarget.GetComponent<SpriteRenderer>();
					if (component != null)
					{
						component.sortingOrder = this.order.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04006251 RID: 25169
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006252 RID: 25170
		public FsmInt order;

		// Token: 0x04006253 RID: 25171
		public FsmFloat delay;

		// Token: 0x04006254 RID: 25172
		private float timer;
	}
}
