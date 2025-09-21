using System;
using UnityEngine;

// Token: 0x02000184 RID: 388
public class FlintUseEffects : MonoBehaviour
{
	// Token: 0x06000CA7 RID: 3239 RVA: 0x0003868C File Offset: 0x0003688C
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<FlintUseEffects.EffectGroup>(ref this.effectGroups, typeof(NailElements));
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x000386A3 File Offset: 0x000368A3
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x000386AC File Offset: 0x000368AC
	private void OnEnable()
	{
		this.ResetParts();
		foreach (FlintUseEffects.EffectGroup effectGroup in this.effectGroups)
		{
			effectGroup.Awake();
			effectGroup.Reset();
		}
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x000386E2 File Offset: 0x000368E2
	private void OnDisable()
	{
		this.ResetParts();
		if (this.currentGroup != null)
		{
			this.currentGroup.Reset();
			this.currentGroup = null;
		}
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x00038704 File Offset: 0x00036904
	private void Update()
	{
		if (this.currentGroup != null && this.currentGroup.HasEffectsEnded())
		{
			base.gameObject.Recycle();
		}
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x00038726 File Offset: 0x00036926
	private void LateUpdate()
	{
		if (this.hero)
		{
			base.transform.SetPosition2D(this.hero.position);
		}
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x00038750 File Offset: 0x00036950
	private void ResetParts()
	{
		if (this.pt1)
		{
			this.pt1.SetActive(false);
		}
		if (this.pt2)
		{
			this.pt2.SetActive(false);
		}
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x00038784 File Offset: 0x00036984
	public void SetGroup(NailElements element)
	{
		if (!this.hero)
		{
			this.hero = HeroController.instance.transform;
		}
		if (this.hero)
		{
			base.transform.SetPosition2D(this.hero.position);
		}
		float x = this.hero.localScale.x;
		Vector3 localScale = base.transform.localScale;
		localScale.x = Mathf.Abs(localScale.x) * x;
		base.transform.localScale = localScale;
		this.currentGroup = this.effectGroups[(int)element];
		this.currentGroup.Reset();
		if (this.currentGroup.Parent)
		{
			this.currentGroup.Parent.SetActive(true);
		}
		SpriteRenderer[] array = this.tintSpriteRenderers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = this.currentGroup.EffectTintColor;
		}
		tk2dSprite[] array2 = this.tintTK2DSprites;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].color = this.currentGroup.EffectTintColor;
		}
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x000388A2 File Offset: 0x00036AA2
	public void SetPt1()
	{
		if (this.pt1)
		{
			this.pt1.SetActive(true);
		}
		if (this.currentGroup != null)
		{
			this.currentGroup.StartPt1();
		}
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x000388D0 File Offset: 0x00036AD0
	public void SetPt2()
	{
		if (this.pt1)
		{
			this.pt1.SetActive(false);
		}
		if (this.pt2)
		{
			this.pt2.SetActive(true);
		}
		if (this.currentGroup != null)
		{
			this.currentGroup.StopPt1();
			this.currentGroup.StartPt2();
		}
		HeroController.instance.AddFrost(-25f);
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0003893C File Offset: 0x00036B3C
	public void SetEnd()
	{
		this.ResetParts();
		if (this.currentGroup != null)
		{
			this.currentGroup.StopAll();
		}
	}

	// Token: 0x04000C2E RID: 3118
	[SerializeField]
	private GameObject pt1;

	// Token: 0x04000C2F RID: 3119
	[SerializeField]
	private GameObject pt2;

	// Token: 0x04000C30 RID: 3120
	[SerializeField]
	private SpriteRenderer[] tintSpriteRenderers;

	// Token: 0x04000C31 RID: 3121
	[SerializeField]
	private tk2dSprite[] tintTK2DSprites;

	// Token: 0x04000C32 RID: 3122
	[SerializeField]
	[ArrayForEnum(typeof(NailElements))]
	private FlintUseEffects.EffectGroup[] effectGroups;

	// Token: 0x04000C33 RID: 3123
	private FlintUseEffects.EffectGroup currentGroup;

	// Token: 0x04000C34 RID: 3124
	private Transform hero;

	// Token: 0x020014B2 RID: 5298
	[Serializable]
	private class EffectGroup
	{
		// Token: 0x0600844E RID: 33870 RVA: 0x0026B32C File Offset: 0x0026952C
		public void Awake()
		{
			this.pt1Particles = (this.Pt1 ? this.Pt1.GetComponentsInChildren<ParticleSystem>(true) : new ParticleSystem[0]);
			this.pt2Particles = (this.Pt2 ? this.Pt2.GetComponentsInChildren<ParticleSystem>(true) : new ParticleSystem[0]);
		}

		// Token: 0x0600844F RID: 33871 RVA: 0x0026B388 File Offset: 0x00269588
		public void StartPt1()
		{
			this.hasPt1Started = true;
			if (this.Pt1)
			{
				this.Pt1.SetActive(true);
			}
			ParticleSystem[] array = this.pt1Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play(true);
			}
		}

		// Token: 0x06008450 RID: 33872 RVA: 0x0026B3D4 File Offset: 0x002695D4
		public void StopPt1()
		{
			ParticleSystem[] array = this.pt1Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
		}

		// Token: 0x06008451 RID: 33873 RVA: 0x0026B400 File Offset: 0x00269600
		public void StartPt2()
		{
			this.hasPt2Started = true;
			if (this.Pt2)
			{
				this.Pt2.SetActive(true);
			}
			ParticleSystem[] array = this.pt2Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Play(true);
			}
		}

		// Token: 0x06008452 RID: 33874 RVA: 0x0026B44C File Offset: 0x0026964C
		public void StopPt2()
		{
			ParticleSystem[] array = this.pt2Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
			}
		}

		// Token: 0x06008453 RID: 33875 RVA: 0x0026B478 File Offset: 0x00269678
		public void StopAll()
		{
			this.StopPt1();
			this.StopPt2();
		}

		// Token: 0x06008454 RID: 33876 RVA: 0x0026B488 File Offset: 0x00269688
		public void Reset()
		{
			this.hasPt1Started = false;
			this.hasPt2Started = false;
			if (this.Parent)
			{
				this.Parent.SetActive(false);
			}
			ParticleSystem[] array = this.pt1Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			array = this.pt2Particles;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
			if (this.Pt1)
			{
				this.Pt1.SetActive(false);
			}
			if (this.Pt2)
			{
				this.Pt2.SetActive(false);
			}
		}

		// Token: 0x06008455 RID: 33877 RVA: 0x0026B52C File Offset: 0x0026972C
		public bool HasEffectsEnded()
		{
			if (!this.hasPt1Started || !this.hasPt2Started)
			{
				return false;
			}
			ParticleSystem[] array = this.pt1Particles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive(true))
				{
					return false;
				}
			}
			array = this.pt2Particles;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive(true))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x04008441 RID: 33857
		public GameObject Parent;

		// Token: 0x04008442 RID: 33858
		public GameObject Pt1;

		// Token: 0x04008443 RID: 33859
		public GameObject Pt2;

		// Token: 0x04008444 RID: 33860
		public Color EffectTintColor;

		// Token: 0x04008445 RID: 33861
		private ParticleSystem[] pt1Particles;

		// Token: 0x04008446 RID: 33862
		private ParticleSystem[] pt2Particles;

		// Token: 0x04008447 RID: 33863
		private bool hasPt1Started;

		// Token: 0x04008448 RID: 33864
		private bool hasPt2Started;
	}
}
