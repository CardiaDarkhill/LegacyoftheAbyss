using System;
using System.Collections;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200058A RID: 1418
public class WeaverSpeedPanel : MonoBehaviour
{
	// Token: 0x1400009E RID: 158
	// (add) Token: 0x060032BC RID: 12988 RVA: 0x000E1D0C File Offset: 0x000DFF0C
	// (remove) Token: 0x060032BD RID: 12989 RVA: 0x000E1D44 File Offset: 0x000DFF44
	public event Action<int> RecordedSpeedThreshold;

	// Token: 0x17000569 RID: 1385
	// (get) Token: 0x060032BE RID: 12990 RVA: 0x000E1D79 File Offset: 0x000DFF79
	// (set) Token: 0x060032BF RID: 12991 RVA: 0x000E1D81 File Offset: 0x000DFF81
	public bool ForceStayLit { get; set; }

	// Token: 0x060032C0 RID: 12992 RVA: 0x000E1D8C File Offset: 0x000DFF8C
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<int>(ref this.heroStateLightThresholds, typeof(WeaverSpeedPanel.HeroStateSpeeds));
		for (int i = 0; i < this.heroStateLightThresholds.Length; i++)
		{
			int num = this.heroStateLightThresholds[i];
			if (num > this.lights.Length)
			{
				this.heroStateLightThresholds[i] = this.lights.Length;
			}
			else if (num < 0)
			{
				this.heroStateLightThresholds[i] = 0;
			}
		}
	}

	// Token: 0x060032C1 RID: 12993 RVA: 0x000E1DF4 File Offset: 0x000DFFF4
	private void Awake()
	{
		this.OnValidate();
		NestedFadeGroupBase[] array = this.lights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].AlphaSelf = 0f;
		}
	}

	// Token: 0x060032C2 RID: 12994 RVA: 0x000E1E29 File Offset: 0x000E0029
	private void OnEnable()
	{
		if (WeaverSpeedPanel._activePanels == null)
		{
			WeaverSpeedPanel._activePanels = new List<WeaverSpeedPanel>();
		}
		WeaverSpeedPanel._activePanels.Add(this);
	}

	// Token: 0x060032C3 RID: 12995 RVA: 0x000E1E47 File Offset: 0x000E0047
	private void OnDisable()
	{
		WeaverSpeedPanel._activePanels.Remove(this);
		if (WeaverSpeedPanel._activePanels.Count == 0)
		{
			WeaverSpeedPanel._activePanels = null;
		}
	}

	// Token: 0x060032C4 RID: 12996 RVA: 0x000E1E68 File Offset: 0x000E0068
	public void RecordSpeed()
	{
		HeroController instance = HeroController.instance;
		Vector2 linearVelocity = instance.Body.linearVelocity;
		if (this.lightRoutine != null)
		{
			base.StopCoroutine(this.lightRoutine);
			NestedFadeGroupBase[] array = this.lights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FadeToZero(this.lightDownDuration);
			}
		}
		float num = Mathf.Abs(linearVelocity.x) / (this.maxSpeed - Mathf.Epsilon);
		if (num > 1f)
		{
			num = 1f;
		}
		int num2 = Mathf.FloorToInt(num * (float)this.lights.Length);
		bool isUsingQuickening = instance.IsUsingQuickening;
		bool isEquipped = Gameplay.SprintmasterTool.IsEquipped;
		WeaverSpeedPanel.HeroStateSpeeds heroStateSpeeds;
		if (isUsingQuickening && isEquipped)
		{
			heroStateSpeeds = WeaverSpeedPanel.HeroStateSpeeds.WithQuickeningAndSprintCharm;
		}
		else if (isUsingQuickening)
		{
			heroStateSpeeds = WeaverSpeedPanel.HeroStateSpeeds.WithQuickening;
		}
		else if (isEquipped)
		{
			heroStateSpeeds = WeaverSpeedPanel.HeroStateSpeeds.WithSprintCharm;
		}
		else
		{
			heroStateSpeeds = WeaverSpeedPanel.HeroStateSpeeds.Normal;
		}
		int num3 = this.heroStateLightThresholds[(int)heroStateSpeeds];
		if (num2 > num3)
		{
			num2 = num3;
		}
		else if (num2 < 1)
		{
			num2 = 1;
		}
		Action<int> recordedSpeedThreshold = this.RecordedSpeedThreshold;
		if (recordedSpeedThreshold != null)
		{
			recordedSpeedThreshold(num2);
		}
		this.lightRoutine = base.StartCoroutine(this.Light(num2));
	}

	// Token: 0x060032C5 RID: 12997 RVA: 0x000E1F73 File Offset: 0x000E0173
	private IEnumerator Light(int threshold)
	{
		Vector3 pos = base.transform.position;
		pos.z = 0f;
		WaitForSeconds wait = new WaitForSeconds(this.lightUpDelay);
		int num2;
		for (int i = 0; i < threshold; i = num2 + 1)
		{
			NestedFadeGroupBase j = this.lights[i];
			yield return wait;
			j.FadeToOne(this.lightUpDuration);
			if (this.lightSound)
			{
				float num = this.lightSoundPitchBase + this.lightSoundPitchIncrease * (float)i;
				new AudioEvent
				{
					Clip = this.lightSound,
					PitchMin = num,
					PitchMax = num,
					Volume = 1f
				}.SpawnAndPlayOneShot(pos, null);
			}
			j = null;
			num2 = i;
		}
		wait = null;
		this.holdTimeLeft = this.litHoldTime;
		for (;;)
		{
			yield return null;
			this.holdTimeLeft -= Time.deltaTime;
			if (this.holdTimeLeft <= 0f && !this.ForceStayLit)
			{
				bool flag = false;
				using (List<WeaverSpeedPanel>.Enumerator enumerator = WeaverSpeedPanel._activePanels.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.holdTimeLeft > 0f)
						{
							flag = true;
							break;
						}
					}
				}
				if (!flag)
				{
					break;
				}
			}
		}
		Action<int> recordedSpeedThreshold = this.RecordedSpeedThreshold;
		if (recordedSpeedThreshold != null)
		{
			recordedSpeedThreshold(0);
		}
		wait = new WaitForSeconds(this.lightDownDelay);
		for (int i = threshold - 1; i >= 0; i = num2 - 1)
		{
			NestedFadeGroupBase j = this.lights[i];
			yield return wait;
			j.FadeToZero(this.lightDownDuration);
			j = null;
			num2 = i;
		}
		wait = null;
		this.lightRoutine = null;
		yield break;
	}

	// Token: 0x040036A6 RID: 13990
	[SerializeField]
	private NestedFadeGroupBase[] lights;

	// Token: 0x040036A7 RID: 13991
	[SerializeField]
	private float lightUpDelay;

	// Token: 0x040036A8 RID: 13992
	[SerializeField]
	private float lightUpDuration;

	// Token: 0x040036A9 RID: 13993
	[SerializeField]
	private float lightDownDelay;

	// Token: 0x040036AA RID: 13994
	[SerializeField]
	private float lightDownDuration;

	// Token: 0x040036AB RID: 13995
	[SerializeField]
	private float litHoldTime;

	// Token: 0x040036AC RID: 13996
	[Space]
	[SerializeField]
	private AudioClip lightSound;

	// Token: 0x040036AD RID: 13997
	[SerializeField]
	private float lightSoundPitchBase;

	// Token: 0x040036AE RID: 13998
	[SerializeField]
	private float lightSoundPitchIncrease;

	// Token: 0x040036AF RID: 13999
	[Space]
	[SerializeField]
	private float maxSpeed;

	// Token: 0x040036B0 RID: 14000
	[SerializeField]
	[ArrayForEnum(typeof(WeaverSpeedPanel.HeroStateSpeeds))]
	private int[] heroStateLightThresholds;

	// Token: 0x040036B1 RID: 14001
	private Coroutine lightRoutine;

	// Token: 0x040036B2 RID: 14002
	private float holdTimeLeft;

	// Token: 0x040036B3 RID: 14003
	private static List<WeaverSpeedPanel> _activePanels;

	// Token: 0x020018A1 RID: 6305
	private enum HeroStateSpeeds
	{
		// Token: 0x040092CB RID: 37579
		Normal,
		// Token: 0x040092CC RID: 37580
		WithQuickening,
		// Token: 0x040092CD RID: 37581
		WithSprintCharm,
		// Token: 0x040092CE RID: 37582
		WithQuickeningAndSprintCharm
	}
}
