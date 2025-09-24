using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class InfectedBurstSmall : MonoBehaviour
{
	// Token: 0x06000495 RID: 1173 RVA: 0x00018AB2 File Offset: 0x00016CB2
	private void Awake()
	{
		this.vibration = base.GetComponent<VibrationPlayer>();
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00018AC0 File Offset: 0x00016CC0
	private void Start()
	{
		this.audioSource.pitch = Random.Range(0.8f, 1.2f);
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00018ADC File Offset: 0x00016CDC
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Nail Attack" || otherCollider.gameObject.tag == "Hero Spell" || (otherCollider.tag == "HeroBox" && HeroController.instance.cState.superDashing))
		{
			this.audioSource.Play();
			this.effects.SetActive(true);
			BloodSpawner.SpawnBlood(base.transform.position, 5, 5, 10f, 20f, 40f, 140f, null, 0f);
			this.spriteRenderer.enabled = false;
			this.animator.enabled = false;
			this.circleCollider.enabled = false;
			if (this.vibration)
			{
				this.vibration.Play();
			}
		}
	}

	// Token: 0x04000453 RID: 1107
	public AudioSource audioSource;

	// Token: 0x04000454 RID: 1108
	public GameObject effects;

	// Token: 0x04000455 RID: 1109
	public SpriteRenderer spriteRenderer;

	// Token: 0x04000456 RID: 1110
	public Animator animator;

	// Token: 0x04000457 RID: 1111
	public CircleCollider2D circleCollider;

	// Token: 0x04000458 RID: 1112
	private VibrationPlayer vibration;
}
