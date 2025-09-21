using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000764 RID: 1892
public class GameplayTimer : MonoBehaviour
{
	// Token: 0x17000790 RID: 1936
	// (get) Token: 0x0600436E RID: 17262 RVA: 0x00128BCD File Offset: 0x00126DCD
	public bool IsTimerComplete
	{
		get
		{
			return this.timeLeft <= 0f;
		}
	}

	// Token: 0x0600436F RID: 17263 RVA: 0x00128BDF File Offset: 0x00126DDF
	private void OnEnable()
	{
		this.gm = GameManager.instance;
	}

	// Token: 0x06004370 RID: 17264 RVA: 0x00128BEC File Offset: 0x00126DEC
	private void Update()
	{
		if (this.timeLeft <= 0f)
		{
			return;
		}
		if (this.gm.GameState != GameState.PLAYING)
		{
			if (!this.wasPaused)
			{
				this.SetPaused(true);
			}
			return;
		}
		if (this.wasPaused)
		{
			this.SetPaused(false);
		}
		this.timeLeft -= Time.deltaTime;
	}

	// Token: 0x06004371 RID: 17265 RVA: 0x00128C4C File Offset: 0x00126E4C
	private void SetPaused(bool paused)
	{
		foreach (Animator animator in this.pauseAnimators)
		{
			if (animator)
			{
				animator.speed = (float)(paused ? 0 : 1);
			}
		}
		foreach (ParticleSystem particleSystem in this.pauseParticleSystems)
		{
			if (paused && particleSystem.isPlaying)
			{
				particleSystem.Pause(true);
			}
			else if (particleSystem.isPaused)
			{
				particleSystem.Play(true);
			}
		}
		this.wasPaused = paused;
	}

	// Token: 0x06004372 RID: 17266 RVA: 0x00128CD0 File Offset: 0x00126ED0
	public void StartTimer(float time)
	{
		this.timeLeft = time;
	}

	// Token: 0x04004512 RID: 17682
	[SerializeField]
	private Animator[] pauseAnimators;

	// Token: 0x04004513 RID: 17683
	[SerializeField]
	private ParticleSystem[] pauseParticleSystems;

	// Token: 0x04004514 RID: 17684
	private float timeLeft;

	// Token: 0x04004515 RID: 17685
	private bool wasPaused;

	// Token: 0x04004516 RID: 17686
	private GameManager gm;
}
