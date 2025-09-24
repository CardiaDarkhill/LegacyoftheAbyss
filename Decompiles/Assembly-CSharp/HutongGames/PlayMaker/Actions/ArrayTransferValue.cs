using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E31 RID: 3633
	[NoActionTargets]
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Transfer a value from one array to another, basically a copy/cut paste action on steroids.")]
	public class ArrayTransferValue : FsmStateAction
	{
		// Token: 0x06006834 RID: 26676 RVA: 0x0020C277 File Offset: 0x0020A477
		public override void Reset()
		{
			this.arraySource = null;
			this.arrayTarget = null;
			this.indexToTransfer = null;
			this.copyType = ArrayTransferValue.ArrayTransferType.Copy;
			this.pasteType = ArrayTransferValue.ArrayPasteType.AsLastItem;
		}

		// Token: 0x06006835 RID: 26677 RVA: 0x0020C2B0 File Offset: 0x0020A4B0
		public override void OnEnter()
		{
			this.DoTransferValue();
			base.Finish();
		}

		// Token: 0x06006836 RID: 26678 RVA: 0x0020C2C0 File Offset: 0x0020A4C0
		private void DoTransferValue()
		{
			if (this.arraySource.IsNone || this.arrayTarget.IsNone)
			{
				return;
			}
			int value = this.indexToTransfer.Value;
			if (value < 0 || value >= this.arraySource.Length)
			{
				base.Fsm.Event(this.indexOutOfRange);
				return;
			}
			object obj = this.arraySource.Values[value];
			if ((ArrayTransferValue.ArrayTransferType)this.copyType.Value == ArrayTransferValue.ArrayTransferType.Cut)
			{
				List<object> list = new List<object>(this.arraySource.Values);
				list.RemoveAt(value);
				this.arraySource.Values = list.ToArray();
			}
			else if ((ArrayTransferValue.ArrayTransferType)this.copyType.Value == ArrayTransferValue.ArrayTransferType.nullify)
			{
				this.arraySource.Values.SetValue(null, value);
			}
			if ((ArrayTransferValue.ArrayPasteType)this.pasteType.Value == ArrayTransferValue.ArrayPasteType.AsFirstItem)
			{
				List<object> list2 = new List<object>(this.arrayTarget.Values);
				list2.Insert(0, obj);
				this.arrayTarget.Values = list2.ToArray();
				return;
			}
			if ((ArrayTransferValue.ArrayPasteType)this.pasteType.Value == ArrayTransferValue.ArrayPasteType.AsLastItem)
			{
				this.arrayTarget.Resize(this.arrayTarget.Length + 1);
				this.arrayTarget.Set(this.arrayTarget.Length - 1, obj);
				return;
			}
			if ((ArrayTransferValue.ArrayPasteType)this.pasteType.Value == ArrayTransferValue.ArrayPasteType.InsertAtSameIndex)
			{
				if (value >= this.arrayTarget.Length)
				{
					base.Fsm.Event(this.indexOutOfRange);
				}
				List<object> list3 = new List<object>(this.arrayTarget.Values);
				list3.Insert(value, obj);
				this.arrayTarget.Values = list3.ToArray();
				return;
			}
			if ((ArrayTransferValue.ArrayPasteType)this.pasteType.Value == ArrayTransferValue.ArrayPasteType.ReplaceAtSameIndex)
			{
				if (value >= this.arrayTarget.Length)
				{
					base.Fsm.Event(this.indexOutOfRange);
					return;
				}
				this.arrayTarget.Set(value, obj);
			}
		}

		// Token: 0x0400675D RID: 26461
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable source.")]
		public FsmArray arraySource;

		// Token: 0x0400675E RID: 26462
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable target.")]
		public FsmArray arrayTarget;

		// Token: 0x0400675F RID: 26463
		[MatchFieldType("array")]
		[Tooltip("The index to transfer.")]
		public FsmInt indexToTransfer;

		// Token: 0x04006760 RID: 26464
		[ActionSection("Transfer Options")]
		[ObjectType(typeof(ArrayTransferValue.ArrayTransferType))]
		[Tooltip("Copy Options.")]
		public FsmEnum copyType;

		// Token: 0x04006761 RID: 26465
		[ObjectType(typeof(ArrayTransferValue.ArrayPasteType))]
		[Tooltip("Paste Options")]
		public FsmEnum pasteType;

		// Token: 0x04006762 RID: 26466
		[ActionSection("Result")]
		[Tooltip("Event sent if the array source does not contains that element.")]
		public FsmEvent indexOutOfRange;

		// Token: 0x02001B9D RID: 7069
		public enum ArrayTransferType
		{
			// Token: 0x04009DE6 RID: 40422
			Copy,
			// Token: 0x04009DE7 RID: 40423
			Cut,
			// Token: 0x04009DE8 RID: 40424
			nullify
		}

		// Token: 0x02001B9E RID: 7070
		public enum ArrayPasteType
		{
			// Token: 0x04009DEA RID: 40426
			AsFirstItem,
			// Token: 0x04009DEB RID: 40427
			AsLastItem,
			// Token: 0x04009DEC RID: 40428
			InsertAtSameIndex,
			// Token: 0x04009DED RID: 40429
			ReplaceAtSameIndex
		}
	}
}
