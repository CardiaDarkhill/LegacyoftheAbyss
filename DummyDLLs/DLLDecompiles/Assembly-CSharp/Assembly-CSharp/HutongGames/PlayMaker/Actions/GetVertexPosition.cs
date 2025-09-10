using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F92 RID: 3986
	[ActionCategory("Mesh")]
	[Tooltip("Gets the position of a vertex in a GameObject's mesh. Hint: Use GetVertexCount to get the number of vertices in a mesh.")]
	public class GetVertexPosition : FsmStateAction
	{
		// Token: 0x06006E2C RID: 28204 RVA: 0x0022276A File Offset: 0x0022096A
		public override void Reset()
		{
			this.gameObject = null;
			this.space = Space.World;
			this.storePosition = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E2D RID: 28205 RVA: 0x00222788 File Offset: 0x00220988
		public override void OnEnter()
		{
			this.DoGetVertexPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E2E RID: 28206 RVA: 0x0022279E File Offset: 0x0022099E
		public override void OnUpdate()
		{
			this.DoGetVertexPosition();
		}

		// Token: 0x06006E2F RID: 28207 RVA: 0x002227A8 File Offset: 0x002209A8
		private void DoGetVertexPosition()
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
				Space space = this.space;
				if (space == Space.World)
				{
					Vector3 position = component.mesh.vertices[this.vertexIndex.Value];
					this.storePosition.Value = ownerDefaultTarget.transform.TransformPoint(position);
					return;
				}
				if (space != Space.Self)
				{
					return;
				}
				this.storePosition.Value = component.mesh.vertices[this.vertexIndex.Value];
			}
		}

		// Token: 0x04006DDC RID: 28124
		[RequiredField]
		[CheckForComponent(typeof(MeshFilter))]
		[Tooltip("The GameObject to check.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006DDD RID: 28125
		[RequiredField]
		[Tooltip("The index of the vertex.")]
		public FsmInt vertexIndex;

		// Token: 0x04006DDE RID: 28126
		[Tooltip("Coordinate system to use.")]
		public Space space;

		// Token: 0x04006DDF RID: 28127
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the vertex position in a variable.")]
		public FsmVector3 storePosition;

		// Token: 0x04006DE0 RID: 28128
		[Tooltip("Repeat every frame. Useful if the mesh is animated.")]
		public bool everyFrame;
	}
}
