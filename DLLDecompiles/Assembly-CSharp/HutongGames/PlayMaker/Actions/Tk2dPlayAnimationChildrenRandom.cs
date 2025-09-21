using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D91 RID: 3473
	[ActionCategory("2D Toolkit/SpriteAnimator")]
	[Tooltip("Plays a sprite animation. \nNOTE: The Game Object must have a tk2dSpriteAnimator attached.")]
	public class Tk2dPlayAnimationChildrenRandom : FsmStateAction
	{
		// Token: 0x06006504 RID: 25860 RVA: 0x001FDF9C File Offset: 0x001FC19C
		public override void Reset()
		{
			this.gameObject = null;
			this.clipNames = null;
		}

		// Token: 0x06006505 RID: 25861 RVA: 0x001FDFAC File Offset: 0x001FC1AC
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
							string name = this.clipNames[Random.Range(0, this.clipNames.Length)];
							component.Play(name);
						}
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04006402 RID: 25602
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006403 RID: 25603
		[RequiredField]
		[Tooltip("The clip name to play")]
		public string[] clipNames;
	}
}
