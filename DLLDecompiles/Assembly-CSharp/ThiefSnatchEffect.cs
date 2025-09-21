using System;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class ThiefSnatchEffect : MonoBehaviour
{
	// Token: 0x060016C6 RID: 5830 RVA: 0x000665C0 File Offset: 0x000647C0
	public void Setup(Transform enemy, bool rosaries, bool shards)
	{
		if (this.rosariesDisplay)
		{
			this.rosariesDisplay.SetActive(rosaries);
		}
		if (this.shardsDisplay)
		{
			this.shardsDisplay.SetActive(shards);
		}
		Vector3 position = HeroController.instance.transform.position;
		Vector3 position2 = base.transform.position;
		if (Random.Range(0, 2) == 0)
		{
			Vector3 localScale = base.transform.localScale;
			localScale.y *= -1f;
			base.transform.localScale = localScale;
		}
		float x = position2.x - position.x;
		float num = Mathf.Atan2(position2.y - position.y, x);
		num *= 57.29578f;
		base.transform.localRotation = Quaternion.Euler(0f, 0f, num);
		this.mainAudio.SpawnAndPlayOneShot(position2, null);
		if (rosaries)
		{
			this.currencyAudio.SpawnAndPlayOneShot(position2, null);
		}
	}

	// Token: 0x04001539 RID: 5433
	[SerializeField]
	private GameObject rosariesDisplay;

	// Token: 0x0400153A RID: 5434
	[SerializeField]
	private GameObject shardsDisplay;

	// Token: 0x0400153B RID: 5435
	[SerializeField]
	private AudioEvent mainAudio = AudioEvent.Default;

	// Token: 0x0400153C RID: 5436
	[SerializeField]
	private AudioEvent currencyAudio = AudioEvent.Default;
}
