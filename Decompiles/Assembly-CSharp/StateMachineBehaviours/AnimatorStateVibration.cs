using System;
using UnityEngine;

namespace StateMachineBehaviours
{
	// Token: 0x02000835 RID: 2101
	public sealed class AnimatorStateVibration : StateMachineBehaviour
	{
		// Token: 0x06004ACB RID: 19147 RVA: 0x0016284C File Offset: 0x00160A4C
		private void OnDisable()
		{
			this.StopVibration();
		}

		// Token: 0x06004ACC RID: 19148 RVA: 0x00162854 File Offset: 0x00160A54
		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			this.PlayVibration();
		}

		// Token: 0x06004ACD RID: 19149 RVA: 0x0016285C File Offset: 0x00160A5C
		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.stopOnExit || this.loop)
			{
				this.StopVibration();
			}
		}

		// Token: 0x06004ACE RID: 19150 RVA: 0x00162874 File Offset: 0x00160A74
		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (this.stopOnAnimFinished && !stateInfo.loop && (stateInfo.normalizedTime >= 1f || stateInfo.normalizedTime <= -1f))
			{
				this.StopVibration();
			}
		}

		// Token: 0x06004ACF RID: 19151 RVA: 0x001628AC File Offset: 0x00160AAC
		public void PlayVibration()
		{
			if (!this.loop || this.emission == null)
			{
				VibrationData vibrationData = this.vibrationDataAsset;
				bool isLooping = this.loop;
				bool isRealtime = this.isRealTime;
				string text = this.tag;
				this.emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, null, isLooping, text, isRealtime);
				VibrationEmission vibrationEmission = this.emission;
				if (vibrationEmission == null)
				{
					return;
				}
				vibrationEmission.SetStrength(this.strength);
			}
		}

		// Token: 0x06004AD0 RID: 19152 RVA: 0x00162916 File Offset: 0x00160B16
		private void StopVibration()
		{
			VibrationEmission vibrationEmission = this.emission;
			if (vibrationEmission != null)
			{
				vibrationEmission.Stop();
			}
			this.emission = null;
		}

		// Token: 0x04004A92 RID: 19090
		[SerializeField]
		private VibrationDataAsset vibrationDataAsset;

		// Token: 0x04004A93 RID: 19091
		[SerializeField]
		private float strength = 1f;

		// Token: 0x04004A94 RID: 19092
		[SerializeField]
		private bool loop;

		// Token: 0x04004A95 RID: 19093
		[SerializeField]
		private bool isRealTime;

		// Token: 0x04004A96 RID: 19094
		[SerializeField]
		private string tag;

		// Token: 0x04004A97 RID: 19095
		[Space]
		[SerializeField]
		private bool stopOnExit;

		// Token: 0x04004A98 RID: 19096
		[SerializeField]
		private bool stopOnAnimFinished;

		// Token: 0x04004A99 RID: 19097
		private VibrationEmission emission;
	}
}
