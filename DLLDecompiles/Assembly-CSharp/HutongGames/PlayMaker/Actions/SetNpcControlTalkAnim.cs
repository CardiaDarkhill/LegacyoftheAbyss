using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001356 RID: 4950
	public class SetNpcControlTalkAnim : FsmStateAction
	{
		// Token: 0x06007FCE RID: 32718 RVA: 0x0025C869 File Offset: 0x0025AA69
		public override void Reset()
		{
			this.Target = null;
			this.Anim = null;
		}

		// Token: 0x06007FCF RID: 32719 RVA: 0x0025C87C File Offset: 0x0025AA7C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				NPCControlBase component = safe.GetComponent<NPCControlBase>();
				if (component)
				{
					component.HeroAnimation = (HeroTalkAnimation.AnimationTypes)this.Anim.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04007F49 RID: 32585
		[RequiredField]
		[CheckForComponent(typeof(NPCControlBase))]
		public FsmOwnerDefault Target;

		// Token: 0x04007F4A RID: 32586
		[ObjectType(typeof(HeroTalkAnimation.AnimationTypes))]
		public FsmEnum Anim;
	}
}
