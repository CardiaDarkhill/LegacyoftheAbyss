using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5D RID: 2909
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Store mesh vertex positions into an arrayList")]
	public class ArrayListGetVertexPositions : ArrayListActions
	{
		// Token: 0x06005A79 RID: 23161 RVA: 0x001C9915 File Offset: 0x001C7B15
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.mesh = null;
		}

		// Token: 0x06005A7A RID: 23162 RVA: 0x001C992C File Offset: 0x001C7B2C
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.getVertexPositions();
			}
			base.Finish();
		}

		// Token: 0x06005A7B RID: 23163 RVA: 0x001C9960 File Offset: 0x001C7B60
		public void getVertexPositions()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.arrayList.Clear();
			GameObject value = this.mesh.Value;
			if (value == null)
			{
				return;
			}
			MeshFilter component = value.GetComponent<MeshFilter>();
			if (component == null)
			{
				return;
			}
			this.proxy.arrayList.InsertRange(0, component.mesh.vertices);
		}

		// Token: 0x0400561B RID: 22043
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400561C RID: 22044
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400561D RID: 22045
		[ActionSection("Source")]
		[Tooltip("the GameObject to get the mesh from")]
		[CheckForComponent(typeof(MeshFilter))]
		public FsmGameObject mesh;
	}
}
