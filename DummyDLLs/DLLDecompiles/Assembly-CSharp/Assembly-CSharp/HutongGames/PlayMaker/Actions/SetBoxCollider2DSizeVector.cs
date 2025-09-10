using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D23 RID: 3363
	[ActionCategory("Physics 2d")]
	[Tooltip("Set the dimensions of the first BoxCollider 2D on object. Uses vector2s")]
	public class SetBoxCollider2DSizeVector : FsmStateAction
	{
		// Token: 0x06006328 RID: 25384 RVA: 0x001F57D8 File Offset: 0x001F39D8
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

		// Token: 0x06006329 RID: 25385 RVA: 0x001F5800 File Offset: 0x001F3A00
		public void SetDimensions()
		{
			BoxCollider2D component = base.Fsm.GetOwnerDefaultTarget(this.gameObject1).GetComponent<BoxCollider2D>();
			if (!this.size.IsNone)
			{
				component.size = this.size.Value;
			}
			if (!this.offset.IsNone)
			{
				component.offset = this.offset.Value;
			}
		}

		// Token: 0x0600632A RID: 25386 RVA: 0x001F5860 File Offset: 0x001F3A60
		public override void OnEnter()
		{
			this.SetDimensions();
			base.Finish();
		}

		// Token: 0x04006196 RID: 24982
		[RequiredField]
		public FsmOwnerDefault gameObject1;

		// Token: 0x04006197 RID: 24983
		public FsmVector2 size;

		// Token: 0x04006198 RID: 24984
		public FsmVector2 offset;
	}
}
