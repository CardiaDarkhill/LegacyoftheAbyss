using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F91 RID: 3985
	[ActionCategory("Mesh")]
	[Tooltip("Gets the number of vertices in a GameObject's mesh. Useful in conjunction with GetVertexPosition.")]
	public class GetVertexCount : FsmStateAction
	{
		// Token: 0x06006E27 RID: 28199 RVA: 0x002226D3 File Offset: 0x002208D3
		public override void Reset()
		{
			this.gameObject = null;
			this.storeCount = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E28 RID: 28200 RVA: 0x002226EA File Offset: 0x002208EA
		public override void OnEnter()
		{
			this.DoGetVertexCount();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E29 RID: 28201 RVA: 0x00222700 File Offset: 0x00220900
		public override void OnUpdate()
		{
			this.DoGetVertexCount();
		}

		// Token: 0x06006E2A RID: 28202 RVA: 0x00222708 File Offset: 0x00220908
		private void DoGetVertexCount()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				MeshFilter component = ownerDefaultTarget.GetComponent<MeshFilter>();
				if (component == null)
				{
					base.LogError("Missing MeshFilter!");
					return;
				}
				this.storeCount.Value = component.mesh.vertexCount;
			}
		}

		// Token: 0x04006DD9 RID: 28121
		[RequiredField]
		[CheckForComponent(typeof(MeshFilter))]
		[Tooltip("The GameObject to check.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006DDA RID: 28122
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the vertex count in a variable.")]
		public FsmInt storeCount;

		// Token: 0x04006DDB RID: 28123
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
