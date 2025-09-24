using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B23 RID: 2851
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Each time this action is called it gets the previous item from a PlayMaker ArrayList Proxy component. \nThis lets you quickly loop backward through all the children of an object to perform actions on them.\nNOTE: To get to specific item use ArrayListGet instead.")]
	public class ArrayListGetPrevious : ArrayListActions
	{
		// Token: 0x06005980 RID: 22912 RVA: 0x001C5CA4 File Offset: 0x001C3EA4
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.reset = null;
			this.startIndex = null;
			this.endIndex = null;
			this.loopEvent = null;
			this.finishedEvent = null;
			this.failureEvent = null;
			this.currentIndex = null;
			this.result = null;
		}

		// Token: 0x06005981 RID: 22913 RVA: 0x001C5CF8 File Offset: 0x001C3EF8
		public override void OnEnter()
		{
			if (this.reset.Value)
			{
				this.reset.Value = false;
				this.nextItemIndex = 0;
			}
			if (this.nextItemIndex == 0)
			{
				if (!base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
				{
					base.Fsm.Event(this.failureEvent);
					base.Finish();
				}
				this.countBase = this.proxy.arrayList.Count - 1;
				if (this.startIndex.Value > 0)
				{
					this.nextItemIndex = this.startIndex.Value;
				}
			}
			this.DoGetPreviousItem();
			base.Finish();
		}

		// Token: 0x06005982 RID: 22914 RVA: 0x001C5DAC File Offset: 0x001C3FAC
		private void DoGetPreviousItem()
		{
			if (this.nextItemIndex > this.countBase)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.GetItemAtIndex();
			if (this.nextItemIndex >= this.countBase)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			if (this.endIndex.Value > 0 && this.nextItemIndex > this.countBase - this.endIndex.Value)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.nextItemIndex++;
			if (this.loopEvent != null)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		// Token: 0x06005983 RID: 22915 RVA: 0x001C5E78 File Offset: 0x001C4078
		public void GetItemAtIndex()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.currentIndex.Value = this.countBase - this.nextItemIndex;
			object value = null;
			try
			{
				value = this.proxy.arrayList[this.currentIndex.Value];
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
				return;
			}
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.result, value);
		}

		// Token: 0x040054E8 RID: 21736
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054E9 RID: 21737
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040054EA RID: 21738
		[Tooltip("Set to true to force iterating from the last item. This variable will be set to false as it carries on iterating, force it back to true if you want to renter this action back to the last item.")]
		[UIHint(UIHint.Variable)]
		public FsmBool reset;

		// Token: 0x040054EB RID: 21739
		[Tooltip("From where to start iteration, leave to 0 to start from the end. index is relative to the last item, so if the start index is 2, this will start 2 items before the last item.")]
		public FsmInt startIndex;

		// Token: 0x040054EC RID: 21740
		[Tooltip("When to end iteration, leave to 0 to iterate until the beginning, index is relative to the last item.")]
		public FsmInt endIndex;

		// Token: 0x040054ED RID: 21741
		[Tooltip("Event to send to get the previous item.")]
		public FsmEvent loopEvent;

		// Token: 0x040054EE RID: 21742
		[Tooltip("Event to send when there are no more items.")]
		public FsmEvent finishedEvent;

		// Token: 0x040054EF RID: 21743
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
		public FsmEvent failureEvent;

		// Token: 0x040054F0 RID: 21744
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmInt currentIndex;

		// Token: 0x040054F1 RID: 21745
		[UIHint(UIHint.Variable)]
		public FsmVar result;

		// Token: 0x040054F2 RID: 21746
		private int nextItemIndex;

		// Token: 0x040054F3 RID: 21747
		private int countBase;
	}
}
