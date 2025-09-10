using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B22 RID: 2850
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Each time this action is called it gets the next item from a PlayMaker ArrayList Proxy component. \nThis lets you quickly loop through all the children of an object to perform actions on them.\nNOTE: To get to specific item use ArrayListGet instead.")]
	public class ArrayListGetNext : ArrayListActions
	{
		// Token: 0x0600597B RID: 22907 RVA: 0x001C5A44 File Offset: 0x001C3C44
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.startIndex = null;
			this.endIndex = null;
			this.reset = null;
			this.loopEvent = null;
			this.finishedEvent = null;
			this.failureEvent = null;
			this.result = null;
			this.currentIndex = null;
		}

		// Token: 0x0600597C RID: 22908 RVA: 0x001C5A98 File Offset: 0x001C3C98
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
				if (this.startIndex.Value > 0)
				{
					this.nextItemIndex = this.startIndex.Value;
				}
			}
			this.DoGetNextItem();
			base.Finish();
		}

		// Token: 0x0600597D RID: 22909 RVA: 0x001C5B34 File Offset: 0x001C3D34
		private void DoGetNextItem()
		{
			if (this.nextItemIndex >= this.proxy.arrayList.Count)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.GetItemAtIndex();
			if (this.nextItemIndex >= this.proxy.arrayList.Count)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			if (this.endIndex.Value > 0 && this.nextItemIndex >= this.endIndex.Value)
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

		// Token: 0x0600597E RID: 22910 RVA: 0x001C5C0C File Offset: 0x001C3E0C
		public void GetItemAtIndex()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			if (this.result.IsNone)
			{
				return;
			}
			object value = null;
			this.currentIndex.Value = this.nextItemIndex;
			try
			{
				value = this.proxy.arrayList[this.nextItemIndex];
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
				return;
			}
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.result, value);
		}

		// Token: 0x040054DD RID: 21725
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054DE RID: 21726
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x040054DF RID: 21727
		[Tooltip("Set to true to force iterating from the first item. This variable will be set to false as it carries on iterating, force it back to true if you want to renter this action back to the first item.")]
		[UIHint(UIHint.Variable)]
		public FsmBool reset;

		// Token: 0x040054E0 RID: 21728
		[Tooltip("From where to start iteration, leave to 0 to start from the beginning")]
		public FsmInt startIndex;

		// Token: 0x040054E1 RID: 21729
		[Tooltip("When to end iteration, leave to 0 to iterate until the end")]
		public FsmInt endIndex;

		// Token: 0x040054E2 RID: 21730
		[Tooltip("Event to send to get the next item.")]
		public FsmEvent loopEvent;

		// Token: 0x040054E3 RID: 21731
		[Tooltip("Event to send when there are no more items.")]
		public FsmEvent finishedEvent;

		// Token: 0x040054E4 RID: 21732
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
		public FsmEvent failureEvent;

		// Token: 0x040054E5 RID: 21733
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The current index.")]
		public FsmInt currentIndex;

		// Token: 0x040054E6 RID: 21734
		[UIHint(UIHint.Variable)]
		[Tooltip("The value for the current index.")]
		public FsmVar result;

		// Token: 0x040054E7 RID: 21735
		private int nextItemIndex;
	}
}
