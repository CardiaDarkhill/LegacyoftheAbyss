using System;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000215 RID: 533
public class AreaEffectTint : MonoBehaviour
{
	// Token: 0x060013C0 RID: 5056 RVA: 0x00059FCC File Offset: 0x000581CC
	private void Awake()
	{
		if (this.tinter)
		{
			this.initialColor = this.tinter.Color;
		}
		AreaEffectTint.RegisterEvents();
	}

	// Token: 0x060013C1 RID: 5057 RVA: 0x00059FF1 File Offset: 0x000581F1
	private void OnEnable()
	{
		this.DoTint();
	}

	// Token: 0x060013C2 RID: 5058 RVA: 0x00059FFC File Offset: 0x000581FC
	public void DoTint()
	{
		if (!this.tinter)
		{
			return;
		}
		Color b;
		bool flag = AreaEffectTint.IsActive(base.transform.position, out b) || (MaggotRegion.IsInsideAny && base.GetComponentInParent<HeroController>() != null);
		if (flag)
		{
			this.tinter.Color = this.initialColor * b;
			return;
		}
		this.tinter.Color = this.initialColor;
	}

	// Token: 0x060013C3 RID: 5059 RVA: 0x0005A078 File Offset: 0x00058278
	private void OnApplicationQuit()
	{
		AreaEffectTint.UnregisterEvents();
	}

	// Token: 0x060013C4 RID: 5060 RVA: 0x0005A07F File Offset: 0x0005827F
	private static void RegisterEvents()
	{
		if (!AreaEffectTint.registeredEvent)
		{
			AreaEffectTint.registeredEvent = true;
			SceneManager.activeSceneChanged += AreaEffectTint.SceneManagerOnActiveSceneChanged;
		}
	}

	// Token: 0x060013C5 RID: 5061 RVA: 0x0005A09F File Offset: 0x0005829F
	private static void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
	{
		AreaEffectTint.sceneDirty = true;
	}

	// Token: 0x060013C6 RID: 5062 RVA: 0x0005A0A7 File Offset: 0x000582A7
	private static void UnregisterEvents()
	{
		if (!AreaEffectTint.registeredEvent)
		{
			return;
		}
		AreaEffectTint.registeredEvent = false;
		SceneManager.activeSceneChanged -= AreaEffectTint.SceneManagerOnActiveSceneChanged;
	}

	// Token: 0x060013C7 RID: 5063 RVA: 0x0005A0C8 File Offset: 0x000582C8
	public static bool IsActive(Vector2 pos, out Color tintColor)
	{
		if (SwampZone.IsInside(pos))
		{
			tintColor = Effects.MossEffectsTintDust;
			return true;
		}
		switch (AreaEffectTint.GetActiveAreaInScene())
		{
		case AreaEffectTint.Area.None:
			tintColor = Color.white;
			return false;
		case AreaEffectTint.Area.Swamp:
			tintColor = Effects.MossEffectsTintDust;
			return true;
		case AreaEffectTint.Area.Abyss:
			tintColor = Color.black;
			return true;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	// Token: 0x060013C8 RID: 5064 RVA: 0x0005A130 File Offset: 0x00058330
	private static AreaEffectTint.Area GetActiveAreaInScene()
	{
		if (!AreaEffectTint.sceneDirty)
		{
			return AreaEffectTint.activeArea;
		}
		GameManager instance = GameManager.instance;
		string sceneNameString = instance.GetSceneNameString();
		MapZone currentMapZoneEnum = instance.GetCurrentMapZoneEnum();
		if (currentMapZoneEnum != MapZone.DUSTPENS)
		{
			switch (currentMapZoneEnum)
			{
			case MapZone.SWAMP:
				AreaEffectTint.activeArea = ((sceneNameString == "Shadow_24") ? AreaEffectTint.Area.None : AreaEffectTint.Area.Swamp);
				break;
			case MapZone.ABYSS:
				AreaEffectTint.activeArea = AreaEffectTint.Area.Abyss;
				break;
			case MapZone.AQUEDUCT:
				AreaEffectTint.activeArea = ((sceneNameString == "Aqueduct_05") ? AreaEffectTint.Area.None : AreaEffectTint.Area.Swamp);
				break;
			default:
				AreaEffectTint.activeArea = AreaEffectTint.Area.None;
				break;
			}
		}
		else
		{
			AreaEffectTint.activeArea = ((sceneNameString == "Shadow_Weavehome") ? AreaEffectTint.Area.None : AreaEffectTint.Area.Swamp);
		}
		AreaEffectTint.sceneDirty = false;
		return AreaEffectTint.activeArea;
	}

	// Token: 0x0400122C RID: 4652
	[SerializeField]
	private TintRendererGroup tinter;

	// Token: 0x0400122D RID: 4653
	private Color initialColor;

	// Token: 0x0400122E RID: 4654
	private static bool registeredEvent;

	// Token: 0x0400122F RID: 4655
	private static bool sceneDirty = true;

	// Token: 0x04001230 RID: 4656
	private static AreaEffectTint.Area activeArea;

	// Token: 0x02001537 RID: 5431
	public enum Area
	{
		// Token: 0x04008656 RID: 34390
		None,
		// Token: 0x04008657 RID: 34391
		Swamp,
		// Token: 0x04008658 RID: 34392
		Abyss
	}
}
