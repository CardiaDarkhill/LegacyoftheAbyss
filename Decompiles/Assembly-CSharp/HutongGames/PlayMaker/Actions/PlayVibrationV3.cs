using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200139F RID: 5023
	[ActionCategory("Hollow Knight")]
	public class PlayVibrationV3 : FsmStateAction
	{
		// Token: 0x060080DB RID: 32987 RVA: 0x0025F61C File Offset: 0x0025D81C
		public override void Reset()
		{
			base.Reset();
			this.vibrationDataAsset = null;
			this.motors = new FsmEnum
			{
				UseVariable = true,
				Value = VibrationMotors.All
			};
			this.loopTime = new FsmFloat
			{
				UseVariable = true
			};
			this.loopAuto = null;
			this.tag = null;
			this.stopOnStateExit = null;
		}

		// Token: 0x060080DC RID: 32988 RVA: 0x0025F67C File Offset: 0x0025D87C
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

		// Token: 0x060080DD RID: 32989 RVA: 0x0025F6B4 File Offset: 0x0025D8B4
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
			this.emissions.Add(VibrationManager.PlayVibrationClipOneShot(vibrationData, new VibrationTarget?(new VibrationTarget(vibrationMotors)), loop, this.tag.Value ?? "", false));
		}

		// Token: 0x060080DE RID: 32990 RVA: 0x0025F75B File Offset: 0x0025D95B
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

		// Token: 0x060080DF RID: 32991 RVA: 0x0025F790 File Offset: 0x0025D990
		public override void OnExit()
		{
			if (this.stopOnStateExit.Value)
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

		// Token: 0x060080E0 RID: 32992 RVA: 0x0025F800 File Offset: 0x0025DA00
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

		// Token: 0x04008014 RID: 32788
		[ObjectType(typeof(VibrationDataAsset))]
		public FsmObject vibrationDataAsset;

		// Token: 0x04008015 RID: 32789
		[ObjectType(typeof(VibrationMotors))]
		public FsmEnum motors;

		// Token: 0x04008016 RID: 32790
		public FsmFloat loopTime;

		// Token: 0x04008017 RID: 32791
		public FsmBool loopAuto;

		// Token: 0x04008018 RID: 32792
		public FsmString tag;

		// Token: 0x04008019 RID: 32793
		public FsmBool stopOnStateExit;

		// Token: 0x0400801A RID: 32794
		private float cooldownTimer;

		// Token: 0x0400801B RID: 32795
		private List<VibrationEmission> emissions = new List<VibrationEmission>();
	}
}
