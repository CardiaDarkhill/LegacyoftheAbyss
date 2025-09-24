using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5C RID: 2908
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Store a mesh vertex colors into an arrayList")]
	public class ArrayListGetVertexColors : ArrayListActions
	{
		// Token: 0x06005A75 RID: 23157 RVA: 0x001C9858 File Offset: 0x001C7A58
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.mesh = null;
		}

		// Token: 0x06005A76 RID: 23158 RVA: 0x001C986F File Offset: 0x001C7A6F
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.getVertexColors();
			}
			base.Finish();
		}

		// Token: 0x06005A77 RID: 23159 RVA: 0x001C98A4 File Offset: 0x001C7AA4
		public void getVertexColors()
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
			this.proxy.arrayList.InsertRange(0, component.mesh.colors);
		}

		// Token: 0x04005618 RID: 22040
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005619 RID: 22041
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400561A RID: 22042
		[ActionSection("Source")]
		[Tooltip("the GameObject to get the mesh from")]
		[CheckForComponent(typeof(MeshFilter))]
		public FsmGameObject mesh;
	}
}
