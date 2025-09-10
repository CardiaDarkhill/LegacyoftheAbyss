using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B21 RID: 2849
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Gets an item from a PlayMaker ArrayList Proxy component")]
	public class ArrayListGet : ArrayListActions
	{
		// Token: 0x06005977 RID: 22903 RVA: 0x001C5954 File Offset: 0x001C3B54
		public override void Reset()
		{
			this.atIndex = null;
			this.gameObject = null;
			this.failureEvent = null;
			this.result = null;
		}

		// Token: 0x06005978 RID: 22904 RVA: 0x001C5972 File Offset: 0x001C3B72
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.GetItemAtIndex();
			}
			base.Finish();
		}

		// Token: 0x06005979 RID: 22905 RVA: 0x001C59A4 File Offset: 0x001C3BA4
		public void GetItemAtIndex()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (this.result.IsNone)
			{
				base.Fsm.Event(this.failureEvent);
				return;
			}
			object value = null;
			try
			{
				value = this.proxy.arrayList[this.atIndex.Value];
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				base.Fsm.Event(this.failureEvent);
				return;
			}
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.result, value);
		}

		// Token: 0x040054D8 RID: 21720
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054D9 RID: 21721
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040054DA RID: 21722
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The index to retrieve the item from")]
		public FsmInt atIndex;

		// Token: 0x040054DB RID: 21723
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVar result;

		// Token: 0x040054DC RID: 21724
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
		public FsmEvent failureEvent;
	}
}
