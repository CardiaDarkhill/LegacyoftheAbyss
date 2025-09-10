using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0C RID: 3340
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Selects a Random Game Object from an array of Game Objects.")]
	public class SelectRandomGameObjectV2 : FsmStateAction
	{
		// Token: 0x060062C2 RID: 25282 RVA: 0x001F37B4 File Offset: 0x001F19B4
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
			this.disallowedGameObject = null;
		}

		// Token: 0x060062C3 RID: 25283 RVA: 0x001F380E File Offset: 0x001F1A0E
		public override void OnEnter()
		{
			this.DoSelectRandomGameObject();
			base.Finish();
		}

		// Token: 0x060062C4 RID: 25284 RVA: 0x001F381C File Offset: 0x001F1A1C
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
				GameObject value = this.gameObjects[randomWeightedIndex].Value;
				if (!this.disallowedGameObject.IsNone && this.disallowedGameObject.Value != null)
				{
					int num = 0;
					while (value == this.disallowedGameObject.Value && num < 100)
					{
						randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
						value = this.gameObjects[randomWeightedIndex].Value;
					}
				}
				this.storeGameObject.Value = value;
			}
		}

		// Token: 0x0400612D RID: 24877
		[CompoundArray("Game Objects", "Game Object", "Weight")]
		public FsmGameObject[] gameObjects;

		// Token: 0x0400612E RID: 24878
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x0400612F RID: 24879
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject storeGameObject;

		// Token: 0x04006130 RID: 24880
		[UIHint(UIHint.Variable)]
		public FsmGameObject disallowedGameObject;
	}
}
