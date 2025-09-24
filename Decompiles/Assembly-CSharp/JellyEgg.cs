using System;
using UnityEngine;

// Token: 0x0200050E RID: 1294
public class JellyEgg : MonoBehaviour
{
	// Token: 0x06002E2F RID: 11823 RVA: 0x000CACAC File Offset: 0x000C8EAC
	private void OnTriggerEnter2D(Collider2D otherCollider)
	{
		if (otherCollider.gameObject.tag == "Nail Attack" || otherCollider.gameObject.tag == "Hero Spell" || otherCollider.gameObject.tag == "HeroBox")
		{
			this.Burst();
		}
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x000CAD04 File Offset: 0x000C8F04
	private void Burst()
	{
		this.meshRenderer.enabled = false;
		this.popEffect.Play();
		this.audioSource.Play();
		this.circleCollider.enabled = false;
		if (this.bomb)
		{
			this.explosionObject.Spawn(base.transform.position, base.transform.localRotation);
			return;
		}
		float num = Random.Range(1f, 1.5f);
		this.strikeEffect.transform.localScale = new Vector3(num, num, num);
		this.strikeEffect.transform.localEulerAngles = new Vector3(this.strikeEffect.transform.localEulerAngles.x, this.strikeEffect.transform.localEulerAngles.y, Random.Range(0f, 360f));
		this.strikeEffect.SetActive(true);
		if (this.falseShiny != null)
		{
			this.falseShiny.SetActive(false);
		}
		if (this.shinyItem != null)
		{
			this.shinyItem.SetActive(true);
		}
		VibrationManager.PlayVibrationClipOneShot(this.popVibration, null, false, "", false);
	}

	// Token: 0x0400306C RID: 12396
	public bool bomb;

	// Token: 0x0400306D RID: 12397
	public GameObject explosionObject;

	// Token: 0x0400306E RID: 12398
	public ParticleSystem popEffect;

	// Token: 0x0400306F RID: 12399
	public GameObject strikeEffect;

	// Token: 0x04003070 RID: 12400
	public MeshRenderer meshRenderer;

	// Token: 0x04003071 RID: 12401
	public AudioSource audioSource;

	// Token: 0x04003072 RID: 12402
	public VibrationData popVibration;

	// Token: 0x04003073 RID: 12403
	public CircleCollider2D circleCollider;

	// Token: 0x04003074 RID: 12404
	public GameObject falseShiny;

	// Token: 0x04003075 RID: 12405
	public GameObject shinyItem;
}
