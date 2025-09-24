using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x0200009B RID: 155
public class PlayableDirectorRamp : SpeedChanger
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x060004D4 RID: 1236 RVA: 0x00019866 File Offset: 0x00017A66
	// (set) Token: 0x060004D5 RID: 1237 RVA: 0x0001988A File Offset: 0x00017A8A
	private double DirectorTime
	{
		get
		{
			if (!this.director)
			{
				return 0.0;
			}
			return this.director.time;
		}
		set
		{
			if (!this.director)
			{
				return;
			}
			this.director.time = value;
			this.director.Evaluate();
		}
	}

	// Token: 0x060004D6 RID: 1238 RVA: 0x000198B1 File Offset: 0x00017AB1
	private void OnEnable()
	{
		this.runRoutine = base.StartCoroutine(this.RunPlayable());
	}

	// Token: 0x060004D7 RID: 1239 RVA: 0x000198C5 File Offset: 0x00017AC5
	[ContextMenu("Play", true)]
	[ContextMenu("Stop", true)]
	[ContextMenu("Reset", true)]
	public bool CanPlay()
	{
		return Application.isPlaying && this.director;
	}

	// Token: 0x060004D8 RID: 1240 RVA: 0x000198DB File Offset: 0x00017ADB
	[ContextMenu("Play", false, 0)]
	public void Play()
	{
		this.isRunning = true;
	}

	// Token: 0x060004D9 RID: 1241 RVA: 0x000198E4 File Offset: 0x00017AE4
	public void Play(bool isReversed)
	{
		this.isRunning = true;
		this.isReversed = isReversed;
	}

	// Token: 0x060004DA RID: 1242 RVA: 0x000198F4 File Offset: 0x00017AF4
	[ContextMenu("Stop", false, 1)]
	public void Stop()
	{
		this.isRunning = false;
	}

	// Token: 0x060004DB RID: 1243 RVA: 0x00019900 File Offset: 0x00017B00
	[ContextMenu("Reset", false, 2)]
	public void ResetDirector()
	{
		if (!this.director)
		{
			return;
		}
		this.director.enabled = true;
		this.director.timeUpdateMode = DirectorUpdateMode.Manual;
		this.DirectorTime = (double)this.startTime;
		this.SendSpeedChanged(0f);
		if (this.OnReset != null)
		{
			this.OnReset.Invoke();
		}
	}

	// Token: 0x060004DC RID: 1244 RVA: 0x0001995E File Offset: 0x00017B5E
	private IEnumerator RunPlayable()
	{
		yield return null;
		this.ResetDirector();
		for (;;)
		{
			if (this.isRunning)
			{
				if (this.OnPlayed != null)
				{
					this.OnPlayed.Invoke();
				}
				WaitForSeconds wait;
				if (this.fpsLimit > 0f)
				{
					wait = new WaitForSeconds(1f / this.fpsLimit);
				}
				else
				{
					wait = null;
				}
				float elapsed = 0f;
				float num = 0f;
				while (elapsed <= this.rampUpDuration)
				{
					float num2 = elapsed / this.rampUpDuration;
					this.DirectorTime += (double)(Mathf.Lerp(0f, num, num2) * (float)(this.isReversed ? -1 : 1));
					this.SendSpeedChanged(num2);
					double previousTime = Time.timeAsDouble;
					yield return wait;
					num = (float)(Time.timeAsDouble - previousTime);
					elapsed += num;
				}
				this.SendSpeedChanged(1f);
				elapsed = 0f;
				num = 0f;
				while (this.isRunning && (this.loopDuration == 0f || elapsed <= this.loopDuration))
				{
					this.DirectorTime += (double)(num * (float)(this.isReversed ? -1 : 1));
					double previousTime = Time.timeAsDouble;
					yield return wait;
					num = (float)(Time.timeAsDouble - previousTime);
					elapsed += num;
				}
				this.isRunning = false;
				if (this.OnStopping != null)
				{
					this.OnStopping.Invoke();
				}
				elapsed = 0f;
				num = 0f;
				while (elapsed <= this.rampDownDuration)
				{
					float num3 = elapsed / this.rampDownDuration;
					this.DirectorTime += (double)(Mathf.Lerp(num, 0f, num3) * (float)(this.isReversed ? -1 : 1));
					this.SendSpeedChanged(1f - num3);
					double previousTime = Time.timeAsDouble;
					yield return wait;
					num = (float)(Time.timeAsDouble - previousTime);
					elapsed += num;
				}
				this.SendSpeedChanged(0f);
				if (this.OnStopped != null)
				{
					this.OnStopped.Invoke();
				}
				wait = null;
			}
			else
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x060004DD RID: 1245 RVA: 0x0001996D File Offset: 0x00017B6D
	private void SendSpeedChanged(float speed)
	{
		if (this.OnSpeedChanged != null)
		{
			this.OnSpeedChanged.Invoke(speed);
		}
		base.CallSpeedChangedEvent(speed);
	}

	// Token: 0x040004A6 RID: 1190
	[SerializeField]
	private PlayableDirector director;

	// Token: 0x040004A7 RID: 1191
	[SerializeField]
	private float startTime;

	// Token: 0x040004A8 RID: 1192
	[SerializeField]
	private float loopDuration;

	// Token: 0x040004A9 RID: 1193
	[SerializeField]
	private float rampUpDuration;

	// Token: 0x040004AA RID: 1194
	[SerializeField]
	private float rampDownDuration;

	// Token: 0x040004AB RID: 1195
	[Space]
	[SerializeField]
	private float fpsLimit;

	// Token: 0x040004AC RID: 1196
	[Space]
	public UnityEvent OnPlayed;

	// Token: 0x040004AD RID: 1197
	public PlayableDirectorRamp.UnityFloatEvent OnSpeedChanged;

	// Token: 0x040004AE RID: 1198
	public UnityEvent OnStopping;

	// Token: 0x040004AF RID: 1199
	public UnityEvent OnStopped;

	// Token: 0x040004B0 RID: 1200
	public UnityEvent OnReset;

	// Token: 0x040004B1 RID: 1201
	private bool isRunning;

	// Token: 0x040004B2 RID: 1202
	private bool isReversed;

	// Token: 0x040004B3 RID: 1203
	private Coroutine runRoutine;

	// Token: 0x02001412 RID: 5138
	[Serializable]
	public class UnityFloatEvent : UnityEvent<float>
	{
	}
}
