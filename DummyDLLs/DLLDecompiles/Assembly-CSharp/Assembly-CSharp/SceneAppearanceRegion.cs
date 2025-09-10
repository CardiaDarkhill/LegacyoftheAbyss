using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020000E5 RID: 229
public class SceneAppearanceRegion : MonoBehaviour
{
	// Token: 0x1700008A RID: 138
	// (get) Token: 0x0600072B RID: 1835 RVA: 0x00023670 File Offset: 0x00021870
	public Color HeroLightColor
	{
		get
		{
			return this.heroLightColor;
		}
	}

	// Token: 0x1700008B RID: 139
	// (get) Token: 0x0600072C RID: 1836 RVA: 0x00023678 File Offset: 0x00021878
	public bool AffectAmbientLight
	{
		get
		{
			return this.affectAmbientLight;
		}
	}

	// Token: 0x1700008C RID: 140
	// (get) Token: 0x0600072D RID: 1837 RVA: 0x00023680 File Offset: 0x00021880
	public Color AmbientLightColor
	{
		get
		{
			return this.ambientLightColor;
		}
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x0600072E RID: 1838 RVA: 0x00023688 File Offset: 0x00021888
	public float AmbientLightIntensity
	{
		get
		{
			return this.ambientLightIntensity;
		}
	}

	// Token: 0x1700008E RID: 142
	// (get) Token: 0x0600072F RID: 1839 RVA: 0x00023690 File Offset: 0x00021890
	public float BloomThreshold
	{
		get
		{
			return this.bloomThreshold;
		}
	}

	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06000730 RID: 1840 RVA: 0x00023698 File Offset: 0x00021898
	public float BloomIntensity
	{
		get
		{
			return this.bloomIntensity;
		}
	}

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06000731 RID: 1841 RVA: 0x000236A0 File Offset: 0x000218A0
	public float BloomBlurSize
	{
		get
		{
			return this.bloomBlurSize;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06000732 RID: 1842 RVA: 0x000236A8 File Offset: 0x000218A8
	public bool AffectSaturation
	{
		get
		{
			return this.affectSaturation;
		}
	}

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06000733 RID: 1843 RVA: 0x000236B0 File Offset: 0x000218B0
	public float Saturation
	{
		get
		{
			return this.saturation;
		}
	}

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x06000734 RID: 1844 RVA: 0x000236B8 File Offset: 0x000218B8
	public Color CharacterTintColor
	{
		get
		{
			return this.characterTint;
		}
	}

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x06000735 RID: 1845 RVA: 0x000236C0 File Offset: 0x000218C0
	public Color CharacterLightDustColor
	{
		get
		{
			return this.characterLightDustColor;
		}
	}

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x06000736 RID: 1846 RVA: 0x000236C8 File Offset: 0x000218C8
	public SceneAppearanceRegion.DustMaterials CharacterLightDustMaterials
	{
		get
		{
			return this.characterLightDustMaterials;
		}
	}

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x06000737 RID: 1847 RVA: 0x000236D0 File Offset: 0x000218D0
	public float FadeDuration
	{
		get
		{
			return this.fadeDuration;
		}
	}

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x06000738 RID: 1848 RVA: 0x000236D8 File Offset: 0x000218D8
	public float ExitFadeDuration
	{
		get
		{
			if (!this.exitFadeDuration.IsEnabled)
			{
				return this.fadeDuration;
			}
			return this.exitFadeDuration.Value;
		}
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x000236FC File Offset: 0x000218FC
	private void OnEnable()
	{
		SceneAppearanceRegion.<>c__DisplayClass51_0 CS$<>8__locals1 = new SceneAppearanceRegion.<>c__DisplayClass51_0();
		CS$<>8__locals1.<>4__this = this;
		if (this.layerMask < 0)
		{
			this.layerMask = global::Helper.GetCollidingLayerMaskForLayer(base.gameObject.layer);
		}
		CS$<>8__locals1.hc = HeroController.instance;
		if (CS$<>8__locals1.hc.isHeroInPosition)
		{
			this.HeroInPosition(false);
			return;
		}
		CS$<>8__locals1.hc.heroInPosition += CS$<>8__locals1.<OnEnable>g__Temp|0;
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x0002376C File Offset: 0x0002196C
	private void HeroInPosition(bool isFromEvent)
	{
		this.isHeroInPosition = true;
		Collider2D[] components = base.GetComponents<Collider2D>();
		for (int i = 0; i < components.Length; i++)
		{
			int num = components[i].Overlap(new ContactFilter2D
			{
				useTriggers = true,
				useLayerMask = true,
				layerMask = this.layerMask
			}, SceneAppearanceRegion._tempResults);
			for (int j = 0; j < num; j++)
			{
				this.HandleTriggerEnter(SceneAppearanceRegion._tempResults[j], isFromEvent);
			}
		}
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x000237F0 File Offset: 0x000219F0
	private void OnDisable()
	{
		if (!GameManager.UnsafeInstance)
		{
			return;
		}
		foreach (GameObject obj in this.insideObjs)
		{
			this.RemoveInside(obj);
		}
		this.insideObjs.Clear();
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x0002385C File Offset: 0x00021A5C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		this.HandleTriggerEnter(collision, false);
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x00023868 File Offset: 0x00021A68
	private void HandleTriggerEnter(Collider2D collision, bool forceImmediate)
	{
		if (collision.GetComponent<HeroController>() && !this.isHeroInPosition)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		this.insideObjs.Add(gameObject);
		HeroLight component = gameObject.GetComponent<HeroLight>();
		if (component)
		{
			if (this.affectHeroLight)
			{
				component.AddInside(this, forceImmediate);
			}
			GameManager.instance.sm.AddInsideAppearanceRegion(this, forceImmediate);
			if (this.affectBloom)
			{
				GameCameras.instance.cameraController.GetComponent<BloomOptimized>().AddInside(this, forceImmediate);
			}
		}
		if (this.affectCharacterTint && CharacterTint.CanAdd(collision.gameObject))
		{
			(gameObject.GetComponent<CharacterTint>() ?? collision.gameObject.AddComponent<CharacterTint>()).AddInside(this, forceImmediate);
		}
		if (this.affectCharacterLightDust)
		{
			CharacterLightDust component2 = gameObject.GetComponent<CharacterLightDust>();
			if (component2)
			{
				component2.AddInside(this, forceImmediate);
			}
		}
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x00023940 File Offset: 0x00021B40
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.GetComponent<HeroController>() && !this.isHeroInPosition)
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		this.RemoveInside(gameObject);
		this.insideObjs.Remove(gameObject);
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x00023980 File Offset: 0x00021B80
	private void RemoveInside(GameObject obj)
	{
		if (!obj)
		{
			return;
		}
		GameCameras silentInstance = GameCameras.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		HeroLight component = obj.GetComponent<HeroLight>();
		if (component)
		{
			if (this.affectHeroLight)
			{
				component.RemoveInside(this);
			}
			GameManager silentInstance2 = GameManager.SilentInstance;
			if (silentInstance2)
			{
				CustomSceneManager sm = silentInstance2.sm;
				if (sm)
				{
					sm.RemoveInsideAppearanceRegion(this, false);
				}
			}
			if (this.affectBloom)
			{
				silentInstance.cameraController.GetComponent<BloomOptimized>().RemoveInside(this, false);
			}
		}
		if (this.affectCharacterTint)
		{
			CharacterTint component2 = obj.GetComponent<CharacterTint>();
			if (component2)
			{
				component2.RemoveInside(this, false);
			}
		}
		if (this.affectCharacterLightDust)
		{
			CharacterLightDust component3 = obj.GetComponent<CharacterLightDust>();
			if (component3)
			{
				component3.RemoveInside(this, false);
			}
		}
	}

	// Token: 0x040006F3 RID: 1779
	[Header("Hero Only")]
	[SerializeField]
	private bool affectHeroLight = true;

	// Token: 0x040006F4 RID: 1780
	[SerializeField]
	[FormerlySerializedAs("color")]
	[ModifiableProperty]
	[Conditional("affectHeroLight", true, false, false)]
	private Color heroLightColor;

	// Token: 0x040006F5 RID: 1781
	[Space]
	[SerializeField]
	private bool affectAmbientLight;

	// Token: 0x040006F6 RID: 1782
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectAmbientLight", true, false, false)]
	private Color ambientLightColor;

	// Token: 0x040006F7 RID: 1783
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectAmbientLight", true, false, false)]
	private float ambientLightIntensity;

	// Token: 0x040006F8 RID: 1784
	[Space]
	[SerializeField]
	private bool affectBloom;

	// Token: 0x040006F9 RID: 1785
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectBloom", true, false, false)]
	[MultiPropRange(0f, 1.5f)]
	private float bloomThreshold = 0.25f;

	// Token: 0x040006FA RID: 1786
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectBloom", true, false, false)]
	[MultiPropRange(0f, 2.5f)]
	private float bloomIntensity = 0.75f;

	// Token: 0x040006FB RID: 1787
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectBloom", true, false, false)]
	[MultiPropRange(0.25f, 5.5f)]
	private float bloomBlurSize = 1f;

	// Token: 0x040006FC RID: 1788
	[Space]
	[SerializeField]
	private bool affectSaturation;

	// Token: 0x040006FD RID: 1789
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectSaturation", true, false, false)]
	private float saturation;

	// Token: 0x040006FE RID: 1790
	[Header("All Characters")]
	[SerializeField]
	private bool affectCharacterTint;

	// Token: 0x040006FF RID: 1791
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectCharacterTint", true, false, false)]
	private Color characterTint;

	// Token: 0x04000700 RID: 1792
	[Space]
	[SerializeField]
	private bool affectCharacterLightDust;

	// Token: 0x04000701 RID: 1793
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectCharacterLightDust", true, false, false)]
	private Color characterLightDustColor;

	// Token: 0x04000702 RID: 1794
	[SerializeField]
	[ModifiableProperty]
	[Conditional("affectCharacterLightDust", true, false, false)]
	private SceneAppearanceRegion.DustMaterials characterLightDustMaterials;

	// Token: 0x04000703 RID: 1795
	[Header("Common")]
	[SerializeField]
	private float fadeDuration = 1f;

	// Token: 0x04000704 RID: 1796
	[SerializeField]
	private OverrideFloat exitFadeDuration;

	// Token: 0x04000705 RID: 1797
	private int layerMask = -1;

	// Token: 0x04000706 RID: 1798
	private bool isHeroInPosition;

	// Token: 0x04000707 RID: 1799
	private static readonly Collider2D[] _tempResults = new Collider2D[100];

	// Token: 0x04000708 RID: 1800
	private readonly HashSet<GameObject> insideObjs = new HashSet<GameObject>();

	// Token: 0x0200144A RID: 5194
	[Serializable]
	public struct DustMaterials
	{
		// Token: 0x040082BB RID: 33467
		public Material Foreground;

		// Token: 0x040082BC RID: 33468
		public Material Background;
	}
}
