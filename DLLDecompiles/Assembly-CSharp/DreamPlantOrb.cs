using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004D1 RID: 1233
public class DreamPlantOrb : MonoBehaviour
{
	// Token: 0x06002C59 RID: 11353 RVA: 0x000C2374 File Offset: 0x000C0574
	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
		PersistentBoolItem persist = base.GetComponent<PersistentBoolItem>();
		if (persist)
		{
			persist.OnGetSaveState += delegate(out bool value)
			{
				value = this.pickedUp;
			};
			persist.OnSetSaveState += delegate(bool value)
			{
				if (!this.didEverSetSaveState)
				{
					this.pickedUp = value;
					this.didEverSetSaveState = true;
					persist.enabled = false;
				}
			};
			persist.PreSetup();
		}
	}

	// Token: 0x06002C5A RID: 11354 RVA: 0x000C23EC File Offset: 0x000C05EC
	private void Start()
	{
		this.SetActive(false);
		this.initialScale = base.transform.localScale;
	}

	// Token: 0x06002C5B RID: 11355 RVA: 0x000C2408 File Offset: 0x000C0608
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.isActive || this.pickedUp || !this.canPickup)
		{
			return;
		}
		if (collision.tag == "Player")
		{
			GameManager.instance.IncrementPlayerDataInt("dreamOrbs");
			EventRegister.SendEvent(EventRegisterEvents.DreamOrbCollect, null);
			if (this.soundSource && this.collectSound)
			{
				if (DreamPlantOrb.currentPitch <= 0f || Time.timeAsDouble >= DreamPlantOrb.pitchReturnTime)
				{
					DreamPlantOrb.currentPitch = this.basePitch;
				}
				if (DreamPlantOrb.currentPitch > this.maxPitch)
				{
					DreamPlantOrb.currentPitch = this.maxPitch;
				}
				this.soundSource.pitch = DreamPlantOrb.currentPitch;
				this.soundSource.PlayOneShot(this.collectSound);
				DreamPlantOrb.currentPitch += this.increasePitch;
				DreamPlantOrb.pitchReturnTime = Time.timeAsDouble + (double)this.pitchReturnDelay;
			}
			if (this.pickupParticles)
			{
				this.pickupParticles.gameObject.SetActive(true);
			}
			if (this.whiteFlash)
			{
				this.whiteFlash.gameObject.SetActive(true);
			}
			if (this.pickupAnim)
			{
				this.pickupAnim.gameObject.SetActive(true);
				base.StartCoroutine(this.DisableAfterTime(this.pickupAnim.gameObject, this.pickupAnim.Length));
			}
			PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
			if (component)
			{
				component.enabled = true;
			}
			this.pickedUp = true;
			this.Disable();
		}
	}

	// Token: 0x06002C5C RID: 11356 RVA: 0x000C2599 File Offset: 0x000C0799
	public void Show()
	{
		if (this.pickedUp)
		{
			return;
		}
		this.SetActive(true);
		DreamPlantOrb.plant.AddOrbCount();
		this.spreadRoutine = base.StartCoroutine(this.Spread());
	}

	// Token: 0x06002C5D RID: 11357 RVA: 0x000C25C7 File Offset: 0x000C07C7
	private void SetActive(bool value)
	{
		this.isActive = value;
		if (this.rend)
		{
			this.rend.enabled = value;
		}
		if (this.loopSource)
		{
			this.loopSource.enabled = value;
		}
	}

	// Token: 0x06002C5E RID: 11358 RVA: 0x000C2602 File Offset: 0x000C0802
	private IEnumerator Spread()
	{
		if (this.rend)
		{
			this.rend.enabled = false;
		}
		yield return null;
		base.transform.localScale = this.initialScale.MultiplyElements(new Vector3(0.5f, 0.5f, 1f));
		Vector3 position = DreamPlantOrb.plant.transform.position;
		position.z = Random.Range(0.003f, 0.004f);
		position.x += (float)Random.Range(-1, 1);
		position.y += (float)Random.Range(-3, -2);
		Vector3 initialPos = base.transform.position;
		initialPos.z = 0.003f;
		base.transform.position = position;
		if (this.rend)
		{
			this.rend.enabled = true;
		}
		if (this.trailParticles)
		{
			this.trailParticles.gameObject.SetActive(true);
		}
		Vector3 vector = initialPos - base.transform.position;
		vector.z = 0f;
		vector.Normalize();
		vector *= Random.Range(2f, 10f);
		Vector3 position2 = base.transform.position + vector;
		yield return base.StartCoroutine(this.TweenPosition(position2, 1f, this.spread1Curve));
		yield return new WaitForSeconds(Random.Range(1f, 1.5f));
		yield return base.StartCoroutine(this.TweenPosition(initialPos, 1f, this.spread2Curve));
		base.transform.localScale = this.initialScale.MultiplyElements(new Vector3(1f, 1f, 1f));
		if (this.whiteFlash)
		{
			this.whiteFlash.gameObject.SetActive(true);
		}
		if (this.activateParticles)
		{
			this.activateParticles.gameObject.SetActive(true);
		}
		if (this.trailParticles)
		{
			this.trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		this.canPickup = true;
		yield break;
	}

	// Token: 0x06002C5F RID: 11359 RVA: 0x000C2611 File Offset: 0x000C0811
	private void Disable()
	{
		this.pickedUp = true;
		if (DreamPlantOrb.plant != null)
		{
			DreamPlantOrb.plant.RemoveOrbCount();
		}
		this.SetActive(false);
		if (this.spreadRoutine != null)
		{
			base.StopCoroutine(this.spreadRoutine);
		}
	}

	// Token: 0x06002C60 RID: 11360 RVA: 0x000C264C File Offset: 0x000C084C
	private IEnumerator DisableAfterTime(GameObject obj, float time)
	{
		yield return new WaitForSeconds(time);
		obj.SetActive(false);
		yield break;
	}

	// Token: 0x06002C61 RID: 11361 RVA: 0x000C2662 File Offset: 0x000C0862
	private IEnumerator TweenPosition(Vector3 position, float time, AnimationCurve curve)
	{
		Vector3 startPos = base.transform.position;
		for (float elapsed = 0f; elapsed <= time; elapsed += Time.deltaTime)
		{
			base.transform.position = Vector3.Lerp(startPos, position, curve.Evaluate(elapsed / time));
			yield return null;
		}
		base.transform.position = position;
		yield break;
	}

	// Token: 0x04002DE5 RID: 11749
	public static DreamPlant plant;

	// Token: 0x04002DE6 RID: 11750
	public BasicSpriteAnimator pickupAnim;

	// Token: 0x04002DE7 RID: 11751
	private Renderer rend;

	// Token: 0x04002DE8 RID: 11752
	private Vector3 initialScale;

	// Token: 0x04002DE9 RID: 11753
	public AudioSource loopSource;

	// Token: 0x04002DEA RID: 11754
	[Space]
	public AudioSource soundSource;

	// Token: 0x04002DEB RID: 11755
	public AudioClip collectSound;

	// Token: 0x04002DEC RID: 11756
	public float basePitch = 0.85f;

	// Token: 0x04002DED RID: 11757
	public float increasePitch = 0.025f;

	// Token: 0x04002DEE RID: 11758
	public float maxPitch = 1.25f;

	// Token: 0x04002DEF RID: 11759
	public float pitchReturnDelay = 3f;

	// Token: 0x04002DF0 RID: 11760
	private static float currentPitch;

	// Token: 0x04002DF1 RID: 11761
	private static double pitchReturnTime;

	// Token: 0x04002DF2 RID: 11762
	[Space]
	public GameObject whiteFlash;

	// Token: 0x04002DF3 RID: 11763
	[Space]
	public ParticleSystem pickupParticles;

	// Token: 0x04002DF4 RID: 11764
	[Space]
	public ParticleSystem trailParticles;

	// Token: 0x04002DF5 RID: 11765
	public ParticleSystem activateParticles;

	// Token: 0x04002DF6 RID: 11766
	[Space]
	public AnimationCurve spread1Curve;

	// Token: 0x04002DF7 RID: 11767
	public AnimationCurve spread2Curve;

	// Token: 0x04002DF8 RID: 11768
	private bool pickedUp;

	// Token: 0x04002DF9 RID: 11769
	private bool canPickup;

	// Token: 0x04002DFA RID: 11770
	private bool isActive;

	// Token: 0x04002DFB RID: 11771
	private bool didEverSetSaveState;

	// Token: 0x04002DFC RID: 11772
	private Coroutine spreadRoutine;
}
