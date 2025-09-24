using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B1C RID: 2844
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Concat joins two or more arrayList proxy components. if a target is specified, the method use the target store the concatenation, else the ")]
	public class ArrayListConcat : ArrayListActions
	{
		// Token: 0x06005964 RID: 22884 RVA: 0x001C52B8 File Offset: 0x001C34B8
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.arrayListGameObjectTargets = null;
			this.referenceTargets = null;
		}

		// Token: 0x06005965 RID: 22885 RVA: 0x001C52D6 File Offset: 0x001C34D6
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListConcat(this.proxy.arrayList);
			}
			base.Finish();
		}

		// Token: 0x06005966 RID: 22886 RVA: 0x001C5314 File Offset: 0x001C3514
		public void DoArrayListConcat(ArrayList source)
		{
			if (!base.isProxyValid())
			{
				return;
			}
			for (int i = 0; i < this.arrayListGameObjectTargets.Length; i++)
			{
				if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.arrayListGameObjectTargets[i]), this.referenceTargets[i].Value) && base.isProxyValid())
				{
					foreach (object value in this.proxy.arrayList)
					{
						source.Add(value);
						Debug.Log("count " + source.Count.ToString());
					}
				}
			}
		}

		// Token: 0x040054C0 RID: 21696
		[ActionSection("Storage")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054C1 RID: 21697
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to store the concatenation ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040054C2 RID: 21698
		[ActionSection("ArrayLists to concatenate")]
		[CompoundArray("ArrayLists", "ArrayList GameObject", "Reference")]
		[RequiredField]
		[Tooltip("The GameObject with the PlayMaker ArrayList Proxy component to copy to")]
		[ObjectType(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault[] arrayListGameObjectTargets;

		// Token: 0x040054C3 RID: 21699
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy to ( necessary if several component coexists on the same GameObject")]
		public FsmString[] referenceTargets;
	}
}
