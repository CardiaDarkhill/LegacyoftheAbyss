using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E61 RID: 3681
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Select a random Color from an array of Colors.")]
	public class SelectRandomColor : FsmStateAction
	{
		// Token: 0x06006913 RID: 26899 RVA: 0x0020FC54 File Offset: 0x0020DE54
		public override void Reset()
		{
			this.colors = new FsmColor[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeColor = null;
		}

		// Token: 0x06006914 RID: 26900 RVA: 0x0020FCA7 File Offset: 0x0020DEA7
		public override void OnEnter()
		{
			this.DoSelectRandomColor();
			base.Finish();
		}

		// Token: 0x06006915 RID: 26901 RVA: 0x0020FCB8 File Offset: 0x0020DEB8
		private void DoSelectRandomColor()
		{
			if (this.colors == null)
			{
				return;
			}
			if (this.colors.Length == 0)
			{
				return;
			}
			if (this.storeColor == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeColor.Value = this.colors[randomWeightedIndex].Value;
			}
		}

		// Token: 0x04006863 RID: 26723
		[CompoundArray("Colors", "Color", "Weight")]
		[Tooltip("A possible Color choice.")]
		public FsmColor[] colors;

		// Token: 0x04006864 RID: 26724
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this color being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x04006865 RID: 26725
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected Color in a Color Variable.")]
		public FsmColor storeColor;
	}
}
