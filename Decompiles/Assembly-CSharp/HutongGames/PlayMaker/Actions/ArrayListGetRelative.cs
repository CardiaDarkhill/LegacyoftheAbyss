using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B29 RID: 2857
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Gets an item from a PlayMaker ArrayList Proxy component using a base index and a relative increment. This allows you to move to next or previous items granuraly")]
	public class ArrayListGetRelative : ArrayListActions
	{
		// Token: 0x06005998 RID: 22936 RVA: 0x001C658C File Offset: 0x001C478C
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.baseIndex = null;
			this.increment = null;
			this.result = null;
			this.resultIndex = null;
		}

		// Token: 0x06005999 RID: 22937 RVA: 0x001C65B8 File Offset: 0x001C47B8
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.GetItemAtIncrement();
			}
			base.Finish();
		}

		// Token: 0x0600599A RID: 22938 RVA: 0x001C65EC File Offset: 0x001C47EC
		public void GetItemAtIncrement()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (this.baseIndex.Value + this.increment.Value >= 0)
			{
				this.resultIndex.Value = (this.baseIndex.Value + this.increment.Value) % this.proxy.arrayList.Count;
			}
			else
			{
				this.resultIndex.Value = this.proxy.arrayList.Count - Mathf.Abs(this.baseIndex.Value + this.increment.Value) % this.proxy.arrayList.Count;
			}
			object value = this.proxy.arrayList[this.resultIndex.Value];
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.result, value);
		}

		// Token: 0x04005515 RID: 21781
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005516 RID: 21782
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005517 RID: 21783
		[Tooltip("The index base to compute the item to get")]
		public FsmInt baseIndex;

		// Token: 0x04005518 RID: 21784
		[Tooltip("The incremental value from the base index to get the value from. Overshooting the range will loop back on the list.")]
		public FsmInt increment;

		// Token: 0x04005519 RID: 21785
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmVar result;

		// Token: 0x0400551A RID: 21786
		[Tooltip("The index of the result")]
		[UIHint(UIHint.Variable)]
		public FsmInt resultIndex;
	}
}
