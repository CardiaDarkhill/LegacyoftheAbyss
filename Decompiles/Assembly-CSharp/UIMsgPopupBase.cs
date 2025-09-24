using System;
using System.Collections;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000743 RID: 1859
public abstract class UIMsgPopupBase<TPopupDisplay, TPopupObject> : UIMsgPopupBaseBase where TPopupDisplay : IUIMsgPopupItem where TPopupObject : UIMsgPopupBase<TPopupDisplay, TPopupObject>
{
	// Token: 0x17000782 RID: 1922
	// (get) Token: 0x06004244 RID: 16964 RVA: 0x00124BD2 File Offset: 0x00122DD2
	// (set) Token: 0x06004245 RID: 16965 RVA: 0x00124BDA File Offset: 0x00122DDA
	public TPopupDisplay DisplayingItem { get; protected set; }

	// Token: 0x06004246 RID: 16966 RVA: 0x00124BE3 File Offset: 0x00122DE3
	private void Awake()
	{
		if (this.sortingGroup)
		{
			this.initialSortingOrder = this.sortingGroup.sortingOrder;
		}
	}

	// Token: 0x06004247 RID: 16967 RVA: 0x00124C03 File Offset: 0x00122E03
	private void OnEnable()
	{
		this.canRemainAlive = true;
	}

	// Token: 0x06004248 RID: 16968 RVA: 0x00124C0C File Offset: 0x00122E0C
	protected static TPopupObject SpawnInternal(TPopupObject prefab, TPopupDisplay item, TPopupObject replacing = default(TPopupObject), bool forceReplacingEffect = false)
	{
		if (UIMsgPopupBaseBase.LastActiveMsgShared && UIMsgPopupBaseBase.LastActiveMsgShared.position.y > 8.5f)
		{
			return default(TPopupObject);
		}
		TPopupObject tpopupObject;
		if (UIMsgPopupBase<TPopupDisplay, TPopupObject>._lastActiveMsg && !replacing)
		{
			TPopupDisplay displayingItem = UIMsgPopupBase<TPopupDisplay, TPopupObject>._lastActiveMsg.DisplayingItem;
			Object representingObject = displayingItem.GetRepresentingObject();
			Object representingObject2 = item.GetRepresentingObject();
			if (representingObject2 != null && representingObject2 == representingObject)
			{
				tpopupObject = UIMsgPopupBase<TPopupDisplay, TPopupObject>._lastActiveMsg;
			}
			else
			{
				tpopupObject = prefab.Spawn<TPopupObject>();
			}
		}
		else
		{
			tpopupObject = prefab.Spawn<TPopupObject>();
		}
		if (tpopupObject.replaceBurstEffect)
		{
			tpopupObject.replaceBurstEffect.SetActive(false);
		}
		if (replacing != null)
		{
			if (replacing == UIMsgPopupBase<TPopupDisplay, TPopupObject>._lastActiveMsg)
			{
				UIMsgPopupBase<TPopupDisplay, TPopupObject>._lastActiveMsg = tpopupObject;
			}
			if (replacing.transform == UIMsgPopupBaseBase.LastActiveMsgShared)
			{
				UIMsgPopupBaseBase.LastActiveMsgShared = tpopupObject.transform;
			}
			tpopupObject.transform.localPosition = replacing.transform.localPosition;
			replacing.End();
			tpopupObject.DoReplacingEffects();
			if (tpopupObject.sortingGroup)
			{
				tpopupObject.sortingGroup.sortingOrder = replacing.sortingGroup.sortingOrder + 1;
			}
		}
		else
		{
			UIMsgPopupBaseBase.UpdatePosition(tpopupObject.transform);
			UIMsgPopupBase<TPopupDisplay, TPopupObject>._lastActiveMsg = tpopupObject;
			if (forceReplacingEffect)
			{
				tpopupObject.DoReplacingEffects();
			}
			if (tpopupObject.sortingGroup)
			{
				tpopupObject.sortingGroup.sortingOrder = tpopupObject.initialSortingOrder;
			}
		}
		tpopupObject.Display(item);
		return tpopupObject;
	}

	// Token: 0x06004249 RID: 16969 RVA: 0x00124DFC File Offset: 0x00122FFC
	public void DoReplacingEffects()
	{
		if (this.replaceBurstEffect)
		{
			this.replaceBurstEffect.SetActive(true);
		}
		UI.ItemQuestMaxPopupSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, Vector3.zero, null);
	}

	// Token: 0x0600424A RID: 16970 RVA: 0x00124E3C File Offset: 0x0012303C
	protected void Display(TPopupDisplay item)
	{
		this.DisplayingItem = item;
		this.UpdateDisplay(item);
		if (this.displayRoutine != null)
		{
			base.StopCoroutine(this.displayRoutine);
			this.displayRoutine = base.StartCoroutine(this.DisplaySequence(false));
			return;
		}
		this.displayRoutine = base.StartCoroutine(this.DisplaySequence(true));
	}

	// Token: 0x0600424B RID: 16971 RVA: 0x00124E92 File Offset: 0x00123092
	public void End()
	{
		this.canRemainAlive = false;
	}

	// Token: 0x0600424C RID: 16972 RVA: 0x00124E9B File Offset: 0x0012309B
	private bool IsInterrupted()
	{
		return !this.canRemainAlive;
	}

	// Token: 0x0600424D RID: 16973 RVA: 0x00124EA6 File Offset: 0x001230A6
	private IEnumerator DisplaySequence(bool fadeIn)
	{
		if (this.fadeGroup)
		{
			if (fadeIn)
			{
				this.fadeGroup.AlphaSelf = 0f;
				yield return new WaitForSecondsInterruptable(this.fadeGroup.FadeTo(1f, this.fadeInTime, null, false, null), new Func<bool>(this.IsInterrupted), true);
			}
			else
			{
				this.fadeGroup.FadeTo(1f, 0f, null, false, null);
			}
			this.fadeGroup.AlphaSelf = 1f;
		}
		yield return new WaitForSecondsInterruptable(this.waitTime, new Func<bool>(this.IsInterrupted), true);
		if (this.fadeGroup)
		{
			yield return new WaitForSecondsRealtime(this.fadeGroup.FadeTo(0f, this.fadeOutTime, null, true, null));
		}
		if (UIMsgPopupBase<TPopupDisplay, TPopupObject>._lastActiveMsg == this)
		{
			UIMsgPopupBase<TPopupDisplay, TPopupObject>._lastActiveMsg = default(TPopupObject);
		}
		if (UIMsgPopupBaseBase.LastActiveMsgShared == base.transform)
		{
			UIMsgPopupBaseBase.LastActiveMsgShared = null;
			if (InteractManager.BlockingInteractable == null)
			{
				GameManager.instance.DoQueuedSaveGame();
			}
		}
		this.displayRoutine = null;
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x0600424E RID: 16974
	protected abstract void UpdateDisplay(TPopupDisplay item);

	// Token: 0x040043E2 RID: 17378
	[SerializeField]
	private SortingGroup sortingGroup;

	// Token: 0x040043E3 RID: 17379
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x040043E4 RID: 17380
	[SerializeField]
	private float fadeInTime = 0.3f;

	// Token: 0x040043E5 RID: 17381
	[SerializeField]
	private float waitTime = 4.25f;

	// Token: 0x040043E6 RID: 17382
	[SerializeField]
	private float fadeOutTime = 0.2f;

	// Token: 0x040043E7 RID: 17383
	[Space]
	[SerializeField]
	private GameObject replaceBurstEffect;

	// Token: 0x040043E8 RID: 17384
	private int initialSortingOrder;

	// Token: 0x040043E9 RID: 17385
	private bool canRemainAlive;

	// Token: 0x040043EA RID: 17386
	private Coroutine displayRoutine;

	// Token: 0x040043EB RID: 17387
	private static TPopupObject _lastActiveMsg;
}
