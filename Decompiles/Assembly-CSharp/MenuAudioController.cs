using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000121 RID: 289
public class MenuAudioController : MonoBehaviour
{
	// Token: 0x060008E4 RID: 2276 RVA: 0x00029AD9 File Offset: 0x00027CD9
	private IEnumerator Start()
	{
		if (SceneManager.GetActiveScene().name == "Pre_Menu_Intro")
		{
			yield break;
		}
		float startVol = this.audioSource.volume;
		this.audioSource.volume = 0f;
		yield return GameManager.instance.timeTool.TimeScaleIndependentWaitForSeconds(1f);
		this.audioSource.volume = startVol;
		yield break;
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00029AE8 File Offset: 0x00027CE8
	public void PlaySelect()
	{
		if (this.select)
		{
			this.audioSource.PlayOneShot(this.select);
		}
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00029B08 File Offset: 0x00027D08
	public void PlaySubmit()
	{
		if (this.submit)
		{
			this.audioSource.PlayOneShot(this.submit);
		}
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00029B28 File Offset: 0x00027D28
	public void PlayCancel()
	{
		if (this.cancel)
		{
			this.audioSource.PlayOneShot(this.cancel);
		}
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x00029B48 File Offset: 0x00027D48
	public void PlaySlider()
	{
		if (this.slider)
		{
			this.audioSource.PlayOneShot(this.slider);
		}
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00029B68 File Offset: 0x00027D68
	public void PlayStartGame()
	{
		if (this.startGame)
		{
			this.audioSource.PlayOneShot(this.startGame);
		}
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00029B88 File Offset: 0x00027D88
	public void PlayOpenProfileSelect()
	{
		if (this.openProfileSelect)
		{
			this.audioSource.PlayOneShot(this.openProfileSelect);
		}
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00029BA8 File Offset: 0x00027DA8
	public void PlayPause()
	{
		if (this.pause)
		{
			this.audioSource.PlayOneShot(this.pause);
		}
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x00029BC8 File Offset: 0x00027DC8
	public void PlayUnpause()
	{
		if (this.unpause)
		{
			this.audioSource.PlayOneShot(this.unpause);
		}
	}

	// Token: 0x0400089D RID: 2205
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400089E RID: 2206
	[Header("Sound Effects")]
	[SerializeField]
	private AudioClip select;

	// Token: 0x0400089F RID: 2207
	[SerializeField]
	private AudioClip submit;

	// Token: 0x040008A0 RID: 2208
	[SerializeField]
	private AudioClip cancel;

	// Token: 0x040008A1 RID: 2209
	[SerializeField]
	private AudioClip slider;

	// Token: 0x040008A2 RID: 2210
	[SerializeField]
	private AudioClip startGame;

	// Token: 0x040008A3 RID: 2211
	[SerializeField]
	private AudioClip openProfileSelect;

	// Token: 0x040008A4 RID: 2212
	[Space]
	[SerializeField]
	private AudioClip pause;

	// Token: 0x040008A5 RID: 2213
	[SerializeField]
	private AudioClip unpause;
}
