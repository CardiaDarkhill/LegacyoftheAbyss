using System;
using UnityEngine;

// Token: 0x020000AF RID: 175
public class SilkPossession : MonoBehaviour
{
	// Token: 0x06000526 RID: 1318 RVA: 0x0001A87B File Offset: 0x00018A7B
	private void Awake()
	{
		this.audioSource = base.gameObject.GetComponent<AudioSource>();
	}

	// Token: 0x06000527 RID: 1319 RVA: 0x0001A890 File Offset: 0x00018A90
	private void OnEnable()
	{
		if (Random.Range(1, 100) > 50)
		{
			base.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			base.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		this.offset_y = 0f;
		this.possessEffect.SetActive(false);
		this.possessAnim.SetActive(false);
		this.idleAnim.SetActive(true);
		this.audioSource.pitch = Random.Range(0.85f, 1.15f);
		this.audioSource.Play();
	}

	// Token: 0x06000528 RID: 1320 RVA: 0x0001A93D File Offset: 0x00018B3D
	private void OnDisable()
	{
		this.possessedEnemy = null;
	}

	// Token: 0x06000529 RID: 1321 RVA: 0x0001A948 File Offset: 0x00018B48
	private void Update()
	{
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.possessEffect.SetActive(true);
			}
			base.transform.position = new Vector3(this.possessedEnemy.transform.position.x, this.possessedEnemy.transform.position.y + this.offset_y, base.transform.position.z);
			base.transform.parent = null;
		}
		if (this.possessedEnemy == null)
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x0600052A RID: 1322 RVA: 0x0001AA08 File Offset: 0x00018C08
	public void PlayPossess()
	{
		this.audioSource.Stop();
		this.idleAnim.SetActive(false);
		this.possessAnim.SetActive(true);
		this.possessionClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		this.timer = this.yankWait;
	}

	// Token: 0x0600052B RID: 1323 RVA: 0x0001AA5C File Offset: 0x00018C5C
	public void SetEnemy(GameObject enemy)
	{
		this.possessedEnemy = enemy;
	}

	// Token: 0x0600052C RID: 1324 RVA: 0x0001AA65 File Offset: 0x00018C65
	public void SetOffsetY(float newOffset)
	{
		this.offset_y = newOffset;
	}

	// Token: 0x04000502 RID: 1282
	public GameObject idleAnim;

	// Token: 0x04000503 RID: 1283
	public GameObject possessAnim;

	// Token: 0x04000504 RID: 1284
	public GameObject yankAnim;

	// Token: 0x04000505 RID: 1285
	public GameObject possessEffect;

	// Token: 0x04000506 RID: 1286
	public GameObject possessedEnemy;

	// Token: 0x04000507 RID: 1287
	public RandomAudioClipTable possessionClipTable;

	// Token: 0x04000508 RID: 1288
	public float yankWait;

	// Token: 0x04000509 RID: 1289
	public float offset_y;

	// Token: 0x0400050A RID: 1290
	private float timer;

	// Token: 0x0400050B RID: 1291
	private AudioSource audioSource;

	// Token: 0x0400050C RID: 1292
	private bool canResetEnemy;
}
