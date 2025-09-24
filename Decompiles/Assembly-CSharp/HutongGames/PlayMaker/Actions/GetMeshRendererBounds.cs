using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6F RID: 3183
	[ActionCategory("GameObject")]
	[Tooltip("Set Mesh Renderer to active or inactive. Can only be one Mesh Renderer on object. ")]
	public class GetMeshRendererBounds : FsmStateAction
	{
		// Token: 0x06006015 RID: 24597 RVA: 0x001E6EDE File Offset: 0x001E50DE
		public override void Reset()
		{
			this.gameObject = null;
			this.width = null;
			this.height = null;
			this.widthMax = null;
			this.heightMax = null;
		}

		// Token: 0x06006016 RID: 24598 RVA: 0x001E6F04 File Offset: 0x001E5104
		public override void OnEnter()
		{
			if (this.gameObject != null)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				MeshRenderer component = ownerDefaultTarget.GetComponent<MeshRenderer>();
				if (component != null)
				{
					this.width.Value = component.bounds.size.x;
				}
				foreach (object obj in ownerDefaultTarget.transform)
				{
					MeshRenderer component2 = ((Transform)obj).GetComponent<MeshRenderer>();
					if (component2 != null)
					{
						float x = component2.bounds.size.x;
						float y = component2.bounds.size.y;
						if (x > this.width.Value)
						{
							this.width.Value = x;
						}
						if (y > this.height.Value)
						{
							this.height.Value = y;
						}
					}
				}
				if (!this.widthMax.IsNone && this.width.Value > this.widthMax.Value)
				{
					this.width.Value = 0f;
				}
				this.height.Value = component.bounds.size.y;
				if (!this.heightMax.IsNone && this.height.Value > this.heightMax.Value)
				{
					this.height.Value = 0f;
				}
			}
			base.Finish();
		}

		// Token: 0x04005D6E RID: 23918
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D6F RID: 23919
		[UIHint(UIHint.Variable)]
		public FsmFloat width;

		// Token: 0x04005D70 RID: 23920
		[UIHint(UIHint.Variable)]
		public FsmFloat height;

		// Token: 0x04005D71 RID: 23921
		public FsmFloat widthMax;

		// Token: 0x04005D72 RID: 23922
		public FsmFloat heightMax;
	}
}
