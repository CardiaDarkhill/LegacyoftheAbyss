using System;
using UnityEngine;

// Token: 0x0200032B RID: 811
public class ZapGrassCluster : MonoBehaviour
{
	// Token: 0x06001C75 RID: 7285 RVA: 0x0008486C File Offset: 0x00082A6C
	private void Start()
	{
		this.ptAttack.emission.enabled = false;
		foreach (object obj in this.grassL)
		{
			Transform transform = (Transform)obj;
			tk2dSpriteAnimator component = transform.gameObject.GetComponent<tk2dSpriteAnimator>();
			string name = this.anim_IdleLeft[Random.Range(0, 3)];
			component.Play(name);
			float num = Random.Range(1f, 1.2f);
			transform.localScale = new Vector3(num, num, num);
		}
		foreach (object obj2 in this.grassR)
		{
			Transform transform2 = (Transform)obj2;
			tk2dSpriteAnimator component2 = transform2.gameObject.GetComponent<tk2dSpriteAnimator>();
			string name2 = this.anim_IdleRight[Random.Range(0, 3)];
			component2.Play(name2);
			float num2 = Random.Range(1f, 1.2f);
			transform2.localScale = new Vector3(num2, num2, num2);
		}
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06001C76 RID: 7286 RVA: 0x000849A4 File Offset: 0x00082BA4
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.collisionTracker.InsideCount >= 1)
		{
			this.inRange = true;
		}
	}

	// Token: 0x06001C77 RID: 7287 RVA: 0x000849BB File Offset: 0x00082BBB
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.collisionTracker.InsideCount <= 0)
		{
			this.inRange = false;
		}
	}

	// Token: 0x06001C78 RID: 7288 RVA: 0x000849D4 File Offset: 0x00082BD4
	private void Update()
	{
		if (this.inRange && !this.zapping)
		{
			this.StartZapping();
		}
		else if (!this.inRange && this.zapping && this.rangeTimer <= 0f)
		{
			this.StopZapping();
		}
		if (this.inRange && this.rangeTimer != 1f)
		{
			this.rangeTimer = 0.25f;
		}
		if (!this.inRange && this.rangeTimer > 0f)
		{
			this.rangeTimer -= Time.deltaTime;
		}
	}

	// Token: 0x06001C79 RID: 7289 RVA: 0x00084A64 File Offset: 0x00082C64
	private void StartZapping()
	{
		this.zapping = true;
		this.ptAttack.emission.enabled = true;
		foreach (object obj in this.grassL)
		{
			((Transform)obj).gameObject.GetComponent<tk2dSpriteAnimator>().Play("Zap L");
		}
		foreach (object obj2 in this.grassR)
		{
			((Transform)obj2).gameObject.GetComponent<tk2dSpriteAnimator>().Play("Zap R");
		}
		this.audioSource.Play();
		this.attackLight.Fade(true);
	}

	// Token: 0x06001C7A RID: 7290 RVA: 0x00084B50 File Offset: 0x00082D50
	private void StopZapping()
	{
		this.zapping = false;
		this.ptAttack.emission.enabled = false;
		foreach (object obj in this.grassL)
		{
			tk2dSpriteAnimator component = ((Transform)obj).gameObject.GetComponent<tk2dSpriteAnimator>();
			string name = this.anim_IdleLeft[Random.Range(0, 3)];
			component.Play(name);
		}
		foreach (object obj2 in this.grassR)
		{
			tk2dSpriteAnimator component2 = ((Transform)obj2).gameObject.GetComponent<tk2dSpriteAnimator>();
			string name2 = this.anim_IdleRight[Random.Range(0, 3)];
			component2.Play(name2);
		}
		this.audioSource.Stop();
		this.attackLight.Fade(false);
	}

	// Token: 0x04001B99 RID: 7065
	public Transform grassL;

	// Token: 0x04001B9A RID: 7066
	public Transform grassR;

	// Token: 0x04001B9B RID: 7067
	public TrackTriggerObjects collisionTracker;

	// Token: 0x04001B9C RID: 7068
	public ParticleSystem ptIdle;

	// Token: 0x04001B9D RID: 7069
	public ParticleSystem ptAttack;

	// Token: 0x04001B9E RID: 7070
	public ColorFader attackLight;

	// Token: 0x04001B9F RID: 7071
	private AudioSource audioSource;

	// Token: 0x04001BA0 RID: 7072
	private bool inRange;

	// Token: 0x04001BA1 RID: 7073
	private bool zapping;

	// Token: 0x04001BA2 RID: 7074
	private float rangeTimer;

	// Token: 0x04001BA3 RID: 7075
	private string[] anim_IdleLeft = new string[]
	{
		"Idle L1",
		"Idle L2",
		"Idle L3",
		"Idle L4"
	};

	// Token: 0x04001BA4 RID: 7076
	private string[] anim_IdleRight = new string[]
	{
		"Idle R1",
		"Idle R2",
		"Idle R3",
		"Idle R4"
	};
}
