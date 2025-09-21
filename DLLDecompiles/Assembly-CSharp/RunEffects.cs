using System;
using GlobalEnums;
using GlobalSettings;
using UnityEngine;

// Token: 0x0200026E RID: 622
public class RunEffects : MonoBehaviour
{
	// Token: 0x0600162C RID: 5676 RVA: 0x00063510 File Offset: 0x00061710
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<RunEffects.RunEffectsWrapper>(ref this.runTypes, typeof(RunEffects.RunTypes));
		for (int i = 0; i < this.runTypes.Length; i++)
		{
			if (this.runTypes[i] == null)
			{
				this.runTypes[i] = new RunEffects.RunEffectsWrapper();
			}
			ArrayForEnumAttribute.EnsureArraySize<RunEffects.EffectsWrapper>(ref this.runTypes[i].Effects, typeof(EnvironmentTypes));
			for (int j = 0; j < this.runTypes[i].Effects.Length; j++)
			{
				if (this.runTypes[i].Effects[j] == null)
				{
					this.runTypes[i].Effects[j] = new RunEffects.EffectsWrapper();
				}
			}
		}
	}

	// Token: 0x0600162D RID: 5677 RVA: 0x000635BA File Offset: 0x000617BA
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x000635C4 File Offset: 0x000617C4
	private void OnDisable()
	{
		Transform transform = base.transform;
		Vector3 localScale = transform.localScale;
		localScale.x = Mathf.Abs(localScale.x);
		transform.localScale = localScale;
		this.UnregisterSprintMasterEffect();
	}

	// Token: 0x0600162F RID: 5679 RVA: 0x000635FC File Offset: 0x000617FC
	private void OnEnable()
	{
		this.isActive = true;
		foreach (RunEffects.RunEffectsWrapper runEffectsWrapper in this.runTypes)
		{
			RunEffects.EffectsWrapper[] effects = runEffectsWrapper.Effects;
			for (int j = 0; j < effects.Length; j++)
			{
				effects[j].Stop(true);
			}
			ParticleSystem[] array2 = runEffectsWrapper.AllEffects;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].Stop(true);
			}
			array2 = runEffectsWrapper.SprintmasterEffects;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].Stop(true);
			}
		}
		this.currentEffects = null;
		this.previousRunType = null;
		this.sprintmasterEmitSoundDelayLeft = 0f;
	}

	// Token: 0x06001630 RID: 5680 RVA: 0x000636B8 File Offset: 0x000618B8
	private void Update()
	{
		if (!this.isHeroSprintmasterEffect || (this.isHeroEffect && this.hasHC && !this.hc.IsSprintMasterActive))
		{
			foreach (ParticleSystem particleSystem in this.runTypes[(int)this.currentRunType].SprintmasterEffects)
			{
				if (particleSystem.isPlaying)
				{
					particleSystem.Stop();
				}
			}
			this.isSprintMasterEffectActive = false;
			this.sprintmasterEmitSoundDelayLeft = 0f;
		}
		if (this.isActive)
		{
			if (this.isHeroSprintmasterEffect && this.isHeroEffect && !this.isSprintMasterEffectActive && this.hasHC && this.hc.IsSprintMasterActive)
			{
				foreach (ParticleSystem particleSystem2 in this.runTypes[(int)this.currentRunType].SprintmasterEffects)
				{
					if (!particleSystem2.isPlaying)
					{
						particleSystem2.Play();
					}
				}
				this.SprintmasterEmitSound();
				this.isSprintMasterEffectActive = true;
			}
			return;
		}
		RunEffects.RunEffectsWrapper[] array = this.runTypes;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive())
			{
				return;
			}
		}
		base.gameObject.Recycle();
	}

	// Token: 0x06001631 RID: 5681 RVA: 0x000637D4 File Offset: 0x000619D4
	private void RegisterSprintMasterEffect()
	{
		if (!this.registeredSprintMaster)
		{
			if (!this.hasHC)
			{
				return;
			}
			if (this.hc.AudioCtrl == null)
			{
				return;
			}
			this.hc.AudioCtrl.OnPlayFootstep += this.SprintmasterEmitSound;
			this.registeredSprintMaster = true;
		}
	}

	// Token: 0x06001632 RID: 5682 RVA: 0x0006382C File Offset: 0x00061A2C
	private void UnregisterSprintMasterEffect()
	{
		if (this.registeredSprintMaster)
		{
			this.registeredSprintMaster = false;
			if (this.hc != null && this.hc.AudioCtrl != null)
			{
				this.hc.AudioCtrl.OnPlayFootstep -= this.SprintmasterEmitSound;
			}
		}
	}

	// Token: 0x06001633 RID: 5683 RVA: 0x00063888 File Offset: 0x00061A88
	public void StartEffect(bool isHero, bool doSprintMasterEffect = false)
	{
		this.isHeroEffect = isHero;
		this.isHeroSprintmasterEffect = doSprintMasterEffect;
		this.hc = (this.isHeroEffect ? HeroController.instance : null);
		this.hasHC = this.hc;
		if (this.isHeroSprintmasterEffect)
		{
			this.RegisterSprintMasterEffect();
		}
		this.UpdateEffects();
	}

	// Token: 0x06001634 RID: 5684 RVA: 0x000638E0 File Offset: 0x00061AE0
	public void UpdateEffects()
	{
		if (!this.isActive)
		{
			return;
		}
		EnviroRegionListener componentInParent = base.GetComponentInParent<EnviroRegionListener>();
		if (componentInParent == null)
		{
			return;
		}
		if (this.currentEffects != null)
		{
			this.currentEffects.Stop(false);
		}
		if (this.hasHC && this.hc.cState.isBackScuttling)
		{
			this.currentRunType = RunEffects.RunTypes.Sprint;
			base.transform.FlipLocalScale(true, false, false);
		}
		else
		{
			this.currentRunType = (componentInParent.IsSprinting ? RunEffects.RunTypes.Sprint : RunEffects.RunTypes.Run);
		}
		RunEffects.RunEffectsWrapper runEffectsWrapper = this.runTypes[(int)this.currentRunType];
		runEffectsWrapper.StartEffect();
		if (this.previousRunType == null || this.currentRunType != this.previousRunType.Value)
		{
			if (this.previousRunType != null)
			{
				ParticleSystem[] array = this.runTypes[(int)this.previousRunType.Value].AllEffects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Stop(true);
				}
			}
			if (this.isHeroEffect)
			{
				ParticleSystem[] array = runEffectsWrapper.AllEffects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Play(true);
				}
				if (Gameplay.SprintmasterTool.IsEquipped)
				{
					this.isSprintMasterEffectActive = true;
					array = runEffectsWrapper.SprintmasterEffects;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].Play(true);
					}
					this.SprintmasterEmitSound();
				}
				else
				{
					this.sprintmasterEmitSoundDelayLeft = 0f;
				}
			}
			else
			{
				this.sprintmasterEmitSoundDelayLeft = 0f;
			}
			this.previousRunType = new RunEffects.RunTypes?(this.currentRunType);
		}
		EnvironmentTypes currentEnvironmentType = componentInParent.CurrentEnvironmentType;
		if (this.hasChangedColor && currentEnvironmentType != this.previousEnvironmentType)
		{
			RunEffects.RunEffectsWrapper[] array2 = this.runTypes;
			for (int i = 0; i < array2.Length; i++)
			{
				RunEffects.EffectsWrapper[] effects = array2[i].Effects;
				for (int j = 0; j < effects.Length; j++)
				{
					effects[j].RevertColors();
				}
			}
			this.hasChangedColor = false;
		}
		this.previousEnvironmentType = currentEnvironmentType;
		this.currentEffects = runEffectsWrapper.Effects[(int)currentEnvironmentType];
		if (currentEnvironmentType == EnvironmentTypes.Moss || currentEnvironmentType == EnvironmentTypes.ShallowWater || currentEnvironmentType == EnvironmentTypes.RunningWater || currentEnvironmentType == EnvironmentTypes.WetMetal || currentEnvironmentType == EnvironmentTypes.WetWood)
		{
			this.currentEffects.RecordColors();
			Color b;
			AreaEffectTint.IsActive(base.transform.position, out b);
			for (int k = 0; k < this.currentEffects.Particles.Length; k++)
			{
				ParticleSystem.MainModule main = this.currentEffects.Particles[k].main;
				ParticleSystem.MinMaxGradient startColor = main.startColor;
				startColor.color = this.currentEffects.InitialParticleColors[k] * b;
				main.startColor = startColor;
			}
			this.hasChangedColor = true;
			this.currentEffects.didChangeColors = true;
		}
		this.currentEffects.Play();
	}

	// Token: 0x06001635 RID: 5685 RVA: 0x00063BA0 File Offset: 0x00061DA0
	private void SprintmasterEmitSound()
	{
		if (this.hasHC && !this.hc.IsSprintMasterActive)
		{
			return;
		}
		RunEffects.RunEffectsWrapper runEffectsWrapper = this.runTypes[(int)this.currentRunType];
		if (runEffectsWrapper.SprintmasterEffects.Length == 0)
		{
			return;
		}
		runEffectsWrapper.SprintmasterEmitAudio.SpawnAndPlayOneShot(base.transform.position, false);
	}

	// Token: 0x06001636 RID: 5686 RVA: 0x00063BF4 File Offset: 0x00061DF4
	public void Stop()
	{
		if (!this.isActive)
		{
			return;
		}
		if (this.currentEffects != null)
		{
			this.currentEffects.Stop(false);
		}
		ParticleSystem[] array = this.runTypes[(int)this.currentRunType].AllEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(true);
		}
		array = this.runTypes[(int)this.currentRunType].SprintmasterEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(true);
		}
		this.isActive = false;
		this.sprintmasterEmitSoundDelayLeft = 0f;
		this.UnregisterSprintMasterEffect();
		base.transform.SetParent(null, true);
	}

	// Token: 0x04001497 RID: 5271
	[SerializeField]
	[ArrayForEnum(typeof(RunEffects.RunTypes))]
	private RunEffects.RunEffectsWrapper[] runTypes;

	// Token: 0x04001498 RID: 5272
	private RunEffects.EffectsWrapper currentEffects;

	// Token: 0x04001499 RID: 5273
	private RunEffects.RunTypes currentRunType;

	// Token: 0x0400149A RID: 5274
	private RunEffects.RunTypes? previousRunType;

	// Token: 0x0400149B RID: 5275
	private bool isActive;

	// Token: 0x0400149C RID: 5276
	private bool isHeroEffect;

	// Token: 0x0400149D RID: 5277
	private bool isHeroSprintmasterEffect;

	// Token: 0x0400149E RID: 5278
	private HeroController hc;

	// Token: 0x0400149F RID: 5279
	private bool hasHC;

	// Token: 0x040014A0 RID: 5280
	private float sprintmasterEmitSoundDelayLeft;

	// Token: 0x040014A1 RID: 5281
	private bool isSprintMasterEffectActive;

	// Token: 0x040014A2 RID: 5282
	private bool hasChangedColor;

	// Token: 0x040014A3 RID: 5283
	private EnvironmentTypes previousEnvironmentType;

	// Token: 0x040014A4 RID: 5284
	private bool registeredSprintMaster;

	// Token: 0x02001553 RID: 5459
	[Serializable]
	private enum RunTypes
	{
		// Token: 0x040086BD RID: 34493
		Run,
		// Token: 0x040086BE RID: 34494
		Sprint
	}

	// Token: 0x02001554 RID: 5460
	[Serializable]
	private class EffectsWrapper
	{
		// Token: 0x0600866D RID: 34413 RVA: 0x00272CB4 File Offset: 0x00270EB4
		public void Play()
		{
			foreach (ParticleSystem particleSystem in this.Particles)
			{
				if (particleSystem != null)
				{
					particleSystem.Play(true);
				}
			}
		}

		// Token: 0x0600866E RID: 34414 RVA: 0x00272CEC File Offset: 0x00270EEC
		public void Stop(bool clear)
		{
			foreach (ParticleSystem particleSystem in this.Particles)
			{
				if (particleSystem != null)
				{
					particleSystem.Stop(true, clear ? ParticleSystemStopBehavior.StopEmittingAndClear : ParticleSystemStopBehavior.StopEmitting);
				}
			}
		}

		// Token: 0x0600866F RID: 34415 RVA: 0x00272D2C File Offset: 0x00270F2C
		public bool IsAlive()
		{
			foreach (ParticleSystem particleSystem in this.Particles)
			{
				if (particleSystem != null && particleSystem.IsAlive(true))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06008670 RID: 34416 RVA: 0x00272D68 File Offset: 0x00270F68
		public void RecordColors()
		{
			if (this.InitialParticleColors == null || !this.recordedColors)
			{
				this.InitialParticleColors = new Color[this.Particles.Length];
				for (int i = 0; i < this.InitialParticleColors.Length; i++)
				{
					this.InitialParticleColors[i] = this.Particles[i].main.startColor.color;
				}
				this.recordedColors = true;
			}
		}

		// Token: 0x06008671 RID: 34417 RVA: 0x00272DDC File Offset: 0x00270FDC
		public void RevertColors()
		{
			if (!this.didChangeColors)
			{
				return;
			}
			this.didChangeColors = false;
			if (this.recordedColors)
			{
				for (int i = 0; i < this.Particles.Length; i++)
				{
					ParticleSystem.MainModule main = this.Particles[i].main;
					ParticleSystem.MinMaxGradient startColor = main.startColor;
					startColor.color = this.InitialParticleColors[i];
					main.startColor = startColor;
				}
			}
		}

		// Token: 0x040086BF RID: 34495
		public ParticleSystem[] Particles;

		// Token: 0x040086C0 RID: 34496
		[NonSerialized]
		public Color[] InitialParticleColors;

		// Token: 0x040086C1 RID: 34497
		[NonSerialized]
		public bool recordedColors;

		// Token: 0x040086C2 RID: 34498
		[NonSerialized]
		public bool didChangeColors;
	}

	// Token: 0x02001555 RID: 5461
	[Serializable]
	private class RunEffectsWrapper
	{
		// Token: 0x06008673 RID: 34419 RVA: 0x00272E4D File Offset: 0x0027104D
		public void StartEffect()
		{
			this.started = true;
		}

		// Token: 0x06008674 RID: 34420 RVA: 0x00272E58 File Offset: 0x00271058
		public bool IsAlive()
		{
			if (!this.started)
			{
				return false;
			}
			RunEffects.EffectsWrapper[] effects = this.Effects;
			for (int i = 0; i < effects.Length; i++)
			{
				if (effects[i].IsAlive())
				{
					return true;
				}
			}
			foreach (ParticleSystem particleSystem in this.AllEffects)
			{
				if (particleSystem != null && particleSystem.IsAlive(true))
				{
					return true;
				}
			}
			foreach (ParticleSystem particleSystem2 in this.SprintmasterEffects)
			{
				if (particleSystem2 != null && particleSystem2.IsAlive(true))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040086C3 RID: 34499
		[ArrayForEnum(typeof(EnvironmentTypes))]
		public RunEffects.EffectsWrapper[] Effects;

		// Token: 0x040086C4 RID: 34500
		public ParticleSystem[] AllEffects;

		// Token: 0x040086C5 RID: 34501
		public ParticleSystem[] SprintmasterEffects;

		// Token: 0x040086C6 RID: 34502
		public RandomAudioClipTable SprintmasterEmitAudio;

		// Token: 0x040086C7 RID: 34503
		private bool started;
	}
}
