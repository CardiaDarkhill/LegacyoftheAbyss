using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000445 RID: 1093
public class MegaJellyZap : MonoBehaviour
{
	// Token: 0x0600267A RID: 9850 RVA: 0x000AE440 File Offset: 0x000AC640
	private void Awake()
	{
		this.col = base.GetComponent<CircleCollider2D>();
		this.fade = base.GetComponentInChildren<ColorFader>();
		if (this.anim)
		{
			this.animMesh = this.anim.GetComponent<MeshRenderer>();
		}
		this.originX = base.transform.position.x;
		this.originY = base.transform.position.y;
	}

	// Token: 0x0600267B RID: 9851 RVA: 0x000AE4B0 File Offset: 0x000AC6B0
	private void OnEnable()
	{
		this.routine = base.StartCoroutine((this.type == MegaJellyZap.Type.Zap) ? this.ZapSequence() : this.MultiZapSequence());
		if (this.shift != 0f)
		{
			base.transform.position = new Vector3(this.originX + Random.Range(-this.shift, this.shift), this.originY + Random.Range(-this.shift, this.shift), base.transform.position.z);
		}
	}

	// Token: 0x0600267C RID: 9852 RVA: 0x000AE53E File Offset: 0x000AC73E
	private void OnDisable()
	{
		if (this.routine != null)
		{
			base.StopCoroutine(this.routine);
		}
	}

	// Token: 0x0600267D RID: 9853 RVA: 0x000AE554 File Offset: 0x000AC754
	private IEnumerator ZapSequence()
	{
		this.col.enabled = false;
		this.anticRing.SetActive(false);
		yield return new WaitForSeconds(this.delay);
		float seconds = 0f;
		if (this.yDelay)
		{
			seconds = (this.yDelay_start - base.transform.position.y) * this.yDelay_factor;
		}
		yield return new WaitForSeconds(seconds);
		this.ptAttack.Stop();
		this.ptAntic.Play();
		yield return new WaitForSeconds(0.15f);
		if (this.fade)
		{
			this.fade.Fade(true);
		}
		this.anticRing.SetActive(true);
		this.zapBugPt1.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		yield return new WaitForSeconds(1.2f);
		this.anticRing.SetActive(false);
		this.zapBugPt2.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		this.ptAttack.Play();
		yield return new WaitForSeconds(0.1f);
		this.col.enabled = true;
		yield return new WaitForSeconds(0.3f);
		if (this.fade)
		{
			this.fade.Fade(false);
		}
		this.ptAttack.Stop();
		this.ptAntic.Stop();
		this.col.enabled = false;
		yield return new WaitForSeconds(0.5f);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x0600267E RID: 9854 RVA: 0x000AE563 File Offset: 0x000AC763
	private IEnumerator MultiZapSequence()
	{
		this.animMesh.enabled = false;
		this.col.enabled = false;
		this.ptAttack.Stop();
		base.transform.SetScaleX((float)((Random.Range(0, 2) == 0) ? 1 : -1));
		base.transform.SetRotation2D(Random.Range(0f, 360f));
		yield return new WaitForSeconds(Random.Range(0f, 0.5f));
		this.anim.Play("Zap Antic");
		this.animMesh.enabled = true;
		yield return new WaitForSeconds(0.8f);
		this.col.enabled = true;
		this.ptAttack.Play();
		this.anim.Play("Zap");
		yield return new WaitForSeconds(1f);
		this.ptAttack.Stop();
		this.col.enabled = false;
		yield return base.StartCoroutine(this.anim.PlayAnimWait("Zap End", null));
		this.animMesh.enabled = false;
		yield return new WaitForSeconds(0.5f);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x040023CA RID: 9162
	public MegaJellyZap.Type type;

	// Token: 0x040023CB RID: 9163
	public float delay;

	// Token: 0x040023CC RID: 9164
	public float shift;

	// Token: 0x040023CD RID: 9165
	public ParticleSystem ptAttack;

	// Token: 0x040023CE RID: 9166
	public ParticleSystem ptAntic;

	// Token: 0x040023CF RID: 9167
	public GameObject anticRing;

	// Token: 0x040023D0 RID: 9168
	public AudioSource audioSourcePrefab;

	// Token: 0x040023D1 RID: 9169
	public AudioEvent zapBugPt1;

	// Token: 0x040023D2 RID: 9170
	public AudioEvent zapBugPt2;

	// Token: 0x040023D3 RID: 9171
	public tk2dSpriteAnimator anim;

	// Token: 0x040023D4 RID: 9172
	public bool yDelay;

	// Token: 0x040023D5 RID: 9173
	public float yDelay_start;

	// Token: 0x040023D6 RID: 9174
	public float yDelay_factor;

	// Token: 0x040023D7 RID: 9175
	private MeshRenderer animMesh;

	// Token: 0x040023D8 RID: 9176
	private CircleCollider2D col;

	// Token: 0x040023D9 RID: 9177
	private ColorFader fade;

	// Token: 0x040023DA RID: 9178
	private Coroutine routine;

	// Token: 0x040023DB RID: 9179
	private float originX;

	// Token: 0x040023DC RID: 9180
	private float originY;

	// Token: 0x0200172D RID: 5933
	public enum Type
	{
		// Token: 0x04008D8D RID: 36237
		Zap,
		// Token: 0x04008D8E RID: 36238
		MultiZap
	}
}
