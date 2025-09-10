using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D90 RID: 3472
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationChildren : FsmStateAction
	{
		// Token: 0x06006501 RID: 25857 RVA: 0x001FDEE7 File Offset: 0x001FC0E7
		public override void Reset()
		{
			this.gameObject = null;
			this.clipName = null;
		}

		// Token: 0x06006502 RID: 25858 RVA: 0x001FDEF8 File Offset: 0x001FC0F8
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget != null)
				{
					foreach (object obj in ownerDefaultTarget.transform)
					{
						tk2dSpriteAnimator component = ((Transform)obj).GetComponent<tk2dSpriteAnimator>();
						if (component)
						{
							component.Play(this.clipName.Value);
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04006400 RID: 25600
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006401 RID: 25601
		[RequiredField]
		[Tooltip("The clip name to play")]
		public FsmString clipName;
	}
}
