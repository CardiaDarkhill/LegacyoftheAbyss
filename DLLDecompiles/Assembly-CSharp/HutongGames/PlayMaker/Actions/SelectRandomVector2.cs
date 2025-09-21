using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001184 RID: 4484
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Select a Random Vector2 from a Vector2 array.")]
	public class SelectRandomVector2 : FsmStateAction
	{
		// Token: 0x06007837 RID: 30775 RVA: 0x002472A8 File Offset: 0x002454A8
		public override void Reset()
		{
			this.vector2Array = new FsmVector2[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeVector2 = null;
		}

		// Token: 0x06007838 RID: 30776 RVA: 0x002472FB File Offset: 0x002454FB
		public override void OnEnter()
		{
			this.DoSelectRandom();
			base.Finish();
		}

		// Token: 0x06007839 RID: 30777 RVA: 0x0024730C File Offset: 0x0024550C
		private void DoSelectRandom()
		{
			if (this.vector2Array == null)
			{
				return;
			}
			if (this.vector2Array.Length == 0)
			{
				return;
			}
			if (this.storeVector2 == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeVector2.Value = this.vector2Array[randomWeightedIndex].Value;
			}
		}

		// Token: 0x040078AC RID: 30892
		[CompoundArray("Vectors", "Vector", "Weight")]
		[Tooltip("A possible Vector2 choice.")]
		public FsmVector2[] vector2Array;

		// Token: 0x040078AD RID: 30893
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this Vector2 being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x040078AE RID: 30894
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected Vector2 in a Vector2 Variable.")]
		public FsmVector2 storeVector2;
	}
}
