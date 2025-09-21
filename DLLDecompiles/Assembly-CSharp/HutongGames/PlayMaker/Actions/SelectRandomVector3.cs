using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200119B RID: 4507
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Select a Random Vector3 from a Vector3 array.")]
	public class SelectRandomVector3 : FsmStateAction
	{
		// Token: 0x0600789D RID: 30877 RVA: 0x00248418 File Offset: 0x00246618
		public override void Reset()
		{
			this.vector3Array = new FsmVector3[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeVector3 = null;
		}

		// Token: 0x0600789E RID: 30878 RVA: 0x0024846B File Offset: 0x0024666B
		public override void OnEnter()
		{
			this.DoSelectRandomColor();
			base.Finish();
		}

		// Token: 0x0600789F RID: 30879 RVA: 0x0024847C File Offset: 0x0024667C
		private void DoSelectRandomColor()
		{
			if (this.vector3Array == null)
			{
				return;
			}
			if (this.vector3Array.Length == 0)
			{
				return;
			}
			if (this.storeVector3 == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeVector3.Value = this.vector3Array[randomWeightedIndex].Value;
			}
		}

		// Token: 0x04007904 RID: 30980
		[CompoundArray("Vectors", "Vector", "Weight")]
		[Tooltip("A possible Vector3 choice.")]
		public FsmVector3[] vector3Array;

		// Token: 0x04007905 RID: 30981
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this Vector3 being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x04007906 RID: 30982
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected Vector3 in a Vector3 Variable.")]
		public FsmVector3 storeVector3;
	}
}
