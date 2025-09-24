using System;
using TeamCherry.PS5;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A1 RID: 5025
	[ActionCategory("Hollow Knight")]
	public class PlayVibrationDelayed : FsmStateAction
	{
		// Token: 0x060080E8 RID: 33000 RVA: 0x0025FA10 File Offset: 0x0025DC10
		public override void Reset()
		{
			base.Reset();
			this.vibrationData = new PlayVibrationDelayed.FSMVibrationData();
			this.vibrationData.Reset();
			this.vibrationDataAsset = null;
			this.motors = new FsmEnum
			{
				UseVariable = false,
				Value = VibrationMotors.All
			};
			this.loopTime = new FsmFloat
			{
				UseVariable = true
			};
			this.delay = null;
			this.strength = 1f;
		}

		// Token: 0x060080E9 RID: 33001 RVA: 0x0025FA87 File Offset: 0x0025DC87
		public override void OnEnter()
		{
			this.timer = 0f;
			if (this.delay.Value == 0f)
			{
				this.Play(false);
				this.EnqueueNextLoop();
			}
		}

		// Token: 0x060080EA RID: 33002 RVA: 0x0025FAB4 File Offset: 0x0025DCB4
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
			VibrationData vibrationData = this.vibrationData;
			if (this.vibrationDataAsset.Value)
			{
				vibrationData = (VibrationDataAsset)this.vibrationDataAsset.Value;
			}
			this.emission = VibrationManager.PlayVibrationClipOneShot(vibrationData, new VibrationTarget?(new VibrationTarget(vibrationMotors)), loop, this.tag.Value ?? "", false);
			if (!this.strength.IsNone)
			{
				VibrationEmission vibrationEmission = this.emission;
				if (vibrationEmission == null)
				{
					return;
				}
				vibrationEmission.SetStrength(this.strength.Value);
			}
		}

		// Token: 0x060080EB RID: 33003 RVA: 0x0025FB74 File Offset: 0x0025DD74
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				if (this.timer >= this.delay.Value)
				{
					this.Play(false);
					this.EnqueueNextLoop();
					return;
				}
			}
			else
			{
				this.cooldownTimer -= Time.deltaTime;
				if (this.cooldownTimer <= 0f)
				{
					this.Play(false);
					this.EnqueueNextLoop();
				}
			}
		}

		// Token: 0x060080EC RID: 33004 RVA: 0x0025FBF3 File Offset: 0x0025DDF3
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

		// Token: 0x060080ED RID: 33005 RVA: 0x0025FC1C File Offset: 0x0025DE1C
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

		// Token: 0x04008024 RID: 32804
		public PlayVibrationDelayed.FSMVibrationData vibrationData;

		// Token: 0x04008025 RID: 32805
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		// Token: 0x04008026 RID: 32806
		[ObjectType(typeof(VibrationMotors))]
		public FsmEnum motors;

		// Token: 0x04008027 RID: 32807
		public FsmFloat loopTime;

		// Token: 0x04008028 RID: 32808
		public FsmBool isLooping;

		// Token: 0x04008029 RID: 32809
		public FsmString tag;

		// Token: 0x0400802A RID: 32810
		public FsmBool stopOnStateExit;

		// Token: 0x0400802B RID: 32811
		public FsmFloat delay;

		// Token: 0x0400802C RID: 32812
		public FsmFloat strength;

		// Token: 0x0400802D RID: 32813
		private float cooldownTimer;

		// Token: 0x0400802E RID: 32814
		private float timer;

		// Token: 0x0400802F RID: 32815
		private VibrationEmission emission;

		// Token: 0x02001BFF RID: 7167
		[Serializable]
		public class FSMVibrationData
		{
			// Token: 0x170011BA RID: 4538
			// (get) Token: 0x06009AA8 RID: 39592 RVA: 0x002B408D File Offset: 0x002B228D
			public VibrationData VibrationData
			{
				get
				{
					return this;
				}
			}

			// Token: 0x06009AA9 RID: 39593 RVA: 0x002B4098 File Offset: 0x002B2298
			public void Reset()
			{
				this.lowFidelityVibration = new FsmEnum
				{
					UseVariable = false
				};
				this.highFidelityVibration = new FsmObject
				{
					UseVariable = false
				};
				this.gamepadVibration = new FsmObject
				{
					UseVariable = false
				};
				this.ps5Vibration = new FsmObject
				{
					UseVariable = false
				};
			}

			// Token: 0x06009AAA RID: 39594 RVA: 0x002B40F0 File Offset: 0x002B22F0
			public static implicit operator VibrationData(PlayVibrationDelayed.FSMVibrationData wrapper)
			{
				if (wrapper != null)
				{
					LowFidelityVibrations lowFidelityVibrations = (LowFidelityVibrations)wrapper.lowFidelityVibration.Value;
					TextAsset textAsset = wrapper.highFidelityVibration.Value as TextAsset;
					GamepadVibration gamepadVibration = wrapper.gamepadVibration.Value as GamepadVibration;
					PS5VibrationData ps5VibrationData = wrapper.ps5Vibration.Value as PS5VibrationData;
					return VibrationData.Create(lowFidelityVibrations, textAsset, gamepadVibration, ps5VibrationData);
				}
				return default(VibrationData);
			}

			// Token: 0x04009FCB RID: 40907
			[ObjectType(typeof(LowFidelityVibrations))]
			public FsmEnum lowFidelityVibration;

			// Token: 0x04009FCC RID: 40908
			[ObjectType(typeof(TextAsset))]
			public FsmObject highFidelityVibration;

			// Token: 0x04009FCD RID: 40909
			[ObjectType(typeof(GamepadVibration))]
			public FsmObject gamepadVibration;

			// Token: 0x04009FCE RID: 40910
			[ObjectType(typeof(PS5VibrationData))]
			public FsmObject ps5Vibration;
		}
	}
}
