using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCD RID: 3533
	public abstract class BaseAnimationAction : ComponentAction<Animation>
	{
		// Token: 0x0600665A RID: 26202 RVA: 0x00206F74 File Offset: 0x00205174
		public override void OnActionTargetInvoked(object targetObject)
		{
			AnimationClip animationClip = targetObject as AnimationClip;
			if (animationClip == null)
			{
				return;
			}
			Animation component = base.Owner.GetComponent<Animation>();
			if (component != null)
			{
				component.AddClip(animationClip, animationClip.name);
			}
		}
	}
}
