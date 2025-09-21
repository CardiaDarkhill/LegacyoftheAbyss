using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using UnityEngine;

// Token: 0x020007A2 RID: 1954
public sealed class HeroVibrationController : MonoBehaviour
{
	// Token: 0x060044E9 RID: 17641 RVA: 0x0012D415 File Offset: 0x0012B615
	private void Awake()
	{
		this.InitialiseVibrations();
		this.InitialiseLookup();
	}

	// Token: 0x060044EA RID: 17642 RVA: 0x0012D423 File Offset: 0x0012B623
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<VibrationDataAsset>(ref this.heroSoundVibrations, typeof(HeroSounds));
	}

	// Token: 0x060044EB RID: 17643 RVA: 0x0012D43C File Offset: 0x0012B63C
	private void InitialiseLookup()
	{
		this.audioClipVibrationLookup.Clear();
		for (int i = 0; i < this.audioClipVibrations.Count; i++)
		{
			HeroVibrationController.AudioClipVibration audioClipVibration = this.audioClipVibrations[i];
			if (audioClipVibration.audioClip == null)
			{
				Debug.LogError(string.Format("Audio clip vibration element {0} is missing an audio clip.", i), this);
			}
			else if (audioClipVibration.vibrationDataAsset == null)
			{
				Debug.LogError(string.Format("Audio clip vibration element {0} is missing vibration data asset.", i), this);
			}
			else if (!this.audioClipVibrationLookup.TryAdd(audioClipVibration.audioClip, audioClipVibration))
			{
				Debug.LogError(string.Format("{0} (i) multiple vibration data assets assigned.", audioClipVibration.audioClip), this);
			}
		}
	}

	// Token: 0x060044EC RID: 17644 RVA: 0x0012D4F3 File Offset: 0x0012B6F3
	private void InitialiseVibrations()
	{
		this.emissions = new VibrationEmission[Enum.GetValues(typeof(HeroSounds)).Length];
	}

	// Token: 0x060044ED RID: 17645 RVA: 0x0012D514 File Offset: 0x0012B714
	public void PlayVibration(AudioSource audioSource, HeroSounds sounds, bool loop = false, float strength = 1f)
	{
		this.StopVibration(sounds);
		bool flag = false;
		if (loop && !audioSource.loop)
		{
			flag = true;
			audioSource.loop = true;
		}
		VibrationEmission vibrationEmission = null;
		VibrationDataAsset vibrationDataAsset;
		HeroVibrationController.AudioClipVibration audioClipVibration;
		if (this.TryGetHeroSoundClipVibration(sounds, out vibrationDataAsset))
		{
			vibrationEmission = VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset.VibrationData, null, loop, "", false);
		}
		else if (this.audioClipVibrationLookup.TryGetValue(audioSource.clip, out audioClipVibration) && audioClipVibration.vibrationDataAsset)
		{
			vibrationEmission = VibrationManager.PlayVibrationClipOneShot(audioClipVibration.vibrationDataAsset, null, loop, "", false);
		}
		if (vibrationEmission != null)
		{
			vibrationEmission.SetStrength(strength);
		}
		this.emissions[(int)sounds] = vibrationEmission;
		if (flag)
		{
			audioSource.loop = false;
		}
	}

	// Token: 0x060044EE RID: 17646 RVA: 0x0012D5CC File Offset: 0x0012B7CC
	public void StopVibration(HeroSounds sounds)
	{
		VibrationEmission vibrationEmission = this.emissions[(int)sounds];
		if (vibrationEmission == null)
		{
			return;
		}
		vibrationEmission.Stop();
	}

	// Token: 0x060044EF RID: 17647 RVA: 0x0012D5E0 File Offset: 0x0012B7E0
	public void StopAllVibrations()
	{
		for (int i = 0; i < this.emissions.Length; i++)
		{
			VibrationEmission vibrationEmission = this.emissions[i];
			if (vibrationEmission != null)
			{
				vibrationEmission.Stop();
			}
			this.emissions[i] = null;
		}
	}

	// Token: 0x060044F0 RID: 17648 RVA: 0x0012D61C File Offset: 0x0012B81C
	private bool TryGetHeroSoundClipVibration(HeroSounds sound, out VibrationDataAsset vibrationDataAsset)
	{
		if (sound < HeroSounds.FOOTSTEPS_RUN || sound >= (HeroSounds)this.heroSoundVibrations.Length)
		{
			vibrationDataAsset = null;
			return false;
		}
		vibrationDataAsset = this.heroSoundVibrations[(int)sound];
		return vibrationDataAsset != null;
	}

	// Token: 0x060044F1 RID: 17649 RVA: 0x0012D654 File Offset: 0x0012B854
	public void PlaySoftLand()
	{
		VibrationManager.PlayVibrationClipOneShot(this.softLandVibration, null, false, "", false);
	}

	// Token: 0x060044F2 RID: 17650 RVA: 0x0012D684 File Offset: 0x0012B884
	public void PlayFootStep()
	{
		VibrationManager.PlayVibrationClipOneShot(this.footStepVibration, null, false, "", false);
	}

	// Token: 0x060044F3 RID: 17651 RVA: 0x0012D6B4 File Offset: 0x0012B8B4
	public void PlayWallJump()
	{
		VibrationManager.PlayVibrationClipOneShot(this.wallJumpVibration, null, false, "", false);
	}

	// Token: 0x060044F4 RID: 17652 RVA: 0x0012D6E4 File Offset: 0x0012B8E4
	public void PlayDash()
	{
		VibrationManager.PlayVibrationClipOneShot(this.dashVibration, null, false, "", false);
	}

	// Token: 0x060044F5 RID: 17653 RVA: 0x0012D714 File Offset: 0x0012B914
	public void PlayAirDash()
	{
		VibrationManager.PlayVibrationClipOneShot(this.airDashVibration, null, false, "", false);
	}

	// Token: 0x060044F6 RID: 17654 RVA: 0x0012D744 File Offset: 0x0012B944
	public void PlayDoubleJump()
	{
		VibrationManager.PlayVibrationClipOneShot(this.doubleJumpVibration, null, false, "", false);
	}

	// Token: 0x060044F7 RID: 17655 RVA: 0x0012D774 File Offset: 0x0012B974
	public void PlayShadowDash()
	{
		VibrationManager.PlayVibrationClipOneShot(this.shadowDashVibration, null, false, "", false);
	}

	// Token: 0x060044F8 RID: 17656 RVA: 0x0012D7A2 File Offset: 0x0012B9A2
	public void StartWallSlide()
	{
		this.wallSlideVibrationPlayer.Play();
	}

	// Token: 0x060044F9 RID: 17657 RVA: 0x0012D7AF File Offset: 0x0012B9AF
	public void StopWallSlide()
	{
		this.wallSlideVibrationPlayer.Stop();
	}

	// Token: 0x060044FA RID: 17658 RVA: 0x0012D7BC File Offset: 0x0012B9BC
	public void StartShuttlecock()
	{
		this.shuttleClockEmission = VibrationManager.PlayVibrationClipOneShot(this.shuttleCockJumpVibration, null, false, "", false);
	}

	// Token: 0x060044FB RID: 17659 RVA: 0x0012D7EF File Offset: 0x0012B9EF
	public void StopShuttlecock()
	{
		VibrationEmission vibrationEmission = this.shuttleClockEmission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		this.shuttleClockEmission = null;
	}

	// Token: 0x060044FC RID: 17660 RVA: 0x0012D80C File Offset: 0x0012BA0C
	public void PlayHeroDeath()
	{
		VibrationManager.PlayVibrationClipOneShot(this.heroDeathVibration, null, false, "", false);
	}

	// Token: 0x060044FD RID: 17661 RVA: 0x0012D83C File Offset: 0x0012BA3C
	public void PlayHeroHazardDeath()
	{
		VibrationManager.PlayVibrationClipOneShot(this.heroHazardDeathVibration, null, false, "", false);
	}

	// Token: 0x060044FE RID: 17662 RVA: 0x0012D86A File Offset: 0x0012BA6A
	public void PlayHeroDamage()
	{
	}

	// Token: 0x060044FF RID: 17663 RVA: 0x0012D86C File Offset: 0x0012BA6C
	public void PlaySwimEnter()
	{
		VibrationManager.PlayVibrationClipOneShot(this.swimEnter, null, false, "", false);
		VibrationEmission vibrationEmission = this.swimLoopEmission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		VibrationEmission vibrationEmission2 = this.swimLoopFastEmission;
		if (vibrationEmission2 != null)
		{
			vibrationEmission2.Stop();
		}
		this.swimLoopEmission = VibrationManager.PlayVibrationClipOneShot(this.swimLoop, null, true, "", false);
		this.swimLoopFastEmission = VibrationManager.PlayVibrationClipOneShot(this.swimLoopFast, null, true, "", false);
		this.isSwimming = false;
		this.isSwimSprint = false;
		this.UpdateSwimSpeed(this.isSwimming, this.isSwimSprint);
	}

	// Token: 0x06004500 RID: 17664 RVA: 0x0012D928 File Offset: 0x0012BB28
	public void PlaySwimExit()
	{
		VibrationManager.PlayVibrationClipOneShot(this.swimExit, null, false, "", false);
		VibrationEmission vibrationEmission = this.swimLoopEmission;
		if (vibrationEmission != null)
		{
			vibrationEmission.Stop();
		}
		VibrationEmission vibrationEmission2 = this.swimLoopFastEmission;
		if (vibrationEmission2 != null)
		{
			vibrationEmission2.Stop();
		}
		this.swimLoopEmission = null;
		this.swimLoopFastEmission = null;
	}

	// Token: 0x06004501 RID: 17665 RVA: 0x0012D988 File Offset: 0x0012BB88
	public void PlayToolThrow()
	{
		VibrationManager.PlayVibrationClipOneShot(this.toolThrowVibration, null, false, "", false);
	}

	// Token: 0x06004502 RID: 17666 RVA: 0x0012D9B8 File Offset: 0x0012BBB8
	public void SetSwimAndSprint(bool swimming, bool isSprinting)
	{
		bool flag = false;
		if (this.isSwimming != swimming)
		{
			this.isSwimming = swimming;
			flag = true;
		}
		if (this.isSwimSprint != isSprinting)
		{
			this.isSwimSprint = isSprinting;
			flag = true;
		}
		if (flag)
		{
			this.UpdateSwimSpeed(this.isSwimming, this.isSwimSprint);
		}
	}

	// Token: 0x06004503 RID: 17667 RVA: 0x0012DA00 File Offset: 0x0012BC00
	public void SetSwimming(bool swimming)
	{
		if (this.isSwimming != swimming)
		{
			this.isSwimming = swimming;
			this.UpdateSwimSpeed(this.isSwimming, this.isSwimSprint);
		}
	}

	// Token: 0x06004504 RID: 17668 RVA: 0x0012DA24 File Offset: 0x0012BC24
	public void SetSwimSprint(bool isSprinting)
	{
		if (this.isSwimSprint != isSprinting)
		{
			this.isSwimSprint = isSprinting;
			this.UpdateSwimSpeed(this.isSwimming, this.isSwimSprint);
		}
	}

	// Token: 0x06004505 RID: 17669 RVA: 0x0012DA48 File Offset: 0x0012BC48
	private void UpdateSwimSpeed(bool isSwimming, bool isSprinting)
	{
		if (isSwimming)
		{
			if (isSprinting)
			{
				VibrationEmission vibrationEmission = this.swimLoopEmission;
				if (vibrationEmission != null)
				{
					vibrationEmission.SetStrength(0f);
				}
				VibrationEmission vibrationEmission2 = this.swimLoopFastEmission;
				if (vibrationEmission2 != null)
				{
					vibrationEmission2.SetStrength(1f);
				}
				VibrationManager.PlayVibrationClipOneShot(this.fastSwimStart, null, false, "", false);
				return;
			}
			VibrationEmission vibrationEmission3 = this.swimLoopEmission;
			if (vibrationEmission3 != null)
			{
				vibrationEmission3.SetStrength(1f);
			}
			VibrationEmission vibrationEmission4 = this.swimLoopFastEmission;
			if (vibrationEmission4 == null)
			{
				return;
			}
			vibrationEmission4.SetStrength(0f);
			return;
		}
		else
		{
			VibrationEmission vibrationEmission5 = this.swimLoopEmission;
			if (vibrationEmission5 != null)
			{
				vibrationEmission5.SetStrength(0f);
			}
			VibrationEmission vibrationEmission6 = this.swimLoopFastEmission;
			if (vibrationEmission6 == null)
			{
				return;
			}
			vibrationEmission6.SetStrength(0f);
			return;
		}
	}

	// Token: 0x06004506 RID: 17670 RVA: 0x0012DB00 File Offset: 0x0012BD00
	public void StartRosaryCannonCharge()
	{
		if (this.rosaryCannonVibration == null)
		{
			this.rosaryCannonVibration = new HeroVibrationController.RampingVibration
			{
				duration = this.rosaryCannonChargeDuration,
				owner = this,
				vibrationDataAsset = this.rosaryCannonCharge
			};
		}
		this.rosaryCannonVibration.StartVibration();
	}

	// Token: 0x06004507 RID: 17671 RVA: 0x0012DB3F File Offset: 0x0012BD3F
	public void StopRosaryCannonCharge()
	{
		HeroVibrationController.RampingVibration rampingVibration = this.rosaryCannonVibration;
		if (rampingVibration == null)
		{
			return;
		}
		rampingVibration.StopVibration();
	}

	// Token: 0x040045C9 RID: 17865
	[SerializeField]
	private VibrationDataAsset softLandVibration;

	// Token: 0x040045CA RID: 17866
	[SerializeField]
	private VibrationDataAsset footStepVibration;

	// Token: 0x040045CB RID: 17867
	[SerializeField]
	private VibrationDataAsset wallJumpVibration;

	// Token: 0x040045CC RID: 17868
	[SerializeField]
	private VibrationDataAsset dashVibration;

	// Token: 0x040045CD RID: 17869
	[SerializeField]
	private VibrationDataAsset airDashVibration;

	// Token: 0x040045CE RID: 17870
	[SerializeField]
	private VibrationDataAsset shadowDashVibration;

	// Token: 0x040045CF RID: 17871
	[SerializeField]
	private VibrationDataAsset doubleJumpVibration;

	// Token: 0x040045D0 RID: 17872
	[SerializeField]
	private VibrationDataAsset heroDeathVibration;

	// Token: 0x040045D1 RID: 17873
	[SerializeField]
	private VibrationDataAsset heroHazardDeathVibration;

	// Token: 0x040045D2 RID: 17874
	[SerializeField]
	private VibrationDataAsset heroDamage;

	// Token: 0x040045D3 RID: 17875
	[SerializeField]
	private VibrationDataAsset swimEnter;

	// Token: 0x040045D4 RID: 17876
	[SerializeField]
	private VibrationDataAsset swimExit;

	// Token: 0x040045D5 RID: 17877
	[SerializeField]
	private VibrationDataAsset swimLoop;

	// Token: 0x040045D6 RID: 17878
	[SerializeField]
	private VibrationDataAsset swimLoopFast;

	// Token: 0x040045D7 RID: 17879
	[SerializeField]
	private VibrationDataAsset fastSwimStart;

	// Token: 0x040045D8 RID: 17880
	[SerializeField]
	private VibrationDataAsset toolThrowVibration;

	// Token: 0x040045D9 RID: 17881
	[Space]
	[SerializeField]
	private VibrationPlayer wallSlideVibrationPlayer;

	// Token: 0x040045DA RID: 17882
	[SerializeField]
	private VibrationDataAsset shuttleCockJumpVibration;

	// Token: 0x040045DB RID: 17883
	[Space]
	[SerializeField]
	private VibrationDataAsset rosaryCannonCharge;

	// Token: 0x040045DC RID: 17884
	[SerializeField]
	private float rosaryCannonChargeDuration = 0.3f;

	// Token: 0x040045DD RID: 17885
	[NonReorderable]
	[ArrayForEnum(typeof(HeroSounds))]
	[SerializeField]
	private VibrationDataAsset[] heroSoundVibrations = new VibrationDataAsset[0];

	// Token: 0x040045DE RID: 17886
	[Space]
	[SerializeField]
	private List<HeroVibrationController.AudioClipVibration> audioClipVibrations = new List<HeroVibrationController.AudioClipVibration>();

	// Token: 0x040045DF RID: 17887
	private VibrationEmission swimLoopEmission;

	// Token: 0x040045E0 RID: 17888
	private VibrationEmission swimLoopFastEmission;

	// Token: 0x040045E1 RID: 17889
	private VibrationEmission[] emissions;

	// Token: 0x040045E2 RID: 17890
	private Dictionary<AudioClip, HeroVibrationController.AudioClipVibration> audioClipVibrationLookup = new Dictionary<AudioClip, HeroVibrationController.AudioClipVibration>();

	// Token: 0x040045E3 RID: 17891
	private HeroVibrationController.RampingVibration rosaryCannonVibration;

	// Token: 0x040045E4 RID: 17892
	private VibrationEmission shuttleClockEmission;

	// Token: 0x040045E5 RID: 17893
	private bool isSwimming;

	// Token: 0x040045E6 RID: 17894
	private bool isSwimSprint;

	// Token: 0x02001A78 RID: 6776
	[Serializable]
	private class AudioClipVibration
	{
		// Token: 0x04009996 RID: 39318
		public AudioClip audioClip;

		// Token: 0x04009997 RID: 39319
		public VibrationDataAsset vibrationDataAsset;
	}

	// Token: 0x02001A79 RID: 6777
	private class RampingVibration
	{
		// Token: 0x06009709 RID: 38665 RVA: 0x002A9580 File Offset: 0x002A7780
		public void StartVibration()
		{
			this.StopVibration();
			if (this.owner == null)
			{
				return;
			}
			if (this.duration > 0f)
			{
				this.emission = VibrationManager.PlayVibrationClipOneShot(this.vibrationDataAsset, null, true, "", false);
				if (this.emission == null)
				{
					return;
				}
				this.emission.SetStrength(0f);
				this.coroutine = this.owner.StartCoroutine(this.RampRoutine());
			}
		}

		// Token: 0x0600970A RID: 38666 RVA: 0x002A9608 File Offset: 0x002A7808
		public void StopVibration()
		{
			VibrationEmission vibrationEmission = this.emission;
			if (vibrationEmission != null)
			{
				vibrationEmission.Stop();
			}
			this.emission = null;
			if (this.owner == null)
			{
				return;
			}
			if (this.coroutine != null)
			{
				this.owner.StopCoroutine(this.coroutine);
				this.coroutine = null;
			}
		}

		// Token: 0x0600970B RID: 38667 RVA: 0x002A965C File Offset: 0x002A785C
		private IEnumerator RampRoutine()
		{
			if (this.duration > 0f)
			{
				this.strength = 0f;
				if (this.emission == null)
				{
					this.coroutine = null;
					yield break;
				}
				float t = 0f;
				float multiplier = 1f / this.duration;
				while (t < 1f)
				{
					t += Time.deltaTime * multiplier;
					this.emission.SetStrength(Mathf.Lerp(0f, 1f, t));
					yield return null;
				}
			}
			this.emission.SetStrength(1f);
			this.coroutine = null;
			yield break;
		}

		// Token: 0x04009998 RID: 39320
		public HeroVibrationController owner;

		// Token: 0x04009999 RID: 39321
		public VibrationDataAsset vibrationDataAsset;

		// Token: 0x0400999A RID: 39322
		public float duration;

		// Token: 0x0400999B RID: 39323
		public float strength;

		// Token: 0x0400999C RID: 39324
		public VibrationEmission emission;

		// Token: 0x0400999D RID: 39325
		public Coroutine coroutine;
	}
}
