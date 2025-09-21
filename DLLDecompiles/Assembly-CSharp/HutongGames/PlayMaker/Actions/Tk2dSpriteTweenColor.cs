using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7D RID: 2941
	[ActionCategory("2D Toolkit/Sprite")]
	[Tooltip("Tween a sprite color \nNOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite)")]
	public class Tk2dSpriteTweenColor : FsmStateAction
	{
		// Token: 0x06005B37 RID: 23351 RVA: 0x001CC53E File Offset: 0x001CA73E
		public override void Reset()
		{
			this.gameObject = null;
			this.color = null;
			this.duration = null;
		}

		// Token: 0x06005B38 RID: 23352 RVA: 0x001CC558 File Offset: 0x001CA758
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.sprite = ownerDefaultTarget.GetComponent<tk2dBaseSprite>();
			if (!this.sprite)
			{
				base.Finish();
				return;
			}
			this.elapsed = 0f;
			this.startColor = this.sprite.color;
		}

		// Token: 0x06005B39 RID: 23353 RVA: 0x001CC5C0 File Offset: 0x001CA7C0
		public override void OnUpdate()
		{
			if (this.elapsed <= this.duration.Value && this.duration.Value > 0f)
			{
				this.sprite.color = Color.Lerp(this.startColor, this.color.Value, this.elapsed / this.duration.Value);
				this.elapsed += Time.deltaTime;
				return;
			}
			this.sprite.color = this.color.Value;
			base.Finish();
		}

		// Token: 0x06005B3A RID: 23354 RVA: 0x001CC654 File Offset: 0x001CA854
		public override void OnExit()
		{
			this.sprite.color = this.color.Value;
		}

		// Token: 0x040056B2 RID: 22194
		[RequiredField]
		[Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dBaseSprite or derived component attached ( tk2dSprite, tk2dAnimatedSprite).")]
		[CheckForComponent(typeof(tk2dBaseSprite))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056B3 RID: 22195
		[RequiredField]
		[UIHint(UIHint.FsmColor)]
		public FsmColor color;

		// Token: 0x040056B4 RID: 22196
		[RequiredField]
		public FsmFloat duration;

		// Token: 0x040056B5 RID: 22197
		private float elapsed;

		// Token: 0x040056B6 RID: 22198
		private Color startColor;

		// Token: 0x040056B7 RID: 22199
		private tk2dBaseSprite sprite;
	}
}
