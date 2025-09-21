using System;
using UnityEngine;

// Token: 0x02000579 RID: 1401
public class TownGrass : MonoBehaviour
{
	// Token: 0x0600322B RID: 12843 RVA: 0x000DF7F8 File Offset: 0x000DD9F8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GrassCut.ShouldCut(collision))
		{
			int num = (int)Mathf.Sign(collision.transform.position.x - base.transform.position.x);
			Vector3 position = (collision.transform.position + base.transform.position) / 2f;
			if (this.nailEffectPrefab)
			{
				GameObject gameObject = this.nailEffectPrefab.Spawn(position);
				Vector3 localScale = gameObject.transform.localScale;
				localScale.x = Mathf.Abs(localScale.x) * (float)(-(float)num);
				gameObject.transform.localScale = localScale;
			}
			if (this.debris.Length != 0)
			{
				foreach (GameObject gameObject2 in this.debris)
				{
					gameObject2.SetActive(true);
					gameObject2.transform.SetParent(null, true);
				}
			}
			if (this.source && this.cutSound.Length != 0)
			{
				this.source.transform.SetParent(null, true);
				this.source.PlayOneShot(this.cutSound[Random.Range(0, this.cutSound.Length)]);
			}
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x040035C8 RID: 13768
	public GameObject[] debris;

	// Token: 0x040035C9 RID: 13769
	public GameObject nailEffectPrefab;

	// Token: 0x040035CA RID: 13770
	public AudioClip[] cutSound;

	// Token: 0x040035CB RID: 13771
	public AudioSource source;
}
