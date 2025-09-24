using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F50 RID: 3920
	[ActionCategory(ActionCategory.Logic)]
	[Tooltip("Tests if a Game Object has a tag.")]
	public class GameObjectCompareTag : FsmStateAction
	{
		// Token: 0x06006CF9 RID: 27897 RVA: 0x0021F120 File Offset: 0x0021D320
		public override void Reset()
		{
			this.gameObject = null;
			this.tag = "Untagged";
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CFA RID: 27898 RVA: 0x0021F155 File Offset: 0x0021D355
		public override void OnEnter()
		{
			this.DoCompareTag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CFB RID: 27899 RVA: 0x0021F16B File Offset: 0x0021D36B
		public override void OnUpdate()
		{
			this.DoCompareTag();
		}

		// Token: 0x06006CFC RID: 27900 RVA: 0x0021F174 File Offset: 0x0021D374
		private void DoCompareTag()
		{
			bool flag = false;
			if (this.gameObject.Value != null)
			{
				flag = this.gameObject.Value.CompareTag(this.tag.Value);
			}
			this.storeResult.Value = flag;
			base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
		}

		// Token: 0x04006CBA RID: 27834
		[RequiredField]
		[Tooltip("The GameObject to test.")]
		public FsmGameObject gameObject;

		// Token: 0x04006CBB RID: 27835
		[RequiredField]
		[UIHint(UIHint.Tag)]
		[Tooltip("The Tag to check for.")]
		public FsmString tag;

		// Token: 0x04006CBC RID: 27836
		[Tooltip("Event to send if the GameObject has the Tag.")]
		public FsmEvent trueEvent;

		// Token: 0x04006CBD RID: 27837
		[Tooltip("Event to send if the GameObject does not have the Tag.")]
		public FsmEvent falseEvent;

		// Token: 0x04006CBE RID: 27838
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04006CBF RID: 27839
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
