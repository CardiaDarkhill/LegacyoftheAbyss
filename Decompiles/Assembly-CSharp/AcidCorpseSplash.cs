using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class AcidCorpseSplash : MonoBehaviour
{
	// Token: 0x06000214 RID: 532 RVA: 0x0000D699 File Offset: 0x0000B899
	private void Start()
	{
		if (this.corpseDetector)
		{
			this.corpseDetector.OnTriggerEntered += delegate(Collider2D col, GameObject sender)
			{
				base.StartCoroutine(this.CorpseSplash(col.gameObject));
			};
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000D6BF File Offset: 0x0000B8BF
	private IEnumerator CorpseSplash(GameObject corpseObject)
	{
		Corpse component = corpseObject.GetComponent<Corpse>();
		if (component)
		{
			component.Acid();
			Rigidbody2D body = corpseObject.GetComponent<Rigidbody2D>();
			this.splashSound.SpawnAndPlayOneShot(this.audioPlayerPefab, corpseObject.transform.position, null);
			Vector3 position = corpseObject.transform.position;
			if (this.corpseDetector)
			{
				BoxCollider2D component2 = this.corpseDetector.GetComponent<BoxCollider2D>();
				if (component2)
				{
					position.y = component2.bounds.max.y;
				}
			}
			if (this.acidSplashPrefab)
			{
				Object.Instantiate<GameObject>(this.acidSplashPrefab, position + new Vector3(0f, 0f, -0.1f), this.acidSplashPrefab.transform.rotation);
			}
			if (this.acidSteamPrefab)
			{
				Object.Instantiate<GameObject>(this.acidSteamPrefab, position, this.acidSteamPrefab.transform.rotation);
			}
			ParticleSystem acidBubble = null;
			if (this.bubCloudPrefab)
			{
				acidBubble = Object.Instantiate<ParticleSystem>(this.bubCloudPrefab, position, this.bubCloudPrefab.transform.rotation);
				if (acidBubble)
				{
					acidBubble.Play();
				}
			}
			ParticleSystem acidSpore = null;
			if (this.sporeCloudPrefab)
			{
				acidSpore = Object.Instantiate<ParticleSystem>(this.sporeCloudPrefab, position, this.sporeCloudPrefab.transform.rotation);
				if (acidSpore)
				{
					acidSpore.Play();
				}
			}
			for (float elapsed = 0f; elapsed <= 0.5f; elapsed += Time.fixedDeltaTime)
			{
				if (body)
				{
					body.linearVelocity *= 0.1f;
				}
				yield return new WaitForFixedUpdate();
			}
			if (body)
			{
				body.isKinematic = true;
			}
			if (corpseObject)
			{
				tk2dSprite rend = corpseObject.GetComponent<tk2dSprite>();
				if (rend)
				{
					float elapsed = 0f;
					float fadeTime = 1f;
					while (elapsed <= fadeTime)
					{
						rend.color = Color.Lerp(Color.white, Color.clear, elapsed / fadeTime);
						yield return null;
						elapsed += Time.deltaTime;
					}
				}
				rend = null;
			}
			if (acidBubble)
			{
				acidBubble.Stop();
			}
			if (acidSpore)
			{
				acidSpore.Stop();
			}
			body = null;
			acidBubble = null;
			acidSpore = null;
		}
		yield break;
	}

	// Token: 0x040001C2 RID: 450
	public TriggerEnterEvent corpseDetector;

	// Token: 0x040001C3 RID: 451
	[Space]
	public GameObject acidSplashPrefab;

	// Token: 0x040001C4 RID: 452
	public GameObject acidSteamPrefab;

	// Token: 0x040001C5 RID: 453
	public ParticleSystem sporeCloudPrefab;

	// Token: 0x040001C6 RID: 454
	public ParticleSystem bubCloudPrefab;

	// Token: 0x040001C7 RID: 455
	[Space]
	public AudioSource audioPlayerPefab;

	// Token: 0x040001C8 RID: 456
	public AudioEvent splashSound;
}
