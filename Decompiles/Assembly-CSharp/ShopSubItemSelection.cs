using System;
using System.Collections;
using GlobalSettings;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000724 RID: 1828
public class ShopSubItemSelection : MonoBehaviour
{
	// Token: 0x0600412D RID: 16685 RVA: 0x0011E043 File Offset: 0x0011C243
	private void OnEnable()
	{
		this.ih = ManagerSingleton<InputHandler>.Instance;
		this.platform = Platform.Current;
		this.item = null;
		this.endTimer = 0f;
	}

	// Token: 0x0600412E RID: 16686 RVA: 0x0011E070 File Offset: 0x0011C270
	private void Update()
	{
		if (this.endTimer > 0f)
		{
			this.endTimer -= Time.deltaTime;
			if (this.endTimer <= 0f)
			{
				base.gameObject.SetActive(false);
				EventRegister.SendEvent(EventRegisterEvents.ResetShopWindow, null);
			}
			return;
		}
		if (!this.item)
		{
			return;
		}
		Platform.MenuActions menuAction = this.platform.GetMenuAction(this.ih.inputActions, false, false);
		if (menuAction == Platform.MenuActions.Submit)
		{
			this.selectorAnimator.Play(ShopSubItemSelection._disappearAnimId);
			this.itemListFsm.FsmVariables.FindFsmInt("Current Item Sub").Value = this.currentIndex;
			this.itemListFsm.SendEvent("TO CONFIRM");
			this.confirmAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, this.cursorTargetPos, null);
			return;
		}
		if (menuAction != Platform.MenuActions.Cancel)
		{
			int num;
			if (this.ih.inputActions.Left.WasPressed || this.ih.inputActions.PaneLeft.WasPressed)
			{
				num = -1;
			}
			else
			{
				if (!this.ih.inputActions.Right.WasPressed && !this.ih.inputActions.PaneRight.WasPressed)
				{
					return;
				}
				num = 1;
			}
			int subItemsCount = this.item.Item.SubItemsCount;
			this.currentIndex += num;
			if (this.currentIndex >= subItemsCount)
			{
				this.currentIndex = 0;
			}
			else if (this.currentIndex < 0)
			{
				this.currentIndex = subItemsCount - 1;
			}
			this.SetSelected(this.currentIndex, false);
			return;
		}
		this.selectorAnimator.Play(ShopSubItemSelection._disappearAnimId);
		this.endTimer = 0.1f;
		this.fadeParent.FadeToZero(this.endTimer);
		this.cancelAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, this.cursorTargetPos, null);
	}

	// Token: 0x0600412F RID: 16687 RVA: 0x0011E252 File Offset: 0x0011C452
	public void SetItem(GameObject itemObj, int initialSelection)
	{
		this.item = itemObj.GetComponent<ShopItemStats>();
		this.currentIndex = initialSelection;
		this.SetSelected(this.currentIndex, true);
		this.selectorAnimator.Play(ShopSubItemSelection._appearAnimId);
	}

	// Token: 0x06004130 RID: 16688 RVA: 0x0011E284 File Offset: 0x0011C484
	private void SetSelected(int index, bool isInstant)
	{
		if (this.cursorMoveRoutine != null)
		{
			base.StopCoroutine(this.cursorMoveRoutine);
			this.cursorMoveRoutine = null;
			this.selectionCursor.SetPosition2D(this.cursorTargetPos);
		}
		int num = 0;
		ShopSubItemStats shopSubItemStats = null;
		foreach (object obj in this.itemsParent)
		{
			Transform transform = (Transform)obj;
			if (transform.gameObject.activeSelf)
			{
				ShopSubItemStats component = transform.GetComponent<ShopSubItemStats>();
				if (component)
				{
					if (num == index)
					{
						shopSubItemStats = component;
						break;
					}
					num++;
				}
			}
		}
		if (!shopSubItemStats)
		{
			return;
		}
		this.cursorTargetPos = shopSubItemStats.transform.position;
		if (isInstant)
		{
			this.selectionCursor.SetPosition2D(this.cursorTargetPos);
			return;
		}
		this.cursorMoveRoutine = base.StartCoroutine(this.MoveCursorTo(this.cursorTargetPos));
	}

	// Token: 0x06004131 RID: 16689 RVA: 0x0011E380 File Offset: 0x0011C580
	private IEnumerator MoveCursorTo(Vector2 targetPos)
	{
		this.moveSelectionAudio.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, targetPos, null);
		this.selectorAnimator.Play(ShopSubItemSelection._disappearAnimId);
		yield return null;
		yield return new WaitForSeconds(this.selectorAnimator.GetCurrentAnimatorStateInfo(0).length);
		this.selectionCursor.SetPosition2D(targetPos);
		this.selectorAnimator.Play(ShopSubItemSelection._appearAnimId);
		yield break;
	}

	// Token: 0x04004287 RID: 17031
	[SerializeField]
	private Transform itemsParent;

	// Token: 0x04004288 RID: 17032
	[SerializeField]
	private PlayMakerFSM itemListFsm;

	// Token: 0x04004289 RID: 17033
	[SerializeField]
	private NestedFadeGroupBase fadeParent;

	// Token: 0x0400428A RID: 17034
	[SerializeField]
	private Transform selectionCursor;

	// Token: 0x0400428B RID: 17035
	[SerializeField]
	private Animator selectorAnimator;

	// Token: 0x0400428C RID: 17036
	[SerializeField]
	private AudioEvent moveSelectionAudio;

	// Token: 0x0400428D RID: 17037
	[SerializeField]
	private AudioEvent confirmAudio;

	// Token: 0x0400428E RID: 17038
	[SerializeField]
	private AudioEvent cancelAudio;

	// Token: 0x0400428F RID: 17039
	private ShopItemStats item;

	// Token: 0x04004290 RID: 17040
	private int currentIndex;

	// Token: 0x04004291 RID: 17041
	private float endTimer;

	// Token: 0x04004292 RID: 17042
	private Vector2 cursorTargetPos;

	// Token: 0x04004293 RID: 17043
	private Coroutine cursorMoveRoutine;

	// Token: 0x04004294 RID: 17044
	private InputHandler ih;

	// Token: 0x04004295 RID: 17045
	private Platform platform;

	// Token: 0x04004296 RID: 17046
	private static readonly int _appearAnimId = Animator.StringToHash("Appear");

	// Token: 0x04004297 RID: 17047
	private static readonly int _disappearAnimId = Animator.StringToHash("Disappear");
}
