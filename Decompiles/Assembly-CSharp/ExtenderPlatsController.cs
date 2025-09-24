using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004DD RID: 1245
public class ExtenderPlatsController : UnlockablePropBase
{
	// Token: 0x06002CB7 RID: 11447 RVA: 0x000C39D0 File Offset: 0x000C1BD0
	private void Awake()
	{
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isUnfolded;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isUnfolded = value;
				if (this.isUnfolded)
				{
					this.SetUnfoldedInstant();
				}
			};
		}
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x000C3A0D File Offset: 0x000C1C0D
	private void Start()
	{
		if (this.isUnfolded)
		{
			return;
		}
		this.ResetChain();
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x000C3A20 File Offset: 0x000C1C20
	[ContextMenu("Reset")]
	private void ResetChain()
	{
		foreach (ExtenderPlatLink extenderPlatLink in this.links)
		{
			extenderPlatLink.UpdateLinkRotation(0f);
			extenderPlatLink.UpdatePlatRotation(0f);
			extenderPlatLink.SetActive(false, true);
		}
		this.isUnfolded = false;
		this.OnSetActive.Invoke(false);
	}

	// Token: 0x06002CBA RID: 11450 RVA: 0x000C3A75 File Offset: 0x000C1C75
	[ContextMenu("Reset", true)]
	[ContextMenu("Unfold", true)]
	private bool CanUnfold()
	{
		return Application.isPlaying;
	}

	// Token: 0x06002CBB RID: 11451 RVA: 0x000C3A7C File Offset: 0x000C1C7C
	[ContextMenu("Unfold")]
	public void Unfold()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (this.isUnfolded)
		{
			return;
		}
		this.isUnfolded = true;
		this.OnSetActive.Invoke(true);
		base.StartCoroutine(this.UnfoldChain());
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x000C3AB0 File Offset: 0x000C1CB0
	public void SetUnfoldedInstant()
	{
		if (this.isUnfolded)
		{
			return;
		}
		this.isUnfolded = true;
		this.OnSetActive.Invoke(true);
		foreach (ExtenderPlatLink extenderPlatLink in this.links)
		{
			extenderPlatLink.UpdateLinkRotation(1f);
			extenderPlatLink.UpdatePlatRotation(1f);
			extenderPlatLink.SetActive(true, true);
		}
		foreach (UnlockablePropBase unlockablePropBase in this.passUnlock)
		{
			if (unlockablePropBase)
			{
				unlockablePropBase.Opened();
			}
		}
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x000C3B35 File Offset: 0x000C1D35
	private IEnumerator UnfoldChain()
	{
		yield return new WaitForSeconds(this.linkUnfoldStartDelay);
		WaitForSeconds linkUnfoldWait = new WaitForSeconds(this.linkUnfoldDelay);
		int lastLink = this.links.Length - 1;
		int j;
		for (int i = 0; i < this.links.Length; i = j + 1)
		{
			ExtenderPlatLink extenderPlatLink = this.links[i];
			if (extenderPlatLink.gameObject.activeSelf)
			{
				extenderPlatLink.LinkRotationStarted();
				base.StartCoroutine(this.LinkTimer(this.linkUnfoldDuration, new Action<float>(extenderPlatLink.UpdateLinkRotation), null));
				if (i != lastLink)
				{
					yield return linkUnfoldWait;
				}
			}
			j = i;
		}
		yield return new WaitForSeconds(this.platUnfoldStartDelay);
		WaitForSeconds platUnfoldWait = new WaitForSeconds(this.platUnfoldDelay);
		for (int i = 0; i < this.links.Length; i = j + 1)
		{
			ExtenderPlatLink link = this.links[i];
			if (link.IsPlatformActive)
			{
				link.PlatRotationStarted();
				base.StartCoroutine(this.LinkTimer(this.platUnfoldDuration, new Action<float>(link.UpdatePlatRotation), delegate
				{
					link.SetActive(true, false);
				}));
			}
			if (i != lastLink)
			{
				yield return platUnfoldWait;
			}
			j = i;
		}
		foreach (UnlockablePropBase unlockablePropBase in this.passUnlock)
		{
			if (unlockablePropBase)
			{
				unlockablePropBase.Open();
			}
		}
		yield break;
	}

	// Token: 0x06002CBE RID: 11454 RVA: 0x000C3B44 File Offset: 0x000C1D44
	private IEnumerator LinkTimer(float duration, Action<float> handler, Action onEnd)
	{
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			float obj = elapsed / duration;
			handler(obj);
			yield return null;
		}
		handler(1f);
		if (onEnd != null)
		{
			onEnd();
		}
		yield break;
	}

	// Token: 0x06002CBF RID: 11455 RVA: 0x000C3B61 File Offset: 0x000C1D61
	public override void Open()
	{
		this.audioStart.Play();
		this.Unfold();
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x000C3B74 File Offset: 0x000C1D74
	public override void Opened()
	{
		this.SetUnfoldedInstant();
	}

	// Token: 0x04002E52 RID: 11858
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04002E53 RID: 11859
	[SerializeField]
	private ExtenderPlatLink[] links;

	// Token: 0x04002E54 RID: 11860
	[SerializeField]
	private float linkUnfoldStartDelay;

	// Token: 0x04002E55 RID: 11861
	[SerializeField]
	private float linkUnfoldDelay;

	// Token: 0x04002E56 RID: 11862
	[SerializeField]
	private float linkUnfoldDuration;

	// Token: 0x04002E57 RID: 11863
	[SerializeField]
	private float platUnfoldStartDelay;

	// Token: 0x04002E58 RID: 11864
	[SerializeField]
	private float platUnfoldDelay;

	// Token: 0x04002E59 RID: 11865
	[SerializeField]
	private float platUnfoldDuration;

	// Token: 0x04002E5A RID: 11866
	[SerializeField]
	private AudioSource audioStart;

	// Token: 0x04002E5B RID: 11867
	[Space]
	public ExtenderPlatsController.UnityBoolEvent OnSetActive;

	// Token: 0x04002E5C RID: 11868
	[Space]
	[SerializeField]
	private UnlockablePropBase[] passUnlock;

	// Token: 0x04002E5D RID: 11869
	private bool isUnfolded;

	// Token: 0x020017E4 RID: 6116
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}
}
