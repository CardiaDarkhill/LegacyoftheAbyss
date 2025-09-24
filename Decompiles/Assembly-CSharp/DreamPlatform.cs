using System;
using UnityEngine;

// Token: 0x020004D2 RID: 1234
public class DreamPlatform : MonoBehaviour
{
	// Token: 0x06002C63 RID: 11363 RVA: 0x000C26BA File Offset: 0x000C08BA
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x06002C64 RID: 11364 RVA: 0x000C26C8 File Offset: 0x000C08C8
	private void Start()
	{
		if (this.showOnEnable)
		{
			return;
		}
		if (this.outerCollider)
		{
			this.outerCollider.OnTriggerExited += delegate(Collider2D collider, GameObject sender)
			{
				this.Hide();
			};
		}
		if (this.innerCollider)
		{
			this.innerCollider.OnTriggerEntered += delegate(Collider2D collider, GameObject sender)
			{
				this.Show();
			};
		}
	}

	// Token: 0x06002C65 RID: 11365 RVA: 0x000C2726 File Offset: 0x000C0926
	private void OnEnable()
	{
		if (this.showOnEnable)
		{
			this.Show();
		}
	}

	// Token: 0x06002C66 RID: 11366 RVA: 0x000C2736 File Offset: 0x000C0936
	public void Show()
	{
		if (!this.visible)
		{
			this.PlayAnimation("Show");
			this.activateSound.PlayOnSource(this.audioSource, 0.85f, 1.15f);
			this.visible = true;
		}
	}

	// Token: 0x06002C67 RID: 11367 RVA: 0x000C276D File Offset: 0x000C096D
	public void Hide()
	{
		if (this.visible)
		{
			this.PlayAnimation("Hide");
			this.deactivateSound.PlayOnSource(this.audioSource, 0.85f, 1.15f);
			this.visible = false;
		}
	}

	// Token: 0x06002C68 RID: 11368 RVA: 0x000C27A4 File Offset: 0x000C09A4
	private void PlayAnimation(string animationName)
	{
		if (this.animator)
		{
			this.animator.Play(animationName);
		}
	}

	// Token: 0x04002DFD RID: 11773
	public TriggerEnterEvent outerCollider;

	// Token: 0x04002DFE RID: 11774
	public TriggerEnterEvent innerCollider;

	// Token: 0x04002DFF RID: 11775
	public Animator animator;

	// Token: 0x04002E00 RID: 11776
	public AudioClip activateSound;

	// Token: 0x04002E01 RID: 11777
	public AudioClip deactivateSound;

	// Token: 0x04002E02 RID: 11778
	private bool visible;

	// Token: 0x04002E03 RID: 11779
	public bool showOnEnable;

	// Token: 0x04002E04 RID: 11780
	private AudioSource audioSource;
}
