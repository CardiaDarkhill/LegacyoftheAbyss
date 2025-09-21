using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000578 RID: 1400
public class TouchShake : MonoBehaviour
{
	// Token: 0x06003225 RID: 12837 RVA: 0x000DF6DC File Offset: 0x000DD8DC
	private void Start()
	{
		if (this.anim)
		{
			this.anim.enabled = false;
		}
	}

	// Token: 0x06003226 RID: 12838 RVA: 0x000DF6F8 File Offset: 0x000DD8F8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag == "HeroBox" || collision.tag == "Player")
		{
			if (this.anim)
			{
				if (!this.anim.gameObject.activeInHierarchy)
				{
					return;
				}
				if (this.anim && this.animateRoutine == null)
				{
					this.animateRoutine = base.StartCoroutine(this.DoAnimation());
					return;
				}
			}
			else if (this.tk2dAnim)
			{
				if (!this.tk2dAnim.gameObject.activeInHierarchy)
				{
					return;
				}
				if (this.tk2dAnim && this.animateRoutine == null)
				{
					this.animateRoutine = base.StartCoroutine(this.DoAnimation());
				}
			}
		}
	}

	// Token: 0x06003227 RID: 12839 RVA: 0x000DF7BB File Offset: 0x000DD9BB
	private IEnumerator DoAnimation()
	{
		if (this.particles)
		{
			this.particles.Emit(this.emitParticles);
		}
		if (this.audioTable)
		{
			this.audioTable.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, false, 1f, null);
		}
		if (this.spriteRenderer && this.anim)
		{
			yield return base.StartCoroutine(this.SpriteAnimation());
		}
		else if (this.tk2dAnim)
		{
			yield return base.StartCoroutine(this.tk2dAnimation());
		}
		this.animateRoutine = null;
		yield break;
	}

	// Token: 0x06003228 RID: 12840 RVA: 0x000DF7CA File Offset: 0x000DD9CA
	private IEnumerator SpriteAnimation()
	{
		Sprite sprite = this.spriteRenderer.sprite;
		this.anim.enabled = true;
		yield return null;
		yield return new WaitForSeconds(this.anim.Length);
		this.anim.enabled = false;
		this.spriteRenderer.sprite = sprite;
		yield break;
	}

	// Token: 0x06003229 RID: 12841 RVA: 0x000DF7D9 File Offset: 0x000DD9D9
	private IEnumerator tk2dAnimation()
	{
		this.tk2dAnim.PlayFromFrame(0);
		yield return new WaitForSeconds(this.tk2dAnim.CurrentClip.Duration);
		yield break;
	}

	// Token: 0x040035C0 RID: 13760
	[Header("If using SpriteRenderer")]
	public SpriteRenderer spriteRenderer;

	// Token: 0x040035C1 RID: 13761
	public BasicSpriteAnimator anim;

	// Token: 0x040035C2 RID: 13762
	[Header("If using tk2D")]
	public tk2dSpriteAnimator tk2dAnim;

	// Token: 0x040035C3 RID: 13763
	private Coroutine animateRoutine;

	// Token: 0x040035C4 RID: 13764
	[Header("General")]
	public ParticleSystem particles;

	// Token: 0x040035C5 RID: 13765
	public int emitParticles = 20;

	// Token: 0x040035C6 RID: 13766
	public AudioSource audioSourcePrefab;

	// Token: 0x040035C7 RID: 13767
	public RandomAudioClipTable audioTable;
}
