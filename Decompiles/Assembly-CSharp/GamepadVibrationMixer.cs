using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000456 RID: 1110
public sealed class GamepadVibrationMixer : VibrationMixer
{
	// Token: 0x1700043F RID: 1087
	// (get) Token: 0x0600274A RID: 10058 RVA: 0x000B07E1 File Offset: 0x000AE9E1
	// (set) Token: 0x0600274B RID: 10059 RVA: 0x000B07E8 File Offset: 0x000AE9E8
	public static bool AllowPlatformAdjustment { get; set; } = true;

	// Token: 0x17000440 RID: 1088
	// (get) Token: 0x0600274C RID: 10060 RVA: 0x000B07F0 File Offset: 0x000AE9F0
	// (set) Token: 0x0600274D RID: 10061 RVA: 0x000B07F7 File Offset: 0x000AE9F7
	public static float AdjustmentFactor { get; set; } = 0.5f;

	// Token: 0x17000441 RID: 1089
	// (get) Token: 0x0600274E RID: 10062 RVA: 0x000B07FF File Offset: 0x000AE9FF
	// (set) Token: 0x0600274F RID: 10063 RVA: 0x000B0807 File Offset: 0x000AEA07
	public override bool IsPaused
	{
		get
		{
			return this.isPaused;
		}
		set
		{
			this.isPaused = value;
		}
	}

	// Token: 0x17000442 RID: 1090
	// (get) Token: 0x06002750 RID: 10064 RVA: 0x000B0810 File Offset: 0x000AEA10
	public override int PlayingEmissionCount
	{
		get
		{
			return this.playingEmissions.Count;
		}
	}

	// Token: 0x06002751 RID: 10065 RVA: 0x000B081D File Offset: 0x000AEA1D
	public override VibrationEmission GetPlayingEmission(int playingEmissionIndex)
	{
		return this.playingEmissions[playingEmissionIndex];
	}

	// Token: 0x06002752 RID: 10066 RVA: 0x000B082B File Offset: 0x000AEA2B
	public GamepadVibrationMixer(GamepadVibrationMixer.PlatformAdjustments platformAdjustment = GamepadVibrationMixer.PlatformAdjustments.None)
	{
		this.platformAdjustment = platformAdjustment;
		this.playingEmissions = new List<GamepadVibrationMixer.GamepadVibrationEmission>(25);
	}

	// Token: 0x06002753 RID: 10067 RVA: 0x000B0848 File Offset: 0x000AEA48
	public override VibrationEmission PlayEmission(VibrationData vibrationData, VibrationTarget vibrationTarget, bool isLooping, string tag, bool isRealtime)
	{
		if (vibrationData.GamepadVibration == null)
		{
			return null;
		}
		GamepadVibrationMixer.GamepadVibrationEmission gamepadVibrationEmission = new GamepadVibrationMixer.GamepadVibrationEmission(this, vibrationData.GamepadVibration, isLooping, tag, vibrationTarget, isRealtime, vibrationData.Strength);
		this.playingEmissions.Add(gamepadVibrationEmission);
		return gamepadVibrationEmission;
	}

	// Token: 0x06002754 RID: 10068 RVA: 0x000B0890 File Offset: 0x000AEA90
	public override VibrationEmission PlayEmission(VibrationEmission emission)
	{
		if (emission == null)
		{
			return null;
		}
		GamepadVibrationMixer.GamepadVibrationEmission gamepadVibrationEmission = emission as GamepadVibrationMixer.GamepadVibrationEmission;
		if (gamepadVibrationEmission != null)
		{
			emission.Play();
			this.playingEmissions.Add(gamepadVibrationEmission);
		}
		return emission;
	}

	// Token: 0x06002755 RID: 10069 RVA: 0x000B08C0 File Offset: 0x000AEAC0
	public override void StopAllEmissions()
	{
		for (int i = 0; i < this.playingEmissions.Count; i++)
		{
			this.playingEmissions[i].Stop();
		}
	}

	// Token: 0x06002756 RID: 10070 RVA: 0x000B08F4 File Offset: 0x000AEAF4
	public override void StopAllEmissionsWithTag(string tag)
	{
		for (int i = 0; i < this.playingEmissions.Count; i++)
		{
			GamepadVibrationMixer.GamepadVibrationEmission gamepadVibrationEmission = this.playingEmissions[i];
			if (gamepadVibrationEmission.Tag == tag)
			{
				gamepadVibrationEmission.Stop();
			}
		}
	}

	// Token: 0x17000443 RID: 1091
	// (get) Token: 0x06002757 RID: 10071 RVA: 0x000B0938 File Offset: 0x000AEB38
	public GamepadVibrationMixer.GamepadVibrationEmission.Values CurrentValues
	{
		get
		{
			return this.currentValues;
		}
	}

	// Token: 0x06002758 RID: 10072 RVA: 0x000B0940 File Offset: 0x000AEB40
	public void Update(float deltaTime)
	{
		GamepadVibrationMixer.GamepadVibrationEmission.Values values = new GamepadVibrationMixer.GamepadVibrationEmission.Values
		{
			Small = 0f,
			Large = 0f
		};
		if (!this.isPaused)
		{
			this.playingEmissions.RemoveAll((GamepadVibrationMixer.GamepadVibrationEmission emission) => !emission.IsPlaying);
			foreach (GamepadVibrationMixer.GamepadVibrationEmission gamepadVibrationEmission in this.playingEmissions)
			{
				if (gamepadVibrationEmission.IsRealtime || Time.timeScale > Mathf.Epsilon)
				{
					GamepadVibrationMixer.GamepadVibrationEmission.Values values2 = gamepadVibrationEmission.GetCurrentValues();
					values.Small = this.AdjustForPlatform(Mathf.Max(values.Small, values2.Small));
					values.Large = this.AdjustForPlatform(Mathf.Max(values.Large, values2.Large));
					gamepadVibrationEmission.Advance(deltaTime);
				}
			}
		}
		this.currentValues = values;
	}

	// Token: 0x06002759 RID: 10073 RVA: 0x000B0A4C File Offset: 0x000AEC4C
	private float AdjustForPlatform(float val)
	{
		if (this.platformAdjustment == GamepadVibrationMixer.PlatformAdjustments.DualShock && GamepadVibrationMixer.AllowPlatformAdjustment)
		{
			float b = Mathf.Clamp01(Mathf.Sin(val * 3.1415927f * 0.5f));
			val = Mathf.Lerp(val, b, GamepadVibrationMixer.AdjustmentFactor);
		}
		return val * VibrationManager.StrengthMultiplier;
	}

	// Token: 0x04002425 RID: 9253
	private bool isPaused;

	// Token: 0x04002426 RID: 9254
	private List<GamepadVibrationMixer.GamepadVibrationEmission> playingEmissions;

	// Token: 0x04002427 RID: 9255
	private GamepadVibrationMixer.PlatformAdjustments platformAdjustment;

	// Token: 0x04002428 RID: 9256
	private GamepadVibrationMixer.GamepadVibrationEmission.Values currentValues;

	// Token: 0x02001756 RID: 5974
	public sealed class GamepadVibrationEmission : VibrationEmission
	{
		// Token: 0x17000F0F RID: 3855
		// (get) Token: 0x06008D5B RID: 36187 RVA: 0x0028997D File Offset: 0x00287B7D
		// (set) Token: 0x06008D5C RID: 36188 RVA: 0x00289985 File Offset: 0x00287B85
		public override bool IsLooping
		{
			get
			{
				return this.isLooping;
			}
			set
			{
				this.isLooping = value;
			}
		}

		// Token: 0x17000F10 RID: 3856
		// (get) Token: 0x06008D5D RID: 36189 RVA: 0x0028998E File Offset: 0x00287B8E
		public override bool IsPlaying
		{
			get
			{
				return this.isPlaying;
			}
		}

		// Token: 0x17000F11 RID: 3857
		// (get) Token: 0x06008D5E RID: 36190 RVA: 0x00289996 File Offset: 0x00287B96
		public override bool IsRealtime
		{
			get
			{
				return this.isRealtime;
			}
		}

		// Token: 0x17000F12 RID: 3858
		// (get) Token: 0x06008D5F RID: 36191 RVA: 0x0028999E File Offset: 0x00287B9E
		// (set) Token: 0x06008D60 RID: 36192 RVA: 0x002899A6 File Offset: 0x00287BA6
		public override string Tag
		{
			get
			{
				return this.tag;
			}
			set
			{
				this.tag = value;
			}
		}

		// Token: 0x17000F13 RID: 3859
		// (get) Token: 0x06008D61 RID: 36193 RVA: 0x002899AF File Offset: 0x00287BAF
		// (set) Token: 0x06008D62 RID: 36194 RVA: 0x002899B7 File Offset: 0x00287BB7
		public override VibrationTarget Target
		{
			get
			{
				return this.target;
			}
			set
			{
				this.target = value;
			}
		}

		// Token: 0x17000F14 RID: 3860
		// (get) Token: 0x06008D63 RID: 36195 RVA: 0x002899C0 File Offset: 0x00287BC0
		// (set) Token: 0x06008D64 RID: 36196 RVA: 0x002899C8 File Offset: 0x00287BC8
		public override float Time
		{
			get
			{
				return this.timer;
			}
			set
			{
				this.timer = value;
			}
		}

		// Token: 0x06008D65 RID: 36197 RVA: 0x002899D4 File Offset: 0x00287BD4
		public GamepadVibrationEmission(GamepadVibrationMixer mixer, GamepadVibration gamepadVibration, bool isLooping, string tag, VibrationTarget target, bool isRealtime, float strength)
		{
			this.mixer = mixer;
			this.gamepadVibration = gamepadVibration;
			this.duration = gamepadVibration.GetDuration();
			this.isLooping = isLooping;
			this.isPlaying = true;
			this.isRealtime = isRealtime;
			this.tag = tag;
			this.target = target;
			this.BaseStrength = strength;
		}

		// Token: 0x06008D66 RID: 36198 RVA: 0x00289A2F File Offset: 0x00287C2F
		public override void Play()
		{
			this.isPlaying = true;
		}

		// Token: 0x06008D67 RID: 36199 RVA: 0x00289A38 File Offset: 0x00287C38
		public override void Stop()
		{
			this.isPlaying = false;
		}

		// Token: 0x06008D68 RID: 36200 RVA: 0x00289A44 File Offset: 0x00287C44
		public GamepadVibrationMixer.GamepadVibrationEmission.Values GetCurrentValues()
		{
			return new GamepadVibrationMixer.GamepadVibrationEmission.Values
			{
				Small = ((this.target.Motors != VibrationMotors.None) ? (this.gamepadVibration.SmallMotor.Evaluate(this.timer) * this.Strength) : 0f),
				Large = ((this.target.Motors != VibrationMotors.None) ? (this.gamepadVibration.LargeMotor.Evaluate(this.timer) * this.Strength) : 0f)
			};
		}

		// Token: 0x06008D69 RID: 36201 RVA: 0x00289ACC File Offset: 0x00287CCC
		public void Advance(float deltaTime)
		{
			this.timer += deltaTime * this.gamepadVibration.PlaybackRate * this.Speed;
			if (this.timer >= this.duration)
			{
				if (this.isLooping)
				{
					this.timer = Mathf.Repeat(this.timer, this.duration);
					return;
				}
				this.isPlaying = false;
			}
		}

		// Token: 0x06008D6A RID: 36202 RVA: 0x00289B2F File Offset: 0x00287D2F
		public override string ToString()
		{
			if (!(this.gamepadVibration != null))
			{
				return "null";
			}
			return this.gamepadVibration.name;
		}

		// Token: 0x04008DE5 RID: 36325
		private GamepadVibrationMixer mixer;

		// Token: 0x04008DE6 RID: 36326
		private GamepadVibration gamepadVibration;

		// Token: 0x04008DE7 RID: 36327
		private float duration;

		// Token: 0x04008DE8 RID: 36328
		private bool isLooping;

		// Token: 0x04008DE9 RID: 36329
		private bool isPlaying;

		// Token: 0x04008DEA RID: 36330
		private bool isRealtime;

		// Token: 0x04008DEB RID: 36331
		private string tag;

		// Token: 0x04008DEC RID: 36332
		private VibrationTarget target;

		// Token: 0x04008DED RID: 36333
		private float timer;

		// Token: 0x02001C19 RID: 7193
		public struct Values
		{
			// Token: 0x170011C4 RID: 4548
			// (get) Token: 0x06009AD5 RID: 39637 RVA: 0x002B46AC File Offset: 0x002B28AC
			public bool IsNearlyZero
			{
				get
				{
					return this.Small < Mathf.Epsilon && this.Large < Mathf.Epsilon;
				}
			}

			// Token: 0x0400A000 RID: 40960
			public float Small;

			// Token: 0x0400A001 RID: 40961
			public float Large;
		}
	}

	// Token: 0x02001757 RID: 5975
	public enum PlatformAdjustments
	{
		// Token: 0x04008DEF RID: 36335
		None,
		// Token: 0x04008DF0 RID: 36336
		DualShock
	}
}
