using System;
using UnityEngine;

// Token: 0x02000691 RID: 1681
public class InventoryItemIconAnimator : CustomInventoryItemCollectableDisplay
{
	// Token: 0x06003C04 RID: 15364 RVA: 0x00108478 File Offset: 0x00106678
	public override void OnSelect()
	{
		this.animator.SetBool(InventoryItemIconAnimator._isSelectedProp, true);
		if (this.selectedAudio)
		{
			this.selectedAudio.Play();
			this.selectedAudio.timeSamples = Random.Range(0, this.selectedAudio.clip.samples);
		}
		EventRegister.SendEvent(this.selectedEvent, null);
	}

	// Token: 0x06003C05 RID: 15365 RVA: 0x001084DB File Offset: 0x001066DB
	public override void OnDeselect()
	{
		this.animator.SetBool(InventoryItemIconAnimator._isSelectedProp, false);
		if (this.selectedAudio)
		{
			this.selectedAudio.Stop();
		}
		EventRegister.SendEvent(this.deselectedEvent, null);
	}

	// Token: 0x06003C06 RID: 15366 RVA: 0x00108512 File Offset: 0x00106712
	public override void OnPrePaneEnd()
	{
		this.OnDeselect();
	}

	// Token: 0x04003E24 RID: 15908
	private static readonly int _isSelectedProp = Animator.StringToHash("Is Selected");

	// Token: 0x04003E25 RID: 15909
	[SerializeField]
	private Animator animator;

	// Token: 0x04003E26 RID: 15910
	[SerializeField]
	private AudioSource selectedAudio;

	// Token: 0x04003E27 RID: 15911
	[SerializeField]
	private string selectedEvent;

	// Token: 0x04003E28 RID: 15912
	[SerializeField]
	private string deselectedEvent;
}
