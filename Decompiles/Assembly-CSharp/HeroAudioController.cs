using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;

// Token: 0x0200011E RID: 286
public class HeroAudioController : MonoBehaviour
{
	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x060008C9 RID: 2249 RVA: 0x0002905D File Offset: 0x0002725D
	private bool CanPlayFootsteps
	{
		get
		{
			return this.canPlayWalk || this.canPlayRun || this.canPlaySprint || this.heroCtrl.cState.dashing || this.canPlayTime > Time.timeAsDouble;
		}
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00029098 File Offset: 0x00027298
	private void Awake()
	{
		this.heroCtrl = base.GetComponent<HeroController>();
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x000290A6 File Offset: 0x000272A6
	private void Start()
	{
		if (this.heroCtrl)
		{
			this.heroVibrationCtrl = this.heroCtrl.GetVibrationCtrl();
		}
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x000290C8 File Offset: 0x000272C8
	public void PlaySound(HeroSounds soundEffect, bool playVibration = false)
	{
		if (this.heroCtrl.cState.isPaused)
		{
			return;
		}
		bool flag = false;
		switch (soundEffect)
		{
		case HeroSounds.FOOTSTEPS_RUN:
			if (!this.BlockFootstepAudio)
			{
				bool canPlayFootsteps = this.CanPlayFootsteps;
				this.canPlayRun = true;
				if (!this.playedRunStart && !canPlayFootsteps)
				{
					this.playedRunStart = true;
					flag = true;
					if (playVibration)
					{
					}
				}
			}
			break;
		case HeroSounds.FOOTSTEPS_WALK:
			if (!this.BlockFootstepAudio)
			{
				bool canPlayFootsteps2 = this.CanPlayFootsteps;
				this.canPlayWalk = true;
				if (!this.playedWalkStart && !canPlayFootsteps2)
				{
					this.playedWalkStart = true;
					flag = true;
					if (playVibration)
					{
					}
				}
			}
			break;
		case HeroSounds.JUMP:
			this.RandomizePitch(this.jump, 0.9f, 1.1f);
			this.jump.Play();
			break;
		case HeroSounds.WALLJUMP:
			this.RandomizePitch(this.walljump, 0.9f, 1.1f);
			this.walljump.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.walljump, soundEffect, false, 1f);
			}
			break;
		case HeroSounds.SOFT_LANDING:
			this.RandomizePitch(this.jump, 0.9f, 1.1f);
			this.softLanding.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.softLanding, soundEffect, false, 1f);
			}
			break;
		case HeroSounds.HARD_LANDING:
			this.hardLanding.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.hardLanding, soundEffect, false, 1f);
			}
			break;
		case HeroSounds.BACKDASH:
			this.backDash.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.backDash, soundEffect, false, 1f);
			}
			break;
		case HeroSounds.DASH:
			this.dash.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.dash, soundEffect, false, 1f);
			}
			break;
		case HeroSounds.TAKE_HIT:
			this.takeHit.Play();
			break;
		case HeroSounds.WALLSLIDE:
			this.wallslide.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.StartWallSlide();
			}
			break;
		case HeroSounds.NAIL_ART_CHARGE:
			this.nailArtCharge.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.nailArtCharge, soundEffect, true, 1f);
			}
			break;
		case HeroSounds.NAIL_ART_READY:
			this.nailArtReady.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.nailArtReady, soundEffect, false, 1f);
			}
			break;
		case HeroSounds.FALLING:
			this.fallingCo = base.StartCoroutine(this.FadeInVolume(this.falling, 0.7f));
			this.falling.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.falling, soundEffect, true, 1f);
			}
			break;
		case HeroSounds.FOOTSTEPS_SPRINT:
			this.canPlaySprint = true;
			break;
		case HeroSounds.UPDRAFT_IDLE:
			if (!this.updraftIdle.isPlaying)
			{
				this.updraftIdle.Play();
				if (playVibration)
				{
					this.heroVibrationCtrl.PlayVibration(this.updraftIdle, soundEffect, true, 1f);
				}
			}
			break;
		case HeroSounds.DASH_SILK:
			this.dashSilk.Play();
			if (playVibration)
			{
				this.heroVibrationCtrl.PlayVibration(this.dashSilk, soundEffect, false, 1f);
			}
			break;
		case HeroSounds.WINDY_IDLE:
			if (!this.windyIdle.isPlaying)
			{
				this.windyIdle.Play();
				if (playVibration)
				{
					this.heroVibrationCtrl.PlayVibration(this.windyIdle, soundEffect, true, 1f);
				}
			}
			break;
		}
		if (flag && !this.heroCtrl.cState.transitioning)
		{
			if (this.heroCtrl.Config.ForceBareInventory)
			{
				this.runStartClipsCloakless.SpawnAndPlayOneShot(this.audioSourcePrefab, this.footSteps.transform.position, false, 1f, null);
				return;
			}
			this.runStartClips.SpawnAndPlayOneShot(this.audioSourcePrefab, this.footSteps.transform.position, false, 1f, null);
		}
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x000294E0 File Offset: 0x000276E0
	public void StopSound(HeroSounds soundEffect, bool resetStarts = true)
	{
		if (this.heroVibrationCtrl)
		{
			this.heroVibrationCtrl.StopVibration(soundEffect);
		}
		switch (soundEffect)
		{
		case HeroSounds.FOOTSTEPS_RUN:
			this.canPlayRun = false;
			if (resetStarts)
			{
				this.playedRunStart = false;
				return;
			}
			break;
		case HeroSounds.FOOTSTEPS_WALK:
			this.canPlayWalk = false;
			if (resetStarts)
			{
				this.playedWalkStart = false;
				return;
			}
			break;
		case HeroSounds.JUMP:
		case HeroSounds.WALLJUMP:
		case HeroSounds.SOFT_LANDING:
		case HeroSounds.HARD_LANDING:
		case HeroSounds.BACKDASH:
		case HeroSounds.DASH:
		case HeroSounds.TAKE_HIT:
		case HeroSounds.DASH_SILK:
			break;
		case HeroSounds.WALLSLIDE:
			this.wallslide.Stop();
			this.heroVibrationCtrl.StopWallSlide();
			return;
		case HeroSounds.NAIL_ART_CHARGE:
			this.nailArtCharge.Stop();
			return;
		case HeroSounds.NAIL_ART_READY:
			this.nailArtReady.Stop();
			return;
		case HeroSounds.FALLING:
			this.falling.Stop();
			if (this.fallingCo != null)
			{
				base.StopCoroutine(this.fallingCo);
				return;
			}
			break;
		case HeroSounds.FOOTSTEPS_SPRINT:
			this.canPlaySprint = false;
			return;
		case HeroSounds.UPDRAFT_IDLE:
			this.updraftIdle.Stop();
			return;
		case HeroSounds.WINDY_IDLE:
			this.windyIdle.Stop();
			break;
		default:
			return;
		}
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x000295E8 File Offset: 0x000277E8
	public void StopAllSounds()
	{
		if (this.heroVibrationCtrl)
		{
			this.heroVibrationCtrl.StopAllVibrations();
		}
		this.softLanding.Stop();
		this.hardLanding.Stop();
		this.jump.Stop();
		this.takeHit.Stop();
		this.backDash.Stop();
		this.dash.Stop();
		this.dashSilk.Stop();
		this.footSteps.Stop();
		this.playedRunStart = false;
		this.wallslide.Stop();
		this.nailArtCharge.Stop();
		this.nailArtReady.Stop();
		this.falling.Stop();
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x00029698 File Offset: 0x00027898
	public void PauseAllSounds()
	{
		this.softLanding.Pause();
		this.hardLanding.Pause();
		this.jump.Pause();
		this.takeHit.Pause();
		this.backDash.Pause();
		this.dash.Pause();
		this.dashSilk.Pause();
		this.footSteps.Pause();
		this.wallslide.Pause();
		this.nailArtCharge.Pause();
		this.nailArtReady.Pause();
		this.falling.Pause();
		this.BlockFootstepAudio = true;
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x00029730 File Offset: 0x00027930
	public void UnPauseAllSounds()
	{
		this.softLanding.UnPause();
		this.hardLanding.UnPause();
		this.jump.UnPause();
		this.takeHit.UnPause();
		this.backDash.UnPause();
		this.dash.UnPause();
		this.dashSilk.UnPause();
		this.footSteps.UnPause();
		this.wallslide.UnPause();
		this.nailArtCharge.UnPause();
		this.nailArtReady.UnPause();
		this.falling.UnPause();
		this.BlockFootstepAudio = false;
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x000297C8 File Offset: 0x000279C8
	public void SetFootstepsTable(RandomAudioClipTable newTable)
	{
		this.footstepsTable = newTable;
	}

	// Token: 0x170000B7 RID: 183
	// (get) Token: 0x060008D2 RID: 2258 RVA: 0x000297D1 File Offset: 0x000279D1
	// (set) Token: 0x060008D3 RID: 2259 RVA: 0x000297D9 File Offset: 0x000279D9
	public bool BlockFootstepAudio { get; set; }

	// Token: 0x1400000E RID: 14
	// (add) Token: 0x060008D4 RID: 2260 RVA: 0x000297E4 File Offset: 0x000279E4
	// (remove) Token: 0x060008D5 RID: 2261 RVA: 0x0002981C File Offset: 0x00027A1C
	public event Action OnPlayFootstep;

	// Token: 0x060008D6 RID: 2262 RVA: 0x00029854 File Offset: 0x00027A54
	public void PlayFootstep()
	{
		if (!this.CanPlayFootsteps || this.BlockFootstepAudio)
		{
			return;
		}
		this.footstepsTable.PlayOneShot(this.footSteps, false);
		Action onPlayFootstep = this.OnPlayFootstep;
		if (onPlayFootstep != null)
		{
			onPlayFootstep();
		}
		if (!this.canPlaySprint)
		{
			return;
		}
		this.heroVibrationCtrl.PlayFootStep();
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x000298A9 File Offset: 0x00027AA9
	public void AllowFootstepsGrace()
	{
		this.canPlayTime = Time.timeAsDouble + 0.10000000149011612;
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x000298C0 File Offset: 0x00027AC0
	private void RandomizePitch(AudioSource src, float minPitch, float maxPitch)
	{
		float pitch = Random.Range(minPitch, maxPitch);
		src.pitch = pitch;
	}

	// Token: 0x060008D9 RID: 2265 RVA: 0x000298DC File Offset: 0x00027ADC
	private void ResetPitch(AudioSource src)
	{
		src.pitch = 1f;
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x000298E9 File Offset: 0x00027AE9
	private IEnumerator FadeInVolume(AudioSource src, float duration)
	{
		float elapsedTime = 0f;
		src.volume = 0f;
		while (elapsedTime < duration)
		{
			elapsedTime += Time.deltaTime;
			float t = elapsedTime / duration;
			src.volume = Mathf.Lerp(0f, 1f, t);
			yield return null;
		}
		yield break;
	}

	// Token: 0x04000874 RID: 2164
	private HeroController heroCtrl;

	// Token: 0x04000875 RID: 2165
	private HeroVibrationController heroVibrationCtrl;

	// Token: 0x04000876 RID: 2166
	[Header("Sound Effects")]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x04000877 RID: 2167
	public AudioSource softLanding;

	// Token: 0x04000878 RID: 2168
	public AudioSource hardLanding;

	// Token: 0x04000879 RID: 2169
	public AudioSource jump;

	// Token: 0x0400087A RID: 2170
	public AudioSource takeHit;

	// Token: 0x0400087B RID: 2171
	public AudioSource backDash;

	// Token: 0x0400087C RID: 2172
	public AudioSource dash;

	// Token: 0x0400087D RID: 2173
	public AudioSource dashSilk;

	// Token: 0x0400087E RID: 2174
	public AudioSource updraftIdle;

	// Token: 0x0400087F RID: 2175
	public AudioSource windyIdle;

	// Token: 0x04000880 RID: 2176
	public AudioSource footSteps;

	// Token: 0x04000881 RID: 2177
	[SerializeField]
	private RandomAudioClipTable runStartClips;

	// Token: 0x04000882 RID: 2178
	[SerializeField]
	private RandomAudioClipTable runStartClipsCloakless;

	// Token: 0x04000883 RID: 2179
	public AudioSource wallslide;

	// Token: 0x04000884 RID: 2180
	public AudioSource nailArtCharge;

	// Token: 0x04000885 RID: 2181
	public AudioSource nailArtReady;

	// Token: 0x04000886 RID: 2182
	public AudioSource falling;

	// Token: 0x04000887 RID: 2183
	public AudioSource walljump;

	// Token: 0x04000888 RID: 2184
	private bool playedRunStart;

	// Token: 0x04000889 RID: 2185
	private bool playedWalkStart;

	// Token: 0x0400088A RID: 2186
	private Coroutine fallingCo;

	// Token: 0x0400088B RID: 2187
	private double canPlayTime;

	// Token: 0x0400088C RID: 2188
	private bool canPlayWalk;

	// Token: 0x0400088D RID: 2189
	private bool canPlayRun;

	// Token: 0x0400088E RID: 2190
	private bool canPlaySprint;

	// Token: 0x0400088F RID: 2191
	private RandomAudioClipTable footstepsTable;
}
