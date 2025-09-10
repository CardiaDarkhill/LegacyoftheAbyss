using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A3 RID: 5027
	[ActionCategory("Hollow Knight")]
	public class StopVibrationV2 : FsmStateAction
	{
		// Token: 0x060080F2 RID: 33010 RVA: 0x0025FCE9 File Offset: 0x0025DEE9
		public override void Reset()
		{
			base.Reset();
			this.tag = new FsmString
			{
				UseVariable = true
			};
		}

		// Token: 0x060080F3 RID: 33011 RVA: 0x0025FD04 File Offset: 0x0025DF04
		public override void OnEnter()
		{
			base.OnEnter();
			bool flag = true;
			if (this.fadeTime.Value > 0f)
			{
				this.emissions.Clear();
				VibrationManager.GetVibrationsWithTag(this.tag.Value, this.emissions);
				if (this.emissions.Count > 0)
				{
					if (this.waitUntilFinish.Value)
					{
						flag = false;
					}
					base.StartCoroutine(this.FadeEmissions());
				}
			}
			else if (this.tag == null || this.tag.IsNone || string.IsNullOrEmpty(this.tag.Value))
			{
				VibrationManager.StopAllVibration();
			}
			else
			{
				VibrationMixer mixer = VibrationManager.GetMixer();
				if (mixer != null)
				{
					mixer.StopAllEmissionsWithTag(this.tag.Value);
				}
			}
			if (flag)
			{
				base.Finish();
			}
		}

		// Token: 0x060080F4 RID: 33012 RVA: 0x0025FDC9 File Offset: 0x0025DFC9
		private IEnumerator FadeEmissions()
		{
			float t = 0f;
			float inverse = 1f / this.fadeTime.Value;
			while (t < 1f)
			{
				t += Time.deltaTime * inverse;
				float strength = Mathf.Clamp01(1f - t);
				for (int i = this.emissions.Count - 1; i >= 0; i--)
				{
					VibrationEmission vibrationEmission = this.emissions[i];
					if (!vibrationEmission.IsPlaying)
					{
						this.emissions.RemoveAt(i);
					}
					else
					{
						vibrationEmission.SetStrength(strength);
					}
				}
				yield return null;
			}
			for (int j = this.emissions.Count - 1; j >= 0; j--)
			{
				this.emissions[j].Stop();
			}
			this.emissions.Clear();
			if (this.waitUntilFinish.Value)
			{
				base.Finish();
			}
			yield break;
		}

		// Token: 0x04008031 RID: 32817
		public FsmString tag;

		// Token: 0x04008032 RID: 32818
		public FsmFloat fadeTime;

		// Token: 0x04008033 RID: 32819
		public FsmBool waitUntilFinish;

		// Token: 0x04008034 RID: 32820
		private List<VibrationEmission> emissions = new List<VibrationEmission>();
	}
}
