using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200034F RID: 847
public class FSMActivator : MonoBehaviour
{
	// Token: 0x06001D53 RID: 7507 RVA: 0x00087702 File Offset: 0x00085902
	private void Awake()
	{
		this.fsms = base.GetComponents<PlayMakerFSM>();
		this.spriteAnim = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06001D54 RID: 7508 RVA: 0x0008771C File Offset: 0x0008591C
	public void Activate()
	{
		if (this.activateStaggered)
		{
			base.StartCoroutine(this.ActivateStaggered());
			return;
		}
		if (!this.activated)
		{
			if (this.fsms.Length != 0)
			{
				for (int i = 0; i < this.fsms.Length; i++)
				{
					this.fsms[i].enabled = true;
				}
			}
			if (this.spriteAnim != null)
			{
				this.spriteAnim.Play();
			}
			this.activated = true;
		}
	}

	// Token: 0x06001D55 RID: 7509 RVA: 0x00087791 File Offset: 0x00085991
	public IEnumerator ActivateStaggered()
	{
		if (!this.activated)
		{
			if (this.fsms.Length != 0)
			{
				this.activated = true;
				int num;
				for (int i = 0; i < this.fsms.Length; i = num + 1)
				{
					this.fsms[i].enabled = true;
					yield return null;
					num = i;
				}
			}
			if (this.spriteAnim != null)
			{
				this.activated = true;
				this.spriteAnim.Play();
			}
		}
		yield break;
	}

	// Token: 0x06001D56 RID: 7510 RVA: 0x000877A0 File Offset: 0x000859A0
	public void Deactivate()
	{
		if (this.activated && this.fsms.Length != 0)
		{
			for (int i = 0; i < this.fsms.Length; i++)
			{
				this.fsms[i].enabled = false;
			}
		}
	}

	// Token: 0x06001D57 RID: 7511 RVA: 0x000877DF File Offset: 0x000859DF
	public IEnumerator DeactivateStaggered()
	{
		if (this.activated && this.fsms.Length != 0)
		{
			int num;
			for (int i = 0; i < this.fsms.Length; i = num + 1)
			{
				this.fsms[i].enabled = false;
				yield return null;
				num = i;
			}
		}
		yield break;
	}

	// Token: 0x04001C8A RID: 7306
	[HideInInspector]
	public bool activateStaggered = true;

	// Token: 0x04001C8B RID: 7307
	private PlayMakerFSM[] fsms;

	// Token: 0x04001C8C RID: 7308
	private tk2dSpriteAnimator spriteAnim;

	// Token: 0x04001C8D RID: 7309
	private bool activated;
}
