using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F87 RID: 3975
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets an Integer Variable to a random value between Min/Max.")]
	public class RandomInt : FsmStateAction
	{
		// Token: 0x06006DFB RID: 28155 RVA: 0x00221C69 File Offset: 0x0021FE69
		public override void Reset()
		{
			this.min = 0;
			this.max = 100;
			this.storeResult = null;
			this.inclusiveMax = false;
			this.noRepeat = true;
		}

		// Token: 0x06006DFC RID: 28156 RVA: 0x00221C9E File Offset: 0x0021FE9E
		public override void OnEnter()
		{
			this.PickRandom();
			base.Finish();
		}

		// Token: 0x06006DFD RID: 28157 RVA: 0x00221CAC File Offset: 0x0021FEAC
		private void PickRandom()
		{
			if (this.noRepeat.Value && this.max.Value != this.min.Value && !this.inclusiveMax && Mathf.Abs(this.max.Value - this.min.Value) > 1)
			{
				do
				{
					this.randomIndex = (this.inclusiveMax ? Random.Range(this.min.Value, this.max.Value + 1) : Random.Range(this.min.Value, this.max.Value));
				}
				while (this.randomIndex == this.lastIndex);
				this.lastIndex = this.randomIndex;
				this.storeResult.Value = this.randomIndex;
				return;
			}
			this.randomIndex = (this.inclusiveMax ? Random.Range(this.min.Value, this.max.Value + 1) : Random.Range(this.min.Value, this.max.Value));
			this.storeResult.Value = this.randomIndex;
		}

		// Token: 0x04006DAB RID: 28075
		[RequiredField]
		[Tooltip("Minimum value for the random number.")]
		public FsmInt min;

		// Token: 0x04006DAC RID: 28076
		[RequiredField]
		[Tooltip("Maximum value for the random number.")]
		public FsmInt max;

		// Token: 0x04006DAD RID: 28077
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in an Integer variable.")]
		public FsmInt storeResult;

		// Token: 0x04006DAE RID: 28078
		[Tooltip("Should the Max value be included in the possible results?")]
		public bool inclusiveMax;

		// Token: 0x04006DAF RID: 28079
		[Tooltip("Don't repeat the same value twice.")]
		public FsmBool noRepeat;

		// Token: 0x04006DB0 RID: 28080
		private int randomIndex;

		// Token: 0x04006DB1 RID: 28081
		private int lastIndex = -1;
	}
}
