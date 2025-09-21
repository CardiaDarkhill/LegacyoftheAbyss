using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C25 RID: 3109
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Color")]
	public class EaseSpriteColor : EaseFsmAction
	{
		// Token: 0x06005EA6 RID: 24230 RVA: 0x001DF052 File Offset: 0x001DD252
		public override void Reset()
		{
			base.Reset();
			this.target = null;
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06005EA7 RID: 24231 RVA: 0x001DF078 File Offset: 0x001DD278
		public override void OnEnter()
		{
			if (this.target.Value != null)
			{
				this.spriteRenderer = this.target.Value.GetComponent<SpriteRenderer>();
				Color color = this.fromValue.Value;
				if (this.spriteRenderer != null)
				{
					if (this.fromValue.IsNone)
					{
						color = (this.fromValue.Value = this.spriteRenderer.color);
					}
					else
					{
						this.spriteRenderer.color = color;
					}
					this.spriteRenderer.enabled = true;
				}
				base.OnEnter();
				this.fromFloats = new float[4];
				this.fromFloats[0] = color.r;
				this.fromFloats[1] = color.g;
				this.fromFloats[2] = color.b;
				this.fromFloats[3] = color.a;
				this.toFloats = new float[4];
				Color value = this.toValue.Value;
				this.toFloats[0] = value.r;
				this.toFloats[1] = value.g;
				this.toFloats[2] = value.b;
				this.toFloats[3] = value.a;
				this.resultFloats = new float[4];
				this.finishInNextStep = false;
			}
		}

		// Token: 0x06005EA8 RID: 24232 RVA: 0x001DF1B9 File Offset: 0x001DD3B9
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06005EA9 RID: 24233 RVA: 0x001DF1C4 File Offset: 0x001DD3C4
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning && this.spriteRenderer != null)
			{
				this.spriteRenderer.color = new Color(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2], this.resultFloats[3]);
			}
			if (this.finishInNextStep)
			{
				if (this.disableSpriteRendererIfTransparent && this.spriteRenderer.color.a == 0f)
				{
					this.spriteRenderer.enabled = false;
				}
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
			if (this.finishAction && !this.finishInNextStep)
			{
				if (this.spriteRenderer != null)
				{
					if (this.reverse.IsNone || !this.reverse.Value)
					{
						this.spriteRenderer.color = this.toValue.Value;
					}
					else
					{
						this.spriteRenderer.color = this.fromValue.Value;
					}
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04005B36 RID: 23350
		[UIHint(UIHint.Variable)]
		[RequiredField]
		public FsmGameObject target;

		// Token: 0x04005B37 RID: 23351
		[RequiredField]
		public FsmColor fromValue;

		// Token: 0x04005B38 RID: 23352
		[RequiredField]
		public FsmColor toValue;

		// Token: 0x04005B39 RID: 23353
		public bool disableSpriteRendererIfTransparent;

		// Token: 0x04005B3A RID: 23354
		private SpriteRenderer spriteRenderer;

		// Token: 0x04005B3B RID: 23355
		private bool finishInNextStep;
	}
}
