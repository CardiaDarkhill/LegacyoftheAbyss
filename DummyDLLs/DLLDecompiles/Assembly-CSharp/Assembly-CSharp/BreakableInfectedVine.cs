using System;
using UnityEngine;

// Token: 0x020004A4 RID: 1188
public class BreakableInfectedVine : MonoBehaviour
{
	// Token: 0x06002B2A RID: 11050 RVA: 0x000BCE74 File Offset: 0x000BB074
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.vibration = base.GetComponent<VibrationPlayer>();
	}

	// Token: 0x06002B2B RID: 11051 RVA: 0x000BCE90 File Offset: 0x000BB090
	private void Start()
	{
		if (Mathf.Abs(base.transform.position.z - 0.004f) > 1f)
		{
			if (this.source)
			{
				this.source.enabled = false;
			}
			Collider2D component = base.GetComponent<Collider2D>();
			if (component)
			{
				component.enabled = false;
			}
			base.enabled = false;
		}
	}

	// Token: 0x06002B2C RID: 11052 RVA: 0x000BCEF8 File Offset: 0x000BB0F8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.activated)
		{
			return;
		}
		bool flag = false;
		if (collision.tag == "Nail Attack")
		{
			flag = true;
		}
		else if (collision.tag == "Hero Spell")
		{
			flag = true;
		}
		else if (collision.tag == "HeroBox" && HeroController.instance.cState.superDashing)
		{
			flag = true;
		}
		if (flag)
		{
			foreach (GameObject gameObject in this.blobs)
			{
				gameObject.SetActive(false);
				this.SpawnSpatters(gameObject.transform.position);
			}
			GameObject[] array = this.effects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(true);
			}
			if (this.source)
			{
				this.source.pitch = Random.Range(this.audioPitchMin, this.audioPitchMax);
				this.source.Play();
			}
			if (this.vibration)
			{
				this.vibration.Play();
			}
			this.activated = true;
		}
	}

	// Token: 0x06002B2D RID: 11053 RVA: 0x000BD00C File Offset: 0x000BB20C
	private void SpawnSpatters(Vector3 position)
	{
		BloodSpawner.SpawnBlood(position, (short)this.spatterAmount, (short)this.spatterAmount, this.spatterSpeedMin, this.spatterSpeedMax, this.spatterAngleMin, this.spatterAngleMax, null, 0f);
	}

	// Token: 0x04002C55 RID: 11349
	public GameObject[] blobs;

	// Token: 0x04002C56 RID: 11350
	[Space]
	public GameObject[] effects;

	// Token: 0x04002C57 RID: 11351
	[Space]
	public int spatterAmount = 5;

	// Token: 0x04002C58 RID: 11352
	public float spatterSpeedMin = 10f;

	// Token: 0x04002C59 RID: 11353
	public float spatterSpeedMax = 20f;

	// Token: 0x04002C5A RID: 11354
	public float spatterAngleMin = 40f;

	// Token: 0x04002C5B RID: 11355
	public float spatterAngleMax = 140f;

	// Token: 0x04002C5C RID: 11356
	[Space]
	public float audioPitchMin = 0.8f;

	// Token: 0x04002C5D RID: 11357
	public float audioPitchMax = 1.1f;

	// Token: 0x04002C5E RID: 11358
	private bool activated;

	// Token: 0x04002C5F RID: 11359
	private AudioSource source;

	// Token: 0x04002C60 RID: 11360
	private VibrationPlayer vibration;
}
