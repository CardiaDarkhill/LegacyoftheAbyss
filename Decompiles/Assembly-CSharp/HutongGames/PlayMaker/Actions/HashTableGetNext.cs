using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B47 RID: 2887
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Each time this action is called it gets the next item from a PlayMaker HashTable Proxy component. \nThis lets you quickly loop through all the children of an object to perform actions on them.\nNOTE: To get to specific item use HashTableGet instead.")]
	public class HashTableGetNext : HashTableActions
	{
		// Token: 0x06005A11 RID: 23057 RVA: 0x001C7E28 File Offset: 0x001C6028
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
			this.result = null;
		}

		// Token: 0x06005A12 RID: 23058 RVA: 0x001C7E74 File Offset: 0x001C6074
		public override void OnEnter()
		{
			if (this.reset.Value)
			{
				this.reset.Value = false;
				this.nextItemIndex = 0;
			}
			if (this.nextItemIndex == 0)
			{
				if (!base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
				{
					base.Fsm.Event(this.failureEvent);
					base.Finish();
				}
				this._keys = new ArrayList(this.proxy.hashTable.Keys);
				if (this.startIndex.Value > 0)
				{
					this.nextItemIndex = this.startIndex.Value;
				}
			}
			this.DoGetNextItem();
			base.Finish();
		}

		// Token: 0x06005A13 RID: 23059 RVA: 0x001C7F2C File Offset: 0x001C612C
		private void DoGetNextItem()
		{
			if (this.nextItemIndex >= this._keys.Count)
			{
				this.nextItemIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.GetItemAtIndex();
			if (this.nextItemIndex >= this._keys.Count)
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

		// Token: 0x06005A14 RID: 23060 RVA: 0x001C7FF8 File Offset: 0x001C61F8
		public void GetItemAtIndex()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			object value = null;
			try
			{
				value = this.proxy.hashTable[this._keys[this.nextItemIndex]];
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
				return;
			}
			this.key.Value = (string)this._keys[this.nextItemIndex];
			PlayMakerUtils.ApplyValueToFsmVar(base.Fsm, this.result, value);
		}

		// Token: 0x0400559A RID: 21914
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400559B RID: 21915
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x0400559C RID: 21916
		[Tooltip("Set to true to force iterating from the first item. This variable will be set to false as it carries on iterating, force it back to true if you want to renter this action back to the first item.")]
		[UIHint(UIHint.Variable)]
		public FsmBool reset;

		// Token: 0x0400559D RID: 21917
		[Tooltip("From where to start iteration, leave to 0 to start from the beginning")]
		public FsmInt startIndex;

		// Token: 0x0400559E RID: 21918
		[Tooltip("When to end iteration, leave to 0 to iterate until the end")]
		public FsmInt endIndex;

		// Token: 0x0400559F RID: 21919
		[Tooltip("Event to send to get the next item.")]
		public FsmEvent loopEvent;

		// Token: 0x040055A0 RID: 21920
		[Tooltip("Event to send when there are no more items.")]
		public FsmEvent finishedEvent;

		// Token: 0x040055A1 RID: 21921
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the action fails ( likely and index is out of range exception)")]
		public FsmEvent failureEvent;

		// Token: 0x040055A2 RID: 21922
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmString key;

		// Token: 0x040055A3 RID: 21923
		[UIHint(UIHint.Variable)]
		public FsmVar result;

		// Token: 0x040055A4 RID: 21924
		private ArrayList _keys;

		// Token: 0x040055A5 RID: 21925
		private int nextItemIndex;
	}
}
