using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000131 RID: 305
public class RegionSetAudio : MonoBehaviour
{
	// Token: 0x06000976 RID: 2422 RVA: 0x0002B91C File Offset: 0x00029B1C
	private void OnEnable()
	{
		HeroController hc = HeroController.instance;
		if (hc.isHeroInPosition)
		{
			this.SubscribeSceneManager();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate(bool direct)
		{
			this.SubscribeSceneManager();
			hc.heroInPosition -= temp;
		};
		hc.heroInPosition += temp;
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0002B97F File Offset: 0x00029B7F
	private void SubscribeSceneManager()
	{
		this.sm = GameManager.instance.sm;
		this.sm.AudioSnapshotsApplied += this.OnSceneManagerAppliedSnapshots;
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x0002B9A8 File Offset: 0x00029BA8
	private void UnsubscribeSceneManager()
	{
		if (this.sceneSetup)
		{
			return;
		}
		this.sm.AudioSnapshotsApplied -= this.OnSceneManagerAppliedSnapshots;
		this.sm = null;
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x0002B9D1 File Offset: 0x00029BD1
	private void OnSceneManagerAppliedSnapshots()
	{
		this.UnsubscribeSceneManager();
		this.sceneSetup = true;
		if (this.queueEnter)
		{
			this.DoEnter();
		}
	}

	// Token: 0x0600097A RID: 2426 RVA: 0x0002B9EE File Offset: 0x00029BEE
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		this.DoEnter();
	}

	// Token: 0x0600097B RID: 2427 RVA: 0x0002B9F6 File Offset: 0x00029BF6
	private void OnTriggerStay2D(Collider2D otherCollider)
	{
		this.DoEnter();
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x0002BA00 File Offset: 0x00029C00
	private void DoEnter()
	{
		if (!this.sceneSetup)
		{
			this.queueEnter = true;
			return;
		}
		if (this.entered)
		{
			return;
		}
		if (this.atmosSnapshotEnter != null)
		{
			AudioManager.TransitionToAtmosOverride(this.atmosSnapshotEnter, this.transitionTime);
		}
		if (this.enviroSnapshotEnter != null)
		{
			this.enviroSnapshotEnter.TransitionTo(this.transitionTime);
		}
		this.entered = true;
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x0002BA6C File Offset: 0x00029C6C
	private void OnTriggerExit2D(Collider2D otherCollider)
	{
		this.queueEnter = false;
		if (!this.entered)
		{
			return;
		}
		if (this.atmosSnapshotExit != null)
		{
			AudioManager.TransitionToAtmosOverride(this.atmosSnapshotExit, this.transitionTime);
		}
		if (this.enviroSnapshotExit != null)
		{
			this.enviroSnapshotExit.TransitionTo(this.transitionTime);
		}
		this.entered = false;
	}

	// Token: 0x0400091D RID: 2333
	[SerializeField]
	private AudioMixerSnapshot atmosSnapshotEnter;

	// Token: 0x0400091E RID: 2334
	[SerializeField]
	private AudioMixerSnapshot enviroSnapshotEnter;

	// Token: 0x0400091F RID: 2335
	[SerializeField]
	private AudioMixerSnapshot atmosSnapshotExit;

	// Token: 0x04000920 RID: 2336
	[SerializeField]
	private AudioMixerSnapshot enviroSnapshotExit;

	// Token: 0x04000921 RID: 2337
	public float transitionTime;

	// Token: 0x04000922 RID: 2338
	private CustomSceneManager sm;

	// Token: 0x04000923 RID: 2339
	private bool sceneSetup;

	// Token: 0x04000924 RID: 2340
	private bool entered;

	// Token: 0x04000925 RID: 2341
	private bool queueEnter;
}
