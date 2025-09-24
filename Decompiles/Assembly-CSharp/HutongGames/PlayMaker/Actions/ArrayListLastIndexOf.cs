using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B28 RID: 2856
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Return the Last index occurence of an item from a PlayMaker Array List Proxy component. Can search within a range")]
	public class ArrayListLastIndexOf : ArrayListActions
	{
		// Token: 0x06005994 RID: 22932 RVA: 0x001C637B File Offset: 0x001C457B
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.startIndex = null;
			this.count = null;
			this.lastIndexOf = null;
			this.itemFound = null;
			this.itemNotFound = null;
			this.failureEvent = null;
		}

		// Token: 0x06005995 RID: 22933 RVA: 0x001C63B5 File Offset: 0x001C45B5
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.DoArrayListLastIndex();
			}
			base.Finish();
		}

		// Token: 0x06005996 RID: 22934 RVA: 0x001C63E8 File Offset: 0x001C45E8
		public void DoArrayListLastIndex()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			object valueFromFsmVar = PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable);
			int num = -1;
			try
			{
				if (this.startIndex.Value == 0 && this.count.Value == 0)
				{
					num = PlayMakerUtils_Extensions.LastIndexOf(this.proxy.arrayList, valueFromFsmVar);
				}
				else if (this.count.Value == 0)
				{
					if (this.startIndex.Value < 0 || this.startIndex.Value >= this.proxy.arrayList.Count)
					{
						Debug.LogError("start index out of range");
						return;
					}
					num = PlayMakerUtils_Extensions.LastIndexOf(this.proxy.arrayList, valueFromFsmVar, this.startIndex.Value);
				}
				else
				{
					if (this.startIndex.Value < 0 || this.startIndex.Value >= this.proxy.arrayList.Count - this.count.Value)
					{
						Debug.LogError("start index and count out of range");
						return;
					}
					num = PlayMakerUtils_Extensions.LastIndexOf(this.proxy.arrayList, valueFromFsmVar, this.startIndex.Value, this.count.Value);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
				return;
			}
			this.lastIndexOf.Value = num;
			if (num == -1)
			{
				base.Fsm.Event(this.itemNotFound);
				return;
			}
			base.Fsm.Event(this.itemFound);
		}

		// Token: 0x0400550C RID: 21772
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400550D RID: 21773
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component (necessary if several component coexists on the same GameObject)")]
		[UIHint(UIHint.FsmString)]
		public FsmString reference;

		// Token: 0x0400550E RID: 21774
		[Tooltip("Optional start index to search from: set to 0 to ignore")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt startIndex;

		// Token: 0x0400550F RID: 21775
		[Tooltip("Optional amount of elements to search within: set to 0 to ignore")]
		[UIHint(UIHint.FsmInt)]
		public FsmInt count;

		// Token: 0x04005510 RID: 21776
		[ActionSection("Data")]
		[RequiredField]
		[Tooltip("The variable to get the index of.")]
		public FsmVar variable;

		// Token: 0x04005511 RID: 21777
		[ActionSection("Result")]
		[RequiredField]
		[Tooltip("The index of the last item described below")]
		public FsmInt lastIndexOf;

		// Token: 0x04005512 RID: 21778
		[Tooltip("Event sent if this arraList contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent itemFound;

		// Token: 0x04005513 RID: 21779
		[Tooltip("Event sent if this arraList does not contains that element ( described below)")]
		[UIHint(UIHint.FsmEvent)]
		public FsmEvent itemNotFound;

		// Token: 0x04005514 RID: 21780
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Optional Event to trigger if the action fails ( likely an out of range exception when using wrong values for index and/or count)")]
		public FsmEvent failureEvent;
	}
}
