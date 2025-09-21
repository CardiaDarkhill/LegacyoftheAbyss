using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000136 RID: 310
public class StartAudioPlayerAfterHeroInPosition : MonoBehaviour
{
	// Token: 0x06000994 RID: 2452 RVA: 0x0002BD32 File Offset: 0x00029F32
	protected IEnumerator Start()
	{
		yield return null;
		while (HeroController.instance == null || !HeroController.instance.isHeroInPosition)
		{
			yield return null;
		}
		AudioSource component = base.GetComponent<AudioSource>();
		if (component != null)
		{
			component.Play();
		}
		yield break;
	}
}
