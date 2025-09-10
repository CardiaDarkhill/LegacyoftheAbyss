using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E1F RID: 3615
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Add multiple items to the end of an array.\nNOTE: There is a bug in this action when resizing Variables. It will be fixed in the next update.")]
	public class ArrayAddRange : FsmStateAction
	{
		// Token: 0x060067E7 RID: 26599 RVA: 0x0020B463 File Offset: 0x00209663
		public override void Reset()
		{
			this.array = null;
			this.variables = new FsmVar[2];
		}

		// Token: 0x060067E8 RID: 26600 RVA: 0x0020B478 File Offset: 0x00209678
		public override void OnEnter()
		{
			this.DoAddRange();
			base.Finish();
		}

		// Token: 0x060067E9 RID: 26601 RVA: 0x0020B488 File Offset: 0x00209688
		private void DoAddRange()
		{
			int num = this.variables.Length;
			if (num > 0)
			{
				this.array.Resize(this.array.Length + num);
				foreach (FsmVar fsmVar in this.variables)
				{
					fsmVar.UpdateValue();
					this.array.Set(this.array.Length - num, fsmVar.GetValue());
					num--;
				}
			}
		}

		// Token: 0x04006719 RID: 26393
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to use.")]
		public FsmArray array;

		// Token: 0x0400671A RID: 26394
		[RequiredField]
		[MatchElementType("array")]
		[Tooltip("The items to add to the array.")]
		public FsmVar[] variables;
	}
}
