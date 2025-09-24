using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D22 RID: 3362
	[ActionCategory("Physics 2d")]
	[Tooltip("Set the dimensions of the first BoxCollider 2D on object")]
	public class SetBoxCollider2DSize : FsmStateAction
	{
		// Token: 0x06006324 RID: 25380 RVA: 0x001F5694 File Offset: 0x001F3894
		public override void Reset()
		{
			this.width = new FsmFloat
			{
				UseVariable = true
			};
			this.height = new FsmFloat
			{
				UseVariable = true
			};
			this.offsetX = new FsmFloat
			{
				UseVariable = true
			};
			this.offsetY = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06006325 RID: 25381 RVA: 0x001F56EC File Offset: 0x001F38EC
		public void SetDimensions()
		{
			BoxCollider2D component = base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<BoxCollider2D>();
			Vector2 size = component.size;
			if (!this.width.IsNone)
			{
				size.x = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				size.y = this.height.Value;
			}
			if (!this.offsetX.IsNone)
			{
				component.offset = new Vector3(this.offsetX.Value, component.offset.y);
			}
			if (!this.offsetY.IsNone)
			{
				component.offset = new Vector3(component.offset.x, this.offsetY.Value);
			}
			component.size = size;
		}

		// Token: 0x06006326 RID: 25382 RVA: 0x001F57C2 File Offset: 0x001F39C2
		public override void OnEnter()
		{
			this.SetDimensions();
			base.Finish();
		}

		// Token: 0x04006191 RID: 24977
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006192 RID: 24978
		public FsmFloat width;

		// Token: 0x04006193 RID: 24979
		public FsmFloat height;

		// Token: 0x04006194 RID: 24980
		public FsmFloat offsetX;

		// Token: 0x04006195 RID: 24981
		public FsmFloat offsetY;
	}
}
