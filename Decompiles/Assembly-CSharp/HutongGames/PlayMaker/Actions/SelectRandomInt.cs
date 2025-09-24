using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F8A RID: 3978
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Select a random Int from an array of Ints.")]
	public class SelectRandomInt : FsmStateAction
	{
		// Token: 0x06006E08 RID: 28168 RVA: 0x00221F44 File Offset: 0x00220144
		public override void Reset()
		{
			this.ints = new FsmInt[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeInt = null;
		}

		// Token: 0x06006E09 RID: 28169 RVA: 0x00221F97 File Offset: 0x00220197
		public override void OnEnter()
		{
			this.DoSelectRandomString();
			base.Finish();
		}

		// Token: 0x06006E0A RID: 28170 RVA: 0x00221FA8 File Offset: 0x002201A8
		private void DoSelectRandomString()
		{
			if (this.ints == null)
			{
				return;
			}
			if (this.ints.Length == 0)
			{
				return;
			}
			if (this.storeInt == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeInt.Value = this.ints[randomWeightedIndex].Value;
			}
		}

		// Token: 0x04006DB9 RID: 28089
		[CompoundArray("Ints", "Int", "Weight")]
		[Tooltip("A possible int choice.")]
		public FsmInt[] ints;

		// Token: 0x04006DBA RID: 28090
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this int being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x04006DBB RID: 28091
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected int in an Int Variable.")]
		public FsmInt storeInt;
	}
}
