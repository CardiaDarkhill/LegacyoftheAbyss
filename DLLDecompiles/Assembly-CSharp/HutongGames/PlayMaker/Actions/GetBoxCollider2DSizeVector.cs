using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5D RID: 3165
	[ActionCategory("Physics 2d")]
	[Tooltip("Get the dimensions of the first BoxCollider 2D on object. Uses vector2s")]
	public class GetBoxCollider2DSizeVector : FsmStateAction
	{
		// Token: 0x06005FC3 RID: 24515 RVA: 0x001E5B76 File Offset: 0x001E3D76
		public override void Reset()
		{
			this.size = new FsmVector2
			{
				UseVariable = true
			};
			this.offset = new FsmVector2
			{
				UseVariable = true
			};
		}

		// Token: 0x06005FC4 RID: 24516 RVA: 0x001E5B9C File Offset: 0x001E3D9C
		public void GetDimensions()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject1);
			if (!ownerDefaultTarget)
			{
				Debug.LogError("gameObject1 is null!", base.Owner);
				return;
			}
			BoxCollider2D component = ownerDefaultTarget.GetComponent<BoxCollider2D>();
			if (!this.size.IsNone)
			{
				this.size.Value = component.size;
			}
			if (!this.offset.IsNone)
			{
				this.offset.Value = component.offset;
			}
		}

		// Token: 0x06005FC5 RID: 24517 RVA: 0x001E5C17 File Offset: 0x001E3E17
		public override void OnEnter()
		{
			this.GetDimensions();
			base.Finish();
		}

		// Token: 0x04005D1A RID: 23834
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		// Token: 0x04005D1B RID: 23835
		public FsmVector2 size;

		// Token: 0x04005D1C RID: 23836
		public FsmVector2 offset;
	}
}
