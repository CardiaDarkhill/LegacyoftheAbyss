using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE4 RID: 3556
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the GameObject mapped to this human bone id")]
	public class GetAnimatorBoneGameObject : ComponentAction<Animator>
	{
		// Token: 0x060066C9 RID: 26313 RVA: 0x002086A8 File Offset: 0x002068A8
		public override void Reset()
		{
			this.gameObject = null;
			this.bone = HumanBodyBones.Hips;
			this.boneGameObject = null;
		}

		// Token: 0x060066CA RID: 26314 RVA: 0x002086CC File Offset: 0x002068CC
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.boneGameObject.Value = this.cachedComponent.GetBoneTransform((HumanBodyBones)this.bone.Value).gameObject;
			}
			base.Finish();
		}

		// Token: 0x0400661C RID: 26140
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400661D RID: 26141
		[Tooltip("The bone reference")]
		[ObjectType(typeof(HumanBodyBones))]
		public FsmEnum bone;

		// Token: 0x0400661E RID: 26142
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Bone's GameObject")]
		public FsmGameObject boneGameObject;
	}
}
