using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200054F RID: 1359
public class SilkRationMachine : MonoBehaviour
{
	// Token: 0x14000098 RID: 152
	// (add) Token: 0x06003089 RID: 12425 RVA: 0x000D6724 File Offset: 0x000D4924
	// (remove) Token: 0x0600308A RID: 12426 RVA: 0x000D675C File Offset: 0x000D495C
	public event Action RationDropped;

	// Token: 0x0600308B RID: 12427 RVA: 0x000D6791 File Offset: 0x000D4991
	private void OnValidate()
	{
		if (this.startCount > this.rationObjects.Count)
		{
			this.startCount = this.rationObjects.Count;
			return;
		}
		if (this.startCount < -1)
		{
			this.startCount = -1;
		}
	}

	// Token: 0x0600308C RID: 12428 RVA: 0x000D67C8 File Offset: 0x000D49C8
	private void Awake()
	{
		this.OnValidate();
		for (int i = this.rationObjects.Count - 1; i >= 0; i--)
		{
			if (!this.rationObjects[i].gameObject.activeInHierarchy)
			{
				this.rationObjects.RemoveAt(i);
			}
		}
		if (this.startCount < 0)
		{
			this.startCount = this.rationObjects.Count;
		}
		for (int j = 0; j < this.rationObjects.Count; j++)
		{
			Animator animator = this.rationObjects[j];
			if (j - this.startCount >= 0)
			{
				animator.gameObject.SetActive(false);
			}
			else
			{
				animator.enabled = false;
			}
		}
		if (this.purchaseTracker)
		{
			this.purchaseTracker.OnGetSaveState += delegate(out int value)
			{
				value = this.purchasedRationsCount;
			};
			this.purchaseTracker.OnSetSaveState += delegate(int value)
			{
				this.purchasedRationsCount = value;
				int num = Mathf.Max(0, this.startCount - value);
				for (int k = this.startCount - 1; k >= num; k--)
				{
					this.rationObjects[k].gameObject.SetActive(false);
				}
			};
		}
		if (this.jammedBreakable)
		{
			this.jammedBreakable.SetActive(false);
		}
	}

	// Token: 0x0600308D RID: 12429 RVA: 0x000D68C8 File Offset: 0x000D4AC8
	private void Start()
	{
		if (this.rationPrefab)
		{
			int remainingRationCount = this.GetRemainingRationCount();
			if (remainingRationCount > 0)
			{
				PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.rationPrefab, remainingRationCount, true, false, false);
			}
		}
	}

	// Token: 0x0600308E RID: 12430 RVA: 0x000D6902 File Offset: 0x000D4B02
	public int GetRemainingRationCount()
	{
		return this.startCount - this.purchasedRationsCount;
	}

	// Token: 0x0600308F RID: 12431 RVA: 0x000D6911 File Offset: 0x000D4B11
	public bool IsAnyRationsLeft()
	{
		return !this.isJammed && this.GetRemainingRationCount() > 0;
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x000D6926 File Offset: 0x000D4B26
	public bool IsJammed()
	{
		return this.isJammed;
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x000D6930 File Offset: 0x000D4B30
	public bool TryDropRation()
	{
		if (!this.IsAnyRationsLeft())
		{
			Debug.LogError("No silk rations left!", this);
			return false;
		}
		int rationIndex = this.startCount - this.purchasedRationsCount - 1;
		base.StartCoroutine(this.DropRationRoutine(rationIndex));
		if (this.canJam && this.purchasedRationsCount >= this.minPurchaseToJam && Random.Range(0f, 1f) <= this.chanceToJam)
		{
			this.isJammed = true;
			if (this.jammedBreakable)
			{
				this.jammedBreakable.SetActive(true);
			}
			return false;
		}
		this.purchasedRationsCount++;
		return true;
	}

	// Token: 0x06003092 RID: 12434 RVA: 0x000D69CE File Offset: 0x000D4BCE
	private IEnumerator DropRationRoutine(int rationIndex)
	{
		Animator ration = this.rationObjects[rationIndex];
		ration.enabled = true;
		ration.Play(SilkRationMachine._dropAnimHash);
		yield return null;
		yield return new WaitForSeconds(ration.GetCurrentAnimatorStateInfo(0).length);
		ration.gameObject.SetActive(false);
		if (this.RationDropped != null)
		{
			this.RationDropped();
		}
		yield break;
	}

	// Token: 0x06003093 RID: 12435 RVA: 0x000D69E4 File Offset: 0x000D4BE4
	public void FlingRations()
	{
	}

	// Token: 0x0400338E RID: 13198
	private static readonly int _dropAnimHash = Animator.StringToHash("Drop");

	// Token: 0x04003390 RID: 13200
	[SerializeField]
	private PersistentIntItem purchaseTracker;

	// Token: 0x04003391 RID: 13201
	[SerializeField]
	private List<Animator> rationObjects;

	// Token: 0x04003392 RID: 13202
	[SerializeField]
	private int startCount = -1;

	// Token: 0x04003393 RID: 13203
	[SerializeField]
	private GameObject rationPrefab;

	// Token: 0x04003394 RID: 13204
	[Header("Jamming")]
	[SerializeField]
	private bool canJam;

	// Token: 0x04003395 RID: 13205
	[SerializeField]
	[Range(0f, 1f)]
	private float chanceToJam;

	// Token: 0x04003396 RID: 13206
	[SerializeField]
	private int minPurchaseToJam;

	// Token: 0x04003397 RID: 13207
	[Space]
	[SerializeField]
	private GameObject jammedBreakable;

	// Token: 0x04003398 RID: 13208
	private bool isJammed;

	// Token: 0x04003399 RID: 13209
	private int purchasedRationsCount;
}
