using System;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003E4 RID: 996
public class MemoryOrbSceneMap : MonoBehaviour
{
	// Token: 0x06002227 RID: 8743 RVA: 0x0009D60A File Offset: 0x0009B80A
	private void Awake()
	{
		base.gameObject.SetActiveChildren(false);
		if (this.needolinScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.needolinScreenEdgePrefab, 10, true, false, false);
		}
	}

	// Token: 0x06002228 RID: 8744 RVA: 0x0009D63C File Offset: 0x0009B83C
	private void Start()
	{
		this.otherOrbs = new Dictionary<Transform, GameObject>();
		string sceneNameString = GameManager.instance.GetSceneNameString();
		PlayerData instance = PlayerData.instance;
		List<Transform> list = new List<Transform>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			list.Clear();
			if (!(transform.name == sceneNameString))
			{
				foreach (object obj2 in transform)
				{
					Transform transform2 = (Transform)obj2;
					MemoryOrbSceneMapConditional component = transform2.GetComponent<MemoryOrbSceneMapConditional>();
					bool flag;
					if (component)
					{
						if (!component.IsActive)
						{
							continue;
						}
						flag = true;
					}
					else
					{
						string text = transform2.name;
						if (text[0] == '!')
						{
							flag = true;
							string text2 = text;
							text = text2.Substring(1, text2.Length - 1);
						}
						else
						{
							flag = false;
						}
						if (VariableExtensions.VariableExists<PlayerData>(text, typeof(bool)))
						{
							if (instance.GetBool(text) != flag)
							{
								continue;
							}
						}
						else
						{
							if (!VariableExtensions.VariableExists<PlayerData>(text, typeof(ulong)))
							{
								Debug.LogError("Could not find matching variable for memory orb map child: " + text);
								continue;
							}
							if (instance.GetVariable(text) != 0UL)
							{
								continue;
							}
						}
					}
					bool flag2 = false;
					foreach (Transform transform3 in list)
					{
						if (Vector2.Distance(transform3.position, transform2.position) <= 1f)
						{
							flag2 = true;
							this.otherOrbs[transform3] = this.needolinScreenEdgePrefabLarge;
							break;
						}
					}
					if (!flag2)
					{
						this.otherOrbs[transform2] = (flag ? this.needolinScreenEdgePrefabLarge : this.needolinScreenEdgePrefab);
						list.Add(transform2);
					}
				}
			}
		}
	}

	// Token: 0x06002229 RID: 8745 RVA: 0x0009D89C File Offset: 0x0009BA9C
	private void OnEnable()
	{
		HeroPerformanceRegion.StartedPerforming += this.OnNeedolinStart;
		HeroPerformanceRegion.StoppedPerforming += this.OnNeedolinStop;
	}

	// Token: 0x0600222A RID: 8746 RVA: 0x0009D8C0 File Offset: 0x0009BAC0
	private void OnDisable()
	{
		HeroPerformanceRegion.StartedPerforming -= this.OnNeedolinStart;
		HeroPerformanceRegion.StoppedPerforming -= this.OnNeedolinStop;
	}

	// Token: 0x0600222B RID: 8747 RVA: 0x0009D8E4 File Offset: 0x0009BAE4
	private void OnNeedolinStart()
	{
		if (NeedolinMsgBox.IsBlocked)
		{
			return;
		}
		if (this.spawnedEffects == null)
		{
			this.spawnedEffects = new List<GameObject>();
		}
		foreach (KeyValuePair<Transform, GameObject> keyValuePair in this.otherOrbs)
		{
			this.spawnedEffects.Add(MemoryOrbSource.SpawnScreenEdgeEffect(keyValuePair.Value, keyValuePair.Value, keyValuePair.Key.position, this.screenEdgePadding));
		}
	}

	// Token: 0x0600222C RID: 8748 RVA: 0x0009D980 File Offset: 0x0009BB80
	private void OnNeedolinStop()
	{
		if (this.spawnedEffects == null)
		{
			return;
		}
		if (this.temp == null)
		{
			this.temp = new List<ParticleSystem>();
		}
		for (int i = this.spawnedEffects.Count - 1; i >= 0; i--)
		{
			GameObject gameObject = this.spawnedEffects[i];
			gameObject.GetComponentsInChildren<ParticleSystem>(this.temp);
			foreach (ParticleSystem particleSystem in this.temp)
			{
				particleSystem.Stop(true);
			}
			if (this.temp.Count == 0)
			{
				gameObject.Recycle();
			}
			this.temp.Clear();
			this.spawnedEffects.RemoveAt(i);
		}
	}

	// Token: 0x040020F9 RID: 8441
	[SerializeField]
	private GameObject needolinScreenEdgePrefab;

	// Token: 0x040020FA RID: 8442
	[SerializeField]
	private GameObject needolinScreenEdgePrefabLarge;

	// Token: 0x040020FB RID: 8443
	[SerializeField]
	private float screenEdgePadding;

	// Token: 0x040020FC RID: 8444
	private Dictionary<Transform, GameObject> otherOrbs;

	// Token: 0x040020FD RID: 8445
	private List<GameObject> spawnedEffects;

	// Token: 0x040020FE RID: 8446
	private List<ParticleSystem> temp;
}
