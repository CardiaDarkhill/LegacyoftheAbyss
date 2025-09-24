using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000193 RID: 403
public class HeroNailImbuement : MonoBehaviour
{
	// Token: 0x17000184 RID: 388
	// (get) Token: 0x06000F91 RID: 3985 RVA: 0x0004B1C6 File Offset: 0x000493C6
	// (set) Token: 0x06000F92 RID: 3986 RVA: 0x0004B1CE File Offset: 0x000493CE
	public NailElements CurrentElement { get; private set; }

	// Token: 0x17000185 RID: 389
	// (get) Token: 0x06000F93 RID: 3987 RVA: 0x0004B1D7 File Offset: 0x000493D7
	public NailImbuementConfig CurrentImbuement
	{
		get
		{
			return this.currentImbuement;
		}
	}

	// Token: 0x06000F94 RID: 3988 RVA: 0x0004B1DF File Offset: 0x000493DF
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<NailImbuementConfig>(ref this.nailConfigs, typeof(NailElements));
	}

	// Token: 0x06000F95 RID: 3989 RVA: 0x0004B1F8 File Offset: 0x000493F8
	private void Awake()
	{
		this.OnValidate();
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		if (this.nailConfigs != null)
		{
			for (int i = 0; i < this.nailConfigs.Length; i++)
			{
				NailImbuementConfig nailImbuementConfig = this.nailConfigs[i];
				if (!(nailImbuementConfig == null))
				{
					nailImbuementConfig.EnsurePersonalPool(base.gameObject);
				}
			}
		}
		EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED").ReceivedEvent += delegate()
		{
			if (ToolItemManager.ActiveState != ToolsActiveStates.Active)
			{
				this.SetElement(NailElements.None);
			}
		};
		EventRegister.GetRegisterGuaranteed(base.gameObject, "BENCHREST START").ReceivedEvent += delegate()
		{
			this.SetElement(NailElements.None);
		};
	}

	// Token: 0x06000F96 RID: 3990 RVA: 0x0004B292 File Offset: 0x00049492
	private void Update()
	{
		if (this.imbuementTimeLeft > 0f)
		{
			this.imbuementTimeLeft -= Time.deltaTime;
			if (this.imbuementTimeLeft <= 0f)
			{
				this.SetElement(NailElements.None);
			}
		}
	}

	// Token: 0x06000F97 RID: 3991 RVA: 0x0004B2C8 File Offset: 0x000494C8
	public void SetElement(NailElements element)
	{
		if (this.CurrentElement == NailElements.Fire && element != NailElements.Fire)
		{
			EventRegister.SendEvent(EventRegisterEvents.FlintSlateExpire, null);
		}
		else if (this.CurrentElement == NailElements.Poison && element != NailElements.Poison)
		{
			EventRegister.SendEvent(EventRegisterEvents.FlintSlateExpire, null);
		}
		this.CurrentElement = element;
		if (this.spawnedParticles)
		{
			this.spawnedParticles.StopParticleSystems();
			this.spawnedParticles = null;
		}
		if (element == NailElements.None)
		{
			this.currentImbuement = null;
			this.imbuementTimeLeft = 0f;
			this.spriteFlash.CancelRepeatingFlash(this.flashingHandle);
			if (this.imbuedHeroLightGroup)
			{
				this.imbuedHeroLightGroup.FadeTo(0f, this.imbuedHeroLightFadeOutDuration, null, false, null);
				return;
			}
		}
		else
		{
			NailImbuementConfig y = this.currentImbuement;
			this.currentImbuement = this.nailConfigs[(int)element];
			this.imbuementTimeLeft = this.currentImbuement.Duration;
			this.spriteFlash.flashFocusHeal();
			if (this.currentImbuement != y || !this.spriteFlash.IsFlashing(true, this.flashingHandle))
			{
				SpriteFlash.FlashConfig heroFlashing = this.currentImbuement.HeroFlashing;
				this.flashingHandle = this.spriteFlash.Flash(heroFlashing.Colour, heroFlashing.Amount, heroFlashing.TimeUp, heroFlashing.StayTime, heroFlashing.TimeDown, 0f, true, 0, 1, false);
				if (this.imbuedHeroLightRenderer)
				{
					this.imbuedHeroLightRenderer.color = this.currentImbuement.ExtraHeroLightColor;
				}
				if (this.imbuedHeroLightGroup)
				{
					this.imbuedHeroLightGroup.FadeTo(1f, this.imbuedHeroLightFadeInDuration, null, false, null);
				}
			}
			if (this.currentImbuement.HeroParticles)
			{
				this.spawnedParticles = this.currentImbuement.HeroParticles.Spawn(base.transform.position);
				this.spawnedParticles.PlayParticleSystems();
			}
		}
	}

	// Token: 0x04000F2F RID: 3887
	[SerializeField]
	private SpriteRenderer imbuedHeroLightRenderer;

	// Token: 0x04000F30 RID: 3888
	[SerializeField]
	private NestedFadeGroupBase imbuedHeroLightGroup;

	// Token: 0x04000F31 RID: 3889
	[SerializeField]
	private float imbuedHeroLightFadeInDuration;

	// Token: 0x04000F32 RID: 3890
	[SerializeField]
	private float imbuedHeroLightFadeOutDuration;

	// Token: 0x04000F33 RID: 3891
	[Space]
	[SerializeField]
	[ArrayForEnum(typeof(NailElements))]
	private NailImbuementConfig[] nailConfigs;

	// Token: 0x04000F34 RID: 3892
	private NailImbuementConfig currentImbuement;

	// Token: 0x04000F35 RID: 3893
	private float imbuementTimeLeft;

	// Token: 0x04000F36 RID: 3894
	private PlayParticleEffects spawnedParticles;

	// Token: 0x04000F37 RID: 3895
	private SpriteFlash spriteFlash;

	// Token: 0x04000F38 RID: 3896
	private SpriteFlash.FlashHandle flashingHandle;
}
