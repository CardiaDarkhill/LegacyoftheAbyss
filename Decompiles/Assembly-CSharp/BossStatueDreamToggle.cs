using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200039B RID: 923
public class BossStatueDreamToggle : MonoBehaviour, IBossStatueToggle
{
	// Token: 0x06001F29 RID: 7977 RVA: 0x0008E75C File Offset: 0x0008C95C
	private void OnEnable()
	{
		if (this.bossStatue && !this.bossStatue.UsingDreamVersion)
		{
			ParticleSystem[] array = this.particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x0008E7A4 File Offset: 0x0008C9A4
	private void Start()
	{
		if (this.litPieces)
		{
			this.litPieces.SetActive(true);
			if (!this.bossStatue || !this.bossStatue.UsingDreamVersion)
			{
				this.litPieces.SetActive(false);
			}
			this.colorFaders = this.litPieces.GetComponentsInChildren<ColorFader>(true);
			ColorFader[] array = this.colorFaders;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnFadeEnd += delegate(bool up)
				{
					if (!up)
					{
						this.waitingForFade--;
					}
				};
			}
		}
		if (this.dreamBurstEffectPrefab)
		{
			this.dreamBurstEffect = Object.Instantiate<GameObject>(this.dreamBurstEffectPrefab, this.dreamBurstSpawnPoint);
			this.dreamBurstEffect.transform.localPosition = Vector3.zero;
			this.dreamBurstEffect.SetActive(false);
		}
		if (this.dreamBurstEffectOffPrefab)
		{
			this.dreamBurstEffectOff = Object.Instantiate<GameObject>(this.dreamBurstEffectOffPrefab, this.dreamBurstSpawnPoint);
			this.dreamBurstEffectOff.transform.localPosition = Vector3.zero;
			this.dreamBurstEffectOff.SetActive(false);
		}
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x0008E8B8 File Offset: 0x0008CAB8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!base.gameObject.activeInHierarchy || !this.canToggle)
		{
			return;
		}
		if (this.bossStatue && collision.tag == "Dream Attack")
		{
			bool flag = !this.bossStatue.UsingDreamVersion;
			this.bossStatue.SetDreamVersion(flag, false, true);
			if (this.dreamImpactPoint && this.dreamImpactPrefab)
			{
				this.dreamImpactPrefab.Spawn(this.dreamImpactPoint.position).transform.localScale = this.dreamImpactScale;
			}
			if (this.dreamBurstEffect)
			{
				this.dreamBurstEffect.SetActive(flag);
			}
			if (this.dreamBurstEffectOff)
			{
				this.dreamBurstEffectOff.SetActive(!flag);
			}
			base.StartCoroutine(this.Fade(this.bossStatue.UsingDreamVersion));
		}
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x0008E9AA File Offset: 0x0008CBAA
	private IEnumerator Fade(bool usingDreamVersion)
	{
		if (usingDreamVersion)
		{
			ParticleSystem[] array = this.particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play();
			}
		}
		else
		{
			ParticleSystem[] array = this.particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
		}
		if (this.litPieces)
		{
			this.litPieces.SetActive(true);
		}
		foreach (ColorFader colorFader in this.colorFaders)
		{
			if (!usingDreamVersion)
			{
				this.waitingForFade++;
			}
			colorFader.Fade(usingDreamVersion);
		}
		if (!usingDreamVersion)
		{
			while (this.waitingForFade > 0)
			{
				yield return null;
			}
			if (this.litPieces)
			{
				this.litPieces.SetActive(false);
			}
		}
		yield break;
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x0008E9C0 File Offset: 0x0008CBC0
	public void SetOwner(BossStatue statue)
	{
		this.bossStatue = statue;
		if (!this.bossStatue.UsingDreamVersion)
		{
			ParticleSystem[] array = this.particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			return;
		}
		if (this.litPieces)
		{
			this.litPieces.SetActive(true);
		}
		ColorFader[] array2 = this.colorFaders;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Fade(true);
		}
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x0008EA38 File Offset: 0x0008CC38
	public void SetState(bool value)
	{
		this.canToggle = value;
		if (!value)
		{
			base.gameObject.SetActive(this.canToggle);
		}
	}

	// Token: 0x04001E12 RID: 7698
	public GameObject litPieces;

	// Token: 0x04001E13 RID: 7699
	public ParticleSystem[] particles;

	// Token: 0x04001E14 RID: 7700
	public GameObject dreamImpactPrefab;

	// Token: 0x04001E15 RID: 7701
	public Vector3 dreamImpactScale = new Vector3(4f, 4f, 1f);

	// Token: 0x04001E16 RID: 7702
	public Transform dreamImpactPoint;

	// Token: 0x04001E17 RID: 7703
	private bool canToggle = true;

	// Token: 0x04001E18 RID: 7704
	private ColorFader[] colorFaders;

	// Token: 0x04001E19 RID: 7705
	private int waitingForFade;

	// Token: 0x04001E1A RID: 7706
	public GameObject dreamBurstEffectPrefab;

	// Token: 0x04001E1B RID: 7707
	public GameObject dreamBurstEffectOffPrefab;

	// Token: 0x04001E1C RID: 7708
	public Transform dreamBurstSpawnPoint;

	// Token: 0x04001E1D RID: 7709
	private GameObject dreamBurstEffect;

	// Token: 0x04001E1E RID: 7710
	private GameObject dreamBurstEffectOff;

	// Token: 0x04001E1F RID: 7711
	private BossStatue bossStatue;
}
