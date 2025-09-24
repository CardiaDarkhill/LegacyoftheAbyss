using System;
using GlobalEnums;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class SceneParticlesController : MonoBehaviour
{
	// Token: 0x0600165B RID: 5723 RVA: 0x000649EB File Offset: 0x00062BEB
	private void OnValidate()
	{
		ArrayForEnumAttribute.EnsureArraySize<GameObject>(ref this.mapZoneParticles, typeof(MapZone));
		ArrayForEnumAttribute.EnsureArraySize<GameObject>(ref this.customParticles, typeof(CustomSceneParticles));
	}

	// Token: 0x0600165C RID: 5724 RVA: 0x00064A17 File Offset: 0x00062C17
	private void Awake()
	{
		this.OnValidate();
	}

	// Token: 0x0600165D RID: 5725 RVA: 0x00064A1F File Offset: 0x00062C1F
	private void OnEnable()
	{
		ForceCameraAspect.ViewportAspectChanged += this.OnCameraAspectChanged;
		this.gc = GameCameras.instance;
		this.gc.cameraController.PositionedAtHero += this.OnPositionedAtHero;
		this.OnPositionedAtHero();
	}

	// Token: 0x0600165E RID: 5726 RVA: 0x00064A60 File Offset: 0x00062C60
	private void OnDisable()
	{
		ForceCameraAspect.ViewportAspectChanged -= this.OnCameraAspectChanged;
		if (this.gc != null)
		{
			this.gc.cameraController.PositionedAtHero -= this.OnPositionedAtHero;
			this.gc = null;
		}
	}

	// Token: 0x0600165F RID: 5727 RVA: 0x00064AB0 File Offset: 0x00062CB0
	private void OnCameraAspectChanged(float aspect)
	{
		float num = aspect / 1.7777778f;
		Transform transform = base.transform;
		Vector3 localScale = transform.localScale;
		localScale.x = localScale.y * num;
		transform.localScale = localScale;
	}

	// Token: 0x06001660 RID: 5728 RVA: 0x00064AE8 File Offset: 0x00062CE8
	public void EnableParticles(bool noSceneParticles)
	{
		this.sceneParticleZoneType = ((this.sm.overrideParticlesWith == MapZone.NONE) ? this.sm.mapZone : this.sm.overrideParticlesWith);
		this.DisableParticles();
		bool flag = PlayerData.instance.blackThreadWorld;
		if (flag)
		{
			if (this.sm.act3ParticlesOverride.IsEnabled)
			{
				flag = (this.sm.act3ParticlesOverride.Value == CustomSceneManager.BoolFriendly.On);
			}
			else
			{
				MapZone mapZone = this.sm.mapZone;
				if (mapZone <= MapZone.CLOVER)
				{
					if (mapZone != MapZone.NONE && mapZone != MapZone.CLOVER)
					{
						goto IL_93;
					}
				}
				else if (mapZone != MapZone.PEAK && mapZone != MapZone.MEMORY && mapZone != MapZone.SURFACE)
				{
					goto IL_93;
				}
				flag = false;
			}
		}
		IL_93:
		this.act3Particles.SetActive(flag);
		if (noSceneParticles)
		{
			return;
		}
		GameObject gameObject = this.defaultParticles;
		if (this.sm.overrideParticlesWithCustom != CustomSceneParticles.None)
		{
			GameObject gameObject2 = this.customParticles[(int)this.sm.overrideParticlesWithCustom];
			if (gameObject2)
			{
				gameObject = gameObject2;
			}
		}
		else if (this.sceneParticleZoneType != MapZone.NONE)
		{
			GameObject gameObject3 = this.mapZoneParticles[(int)this.sceneParticleZoneType];
			if (gameObject3)
			{
				gameObject = gameObject3;
			}
		}
		if (gameObject)
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x06001661 RID: 5729 RVA: 0x00064C00 File Offset: 0x00062E00
	public void DisableParticles()
	{
		if (this.defaultParticles)
		{
			this.defaultParticles.SetActive(false);
		}
		foreach (GameObject gameObject in this.mapZoneParticles)
		{
			if (gameObject)
			{
				gameObject.SetActive(false);
			}
		}
		foreach (GameObject gameObject2 in this.customParticles)
		{
			if (gameObject2)
			{
				gameObject2.SetActive(false);
			}
		}
		this.act3Particles.SetActive(false);
	}

	// Token: 0x06001662 RID: 5730 RVA: 0x00064C82 File Offset: 0x00062E82
	public void SceneInit()
	{
		this.DisableParticles();
	}

	// Token: 0x06001663 RID: 5731 RVA: 0x00064C8C File Offset: 0x00062E8C
	private void OnPositionedAtHero()
	{
		this.gm = GameManager.instance;
		this.sm = this.gm.sm;
		if (this.sm == null)
		{
			this.sm = Object.FindObjectOfType<CustomSceneManager>();
		}
		if (this.gm.IsGameplayScene() && !this.gm.IsCinematicScene())
		{
			this.EnableParticles(this.sm.noParticles);
			return;
		}
		this.DisableParticles();
	}

	// Token: 0x040014D4 RID: 5332
	[SerializeField]
	private GameObject defaultParticles;

	// Token: 0x040014D5 RID: 5333
	[SerializeField]
	[ArrayForEnum(typeof(MapZone))]
	private GameObject[] mapZoneParticles;

	// Token: 0x040014D6 RID: 5334
	[SerializeField]
	[ArrayForEnum(typeof(CustomSceneParticles))]
	private GameObject[] customParticles;

	// Token: 0x040014D7 RID: 5335
	[Space]
	[SerializeField]
	private GameObject act3Particles;

	// Token: 0x040014D8 RID: 5336
	private GameManager gm;

	// Token: 0x040014D9 RID: 5337
	private CustomSceneManager sm;

	// Token: 0x040014DA RID: 5338
	private MapZone sceneParticleZoneType;

	// Token: 0x040014DB RID: 5339
	private GameCameras gc;
}
