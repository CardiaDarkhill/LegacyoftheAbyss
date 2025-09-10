using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F89 RID: 3977
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Select a random float from an array of floats.")]
	public class SelectRandomFloat : FsmStateAction
	{
		// Token: 0x06006E04 RID: 28164 RVA: 0x00221E84 File Offset: 0x00220084
		public override void Reset()
		{
			this.floats = new FsmFloat[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeFloat = null;
		}

		// Token: 0x06006E05 RID: 28165 RVA: 0x00221ED7 File Offset: 0x002200D7
		public override void OnEnter()
		{
			this.DoSelectRandomString();
			base.Finish();
		}

		// Token: 0x06006E06 RID: 28166 RVA: 0x00221EE8 File Offset: 0x002200E8
		private void DoSelectRandomString()
		{
			if (this.floats == null)
			{
				return;
			}
			if (this.floats.Length == 0)
			{
				return;
			}
			if (this.storeFloat == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeFloat.Value = this.floats[randomWeightedIndex].Value;
			}
		}

		// Token: 0x04006DB6 RID: 28086
		[CompoundArray("Floats", "Float", "Weight")]
		[Tooltip("A possible float choice.")]
		public FsmFloat[] floats;

		// Token: 0x04006DB7 RID: 28087
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this float being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x04006DB8 RID: 28088
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected float in a Float Variable.")]
		public FsmFloat storeFloat;
	}
}
