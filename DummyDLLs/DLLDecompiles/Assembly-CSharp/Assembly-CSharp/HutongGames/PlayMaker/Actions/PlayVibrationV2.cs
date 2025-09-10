using System;
using System.Collections.Generic;
using TeamCherry.PS5;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200139E RID: 5022
	[ActionCategory("Hollow Knight")]
	public class PlayVibrationV2 : FsmStateAction
	{
		// Token: 0x060080D4 RID: 32980 RVA: 0x0025F3C0 File Offset: 0x0025D5C0
		public override void Reset()
		{
			base.Reset();
			this.vibrationData = new PlayVibrationV2.FSMVibrationData();
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
		}

		// Token: 0x060080D5 RID: 32981 RVA: 0x0025F420 File Offset: 0x0025D620
		public override void OnEnter()
		{
			base.OnEnter();
			bool value = this.loopAuto.Value;
			this.Play(value);
			if (value)
			{
				base.Finish();
				return;
			}
			this.EnqueueNextLoop();
		}

		// Token: 0x060080D6 RID: 32982 RVA: 0x0025F458 File Offset: 0x0025D658
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
				if (vibrationMotors == VibrationMotors.None && this.vibrationData.gamepadVibration != null)
				{
					vibrationMotors = VibrationMotors.All;
				}
			}
			VibrationData vibrationData = this.vibrationData;
			if (this.vibrationDataAsset.Value)
			{
				vibrationData = (VibrationDataAsset)this.vibrationDataAsset.Value;
			}
			this.emissions.Add(VibrationManager.PlayVibrationClipOneShot(vibrationData, new VibrationTarget?(new VibrationTarget(vibrationMotors)), loop, this.tag.Value ?? "", false));
		}

		// Token: 0x060080D7 RID: 32983 RVA: 0x0025F505 File Offset: 0x0025D705
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

		// Token: 0x060080D8 RID: 32984 RVA: 0x0025F53C File Offset: 0x0025D73C
		public override void OnExit()
		{
			if (this.stopOnStateExit.Value || this.loopAuto.Value)
			{
				foreach (VibrationEmission vibrationEmission in this.emissions)
				{
					if (vibrationEmission != null)
					{
						vibrationEmission.Stop();
					}
				}
			}
			this.emissions.Clear();
		}

		// Token: 0x060080D9 RID: 32985 RVA: 0x0025F5B8 File Offset: 0x0025D7B8
		private void EnqueueNextLoop()
		{
			if (this.loopAuto.Value)
			{
				return;
			}
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

		// Token: 0x0400800B RID: 32779
		public PlayVibrationV2.FSMVibrationData vibrationData;

		// Token: 0x0400800C RID: 32780
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		// Token: 0x0400800D RID: 32781
		[ObjectType(typeof(VibrationMotors))]
		public FsmEnum motors;

		// Token: 0x0400800E RID: 32782
		public FsmFloat loopTime;

		// Token: 0x0400800F RID: 32783
		public FsmBool loopAuto;

		// Token: 0x04008010 RID: 32784
		public FsmString tag;

		// Token: 0x04008011 RID: 32785
		public FsmBool stopOnStateExit;

		// Token: 0x04008012 RID: 32786
		private float cooldownTimer;

		// Token: 0x04008013 RID: 32787
		private List<VibrationEmission> emissions = new List<VibrationEmission>();

		// Token: 0x02001BFE RID: 7166
		[Serializable]
		public class FSMVibrationData
		{
			// Token: 0x170011B9 RID: 4537
			// (get) Token: 0x06009AA4 RID: 39588 RVA: 0x002B3FBD File Offset: 0x002B21BD
			public VibrationData VibrationData
			{
				get
				{
					return this;
				}
			}

			// Token: 0x06009AA5 RID: 39589 RVA: 0x002B3FC8 File Offset: 0x002B21C8
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

			// Token: 0x06009AA6 RID: 39590 RVA: 0x002B4020 File Offset: 0x002B2220
			public static implicit operator VibrationData(PlayVibrationV2.FSMVibrationData wrapper)
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

			// Token: 0x04009FC7 RID: 40903
			[ObjectType(typeof(LowFidelityVibrations))]
			public FsmEnum lowFidelityVibration;

			// Token: 0x04009FC8 RID: 40904
			[ObjectType(typeof(TextAsset))]
			public FsmObject highFidelityVibration;

			// Token: 0x04009FC9 RID: 40905
			[ObjectType(typeof(GamepadVibration))]
			public FsmObject gamepadVibration;

			// Token: 0x04009FCA RID: 40906
			[ObjectType(typeof(PS5VibrationData))]
			public FsmObject ps5Vibration;
		}
	}
}
