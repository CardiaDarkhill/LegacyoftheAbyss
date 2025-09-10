using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A0 RID: 5024
	[ActionCategory("Hollow Knight")]
	public class PlayVibrationV4 : FsmStateAction
	{
		// Token: 0x060080E2 RID: 32994 RVA: 0x0025F864 File Offset: 0x0025DA64
		public override void Reset()
		{
			base.Reset();
			this.vibrationDataAsset = null;
			this.strength = 1f;
			this.motors = new FsmEnum
			{
				UseVariable = true,
				Value = VibrationMotors.All
			};
			this.loop = null;
			this.tag = null;
			this.stopOnStateExit = null;
		}

		// Token: 0x060080E3 RID: 32995 RVA: 0x0025F8C4 File Offset: 0x0025DAC4
		public override void OnEnter()
		{
			base.OnEnter();
			bool value = this.loop.Value;
			this.Play(value);
			if (value || !this.waitUntilFinished.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x060080E4 RID: 32996 RVA: 0x0025F900 File Offset: 0x0025DB00
		private void Play(bool loop)
		{
			if (ObjectPool.IsCreatingPool)
			{
				return;
			}
			VibrationData vibrationData = (VibrationDataAsset)this.vibrationDataAsset.Value;
			vibrationData = (VibrationDataAsset)this.vibrationDataAsset.Value;
			VibrationMotors vibrationMotors = VibrationMotors.All;
			if (!this.motors.IsNone)
			{
				vibrationMotors = (VibrationMotors)this.motors.Value;
				if (vibrationMotors == VibrationMotors.None && vibrationData.GamepadVibration != null)
				{
					vibrationMotors = VibrationMotors.All;
				}
			}
			this.emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, new VibrationTarget?(new VibrationTarget(vibrationMotors)), loop, this.tag.Value ?? "", false);
			if (this.emission != null)
			{
				this.emission.SetStrength(this.strength.Value);
			}
		}

		// Token: 0x060080E5 RID: 32997 RVA: 0x0025F9C0 File Offset: 0x0025DBC0
		public override void OnUpdate()
		{
			if (this.emission == null || !this.emission.IsPlaying)
			{
				return;
			}
			base.Finish();
		}

		// Token: 0x060080E6 RID: 32998 RVA: 0x0025F9DE File Offset: 0x0025DBDE
		public override void OnExit()
		{
			if (this.stopOnStateExit.Value)
			{
				VibrationEmission vibrationEmission = this.emission;
				if (vibrationEmission != null)
				{
					vibrationEmission.Stop();
				}
				this.emission = null;
			}
		}

		// Token: 0x0400801C RID: 32796
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		// Token: 0x0400801D RID: 32797
		public FsmFloat strength;

		// Token: 0x0400801E RID: 32798
		[ObjectType(typeof(VibrationMotors))]
		public FsmEnum motors;

		// Token: 0x0400801F RID: 32799
		public FsmBool loop;

		// Token: 0x04008020 RID: 32800
		public FsmString tag;

		// Token: 0x04008021 RID: 32801
		public FsmBool stopOnStateExit;

		// Token: 0x04008022 RID: 32802
		public FsmBool waitUntilFinished;

		// Token: 0x04008023 RID: 32803
		private VibrationEmission emission;
	}
}
