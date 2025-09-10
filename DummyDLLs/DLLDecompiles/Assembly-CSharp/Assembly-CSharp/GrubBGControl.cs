using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004F2 RID: 1266
public class GrubBGControl : MonoBehaviour
{
	// Token: 0x06002D5B RID: 11611 RVA: 0x000C608C File Offset: 0x000C428C
	private void Awake()
	{
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06002D5C RID: 11612 RVA: 0x000C609C File Offset: 0x000C429C
	private void Start()
	{
		int playerDataInt = GameManager.instance.GetPlayerDataInt("grubsCollected");
		if (this.grubNumber > playerDataInt)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.idleRoutine = base.StartCoroutine(this.Idle());
		if (this.waveRegion)
		{
			this.waveRegion.OnTriggerEntered += delegate(Collider2D col, GameObject sender)
			{
				if (this.waveRoutine == null)
				{
					this.waveRoutine = base.StartCoroutine(this.Wave());
				}
			};
		}
	}

	// Token: 0x06002D5D RID: 11613 RVA: 0x000C6105 File Offset: 0x000C4305
	private IEnumerator Idle()
	{
		this.anim.Play("Home Bounce");
		for (;;)
		{
			yield return new WaitForSeconds(Random.Range(3f, 10f));
			this.idleSounds.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		yield break;
	}

	// Token: 0x06002D5E RID: 11614 RVA: 0x000C6114 File Offset: 0x000C4314
	private IEnumerator Wave()
	{
		if (this.idleRoutine != null)
		{
			base.StopCoroutine(this.idleRoutine);
		}
		Vector3 position = base.transform.position;
		position.z = 0f;
		this.waveSounds.SpawnAndPlayOneShot(this.audioSourcePrefab, position, null);
		yield return base.StartCoroutine(this.anim.PlayAnimWait("Home Wave", null));
		this.waveRoutine = null;
		this.idleRoutine = base.StartCoroutine(this.Idle());
		yield break;
	}

	// Token: 0x04002F0B RID: 12043
	public int grubNumber;

	// Token: 0x04002F0C RID: 12044
	[Space]
	public TriggerEnterEvent waveRegion;

	// Token: 0x04002F0D RID: 12045
	[Space]
	public AudioSource audioSourcePrefab;

	// Token: 0x04002F0E RID: 12046
	public AudioEventRandom idleSounds;

	// Token: 0x04002F0F RID: 12047
	public AudioEventRandom waveSounds;

	// Token: 0x04002F10 RID: 12048
	private Coroutine idleRoutine;

	// Token: 0x04002F11 RID: 12049
	private Coroutine waveRoutine;

	// Token: 0x04002F12 RID: 12050
	private tk2dSpriteAnimator anim;
}
