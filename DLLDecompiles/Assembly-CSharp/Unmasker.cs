using System;
using UnityEngine;

// Token: 0x02000651 RID: 1617
public class Unmasker : MaskerBase
{
	// Token: 0x060039EC RID: 14828 RVA: 0x000FDF77 File Offset: 0x000FC177
	private void Reset()
	{
		this.persistent = base.GetComponent<PersistentBoolItem>();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060039ED RID: 14829 RVA: 0x000FDF91 File Offset: 0x000FC191
	private new void Awake()
	{
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isUncovered;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isUncovered = value;
				if (this.isUncovered)
				{
					base.AlphaSelf = 0f;
				}
			};
		}
	}

	// Token: 0x060039EE RID: 14830 RVA: 0x000FDFCE File Offset: 0x000FC1CE
	protected override void OnEnable()
	{
		base.OnEnable();
		base.AlphaSelf = 1f;
	}

	// Token: 0x060039EF RID: 14831 RVA: 0x000FDFE1 File Offset: 0x000FC1E1
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.tag != "Player")
		{
			return;
		}
		this.Uncover();
	}

	// Token: 0x060039F0 RID: 14832 RVA: 0x000FDFFC File Offset: 0x000FC1FC
	public void Uncover()
	{
		if (this.isUncovered)
		{
			return;
		}
		this.isUncovered = true;
		if (this.playSound)
		{
			EventRegister.SendEvent("SECRET TONE", null);
		}
		base.FadeTo(0f, this.fadeTime, null, false, null);
	}

	// Token: 0x04003C8F RID: 15503
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04003C90 RID: 15504
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04003C91 RID: 15505
	private bool isUncovered;

	// Token: 0x04003C92 RID: 15506
	[Space]
	[SerializeField]
	private float fadeTime = 0.5f;

	// Token: 0x04003C93 RID: 15507
	[SerializeField]
	private bool playSound = true;
}
