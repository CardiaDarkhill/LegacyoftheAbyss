using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020007C1 RID: 1985
public class MazeCorpseSpawner : MonoBehaviour
{
	// Token: 0x060045F7 RID: 17911 RVA: 0x00130524 File Offset: 0x0012E724
	private void Start()
	{
		PlayerData instance = PlayerData.instance;
		IReadOnlyCollection<string> sceneNames = this.readScenesFromController.SceneNames;
		if (this.readScenesFromController.IsCapScene || string.IsNullOrEmpty(instance.HeroCorpseScene) || !sceneNames.Contains(instance.HeroCorpseScene) || sceneNames.Contains(instance.PreviousMazeScene))
		{
			return;
		}
		this.hc = HeroController.instance;
		if (this.hc.isHeroInPosition)
		{
			this.SpawnCorpse();
			return;
		}
		HeroController.HeroInPosition temp = null;
		temp = delegate(bool _)
		{
			this.SpawnCorpse();
			this.hc.heroInPosition -= temp;
		};
		this.hc.heroInPosition += temp;
	}

	// Token: 0x060045F8 RID: 17912 RVA: 0x001305D0 File Offset: 0x0012E7D0
	private void SpawnCorpse()
	{
		HeroCorpseMarker closest = HeroCorpseMarker.GetClosest(this.hc.transform.position);
		if (closest == null)
		{
			Debug.LogError("Could not find a HeroCorpseMarker", this);
			return;
		}
		Vector2 position = closest.Position;
		GameObject heroCorpsePrefab = GameManager.instance.sm.heroCorpsePrefab;
		GameObject gameObject = Object.Instantiate<GameObject>(heroCorpsePrefab, new Vector3(position.x, position.y, heroCorpsePrefab.transform.position.z), Quaternion.identity);
		RepositionFromWalls component = gameObject.GetComponent<RepositionFromWalls>();
		if (component)
		{
			component.enabled = false;
			gameObject.transform.position = position;
		}
		gameObject.transform.SetParent(base.transform, true);
		gameObject.transform.SetParent(null);
	}

	// Token: 0x04004695 RID: 18069
	[SerializeField]
	private MazeController readScenesFromController;

	// Token: 0x04004696 RID: 18070
	private HeroController hc;
}
