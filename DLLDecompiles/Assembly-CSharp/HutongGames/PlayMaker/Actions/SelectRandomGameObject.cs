using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000EBC RID: 3772
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Selects a Random Game Object from an array of Game Objects.")]
	public class SelectRandomGameObject : FsmStateAction
	{
		// Token: 0x06006AA7 RID: 27303 RVA: 0x002147FC File Offset: 0x002129FC
		public override void Reset()
		{
			this.gameObjects = new FsmGameObject[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeGameObject = null;
		}

		// Token: 0x06006AA8 RID: 27304 RVA: 0x0021484F File Offset: 0x00212A4F
		public override void OnEnter()
		{
			this.DoSelectRandomGameObject();
			base.Finish();
		}

		// Token: 0x06006AA9 RID: 27305 RVA: 0x00214860 File Offset: 0x00212A60
		private void DoSelectRandomGameObject()
		{
			if (this.gameObjects == null)
			{
				return;
			}
			if (this.gameObjects.Length == 0)
			{
				return;
			}
			if (this.storeGameObject == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeGameObject.Value = this.gameObjects[randomWeightedIndex].Value;
			}
		}

		// Token: 0x040069F0 RID: 27120
		[CompoundArray("Game Objects", "Game Object", "Weight")]
		[Tooltip("A possible GameObject choice.")]
		public FsmGameObject[] gameObjects;

		// Token: 0x040069F1 RID: 27121
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The relative probability of this GameObject being picked. E.g. a weight of 0.5 is half as likely to be picked as a weight of 1.")]
		public FsmFloat[] weights;

		// Token: 0x040069F2 RID: 27122
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the selected GameObject in a GameObject Variable.")]
		public FsmGameObject storeGameObject;
	}
}
