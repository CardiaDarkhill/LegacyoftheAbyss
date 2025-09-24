using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E20 RID: 3616
	[ActionCategory(ActionCategory.Array)]
	[Tooltip("Sets all items in an Array to their default value: 0, empty string, false, or null depending on their type. Optionally defines a reset value to use.")]
	public class ArrayClear : FsmStateAction
	{
		// Token: 0x060067EB RID: 26603 RVA: 0x0020B503 File Offset: 0x00209703
		public override void Reset()
		{
			this.array = null;
			this.resetValue = new FsmVar
			{
				useVariable = true
			};
		}

		// Token: 0x060067EC RID: 26604 RVA: 0x0020B520 File Offset: 0x00209720
		public override void OnEnter()
		{
			int length = this.array.Length;
			this.array.Reset();
			this.array.Resize(length);
			if (!this.resetValue.IsNone)
			{
				this.resetValue.UpdateValue();
				object value = this.resetValue.GetValue();
				for (int i = 0; i < length; i++)
				{
					this.array.Set(i, value);
				}
			}
			base.Finish();
		}

		// Token: 0x0400671B RID: 26395
		[UIHint(UIHint.Variable)]
		[Tooltip("The Array Variable to clear.")]
		public FsmArray array;

		// Token: 0x0400671C RID: 26396
		[MatchElementType("array")]
		[Tooltip("Optional reset value. Leave as None for default value.")]
		public FsmVar resetValue;
	}
}
