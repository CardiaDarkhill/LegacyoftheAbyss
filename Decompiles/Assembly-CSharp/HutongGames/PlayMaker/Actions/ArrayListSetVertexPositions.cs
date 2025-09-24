using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5F RID: 2911
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Set mesh vertex positions based on vector3 found in an arrayList")]
	public class ArrayListSetVertexPositions : ArrayListActions
	{
		// Token: 0x06005A82 RID: 23170 RVA: 0x001C9B24 File Offset: 0x001C7D24
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.mesh = null;
			this.everyFrame = false;
		}

		// Token: 0x06005A83 RID: 23171 RVA: 0x001C9B44 File Offset: 0x001C7D44
		public override void OnEnter()
		{
			GameObject value = this.mesh.Value;
			if (value == null)
			{
				base.Finish();
				return;
			}
			MeshFilter component = value.GetComponent<MeshFilter>();
			if (component == null)
			{
				base.Finish();
				return;
			}
			this._mesh = component.mesh;
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.SetVertexPositions();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A84 RID: 23172 RVA: 0x001C9BC8 File Offset: 0x001C7DC8
		public override void OnUpdate()
		{
			this.SetVertexPositions();
		}

		// Token: 0x06005A85 RID: 23173 RVA: 0x001C9BD0 File Offset: 0x001C7DD0
		public void SetVertexPositions()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this._vertices = new Vector3[this.proxy.arrayList.Count];
			int num = 0;
			foreach (object obj in this.proxy.arrayList)
			{
				Vector3 vector = (Vector3)obj;
				this._vertices[num] = vector;
				num++;
			}
			this._mesh.vertices = this._vertices;
		}

		// Token: 0x04005624 RID: 22052
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005625 RID: 22053
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005626 RID: 22054
		[ActionSection("Target")]
		[Tooltip("The GameObject to set the mesh vertex positions to")]
		[CheckForComponent(typeof(MeshFilter))]
		public FsmGameObject mesh;

		// Token: 0x04005627 RID: 22055
		public bool everyFrame;

		// Token: 0x04005628 RID: 22056
		private Mesh _mesh;

		// Token: 0x04005629 RID: 22057
		private Vector3[] _vertices;
	}
}
