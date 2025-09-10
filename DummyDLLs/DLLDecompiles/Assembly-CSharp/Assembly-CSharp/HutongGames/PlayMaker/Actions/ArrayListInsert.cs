using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B26 RID: 2854
	[ActionCategory("ArrayMaker/ArrayList")]
	[Tooltip("Insert item at a specified index of a PlayMaker ArrayList Proxy component")]
	public class ArrayListInsert : ArrayListActions
	{
		// Token: 0x0600598D RID: 22925 RVA: 0x001C6200 File Offset: 0x001C4400
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.variable = null;
			this.failureEvent = null;
			this.index = null;
		}

		// Token: 0x0600598E RID: 22926 RVA: 0x001C6225 File Offset: 0x001C4425
		public override void OnEnter()
		{
			if (base.SetUpArrayListProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doArrayListInsert();
			}
			base.Finish();
		}

		// Token: 0x0600598F RID: 22927 RVA: 0x001C6258 File Offset: 0x001C4458
		public void doArrayListInsert()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			try
			{
				this.proxy.arrayList.Insert(this.index.Value, PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable));
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				base.Fsm.Event(this.failureEvent);
			}
		}

		// Token: 0x04005502 RID: 21762
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker ArrayList Proxy component")]
		[CheckForComponent(typeof(PlayMakerArrayListProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005503 RID: 21763
		[Tooltip("Author defined Reference of the PlayMaker ArrayList Proxy component ( necessary if several component coexists on the same GameObject")]
		public FsmString reference;

		// Token: 0x04005504 RID: 21764
		[UIHint(UIHint.FsmInt)]
		[Tooltip("The index to remove at")]
		public FsmInt index;

		// Token: 0x04005505 RID: 21765
		[ActionSection("Data")]
		[RequiredField]
		[Tooltip("The variable to add.")]
		public FsmVar variable;

		// Token: 0x04005506 RID: 21766
		[ActionSection("Result")]
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger if the removeAt throw errors")]
		public FsmEvent failureEvent;
	}
}
