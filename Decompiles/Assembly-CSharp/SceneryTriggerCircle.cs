using System;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class SceneryTriggerCircle : MonoBehaviour
{
	// Token: 0x1700025D RID: 605
	// (get) Token: 0x06001665 RID: 5733 RVA: 0x00064D08 File Offset: 0x00062F08
	// (set) Token: 0x06001666 RID: 5734 RVA: 0x00064D10 File Offset: 0x00062F10
	public bool active { get; private set; }

	// Token: 0x06001667 RID: 5735 RVA: 0x00064D1C File Offset: 0x00062F1C
	private void Awake()
	{
		this.col2ds = base.GetComponentsInChildren<CircleCollider2D>();
		this.animator = base.GetComponentInChildren<Animator>();
		if (this.col2ds.Length > 2 || this.col2ds.Length < 2)
		{
			Debug.LogError("Scenery Trigger requires exactly two Collider components attached to work correctly.");
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001668 RID: 5736 RVA: 0x00064D70 File Offset: 0x00062F70
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.layer == 9)
		{
			if (this.enterCount == 0)
			{
				this.enterCount = 1;
				return;
			}
			if (this.enterCount == 1)
			{
				this.active = true;
				this.animator.Play("Show");
				if (this.activateSound != null && this.audioSource != null)
				{
					this.RandomizePitch(this.audioSource, 0.85f, 1.15f);
					this.audioSource.PlayOneShot(this.activateSound);
				}
				this.enterCount = 2;
			}
		}
	}

	// Token: 0x06001669 RID: 5737 RVA: 0x00064E06 File Offset: 0x00063006
	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.layer == 9 && this.enterCount == 0)
		{
			this.enterCount = 1;
		}
	}

	// Token: 0x0600166A RID: 5738 RVA: 0x00064E28 File Offset: 0x00063028
	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject.layer == 9)
		{
			if (this.enterCount == 1)
			{
				this.active = false;
				this.animator.Play("Hide");
				if (this.deactivateSound != null && this.audioSource != null)
				{
					this.RandomizePitch(this.audioSource, 0.85f, 1.15f);
					this.audioSource.PlayOneShot(this.deactivateSound);
				}
				this.enterCount = 0;
				return;
			}
			if (this.enterCount == 2)
			{
				this.enterCount = 1;
			}
		}
	}

	// Token: 0x0600166B RID: 5739 RVA: 0x00064EC0 File Offset: 0x000630C0
	private void RandomizePitch(AudioSource src, float minPitch, float maxPitch)
	{
		float pitch = Random.Range(minPitch, maxPitch);
		src.pitch = pitch;
	}

	// Token: 0x0600166C RID: 5740 RVA: 0x00064EDC File Offset: 0x000630DC
	private void ResetPitch(AudioSource src)
	{
		src.pitch = 1f;
	}

	// Token: 0x040014DC RID: 5340
	private Animator animator;

	// Token: 0x040014DD RID: 5341
	private CircleCollider2D[] col2ds;

	// Token: 0x040014DE RID: 5342
	private int enterCount;

	// Token: 0x040014E0 RID: 5344
	public AudioSource audioSource;

	// Token: 0x040014E1 RID: 5345
	public AudioClip activateSound;

	// Token: 0x040014E2 RID: 5346
	public AudioClip deactivateSound;
}
