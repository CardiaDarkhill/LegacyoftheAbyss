using System;
using System.Collections;
using GlobalSettings;
using HutongGames.PlayMaker;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020006B4 RID: 1716
public class InventoryPaneStandalone : InventoryPaneBase
{
	// Token: 0x140000D9 RID: 217
	// (add) Token: 0x06003DD5 RID: 15829 RVA: 0x0010FBA8 File Offset: 0x0010DDA8
	// (remove) Token: 0x06003DD6 RID: 15830 RVA: 0x0010FBE0 File Offset: 0x0010DDE0
	public event Action PaneClosedAnimEnd;

	// Token: 0x140000DA RID: 218
	// (add) Token: 0x06003DD7 RID: 15831 RVA: 0x0010FC18 File Offset: 0x0010DE18
	// (remove) Token: 0x06003DD8 RID: 15832 RVA: 0x0010FC50 File Offset: 0x0010DE50
	public event Action PaneOpenedAnimEnd;

	// Token: 0x17000718 RID: 1816
	// (get) Token: 0x06003DD9 RID: 15833 RVA: 0x0010FC85 File Offset: 0x0010DE85
	// (set) Token: 0x06003DDA RID: 15834 RVA: 0x0010FC8D File Offset: 0x0010DE8D
	public bool SkipInputEnable { get; set; }

	// Token: 0x06003DDB RID: 15835 RVA: 0x0010FC96 File Offset: 0x0010DE96
	private void Awake()
	{
		if (!this.paneAnimator)
		{
			this.paneAnimator = base.GetComponent<Animator>();
		}
		this.paneInput = base.GetComponent<InventoryPaneInput>();
	}

	// Token: 0x06003DDC RID: 15836 RVA: 0x0010FCC0 File Offset: 0x0010DEC0
	private void OnEnable()
	{
		NestedFadeGroupBase component = base.GetComponent<NestedFadeGroupBase>();
		if (component)
		{
			component.AlphaSelf = 0f;
		}
	}

	// Token: 0x06003DDD RID: 15837 RVA: 0x0010FCE7 File Offset: 0x0010DEE7
	private void Start()
	{
		if (this.paneAnimator)
		{
			this.paneAnimator.Update(0f);
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003DDE RID: 15838 RVA: 0x0010FD12 File Offset: 0x0010DF12
	[ContextMenu("Pane Start", true)]
	[ContextMenu("Pane End", true)]
	private bool IsPlayMode()
	{
		return Application.isPlaying;
	}

	// Token: 0x06003DDF RID: 15839 RVA: 0x0010FD1C File Offset: 0x0010DF1C
	[ContextMenu("Pane Start")]
	public override void PaneStart()
	{
		this.StopCloseAnim();
		base.gameObject.SetActive(true);
		base.PaneStart();
		PlayMakerFSM playMakerFSM = PlayMakerFSM.FindFsmOnGameObject(base.gameObject, "Inventory Proxy");
		if (playMakerFSM)
		{
			FsmBool fsmBool = playMakerFSM.FsmVariables.FindFsmBool("Only Pane");
			if (fsmBool != null)
			{
				fsmBool.Value = true;
			}
		}
		FSMUtility.SendEventToGameObject(base.gameObject, "UI ACTIVE", false);
		FSMUtility.SendEventToGameObject(base.gameObject, "ACTIVATE", false);
		if (!this.SkipInputEnable && this.paneInput)
		{
			this.paneInput.enabled = true;
		}
		if (this.paneAnimator)
		{
			this.paneAnimator.Update(0f);
		}
		this.StopOpenAnim();
		this.onOpenAnimEnd = delegate()
		{
			if (this.PaneOpenedAnimEnd != null)
			{
				this.PaneOpenedAnimEnd();
			}
		};
		this.openAnimRoutine = base.StartCoroutine(this.OpenAnimation());
	}

	// Token: 0x06003DE0 RID: 15840 RVA: 0x0010FE00 File Offset: 0x0010E000
	[ContextMenu("Pane End")]
	public override void PaneEnd()
	{
		this.StopCloseAnim();
		if (this.paneInput)
		{
			this.paneInput.enabled = false;
		}
		this.onCloseAnimEnd = delegate()
		{
			base.PaneEnd();
			FSMUtility.SendEventToGameObject(base.gameObject, "UI INACTIVE", false);
			base.gameObject.SetActive(false);
			if (this.PaneClosedAnimEnd != null)
			{
				this.PaneClosedAnimEnd();
			}
		};
		this.closeAnimRoutine = base.StartCoroutine(this.CloseAnimation());
	}

	// Token: 0x06003DE1 RID: 15841 RVA: 0x0010FE50 File Offset: 0x0010E050
	private void StopOpenAnim()
	{
		if (this.openAnimRoutine != null)
		{
			base.StopCoroutine(this.openAnimRoutine);
			if (this.onOpenAnimEnd != null)
			{
				this.onOpenAnimEnd();
			}
		}
	}

	// Token: 0x06003DE2 RID: 15842 RVA: 0x0010FE79 File Offset: 0x0010E079
	private IEnumerator OpenAnimation()
	{
		this.openSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		if (this.paneAnimator)
		{
			this.paneAnimator.Play(this.openAnim);
			yield return null;
			yield return new WaitForSeconds(this.paneAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		this.closeAnimRoutine = null;
		this.onOpenAnimEnd();
		this.onOpenAnimEnd = null;
		yield break;
	}

	// Token: 0x06003DE3 RID: 15843 RVA: 0x0010FE88 File Offset: 0x0010E088
	private void StopCloseAnim()
	{
		if (this.closeAnimRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.closeAnimRoutine);
		this.closeAnimRoutine = null;
		Action action = this.onCloseAnimEnd;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06003DE4 RID: 15844 RVA: 0x0010FEB6 File Offset: 0x0010E0B6
	private IEnumerator CloseAnimation()
	{
		this.closeSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		if (this.paneAnimator)
		{
			this.paneAnimator.Play(this.closeAnim);
			yield return null;
			yield return new WaitForSeconds(this.paneAnimator.GetCurrentAnimatorStateInfo(0).length);
		}
		this.closeAnimRoutine = null;
		this.onCloseAnimEnd();
		this.onCloseAnimEnd = null;
		yield break;
	}

	// Token: 0x04003F79 RID: 16249
	[SerializeField]
	private string openAnim = "Open";

	// Token: 0x04003F7A RID: 16250
	[SerializeField]
	private string closeAnim = "Close";

	// Token: 0x04003F7B RID: 16251
	[SerializeField]
	private Animator paneAnimator;

	// Token: 0x04003F7C RID: 16252
	[Space]
	[SerializeField]
	private AudioEvent openSound;

	// Token: 0x04003F7D RID: 16253
	[SerializeField]
	private AudioEvent closeSound;

	// Token: 0x04003F7E RID: 16254
	private Action onOpenAnimEnd;

	// Token: 0x04003F7F RID: 16255
	private Coroutine openAnimRoutine;

	// Token: 0x04003F80 RID: 16256
	private Action onCloseAnimEnd;

	// Token: 0x04003F81 RID: 16257
	private Coroutine closeAnimRoutine;

	// Token: 0x04003F82 RID: 16258
	private InventoryPaneInput paneInput;

	// Token: 0x04003F83 RID: 16259
	private bool hasStarted;
}
