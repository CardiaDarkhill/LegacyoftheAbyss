using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200139D RID: 5021
	[ActionCategory("Hollow Knight")]
	public class PlayVibration : FsmStateAction
	{
		// Token: 0x060080CE RID: 32974 RVA: 0x0025F230 File Offset: 0x0025D430
		public override void Reset()
		{
			base.Reset();
			this.lowFidelityVibration = new FsmEnum
			{
				UseVariable = false
			};
			this.highFidelityVibration = new FsmObject
			{
				UseVariable = false
			};
			this.motors = new FsmEnum
			{
				UseVariable = false,
				Value = VibrationMotors.All
			};
			this.loopTime = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x060080CF RID: 32975 RVA: 0x0025F297 File Offset: 0x0025D497
		public override void OnEnter()
		{
			base.OnEnter();
			this.Play(false);
			this.EnqueueNextLoop();
		}

		// Token: 0x060080D0 RID: 32976 RVA: 0x0025F2AC File Offset: 0x0025D4AC
		private void Play(bool loop)
		{
			if (ObjectPool.IsCreatingPool)
			{
				return;
			}
			VibrationMotors vibrationMotors = VibrationMotors.All;
			if (!this.motors.IsNone)
			{
				vibrationMotors = (VibrationMotors)this.motors.Value;
			}
			VibrationManager.PlayVibrationClipOneShot(VibrationData.Create((LowFidelityVibrations)this.lowFidelityVibration.Value, this.highFidelityVibration.Value as TextAsset, this.gamepadVibration.Value as GamepadVibration, null), new VibrationTarget?(new VibrationTarget(vibrationMotors)), loop, this.tag.Value ?? "", false);
		}

		// Token: 0x060080D1 RID: 32977 RVA: 0x0025F33E File Offset: 0x0025D53E
		public override void OnUpdate()
		{
			base.OnUpdate();
			this.cooldownTimer -= Time.deltaTime;
			if (this.cooldownTimer <= 0f)
			{
				this.Play(false);
				this.EnqueueNextLoop();
			}
		}

		// Token: 0x060080D2 RID: 32978 RVA: 0x0025F374 File Offset: 0x0025D574
		private void EnqueueNextLoop()
		{
			float num = 0f;
			if (!this.loopTime.IsNone)
			{
				num = this.loopTime.Value;
			}
			if (num < Mathf.Epsilon)
			{
				base.Finish();
				return;
			}
			this.cooldownTimer = num;
		}

		// Token: 0x04008003 RID: 32771
		[ObjectType(typeof(LowFidelityVibrations))]
		public FsmEnum lowFidelityVibration;

		// Token: 0x04008004 RID: 32772
		[ObjectType(typeof(TextAsset))]
		public FsmObject highFidelityVibration;

		// Token: 0x04008005 RID: 32773
		[ObjectType(typeof(VibrationMotors))]
		public FsmEnum motors;

		// Token: 0x04008006 RID: 32774
		public FsmFloat loopTime;

		// Token: 0x04008007 RID: 32775
		public FsmBool isLooping;

		// Token: 0x04008008 RID: 32776
		public FsmString tag;

		// Token: 0x04008009 RID: 32777
		[ObjectType(typeof(GamepadVibration))]
		public FsmObject gamepadVibration;

		// Token: 0x0400800A RID: 32778
		private float cooldownTimer;
	}
}
