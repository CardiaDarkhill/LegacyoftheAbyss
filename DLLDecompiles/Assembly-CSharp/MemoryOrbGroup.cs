using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003E3 RID: 995
public class MemoryOrbGroup : MonoBehaviour
{
	// Token: 0x17000389 RID: 905
	// (get) Token: 0x06002217 RID: 8727 RVA: 0x0009D16D File Offset: 0x0009B36D
	public string PdBitmask
	{
		get
		{
			return this.pdBitmask;
		}
	}

	// Token: 0x1700038A RID: 906
	// (get) Token: 0x06002218 RID: 8728 RVA: 0x0009D178 File Offset: 0x0009B378
	public bool IsAllCollected
	{
		get
		{
			if (string.IsNullOrWhiteSpace(this.pdBitmask))
			{
				return true;
			}
			ulong variable = PlayerData.instance.GetVariable(this.pdBitmask);
			for (int i = 0; i < this.orbsParent.childCount; i++)
			{
				if (!variable.IsBitSet(i))
				{
					return false;
				}
			}
			return true;
		}
	}

	// Token: 0x06002219 RID: 8729 RVA: 0x0009D1C8 File Offset: 0x0009B3C8
	private void Awake()
	{
		if (this.readActivated)
		{
			this.readActivated.OnSetSaveState += this.OnReadActivated;
		}
		if (this.finishSingleScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.finishSingleScreenEdgePrefab, 5, false, false, true);
		}
		if (this.finishTargetScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.finishTargetScreenEdgePrefab, 1, false, false, true);
		}
		if (this.finishAllScreenEdgePrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.finishAllScreenEdgePrefab, 1, false, false, true);
		}
		PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		bool instantOrb = !string.IsNullOrWhiteSpace(this.pdBitmask);
		for (int i = 0; i < this.orbsParent.childCount; i++)
		{
			Transform child = this.orbsParent.GetChild(i);
			MemoryOrb component = child.gameObject.GetComponent<MemoryOrb>();
			component.Setup(this, i);
			component.InstantOrb = instantOrb;
			child.gameObject.SetActive(false);
		}
		if (this.activeWhileUncollected)
		{
			this.activeWhileUncollected.SetActive(false);
		}
	}

	// Token: 0x0600221A RID: 8730 RVA: 0x0009D2D8 File Offset: 0x0009B4D8
	private void Start()
	{
		if (!string.IsNullOrEmpty(this.readPdBool))
		{
			this.OnReadActivated(PlayerData.instance.GetBool(this.readPdBool));
		}
	}

	// Token: 0x0600221B RID: 8731 RVA: 0x0009D2FD File Offset: 0x0009B4FD
	private void OnDestroy()
	{
		if (this.readActivated)
		{
			this.readActivated.OnSetSaveState -= this.OnReadActivated;
		}
	}

	// Token: 0x0600221C RID: 8732 RVA: 0x0009D323 File Offset: 0x0009B523
	private void OnReadActivated(bool value)
	{
		if (value)
		{
			this.Reappear();
		}
	}

	// Token: 0x0600221D RID: 8733 RVA: 0x0009D330 File Offset: 0x0009B530
	public void Appear()
	{
		if (string.IsNullOrWhiteSpace(this.pdBitmask))
		{
			return;
		}
		ulong variable = PlayerData.instance.GetVariable(this.pdBitmask);
		bool active = false;
		for (int i = 0; i < this.orbsParent.childCount; i++)
		{
			if (!variable.IsBitSet(i))
			{
				this.orbsParent.GetChild(i).gameObject.SetActive(true);
				active = true;
			}
		}
		if (this.activeWhileUncollected)
		{
			this.activeWhileUncollected.SetActive(active);
		}
	}

	// Token: 0x0600221E RID: 8734 RVA: 0x0009D3AF File Offset: 0x0009B5AF
	public void Reappear()
	{
		this.Appear();
	}

	// Token: 0x0600221F RID: 8735 RVA: 0x0009D3B8 File Offset: 0x0009B5B8
	public void CollectedOrb(int index)
	{
		if (string.IsNullOrWhiteSpace(this.pdBitmask))
		{
			return;
		}
		PlayerData instance = PlayerData.instance;
		ulong num = instance.GetVariable(this.pdBitmask);
		num = num.SetBitAtIndex(index);
		instance.SetVariable(this.pdBitmask, num);
		bool active = false;
		for (int i = 0; i < this.orbsParent.childCount; i++)
		{
			if (!num.IsBitSet(i))
			{
				active = true;
			}
		}
		if (this.activeWhileUncollected)
		{
			this.activeWhileUncollected.SetActive(active);
		}
	}

	// Token: 0x06002220 RID: 8736 RVA: 0x0009D435 File Offset: 0x0009B635
	public void LargeOrbReturned()
	{
		this.SpawnScreenEdgeEffect(this.finishSingleScreenEdgePrefab);
	}

	// Token: 0x06002221 RID: 8737 RVA: 0x0009D443 File Offset: 0x0009B643
	public void OrbReturned(int total)
	{
		if (total == 12)
		{
			this.eventFsm.SendEvent("LAST ORB COLLECTED");
			this.SpawnScreenEdgeEffect(this.finishTargetScreenEdgePrefab);
			return;
		}
		if (total != 17)
		{
			return;
		}
		this.SpawnScreenEdgeEffect(this.finishAllScreenEdgePrefab);
	}

	// Token: 0x06002222 RID: 8738 RVA: 0x0009D47C File Offset: 0x0009B67C
	private void SpawnScreenEdgeEffect(GameObject prefab)
	{
		if (!prefab)
		{
			return;
		}
		Transform transform = GameCameras.instance.mainCamera.transform;
		Vector2 vector = transform.position;
		Vector2 a = this.orbReturnTarget.position;
		Vector2 vector2 = new Vector2(8.3f * ForceCameraAspect.CurrentViewportAspect, 8.3f);
		Vector2 vector3 = vector - vector2;
		Vector2 vector4 = vector + vector2;
		Vector2 normalized = (a - vector).normalized;
		Vector2 vector5 = vector + normalized * vector2.x;
		if (vector5.x < vector3.x)
		{
			vector5.x = vector3.x;
		}
		else if (vector5.x > vector4.x)
		{
			vector5.x = vector4.x;
		}
		if (vector5.y < vector3.y)
		{
			vector5.y = vector3.y;
		}
		else if (vector5.y > vector4.y)
		{
			vector5.y = vector4.y;
		}
		vector5 += normalized * this.screenEdgePadding;
		prefab.Spawn(vector5.ToVector3(prefab.transform.localScale.z), Quaternion.Euler(0f, 0f, normalized.DirectionToAngle())).transform.SetParent(transform);
	}

	// Token: 0x06002223 RID: 8739 RVA: 0x0009D5D8 File Offset: 0x0009B7D8
	public void TestSingle()
	{
		this.SpawnScreenEdgeEffect(this.finishSingleScreenEdgePrefab);
	}

	// Token: 0x06002224 RID: 8740 RVA: 0x0009D5E6 File Offset: 0x0009B7E6
	public void TestTarget()
	{
		this.SpawnScreenEdgeEffect(this.finishTargetScreenEdgePrefab);
	}

	// Token: 0x06002225 RID: 8741 RVA: 0x0009D5F4 File Offset: 0x0009B7F4
	public void TestAll()
	{
		this.SpawnScreenEdgeEffect(this.finishAllScreenEdgePrefab);
	}

	// Token: 0x040020EE RID: 8430
	[SerializeField]
	[PlayerDataField(typeof(ulong), false)]
	private string pdBitmask;

	// Token: 0x040020EF RID: 8431
	[Space]
	[SerializeField]
	private PersistentBoolItem readActivated;

	// Token: 0x040020F0 RID: 8432
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string readPdBool;

	// Token: 0x040020F1 RID: 8433
	[Space]
	[SerializeField]
	private PlayMakerFSM eventFsm;

	// Token: 0x040020F2 RID: 8434
	[SerializeField]
	private Transform orbsParent;

	// Token: 0x040020F3 RID: 8435
	[SerializeField]
	private Transform orbReturnTarget;

	// Token: 0x040020F4 RID: 8436
	[SerializeField]
	private GameObject activeWhileUncollected;

	// Token: 0x040020F5 RID: 8437
	[Space]
	[SerializeField]
	private GameObject finishSingleScreenEdgePrefab;

	// Token: 0x040020F6 RID: 8438
	[SerializeField]
	private GameObject finishTargetScreenEdgePrefab;

	// Token: 0x040020F7 RID: 8439
	[SerializeField]
	private GameObject finishAllScreenEdgePrefab;

	// Token: 0x040020F8 RID: 8440
	[SerializeField]
	private float screenEdgePadding;
}
