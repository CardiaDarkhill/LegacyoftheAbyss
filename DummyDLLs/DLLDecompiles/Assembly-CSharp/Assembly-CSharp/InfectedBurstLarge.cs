using System;
using UnityEngine;

// Token: 0x02000090 RID: 144
public class InfectedBurstLarge : MonoBehaviour
{
	// Token: 0x06000491 RID: 1169 RVA: 0x00018990 File Offset: 0x00016B90
	private void Awake()
	{
		this.vibration = base.GetComponent<VibrationPlayer>();
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x0001899E File Offset: 0x00016B9E
	private void Start()
	{
		this.audioSource.pitch = Random.Range(0.8f, 1.2f);
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x000189BC File Offset: 0x00016BBC
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Nail Attack" || otherCollider.gameObject.tag == "Hero Spell" || (otherCollider.tag == "HeroBox" && HeroController.instance.cState.superDashing))
		{
			this.audioSource.Play();
			this.effects.SetActive(true);
			BloodSpawner.SpawnBlood(base.transform.position, 15, 15, 10f, 20f, 40f, 140f, null, 0f);
			this.spriteRenderer.enabled = false;
			this.animator.enabled = false;
			this.circleCollider.enabled = false;
			if (this.vibration)
			{
				this.vibration.Play();
			}
		}
	}

	// Token: 0x0400044D RID: 1101
	public AudioSource audioSource;

	// Token: 0x0400044E RID: 1102
	public GameObject effects;

	// Token: 0x0400044F RID: 1103
	public SpriteRenderer spriteRenderer;

	// Token: 0x04000450 RID: 1104
	public Animator animator;

	// Token: 0x04000451 RID: 1105
	public CircleCollider2D circleCollider;

	// Token: 0x04000452 RID: 1106
	private VibrationPlayer vibration;
}
