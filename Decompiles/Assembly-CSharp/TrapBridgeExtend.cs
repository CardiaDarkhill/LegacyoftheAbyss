using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200057C RID: 1404
public class TrapBridgeExtend : TrapBridge
{
	// Token: 0x0600323F RID: 12863 RVA: 0x000E0094 File Offset: 0x000DE294
	private void Awake()
	{
		this.SetLeversUnlocked(false);
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.activated;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				bool flag = this.activated;
				this.activated = value;
				if (!flag && this.activated)
				{
					this.SetLeversUnlocked(true);
				}
			};
		}
		this.onOpened.AddListener(new UnityAction(this.UnlockLevers));
	}

	// Token: 0x06003240 RID: 12864 RVA: 0x000E00FA File Offset: 0x000DE2FA
	protected override IEnumerator DoOpenAnim()
	{
		this.OnExtend.Invoke();
		this.animator.Play(TrapBridgeExtend._openAnim);
		yield return null;
		yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		yield break;
	}

	// Token: 0x06003241 RID: 12865 RVA: 0x000E0109 File Offset: 0x000DE309
	protected override IEnumerator DoCloseAnim()
	{
		this.OnRetract.Invoke();
		this.animator.Play(TrapBridgeExtend._closeAnim);
		yield return null;
		yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		yield break;
	}

	// Token: 0x06003242 RID: 12866 RVA: 0x000E0118 File Offset: 0x000DE318
	private void UnlockLevers()
	{
		if (this.activated)
		{
			return;
		}
		this.activated = true;
		foreach (TrapBridgeExtend.LockedLeverGroup lever in this.lockedLevers)
		{
			base.StartCoroutine(this.UnlockLever(lever));
		}
	}

	// Token: 0x06003243 RID: 12867 RVA: 0x000E0160 File Offset: 0x000DE360
	private void SetLeversUnlocked(bool unlock)
	{
		foreach (TrapBridgeExtend.LockedLeverGroup lockedLeverGroup in this.lockedLevers)
		{
			if (lockedLeverGroup.Lever)
			{
				lockedLeverGroup.Lever.SetActive(unlock);
			}
			if (lockedLeverGroup.LockedLever)
			{
				lockedLeverGroup.LockedLever.gameObject.SetActive(!unlock);
			}
		}
	}

	// Token: 0x06003244 RID: 12868 RVA: 0x000E01C4 File Offset: 0x000DE3C4
	private IEnumerator UnlockLever(TrapBridgeExtend.LockedLeverGroup lever)
	{
		if (lever.UnlockDetector)
		{
			while (!lever.UnlockDetector.IsInside)
			{
				yield return null;
			}
		}
		if (lever.LockedLever)
		{
			lever.LockedLever.Play(TrapBridgeExtend._unlockAnim);
			yield return null;
			yield return new WaitForSeconds(lever.LockedLever.GetCurrentAnimatorStateInfo(0).length);
			lever.LockedLever.gameObject.SetActive(false);
		}
		if (lever.Lever)
		{
			lever.Lever.SetActive(true);
		}
		yield break;
	}

	// Token: 0x040035EF RID: 13807
	[Header("Extender")]
	[SerializeField]
	private Animator animator;

	// Token: 0x040035F0 RID: 13808
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x040035F1 RID: 13809
	[SerializeField]
	private TrapBridgeExtend.LockedLeverGroup[] lockedLevers;

	// Token: 0x040035F2 RID: 13810
	[Space]
	public UnityEvent OnExtend;

	// Token: 0x040035F3 RID: 13811
	public UnityEvent OnRetract;

	// Token: 0x040035F4 RID: 13812
	private bool activated;

	// Token: 0x040035F5 RID: 13813
	private static readonly int _closeAnim = Animator.StringToHash("Close");

	// Token: 0x040035F6 RID: 13814
	private static readonly int _openAnim = Animator.StringToHash("Open");

	// Token: 0x040035F7 RID: 13815
	private static readonly int _unlockAnim = Animator.StringToHash("Unlock");

	// Token: 0x02001889 RID: 6281
	[Serializable]
	private struct LockedLeverGroup
	{
		// Token: 0x04009269 RID: 37481
		public GameObject Lever;

		// Token: 0x0400926A RID: 37482
		public Animator LockedLever;

		// Token: 0x0400926B RID: 37483
		public TrackTriggerObjects UnlockDetector;
	}
}
