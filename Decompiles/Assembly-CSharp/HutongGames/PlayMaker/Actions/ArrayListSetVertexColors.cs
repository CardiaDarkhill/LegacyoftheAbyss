using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5E RID: 2910
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Set a mesh vertex colors based on colors found in an arrayList")]
	public class ArrayListSetVertexColors : ArrayListActions
	{
		// Token: 0x06005A7D RID: 23165 RVA: 0x001C99D1 File Offset: 0x001C7BD1
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.mesh = null;
			this.everyFrame = false;
		}

		// Token: 0x06005A7E RID: 23166 RVA: 0x001C99F0 File Offset: 0x001C7BF0
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
				this.SetVertexColors();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005A7F RID: 23167 RVA: 0x001C9A74 File Offset: 0x001C7C74
		public override void OnUpdate()
		{
			this.SetVertexColors();
		}

		// Token: 0x06005A80 RID: 23168 RVA: 0x001C9A7C File Offset: 0x001C7C7C
		public void SetVertexColors()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this._colors = new Color[this.proxy.arrayList.Count];
			int num = 0;
			foreach (object obj in this.proxy.arrayList)
			{
				Color color = (Color)obj;
				this._colors[num] = color;
				num++;
			}
			this._mesh.colors = this._colors;
		}

		// Token: 0x0400561E RID: 22046
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400561F RID: 22047
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005620 RID: 22048
		[ActionSection("Target")]
		[Tooltip("The GameObject to set the mesh colors to")]
		[CheckForComponent(typeof(MeshFilter))]
		public FsmGameObject mesh;

		// Token: 0x04005621 RID: 22049
		public bool everyFrame;

		// Token: 0x04005622 RID: 22050
		private Mesh _mesh;

		// Token: 0x04005623 RID: 22051
		private Color[] _colors;
	}
}
