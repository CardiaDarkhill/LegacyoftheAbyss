using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B1E RID: 2846
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Copy elements from one PlayMaker ArrayList Proxy component to another")]
	public class ArrayListCopyTo : ArrayListActions
	{
		// Token: 0x0600596C RID: 22892 RVA: 0x001C56C0 File Offset: 0x001C38C0
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.gameObjectTarget = null;
			this.referenceTarget = null;
			this.startIndex = new FsmInt
			{
				UseVariable = true
			};
			this.count = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x0600596D RID: 22893 RVA: 0x001C570D File Offset: 0x001C390D
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListCopyTo(this.proxy.arrayList);
			}
			base.Finish();
		}

		// Token: 0x0600596E RID: 22894 RVA: 0x001C574C File Offset: 0x001C394C
		public void DoArrayListCopyTo(ArrayList source)
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObjectTarget), this.referenceTarget.Value))
			{
				return;
			}
			if (!base.isProxyValid())
			{
				return;
			}
			int value = this.startIndex.Value;
			int num = source.Count;
			int value2 = source.Count;
			if (!this.count.IsNone)
			{
				value2 = this.count.Value;
			}
			if (value < 0 || value >= source.Count)
			{
				base.LogError("start index out of range");
				return;
			}
			if (this.count.Value < 0)
			{
				base.LogError("count can not be negative");
				return;
			}
			num = Mathf.Min(value + value2, source.Count);
			for (int i = value; i < num; i++)
			{
				this.proxy.arrayList.Add(source[i]);
			}
		}

		// Token: 0x040054CA RID: 21706
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to copy from")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054CB RID: 21707
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy from ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040054CC RID: 21708
		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component to copy to")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObjectTarget;

		// Token: 0x040054CD RID: 21709
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component to copy to ( necessary if several component coexists on the same GameObject")]
		public FsmString referenceTarget;

		// Token: 0x040054CE RID: 21710
		[Tooltip("Optional start index to copy from the source, if not set, starts from the beginning")]
		public FsmInt startIndex;

		// Token: 0x040054CF RID: 21711
		[Tooltip("Optional amount of elements to copy, If not set, will copy all from start index.")]
		public FsmInt count;
	}
}
