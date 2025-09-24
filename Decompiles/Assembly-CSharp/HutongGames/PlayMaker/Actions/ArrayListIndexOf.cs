using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B25 RID: 2853
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the index of an item from a PlayMaker Array List Proxy component. Can search within a range")]
	public class ArrayListIndexOf : ArrayListActions
	{
		// Token: 0x06005989 RID: 22921 RVA: 0x001C6014 File Offset: 0x001C4214
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.startIndex = null;
			this.count = null;
			this.itemFound = null;
			this.itemNotFound = null;
			this.variable = null;
		}

		// Token: 0x0600598A RID: 22922 RVA: 0x001C6047 File Offset: 0x001C4247
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListIndexOf();
			}
			base.Finish();
		}

		// Token: 0x0600598B RID: 22923 RVA: 0x001C607C File Offset: 0x001C427C
		public void DoArrayListIndexOf()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable);
			int num = -1;
			try
			{
				if (this.startIndex.IsNone)
				{
					Debug.Log("hello");
					num = PlayMakerUtils_Extensions.IndexOf(this.proxy.arrayList, valueFromFsmVar);
				}
				else if (this.count.IsNone || this.count.Value == 0)
				{
					if (this.startIndex.Value < 0 || this.startIndex.Value >= this.proxy.arrayList.Count)
					{
						base.LogError("start index out of range");
						return;
					}
					num = PlayMakerUtils_Extensions.IndexOf(this.proxy.arrayList, valueFromFsmVar);
				}
				else
				{
					if (this.startIndex.Value < 0 || this.startIndex.Value >= this.proxy.arrayList.Count - this.count.Value)
					{
						base.LogError("start index and count out of range");
						return;
					}
					num = PlayMakerUtils_Extensions.IndexOf(this.proxy.arrayList, valueFromFsmVar);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
				return;
			}
			this.indexOf.Value = num;
			if (num == -1)
			{
				base.Fsm.Event(this.itemNotFound);
				return;
			}
			base.Fsm.Event(this.itemFound);
		}

		// Token: 0x040054F9 RID: 21753
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054FA RID: 21754
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x040054FB RID: 21755
		[Tooltip("Optional start index to search from: set to 0 to ignore")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt startIndex;

		// Token: 0x040054FC RID: 21756
		[Tooltip("Optional amount of elements to search within: set to 0 to ignore")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt count;

		// Token: 0x040054FD RID: 21757
		[ActionSection("Data")]
		[RequiredField]
		[Tooltip("The variable to get the index of.")]
		public FsmVar variable;

		// Token: 0x040054FE RID: 21758
		[ActionSection("Result")]
		[Tooltip("The index of the item described below")]
		[UIHint(UIHint.Variable)]
		public FsmInt indexOf;

		// Token: 0x040054FF RID: 21759
		[Tooltip("Optional Event sent if this arrayList contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent itemFound;

		// Token: 0x04005500 RID: 21760
		[Tooltip("Optional Event sent if this arrayList does not contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent itemNotFound;

		// Token: 0x04005501 RID: 21761
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Optional Event to trigger if the action fails ( likely an out of range exception when using wrong values for index and/or count)")]
		public FsmEvent failureEvent;
	}
}
