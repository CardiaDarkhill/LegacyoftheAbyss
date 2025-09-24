using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B51 RID: 2897
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Store all active GameObjects with a specific tag. Tags must be declared in the tag manager before using them")]
	public class ArrayListFindGameObjectsByTag : ArrayListActions
	{
		// Token: 0x06005A39 RID: 23097 RVA: 0x001C8740 File Offset: 0x001C6940
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.tag = null;
		}

		// Token: 0x06005A3A RID: 23098 RVA: 0x001C8757 File Offset: 0x001C6957
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.FindGOByTag();
			}
			base.Finish();
		}

		// Token: 0x06005A3B RID: 23099 RVA: 0x001C878C File Offset: 0x001C698C
		public void FindGOByTag()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.proxy.arrayList.Clear();
			GameObject[] c = GameObject.FindGameObjectsWithTag(this.tag.Value);
			this.proxy.arrayList.InsertRange(0, c);
		}

		// Token: 0x040055CB RID: 21963
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055CC RID: 21964
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040055CD RID: 21965
		[Tooltip("the tag")]
		public FsmString tag;
	}
}
