using System;
using System.Linq;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0B RID: 3339
	public class SelectRandomAudioClipFair : FsmStateAction
	{
		// Token: 0x060062BE RID: 25278 RVA: 0x001F368E File Offset: 0x001F188E
		public bool IsNotUsingTrackingArray()
		{
			return this.TrackingArray.IsNone;
		}

		// Token: 0x060062BF RID: 25279 RVA: 0x001F369B File Offset: 0x001F189B
		public override void Reset()
		{
			this.Clips = null;
			this.TrackingArray = null;
			this.MissedMultiplier = 2f;
			this.StoreClip = null;
		}

		// Token: 0x060062C0 RID: 25280 RVA: 0x001F36C4 File Offset: 0x001F18C4
		public override void OnEnter()
		{
			float[] array = null;
			if (!this.TrackingArray.IsNone)
			{
				if (this.TrackingArray.floatValues.Length != this.Clips.Length)
				{
					array = (from e in this.Clips
					select e.Probability).ToArray<float>();
				}
				else
				{
					array = this.TrackingArray.floatValues;
				}
			}
			int num;
			AudioClip randomItemByProbability = Probability.GetRandomItemByProbability<SelectRandomAudioClipFair.ProbabilityAudioClip, AudioClip>(this.Clips, out num, array, null);
			if (!this.TrackingArray.IsNone)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (i == num)
					{
						array[i] = this.Clips[i].Probability;
					}
					else
					{
						array[i] *= this.MissedMultiplier.Value;
					}
				}
				this.TrackingArray.floatValues = array;
			}
			this.StoreClip.Value = randomItemByProbability;
			base.Finish();
		}

		// Token: 0x04006129 RID: 24873
		public SelectRandomAudioClipFair.ProbabilityAudioClip[] Clips;

		// Token: 0x0400612A RID: 24874
		[ArrayEditor(VariableType.Float, "", 0, 0, 65536)]
		[UIHint(UIHint.Variable)]
		public FsmArray TrackingArray;

		// Token: 0x0400612B RID: 24875
		[HideIf("IsNotUsingTrackingArray")]
		public FsmFloat MissedMultiplier;

		// Token: 0x0400612C RID: 24876
		[ObjectType(typeof(AudioClip))]
		[UIHint(UIHint.Variable)]
		public FsmObject StoreClip;

		// Token: 0x02001B89 RID: 7049
		[Serializable]
		public class ProbabilityAudioClip : Probability.ProbabilityBase<AudioClip>
		{
			// Token: 0x170011A6 RID: 4518
			// (get) Token: 0x06009A3A RID: 39482 RVA: 0x002B2F5F File Offset: 0x002B115F
			public override AudioClip Item
			{
				get
				{
					return this.Clip;
				}
			}

			// Token: 0x04009D84 RID: 40324
			public AudioClip Clip;
		}
	}
}
