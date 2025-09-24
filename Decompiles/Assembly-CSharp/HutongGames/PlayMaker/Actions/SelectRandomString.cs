using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010C6 RID: 4294
	[ActionCategory(ActionCategory.String)]
	[Tooltip("Select a Random String from an array of Strings.")]
	public class SelectRandomString : FsmStateAction
	{
		// Token: 0x06007466 RID: 29798 RVA: 0x0023A4B4 File Offset: 0x002386B4
		public override void Reset()
		{
			this.strings = new FsmString[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeString = null;
		}

		// Token: 0x06007467 RID: 29799 RVA: 0x0023A507 File Offset: 0x00238707
		public override void OnEnter()
		{
			this.DoSelectRandomString();
			base.Finish();
		}

		// Token: 0x06007468 RID: 29800 RVA: 0x0023A518 File Offset: 0x00238718
		private void DoSelectRandomString()
		{
			if (this.strings == null)
			{
				return;
			}
			if (this.strings.Length == 0)
			{
				return;
			}
			if (this.storeString == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeString.Value = this.strings[randomWeightedIndex].Value;
			}
		}

		// Token: 0x040074A5 RID: 29861
		[CompoundArray("Strings", "String", "Weight")]
		[Tooltip("A possible String choice.")]
		public FsmString[] strings;

		// Token: 0x040074A6 RID: 29862
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this string being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x040074A7 RID: 29863
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the chosen String in a String Variable.")]
		public FsmString storeString;
	}
}
