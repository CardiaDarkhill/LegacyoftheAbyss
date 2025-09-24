using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B40 RID: 2880
	[ActionCategory("ArrayMaker/HashTable")]
	[Tooltip("Check if a value exists in a PlayMaker HashTable Proxy component (PlayMakerHashTablePRoxy)")]
	public class HashTableContainsValue : HashTableActions
	{
		// Token: 0x060059F6 RID: 23030 RVA: 0x001C7850 File Offset: 0x001C5A50
		public override void Reset()
		{
			this.gameObject = null;
			this.reference = null;
			this.containsValue = null;
			this.valueFoundEvent = null;
			this.valueNotFoundEvent = null;
			this.variable = null;
		}

		// Token: 0x060059F7 RID: 23031 RVA: 0x001C787C File Offset: 0x001C5A7C
		public override void OnEnter()
		{
			if (base.SetUpHashTableProxyPointer(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.reference.Value))
			{
				this.doContainsValue();
			}
			base.Finish();
		}

		// Token: 0x060059F8 RID: 23032 RVA: 0x001C78B0 File Offset: 0x001C5AB0
		public void doContainsValue()
		{
			if (!base.isProxyValid())
			{
				return;
			}
			this.containsValue.Value = this.proxy.hashTable.ContainsValue(PlayMakerUtils.GetValueFromFsmVar(base.Fsm, this.variable));
			if (this.containsValue.Value)
			{
				base.Fsm.Event(this.valueFoundEvent);
				return;
			}
			base.Fsm.Event(this.valueNotFoundEvent);
		}

		// Token: 0x04005576 RID: 21878
		[ActionSection("Set up")]
		[RequiredField]
		[Tooltip("The gameObject with the PlayMaker HashTable Proxy component")]
		[CheckForComponent(typeof(PlayMakerHashTableProxy))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005577 RID: 21879
		[Tooltip("Author defined Reference of the PlayMaker HashTable Proxy component (necessary if several component coexists on the same GameObject)")]
		public FsmString reference;

		// Token: 0x04005578 RID: 21880
		[Tooltip("The variable to check for.")]
		public FsmVar variable;

		// Token: 0x04005579 RID: 21881
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result of the test")]
		public FsmBool containsValue;

		// Token: 0x0400557A RID: 21882
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when value is found")]
		public FsmEvent valueFoundEvent;

		// Token: 0x0400557B RID: 21883
		[UIHint(UIHint.FsmEvent)]
		[Tooltip("The event to trigger when value is not found")]
		public FsmEvent valueNotFoundEvent;
	}
}
